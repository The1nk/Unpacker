using System;
using System.Diagnostics;
using System.IO;

namespace Unpacker
{
    public class Archive
    {
        public string Filename;

        public Archive(string filename)
        {
            Filename = filename;
        }

        public void Unpack()
        {
            Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Filename));
            var p = new Process
            {
                StartInfo =
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };


            if (Filename.Contains(".7z", StringComparison.CurrentCultureIgnoreCase))
            {
                p.StartInfo.FileName = "7z";
                p.StartInfo.Arguments = $"e -aoa \"{Filename}\"";
            } else if (Filename.Contains(".rar", StringComparison.CurrentCultureIgnoreCase))
            {
                p.StartInfo.FileName = "7z";
                p.StartInfo.Arguments = $"e -aoa \"{Filename}\"";
            } else if (Filename.Contains(".zip", StringComparison.CurrentCultureIgnoreCase))
            {
                p.StartInfo.FileName = "7z";
                p.StartInfo.Arguments = $"e -aoa \"{Filename}\"";
            }

            p.Start();
            p.WaitForExit();

            if (p.ExitCode != 0)
                throw new Exception($"File '{Filename}' failed to unpack - Exit code {p.ExitCode} returned");
            
        }
    }
}