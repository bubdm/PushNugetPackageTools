using System;
using System.Collections.Generic;
using System.Text;

namespace PushNugetPackageTools.Models
{


    public class NupkgsInfoModel
    {

        public List<string> NupkgFullPathList { get; set; } = new List<string>();

        //public List<SingleNupkgInfoModel> NupkgInfoType { get; set; } = new List<SingleNupkgInfoModel>();

        public string NupkgPublishKey { get; set; }

        public string NugetServerUrl { get; set; }

    }

}
