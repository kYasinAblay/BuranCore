using System.IO;

namespace Buran.Core.Library.Utils
{
    public class FileHelper
    {
        public void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var file2 = Path.GetFileName(file);
                if (file2 == null)
                {
                    continue;
                }

                var dest = Path.Combine(destPath, file2);
                File.Copy(file, dest);
            }
            foreach (var folder in Directory.GetDirectories(sourcePath))
            {
                var file2 = Path.GetFileName(folder);
                if (file2 == null)
                {
                    continue;
                }

                var dest = Path.Combine(destPath, file2);
                CopyDirectory(folder, dest);
            }
        }

        public void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Directory.Delete(targetDir, false);
        }

        public void CopyFiles(string sourceFolder, string targetFolder)
        {
            var files = Directory.GetFiles(sourceFolder);
            foreach (var file in files)
            {
                File.Copy(file, file.Replace(sourceFolder, targetFolder));
            }
        }

        public void CopyDirs(string sourceFolder, string targetFolder)
        {
            var directories = Directory.GetDirectories(sourceFolder);
            foreach (var directory in directories)
            {
                var newSubFolder = directory.Replace(sourceFolder, targetFolder);
                Directory.CreateDirectory(newSubFolder);
                CopyFiles(directory, directory.Replace(sourceFolder, targetFolder));
                CopyDirs(directory, newSubFolder);
            }
        }
    }
}