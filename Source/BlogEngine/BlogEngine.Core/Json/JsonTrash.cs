namespace BlogEngine.Core.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// JSON trash object
    /// </summary>
    public class JsonTrash
    {
        /// <summary>
        ///     Deleted item ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Deleted item title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Type of deleted object
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        ///     Gets or sets the date portion of published date
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        ///     Gets or sets the time portion of published date
        /// </summary>
        public string Time { get; set; }
    }
}
