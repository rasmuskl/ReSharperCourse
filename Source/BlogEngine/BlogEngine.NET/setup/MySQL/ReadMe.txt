Running BlogEngine.NET 2.6 using MySQL:

If you wish to use MySQL to store all your blog data, this folder has all the 
information you'll likely need.  The scripts included here are for MySQL 5.1.
They could be modified to be used with earlier versions if needed.

Included is the Initial Setup script for use with new installation of 2.6.  Also,
included is an upgrade script for users of BlogEngine 2.5.  In addition, you 
will find a sample web.config file with the needed changes to use MySQL.

Instructions for new setup:

1. Using the tool of your choice, execute the Setup script against the database you 
want to add the BlogEngine data to.  This can be a new or existing database.
2. Rename MySQLWeb.Config to Web.config and copy it to your blog folder.  (This will
overwrite your existing web.config file.  If this is not a new installation, make sure 
you have a backup).
3. Update the BlogEngine connection string in the web.config.
4. Add the MySQL .NET Connector (6.5.4) to the bin folder. If a different version is used,
please update the web.config to the correct version.
5. Surf out to your Blog and see the welcome post.
6. Login with the username admin and password admin.  Change the password.

Upgrading from 2.5:

1. Using the tool of your choice, execute the upgrade script against the database where 
you have your BlogEngine data.
2. The web.config file has changed from 2.5 to 2.6.  It will likely be easiest to start
with the sample web.config file as described above, but if you have other changes in it, 
you'll need to merge them.  Don't forget to move your connectionString over and update the
MySQL .NET Connector version if different than 6.5.4.

Additional information can be found at http://dotnetblogengine.net