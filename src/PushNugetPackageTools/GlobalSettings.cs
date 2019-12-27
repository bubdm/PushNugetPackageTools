using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lanymy.Common;

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

        static GlobalSettings()
        {


            CallDomainBasePath = PathHelper.GetCallDomainPath();


            InitBasePathInfo();


        }


        private static void InitBasePathInfo()
        {

            _NuGetPackagesFolderFullPath = Path.Combine(CallDomainBasePath, NUGET_PACKAGES_FOLDER_NAME);

        }

    }

}
