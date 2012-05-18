namespace BlogEngine.Core.Json
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using Web.Extensions;

    ///<summary>
    /// Handle json-friendly list of custom filters
    ///</summary>
    public class JsonCustomFilterList
    {
        static protected ExtensionSettings CustomFilters;

        /// <summary>
        /// Get list of custom filters
        /// </summary>
        /// <returns>List of filters</returns>
        public static List<JsonCustomFilter> GetCustomFilters()
        {
            var filterList = new List<JsonCustomFilter>();
            try
            {
                CustomFilters = ExtensionManager.GetSettings("MetaExtension", "BeCustomFilters");
                DataTable dt = CustomFilters.GetDataTable();

                foreach (DataRow row in dt.Rows)
                {
                    var f = new JsonCustomFilter
                    {
                        Name = row["Name"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Checked = int.Parse(row["Checked"].ToString()),
                        Spam = int.Parse(row["Cought"].ToString()),
                        Mistakes = int.Parse(row["Reported"].ToString())
                    };

                    var ext = ExtensionManager.GetExtension(f.Name);
                    f.Enabled = ext == null ? true : ext.Enabled;

                    filterList.Add(f);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("JsonCustomFilterList.GetCustomFilters(): " + ex.Message);
            }
            return filterList;
        }
        
        /// <summary>
        /// Reset counters for custom filter
        /// </summary>
        /// <param name="filterName">Filter name</param>
        /// <returns>Json response</returns>
        public static JsonResponse ResetCounters(string filterName)
        {
            try
            {
                if (!string.IsNullOrEmpty(filterName))
                {
                    // reset statistics for this filter
                    for (int i = 0; i < CustomFilters.Parameters[0].Values.Count; i++)
                    {
                        if (CustomFilters.Parameters[1].Values[i] == filterName)
                        {
                            CustomFilters.Parameters[2].Values[i] = "0";
                            CustomFilters.Parameters[3].Values[i] = "0";
                            CustomFilters.Parameters[4].Values[i] = "0";
                        }
                    }
                    ExtensionManager.SaveSettings("MetaExtension", CustomFilters);
                }
                return new JsonResponse() { Success = true, Message = string.Format("Counters for {0} reset", filterName) };
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("JsonCustomFilterList.ResetCounters: {0}", ex.Message));
                return new JsonResponse() { Message = "Error resetting counters" };
            }
        }
    }
}
