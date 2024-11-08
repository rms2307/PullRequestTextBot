using PullRequestTextBot;

Console.WriteLine("Starting App...");

string branch1 = "main";
string branch2 = "develop";

if (!LoginManager.CheckIfIsLoggedIn())
{
    LoginManager.Login();
}

string diffs = GitManager.ExecuteGitCommandDiff(branch1, branch2);

if (!string.IsNullOrWhiteSpace(diffs))
{
    string requestText = await IAManager.GenerateTextPullRequest(diffs, LoginManager.GetApiKey());
}

Console.WriteLine("Finishing App...");