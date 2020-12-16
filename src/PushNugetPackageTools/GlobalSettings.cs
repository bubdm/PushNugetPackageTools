using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lanymy.Common;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using PushNugetPackageTools.Models;

namespace PushNugetPackageTools
{

    internal class GlobalSettings
    {

        /// <summary>
        /// 程序域 根目录 全路径
        /// </summary>
        public static readonly string CallDomainBasePath;

        /// <summary>
        /// NUGET 包文件 后缀名 .nupkg
        /// </summary>
        public const string NUGET_PACKAGE_FILE_SUFFIX = ".nupkg";

        /// <summary>
        /// NuGetPackages
        /// </summary>
        public const string NUGET_PACKAGES_FOLDER_NAME = "NuGetPackages";


        private static string _NuGetPackagesFolderFullPath;

        /// <summary>
        /// 配置表  文件夹  全路径
        /// </summary>
        public static string NuGetPackagesFolderFullPath => _NuGetPackagesFolderFullPath;

        public static NuGetSettingConfigModel CurrentNuGetSettingConfigModel { get; private set; }


        static GlobalSettings()
        {


            CallDomainBasePath = PathHelper.GetCallDomainPath();


            InitBasePathInfo();

            LoadNuGetSettingConfigModel();

        }


        private static void InitBasePathInfo()
        {

            _NuGetPackagesFolderFullPath = Path.Combine(CallDomainBasePath, NUGET_PACKAGES_FOLDER_NAME);

        }

        private static void LoadNuGetSettingConfigModel()
        {

            NuGetSettingConfigModel currentNuGetSettingConfigModel = null;

            try
            {

                currentNuGetSettingConfigModel = IsolatedStorageHelper.GetModel<NuGetSettingConfigModel>();

            }
            catch (Exception e)
            {
                currentNuGetSettingConfigModel = null;
            }

            if (currentNuGetSettingConfigModel.IfIsNullOrEmpty())
            {
                currentNuGetSettingConfigModel = new NuGetSettingConfigModel
                {
                    CurrentNuGetSettingInfoModel = new NuGetSettingInfoModel(),
                    NuGetSettingList = new List<NuGetSettingInfoModel>(),
                    CreateDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now,

                };
            }


            CurrentNuGetSettingConfigModel = currentNuGetSettingConfigModel;

        }


        public static void SaveNuGetSettingConfigModel()
        {

            IsolatedStorageHelper.SaveModel(CurrentNuGetSettingConfigModel);

        }


    }

}
