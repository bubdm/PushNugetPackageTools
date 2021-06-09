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
using Lanymy.Common;
using Lanymy.Common.ExtensionFunctions;
using Microsoft.Win32;
using PushNugetPackageTools.Models;
using Path = System.IO.Path;

namespace PushNugetPackageTools
{
    /// <summary>
    /// NuGetSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NuGetSettingWindow : Window
    {

        private static readonly NuGetSettingConfigModel _CurrentNuGetSettingConfigModel = GlobalSettings.CurrentNuGetSettingConfigModel;

        private NuGetSettingInfoModel _CurrentNuGetSettingInfoModel;
        private bool _IsAdd = false;

        private readonly ObservableCollection<string> _NupkgFullPathSourceList = new ObservableCollection<string>();


        public NuGetSettingWindow(NuGetSettingInfoModel nuGetSettingInfoModel)
        {

            InitializeComponent();

            _CurrentNuGetSettingInfoModel = nuGetSettingInfoModel;
            if (_CurrentNuGetSettingInfoModel.IfIsNullOrEmpty())
            {

                _IsAdd = true;
                _CurrentNuGetSettingInfoModel = new NuGetSettingInfoModel
                {
                    ID = Guid.NewGuid(),
                };

            }
            else
            {
                _IsAdd = false;
            }


            Loaded += NuGetSettingWindow_Loaded;

        }

        private void NuGetSettingWindow_Loaded(object sender, RoutedEventArgs e)
        {

            Title = string.Format("{0} - {1}", Title, _IsAdd ? "新增" : "修改");

            tbxName.Text = _CurrentNuGetSettingInfoModel.NuGetSettingName;
            tbxNugetServerUrl.Text = _CurrentNuGetSettingInfoModel.NugetServerUrl;
            tbxNupkgPublishKey.Text = _CurrentNuGetSettingInfoModel.NupkgPublishKey;
            cbxClearOldNupkg.IsChecked = _CurrentNuGetSettingInfoModel.IsClearOldNupkg;

            foreach (var nupkgFullPath in _CurrentNuGetSettingInfoModel.NupkgFullPathList)
            {
                _NupkgFullPathSourceList.Add(nupkgFullPath);
            }

            lbxNupkgFullPath.ItemsSource = _NupkgFullPathSourceList;

        }

        private void btnSelectScanRootDirectoryFullPath_Click(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new OpenFileDialog
            {
                Title = "选择要扫描包根路径下的任一文件",
                //Filter = "Nuget包|" + "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX,
                FileName = string.Empty,
                Multiselect = false,
                RestoreDirectory = true
            };

            //openFileDialog.Filter = "txt文件|*.txt|rar文件|*.rar|所有文件|*.*";
            //openFileDialog.FilterIndex = 1;

            if ((bool)openFileDialog.ShowDialog())
            {

                var scanRootDirectoryFullPath = openFileDialog.FileName;
                tbxScanRootDirectoryFullPath.Text = Path.GetDirectoryName(scanRootDirectoryFullPath) ?? string.Empty;

            }


        }


        private void btnScan_Click(object sender, RoutedEventArgs e)
        {

            var scanRootDirectoryFullPath = tbxScanRootDirectoryFullPath.Text;

            if (scanRootDirectoryFullPath.IfIsNullOrEmpty() || !Directory.Exists(scanRootDirectoryFullPath))
            {
                MessageBox.Show("请选择一个有效的扫描根目录路径");

                return;
            }

            var matchKeyword = tbxMatchKeyword.Text.Trim() == "路径匹配关键字" ? string.Empty : tbxMatchKeyword.Text.Trim();

            var allNupkgFileFullPathList = Directory.GetFiles(scanRootDirectoryFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.AllDirectories).ToList();

            if (!matchKeyword.IfIsNullOrEmpty())
            {
                allNupkgFileFullPathList = allNupkgFileFullPathList.Where(o => o.Contains(matchKeyword)).ToList();
            }

            allNupkgFileFullPathList = allNupkgFileFullPathList.Select(Path.GetDirectoryName).ToList();

            if (allNupkgFileFullPathList.IfIsNullOrEmpty())
            {
                allNupkgFileFullPathList = new List<string>();
            }


            allNupkgFileFullPathList.AddRange(_NupkgFullPathSourceList);
            allNupkgFileFullPathList = allNupkgFileFullPathList.Distinct().OrderBy(o => o).ToList();

            _NupkgFullPathSourceList.Clear();

            foreach (var nupkgFileFullPath in allNupkgFileFullPathList)
            {
                _NupkgFullPathSourceList.Add(nupkgFileFullPath);
            }


        }

        private void btnAddOneNupkg_Click(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new OpenFileDialog
            {
                Title = "选择要发布的Nuget包",
                Filter = "Nuget包|" + "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX,
                FileName = string.Empty,
                Multiselect = false,
                RestoreDirectory = true
            };

            //openFileDialog.Filter = "txt文件|*.txt|rar文件|*.rar|所有文件|*.*";
            //openFileDialog.FilterIndex = 1;

            if ((bool)openFileDialog.ShowDialog())
            {

                var newNupkgFileFullPath = openFileDialog.FileName;
                var newNupkgFileDirectoryFullPath = Path.GetDirectoryName(newNupkgFileFullPath) ?? string.Empty;
                if (!newNupkgFileDirectoryFullPath.IfIsNullOrEmpty())
                {

                    var allNupkgFileFullPathList = new List<string>(_NupkgFullPathSourceList)
                        {
                            newNupkgFileDirectoryFullPath
                        }
                        .Distinct()
                        .OrderBy(o => o)
                        .ToList();

                    _NupkgFullPathSourceList.Clear();

                    foreach (var nupkgFileFullPath in allNupkgFileFullPathList)
                    {
                        _NupkgFullPathSourceList.Add(nupkgFileFullPath);
                    }

                }

            }


        }

        private void btnRemoveSelectedNupkgs_Click(object sender, RoutedEventArgs e)
        {

            if (lbxNupkgFullPath.SelectedItems.Count <= 0)
            {
                return;
            }

            var list = (from object nupkgFullPath in lbxNupkgFullPath.SelectedItems select nupkgFullPath as string).ToList();
            foreach (var item in list)
            {
                _NupkgFullPathSourceList.Remove(item);
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            _CurrentNuGetSettingInfoModel.NuGetSettingName = tbxName.Text.Trim();
            _CurrentNuGetSettingInfoModel.ScanRootDirectoryFullPath = tbxScanRootDirectoryFullPath.Text.Trim();
            _CurrentNuGetSettingInfoModel.NugetServerUrl = tbxNugetServerUrl.Text.Trim();
            _CurrentNuGetSettingInfoModel.NupkgPublishKey = tbxNupkgPublishKey.Text.Trim();
            _CurrentNuGetSettingInfoModel.IsClearOldNupkg = cbxClearOldNupkg.IsChecked.Value;


            _CurrentNuGetSettingInfoModel.NupkgFullPathList = _NupkgFullPathSourceList.ToList();

            if (_IsAdd)
            {
                _CurrentNuGetSettingInfoModel.CreateDateTime = DateTime.Now;
                _CurrentNuGetSettingConfigModel.NuGetSettingList.Add(_CurrentNuGetSettingInfoModel);

            }

            _CurrentNuGetSettingInfoModel.LastUpdateDateTime = DateTime.Now;
            _CurrentNuGetSettingConfigModel.CurrentNuGetSettingInfoModel = _CurrentNuGetSettingInfoModel;

            MessageBox.Show("保存成功");

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }


}
