using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyDotexe
{
    public class MergeSet
    {
        public string src_zip = "";
        public string start_bin = "";
        public string start_src = "";
        public string start_argv = "";
        public string version = "00";
        public string icon_path = "";
        public string output_path = "";
        public bool use_cache = true;
        public bool onefile = true;
        public bool out_bin = false;

        public bool Decision()
        {
            // Found source file.
            if (src_zip == Path.GetFileName(src_zip))
                src_zip = Environment.CurrentDirectory + @"\" + src_zip;
            if (start_src == "") start_src = start_bin;
            if (!onefile) use_cache = false;

            // Is Zip file?
            if (!File.Exists(src_zip) & (Path.GetFileNameWithoutExtension(src_zip).ToLower() != ".zip") & onefile)
            {
                Console.WriteLine("[-] Selected file is not ZIP file or the file is not found.");
                return false;
            }
            // Set icon.
            if (icon_path == Path.GetFileName(icon_path))
            {
                if (File.Exists(Path.GetDirectoryName(src_zip) + @"\" + icon_path))
                    icon_path = Path.GetDirectoryName(src_zip) + @"\" + icon_path;
                else
                    icon_path = "";
            }
            if (output_path == "")
                if (onefile)
                    output_path = Path.GetDirectoryName(src_zip) + @"\" + Path.GetFileNameWithoutExtension(start_src) + ".exe";
                else
                {
                    output_path = src_zip + @"\" + Path.GetFileNameWithoutExtension(start_src) + ".exe";
                    if (!Directory.Exists(Path.GetDirectoryName(output_path))) new DirectoryInfo(output_path).Create();
                }
            else
                if (output_path == Path.GetFileName(output_path))
                output_path = Path.GetDirectoryName(src_zip) + @"\" + output_path;
            return true;
        }

        public MergeSet ConvertMergeSet(BuildSet bset)
        {
            MergeSet mset = new MergeSet();
            mset.src_zip = bset.zip_path;
            mset.start_bin = Path.GetFileName(bset.default_python_bin);
            mset.start_src = Path.GetFileNameWithoutExtension(bset.source_path) + bset.default_src_ex;
            mset.start_argv = bset.default_argv;
            mset.version = bset.py_version;
            mset.icon_path = bset.icon_path;
            mset.output_path = bset.output_path;
            mset.use_cache = bset.cache;
            mset.onefile = bset.one_file;
            mset.out_bin = !bset.standalone;
            mset.Decision();

            return mset;
        }
    }
}
