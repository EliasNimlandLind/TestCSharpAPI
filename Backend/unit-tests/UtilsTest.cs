using Xunit;
using Xunit.Abstractions;
namespace WebApp;

public class UtilsTest(Xlog Console)
{
    /*
    // Read all mock users from file
    private static readonly Arr mockUsers = JSON.Parse(File.ReadAllText(FilePath("json",
                                                                                 "mock-users.json")));

    [Theory]
    [InlineData("abC9#fgh", true)]  // ok
    [InlineData("stU5/xyz", true)]  // ok too
    [InlineData("abC9#fg", false)]  // too short
    [InlineData("abCd#fgh", false)] // no digit
    [InlineData("abc9#fgh", false)] // no capital letter
    [InlineData("abC9efgh", false)] // no special character
    public void TestIsPasswordGoodEnough(string password, bool expected)
    {
        Assert.Equal(expected, Utils.IsPasswordGoodEnough(password));
    }

    [Theory]
    [InlineData("abC9#fgh", true)]  // ok
    [InlineData("stU5/xyz", true)]  // ok too
    [InlineData("abC9#fg", false)]  // too short
    [InlineData("abCd#fgh", false)] // no digit
    [InlineData("abc9#fgh", false)] // no capital letter
    [InlineData("abC9efgh", false)] // no special character
    public void TestIsPasswordGoodEnoughRegexVersion(string password, bool expected)
    {
        Assert.Equal(expected, Utils.IsPasswordGoodEnoughRegexVersion(password));
    }

    [Theory]
    [InlineData("---",
                @"Hello, I am going through hell. Hell is a real fucking place 
                  outside your goddamn comfy tortoiseshell!",
                @"Hello, I am going through ---. --- is a real --- place 
                  outside your --- comfy tortoiseshell!"
    )]
    [InlineData("---",
                @"Rhinos have a horny knob? (or what should I call it) on 
                  their heads. And doorknobs are damn round.",
                @"Rhinos have a --- ---? (or what should I call it) on 
                  their heads. And doorknobs are --- round."
    )]
    public void TestRemoveBadWords(string replaceWith, string original, string expected)
    {
        Assert.Equal(expected, Utils.RemoveBadWords(original, replaceWith));
    }

    [Fact]
    public void TestCreateMockUsers()
    {
        // Read all mock users from the JSON file
        string mockUsersAsJson = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(mockUsersAsJson);
        // Get all users from the database
        Arr usersInDb = SQLQuery("SELECT email FROM users");
        Arr emailsInDb = usersInDb.Map(user => user.email);
        // Only keep the mock users not already in db
        Arr mockUsersNotInDb = mockUsers.Filter(mockUser => !emailsInDb.Contains(mockUser.email));
        // Get the result of running the method in our code
        Arr result = Utils.CreateMockUsers();
        // Assert that the CreateMockUsers only return
        // newly created users in the db
        string outputText = @$"The test expected that {mockUsersNotInDb.Length} users should be added.\n
                               And {result.Length} users were added.\n
                               The test also asserts that the users added \n
                               are equivalent (the same) to the expected users!";
        Console.WriteLine(outputText);
        Assert.Equivalent(mockUsersNotInDb, result);
        Console.WriteLine("The test passed!");
    }

    [Fact]
    public void TestRemoveMockUsers()
    {
        Arr result = Utils.RemoveMockUsers();
        Assert.Equivalent(mockUsers, result);

        Console.WriteLine(@$"The test expected that {mockUsers} to be removed from the database and returned.
                             The test passed.");
    }*/

    [Fact]
    public void TestCountDomainsFromUserEmails()
    {
        Arr userEmails = SQLQuery("SELECT email FROM users");
        Arr domains = new Arr();

        Obj domainObj = new Obj();
        foreach (Obj email in userEmails)
        {
            string filter = email.ToString();
            string domainString = filter.Split('@')[1];
            domainString = domainString.Split("\"}")[0];

            domainObj = Obj(new { domain = domainString });
            domains.Append(domainObj);
            //Console.WriteLine(domainObj.GetValues()[0]);
        }

        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (Obj domain in domains)
        {
            int amountOfOccurrences = domains.Count(x => (string)x == domainObj.GetValues()[0]);
            KeyValuePair<string, int> keyValuePair = new KeyValuePair<string, int>(domainObj.GetValues()[0], amountOfOccurrences);
            dictionary.Append(keyValuePair);
            Console.WriteLine(keyValuePair.ToString());
        }

        //     // FAULT due to initializing Obj variable to string value ------------------------------------
        //     Dictionary<string, int> dictionary = new Dictionary<string, int>();
        //     foreach (string domain in domains)
        //     {
        //         int amountOfOccurrences = domains.Count(x => (string)x == domain);
        //         KeyValuePair<string, int> keyValuePair = new KeyValuePair<string, int>(domain, amountOfOccurrences);
        //         dictionary.Append(keyValuePair);
        //         Console.WriteLine(keyValuePair.ToString());
        //     }
        //     // ---------------------------------------------

        //     Obj domainsCountTotal = new Obj(dictionary);

        //     Obj result = Utils.CountDomainsFromUserEmails();

        //     string outputText = @$"The test expected that {domains.Length} domains should be added.\n
        //                            and {result} emails were added.";
        //     Console.WriteLine(outputText);

        //     Assert.Equivalent(domainsCountTotal, result);
        //     Console.WriteLine("The test passed!");
    }
}