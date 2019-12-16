using System;
using OvoGitTest.Controllers;

namespace OvoGitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var gitctrl = new GitController();
            var data = gitctrl.GetDockerFileContent("dummy", "xxxxxxxx");
            Console.Write(data);
            data = "HalloDummy" + Environment.NewLine + data;
            gitctrl.UpdateDockerFileContent(data,"dummy", "xxxxxxxx");
        }
    }
}
