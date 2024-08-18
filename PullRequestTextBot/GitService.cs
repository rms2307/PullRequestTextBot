using System.Diagnostics;

namespace PullRequestTextBot
{
    public class GitService
    {
        public string ExecuteGitCommandDiff(string source, string destination)
        {
            Process process = new();
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = $"diff {source} {destination}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
