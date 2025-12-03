namespace Aeroverra.StreamDeck.Client.Extensions
{
    internal static class DirectoryInfoExtensions
    {
        public static bool TryCopyContents(this DirectoryInfo source, DirectoryInfo destination, bool overwrite = false)
        {
            Directory.CreateDirectory(destination.FullName);
            var allSuccess = true;
            var files = source.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    var destinationPath = Path.Combine(destination.FullName, file.Name);
                    file.CopyTo(destinationPath, overwrite);
                }
                catch
                {
                    allSuccess = false;
                }
            }
            var directories = source.GetDirectories();
            foreach (var directory in directories)
            {
                var destinationDirectoryPath = Path.Combine(destination.FullName, directory.Name);
                var destinationDirectory = new DirectoryInfo(destinationDirectoryPath);
                var success = directory.TryCopyContents(destinationDirectory);
                if (!success) { allSuccess = false; }
            }
            return allSuccess;
        }
    }
}
