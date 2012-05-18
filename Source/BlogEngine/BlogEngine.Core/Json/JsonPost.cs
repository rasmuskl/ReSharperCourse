

namespace BlogEngine.Core.Json
{
    using System;

    /// <summary>
    /// Wrapper aroung Post
    /// used to show post list in the admin
    /// </summary>
    public class JsonPost
    {
        /// <summary>
        /// Post ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Post title
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Post author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Gets or sets the date portion of published date
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        ///     Gets or sets the time portion of published date
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Comma separated list of post categories
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// Comma separated list of post tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Comment counts for the post
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets post status
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// If the current user can delete this page.
        /// </summary>
        public bool CanUserDelete { get; set; }

        /// <summary>
        /// If the current user can edit this page.
        /// </summary>
        public bool CanUserEdit { get; set; }
    }
}
