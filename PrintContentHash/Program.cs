using System;
using System.IO;
using System.Threading;
using NuGet.Packaging;
using NuGet.Protocol;

namespace PrintContentHash
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = "Microsoft.CSharp";
            var version = NuGet.Versioning.NuGetVersion.Parse("4.0.1");
            var rootPath = Environment.CurrentDirectory;
            var nugetPath = Path.Combine(rootPath, "testpackages");
            var pathResolver = new VersionFolderPathResolver(nugetPath);

            //var nupkgMetadataPath = pathResolver.GetNupkgMetadataPath(id, version);
            var nupkgMetadataPath = Path.Combine(rootPath, ".nupkg.metadata");
            var hashPath = pathResolver.GetHashPath(id, version);
            var zipPath = pathResolver.GetPackageFilePath(id, version);
            var installPath = pathResolver.GetInstallPath(id, version);

            LocalFolderUtility.GenerateNupkgMetadataFile(zipPath, installPath, hashPath, nupkgMetadataPath);

            var nupkgMetadata = NupkgMetadataFileFormat.Read(nupkgMetadataPath);
            File.Delete(nupkgMetadataPath);
            Console.WriteLine(nupkgMetadata.ContentHash);

            using (var stream = File.OpenRead(zipPath))
            using (var packageReader = new PackageArchiveReader(stream, leaveStreamOpen: true))
            {
                var contentHash = packageReader.GetContentHash(CancellationToken.None);
                Console.WriteLine(contentHash);
            }
        }
    }
}
