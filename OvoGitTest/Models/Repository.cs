using System;
namespace OvoGitTest.Models
{
    internal class Repository
    {
        public string CloneUrl { get; set; }
        public bool Forkable { get; set; }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public Project Project { get; set; }

        public bool Public { get; set; }

        public string ScmId { get; set; }

        public string Slug { get; set; }
        public string State { get; set; }

        public string StatusMessage { get; set; }
    }
}
