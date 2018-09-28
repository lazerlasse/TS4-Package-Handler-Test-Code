using s4pi.Interfaces;
using s4pi.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace The_Sims_4_Packet___Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ScanButton.IsEnabled = false;
        }

        // Declare Section...
        private string SourcePath;
        private int ProgressCurrentCounter = 0;
        private int ProgressMaxCounter = 0;
        private List<FileInfo> SourceFilesList = new List<FileInfo>();
        private List<FileInfo> ScriptFilesList = new List<FileInfo>();
        private List<FileInfo> PackageFilesList = new List<FileInfo>();
        private List<FileInfo> ErrorPackageList = new List<FileInfo>();
        private List<FileInfo> NotAcceptedFileTypes = new List<FileInfo>();
        private List<PackageDetailedViewList> DetailedPackageList = new List<PackageDetailedViewList>();
		public List<IResourceKey> resourceKeys = new List<IResourceKey>();

		// Browse button click...
		private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset FileViewer and FileInfo List´s before new job...
            FileViewerDataGrid.Items.Clear();
            SourceFilesList.Clear();
            ScriptFilesList.Clear();
            PackageFilesList.Clear();
            ErrorPackageList.Clear();
            DetailedPackageList.Clear();

            // Create directory-browser dialog...
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            // Check browser dialog result..
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Get source patch and update patch textbox...
                SourcePath = dialog.SelectedPath;
                SourcePatchTextView.Text = SourcePath;
                ScanButton.IsEnabled = true;
            }
        }

        // ScanButton click eventhandler....
        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            // Create backgroudworker for directory processing...
            BackgroundWorker ProcessDirectoryWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            ProcessDirectoryWorker.DoWork += ProcessDirectory_DoWork;
            ProcessDirectoryWorker.ProgressChanged += ProcessDirectory_ProgressChanged;
            ProcessDirectoryWorker.RunWorkerCompleted += ProcessDirectory_RunWorkerCompleted;
            ProcessDirectoryWorker.RunWorkerAsync(1000);
        }

        private void ProcessDirectory_DoWork(object sender, DoWorkEventArgs e)
        {
            // Reset counters and progress bar...
            ProgressCurrentCounter = 0;
            ProgressMaxCounter = 0;

            // Check selected path for files and index them...
            try
            {
                // Add all files from selected patch and subfolders to FileInfo List...
                SourceFilesList = new DirectoryInfo(SourcePath).GetFiles("*.*", SearchOption.AllDirectories).ToList();

                // Count files for the progress calculation...
                ProgressMaxCounter = SourceFilesList.Count;

                // Index files and resources...
                foreach (FileInfo file in SourceFilesList)
                {
                    if ((file.Attributes & FileAttributes.Directory) != 0) continue;
                    {
                        // Index all Sims Package files...
                        if (file.Extension == ".package" | file.Extension == ".sim3pack")
                        {
							IPackage package = Package.OpenPackage(1, file.FullName);
							PackageFilesList.Add(file);
                            ProgressCurrentCounter++;

							PackageProgress progress = new PackageProgress()
							{
								Package = package,
								FileInfos = file,
								Conflict = "Nej"
							};

                            (sender as BackgroundWorker).ReportProgress(ProgressCurrentCounter, progress);
                        }

                        // Index all Sims 4 script files and count...
                        else if (file.Extension == ".ts4script")
                        {
                            ScriptFilesList.Add(file);
                            ProgressCurrentCounter++;
                            (sender as BackgroundWorker).ReportProgress(ProgressCurrentCounter,
                                new PackageProgress()
                                {
                                    FileInfos = file,
									Conflict = "Nej"
                                });
                        }

                        // Index all not accepted files...
                        else
                        {
                            NotAcceptedFileTypes.Add(file);
                            ProgressCurrentCounter++;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Der opstod en uventet fejl!");
            }
        }

        // Directory processing progress change events...
        private void ProcessDirectory_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Convert UserStatet and calculate progress...
            PackageProgress progress = e.UserState as PackageProgress;
            double ProgressValue = (((double)e.ProgressPercentage / ProgressMaxCounter) * 100);

            // Send progress to ProgressBar...
            UpdateProgressBar1(ProgressValue);

            // Update Progress TextLabel...
            UpdateProgressTextLabel("Processing: " + progress.FileInfos.Name);

			List<IResourceIndexEntry> indexEntries = progress.Package.GetResourceList;

			// Update the fileviewer...
			foreach (var resource in indexEntries)
			{
				string[] TGI = resource.ToString().Split('-');
				UpdatePackageView(new PackageViewList()
				{
					PackageName = Path.GetFileNameWithoutExtension(progress.FileInfos.Name),
					PackageType = progress.FileInfos.Extension,
					PackageCreated = progress.FileInfos.CreationTime,
					PackageEdited = progress.FileInfos.LastWriteTime,
					ResourceTag = resource.ToString(),
					ResourceType = TGI[0],
					ResourceGroup = TGI[1],
					ResourceInstance = TGI[2],
					ConflictDetected = progress.Conflict
				});
			}
            
        }

        // Directory processing work completed events....
        private void ProcessDirectory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Der blev fundet "
                + NotAcceptedFileTypes.Count.ToString()
                + " ikke accepteret filer...",
                "Der blev fundet filer");

            ScanButton.Content = "Check Mods";
            ScanButton.IsEnabled = true;
            UpdateProgressTextLabel("Klar...");
            UpdateProgressBar1(0);
            SourcePath = null;
            SourcePatchTextView.Text = "Vælg den mappe du vil importere dine mods fra.";
        }

        /*/
         * ################################################################################################
         * ############################ User interface update section #####################################
         * ################################################################################################
         /*/

        

        /*/
         * ################################################################################################
         * ############################ User interface update section #####################################
         * ################################################################################################
         /*/

        // Update log viewer text.
        private void UpdatePackageView(PackageViewList processOutput)
        {
            FileViewerDataGrid.Items.Add(processOutput);
        }

        // Update ProgressBar1...
        private void UpdateProgressBar1(double Value)
        {
            ProgressBar1.Value = Value;
        }

        // Update Progress Text Label...
        private void UpdateProgressTextLabel(string ProgressText)
        {
            ProgressTextBox.Content = ProgressText;
        }
    }
}
