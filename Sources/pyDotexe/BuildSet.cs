using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace pyDotexe
{
    public class BuildSet
    {
        public string path = "";
        public string python_path = "";
        public string source_path = "";
        public string output_path = "";
        public string icon_path = "";
        public string default_argv = "";
        public string hooks_path = "";
        public string module_loader = "";
        public string pyver = "";
        public bool cache = true;
        public bool debug = false;
        public bool all_encodes = true;
        public bool dll_out = false;
        public bool all_imports = false;
        public bool show_window = true;
        public bool search_files = false;
        public bool zip_out = false;
        public bool add_fixed = true;
        public bool one_file = true;
        public bool hooks = true;
        public bool check_only = false;
        public bool optimize = false;
        public int compress = 9;

        public string default_src_ex = ".py";
        public string module_load_path { get; private set; }
        public string source_folder_path { get; private set; }
        public string python_binary_path { get; private set; }
        public string python_lib_path { get; private set; }
        public string tmp_folder_path { get; private set; }
        public string tmp_module_path { get; set; }
        public string py_version { get; set; }
        public string py_version_raw { get; set; }
        public string zip_path { get; set; }
        public string default_python_bin { get; set; }
        public string python_dll { get; set; }
        public string python_ver_dll { get; set; }

        public List<string> base_module = new List<string>();
        public List<string> module_path = new List<string>();
        public List<string> copy_error = new List<string>();
        public List<string> add_modules = new List<string>();
        public List<string> error_modules_list = new List<string>();
        public List<string> default_import = new List<string>();
        public List<string> pyd_files = new List<string>();
        public List<string> dll_paths = new List<string>();
        public List<string> dll_modules = new List<string>();
        public List<string> import_pyfile_list = new List<string>();
        public List<string> import_pydir_list = new List<string>();
        public List<string> import_pydirfile_list = new List<string>();
        public List<string> import_pydirdir_list = new List<string>();
        public List<string> import_pyfilecmd_list = new List<string>();
        public List<string> import_pydircmd_list = new List<string>();
        public List<string> add_pymodules_list = new List<string>();
        public List<string> resource_file = new List<string>();
        public List<string> resource_folder = new List<string>();
        public List<string> except_file_list = new List<string>();
        public List<string> delete_error_list = new List<string>();
        public List<string> module_hooks_list = new List<string>();

        public BuildSet(string my_folder_path)
        {
            path = my_folder_path;
        }

        /// <summary>
        /// Settings data decision and get Python version
        /// </summary>
        public void Decision()
        {
            if (source_path == Path.GetFileName(source_path))
            {
                source_folder_path = Environment.CurrentDirectory;
                source_path = source_folder_path + @"\" + source_path;
            } else source_folder_path = Path.GetDirectoryName(source_path);

            if (icon_path == Path.GetFileName(icon_path))
            {
                if (File.Exists(source_folder_path + @"\" + icon_path))
                    icon_path = source_folder_path + @"\" + icon_path;
                else
                    icon_path = "";
            }

            // Convert modules
            foreach (string lib_name in add_pymodules_list)
                pymodule_to_pyfiledir(lib_name);

            // Set any paths
            if (module_loader == "")
                module_load_path = source_folder_path + @"\module_loader_pyDotexe.py";
            else
                module_load_path = module_loader;

            python_binary_path = python_path + @"\python.exe";
            tmp_folder_path = Path.GetTempPath() + @"pyDotexe_cache";
            python_lib_path = python_path + @"\lib";
            tmp_module_path = tmp_folder_path + @"\" + Path.GetFileNameWithoutExtension(source_path);
            hooks_path = path + @"\module-hooks";
            if (!Directory.Exists(hooks_path) & hooks)
            {
                Console.WriteLine("[!] Module-Hooksのデータが見つかりません。インストールを開始します...\r\n");
                hooks = Hooker.Hook.Upgrade(path);
            }

            if (!one_file) { dll_out = false; zip_out = false; }
            if (show_window) default_python_bin = python_binary_path;
            else default_python_bin = python_path + @"\pythonw.exe";
            base_module.Add("import os, sys"); // merge python path

            // Get Python's version
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = python_binary_path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "-c \"import sys;print(str(sys.version_info[0]) + '.'+ str(sys.version_info[1]) )\"";
            try
            {
                if (pyver == "")
                {
                    p.Start(); // Get python version
                    py_version_raw = p.StandardOutput.ReadLine();
                    py_version = py_version_raw.Replace(".", "");
                    p.WaitForExit();
                    p.Close();
                }
                else
                {
                    py_version = pyver;
                    py_version_raw = pyver[0] + "." + pyver[1];
                }
                Console.WriteLine("\r\n[+] Pythonのバージョンを検出しました: " + py_version_raw + ".x");

                version_selecter();
                // Set including python library and binary
                default_import = new List<string>()
                {
                    source_path,
                    default_python_bin,
                    python_dll
                };
                if (!dll_out) default_import.Add(python_ver_dll);
                default_import.RemoveAll(str => str == null);
            }
            catch (DLLError)
            {
                Console.WriteLine("[-] Pythonのライブラリファイルが見つかりません: [python"+ py_version +".dll]");
            }
            catch
            {
                Console.WriteLine("[-] Pythonのバイナリファイルが見つかりません: [" + python_binary_path + "]");
            }
        }

        private class DLLError : Exception
        {
            public DLLError() { }
        }

        /// <summary>
        /// Add import_pyfile list
        /// </summary>
        /// <param name="lib_name"></param>
        public void pymodule_to_pyfiledir(string lib_name)
        {
            string name = @"\" + lib_name.Replace(".", @"\");
            import_pyfile_list.AddRange(new string[] { name + ".py", name + ".pyd" });
            import_pydir_list.Add(name);
        }

        /// <summary>
        /// Select Python version
        /// </summary>
        private void version_selecter()
        {
            if (int.Parse(py_version) >= 35) 
            {
                // Python 3.5.x ~
                python_dll = python_path + @"\python" + py_version[0] + ".dll";
                python_ver_dll = python_path + @"\python" + py_version + ".dll";
                base_module.Add("import os, sys, ctypes, codecs");
            }
            else
            {             
                string dllx86 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + @"\python" + py_version + ".dll";
                string dllx64 = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\python" + py_version + ".dll";
                if (File.Exists(dllx86)) python_ver_dll = dllx86;
                else if (File.Exists(dllx64)) python_ver_dll = dllx64;
                else throw new DLLError();

                if (int.Parse(py_version) >= 26)
                {
                    base_module.Add("import os, sys, ctypes, codecs");
                    if (int.Parse(py_version) >= 32)
                        // Python 3.2.x ~ Python 3.4.x
                        python_dll = python_path + @"\DLLs\python" + py_version[0] + ".dll";
                    else
                        // Python 2.5.x ~ 2.7.x
                        python_dll = null;
                }
                else base_module.Add("import os, sys"); // ~ Python 2.5
            }      
        }
    }
}
