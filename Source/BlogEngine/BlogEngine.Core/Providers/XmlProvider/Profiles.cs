namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The xml blog provider.
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// The delete profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void DeleteProfile(AuthorProfile profile)
        {
            var fileName = string.Format("{0}profiles{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, profile.Id);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (AuthorProfile.Profiles.Contains(profile))
            {
                AuthorProfile.Profiles.Remove(profile);
            }
        }

        /// <summary>
        /// The fill profiles.
        /// </summary>
        /// <returns>
        /// A list of AuthorProfile.
        /// </returns>
        public override List<AuthorProfile> FillProfiles()
        {
            var folder = string.Format("{0}profiles{1}", this.Folder, Path.DirectorySeparatorChar);

            return (from file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly)
                    select new FileInfo(file)
                    into info 
                    select info.Name.Replace(".xml", string.Empty)
                    into username
                    select AuthorProfile.Load(username)).ToList();
        }

        /// <summary>
        /// The insert profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void InsertProfile(AuthorProfile profile)
        {
            if (!Directory.Exists(string.Format("{0}profiles", this.Folder)))
            {
                Directory.CreateDirectory(string.Format("{0}profiles", this.Folder));
            }

            var fileName = string.Format("{0}profiles{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, profile.Id);
            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("profileData");

                writer.WriteElementString("DisplayName", profile.DisplayName);
                writer.WriteElementString("FirstName", profile.FirstName);
                writer.WriteElementString("MiddleName", profile.MiddleName);
                writer.WriteElementString("LastName", profile.LastName);

                writer.WriteElementString("CityTown", profile.CityTown);
                writer.WriteElementString("RegionState", profile.RegionState);
                writer.WriteElementString("Country", profile.Country);

                writer.WriteElementString("Birthday", profile.Birthday.ToString("yyyy-MM-dd"));
                writer.WriteElementString("AboutMe", profile.AboutMe);
                writer.WriteElementString("PhotoURL", profile.PhotoUrl);

                writer.WriteElementString("Company", profile.Company);
                writer.WriteElementString("EmailAddress", profile.EmailAddress);
                writer.WriteElementString("PhoneMain", profile.PhoneMain);
                writer.WriteElementString("PhoneMobile", profile.PhoneMobile);
                writer.WriteElementString("PhoneFax", profile.PhoneFax);

                writer.WriteElementString("IsPrivate", profile.Private.ToString());

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Retrieves a Page from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>An AuthorProfile.</returns>
        public override AuthorProfile SelectProfile(string id)
        {
            var fileName = string.Format("{0}profiles{1}{2}.xml", this.Folder, Path.DirectorySeparatorChar, id);
            var doc = new XmlDocument();
            doc.Load(fileName);

            var profile = new AuthorProfile(id);

            if (doc.SelectSingleNode("//DisplayName") != null)
            {
                profile.DisplayName = doc.SelectSingleNode("//DisplayName").InnerText;
            }

            if (doc.SelectSingleNode("//FirstName") != null)
            {
                profile.FirstName = doc.SelectSingleNode("//FirstName").InnerText;
            }

            if (doc.SelectSingleNode("//MiddleName") != null)
            {
                profile.MiddleName = doc.SelectSingleNode("//MiddleName").InnerText;
            }

            if (doc.SelectSingleNode("//LastName") != null)
            {
                profile.LastName = doc.SelectSingleNode("//LastName").InnerText;
            }

            // profile.Address1 = doc.SelectSingleNode("//Address1").InnerText;
            // profile.Address2 = doc.SelectSingleNode("//Address2").InnerText;
            if (doc.SelectSingleNode("//CityTown") != null)
            {
                profile.CityTown = doc.SelectSingleNode("//CityTown").InnerText;
            }

            if (doc.SelectSingleNode("//RegionState") != null)
            {
                profile.RegionState = doc.SelectSingleNode("//RegionState").InnerText;
            }

            if (doc.SelectSingleNode("//Country") != null)
            {
                profile.Country = doc.SelectSingleNode("//Country").InnerText;
            }

            if (doc.SelectSingleNode("//Birthday") != null)
            {
                DateTime date;
                if (DateTime.TryParse(doc.SelectSingleNode("//Birthday").InnerText, out date))
                {
                    profile.Birthday = date;
                }
            }

            if (doc.SelectSingleNode("//AboutMe") != null)
            {
                profile.AboutMe = doc.SelectSingleNode("//AboutMe").InnerText;
            }

            if (doc.SelectSingleNode("//PhotoURL") != null)
            {
                profile.PhotoUrl = doc.SelectSingleNode("//PhotoURL").InnerText;
            }

            if (doc.SelectSingleNode("//Company") != null)
            {
                profile.Company = doc.SelectSingleNode("//Company").InnerText;
            }

            if (doc.SelectSingleNode("//EmailAddress") != null)
            {
                profile.EmailAddress = doc.SelectSingleNode("//EmailAddress").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneMain") != null)
            {
                profile.PhoneMain = doc.SelectSingleNode("//PhoneMain").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneMobile") != null)
            {
                profile.PhoneMobile = doc.SelectSingleNode("//PhoneMobile").InnerText;
            }

            if (doc.SelectSingleNode("//PhoneFax") != null)
            {
                profile.PhoneFax = doc.SelectSingleNode("//PhoneFax").InnerText;
            }

            if (doc.SelectSingleNode("//IsPrivate") != null)
            {
                profile.Private = doc.SelectSingleNode("//IsPrivate").InnerText == "true";
            }

            // page.DateCreated = DateTime.Parse(doc.SelectSingleNode("page/datecreated").InnerText, CultureInfo.InvariantCulture);
            // page.DateModified = DateTime.Parse(doc.SelectSingleNode("page/datemodified").InnerText, CultureInfo.InvariantCulture);
            return profile;
        }

        /// <summary>
        /// The update profile.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public override void UpdateProfile(AuthorProfile profile)
        {
            this.InsertProfile(profile);
        }

        #endregion
    }
}