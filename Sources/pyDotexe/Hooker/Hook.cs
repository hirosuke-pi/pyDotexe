using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyDotexe.Hooker
{
    public class Hook
    {

        /// <summary>
        /// Start updating module-hooks
        /// </summary>
        /// <param name="dirpath">Folder path</param>
        /// <param name="now_dirpath">Now folder path</param>
        public static void Update(string dirpath, string now_dirpath)
        {
            if (Directory.Exists(dirpath))
            {
                string hooks_path = now_dirpath + @"\module-hooks";
                if (Directory.Exists(hooks_path)) 
                {
                    // Found module-hooks folder.
                    Console.WriteLine("\r\n[+] フックファイルをアップデートしています...");
                    foreach (string filename in Directory.GetFiles(dirpath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            // Copy hooks data.
                            File.Copy(filename, hooks_path + @"\" + Path.GetFileName(filename));
                            Console.WriteLine(" + コピー: "+ filename);
                        }
                        catch
                        {
                            try
                            {
                                // Update dist data.
                                string distfile = hooks_path + @"\" + Path.GetFileName(filename);
                                List<string> src_data = read_filelines(filename);
                                List<string> dist_data = read_filelines(distfile);
                                src_data.AddRange(dist_data);
                                List<string> clean_data = new List<string>(src_data.Distinct().ToArray());

                                using (StreamWriter sw = new StreamWriter(distfile))
                                {
                                    foreach (string line in clean_data)
                                    {
                                        sw.WriteLine(line);
                                    }
                                }
                                Console.WriteLine(" + アップデート: " + distfile);
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\r\n[+] フックファイルをコピーしています...");
                    copydir(dirpath, hooks_path); // Copy folder.
                }
            }
            else
            {
                Console.WriteLine("[!] ["+ dirpath +"] が見つかりません。");
            }
        }

        /// <summary>
        /// Read file line
        /// </summary>
        /// <param name="filename">File path</param>
        /// <returns>file line list</returns>
        private static List<string> read_filelines(string filename)
        {
            List<string> src_data = new List<string>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string src_line = "";
                while ((src_line = sr.ReadLine()) != null)
                {
                    src_data.Add(src_line); // Read line.
                }
            }
            return src_data;
        }

        /// <summary>
        /// Add module data
        /// </summary>
        /// <param name="module_name">Add module name</param>
        /// <param name="command">Add module command</param>
        /// <param name="module_data">Add Python module name</param>
        /// <param name="now_dirpath">Now folder path</param>
        public static void Add(string module_name, string command, string module_data, string now_dirpath)
        {
            try
            {
                string hooks_path = now_dirpath + @"\module-hooks";
                using (StreamWriter sw = new StreamWriter(hooks_path + @"\" + module_name + ".txt", true))
                {
                    sw.WriteLine("[" + command + ", " + module_data + "]"); // Add module data.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] "+ ex.Message);
            }
        }

        /// <summary>
        /// Download module-hooks by Github
        /// </summary>
        /// <param name="path">Save path</param>
        public static bool Upgrade(string path, bool clean = false)
        {          
            try
            {
                if (clean) Directory.Delete(path + @"\module-hooks", true);
                string url = "https://github.com/betacode-project/pyDotexe/raw/master/Binary/Latest/module-hooks.zip";
                Console.WriteLine("[+] Module-Hooksデータをダウンロードしています...");
                Console.WriteLine("[+] " + url);
                string zip_path = path + @"\update-hooks.zip";
                // Download module-hooks.
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(url, zip_path);
                wc.Dispose();

                delete_tmp_upgrade(path, zip_path, false); // Delete tmp folder.
                Console.WriteLine("[+] データを展開しています...");
                ICSharpCode.SharpZipLib.Zip.FastZip fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                fastZip.ExtractZip(zip_path, path + @"\update_tmp", "");

                //System.IO.Compression.ZipFile.ExtractToDirectory(zip_path, path + @"\update_tmp"); // Extract downloaded zip file.
                // Start Update.
                Update(path + @"\update_tmp\module-hooks", path);
                delete_tmp_upgrade(path, zip_path);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] "+ ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Delete TMP folder
        /// </summary>
        /// <param name="path">Delete folder</param>
        /// <param name="zip_path">Zip file path</param>
        /// <param name="delete_file">Want deleting files</param>
        private static void delete_tmp_upgrade(string path, string zip_path, bool delete_file=true)
        {
            try
            {
                Directory.Delete(path + @"\update_tmp", true);
                if (delete_file) File.Delete(zip_path);
            } catch { }
        }

        /// <summary>
        /// Copy folder
        /// </summary>
        /// <param name="sourceDirName">Source folder path</param>
        /// <param name="destDirName">Copy to path</param>
        private static void copydir(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }

            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                destDirName = destDirName + Path.DirectorySeparatorChar;

            string[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                File.Copy(file, destDirName + System.IO.Path.GetFileName(file), true);
                Console.WriteLine(" + "+ file);
            }

            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
                copydir(dir, destDirName + Path.GetFileName(dir));
        }
    }
}
