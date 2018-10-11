namespace ChangesFinder
{
    using EnvDTE;
    using EnvDTE80;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Shell;
    using System.Reflection;
    using System.IO;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.PlatformUI;
    using System;
    using System.Windows.Threading;
    using Microsoft.VisualStudio;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for ChangesFinderWindowControl.
    /// </summary>
    public partial class OptionsDialog : DialogBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangesFinderWindowControl"/> class.
        /// </summary>
        public OptionsDialog()
        {
            this.InitializeComponent();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection =
                    (IVsMonitorSelection)Package.GetGlobalService(
                    typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                                                 out projectItemId,
                                                 out multiItemSelect,
                                                 out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                 hierarchyPointer,
                                                 typeof(IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }

            Project selectedProject = selectedObject as Project;

            string ProjectPath = System.IO.Path.GetDirectoryName(selectedProject.FullName);
            string ProjectName = selectedProject.Name;

            Logic.GetChanges(ProjectPath, TxtSavePath.Text, ProjectName, int.Parse(TxtDayCount.Text), ChkShowInExplorer.IsChecked.Value, ChkGetAll.IsChecked.Value, ChkMakeRar.IsChecked.Value, TxtExt.Text);
            Close();
        }

        private void DayDown_Click(object sender, RoutedEventArgs e)
        {
            int day = int.Parse(TxtDayCount.Text);
            if (day > 1)
                TxtDayCount.Text = (day - 1).ToString();
        }

        private void DayUp_Click(object sender, RoutedEventArgs e)
        {
            int day = int.Parse(TxtDayCount.Text);
            TxtDayCount.Text = (day + 1).ToString();
        }


        private void ChkGetAll_Click(object sender, RoutedEventArgs e)
        {
            TxtDayCount.IsEnabled = DayDown.IsEnabled = DayUp.IsEnabled = !ChkGetAll.IsChecked.Value;
        }
       




        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bits-developments.com");
        }

        private void ExtensionsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TxtExt.Visibility != Visibility.Visible)
            {
                TxtExt.Visibility = Visibility.Visible;
                Height += TxtExt.Height;
            }
            else
            {
                TxtExt.Visibility = Visibility.Hidden;
                Height -= TxtExt.Height;
            }

        }
    }
}