namespace BlogEngine.Core.Json
{
    using System;

    /// <summary>
    /// Json friendly Role wrapper
    /// </summary>
    public class JsonRole
    {
        /// <summary>
        /// Role Name
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Is System Role
        /// </summary>
        public bool IsSystemRole { get; set; }
    }
}
