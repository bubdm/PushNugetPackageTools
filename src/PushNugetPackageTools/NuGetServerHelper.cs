//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using NuGet.Common;
//using NuGet.Configuration;
//using NuGet.Protocol;
//using NuGet.Protocol.Core.Types;

//namespace PushNugetPackageTools
//{

//    public class NuGetServerHelper
//    {


//        private const string NUGET_SERVER_URL = "https://api.nuget.org/v3/index.json";

//        private static readonly SourceRepository _CurrentRepository = CreateRepository(NUGET_SERVER_URL);


//        public static SourceRepository CreateRepository(string source)
//        {
//            return CreateRepository(new PackageSource(source), null);
//        }


//        public static SourceRepository CreateRepository(PackageSource packageSource, IEnumerable<Lazy<INuGetResourceProvider>> additionalProviders)
//        {
//            var providers = Repository.Provider.GetCoreV3();

//            if (additionalProviders != null)
//            {
//                providers = providers.Concat(additionalProviders);
//            }

//            return Repository.CreateSource(providers, packageSource);
//        }


//        public static async Task<IPackageSearchMetadata> GetNuGetPackageMetadataByIdAsync(string packageID)
//        {

//            var findPackageByIdResource = await _CurrentRepository.GetResourceAsync<PackageMetadataResource>();

//            var metadata = await findPackageByIdResource.GetMetadataAsync(packageID, false, false, NullSourceCacheContext.Instance, NullLogger.Instance, new CancellationTokenSource().Token);

//            var result = metadata.OrderByDescending(m => m.Identity.Version).Take(1).FirstOrDefault();

//            return result;

//        }


//        public static async Task PublishNuGetPackage(string packageFileFullPath, string publishKey)
//        {

//            var packageResource = await _CurrentRepository.GetResourceAsync<PackageUpdateResource>();

//            await packageResource.Push(packageFileFullPath, null, 999, false, s => publishKey, s => publishKey, false, NullLogger.Instance);

//        }


//    }

//}
