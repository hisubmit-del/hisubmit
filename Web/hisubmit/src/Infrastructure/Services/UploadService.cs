using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using System;
using System.IO;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        public UploadService()
        {
          //  TryCreateFilesFolder();
        }
        public string UploadAsync(UploadRequest request)
        {
            if (request.Data == null) return string.Empty;
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                var folder = request.UploadType.ToDescriptionString();
                var folderName = Path.Combine("Files", folder);
                var pathToSave = GetOrCreateFolderPath(folderName);
                var fileName = SanitizeFileName(request.FileName);
                var fullPath = EnsurePathWithinRoot(pathToSave, Path.Combine(pathToSave, fileName));
                var dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(fullPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = EnsurePathWithinRoot(pathToSave, NextAvailableFilename(fullPath));
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string numberPattern = " ({0})";

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            //if (tmp == pattern)
            //throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public bool DeleteAsync(DeleteFileRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.RelativeDirectory))
            {
                var rootDirectory = Directory.GetCurrentDirectory();
                var absoluteDirectory = EnsurePathWithinRoot(rootDirectory, Path.Combine(rootDirectory, request.RelativeDirectory));
                if (File.Exists(absoluteDirectory))
                {
                    File.Delete(absoluteDirectory);
                    return true;
                }
            }
            return false;
        }

        public bool ExistAsync(ExistFileRequest request)
        {
            var folder = request.UploadType.ToDescriptionString();
            var folderName = Path.Combine("Files", folder);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            
                bool existsFolder = Directory.Exists(pathToSave);
            if (!existsFolder)
            {
                return false;
            }

            var fileName = SanitizeFileName(request.Name);
            var fullPath = EnsurePathWithinRoot(pathToSave, Path.Combine(pathToSave, fileName));
            return File.Exists(fullPath);
        }

        public bool DeleteAsync(DeleteFileWithUploadTypeRequest request)
        {
            var folder = request.UploadType.ToDescriptionString();
            var folderName = Path.Combine("Files", folder);
            var pathToDelete = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            var fileName = SanitizeFileName(request.Name);
            var fullPath = EnsurePathWithinRoot(pathToDelete, Path.Combine(pathToDelete, fileName));
            bool exists = File.Exists(fullPath);
            if (exists)
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task AppendAsync(AppendFileRequest request)
        {
            var folder = request.UploadType.ToDescriptionString();
            var folderName = Path.Combine("Files", folder);
            var pathToSave = GetOrCreateFolderPath(folderName);
            var fileName = SanitizeFileName(request.File.FileName);
            var fullPath = EnsurePathWithinRoot(pathToSave, Path.Combine(pathToSave, fileName));

            using var fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.None);
            await request.File.CopyToAsync(fileStream);
        }

        private static string GetOrCreateFolderPath(string folderName)
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            return pathToSave;
        }

        private static string SanitizeFileName(string? fileName)
        {
            var safeFileName = Path.GetFileName((fileName ?? string.Empty).Trim('"'));
            return string.IsNullOrWhiteSpace(safeFileName)
                ? $"{Guid.NewGuid():N}.bin"
                : safeFileName;
        }

        private static string EnsurePathWithinRoot(string rootPath, string candidatePath)
        {
            var fullRootPath = Path.GetFullPath(rootPath);
            var fullCandidatePath = Path.GetFullPath(candidatePath);

            if (!fullCandidatePath.StartsWith(fullRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            return fullCandidatePath;
        }

        // private void TryCreateFilesFolder()
        // {
        //     var path = Path.Combine(Directory.GetCurrentDirectory(), "/Files");
        //     bool exists = Directory.Exists(path);
        //     if (!exists)
        //     {
        //         Directory.CreateDirectory(path);
        //     }
        // }
    }
}