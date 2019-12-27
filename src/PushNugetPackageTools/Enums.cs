using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.Console.CustomAttributes;

namespace PushNugetPackageTools
{

    public enum ConsoleArgumentTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 未知类型
        /// </summary>
        UnKnown,

        [ConsoleArgumentEnum("TargetFramework", "$(TargetFramework) 目标平台编译标识符", true)]
        TargetFrameworkArg,


        [ConsoleArgumentEnum("PublishRootDirectoryFullPath", "$(TargetDir) 目标编译输出的路径", true)]
        PublishRootDirectoryFullPathArg,


    }

}
