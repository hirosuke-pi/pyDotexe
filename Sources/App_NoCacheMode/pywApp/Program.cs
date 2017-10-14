using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

/*
 * ---------------------------------------------------------------
 *                    None-cache Mode
 * ---------------------------------------------------------------
 */

namespace pywApp
{
    class Program
    {
        static string pathApp = Assembly.GetExecutingAssembly().Location;
        static BootSet bset = null;

        [STAThread]
        static void Main(string[] args)
        {
            bset = new BootSet();
            try
            {
                load_start_filename(); // Get start_filename, python_bin
                bset.Decision();

                create_file();
                StreamWriter sw = new StreamWriter(bset.cachefile, false);
                sw.WriteLine(pathApp); // Write My application path.
                sw.Close(); // Write My application built day.
                GC.Collect(); // Free memory

                try
                {
                    start_process(args); // Start application
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Path.GetFileName(pathApp) + " - Runtime Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Path.GetFileName(pathApp) + " - Application Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        /// <summary>
        /// Start application process
        /// </summary>
        /// <param name="args"></param>
        private static void start_process(string[] args)
        {
            Process pro = null;
            // Start application process.
            if (bset.start_filename == bset.python_bin)
                pro = Process.Start(bset.extract_path + @"\" + bset.python_bin, bset.default_argv + string.Join(" ", args));
            else
                pro = Process.Start(bset.extract_path + @"\" + bset.python_bin, "\""+ bset.extract_path + @"\" + bset.start_filename + "\" " + bset.default_argv + string.Join(" ", args));
            try
            {
                pro.WaitForExit();
                Directory.Delete(bset.delete_folder_path, true);
            }
            catch { }
        }

        /// <summary>
        /// Create any files
        /// </summary>
        private static void create_file()
        {
            // Get zip file data by this application and create zip file.
            load_create_zip();
            if (found_process_name())
            {
                bset.change_extract_path();
                bset.folder_active = true;
            }
            else if (Directory.Exists(bset.extract_path)) Directory.Delete(bset.extract_path, true);

            // Extract zip file.
            ICSharpCode.SharpZipLib.Zip.FastZip fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
            fastZip.ExtractZip(bset.zip_path, Path.GetDirectoryName(bset.extract_path), "");

            File.Delete(bset.zip_path);

            // Copy python library file.
            if (File.Exists(Path.GetDirectoryName(pathApp) + @"\python" + bset.python_ver + ".dll"))
                File.Copy(Path.GetDirectoryName(pathApp) + @"\python" + bset.python_ver + ".dll",
                   bset.extract_path + @"\" + @"\python" + bset.python_ver + ".dll", true);
        }

        /// <summary>
        /// Load header informations (Base64)
        /// </summary>
        private static void load_start_filename()
        {
            int offset = 0;
            // Open this application.
            using (FileStream fs = new FileStream(pathApp, FileMode.Open, FileAccess.Read))
            {
                for (offset = 1; offset <= fs.Length; offset++)
                {
                    fs.Seek(-offset, SeekOrigin.End);
                    if (255 == fs.ReadByte()) // Get header part.
                    {
                        StreamReader sr = new StreamReader(fs);
                        string[] sp_data = Encoding.UTF8.GetString(Convert.FromBase64String((sr.ReadToEnd()))).Split('|');
                        bset.start_filename = sp_data[0]; // Get starting file name.
                        bset.python_bin = sp_data[1]; // Get Python binary file name.
                        bset.python_ver = sp_data[2]; // Get Python version data.
                        if (sp_data[3] != "") bset.default_argv = sp_data[3] + " "; // Get fixed argv data.
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Load zip file and write zip file data
        /// </summary>
        private static void load_create_zip()
        {
            int offset = 0;
            bool true_found = false;
            byte[] header = { 0x50, 0x4b, 0x03 };

            // Open this application.
            using (FileStream fs = new FileStream(pathApp, FileMode.Open, FileAccess.Read))
            {
                for (offset = 0; offset < fs.Length; offset++)
                {
                    // Found zip header
                    if (fs.ReadByte() == 0x50)
                    {
                        offset++;
                        if (fs.ReadByte() == 0x4b)
                        {
                            offset++;
                            if (fs.ReadByte() == 0x03)
                            {
                                if (true_found)
                                {
                                    // Second time only
                                    int zip_byte = (int)fs.Length - offset;
                                    byte[] data = new byte[zip_byte];
                                    fs.Read(data, 0, zip_byte);

                                    // Write zip data.
                                    FileStream fsm = new FileStream(bset.zip_path, FileMode.Create);
                                    fsm.Write(header, 0, header.Length);
                                    fsm.Write(data, 0, zip_byte);
                                    fsm.Close();
                                    break;
                                }
                                else
                                {
                                    offset++;
                                    true_found = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Search my process information
        /// </summary>
        /// <returns></returns>
        private static bool found_process_name()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    string file_path = p.MainModule.FileName;
                    if ((bset.extract_path + @"\" + bset.python_bin).ToLower() == file_path.ToLower()) return true;
                }
                catch { }
            }
            return false;
        }
    }
}