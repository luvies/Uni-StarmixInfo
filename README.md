# Starmix Info
The website for out Unversity year 2 SEPR module.

The site is an ASP.NET Core site that uses a MySQL database for storing project data. It is designed to be used over the course of the whole project, meaning that it is designed to allow the adding, updating and removing of projects without having to update the codebase, as this allows us to iterate site changes much quicker.

The projects data is given in a form that allows us to specify the following items (all are HTML-escaped unless stated):

- Title
- Short description
- Long description (raw HTML to allow formatting)
- Unity Organisation ID (for build API)
- Unity Project ID (for build API)
- Google Drive folder ID (for documentation storage)

If the Unity organisation or project ID contains the string `-`, then the builds are assumed to not exist, the the 'View Builds' button is not shown.

The admin page is locked behind a simple login system that hands out a session token to the user that is logged in. It only stores a single token, meaning that the database will not get too full and it stops a user agent from staying logged in permanently (which could be a security risk). The password is set via a 'Set Password' form that is shown if the password hash in the database doesn't exist. While this presents a small security risk in setting up, since we don't plan on huge visiter numbers, and we don't plan on setting up more than once, so this is considered a non-issue.

## Dev Database
In order to test the server properly, I have set up a system where a MySQL database can be created and managed through a docker container. This allows the database to be isolated from the rest of the parent system, and allow for finer control over the database. The SQL folder is for all the SQL that is needed to set up the database, and also modify it in future (meaning if we need to change the tables etc. we can test the SQL using this system). I've set it up so that if changes are made, then rebuilding the database will cause it to be initialised using the base script, then modified using the following, ensuring that the database is exactly the same.

This dev DB system will be put into a gist evetually, since I will re-use it for other systems in future (as it is useful for managing dev DB environments).

## App Secrets
Since we have the project tracked under git, we have to make sure that we do not store sensitive information in the repository. To acomplish this while still being able to use the standard system, I've add the ability to store them in a separate JSON file that inside a folder that is ignored by git. These are the files and their uses.

### `~/StarmixInfo/Secrets/appsecrets.json`
This is used to store the Unity API Auth Token. The structure is as follows:

```json
{
  "UnityAuthToken": "<auth token>"
}
```

This is need to load any build page.

### `~/StarmixInfo/Secrets/appsecrets.Development.json` & `~/StarmixInfo/Secrets/appsecrets.Production.json`
These files are to store the MySQL database connection strings for both the development and production databases. These are not required, however the connection string they are designed to contain is. This means that, if you do not want these, you will still have to put the connection string in the main `appsecrets.json`. The structure is as follows:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<server>;port=<port>;database=<database>;user=<user>;pwd=<password>"
  }
}
```

This is needed to connect to the MySQL database, which is required for running.
