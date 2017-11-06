using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace pyfApp
{
    class Program
    {
        static string pathApp = Assembly.GetExecutingAssembly().Location;
        static BootSet bset = null;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                bset = new BootSet(Path.GetDirectoryName(pathApp));
                // Load header informations.
                load_start_filename();
                // Write cache data.
                StreamWriter sw = new StreamWriter(bset.cachefile, false);
                sw.WriteLine(pathApp); // Write this application path.
                sw.WriteLine(File.GetLastWriteTime(pathApp)); // Write this application built day.
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Path.GetFileName(pathApp) + " - Application Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Start application process.
            try
            {
                Process pro = null;
                string start_bin = "";
                if (bset.start_out_bin) start_bin = bset.dir_path + @"\" + bset.start_filename;
                else start_bin = bset.dir_path + @"\" + bset.python_bin;

                if ((bset.start_filename == bset.python_bin) | bset.start_out_bin)
                    pro = Process.Start(start_bin, bset.default_argv + string.Join(" ", args));
                else
                    pro = Process.Start(start_bin, "\""+ bset.dir_path + @"\" + bset.start_filename + "\" " + bset.default_argv + string.Join(" ", args));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Path.GetFileName(pathApp) + " - Runtime Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        /// <summary>
        /// Load this application header (Base64)
        /// </summary>
        private static void load_start_filename()
        {
            int offset = 0;
            using (FileStream fs = new FileStream(pathApp, FileMode.Open, FileAccess.Read))
            {
                for (offset = 1; offset <= fs.Length; offset++)
                {
                    fs.Seek(-offset, SeekOrigin.End);
                    if (255 == fs.ReadByte()) // Get header part.
                    {
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                        string[] sp_data = Encoding.UTF8.GetString(Convert.FromBase64String((sr.ReadToEnd()))).Split('|');
                        bset.start_filename = sp_data[0]; // Get starting file name.
                        bset.python_bin = sp_data[1]; // Get Python binary file name.
                        bset.python_ver = sp_data[2]; // Get Python version data.
                        bset.start_out_bin = bool.Parse(sp_data[3]); // Get start out binary.
                        if (sp_data[3] != "") bset.default_argv = sp_data[3] + " "; // Get fixed argv data.
                        break;
                    }
                }
            }
            GC.Collect();
        }
    }
}
