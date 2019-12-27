using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Lanymy.Common;
using Lanymy.Common.Console;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.Console.ExtensionFunctions;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models;

namespace PushNugetPackageTools
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {

            //PathHelper.InitDirectoryPath(GlobalSettings.NuGetPackagesFolderFullPath);

            //            var args = e.Args;

            //            if (!args.IfIsNullOrEmpty())
            //            {

            //                bool isDebugMode = false;

            //#if DEBUG
            //                isDebugMode = true;
            //#endif

            //                var logMessage = new StringBuilder();


            //                //var fileLogFullPath = @"E:\VS Project\Github\Lanymy\Lanymy.NET\src\Commons\Lanymy.Common\bin\Release\1.txt";

            //                //File.WriteAllText(fileLogFullPath, string.Join(",", args));

            //                //var domainPath = PathHelper.GetCallDomainPath();

            //                //logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(domainPath), domainPath));
            //                //logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(args), string.Join("[-]", args)));


            //                var resultModel = ConsoleHelper.MatchConsoleArguments<ConsoleArgumentTypeEnum, ConsoleArgumentEnumAttribute>(args);

            //                if (resultModel.IsSuccess)
            //                {

            //                    var targetFramework = ConsoleArgumentTypeEnum.TargetFrameworkArg.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>().ToLower();
            //                    var publishRootDirectoryFullPath = ConsoleArgumentTypeEnum.PublishRootDirectoryFullPathArg.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>();

            //                    if (publishRootDirectoryFullPath.EndsWith('"'))
            //                    {
            //                        publishRootDirectoryFullPath = publishRootDirectoryFullPath.RightRemoveString(1);
            //                    }

            //                    if (!publishRootDirectoryFullPath.EndsWith('\\'))
            //                    {
            //                        publishRootDirectoryFullPath += "\\";
            //                    }

            //                    publishRootDirectoryFullPath = PathHelper.CombineDirectoryRelativePath(publishRootDirectoryFullPath, @"..\");

            //                    logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(targetFramework), targetFramework));
            //                    logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(publishRootDirectoryFullPath), publishRootDirectoryFullPath));


            //                    if (targetFramework.StartsWith("netstandard"))
            //                    {

            //                        //var list = Directory.GetFiles(publishRootDirectoryFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.TopDirectoryOnly).Select(o => new FileInfo(o)).ToList();
            //                        //list = list.OrderByDescending(o => o.CreationTime).ToList();

            //                        var lastNupkgFileinfo = Directory.GetFiles(publishRootDirectoryFullPath, "*" + GlobalSettings.NUGET_PACKAGE_FILE_SUFFIX, SearchOption.TopDirectoryOnly).Select(o => new FileInfo(o)).OrderByDescending(o => o.CreationTimeUtc).FirstOrDefault();
            //                        if (!lastNupkgFileinfo.IfIsNullOrEmpty())
            //                        {

            //                            var scheduleFileInfoModel = new ScheduleFileInfoModel
            //                            {
            //                                SourceFileFullPath = lastNupkgFileinfo.FullName,
            //                                TargetFileFullPath = Path.Combine(GlobalSettings.NuGetPackagesFolderFullPath, lastNupkgFileinfo.Name),
            //                            };

            //                            logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(scheduleFileInfoModel.SourceFileFullPath), scheduleFileInfoModel.SourceFileFullPath));
            //                            logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", nameof(scheduleFileInfoModel.TargetFileFullPath), scheduleFileInfoModel.TargetFileFullPath));

            //                            File.Copy(scheduleFileInfoModel.SourceFileFullPath, scheduleFileInfoModel.TargetFileFullPath, true);

            //                        }

            //                    }
            //                    else if (isDebugMode)
            //                    {
            //                        Shutdown();
            //                    }

            //                }

            //                logMessage.AppendLine(string.Format("[ {0} ] - [ {1} ]", resultModel.IsSuccess, resultModel.ErrorMessage));

            //                if (isDebugMode)
            //                {
            //                    MessageBox.Show(logMessage.ToString());
            //                }
            //                else
            //                {
            //                    Shutdown();
            //                }

            //            }


            base.OnStartup(e);

        }


    }

}
