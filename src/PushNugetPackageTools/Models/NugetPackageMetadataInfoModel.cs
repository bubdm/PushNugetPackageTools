using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace PushNugetPackageTools.Models
{

    /// <summary>
    /// Nuget 包 元数据信息 实体类
    /// </summary>
    public class NugetPackageMetadataInfoModel
    {
        /// <summary>
        /// 包 全路径
        /// </summary>
        public string NugetPackageFileFullPath { get; }
        /// <summary>
        /// 包 全名称
        /// </summary>
        public string NugetPackageFileFullName { get; }

        /// <summary>
        /// 包 ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public NuGetVersion Version { get; private set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 作者信息
        /// </summary>
        public IEnumerable<string> Authors { get; private set; }

        /// <summary>
        /// 拥有者 信息
        /// </summary>
        public IEnumerable<string> Owners { get; private set; }


        public string Icon { get; private set; }

        public Uri IconUrl { get; private set; }

        public Uri LicenseUrl { get; private set; }

        public Uri ProjectUrl { get; private set; }

        public bool RequireLicenseAcceptance { get; private set; }

        public bool DevelopmentDependency { get; private set; }

        public string Description { get; private set; }

        public string Summary { get; private set; }

        public string ReleaseNotes { get; private set; }

        public string Language { get; private set; }

        public string Tags { get; private set; }

        public bool Serviceable { get; private set; }

        public string? Copyright { get; private set; }

        public Version MinClientVersion { get; private set; }

        public IEnumerable<PackageDependencyGroup> DependencyGroups { get; private set; }

        public IEnumerable<PackageReferenceSet> PackageAssemblyReferences { get; private set; }

        public IEnumerable<FrameworkAssemblyReference> FrameworkReferences { get; private set; }

        public IEnumerable<ManifestContentFiles> ContentFiles { get; private set; }

        public IEnumerable<PackageType> PackageTypes { get; private set; }

        public RepositoryMetadata Repository { get; private set; }

        public LicenseMetadata LicenseMetadata { get; private set; }

        public IEnumerable<FrameworkReferenceGroup> FrameworkReferenceGroups { get; private set; }


        /// <summary>
        /// Nuget 包 元数据信息 实体类 构造方法
        /// </summary>
        /// <param name="nugetPackageFileFullPath">包 全路径</param>
        public NugetPackageMetadataInfoModel(string nugetPackageFileFullPath)
        {

            NugetPackageFileFullPath = nugetPackageFileFullPath;
            NugetPackageFileFullName = Path.GetFileName(NugetPackageFileFullPath);

            AnalysisManifest();

        }

        /// <summary>
        /// 读取 包 元数据
        /// </summary>
        private void AnalysisManifest()
        {
            //using var stream = _streamFactory();
            //using var reader = new PackageArchiveReader(stream);
            //using var nuspecStream = reader.GetNuspec();
            //using var manifestStream = ManifestUtility.ReadManifest(nuspecStream);

            //var manifest = Manifest.ReadFrom(manifestStream, false);
            //_metadata = manifest.Metadata;

            using var stream = File.Open(NugetPackageFileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new PackageArchiveReader(stream);
            using var nuspecStream = reader.GetNuspec();
            //using var manifestStream = ManifestUtility.ReadManifest(nuspecStream);
            var manifest = Manifest.ReadFrom(nuspecStream, false);
            var metadata = manifest.Metadata;

            ID = metadata.Id;
            Version = metadata.Version;
            Title = metadata.Title;
            Authors = metadata.Authors;
            Owners = metadata.Owners;
            Icon = metadata.Icon;
            IconUrl = metadata.IconUrl;
            LicenseUrl = metadata.LicenseUrl;
            ProjectUrl = metadata.ProjectUrl;
            RequireLicenseAcceptance = metadata.RequireLicenseAcceptance;
            DevelopmentDependency = metadata.DevelopmentDependency;
            Description = metadata.Description;
            Summary = metadata.Summary;
            ReleaseNotes = metadata.ReleaseNotes;
            Language = metadata.Language;
            Tags = metadata.Tags;
            Serviceable = metadata.Serviceable;
            Copyright = metadata.Copyright;
            MinClientVersion = metadata.MinClientVersion;
            DependencyGroups = metadata.DependencyGroups;
            PackageAssemblyReferences = metadata.PackageAssemblyReferences;
            FrameworkReferences = metadata.FrameworkReferences;
            ContentFiles = metadata.ContentFiles;
            PackageTypes = metadata.PackageTypes;
            Repository = metadata.Repository;
            LicenseMetadata = metadata.LicenseMetadata;
            FrameworkReferenceGroups = metadata.FrameworkReferenceGroups;





        }

    }

}
