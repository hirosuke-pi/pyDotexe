using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyDotexe.Builder
{
    class CheckFiles
    {
        /// <summary>
        /// Start check BuildSet data
        /// </summary>
        /// <param name="bset">BuildSet Object</param>
        /// <returns></returns>
        public static bool Start(ref BuildSet bset)
        {
            try
            {
                Directory.Delete(bset.tmp_folder_path, true);
            }
            catch { }

            Console.WriteLine("[+] 指定されたパラメータを確認しています...");
            if (!File.Exists(bset.source_path))
            {
                Console.WriteLine("[-] ソースコードが見つかりません: [" + bset.source_path + "]\r\n");
                return false;
            }
            else if (!Directory.Exists(bset.python_path))
            {
                Console.WriteLine("[-] ディレクトリが見つかりません: [" + bset.python_path + "]\r\n");
                return false;
            }
            else if (!File.Exists(bset.python_binary_path))
            {
                Console.WriteLine("[-] Pythonのバイナリファイルが見つかりません: [" + bset.python_binary_path + "]\r\n");
                return false;
            }

            try
            {
                // Can read output binary
                if (bset.output_path == Path.GetFileName(bset.output_path))
                    bset.output_path = bset.source_folder_path + @"\" + bset.output_path;
                using (FileStream fs = new FileStream(bset.output_path, FileMode.Create))
                {
                }
            }
            catch
            {
                if (bset.one_file)
                    bset.output_path = bset.source_folder_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + ".exe";
                else
                    bset.output_path = bset.source_folder_path + @"\pyDotexe_" + Path.GetFileNameWithoutExtension(bset.source_path) +@"\"+ Path.GetFileNameWithoutExtension(bset.source_path) + ".exe";
            }
            return true;
        }
    }
}
