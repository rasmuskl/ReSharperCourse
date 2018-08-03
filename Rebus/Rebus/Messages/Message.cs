using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebus.Messages
{
    /// <summary>
    /// Message wrapper object that may contain a collection of headers and multiple logical messages.
    /// </summary>
    [Serializable]
    public class Message
    {
        public Message()
        {
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Headers of this message. May include metadata like e.g. the address of the sender.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Collection of logical messages that are contained within this transport message.
        /// </summary>
        public object[] Messages { get; set; }

        /// <summary>
        /// Gets the header with the specified key or null if the given key is not present.
        /// Lookup names of pre-defined keys via <see cref="Headers"/>.
        /// </summary>
        public string GetHeader(string key)
        {
            if (!Headers.ContainsKey(key))
                return null;
            
            return Headers[key];
        }

        /// <summary>
        /// Gets some kind of headline that somehow describes this message. May be used by the queue
        /// infrastructure to somehow label a message.
        /// </summary>
        public string GetLabel()
        {
            if (Messages == null || Messages.Length == 0)
                return "Empty Message";

            return string.Join(" + ", Messages.Select(m => m.ToString()));
        }
    }
}