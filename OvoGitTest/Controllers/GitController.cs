using System;
namespace OvoGitTest.Controllers
{
    public class GitController
    {
        public string GetDockerFileContent(string customer, string personalAccessKey)
        {
            var gitClient = new GitClient();
            var repo = gitClient.GetDeploymentRepo(customer, personalAccessKey);
            var retVal = gitClient.GetDockerComposeFileContent(repo, personalAccessKey);

            return retVal;
        }

        public void UpdateDockerFileContent(string fileContent, string customer, string personalAccessKey)
        {
            var gitClient = new GitClient();
            var repo = gitClient.GetDeploymentRepo(customer, personalAccessKey);
            gitClient.WriteDockerComposeFileContent(repo, fileContent, personalAccessKey);
        }
        public void UpdateEnvFileContent(string fileContent, string customer, string personalAccessKey)
        {
            var gitClient = new GitClient();
            var repo = gitClient.GetDeploymentRepo(customer, personalAccessKey);
            gitClient.WriteEnvFileContent(repo, fileContent, personalAccessKey);
        }

        public string CreateNewDeploymentRepo(string customer, string personalAccessKey)
        {
            var retVal = "";
            return retVal;
        }
        public string GetEnvFileContent(string customer, string personalAccessKey)
        {
            var gitClient = new GitClient();
            var repo = gitClient.GetDeploymentRepo(customer, personalAccessKey);
            var retVal = gitClient.GetEnvFileContent(repo, personalAccessKey);

            return retVal;
        }
    }
}
