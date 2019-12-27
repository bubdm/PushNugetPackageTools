using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using NuGet.Versioning;
using PushNugetPackageTools.Annotations;

namespace PushNugetPackageTools.Models
{

    public class NupkgItemInfoModel //: INotifyPropertyChanged
    {

        public string ID { get; set; }
        public string FileFullName { get; set; }
        public string FileFullPath { get; set; }
        public NuGetVersion Version { get; set; } = new NuGetVersion("0");
        public NuGetVersion RemoteVersion { get; set; } = new NuGetVersion("0");

        public string DisplayTitle
        {
            get
            {
                var title = string.Format("[ {0} ] [ {1} ] [ {2} ] [ {3} ]", FileFullName, ID, Version, RemoteVersion);
                if (!IsEnable)
                {
                    title = "X " + title;
                }
                return title;
            }
        }

        public bool IsEnable
        {
            get
            {
                return Version > RemoteVersion;
                //return false;
            }
        }


        //private bool _IsChecked;
        //public bool IsChecked
        //{
        //    get
        //    {
        //        return _IsChecked;
        //    }
        //    set
        //    {
        //        _IsChecked = value;
        //        OnPropertyChanged();
        //    }
        //}


        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

    }

}
