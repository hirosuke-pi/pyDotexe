using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pyfApp
{
    public class BootSet
    {
        public string python_bin = "";
        public string python_ver = "";
        public string start_filename = "";
        public string default_argv = "";
        public string cachefile = "";
        public string dir_path = "";
        public bool start_out_bin = false;

        public BootSet(string main_dir_path)
        {
            dir_path = main_dir_path; // Set main folder path.
            cachefile = dir_path + @"\cache.sys"; // Set cache file path.
        }
    }
}

