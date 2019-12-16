using System;
namespace OvoGitTest.Helpers
{
    public static class GitExtensions
    {
        internal static Models.Repository ConvertToRepository(this Atlassian.Stash.Entities.Repository stashRepo)
        {
            var cloneUrl = "";
            var linkUrl = "";
            foreach (var clone in stashRepo.Links.Clone)
            {
                if (clone.Name.ToLower().Equals("http")) cloneUrl = clone.Href.AbsoluteUri;
            }
            foreach (var self in stashRepo.Links.Self)
            {
                if (self.Href.AbsoluteUri.ToLower().Contains("http")) linkUrl = self.Href.AbsoluteUri;
            }

            var retVal = new Models.Repository
            {
                Name = stashRepo.Name,
                CloneUrl = cloneUrl,
                Forkable = stashRepo.Forkable,
                Id = stashRepo.Id,
                Project = stashRepo.Project.ConvertToProject(),
                Public = stashRepo.Public,
                ScmId = stashRepo.ScmId,
                Slug = stashRepo.Slug,
                State = stashRepo.State,
                StatusMessage = stashRepo.StatusMessage,
                Url = linkUrl
            };


            return retVal;
        }

        internal static Models.Project ConvertToProject(this Atlassian.Stash.Entities.Project stashProject)
        {
            var linkUrl = "";
            foreach (var self in stashProject.Links.Self)
            {
                if (self.Href.AbsoluteUri.ToLower().Contains("http")) linkUrl = self.Href.AbsoluteUri;
            }
            var retVal = new Models.Project
            {
                Description = stashProject.Description,
                Id = stashProject.Id,
                Key = stashProject.Key,
                Url = linkUrl,
                Name = stashProject.Name,
                Public = stashProject.Public,
                Type = stashProject.Type
            };
            return retVal;
        }
    }
}
