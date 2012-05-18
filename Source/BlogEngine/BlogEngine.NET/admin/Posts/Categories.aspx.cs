#region Using

using System;
using System.Web.UI.WebControls;
using BlogEngine.Core;
using App_Code;

#endregion

namespace Admin.Posts
{
    public partial class Categories : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminPostPages(false);

            if (!Page.IsPostBack)
            {
                BindGrid();

                LoadParentDropDown(ddlNewParent, null);
            }

            grid.RowEditing += new GridViewEditEventHandler(grid_RowEditing);
            grid.RowUpdating += new GridViewUpdateEventHandler(grid_RowUpdating);
            grid.RowCancelingEdit += delegate { Response.Redirect(Request.RawUrl); };
            grid.RowDeleting += new GridViewDeleteEventHandler(grid_RowDeleting);
            grid.RowDataBound += new GridViewRowEventHandler(grid_RowDataBound);
            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnAdd.Text = Resources.labels.add + " " + Resources.labels.category.ToLowerInvariant();
            valExist.ServerValidate += new ServerValidateEventHandler(valExist_ServerValidate);
            Page.Title = Resources.labels.categories;
        }

        void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowState == DataControlRowState.Edit ||
                    e.Row.RowState == (DataControlRowState.Alternate | DataControlRowState.Edit))
            {
                Category self = (Category)e.Row.DataItem;
                DropDownList ddlParent = (DropDownList)e.Row.FindControl("ddlParent");
                LoadParentDropDown(ddlParent, self);

                Category temp = (Category)e.Row.DataItem;
                if (temp.Parent != null)
                {
                    foreach (ListItem item in ddlParent.Items)
                    {
                        if (item.Value == temp.Parent.ToString())
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        private void LoadParentDropDown(DropDownList ddl, Category self)
        {
            // Load up the Parent DropDown
            ddl.ClearSelection();
            ddl.Items.Add(new ListItem("none", "0"));
            foreach (Category cat in Category.Categories)
            {
                if (self == null || !cat.Id.Equals(self.Id))
                    ddl.Items.Add(new ListItem(cat.CompleteTitle(), cat.Id.ToString()));
            }
        }

        /// <summary>
        /// Handles the ServerValidate event of the valExist control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void valExist_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            foreach (Category category in Category.Categories)
            {
                if (category.Title.Equals(txtNewCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    args.IsValid = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string description = txtNewNewDescription.Text;
                if (description.Length > 255)
                    description = description.Substring(0, 255);

                Category cat = new Category(txtNewCategory.Text, description);
                if (ddlNewParent.SelectedValue != "0")
                    cat.Parent = new Guid(ddlNewParent.SelectedValue);

                cat.Save();
                Response.Redirect(Request.RawUrl, true);
            }
            else
            {
                Master.SetStatus("warning", "Duplicate category");
            }
        }

        /// <summary>
        /// Handles the RowDeleting event of the grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
        void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid id = (Guid)grid.DataKeys[e.RowIndex].Value;
            Category cat = Category.GetCategory(id);

            // Removes all references to the category
            foreach (Post post in Post.Posts)
            {
                if (post.Categories.Contains(cat))
                {
                    post.Categories.Remove(cat);
                }
            }

            cat.Delete();
            cat.Save();
            Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// Handles the RowUpdating event of the grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewUpdateEventArgs"/> instance containing the event data.</param>
        void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Guid id = (Guid)grid.DataKeys[e.RowIndex].Value;
            TextBox textboxTitle = (TextBox)grid.Rows[e.RowIndex].FindControl("txtTitle");
            TextBox textboxDescription = (TextBox)grid.Rows[e.RowIndex].FindControl("txtDescription");
            DropDownList ddlParent = (DropDownList)grid.Rows[e.RowIndex].FindControl("ddlParent");
            Category cat = Category.GetCategory(id);
            cat.Title = textboxTitle.Text;
            cat.Description = textboxDescription.Text;
            if (ddlParent.SelectedValue == "0")
                cat.Parent = null;
            else
                cat.Parent = new Guid(ddlParent.SelectedValue);
            cat.Save();

            Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// Handles the RowEditing event of the grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
        void grid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grid.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        /// <summary>
        /// Binds the grid with all the categories.
        /// </summary>
        private void BindGrid()
        {
            grid.DataKeyNames = new string[] { "Id" };
            grid.DataSource = Category.Categories;
            grid.DataBind();
        }

        protected string GetParentTitle(object item)
        {
            Category temp = (Category)item;
            if (temp.Parent == null)
                return "";
            else
                return Category.GetCategory((Guid)temp.Parent).Title;
        }
    }
}

