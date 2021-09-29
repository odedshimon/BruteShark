using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkDesktop
{
    public struct GithubReleaseVersion
    {
        public string Version { get; set; }
        public string LatestVersionUrl { get; set; }
    }

    public struct GithubUpdateReleaseResponse
    {
        public bool ShouldUpdate { get; set; }
        public string NewVersionUrl { get; set; }
    }

    public static class GithubAutoUpdater
    {
        public static string GetAssemblyVersion()
        {
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            return fieVersionInfo.FileVersion;
        }

        public static async Task<GithubReleaseVersion> GetRemoteVersion(string ownerName, string projectName)
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("my-cool-app"));
            var releases = await client.Repository.Release.GetAll(owner: ownerName, name: projectName);
            var latest = releases[0];

            return new GithubReleaseVersion()
            {
                Version = latest.TagName,
                LatestVersionUrl = latest.HtmlUrl
            };
        }

        public static async Task<GithubUpdateReleaseResponse> ShouldUpdate(string ownerName, string projectName)
        {
            // Get current running assembly version and the latest release from GitHub.
            Task<GithubReleaseVersion> getRemoteVersion = GithubAutoUpdater.GetRemoteVersion(ownerName, projectName);
            string currentVersionName = GithubAutoUpdater.GetAssemblyVersion();
            GithubReleaseVersion remoteVersionDetails = await getRemoteVersion;
            string remoteVersionName = remoteVersionDetails.Version;

            // Remove the "V" heading char from the version name (e.g "V1.2.4")
            char[] charsToRemove = { 'V', 'v' };
            var remoteVersion = new Version(remoteVersionName.TrimStart(charsToRemove));
            var currentVersion = new Version(currentVersionName.TrimStart(charsToRemove));

            var result = new GithubUpdateReleaseResponse();

            if (currentVersion < remoteVersion)
            {
                result.ShouldUpdate = true;
                result.NewVersionUrl = remoteVersionDetails.LatestVersionUrl;
            }
            else
            {
                result.ShouldUpdate = false;
                result.NewVersionUrl = string.Empty;
            }

            return result;
        }

    }
}
