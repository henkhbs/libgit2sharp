using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Atlassian.Stash;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Repository = LibGit2Sharp.Repository;
using static System.Environment;
namespace OvoGitTest.Helpers
{
    internal class GitClient
    {


        private StashClient getStashClient(string personalAccessKey)
        {
            var client = new StashClient("https://repo.ovotrack.nl", personalAccessKey, true);
            return client;
        }
        private CloneOptions getCloneOptions(string personalAccessKey)
        {
            var co = new CloneOptions
            {
                FetchOptions = new FetchOptions { CustomHeaders = new[] { "Authorization: Bearer " + personalAccessKey } }
            };
            return co;

        }

        private FetchOptions getFetchOptions(string personalAccessKey)
        {

            var fo = new FetchOptions()
            {
                CustomHeaders = new[] { "Authorization: Bearer " + personalAccessKey }

            };
            return fo;
        }

        private PushOptions getPushOptions(string personalAccessKey)
        {

            var po = new PushOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "x-token-auth:" + personalAccessKey, Password = "" }

            };
            return po;
        }

        public Models.Repository GetRepo(string projectName, string repoName, string personalAccessKey)
        {

            var stashRepo = getStashClient(personalAccessKey).Repositories.GetById(projectName, repoName).Result;

            var retVal = stashRepo.ConvertToRepository();
            return retVal;
        }

        public Models.Project GetProject(string projectKey, string personalAccessKey)
        {
            var stashProject = getStashClient(personalAccessKey).Projects.GetById(projectKey).Result;
            var retVal = stashProject.ConvertToProject();
            return retVal;
        }
        public List<Models.Repository> GetDeploymentRepos(string personalAccessKey)
        {
            return GetProjectRepositories("ov4d", personalAccessKey);
        }

        public List<Models.Repository> GetProjectRepositories(string projectKey, string personalAccessKey)
        {
            var stashRepos = getStashClient(personalAccessKey).Repositories.Get(projectKey);
            return stashRepos.Result.Values.Select(stashRepo => stashRepo.ConvertToRepository()).ToList();
        }
        public List<Models.Project> GetGitProjects(string personalAccessKey)
        {
            var stashProjects = getStashClient(personalAccessKey).Projects.Get();
            return stashProjects.Result.Values.Select(project => project.ConvertToProject()).ToList();
        }

        private string GetLocalRepoFolder(Models.Repository repo, string personalAccessKey)
        {
            var checkOutFolder = GetLocalFolder(repo);
            string logMessage = "";
            if (Repository.IsValid(checkOutFolder) == false)
            {
                checkOutFolder = Repository.Clone(repo.CloneUrl, checkOutFolder, getCloneOptions(personalAccessKey));
            }
            else
            {
                using (var tmpRepo = new Repository(checkOutFolder))
                {
                    var remote = tmpRepo.Network.Remotes["origin"];
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);

                    Commands.Fetch(tmpRepo, remote.Name, refSpecs, getFetchOptions(personalAccessKey), logMessage);
                }

            }

            return checkOutFolder;
        }

        private void CommitFileToRepo(string fileName, Models.Repository repo, string personalAccessKey)
        {
            using (var gitRepo = new Repository(GetLocalFolder(repo)))
            {
                gitRepo.Index.Add(fileName);
                gitRepo.Index.Write();
                // Create the committer's signature and commit
                Signature author = new Signature("Webservice", "support@ovotrack.nl", DateTime.Now);
                Signature committer = author;
                Commit commit = gitRepo.Commit("Modified " + fileName + " by webservice", author, committer);
                foreach (var gitRepoBranch in gitRepo.Branches)
                {
                    gitRepo.Network.Push(gitRepoBranch, getPushOptions(personalAccessKey));
                }

            }
        }
        public string GetDockerComposeFileContent(Models.Repository repo, string personalAccessKey)
        {
            var checkOutFolder = GetLocalRepoFolder(repo, personalAccessKey);
            var dockerComposePath = checkOutFolder + "/docker-compose.yml";
            var dockerComposeContent = File.ReadAllText(dockerComposePath);

            return dockerComposeContent;

        }

        public void WriteDockerComposeFileContent(Models.Repository repo, string fileContent, string personalAccessKey)
        {
            var checkOutFolder = GetLocalRepoFolder(repo, personalAccessKey);
            var dockerComposePath = checkOutFolder + "/docker-compose.yml";

            try
            {
                File.WriteAllText(dockerComposePath, fileContent);
                CommitFileToRepo("docker-compose.yml", repo, personalAccessKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }

        public string GetEnvFileContent(Models.Repository repo, string personalAccessKey)
        {
            var checkOutFolder = GetLocalRepoFolder(repo, personalAccessKey);

            var envContent = File.ReadAllText(checkOutFolder + "/.env");

            return envContent;

        }

        public void WriteEnvFileContent(Models.Repository repo, string fileContent, string personalAccessKey)
        {
            var checkOutFolder = GetLocalRepoFolder(repo, personalAccessKey);
            var dockerComposePath = checkOutFolder + "/.env";

            try
            {
                File.WriteAllText(dockerComposePath, fileContent);
                CommitFileToRepo(".env", repo, personalAccessKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }

        private string GetLocalFolder(Models.Repository repo)
        {

            string appData = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "ovotrack_git", repo.Project.Key, repo.Slug);

            if (Directory.Exists(appData) == false)
            {
                Directory.CreateDirectory(appData);
            }

            return appData;
        }

        public Models.Repository GetDeploymentRepo(string repoName, string personalAccessKey)
        {
            var repos = GetDeploymentRepos(personalAccessKey);
            var depRepo = new Models.Repository();
            foreach (var repo in repos.Where(repo => repo.Name == repoName))
            {
                depRepo = repo;
            }

            if (depRepo.Name.Length != 0)
            {
                GetLocalRepoFolder(depRepo, personalAccessKey);
                return depRepo;
            }
            return null;
        }
    }
}
