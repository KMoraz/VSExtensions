using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Linq;

namespace VSUtils.Project
{
    public class SolutionHelper
    {
        private static IEnumerable<EnvDTE.Project> GetSolutionFolderProjects(EnvDTE.Project solutionFolder)
        {
            var list = new List<EnvDTE.Project>();

            foreach (var subProject in solutionFolder.ProjectItems.Cast<ProjectItem>().Select(projectItem => projectItem.SubProject).Where(subProject => subProject != null))
            {
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    list.Add(subProject);
                }
            }

            return list;
        }

        public static IList<EnvDTE.Project> GetProjects(DTE dte)
        {
            var projects = dte.Solution.Projects;
            var list = new List<EnvDTE.Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as EnvDTE.Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(project));
                }
                else
                {
                    list.Add(project);
                }
            }

            return list;
        }

        public static EnvDTE.Project FindProject(_DTE dte, string path, string uniqueName)
        {
            return dte.Solution.Projects.Cast<EnvDTE.Project>().FirstOrDefault(proj => proj.FullName == path);
        }
    }
}
