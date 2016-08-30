using CodeValue.VSCopyLocal.OptionsPages;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using VSUtils;
using VSUtils.Project;

namespace CodeValue.VSCopyLocal
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidVsPackageTemplatePkgString)]
    [ProvideOptionPage(typeof(OptionsPage), "VSCopyLocal", "Settings", 0, 0, true)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")] //SolutionExists
    public sealed class VSCopyLocalPackage : Package
    {
        private static DTE _dte;
        private static OptionsPage _options;

        private const string CActionTextFormat = "Turn {0} Copy Local";
        private const string CProjectActionTextFormat = CActionTextFormat + " for this Project";
        private const string CSolutionActionTextFormat = CActionTextFormat + " for this Solution";

        public string Name
        {
            get { return "VSCopyLocal"; }
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            // Solution level command
            var menuCommandId = new CommandID(GuidList.GuidVSCopyLocalCmdSolutionSet, (int)PkgCmdIDList.CmdidSolutionCommand);
            var menuItem = new OleMenuCommand(SolutionMenuItemCallback, menuCommandId);
            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;
            mcs.AddCommand(menuItem);

            // Project level command
            menuCommandId = new CommandID(GuidList.GuidVSCopyLocalCmdSet, (int) PkgCmdIDList.CmdidProjectCommand);
            menuItem = new OleMenuCommand(ProjectMenuItemCallback, menuCommandId);
            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;
            mcs.AddCommand(menuItem);

            // References level command
            menuCommandId = new CommandID(GuidList.GuidVSCopyLocalCmdReferencesSet, (int) PkgCmdIDList.CmdidReferencesCommand);
            menuItem = new OleMenuCommand(ReferencesMenuItemCallback, menuCommandId);
            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;
            mcs.AddCommand(menuItem);
            
            _dte = (DTE)GetGlobalService(typeof(DTE));

            _options = GetDialogPage(typeof(OptionsPage))as OptionsPage;
        }

        private static void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            UpdateLabelText(command);
        }

        private static void ProjectMenuItemCallback(object sender, EventArgs e)
        {
            SetCopyLocalForProject();
        }

        private static void ReferencesMenuItemCallback(object sender, EventArgs e)
        {
            SetCopyLocalForProject();
        }

        private static void SolutionMenuItemCallback(object sender, EventArgs e)
        {
            SetCopyLocalForSolution();
        }

        /// <summary>
        /// Set Copy Local for the active project.
        /// </summary>
        private static void SetCopyLocalForProject()
        {
            var projects = (Array) _dte.ActiveSolutionProjects;
            var activeProject = (Project) projects.GetValue(0);
            if (!_options.Skip(activeProject.Name))
            {
                int changeCount = ReferencesHelper.SetCopyLocalFlag(activeProject, _options.CopyLocalFlag,
                                                                    _options.PreviewMode);
                SaveProjectIfNeeded(activeProject);
                LogChangesToOutput(changeCount);
            }
            else
            {
                Common.WriteToDTEOutput(_dte, string.Format(
                    "'{0}' was skipped from processing (Set in Tools -> Options -> VSCopyLocal).",
                    activeProject.Name));
            }
        }

        /// <summary>
        /// Set Copy Local for the current solution.
        /// </summary>
        private static void SetCopyLocalForSolution()
        {
            var projects = SolutionHelper.GetProjects(_dte);
            int changeCount = 0;

            foreach (var project in projects.Where(project => !_options.Skip(project.Name)))
            {
                changeCount += ReferencesHelper.SetCopyLocalFlag(project, _options.CopyLocalFlag, _options.PreviewMode);
                if (!_options.PreviewMode)
                    SaveProjectIfNeeded(project);
            }

            LogChangesToOutput(changeCount);
        }

        private static void LogChangesToOutput(int changeCount)
        {
            var msg = changeCount > 0
                          ? string.Format(CultureInfo.CurrentCulture,
                                          "Copy Local set to {0} in {1} references{2}.", _options.CopyLocalFlag, changeCount,
                                          _options.PreviewMode ? " (Preview)" : string.Empty)
                          : "No Copy Local references found to set.";
            
            Common.WriteToDTEOutput(_dte, msg);
        }

        private static void SaveProjectIfNeeded(Project project)
        {
            if (!project.IsDirty) return;

            project.Save();
            Common.WriteToDTEOutput(_dte, string.Format("'{0}' saved.", project.Name));
        }

        private static void UpdateLabelText(OleMenuCommand cmd)
        {
            if (cmd == null) return;

            var action = _options.CopyLocalFlag ? "On" : "Off";

            if (cmd.CommandID.ID.Equals((int)PkgCmdIDList.CmdidSolutionCommand)) // solution node
            {
                cmd.Text = string.Format(CSolutionActionTextFormat, action);
            }
            if (cmd.CommandID.ID.Equals((int)PkgCmdIDList.CmdidProjectCommand)) // project node
            {
                cmd.Text = string.Format(CProjectActionTextFormat, action);
            }
            if (cmd.CommandID.ID.Equals((int)PkgCmdIDList.CmdidReferencesCommand)) // references node
            {
                cmd.Text = string.Format(CActionTextFormat, action);
            }
        }
    }

    /// <summary>
    /// VSCopyLocalPackageExtensions
    /// </summary>
    public static class VSCopyLocalPackageExtensions
    {
        /// <summary>
        /// Returns a filtered list of projects.
        /// </summary>
        public static IEnumerable<Project> Filtered(this IList<Project> projects, IEnumerable<string> projectsToSkipList)
        {
            var skipList = projectsToSkipList.ToList();
            var filteredProjects = new List<Project>();

            foreach (var s in skipList)
            {
                bool wildCardStart = s.StartsWith("*");
                bool wildCardEnd = s.EndsWith("*");

                var p = s.Trim('*').ToLowerInvariant();

                if (wildCardStart && wildCardEnd)
                    filteredProjects.AddRange(projects.Where(proj => !proj.Name.ToLowerInvariant().Contains(p)));
                else if (wildCardStart)
                    filteredProjects.AddRange(projects.Where(proj => proj.Name.ToLowerInvariant().EndsWith(p)));
                else if (wildCardEnd)
                    filteredProjects.AddRange(projects.Where(proj => proj.Name.ToLowerInvariant().StartsWith(p)));
                else
                    filteredProjects.AddRange(projects.Where(proj => proj.Name.ToLowerInvariant().Equals(p)));
            }

            return projects.Except(filteredProjects);
        }
    }
}
