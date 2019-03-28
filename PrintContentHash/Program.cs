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
            var rootPath = Environment.CurrentDirectory;

            // default args
            var nugetPath = Path.Combine(rootPath, "testpackages");
            var id = "Microsoft.CSharp";
            var version = "4.0.1";

            // specify the nuget packages path
            if (args.Length == 1) {
                nugetPath = args[0];
            }
            else if (args.Length == 3)
            {
                nugetPath = args[0];
                id = args[1];
                version = args[2];
            }

            var nugetVersion = NuGet.Versioning.NuGetVersion.Parse(version);
            var pathResolver = new VersionFolderPathResolver(nugetPath);

            //var nupkgMetadataPath = pathResolver.GetNupkgMetadataPath(id, version);
            var nupkgMetadataPath = Path.Combine(rootPath, ".nupkg.metadata");
            var hashPath = pathResolver.GetHashPath(id, nugetVersion);
            var zipPath = pathResolver.GetPackageFilePath(id, nugetVersion);
            var installPath = pathResolver.GetInstallPath(id, nugetVersion);

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
