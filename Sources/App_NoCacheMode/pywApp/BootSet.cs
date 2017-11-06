using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pywApp
{
    class BootSet
    {
        public string load_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Program\Cache";
        public string python_bin = "";
        public string python_ver = "";
        public string start_filename = "";
        public string default_argv = "";
        public string extract_path = "";
        public string random_str = "";
        public string zip_path = "";
        public string delete_folder_path = "";
        public string cachefile = "";

        public bool folder_active = false;
        public bool start_out_bin = false;

        public BootSet()
        {
            random_str = "_" + RandomString(20);
        }

        public void change_extract_path()
        {
            // Change extract file path.
            extract_path = Path.GetDirectoryName(extract_path) + @"\" + Path.GetFileNameWithoutExtension(start_filename) + random_str + @"\" + Path.GetFileNameWithoutExtension(start_filename);
            delete_folder_path = Path.GetDirectoryName(extract_path);
        }

        public void Decision()
        {
            // Set any paths and create folder.
            extract_path = load_path + @"\" + Path.GetFileNameWithoutExtension(start_filename) + random_str + @"\" + Path.GetFileNameWithoutExtension(start_filename);
            zip_path = load_path + @"\" + Path.GetFileNameWithoutExtension(start_filename) + random_str + ".zip";
            delete_folder_path = Path.GetDirectoryName(extract_path);
            cachefile = extract_path + @"\cache.sys";
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
                //文字の位置をランダムに選択
                int pos = r.Next(passwordChars.Length);
                //選択された位置の文字を取得
                char c = passwordChars[pos];
                //パスワードに追加
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
