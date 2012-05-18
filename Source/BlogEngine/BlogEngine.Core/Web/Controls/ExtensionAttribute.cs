namespace BlogEngine.Core.Web.Controls
{
    using System;

    /// <summary>
    /// All extensions must decorate the class with this attribute.
    ///     It is used for reflection.
    ///     <remarks>
    /// When using this attribute, you must make sure
    ///         to have a default constructor. It will be used to create
    ///         an instance of the extension through reflection.
    ///     </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExtensionAttribute : Attribute
    {
        #region Constants and Fields

        /// <summary>
        /// The author.
        /// </summary>
        private readonly string author;

        /// <summary>
        /// The description.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// The version.
        /// </summary>
        private readonly string version;

        /// <summary>
        /// The priority.
        /// </summary>
        private readonly int priority;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionAttribute"/> class. 
        /// Creates an instance of the attribute and assigns a description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="author">
        /// The author.
        /// </param>
        public ExtensionAttribute(string description, string version, string author)
        {
            this.description = description;
            this.version = version;
            this.author = author;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionAttribute"/> class. 
        /// Creates an instance of the attribute and assigns a description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="author">
        /// The author.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        public ExtensionAttribute(string description, string version, string author, int priority)
        {
            this.description = description;
            this.version = version;
            this.author = author;
            this.priority = priority;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the author of the extension
        /// </summary>
        /// <value>The author.</value>
        public string Author
        {
            get
            {
                return this.author;
            }
        }

        /// <summary>
        /// Gets the description of the extension.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets the priority of the extension
        /// This determins in what order extensions instantiated
        /// and in what order they will respond to events
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get
            {
                return this.priority;
            }
        }

        /// <summary>
        ///     Gets the version number of the extension
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }
        }

        #endregion
    }

    /// <summary>
    /// Helper class for sorting extensions by priority
    /// </summary>
    public class SortedExtension
    {
        #region Constants and Fields

        /// <summary>
        ///     Gets or sets the name of the extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the order in which extensions are sorted and respond to events
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets the type of the extension
        /// </summary>
        public string Type { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedExtension"/> class.
        /// </summary>
        /// <param name="priority">The extension priority.</param>
        /// <param name="name">The extension name.</param>
        /// <param name="type">The extension type.</param>
        public SortedExtension(int priority, string name, string type)
        {
            this.Priority = priority;
            this.Name = name;
            this.Type = type;
        }

        #endregion
    }
}