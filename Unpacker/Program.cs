using System;
using System.Collections.Generic;
using System.Linq;

namespace Unpacker
{
    class Program
    {
        private const string ProcessedFile = ".UnpackerProcessed";
        
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now:G}\tUnpacker starting");

            DirectoryScanner.ProcessedFile = ProcessedFile;
            DirectoryScanner.Scan("/archives");
            
            Console.WriteLine($"{DateTime.Now:G}\tUnpacker terminating");
        }
    }

    internal static class DirectoryScanner
    {
        public static string ProcessedFile;

        public static void Scan(string path)
        {
            var dirs = System.IO.Directory.EnumerateDirectories(path);

            foreach (var dir in dirs)
            {
                Scan(dir);
            }

            var archives = GetArchives(path);
            if (!archives.Any())
                return;

            UnpackArchives(path, archives);
        }

        private static IEnumerable<Archive> GetArchives(string path)
        {
            var ret = new List<Archive>();

            if (AlreadyUnpacked(path))
            {
                // We've already unpacked everything in this folder, so pretend we don't have anything to unpack                
                return ret;
            }

            var files = System.IO.Directory.GetFiles(path);
            
            // multi-part 7z files
            ret.AddRange(files.Where(f => f.EndsWith(".7z.001", StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Archive(f)));
            
            // single-part 7z files
            ret.AddRange(files.Where(f => f.EndsWith(".7z", StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Archive(f)));
            
            // multi-part rar files
            ret.AddRange(files.Where(f => f.EndsWith(".part01.rar", StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Archive(f)));
            
            // single-part rar files
            ret.AddRange(files
                .Where(f => f.EndsWith(".rar", StringComparison.CurrentCultureIgnoreCase) &&
                            !f.Contains(".part", StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Archive(f)));
            
            // ALL zip files
            ret.AddRange(files.Where(f => f.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Archive(f)));
            
            return ret;
        }

        private static bool AlreadyUnpacked(string path)
        {
            return System.IO.File.Exists(System.IO.Path.Combine(path, ProcessedFile));
        }

        private static void UnpackArchives(string path, IEnumerable<Archive> archives)
        {
            // Finally .. time to unpack!
            foreach (var archive in archives)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now:G}\tStart\t{archive.Filename}");
                    archive.Unpack();
                    Console.WriteLine($"{DateTime.Now:G}\tEnd\t{archive.Filename}");                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to unpack '{archive.Filename}': {ex.Message}");
                }
            }
            
            // Mark directory as processed
            System.IO.File.Create(System.IO.Path.Combine(path, ProcessedFile)).Close();
        }
    }
}