using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using NuGet.Versioning;
using PushNugetPackageTools.Models;

namespace PushNugetPackageTools
{
    /// <summary>
    /// NewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly NuGetSettingConfigModel _CurrentNuGetSettingConfigModel = GlobalSettings.CurrentNuGetSettingConfigModel;

        private NuGetSettingInfoModel _CurrentNuGetSettingInfoModel = _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel;

        private readonly ObservableCollection<NuGetSettingInfoModel> _NameSourceList = new ObservableCollection<NuGetSettingInfoModel>();

        private readonly ObservableCollection<NupkgItemInfoViewModel> _NupkgsSourceList = new ObservableCollection<NupkgItemInfoViewModel>();

        private NuGetServerManipulater _CurrentNuGetServerManipulater;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += NewWindow_Loaded;
            Closing += NewWindow_Closing;
        }

        private void NewWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel = _CurrentNuGetSettingInfoModel;
            _CurrentNuGetSettingConfigModel.NuGetSettingList = new List<NuGetSettingInfoModel>(_NameSourceList);
            _CurrentNuGetSettingConfigModel.LastUpdateDateTime = DateTime.Now;
            GlobalSettings.SaveNuGetSettingConfigModel();
        }

        private void NewWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var version = VersionHelper.GetCallDomainAssemblyVersion();

            Title = string.Format("{0} - [ v{1} ]", Title, version);

            _NameSourceList.Clear();

            foreach (var nuGetSettingInfoModel in _CurrentNuGetSettingConfigModel.NuGetSettingList)
            {
                _NameSourceList.Add(nuGetSettingInfoModel);
            }

            cbxName.ItemsSource = _NameSourceList;

            var currentNuGetSettingInfoModel = _NameSourceList.Where(o => o.ID == _CurrentNuGetSettingInfoModel.ID).FirstOrDefault();
            if (!currentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {
                cbxName.SelectedItem = currentNuGetSettingInfoModel;
            }

            lbxNupkgs.ItemsSource = _NupkgsSourceList;
            lbxNupkgs.DisplayMemberPath = "DisplayTitle";

        }


        private void CbxName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            _CurrentNuGetServerManipulater = null;
            tbxNugetServerUrl.Text = string.Empty;
            tbxNupkgPublishKey.Text = string.Empty;
            tbxAllNupkgFileFullPath.Text = string.Empty;
            cbxClearOldNupkg.IsChecked = false;
            _NupkgsSourceList.Clear();


            _CurrentNuGetSettingInfoModel = cbxName.SelectedItem as NuGetSettingInfoModel;


            if (!_CurrentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {
                tbxNugetServerUrl.Text = _CurrentNuGetSettingInfoModel.NugetServerUrl;
                tbxNupkgPublishKey.Text = _CurrentNuGetSettingInfoModel.NupkgPublishKey;
                cbxClearOldNupkg.IsChecked = _CurrentNuGetSettingInfoModel.IsClearOldNupkg;

                var sb = new StringBuilder();

                foreach (var nupkgFullPath in _CurrentNuGetSettingInfoModel.NupkgFullPathList)
                {
                    sb.AppendLine(nupkgFullPath);
                }

                tbxAllNupkgFileFullPath.Text = sb.ToString();
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            var nuGetSettingWindow = new NuGetSettingWindow(null)
            {
                Owner = this
            };

            //nuGetSettingWindow.Show();

            nuGetSettingWindow.ShowDialog();

            if (_CurrentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {
                _CurrentNuGetSettingInfoModel = new NuGetSettingInfoModel
                {
                    ID = Guid.NewGuid(),
                };
            }

            if (_CurrentNuGetSettingInfoModel.ID != _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel.ID)
            {

                _NameSourceList.Clear();
                foreach (var nuGetSettingInfoModel in _CurrentNuGetSettingConfigModel.NuGetSettingList)
                {
                    _NameSourceList.Add(nuGetSettingInfoModel);
                }

                _CurrentNuGetSettingInfoModel = _NameSourceList.Where(o => o.ID == _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel.ID).FirstOrDefault();

                cbxName.SelectedItem = _CurrentNuGetSettingInfoModel;


            }

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            if (_CurrentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {
                return;
            }

            var nuGetSettingWindow = new NuGetSettingWindow(_CurrentNuGetSettingInfoModel)
            {
                Owner = this
            };

            nuGetSettingWindow.ShowDialog();

            //_NameSourceList.Clear();
            //foreach (var nuGetSettingInfoModel in _CurrentNuGetSettingConfigModel.NuGetSettingList)
            //{
            //    _NameSourceList.Add(nuGetSettingInfoModel);
            //}
            cbxName.SelectedItem = null;
            _CurrentNuGetSettingInfoModel = _NameSourceList.Where(o => o.ID == _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel.ID).FirstOrDefault();
            cbxName.SelectedItem = _CurrentNuGetSettingInfoModel;

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

            if (_CurrentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {
                return;
            }

            _NameSourceList.Remove(_CurrentNuGetSettingInfoModel);

            _CurrentNuGetSettingInfoModel = _NameSourceList.FirstOrDefault();

            cbxName.SelectedItem = _CurrentNuGetSettingInfoModel;

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


                if (_CurrentNuGetServerManipulater.IfIsNullOrEmpty())
                {
                    _CurrentNuGetServerManipulater = new NuGetServerManipulater(_CurrentNuGetSettingInfoModel.NupkgPublishKey, _CurrentNuGetSettingInfoModel.NugetServerUrl);
                }


                await Dispatcher.InvokeAsync(() =>
                {

                    btnRefreshNupkgs.IsEnabled = false;
                    _NupkgsSourceList.Clear();

                });

                int count = _CurrentNuGetSettingInfoModel.NupkgFullPathList.Count;
                int index = 0;

                var list = new List<NupkgItemInfoViewModel>();

                foreach (var nupkgFullPath in _CurrentNuGetSettingInfoModel.NupkgFullPathList)
                {

                    index++;

                    await Dispatcher.InvokeAsync(() =>
                    {

                        btnRefreshNupkgs.Content = string.Format("{0}/{1}", index, count);

                    });

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


                        //_NupkgsSourceList.Add(nupkgItemInfoModel);

                        list.Add(nupkgItemInfoModel);



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

                //list = list.OrderBy(o => o.ID).ToList();


                foreach (var nupkgItemInfoModel in list.OrderBy(o => o.ID))
                {
                    _NupkgsSourceList.Add(nupkgItemInfoModel);
                }

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.ToString());
            }
            finally
            {
                btnRefreshNupkgs.IsEnabled = true;
                btnRefreshNupkgs.Content = "刷新";
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

        private async void btnPublishNupkgs_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                if (lbxNupkgs.SelectedItems.Count <= 0)
                {
                    return;
                }

                //var publishKey = tbxNupkgKey.Text.Trim();

                int count = lbxNupkgs.SelectedItems.Count;
                int index = 0;

                await Dispatcher.InvokeAsync(() =>
                {
                    btnPublishNupkgs.IsEnabled = false;
                });

                foreach (var lbxNupkgsSelectedItem in lbxNupkgs.SelectedItems)
                {

                    index++;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        btnPublishNupkgs.Content = string.Format("{0}/{1}", index, count);
                    });

                    var nupkgItemInfoModel = lbxNupkgsSelectedItem as NupkgItemInfoViewModel;

                    if (nupkgItemInfoModel.IsEnable)
                    {

                        //await NuGetServerHelper.PublishNuGetPackage(nupkgItemInfoModel.FileFullPath, publishKey);
                        await _CurrentNuGetServerManipulater.PublishNuGetPackage(nupkgItemInfoModel.FileFullPath);

                    }

                }

                //_CurrentNupkgsInfoModel.NupkgPublishKey = publishKey;

                var scanRootDirectoryFullPath = _CurrentNuGetSettingInfoModel.ScanRootDirectoryFullPath;

                if (!scanRootDirectoryFullPath.IfIsNullOrEmpty() && Directory.Exists(scanRootDirectoryFullPath))
                {

                    var allNupkgFileFullPathList = Directory.GetFiles(scanRootDirectoryFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.AllDirectories)
                        .Select(System.IO.Path.GetDirectoryName)
                        .Distinct()
                        .OrderBy(o => o)
                        .ToList();

                    if (allNupkgFileFullPathList.IfIsNullOrEmpty())
                    {
                        allNupkgFileFullPathList = new List<string>();
                    }

                    foreach (var nupkgFileFullPath in allNupkgFileFullPathList)
                    {

                        var nupkgList = Directory.GetFiles(nupkgFileFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.TopDirectoryOnly)
                            .Select(o => new FileInfo(o))
                            .OrderByDescending(o => o.CreationTime)
                            .ToList();

                        if (nupkgList.IfIsNullOrEmpty())
                        {
                            nupkgList = new List<FileInfo>();
                        }

                        nupkgList.RemoveAt(0);

                        foreach (var fileInfo in nupkgList)
                        {
                            fileInfo.Delete();
                        }

                    }

                }


                MessageBox.Show("发布完成");

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
            finally
            {
                btnPublishNupkgs.Content = "发布包";
                btnPublishNupkgs.IsEnabled = true;
            }

        }
    }
}
