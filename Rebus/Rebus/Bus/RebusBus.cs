﻿using System;
using System.Collections.Generic;
using System.Threading;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Messages;
using System.Linq;
using Rebus.Shared;
using Rebus.Extensions;

namespace Rebus.Bus
{
    /// <summary>
    /// Implements <see cref="IBus"/> as Rebus would do it.
    /// </summary>
    public class RebusBus : IStartableBus, IBus
    {
        static ILog log;

        static RebusBus()
        {
            RebusLoggerFactory.Changed += f => log = f.GetCurrentClassLogger();
        }

        /// <summary>
        /// Event that will be raised immediately after receiving a transport 
        /// message, before any other actions are executed.
        /// </summary>
        public event Action BeforeMessage = delegate { };

        /// <summary>
        /// Event that will be raised after a transport message has been handled.
        /// If an error occurs, the caught exception will be passed to the
        /// listeners. If no errors occur, the passed exception will be null.
        /// </summary>
        public event Action<Exception> AfterMessage = delegate { };

        /// <summary>
        /// Event that will be raised whenever it is determined that a message
        /// has failed too many times.
        /// </summary>
        public event Action PoisonMessage = delegate { };

        readonly ISendMessages sendMessages;
        readonly IReceiveMessages receiveMessages;
        readonly IStoreSubscriptions storeSubscriptions;
        readonly IDetermineDestination determineDestination;
        readonly IActivateHandlers activateHandlers;
        readonly ISerializeMessages serializeMessages;
        readonly IStoreSagaData storeSagaData;
        readonly IInspectHandlerPipeline inspectHandlerPipeline;
        readonly List<Worker> workers = new List<Worker>();
        readonly ErrorTracker errorTracker = new ErrorTracker();
        readonly HeaderContext headerContext = new HeaderContext();

        static int rebusIdCounter;
        readonly int rebusId;

        /// <summary>
        /// Constructs the bus with the specified ways of achieving its goals.
        /// </summary>
        /// <param name="activateHandlers">The bus will use this to construct handlers for received messages.</param>
        /// <param name="sendMessages">Will be used to send transport messages when you send, publish, and reply.</param>
        /// <param name="receiveMessages">Will be used to receive transport messages. If the bus is configured to run with multiple threads, this one should be reentrant.</param>
        /// <param name="storeSubscriptions">Will be used to store subscription information. Is only relevant if the bus is a publisher, i.e. it publishes messages and other services assume they can subscribe to its messages.</param>
        /// <param name="storeSagaData">Will be used to store saga data. Is only relevant if one or more handlers are derived from <see cref="Saga"/>.</param>
        /// <param name="determineDestination">Will be used to resolve a destination in cases where the message destination is not explicitly specified as part of a send/subscribe operation.</param>
        /// <param name="serializeMessages">Will be used to serialize and deserialize transport messages.</param>
        /// <param name="inspectHandlerPipeline">Will be called to inspect the pipeline of handlers constructed to handle an incoming message.</param>
        public RebusBus(IActivateHandlers activateHandlers, ISendMessages sendMessages, IReceiveMessages receiveMessages, IStoreSubscriptions storeSubscriptions, IStoreSagaData storeSagaData, IDetermineDestination determineDestination, ISerializeMessages serializeMessages, IInspectHandlerPipeline inspectHandlerPipeline)
        {
            this.activateHandlers = activateHandlers;
            this.sendMessages = sendMessages;
            this.receiveMessages = receiveMessages;
            this.storeSubscriptions = storeSubscriptions;
            this.determineDestination = determineDestination;
            this.serializeMessages = serializeMessages;
            this.storeSagaData = storeSagaData;
            this.inspectHandlerPipeline = inspectHandlerPipeline;

            rebusId = Interlocked.Increment(ref rebusIdCounter);

            log.Info("Rebus bus created");
        }

        public IBus Start()
        {
            const int defaultNumberOfWorkers = 1;

            var numberOfWorkers = RebusConfigurationSection
                .GetConfigurationValueOrDefault(s => s.Workers, defaultNumberOfWorkers)
                .GetValueOrDefault(defaultNumberOfWorkers);

            return Start(numberOfWorkers);
        }

        public RebusBus Start(int numberOfWorkers)
        {
            log.Info("Initializing bus with {0} workers", numberOfWorkers);
            SetNumberOfWorkers(numberOfWorkers);
            log.Info("Bus started");
            return this;
        }

        public void Send<TCommand>(TCommand message)
        {
            var destinationEndpoint = GetMessageOwnerEndpointFor(message.GetType());

            InternalSend(destinationEndpoint, new List<object> { message });
        }

        public void Send<TCommand>(string endpoint, TCommand message)
        {
            InternalSend(endpoint, new List<object> { message });
        }

        public void SendLocal<TCommand>(TCommand message)
        {
            var destinationEndpoint = receiveMessages.InputQueue;

            InternalSend(destinationEndpoint, new List<object> { message });
        }

        internal void SendBatch(params object[] messages)
        {
            var groupedByEndpoints = GetMessagesGroupedByEndpoints(messages);

            foreach (var batch in groupedByEndpoints)
            {
                InternalSend(batch.Key, batch.Value);
            }
        }

        public void Publish<TEvent>(TEvent message)
        {
            var subscriberEndpoints = storeSubscriptions.GetSubscribers(message.GetType());

            foreach (var subscriberInputQueue in subscriberEndpoints)
            {
                InternalSend(subscriberInputQueue, new List<object> { message });
            }
        }

        internal void PublishBatch(params object[] messages)
        {
            var groupedByEndpoints = GetMessagesGroupedBySubscriberEndpoints(messages);

            foreach (var batch in groupedByEndpoints)
            {
                InternalSend(batch.Key, batch.Value);
            }
        }

        public void Reply<TResponse>(TResponse message)
        {
            var returnAddress = MessageContext.GetCurrent().ReturnAddress;

            InternalSend(returnAddress, new List<object> { message });
        }

        public void Subscribe<TMessage>()
        {
            var publisherInputQueue = GetMessageOwnerEndpointFor(typeof(TMessage));

            InternalSubscribe<TMessage>(publisherInputQueue);
        }

        public void Subscribe<TMessage>(string publisherInputQueue)
        {
            InternalSubscribe<TMessage>(publisherInputQueue);
        }

        void InternalSubscribe<TMessage>(string publisherInputQueue)
        {
            var message = new SubscriptionMessage { Type = typeof(TMessage).AssemblyQualifiedName };

            InternalSend(publisherInputQueue, new List<object> { message });
        }

        /// <summary>
        /// Core send method. This should be the only place where calls to the bus'
        /// <see cref="ISendMessages"/> instance gets called, except for when moving
        /// messages to the error queue. This method will bundle the specified batch
        /// of messages inside one single transport message, which it will send.
        /// </summary>
        void InternalSend(string endpoint, List<object> messages)
        {
            var messageToSend = new Message{Messages = messages.ToArray(),};
            var headers = MergeHeaders(messageToSend);
            if (!headers.ContainsKey(Headers.ReturnAddress))
            {
                headers[Headers.ReturnAddress] = receiveMessages.InputQueue;
            }
            messageToSend.Headers = headers;

            log.Info("Sending {0} to {1}", string.Join("+", messageToSend.Messages), endpoint);
            var transportMessage = serializeMessages.Serialize(messageToSend);

            sendMessages.Send(endpoint, transportMessage);
        }

        IEnumerable<KeyValuePair<string, List<object>>> GetMessagesGroupedBySubscriberEndpoints(object[] messages)
        {
            var dict = new Dictionary<string, List<object>>();
            var endpointsByType = messages.Select(m => m.GetType()).Distinct()
                .Select(t => new KeyValuePair<Type, string[]>(t, storeSubscriptions.GetSubscribers(t) ?? new string[0]))
                .ToDictionary(d => d.Key, d => d.Value);

            foreach (var message in messages)
            {
                var endpoints = endpointsByType[message.GetType()];
                foreach (var endpoint in endpoints)
                {
                    if (!dict.ContainsKey(endpoint))
                    {
                        dict[endpoint] = new List<object>();
                    }
                    dict[endpoint].Add(message);
                }
            }

            return dict;
        }

        IEnumerable<KeyValuePair<string, List<object>>> GetMessagesGroupedByEndpoints(object[] messages)
        {
            var dict = new Dictionary<string, List<object>>();
            var endpointsByType = messages.Select(m => m.GetType()).Distinct()
                .Select(t => new KeyValuePair<Type, string>(t, determineDestination.GetEndpointFor(t) ?? ""))
                .ToDictionary(d => d.Key, d => d.Value);

            foreach (var message in messages)
            {
                var endpoint = endpointsByType[message.GetType()];
                if (!dict.ContainsKey(endpoint))
                {
                    dict[endpoint] = new List<object>();
                }
                dict[endpoint].Add(message);
            }

            return dict;
        }

        IDictionary<string, string> MergeHeaders(Message messageToSend)
        {
            var transportMessageHeaders = messageToSend.Headers.Clone();

            var messages = messageToSend.Messages
                .Select(m => new Tuple<object, Dictionary<string, string>>(m, headerContext.GetHeadersFor(m)))
                .ToList();

            AssertTimeToBeReceivedIsNotInconsistent(messages);
            AssertReturnAddressIsNotInconsistent(messages);

            // stupid trivial merge of all headers - will not detect inconsistensies at this point,
            // and duplicated headers will be overwritten, so it's pretty important that silly
            // stuff has been prevented
            foreach (var header in messages.SelectMany(m => m.Item2))
            {
                transportMessageHeaders[header.Key] = header.Value;
            }

            return transportMessageHeaders;
        }

        void AssertReturnAddressIsNotInconsistent(List<Tuple<object, Dictionary<string, string>>> messages)
        {
            if (messages.Any(m => m.Item2.ContainsKey(Headers.ReturnAddress)))
            {
                var returnAddresses = messages.Select(m => m.Item2[Headers.ReturnAddress]).Distinct();
                if (returnAddresses.Count() > 1)
                {
                    throw new InconsistentReturnAddressException("These return addresses were specified: {0}", string.Join(", ", returnAddresses));
                }
            }
        }

        static void AssertTimeToBeReceivedIsNotInconsistent(List<Tuple<object, Dictionary<string, string>>> messages)
        {
            if (messages.Any(m => m.Item2.ContainsKey(Headers.TimeToBeReceived)))
            {
                // assert all messages have the header
                if (!(messages.All(m => m.Item2.ContainsKey(Headers.TimeToBeReceived))))
                {
                    throw new InconsistentTimeToBeReceivedException("Not all messages in the batch had an attached time to be received header!");
                }

                // assert all values are the same
                var timesToBeReceived = messages.Select(m => m.Item2[Headers.TimeToBeReceived]).Distinct();
                if (timesToBeReceived.Count() > 1)
                {
                    throw new InconsistentTimeToBeReceivedException("These times to be received were specified: {0}", string.Join(", ", timesToBeReceived));
                }
            }
        }

        void HandleMessageFailedMaxNumberOfTimes(ReceivedTransportMessage receivedTransportMessage, string errorDetail)
        {
            var transportMessageToSend = receivedTransportMessage.ToForwardableMessage();

            log.Warn("Message {0} is forwarded to error queue", transportMessageToSend.Label);

            transportMessageToSend.Headers[Headers.SourceQueue] = receiveMessages.InputQueue;
            transportMessageToSend.Headers[Headers.ErrorMessage] = errorDetail;

            try
            {
                sendMessages.Send(receiveMessages.ErrorQueue, transportMessageToSend);
            }
            catch (Exception e)
            {
                log.Error(e, "Wanted to move message with id {0} to the error queue, but an exception occurred!", receivedTransportMessage.Id);

                // what to do? we need to throw again, or the message will not be rolled back and will thus be lost
                // - but we want to avoid thrashing, so we just log the badness and relax a little bit - that's
                // probably the best we can do
                Thread.Sleep(300);

                throw;
            }
        }

        public void Dispose()
        {
            workers.ForEach(w => w.Stop());
            workers.ForEach(w => w.Dispose());
            headerContext.Dispose();
        }

        string GetMessageOwnerEndpointFor(Type messageType)
        {
            return determineDestination.GetEndpointFor(messageType);
        }

        void AddWorker()
        {
            lock (workers)
            {
                var worker = new Worker(errorTracker,
                                        receiveMessages,
                                        activateHandlers,
                                        storeSubscriptions,
                                        serializeMessages,
                                        storeSagaData,
                                        inspectHandlerPipeline,
                                        string.Format("Rebus {0} worker {1}", rebusId, workers.Count + 1),
                                        new DeferredMessageReDispatcher(this));
                workers.Add(worker);
                worker.MessageFailedMaxNumberOfTimes += HandleMessageFailedMaxNumberOfTimes;
                worker.UserException += LogUserException;
                worker.SystemException += LogSystemException;
                worker.BeforeMessage += RaiseBeforeMessage;
                worker.AfterMessage += RaiseAfterMessage;
                worker.PoisonMessage += RaisePosionMessage;
                worker.Start();
            }
        }

        void RaiseBeforeMessage()
        {
            BeforeMessage();
        }

        void RaiseAfterMessage(Exception exception)
        {
            AfterMessage(exception);
        }

        void RaisePosionMessage()
        {
            PoisonMessage();
        }

        void RemoveWorker()
        {
            lock (workers)
            {
                if (workers.Count == 0) return;
                var workerToRemove = workers.Last();

                workers.Remove(workerToRemove);

                try
                {
                    workerToRemove.Stop();
                }
                finally
                {
                    workerToRemove.MessageFailedMaxNumberOfTimes -= HandleMessageFailedMaxNumberOfTimes;
                    workerToRemove.UserException -= LogUserException;
                    workerToRemove.SystemException -= LogSystemException;
                    workerToRemove.Dispose();
                }
            }
        }

        void LogSystemException(Worker worker, Exception exception)
        {
            log.Error(exception, "Unhandled system exception in {0}", worker.WorkerThreadName);
        }

        void LogUserException(Worker worker, Exception exception)
        {
            log.Warn("User exception in {0}: {1}", worker.WorkerThreadName, exception);
        }

        public void SetNumberOfWorkers(int newNumberOfWorkers)
        {
            while (workers.Count < newNumberOfWorkers) AddWorker();
            while (workers.Count > newNumberOfWorkers) RemoveWorker();
        }

        public void Defer(TimeSpan delay, object message)
        {
            var customData = TimeoutReplyHandler.Serialize(message);

            var messages = new List<object>
                               {
                                   new TimeoutRequest
                                       {
                                           Timeout = delay,
                                           CustomData = customData,
                                           CorrelationId = TimeoutReplyHandler.TimeoutReplySecretCorrelationId
                                       }
                               };

            InternalSend("rebus.timeout", messages);
        }

        public void AttachHeader(object message, string key, string value)
        {
            headerContext.AttachHeader(message, key, value);
        }

        internal class HeaderContext
        {
            internal readonly List<Tuple<WeakReference, Dictionary<string, string>>> Headers = new List<Tuple<WeakReference, Dictionary<string, string>>>();

            internal readonly System.Timers.Timer CleanupTimer;

            public HeaderContext()
            {
                CleanupTimer = new System.Timers.Timer { Interval = 1000 };
                CleanupTimer.Elapsed += (o, ea) => Headers.RemoveDeadReferences();
            }

            public void AttachHeader(object message, string key, string value)
            {
                var headerDictionary = Headers.GetOrAdd(message, () => new Dictionary<string, string>());

                headerDictionary.Add(key, value);
            }

            public Dictionary<string, string> GetHeadersFor(object message)
            {
                Dictionary<string, string> temp;

                var headersForThisMessage = Headers.TryGetValue(message, out temp)
                                                ? temp
                                                : new Dictionary<string, string>();

                return headersForThisMessage;
            }

            public void Tick()
            {
                Headers.RemoveDeadReferences();
            }

            public void Dispose()
            {
                CleanupTimer.Dispose();
            }
        }
    }

    class DeferredMessageReDispatcher: IHandleDeferredMessage
    {
        readonly IBus bus;

        public DeferredMessageReDispatcher(IBus bus)
        {
            this.bus = bus;
        }

        public void Dispatch(object deferredMessage)
        {
            bus.SendLocal(deferredMessage);
        }
    }

    public static class HeaderContextExtensions
    {
        public static Dictionary<string,string> GetOrAdd(this List<Tuple<WeakReference, Dictionary<string,string>>> contexts, object key, Func<Dictionary<string, string>> factory)
        {
            var entry = contexts.FirstOrDefault(c => c.Item1.Target == key);
            if (entry == null)
            {
                lock(contexts)
                {
                    entry = contexts.FirstOrDefault(c => c.Item1.Target == key);

                    if (entry == null)
                    {
                        entry = new Tuple<WeakReference, Dictionary<string, string>>(new WeakReference(key), factory());
                        contexts.Add(entry);
                    }
                }
            }
            return entry.Item2;
        }

        public static void RemoveDeadReferences(this List<Tuple<WeakReference, Dictionary<string, string>>> contexts)
        {
            if (contexts.Any(c => !c.Item1.IsAlive))
            {
                lock (contexts)
                {
                    contexts.RemoveAll(c => !c.Item1.IsAlive);
                }
            }
        }

        public static bool TryGetValue(this List<Tuple<WeakReference, Dictionary<string,string>>> contexts, object key, out Dictionary<string, string> dictionery)
        {
            var entry = contexts.FirstOrDefault(c => c.Item1.Target == key);

            if (entry == null)
            {
                dictionery = new Dictionary<string, string>();
                return false;
            }
            
            dictionery = entry.Item2;
            return true;
        }
    }

    public static class MessageExtensions
    {
        //public static TMessage AttachHeader<TMessage>(this TMessage message, string key, string value)
        //{
        //    HeaderContext.Current.AttachHeader(message, key, value);
        //    return message;
        //}
    }
}