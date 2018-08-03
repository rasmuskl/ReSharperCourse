using System;
using System.Messaging;

namespace Rebus.Shared
{
    static internal class MsmqUtil
    {
        public static void PurgeQueue(string queueName)
        {
            var path = GetPath(queueName);
            
            if (!MessageQueue.Exists(path)) return;

            using(var messageQueue = new MessageQueue(path))
            {
                messageQueue.Purge();
            }
        }

        public static string GetPath(string queueName)
        {
            var bim = Parse(queueName);

            return GenerateSimplePath(bim.MachineName ?? ".", bim.QueueName);
        }

        public static string GetFullPath(string queueName)
        {
            var bim = Parse(queueName);

            return GenerateFullPath(bim.MachineName ?? Environment.MachineName, bim.QueueName);
        }

        public static string GenerateFullPath(string machineName, string queueName)
        {
            return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", machineName.ToLower(), queueName);
        }

        static string GenerateSimplePath(string machineName, string queueName)
        {
            return string.Format(@"{0}\private$\{1}", machineName, queueName);
        }

        class QueueInfo
        {
            public string MachineName { get; set; }
            public string QueueName { get; set; }
        }

        static QueueInfo Parse(string queueName)
        {
            if (queueName.Contains("@"))
            {
                var tokens = queueName.Split('@');
                if (tokens.Length != 2)
                {
                    throw new ArgumentException(string.Format("The specified MSMQ input queue is invalid!: {0}", queueName));
                }
                return new QueueInfo {MachineName = tokens[0], QueueName = tokens[1]};
            }
            return new QueueInfo {QueueName = queueName};
        }
    }
}