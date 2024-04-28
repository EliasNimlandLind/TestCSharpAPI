namespace WebApp;
public static class Acl
{
    private static Arr rules;

    public static async void Start()
    {
        // Read rules from db once a minute
        while (true)
        {
            UnpackRules(SQLQuery("SELECT * FROM acl ORDER BY allow"));
            await Task.Delay(60000);
        }
    }

    public static void UnpackRules(Arr allRules)
    {
        // Unpack db response -> routes to regexps and userRoles to arrays
        rules = allRules.Map(x => new
        {
            ___ = x,
            regexPattern = @"^" + x.route.Replace("/", @"\/") + @"\/",
            userRoles = ((Arr)Arr(x.userRoles.Split(','))).Map(x => x.Trim())
        });
    }

    public static bool Allow(
        HttpContext context, string method = "", string path = ""
    )
    {
        // Return true/allowed for everything if acl is off in Globals
        if (!Globals.aclOn) { return true; }

        // Get info about the requested route and logged in user
        method = method != "" ? method : context.Request.Method;
        path = path != "" ? path : context.Request.Path;
        var user = Session.Get(context, "user");
        var userRole = user == null ? "visitor" : user.role;
        var userEmail = user == null ? "" : user.email;

        // Go through all acl rules to and set allowed accordingly!
        var allowed = false;
        foreach (var rule in rules)
        {
            // Get the properties of the rule as variables
            var ruleMethod = rule.method;
            var ruleRegexPattern = rule.regexPattern;
            var ruleRoles = (Arr)rule.userRoles;
            var ruleMatch = rule.match == "true";
            var ruleAllow = rule.allow == "allow";

            // Check if role, method and path is allowed according to the rule
            var roleOk = ruleRoles.Includes(userRole);

            Log("include", ruleRoles.Includes(userRole), "some", ruleRoles.Some(x => x == userRole));

            var methodOk = method == ruleMethod || ruleMethod == "*";
            var pathOk = Regex.IsMatch(path + "/", ruleRegexPattern);
            // Note: "match" can be false - in that case we negate pathOk!
            pathOk = ruleMatch ? pathOk : !pathOk;

            // Is everything ok?
            var allOk = roleOk && methodOk && pathOk;

            // Note: We whitelist first (check all allow rules) - ORDER BY allow
            // and then we blacklist on top of that (check all disallow rules)
            allowed = ruleAllow ? allowed || allOk : allOk ? false : allowed;
        }
        return allowed;
        /*var userLabel = userEmail != "" ? "the user" : "the anonymous visitor";
        userEmail = userEmail != "" ? "'" + userEmail + "' " : "";
        Debug.Log(
            "acl", $"  ACL: {allowed} for {userLabel}"
            + $" {userEmail}with the role '{userRole}'.");
        return allowed;*/
    }
}