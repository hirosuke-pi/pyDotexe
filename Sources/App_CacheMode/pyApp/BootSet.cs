using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyApp
{
    class BootSet
    {
        public string load_path = Path.GetTempPath() + @"Program\Cache";
        public string python_bin = "";
        public string python_ver = "";
        public string start_filename = "";
        public string default_argv = "";
        public string cachefile = "";
        public string extract_path = "";
        public string random_str =  "";
        public string zip_path = "";
        public string delete_folder_path = "";

        public bool folder_active = false;
        public bool start_out_bin = false;

        public BootSet()
        {
            random_str = "_"+ RandomString(20);
        }

        public void change_extract_path()
        {
            // Change extract file path.
            extract_path = Path.GetDirectoryName(extract_path) + @"\" + Path.GetFileNameWithoutExtension(start_filename) + random_str + @"\" + Path.GetFileNameWithoutExtension(start_filename);
            cachefile = extract_path + @"\cache.sys";
            delete_folder_path = Path.GetDirectoryName(extract_path);
        }

        public void Decision()
        {
            // Set any paths.
            extract_path = load_path + @"\" + Path.GetFileNameWithoutExtension(start_filename);
            cachefile = extract_path + @"\cache.sys";
            zip_path = load_path + @"\" + Path.GetFileNameWithoutExtension(start_filename) + random_str + ".zip";
            delete_folder_path = extract_path;
            new DirectoryInfo(load_path).Create();
        }

        // Get random string data.
        private static readonly string passwordChars = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static string RandomString(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++)
            {
                int pos = r.Next(passwordChars.Length);
                char c = passwordChars[pos];

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
