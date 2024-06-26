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
        foreach (var user in mockUsers)
        {
            //user.password = "12345678";
            var result = SQLQueryOne(@"INSERT INTO users(firstName, lastName, email, password)
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
        foreach (var userToRemove in mockUsers)
        {
            SQLQueryOne($"DELETE FROM users WHERE id = {userToRemove.Id}");
            userToRemove.Delete("password");
        }
        return mockUsers;
    }

    public static Obj CountDomainsFromUserEmails()
    {
        Arr userEmails = SQLQuery("SELECT email FROM users");
        List<string> domains = new List<string>();
        dynamic domainObj = Obj();
        foreach (dynamic email in userEmails)
        {
            string filter = email.ToString();
            string domainString = filter.Split('@')[1];
            domainString = domainString.Split("\"}")[0];
            domains.Add(domainString);
        }

        var countingQuery = domains.
                            GroupBy(filter => filter).
                            Select(selected => new { Value = selected.Key, Count = selected.Count() }).
                            OrderByDescending(filter => filter.Count);

        domainObj.totalCountOfUniqueDomains = 0;
        foreach (var pair in countingQuery)
        {
            domainObj.Merge(new { pair.Value, countingQuery });
            domainObj.totalCountOfUniqueDomains++;
        }

        return domainObj;
    }
}