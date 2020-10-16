using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Lanymy.Common;
using Lanymy.Common.ExtensionFunctions;
using Microsoft.Win32;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PushNugetPackageTools.Models;
using Path = System.IO.Path;

namespace PushNugetPackageTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private NupkgsInfoModel _CurrentNupkgsInfoModel;
        private readonly ObservableCollection<string> _NupkgFullPathSourceList = new ObservableCollection<string>();
        private readonly ObservableCollection<NupkgItemInfoViewModel> _NupkgsSourceList = new ObservableCollection<NupkgItemInfoViewModel>();

        private string _NupkgPublishKeyTemp;
        private string _NugetServerUrlTemp;

        private NuGetServerManipulater _CurrentNuGetServerManipulater;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            _CurrentNupkgsInfoModel.NupkgFullPathList = _NupkgFullPathSourceList.ToList();

            IsolatedStorageHelper.SaveModel(_CurrentNupkgsInfoModel);

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //判断网络是否可用
            //NetworkInterface.GetIsNetworkAvailable()

            _CurrentNupkgsInfoModel = IsolatedStorageHelper.GetModel<NupkgsInfoModel>();

            if (_CurrentNupkgsInfoModel.IfIsNullOrEmpty())
            {
                _CurrentNupkgsInfoModel = new NupkgsInfoModel();
            }
            else
            {

                foreach (var nupkgFullPathItem in _CurrentNupkgsInfoModel.NupkgFullPathList)
                {

                    _NupkgFullPathSourceList.Add(nupkgFullPathItem);

                }

                if (!_CurrentNupkgsInfoModel.NugetServerUrl.IfIsNullOrEmpty())
                {
                    tbxNugetServerUrl.Text = _CurrentNupkgsInfoModel.NugetServerUrl;
                }

                if (!_CurrentNupkgsInfoModel.NupkgPublishKey.IfIsNullOrEmpty())
                {
                    tbxNupkgKey.Text = _CurrentNupkgsInfoModel.NupkgPublishKey;
                }



            }

            //_NupkgFullPathSourceList.Add(@"E:\VS Project\Github\Lanymy\Lanymy.NET\src\Commons\Lanymy.Common\bin\Debug");
            //_NupkgFullPathSourceList.Add(@"E:\VS Project\Github\Lanymy\Lanymy.NET\src\Common.Clients\Lanymy.Common.Console\bin\Debug");

            lbxNupkgFullPath.ItemsSource = _NupkgFullPathSourceList;
            lbxNupkgs.ItemsSource = _NupkgsSourceList;
            lbxNupkgs.DisplayMemberPath = "DisplayTitle";

            _CurrentNuGetServerManipulater = new NuGetServerManipulater(_CurrentNupkgsInfoModel.NupkgPublishKey, _CurrentNupkgsInfoModel.NugetServerUrl);

        }

        private void SetNuGetConfigControlsIsEnabledStatus(bool isSettings)
        {
            tbxNugetServerUrl.IsEnabled = isSettings;
            tbxNupkgKey.IsEnabled = isSettings;
            btnSetNuGetConfig.IsEnabled = !isSettings;
            btnSaveNuGetConfig.IsEnabled = isSettings;
            btnCancelNuGetConfig.IsEnabled = isSettings;
        }

        private void BtnSetNuGetConfig_OnClick(object sender, RoutedEventArgs e)
        {

            SetNuGetConfigControlsIsEnabledStatus(true);

            _NugetServerUrlTemp = tbxNugetServerUrl.Text.Trim();
            _NupkgPublishKeyTemp = tbxNupkgKey.Text.Trim();

        }

        private void BtnSaveNuGetConfig_OnClick(object sender, RoutedEventArgs e)
        {

            SetNuGetConfigControlsIsEnabledStatus(false);

            _CurrentNupkgsInfoModel.NugetServerUrl = tbxNugetServerUrl.Text.Trim();
            _CurrentNupkgsInfoModel.NupkgPublishKey = tbxNupkgKey.Text.Trim();

            _CurrentNuGetServerManipulater = new NuGetServerManipulater(_CurrentNupkgsInfoModel.NupkgPublishKey, _CurrentNupkgsInfoModel.NugetServerUrl);

        }

        private void BtnCancelNuGetConfig_OnClick(object sender, RoutedEventArgs e)
        {
            SetNuGetConfigControlsIsEnabledStatus(false);
            tbxNugetServerUrl.Text = _NugetServerUrlTemp;
            tbxNupkgKey.Text = _NupkgPublishKeyTemp;
        }




        private void btnNupkgFullPath_Click(object sender, RoutedEventArgs e)
        {


            var openFileDialog = new OpenFileDialog
            {
                Title = "选择要发布的包",
                Filter = "Nuget包|" + "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX,
                FileName = string.Empty,
                Multiselect = false,
                RestoreDirectory = true
            };
            //openFileDialog.Filter = "txt文件|*.txt|rar文件|*.rar|所有文件|*.*";
            //openFileDialog.FilterIndex = 1;

            if ((bool)openFileDialog.ShowDialog())
            {

                string nupkgFileFullPath = openFileDialog.FileName;
                tbxAddNupkgFullPath.Text = Path.GetDirectoryName(nupkgFileFullPath);

            }


        }

        private void btnAddNupkgFullPath_Click(object sender, RoutedEventArgs e)
        {

            var addNupkgFullPath = tbxAddNupkgFullPath.Text.Trim();

            var addNupkgFullPathLower = addNupkgFullPath.ToLower();
            var item = _NupkgFullPathSourceList.Where(o => o.ToLower() == addNupkgFullPathLower).FirstOrDefault();

            if (item.IfIsNullOrEmpty())
            {

                _NupkgFullPathSourceList.Add(addNupkgFullPath);

                var list = _NupkgFullPathSourceList.OrderBy(o => o).ToList();

                _NupkgFullPathSourceList.Clear();

                foreach (var nupkgFullPathSourceItem in list)
                {
                    _NupkgFullPathSourceList.Add(nupkgFullPathSourceItem);
                }

            }



        }

        private void btnRemoveNupkgFullPath_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lbxNupkgFullPath.SelectedItem;
            if (!selectedItem.IfIsNullOrEmpty())
            {
                _NupkgFullPathSourceList.Remove(selectedItem as string);
            }
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            btnUnSelectAll_Click(null, null);

            foreach (var nupkgItemInfoModel in _NupkgsSourceList)
            {
                lbxNupkgs.SelectedItems.Add(nupkgItemInfoModel);
            }

        }

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            lbxNupkgs.SelectedItems.Clear();

        }


        private void LbxNupkgs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (var addedItem in e.AddedItems)
            {

                var nupkgItemInfoModel = addedItem as NupkgItemInfoViewModel;
                if (!nupkgItemInfoModel.IsEnable)
                {
                    lbxNupkgs.SelectedItems.Remove(nupkgItemInfoModel);
                }

            }

        }


        private async void btnRefreshNupkgs_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //var nupkgItemInfoModelA = new NupkgItemInfoModel
                //{
                //    ID = "Lanymy.Common",
                //    FileFullName = "Lanymy.Common.0.2.1.nupkg",
                //    FileFullPath = "Lanymy.Common.0.2.1.nupkg",
                //    Version = new NuGetVersion(0, 2, 3),
                //    RemoteVersion = new NuGetVersion(0, 2, 2),
                //};
                //var nupkgItemInfoModelB = new NupkgItemInfoModel
                //{
                //    ID = "Lanymy.Common.Console",
                //    FileFullName = "Lanymy.Common.Console.0.1.1.nupkg",
                //    FileFullPath = "Lanymy.Common.Console.0.1.1.nupkg",
                //    Version = new NuGetVersion(0, 1, 1),
                //    RemoteVersion = new NuGetVersion(0, 1, 1),
                //};

                //_NupkgsSourceList.Add(nupkgItemInfoModelA);
                //_NupkgsSourceList.Add(nupkgItemInfoModelB);

                ////var json = "[{\"DisplayTitle\":\"[ Lanymy.Common.0.2.1.nupkg ] [ Lanymy.Common ] [ 0.2.1 ] [ 0.2.2 ]\",\"ID\":\"Lanymy.Common\",\"FileFullName\":\"Lanymy.Common.0.2.1.nupkg\",\"FileFullPath\":\"E:\\\\VS Project\\\\Github\\\\Lanymy\\\\Lanymy.NET\\\\src\\\\Commons\\\\Lanymy.Common\\\\bin\\\\Debug\\\\Lanymy.Common.0.2.1.nupkg\",\"Version\":{\"Version\":\"0.2.1.0\",\"IsLegacyVersion\":false,\"Revision\":0,\"IsSemVer2\":false,\"OriginalVersion\":\"0.2.1\",\"Major\":0,\"Minor\":2,\"Patch\":1,\"ReleaseLabels\":[],\"Release\":\"\",\"IsPrerelease\":false,\"HasMetadata\":false,\"Metadata\":\"\"},\"RemoteVersion\":{\"Version\":\"0.2.2.0\",\"IsLegacyVersion\":false,\"Revision\":0,\"IsSemVer2\":false,\"OriginalVersion\":\"0.2.2\",\"Major\":0,\"Minor\":2,\"Patch\":2,\"ReleaseLabels\":[],\"Release\":\"\",\"IsPrerelease\":false,\"HasMetadata\":false,\"Metadata\":\"\"},\"IsEnable\":false,\"IsChecked\":false},{\"DisplayTitle\":\"[ Lanymy.Common.Console.0.1.1.nupkg ] [ Lanymy.Common.Console ] [ 0.1.1 ] [ 0.1.1 ]\",\"ID\":\"Lanymy.Common.Console\",\"FileFullName\":\"Lanymy.Common.Console.0.1.1.nupkg\",\"FileFullPath\":\"E:\\\\VS Project\\\\Github\\\\Lanymy\\\\Lanymy.NET\\\\src\\\\Common.Clients\\\\Lanymy.Common.Console\\\\bin\\\\Debug\\\\Lanymy.Common.Console.0.1.1.nupkg\",\"Version\":{\"Version\":\"0.1.1.0\",\"IsLegacyVersion\":false,\"Revision\":0,\"IsSemVer2\":false,\"OriginalVersion\":\"0.1.1\",\"Major\":0,\"Minor\":1,\"Patch\":1,\"ReleaseLabels\":[],\"Release\":\"\",\"IsPrerelease\":false,\"HasMetadata\":false,\"Metadata\":\"\"},\"RemoteVersion\":{\"Version\":\"0.1.1.0\",\"IsLegacyVersion\":false,\"Revision\":0,\"IsSemVer2\":false,\"OriginalVersion\":\"0.1.1\",\"Major\":0,\"Minor\":1,\"Patch\":1,\"ReleaseLabels\":[],\"Release\":\"\",\"IsPrerelease\":false,\"HasMetadata\":false,\"Metadata\":\"\"},\"IsEnable\":false,\"IsChecked\":false}]";
                ////var list = SerializeHelper.DeserializeFromJson<List<NupkgItemInfoModel>>(json);
                ////foreach (var nupkgItemInfoModel in list)
                ////{
                ////    _NupkgsSourceList.Add(nupkgItemInfoModel);
                ////}

                //return;

                foreach (var nupkgFullPath in _NupkgFullPathSourceList)
                {

                    var lastNupkgFileinfo = Directory.GetFiles(nupkgFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.TopDirectoryOnly).Select(o => new FileInfo(o)).OrderByDescending(o => o.CreationTimeUtc).FirstOrDefault();
                    if (!lastNupkgFileinfo.IfIsNullOrEmpty())
                    {

                        //var tempFile = Path.GetTempFileName();
                        //File.Copy(packagePath, tempFile, overwrite: true);

                        var nupkgFileFullPath = lastNupkgFileinfo.FullName;
                        var nugetPackageMetadataInfoModel = new NugetPackageMetadataInfoModel(nupkgFileFullPath);

                        //var remoteNugetPackageMetadata = await NuGetServerHelper.GetNuGetPackageMetadataByIdAsync(nugetPackageMetadataInfoModel.ID);
                        var remoteNugetPackageMetadata = await _CurrentNuGetServerManipulater.GetNuGetPackageMetadataByIdAsync(nugetPackageMetadataInfoModel.ID);

                        var remoteVersion = remoteNugetPackageMetadata.IfIsNullOrEmpty() ? new NuGetVersion("0") : remoteNugetPackageMetadata.Identity.Version;


                        var nupkgItemInfoModel = new NupkgItemInfoViewModel
                        {
                            ID = nugetPackageMetadataInfoModel.ID,
                            FileFullName = nugetPackageMetadataInfoModel.NugetPackageFileFullName,
                            FileFullPath = nugetPackageMetadataInfoModel.NugetPackageFileFullPath,
                            Version = nugetPackageMetadataInfoModel.Version,
                            RemoteVersion = remoteVersion,
                        };


                        _NupkgsSourceList.Add(nupkgItemInfoModel);





                        //lbxNupkgs.SelectedItem = nupkgItemInfoModel;

                        //var scheduleFileInfoModel = new ScheduleFileInfoModel
                        //{
                        //    SourceFileFullPath = lastNupkgFileinfo.FullName,
                        //    TargetFileFullPath = Path.Combine(GlobalSettings.NuGetPackagesFolderFullPath, lastNupkgFileinfo.Name),
                        //};

                        //logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(scheduleFileInfoModel.SourceFileFullPath), scheduleFileInfoModel.SourceFileFullPath));
                        //logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(scheduleFileInfoModel.TargetFileFullPath), scheduleFileInfoModel.TargetFileFullPath));

                        //File.Copy(scheduleFileInfoModel.SourceFileFullPath, scheduleFileInfoModel.TargetFileFullPath, true);

                    }
                }

                var list = _NupkgsSourceList.OrderBy(o => o.ID).ToList();

                _NupkgsSourceList.Clear();

                foreach (var nupkgItemInfoModel in list)
                {
                    _NupkgsSourceList.Add(nupkgItemInfoModel);
                }

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.ToString());
            }

        }

        private async void btnPublishNupkgs_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (lbxNupkgs.SelectedItems.Count <= 0 || tbxNupkgKey.Text.Trim().IfIsNullOrEmpty())
                {
                    return;
                }

                var publishKey = tbxNupkgKey.Text.Trim();

                foreach (var lbxNupkgsSelectedItem in lbxNupkgs.SelectedItems)
                {

                    var nupkgItemInfoModel = lbxNupkgsSelectedItem as NupkgItemInfoViewModel;

                    if (nupkgItemInfoModel.IsEnable)
                    {

                        //await NuGetServerHelper.PublishNuGetPackage(nupkgItemInfoModel.FileFullPath, publishKey);
                        await _CurrentNuGetServerManipulater.PublishNuGetPackage(nupkgItemInfoModel.FileFullPath, publishKey);

                    }


                }

                _CurrentNupkgsInfoModel.NupkgPublishKey = publishKey;

                MessageBox.Show("发布完成");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

        }


    }
}
