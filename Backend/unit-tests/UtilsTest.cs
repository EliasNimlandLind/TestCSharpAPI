namespace WebApp;

public class UtilsTest(Xlog Console)
{ 
    // Read all mock users from file
    private static readonly Arr mockUsers = JSON.Parse(
        File.ReadAllText(FilePath("json", "mock-users.json"))
    );

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
    [InlineData(
        "---",
        "Hello, I am going through hell. Hell is a real fucking place " +
            "outside your goddamn comfy tortoiseshell!",
        "Hello, I am going through ---. --- is a real --- place " +
            "outside your --- comfy tortoiseshell!"
    )]
    [InlineData(
        "---",
        "Rhinos have a horny knob? (or what should I call it) on " +
            "their heads. And doorknobs are damn round.",
        "Rhinos have a --- ---? (or what should I call it) on " +
            "their heads. And doorknobs are --- round."
    )]
    public void TestRemoveBadWords(string replaceWith, string original, string expected)
    {
        Assert.Equal(expected, Utils.RemoveBadWords(original, replaceWith));
    }


    [Fact]
    public void TestRemoveMockUsers()
    {
        Arr result = Utils.RemoveMockUsers();
        Assert.Equivalent(mockUsers, result);

        Console.WriteLine(@$"The test expected that {mockUsers} to be removed from the database and returned.
                             The test passed.");
    }

    // Add AddMockUsers method again

    // Now write the two last ones yourself!
    // See: https://sys23m-jensen.lms.nodehill.se/uploads/videos/2021-05-18T15-38-54/sysa-23-presentation-2024-05-02-updated.html#8
}