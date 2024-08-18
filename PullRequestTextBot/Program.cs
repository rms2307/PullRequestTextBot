using PullRequestTextBot;

Console.WriteLine("Starting App...");

string branch1 = "main";
string branch2 = "develop";

GitService gitService = new();
string diffs = gitService.ExecuteGitCommandDiff(branch1, branch2);

IAService iaService = new();
string requestText = await iaService.GenerateTextPullRequest(diffs);

Console.WriteLine("Finishing App...");