using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Interfaces;

namespace PushNugetPackageTools.Models
{

    public class NuGetSettingInfoModel : ITimestampProperties
    {

        public Guid ID { get; set; }

        public string NuGetSettingName { get; set; } = "方案名称";

        public string NugetServerUrl { get; set; } = "https://api.nuget.org/v3/index.json";
        public string NupkgPublishKey { get; set; } = "Nuget API Key";
        public List<string> NupkgFullPathList { get; set; } = new List<string>();
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

    }

}
