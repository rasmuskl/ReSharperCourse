namespace BlogEngine.Core.Json
{
    using System;

    /// <summary>
    /// Json friendly Page wrapper
    /// </summary>
    public class JsonPage
    {
        /// <summary>
        /// Page ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Page title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Page author
        /// </summary>
        public bool ShowInList { get; set; }

        /// <summary>
        /// If page is published
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        ///     Gets or sets the date portion of published date
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        ///     Gets or sets the time portion of published date
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Parent page Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Parent page title
        /// </summary>
        public string ParentTitle { get; set; }

        /// <summary>
        /// If has child pages
        /// </summary>
        public bool HasChildren { get; set; }

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
