namespace WebApp;
public static class Utils
{
    // Read all mock users from file
    private static readonly Arr mockUsers = JSON.Parse(File.ReadAllText(FilePath("json", 
                                                                                 "mock-users.json")));

    // Read all bad words from file and sort from longest to shortest
    // if we didn't sort we would often get "---ing" instead of "---" etc.
    // (Comment out the sort - run the unit tests and see for yourself...)
    private static readonly Arr badWords = ((Arr)JSON.
                                                 Parse(File.ReadAllText(FilePath("json", "bad-words.json")))).
                                                 Sort((a, b) => ((string)b).Length - ((string)a).Length);

    public static bool IsPasswordGoodEnough(string password)
    {
        return password.Length >= 8 &&
               password.Any(Char.IsAsciiDigit) && 
               password.Any(Char.IsAsciiLetterLower) && 
               password.Any(Char.IsAsciiLetterUpper) && 
               password.Any(x => !Char.IsAsciiLetterOrDigit(x));
    }

    public static bool IsPasswordGoodEnoughRegexVersion(string password)
    {
        // See: https://dev.to/rasaf_ibrahim/write-regex-password-validation-like-a-pro-5175
        string pattern = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
        return Regex.IsMatch(password, pattern);
    }
    
    public static string RemoveBadWords(string comment, string replaceWith = "---")
    {
        comment = $" {comment}";
        replaceWith = $" {replaceWith}$1";
        badWords.ForEach(badWord =>
        {
            string pattern = @$" {badWord}([\.\!\?\:\; ])";
            comment = Regex.Replace(comment, 
                                    pattern, 
                                    replaceWith, 
                                    RegexOptions.IgnoreCase);
        });
        return comment[1..];
    }

    public static Arr CreateMockUsers()
    {
        // Read all mock users from the JSON file
        string read = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(read);
        Arr successFullyWrittenUsers = Arr();
        foreach (dynamic user in mockUsers)
        {
            //user.password = "12345678";
            dynamic result = SQLQueryOne(@"INSERT INTO users(firstName, lastName, email, password)
                                           VALUES($firstName, $lastName, $email, $password)", user);
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

    public static Arr RemoveMockUsers()
    {
        foreach (dynamic userToRemove in mockUsers)
        {
            SQLQueryOne($"DELETE FROM users WHERE id = {userToRemove.Id}");
            userToRemove.Delete("password");
        }
        return mockUsers;
    }

    public static Obj CountDomainsFromUserEmails()
    {   
        Obj domainsCount = new Obj();

        return domainsCount;    
    }

    // Now write the two last ones yourself!
    // See: https://sys23m-jensen.lms.nodehill.se/uploads/videos/2021-05-18T15-38-54/sysa-23-presentation-2024-05-02-updated.html#8
}