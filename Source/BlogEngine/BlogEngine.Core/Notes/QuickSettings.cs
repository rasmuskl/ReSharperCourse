using System;
using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core.Json;
using BlogEngine.Core.Providers;

namespace BlogEngine.Core.Notes
{
    /// <summary>
    /// Settings for quick notes
    /// </summary>
    public class QuickSettings
    {
        /// <summary>
        /// Quick settings
        /// </summary>
        /// <param name="user"></param>
        public QuickSettings(string user)
        {
            Settings = BlogService.FillQuickSettings(user);

            if (Settings == null)
                Settings = new List<QuickSetting>();
        }
        /// <summary>
        /// List of settings
        /// </summary>
        public List<QuickSetting> Settings { get; set; }

        /// <summary>
        /// List of categories
        /// </summary>
        public List<JsonCategory> Categories
        { 
            get 
            {
                var cats = new List<JsonCategory>();
                foreach (var c in Category.Categories)
                {
                    cats.Add(new JsonCategory { Id = c.Id, Title = c.Title });
                }
                return cats;
            } 
        }

        #region Methods

        /// <summary>
        /// Save to collection (add or replace)
        /// </summary>
        /// <param name="setting">Setting</param>
        public void Save(QuickSetting setting)
        {
            var idx = Settings.FindIndex(s => s.Author == setting.Author && s.SettingName == setting.SettingName);
            if(idx < 0)
            {
                Settings.Add(setting);
            }
            else
            {
                Settings[idx] = setting;
            }
        }

        #endregion
    }
}
