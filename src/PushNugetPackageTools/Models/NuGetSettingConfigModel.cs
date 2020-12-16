using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Interfaces;

namespace PushNugetPackageTools.Models
{

    public class NuGetSettingConfigModel : ITimestampProperties
    {


        public NuGetSettingInfoModel CurrentNuGetSettingInfoModel { get; set; }

        public List<NuGetSettingInfoModel> NuGetSettingList { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

    }


}
