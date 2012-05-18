Theme: JQ Mobile
Version: 1.3.0
Date: 2012-05-04
Author: Michael Baird (http://michaeljbaird.com)
Tested with BE Version: 2.6 RC
Details: http://www.michaeljbaird.com/post/2011/11/14/JQ-Mobile-Theme-for-Blogenginenet.aspx
Requirements: App_Code/JQ-Mobile/ThemeHelper.cs
***************************************************************************************************
Instructions:
  1. Make sure to add App_Code/JQ-Mobile/ThemeHelper.cs
  2. Assign your mobile theme to JQ-Mobile in you Blogengine.net admin settings
  3. To Change the JQuery Mobile theme type, open site.master and find <div data-role="page"> and add data-theme="a" (a,b,c,d,e are available). 
     A list of the theme types can be found here: http://jquerymobile.com/demos/1.1.0/docs/pages/pages-themes.html 
***************************************************************************************************
Release notes:
  Version 1.3.0:
    * Upgraded JQuery Mobile references to latest 1.1.0
    * Fixed Frontpage PostList to work with BlogEngine.net 2.6
    * Changed JQuery Mobile to make AJAX Requests
    * Added data-ajax="false" to Contact page in order for the form to work properly.
    * Added page transition animations to menu links

  Version 1.2.3:
    * Fixed bugs with Blogengine being a Virtual App in sub directory

  Version 1.2.2:
    * Disabled ajax/caching in JQuery Mobile by mobileinit that was affecting posting of forms in comments and search page.
    * Deleted some unnecessary files

  Version 1.2:
    * Embedded images into stylesheet for less Http Requests
    * Force images to be max-width 100% of screen width
    * Some code clean-up

  Version 1.1:
    * Updated JQuery Mobile to RC3
    * Added Archives page
    * Updated Search Page Style
    * other various bug fixes.
***************************************************************************************************