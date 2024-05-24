using System.Security.Cryptography.X509Certificates;

namespace WebApp;
public static class Utils
{
    // Read all mock users from file
    // Read all bad words from file and sort from longest to shortest
    // if we didn't sort we would often get "---ing" instead of "---" etc.
    // (Comment out the sort - run the unit tests and see for yourself...)

    private static readonly Arr mockUsers = (Arr)JSON.Parse(
        File.ReadAllText(FilePath("json", "mock-users.json"))
    );
    private static readonly Arr badWords = ((Arr)JSON.Parse(
        File.ReadAllText(FilePath("json", "bad-words.json"))
    )).Sort((a, b) => ((string)b).Length - ((string)a).Length);
    public static bool IsPasswordGoodEnough(string password)
    {
        return password.Length >= 8
            && password.Any(Char.IsDigit)
            && password.Any(Char.IsLower)
            && password.Any(Char.IsUpper)
            && password.Any(x => !Char.IsLetterOrDigit(x));
    }

    public static bool IsPasswordGoodEnoughRegexVersion(string password)
    {
        // See: https://dev.to/rasaf_ibrahim/write-regex-password-validation-like-a-pro-5175
        var pattern = @"^(?=.*[0-9])(?=.*[a-zåäö])(?=.*[A-ZÅÄÖ])(?=.*\W).{8,}$";
        return Regex.IsMatch(password, pattern);
    }

    public static string RemoveBadWords(string comment, string replaceWith = "---")
    {
        comment = " " + comment;
        replaceWith = " " + replaceWith + "$1";
        badWords.ForEach(bad =>
        {
            var pattern = @$" {bad}([\,\.\!\?\:\; ])";
            comment = Regex.Replace(
                comment, pattern, replaceWith, RegexOptions.IgnoreCase);
        });
        return comment[1..];
    }

    public static Arr CreateMockUsers()
    {
        // Read all mock users from the JSON file
        
        Arr successFullyWrittenUsers = Arr();
        foreach (var user in mockUsers)
        {
            // user.password = "12345678";
            var result = SQLQueryOne(
                @"INSERT INTO users(firstName,lastName,email,password)
                VALUES($firstName, $lastName, $email, $password)
            ", user);
            // If we get an error from the DB then we haven't added
            // the mock users, if not we have so add to the successful list
            if (!result.HasKey("error"))
            {
                // The specification says return the user list without password
                user.Delete("password");
                successFullyWrittenUsers.Push(user);
            }
        }
        return successFullyWrittenUsers;
    }

    // Now write the two last ones yourself!
    // See: https://sys23m-jensen.lms.nodehill.se/uploads/videos/2021-05-18T15-38-54/sysa-23-presentation-2024-05-02-updated.html#8
   public class User
{
    public int Id;
    public string FirstName;
    public string LastName;
    public string Email;
    public string Role;
    public override string ToString()
    {
    return $@"
        Id:         {Id}, 
        Username:   {FirstName}, 
        Last name:  {LastName}, 
        Email:      {Email}, 
        Role:       {Role}";
    }
}   
    public static string connectionString = "Data Source = _db.sqlite3;";
    public static Arr DeleteMockUsers()
    {
        Arr successfullyDeletedUsers = Arr();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var selectQuery = @"SELECT id, firstName, lastName, email, role FROM users WHERE id >= 6";
            using (SqliteCommand selectCommand = new SqliteCommand(selectQuery, conn))
            {
                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4)
                        };
                        successfullyDeletedUsers.Push(user);
                        foreach (var dUser in successfullyDeletedUsers){
                            Console.WriteLine(@$"
                            Id          = {user.Id},
                            firstName   = {user.FirstName},
                            lastName    = {user.LastName},
                            email       = {user.Email},
                            role        = {user.Role},
                            ");
                        }
                    } 
                }
                var deleteQuery = "DELETE FROM users WHERE id >= 6;";
                using (var deleteCommand = new SqliteCommand(deleteQuery, conn))
                {
                    deleteCommand.ExecuteNonQuery();
                }
            }
            conn.Close();
        }
        return successfullyDeletedUsers;
    } 

    public static Obj CountDomainsFromUserEmails()
    {
        var domainKey = Obj();

        Arr usersInDb = SQLQuery("SELECT email FROM users");
        Arr emailsInDb = usersInDb.Map(user => user["email"]);

        foreach (string email in emailsInDb)
        {
            string domain = email.Split('@')[1];
            if (!domainKey.HasKey(domain)){
                domainKey[domain] = 0;
            }
            domainKey[domain]++;
        }

        Log(domainKey.GetEntries());

        return domainKey;
    }   
}