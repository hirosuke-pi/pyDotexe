using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyDotexe.Merger
{
    class Merge
    {
        private static MergeSet mset;
        private class StartMergeError : Exception
        {
            public StartMergeError(string Message) { }
        }

        /// <summary>
        /// Start merge Application
        /// </summary>
        /// <param name="MergeSettings">MergeSet Object</param>
        /// <returns></returns>
        public static bool Start(ref MergeSet MergeSettings)
        {
            try
            {
                mset = MergeSettings;
                Console.WriteLine("[+] Merging with an application binary...");
                start_merge(); // Start merge
                return true;
            }
            catch (StartMergeError)
            {
                return false;
            }
        }

        public static bool Start(BuildSet BuildSettings)
        {
            try
            {
                mset = new MergeSet().ConvertMergeSet(BuildSettings);
                Console.WriteLine("[+] Merging with an application binary...");
                start_merge(); // Start merge
                return true;
            }
            catch (StartMergeError)
            {
                return false;
            }
        }

        private static byte[] build_header()
        {
            // Build header
            byte[] header = Encoding.UTF8.GetBytes(mset.start_src + "|" + mset.start_bin + "|" + mset.version + "|" + mset.start_argv);
            return Enumerable.Concat(new byte[] { 0xff }, Encoding.UTF8.GetBytes(Convert.ToBase64String(header))).ToArray();
            
        }

        private static void start_merge()
        {
            try
            {

                using (FileStream fs = new FileStream(mset.output_path, FileMode.Create, FileAccess.Write))
                {
                    if (!mset.onefile)
                        fs.Write(Properties.Resource.Application_f, 0, Properties.Resource.Application_f.Length);
                    else if (mset.use_cache)
                        fs.Write(Properties.Resource.Application, 0, Properties.Resource.Application.Length);
                    else
                        fs.Write(Properties.Resource.Application_, 0, Properties.Resource.Application_.Length);
                }

                try
                {
                    if (mset.icon_path != "")
                    {
                        IconChanger icon_change = new IconChanger();
                        icon_change.ChangeIcon(mset.output_path, mset.icon_path);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[!] Could not change the binary icon: " + ex.Message);
                }

                // Start merging
                if (mset.onefile)
                {
                    byte[] before_data = null;
                    using (FileStream fs = new FileStream(mset.output_path, FileMode.Open, FileAccess.Read))
                    {
                        before_data = new byte[fs.Length];
                        fs.Read(before_data, 0, (int)fs.Length);
                    }
                    using (FileStream fs = new FileStream(mset.output_path, FileMode.Create, FileAccess.Write))
                    {
                        // Write all bytes
                        fs.Write(before_data, 0, before_data.Length);
                        FileStream fsm = new FileStream(mset.src_zip, FileMode.Open, FileAccess.Read);
                        byte[] zip_bytes = new byte[fsm.Length];

                        // Build header
                        byte[] start_path = build_header();

                        fsm.Read(zip_bytes, 0, (int)fsm.Length);
                        fs.Write(zip_bytes, 0, zip_bytes.Length);
                        fs.Write(start_path, 0, start_path.Length);
                        fsm.Close();
                    }
                }
                else
                {
                    byte[] before_data = null;
                    using (FileStream fs = new FileStream(mset.output_path, FileMode.Open, FileAccess.Read))
                    {
                        before_data = new byte[fs.Length];
                        fs.Read(before_data, 0, (int)fs.Length);
                    }
                    using (FileStream fs = new FileStream(mset.output_path, FileMode.Create, FileAccess.Write))
                    {
                        // Write all bytes
                        fs.Write(before_data, 0, before_data.Length);
                        // Build header
                        byte[] header = build_header();
                        fs.Write(header, 0, header.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\n[-] Failed to merge Application-file and ZIP-file");
                Console.WriteLine("[!] " + ex.Message);
                throw new StartMergeError("");
            }
            // Set time
            try
            {
                File.SetCreationTime(mset.output_path, DateTime.Now);
                File.SetLastAccessTime(mset.output_path, DateTime.Now);
            }
            catch { }
        }
    }
}
