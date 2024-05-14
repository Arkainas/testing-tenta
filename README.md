## Basics

### After git clone, before you start the app!
* Make a copy of DbTemplate/_db.sqlite3 to Backend/_db.sqlite3
* Why? We don't want small changes (reading/writing data) to
  affect git, and Backend/_db.sqlite3 is gitignored!

### After major/structural changes to the db
If you make structural changes to the db or add important data:
* It's fine to make these changes in Backend/_db.sqlite3.
* That way you can check out that they work with the backend!

To commit them to git:
* Turn off the backend/server!
* Make a copy of Backend/_db.sqlite3 to DbTemplate/_db.sqlite3.
* Commit to git!

## About the REST-api

### Backend/src/App.cs
This is the starting point. Here you can easily turn off ACL and/or the Debugger.

### Have a look in the db
Currently we have three users, one with admin rights, and all users have password "12345678".
We recommend SQLiteStudio as a DB editor!

### Rest routes are dynamic - there will automatically be REST routes for all tables and views in the database!
If you add a table or view in the DB, corresponding routes will be created automatically.

**I say this once more:** YOU NORMALLY DON'T ADD ANY CODE IN ORDER TO CREATE NEW ROUTES. YOU ADD TABLES AND VIEWS IN THE DB!

#### Five standard routes per table
Of course we have the bread and butter of all REST-api:s covered. For any table, replace tablename below with a tablename and the id with a specific id:
* POST /api/tablename - with a request body in JSON format - create a new row in the table and get the insert id back.
* GET /api/tablename - get all rows from the table as a JSON array of objects.
* GET /api/tablename/id - get the row from the table with a specific id as a JSON object. (Note: You must name your id columns in your tables just id column "id", not "elephantId" etc).
* PUT /api/tablename/id - change one or many properties for a existing row. Send a request body in JSON format containing only the fields/columns you want to change.
* DELETE /api/tablename/id - delete a specific row in the table.

#### More than just standard routes
* You can use (up to) four different query parameters for the GET query without id to do much more, and find what you are looking for directly:
* where = condition, to filter posts returned
* orderby = field1,[field2... etc] to sort the posts returned. For a descending sort prefix the field name with "-"
* limit = numberOfPosts, to limit the number of posts
* offset = numberOfPosts, to skip a number of posts at the beginning.

An example:
* /api/users?where=firstName=Thomas_AND_lastName!=Irons&orderby=email&limit=2&offset=1
* Note: Don't surround strings with quotes, as you can see in the example above we don't.

##### Currently supported operators for where
*  !=, >=, <=, =, >, <, _AND_, _OR_, _LIKE_  (writing the latter three with underscores are optional but improves readability)
*  Parentheses are currently not supported.

### SQL Injections? Are we safe against them?
* Everything that we can move to parameters in prepared statement are parameters in prepared statements. The Debugger shows the SQL that is generated for a particular route, including the parameters!
* User input that has to be part of the main query (and can not be parametrized) - like data for ORDER BY, LIMIT etc. is sanitized with rather restrictive regexs. 
* So hopefully we are safe against SQL injections.

### If the ACL is on you need to add rules in the acl table!
* You whitelist routes based on userRoles. 
* If you set match to false this negates the route matching. See comments in ACL table.
* All rules marked "allow" (whitelisting) runs before rules marked "disallow" (blacklisting).
* So you only use blacklisting to tighten up your previous whitelisting. See comments in the ACL table.
* The ACL is not just for the REST-api but for all routes (i.e. fronend/static files as well).
* ACL rules are reread every 60 seconds, alternatively you can restart the server to observe changes at once.

### Cookies and session
* All users (even visitors) gets a cookie bound to a session stored in the DB table sessions,
  as soon as they visit a page. 
* Everytime you make a request the modified time is updated for the session in the DB.
* If a session has not been modified for 2 hours it gets deleted.
* However cookies survives for a browser session (as long as the browser is open).
* This means: A new session can be created from an "old" cookie, which is not a problem.

### Login-routes
The only api routes that are not governed by which tables and views you have in your DB are the login routes.

#### POST /api/login: Login
* The request body should be in JSON-format and contain an existing email and password. 

#### GET /api/login: Check if someone is logged in and retrieve user details.
* Check if someone is logged in and retrieve user details.

#### DELETE /api/login: Logout
*  Logout!

### But how do I register a new user? POST /api/users
* The request body should be in JSON-format and contain a non-existing email, a password, a firstName and a lastName.

### Passwords
* Passwords are BCrypted (with strength 13) and are removed from REST-api responses as well.
* There are no restrictions as to how complex they have to be for now - but I'm thinking of adding a check for a minumum password entropy and/or demand a minumum length and that they are a mix of small and large letters, numbers and at least on other character.

## Dependencies - Nuget packages
The project has three dependencies, all specified in the **Backend.csjproj** file:
* [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite) - a lightweight driver (ADO.NET provider) for SQLite - [read the documentation here](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli).
* [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next) - a password hashing function - [read the documentation here](https://github.com/BcryptNet/bcrypt.net)
* [Dyndata](https://www.nuget.org/packages/Dyndata) - for simplified handling of objects and lists/arrays (+ also has a nice logging and JSON functionality) - [read the documentation here](https://dyndata.nodehill.com)

## Source code - an quick overview
* The source code for the Backend is locatee in the Backend/src-folder and a large part of it is richly commented.
* The source code for the Frontend (just a small SPA example - not connected to the REST-api in anyway yet) is located in the Frontend-folder.

TEST
