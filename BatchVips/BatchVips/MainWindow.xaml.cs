using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using Ookii.Dialogs;
using System.Diagnostics;
using System.IO;

namespace BatchVips
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isCurrentlyRunning = false;
        string sourcePath = string.Empty;
        string destPath = string.Empty;
        string defaultVIPSPath = @"Z:\Pat\NZ Brain Scans\Software\vips-dev-8.1.0\vips-dev-8.1.0\bin\vips.exe";
        int fileCount = 0;
        int currentFile = 0;
        string[] fileNames = null;

        public MainWindow()
        {
            InitializeComponent();

            VIPSTextBlock.Text = defaultVIPSPath;
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "Please select a folder where your TIF images are located.";
            dlg.UseDescriptionForTitle = true;
            bool? dialogSuccess = dlg.ShowDialog();

            if (dialogSuccess.HasValue)
            {
                if (dialogSuccess.Value)
                {
                    sourcePath = dlg.SelectedPath;
                }
            }

            //Update the GUI
            SourceTextBox.Text = sourcePath;
        }

        private void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "Please select a folder where you would like to save the DeepZoom images.";
            dlg.UseDescriptionForTitle = true;
            bool? dialogSuccess = dlg.ShowDialog();

            if (dialogSuccess.HasValue)
            {
                if (dialogSuccess.Value)
                {
                    destPath = dlg.SelectedPath;
                }
            }

            //Update the GUI
            DestinationTextBox.Text = destPath;
        }

        private void FindVIPSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executable files (*.exe)|*.exe";

            bool? dlgSuccess = dlg.ShowDialog();

            if (dlgSuccess.HasValue)
            {
                if (dlgSuccess.Value)
                {
                    defaultVIPSPath = dlg.FileName;
                    VIPSTextBlock.Text = defaultVIPSPath;
                }
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCurrentlyRunning)
            {
                isCurrentlyRunning = false;
                GoButton.Content = "Go!";
                GoButton.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                fileNames = System.IO.Directory.GetFiles(sourcePath, "*.tif");
                fileCount = fileNames.Length;
                currentFile = 0;

                isCurrentlyRunning = true;
                GoButton.Content = "Stop!";
                GoButton.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                VipsProgressBar.Minimum = 0;
                VipsProgressBar.Maximum = fileCount;

                RunNextVIPSProcess();
            }
        }

        private void RunNextVIPSProcess ()
        {
            if (isCurrentlyRunning && currentFile < fileCount)
            {
                //VipsProgressBar.Value = currentFile;

                int i = currentFile;
                currentFile++;

                //ProgressBarText.Text = "File " + currentFile.ToString() + "/" + fileCount.ToString();

                
                string s = fileNames[i];
                string filename_only = System.IO.Path.GetFileName(s);
                string dest_full_path = destPath + "/" + filename_only;
                string args = "dzsave " + "\"" + s + "\"" + " " +
                    "\"" + dest_full_path + "\"" + " " +
                    "--properties --vips-progress";
                //BigTextBox.Text += defaultVIPSPath + " " + args + "\n";

                Process p = new Process();
                p.StartInfo.FileName = defaultVIPSPath;
                p.StartInfo.Arguments = args;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.EnableRaisingEvents = true;
                p.Exited += p_Exited;
                p.Start();

                //Process p = Process.Start(defaultVIPSPath, args);
                
            }
            else
            {
                isCurrentlyRunning = false;
                //GoButton.Content = "Go!";
                //GoButton.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //VipsProgressBar.Value = fileCount;
            }
        }

        void p_Exited(object sender, EventArgs e)
        {
            RunNextVIPSProcess();
        }

    }
}
