namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;

    using BlogEngine.Core.Providers;
    using Json;

    /// <summary>
    /// The author profile.
    /// </summary>
    public class AuthorProfile : BusinessBase<AuthorProfile, string>
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The profiles.
        /// </summary>
        private static Dictionary<Guid, List<AuthorProfile>> profiles;

        /// <summary>
        /// The about me.
        /// </summary>
        private string aboutMe;

        /// <summary>
        /// The birthday.
        /// </summary>
        private DateTime birthday;

        /// <summary>
        /// The city town.
        /// </summary>
        private string cityTown;

        /// <summary>
        /// The company.
        /// </summary>
        private string company;

        /// <summary>
        /// The country.
        /// </summary>
        private string country;

        /// <summary>
        /// The display name.
        /// </summary>
        private string displayName;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailAddress;

        /// <summary>
        /// The first name.
        /// </summary>
        private string firstName;

        /// <summary>
        /// The is private.
        /// </summary>
        private bool isprivate;

        /// <summary>
        /// The last name.
        /// </summary>
        private string lastName;

        /// <summary>
        /// The middle name.
        /// </summary>
        private string middleName;

        /// <summary>
        /// The phone fax.
        /// </summary>
        private string phoneFax;

        /// <summary>
        /// The phone main.
        /// </summary>
        private string phoneMain;

        /// <summary>
        /// The phone mobile.
        /// </summary>
        private string phoneMobile;

        /// <summary>
        /// The photo url.
        /// </summary>
        private string photoUrl;

        /// <summary>
        /// The region state.
        /// </summary>
        private string regionState;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorProfile"/> class.
        /// </summary>
        public AuthorProfile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorProfile"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        public AuthorProfile(string username)
        {
            this.Id = username;
        }

        static AuthorProfile()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'profiles'

                        if (profiles != null && profiles.ContainsKey(blog.Id))
                            profiles.Remove(blog.Id);
                    }
                }
            };
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets an unsorted list of all pages.
        /// </summary>
        public static List<AuthorProfile> Profiles
        {
            get
            {
                Blog blog = Blog.CurrentInstance;

                if (profiles == null || !profiles.ContainsKey(blog.Id))
                {
                    lock (SyncRoot)
                    {
                        if (profiles == null || !profiles.ContainsKey(blog.Id))
                        {
                            if (profiles == null)
                                profiles = new Dictionary<Guid, List<AuthorProfile>>();

                            profiles[blog.Id] = BlogService.FillProfiles();
                        }
                    }
                }

                return profiles[blog.Id];
            }
        }

        /// <summary>
        /// Gets or sets AboutMe.
        /// </summary>
        public string AboutMe
        {
            get
            {
                return this.aboutMe;
            }

            set
            {
                base.SetValue("AboutMe", value, ref this.aboutMe);
            }
        }

        /// <summary>
        /// Gets or sets Birthday.
        /// </summary>
        public DateTime Birthday
        {
            get
            {
                return this.birthday;
            }

            set
            {
                base.SetValue("Birthday", value, ref this.birthday);
            }
        }

        /// <summary>
        /// Gets or sets CityTown.
        /// </summary>
        public string CityTown
        {
            get
            {
                return this.cityTown;
            }

            set
            {
                base.SetValue("CityTown", value, ref this.cityTown);
            }
        }

        /// <summary>
        /// Gets or sets Company.
        /// </summary>
        public string Company
        {
            get
            {
                return this.company;
            }

            set
            {
                base.SetValue("Company", value, ref this.company);
            }
        }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        public string Country
        {
            get
            {
                return this.country;
            }

            set
            {
                base.SetValue("Country", value, ref this.country);
            }
        }

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                base.SetValue("DisplayName", value, ref this.displayName);
            }
        }

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        public string EmailAddress
        {
            get
            {
                return this.emailAddress;
            }

            set
            {
                base.SetValue("EmailAddress", value, ref this.emailAddress);
            }
        }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                base.SetValue("FirstName", value, ref this.firstName);
            }
        }

        /// <summary>
        /// Gets FullName.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}", this.FirstName, this.MiddleName, this.LastName).Replace("  ", " ");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Private.
        /// </summary>
        public bool Private
        {
            get
            {
                return this.isprivate;
            }

            set
            {
                base.SetValue("Private", value, ref this.isprivate);
            }
        }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        public string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                base.SetValue("LastName", value, ref this.lastName);
            }
        }

        /// <summary>
        /// Gets or sets MiddleName.
        /// </summary>
        public string MiddleName
        {
            get
            {
                return this.middleName;
            }

            set
            {
                base.SetValue("MiddleName", value, ref this.middleName);
            }
        }

        /// <summary>
        /// Gets or sets PhoneFax.
        /// </summary>
        public string PhoneFax
        {
            get
            {
                return this.phoneFax;
            }

            set
            {
                base.SetValue("PhoneFax", value, ref this.phoneFax);
            }
        }

        /// <summary>
        /// Gets or sets PhoneMain.
        /// </summary>
        public string PhoneMain
        {
            get
            {
                return this.phoneMain;
            }

            set
            {
                base.SetValue("PhoneMain", value, ref this.phoneMain);
            }
        }

        /// <summary>
        /// Gets or sets PhoneMobile.
        /// </summary>
        public string PhoneMobile
        {
            get
            {
                return this.phoneMobile;
            }

            set
            {
                base.SetValue("PhoneMobile", value, ref this.phoneMobile);
            }
        }

        /// <summary>
        /// Gets or sets PhotoURL.
        /// </summary>
        public string PhotoUrl
        {
            get
            {
                return this.photoUrl;
            }

            set
            {
                base.SetValue("PhotoUrl", value, ref this.photoUrl);
            }
        }

        /// <summary>
        /// Gets or sets RegionState.
        /// </summary>
        public string RegionState
        {
            get
            {
                return this.regionState;
            }

            set
            {
                base.SetValue("RegionState", value, ref this.regionState);
            }
        }

        /// <summary>
        /// Gets RelativeLink.
        /// </summary>
        public string RelativeLink
        {
            get
            {
                return string.Format("{0}author/{1}.aspx", Utils.RelativeWebRoot, this.Id);
            }
        }

        /// <summary>
        /// Gets UserName.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.Id;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The AuthorProfile.</returns>
        public static AuthorProfile GetProfile(string username)
        {
            return
                Profiles.Find(p => p.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.FullName;
        }

        public static JsonProfile ToJson(string username)
        {
            var j = new JsonProfile();
            var p = Profiles.Find(ap => ap.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (p != null)
            {
                j.AboutMe = p.AboutMe;
                j.Birthday = p.Birthday.ToShortDateString();
                j.CityTown = p.CityTown;
                j.Country = p.Country;
                j.DisplayName = p.DisplayName;
                j.EmailAddress = p.EmailAddress;
                j.PhoneFax = p.PhoneFax;
                j.FirstName = p.FirstName;
                j.Private = p.Private.ToString();
                j.LastName = p.LastName;
                j.MiddleName = p.MiddleName;
                j.PhoneMobile = p.PhoneMobile;
                j.PhoneMain = p.PhoneMain;
                j.PhotoUrl = p.PhotoUrl;
                j.RegionState = p.RegionState;
            }
            else
            {
                j.AboutMe = "";
                j.Birthday = "01/01/1900";
                j.CityTown = "";
                j.Country = "";
                j.DisplayName = username;
                j.EmailAddress = "";
                j.PhoneFax = "";
                j.FirstName = username;
                j.Private = "yes";
                j.LastName = "";
                j.MiddleName = "";
                j.PhoneMobile = "";
                j.PhoneMain = "";
                j.PhotoUrl = "";
                j.RegionState = "";
            }

            return j;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Datas the delete.
        /// </summary>
        protected override void DataDelete()
        {
            BlogService.DeleteProfile(this);
            if (Profiles.Contains(this))
            {
                Profiles.Remove(this);
            }
        }

        /// <summary>
        /// Datas the insert.
        /// </summary>
        protected override void DataInsert()
        {
            BlogService.InsertProfile(this);

            if (this.New)
            {
                Profiles.Add(this);
            }
        }

        /// <summary>
        /// Datas the select.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>The AuthorProfile.</returns>
        protected override AuthorProfile DataSelect(string id)
        {
            return BlogService.SelectProfile(id);
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        protected override void DataUpdate()
        {
            BlogService.UpdateProfile(this);
        }

        /// <summary>
        /// Validates based on rules.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule(
                "Id",
                "Id must be set to the username of the user who the profile belongs to",
                string.IsNullOrEmpty(this.Id));
        }

        #endregion
    }
}