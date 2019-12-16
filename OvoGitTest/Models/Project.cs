using System;
namespace OvoGitTest.Models
{
    internal class Project
    {
        public string Description { get; set; }
        public int Id { get; set; }

        public string Key { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }
        public string Type { get; set; }
    }
}
