# Starmix Info
The website we have to use for the SEPR project.

I plan to make the site in such a way that we can change the information on it without requiring a change of the source + redeployment. It will be hosted on Amazon AWS as I'm buying domains through them anyway, and so they will give me the 12 month free trial of their services, which includes a database and hosted virtual machine.

While I will try to allow most changes to go through the site, I am going to make deployment to the server manageable via scripts (since it's something I need to get working for another future project), so I won't mind making changes.

## Dev Database
In order to test the server properly, I have set up a system where a MySQL database can be created and managed through a docker container. This allows the database to be isolated from the rest of the parent system, and allow for finer control over the database. The SQL folder is for all the SQL that is needed to set up the database, and also modify it in future (meaning if we need to change the tables etc. we can test the SQL using this system). I've set it up so that if changes are made, then rebuilding the database will cause it to be initialised using the base script, then modified using the following, ensuring that the database is exactly the same.

This dev DB system will be put into a gist evetually, since I will re-use it for other systems in future (as it is useful for managing dev DB environments).