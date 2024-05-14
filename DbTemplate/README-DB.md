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