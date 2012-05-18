namespace Admin.Posts
{
    public partial class Menu : System.Web.UI.UserControl
    {
        /// <summary>
        /// Indicate that menu item selected
        /// </summary>
        /// <param name="pg">Page address</param>
        /// <returns>CSS class to append for current menu item</returns>
        protected string Current(string pg)
        {
            if (Request.Path.ToLower().Contains(pg.ToLower()))
            {
                return "class=\"content-box-selected\"";
            }
            return "";
        }
    }
}