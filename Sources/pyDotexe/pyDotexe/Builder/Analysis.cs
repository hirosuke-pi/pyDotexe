using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace pyDotexe.Builder
{
    class Analysis
    {
        private static BuildSet bset = null;

        private class GetImportDataError : Exception
        {
            public GetImportDataError(string Message) { }
        }
        private class MakeModuleLoaderError : Exception
        {
            public MakeModuleLoaderError(string Message) { }
        }
        private class GetImportFilePathError : Exception
        {
            public GetImportFilePathError() { }
        }
        private class GetModulesByProcessError : Exception
        {
            public GetModulesByProcessError(string Message) { }
        }

        /// <summary>
        /// Start source-file analysis
        /// </summary>
        /// <param name="BuildSettings">BuildSet Object</param>
        /// <returns></returns>
        public static bool Start(ref BuildSet BuildSettings)
        {
            try
            {
                bset = BuildSettings;
                if (!bset.standalone)
                {
                    bset.module_path.Add(bset.source_path);
                    bset.except_file_list.Add(bset.python_path.Replace(@"\", @"\\"));
                }

                Console.WriteLine("[+] Getting imported default modules path...");
                get_import_modules_name(); // Get imported module names by source code.
                make_module_loader(); // // Make module loader file. (pyDotexe_module_loader.py)
                get_import_file_path(); // Start module loader by Python-Shell and Get module paths.

                Console.WriteLine("\r\n[+] Searching selected modules...");
                if (bset.hooks) check_copy_hooks(); // Check some modules.
                import_pyfilecmd(); // Search selected python file by Regex.
                import_pydircmd(); // Search seletcted python folder by Regex.

                if (bset.except_file_list.Count != 0)
                {
                    Console.WriteLine("\r\n[+] Excluding selected module files and folders...");
                    exc_pyfile(); // Exclude selected module names by file.
                }

                if (extract_pyd()) // Search for pyd files by Python module paths list
                {
                    Console.WriteLine("\r\n[+] Getting Dynamic-Link-Library by *.pyd files...");
                    analysis_pyd(); // Open pyd files and Get some DLL file names.
                    search_dll_in_folder(); // Search DLL file by names.
                }

                if (bset.check_only)
                {
                    Console.WriteLine("\r\n[+] Getting default Python libararies and binaries...");
                    foreach (string default_modules in bset.default_import)
                        Console.WriteLine(" + " + default_modules);

                    Console.WriteLine("\r\n[+] Imported module name list:");
                    foreach (string suc_module in bset.module_hooks_list)
                        Console.Write(suc_module + ", ");

                    Console.WriteLine("\r\n\r\n[-] Could not get module path list:");
                    foreach (string err_module in bset.error_modules_list)
                        Console.Write(err_module + ", ");

                    Console.WriteLine("\r\n\r\n\a[+] Get module Python function: sys.modules.keys()");
                    Console.WriteLine("[+] Completed getting Python module and library files.");
                    return false;
                }

                return true;
            }
            catch (GetImportDataError ex)
            {
                Console.WriteLine("[-] Could not read main-source file.\r\n[!] " + ex.Message);
            }
            catch (MakeModuleLoaderError ex)
            {
                Console.WriteLine("[-] Could not make 'module_load_pyDotexe.py' file.\r\n[!] " + ex.Message);
            }
            catch (GetImportFilePathError)
            {
                Console.WriteLine("[-] Import error found. Please check your code or file path.");
            }
            catch (GetModulesByProcessError ex)
            {
                Console.WriteLine("[-] Could not get python modules.\r\n[!] " + ex.Message);
            }
            finally
            {
                try
                {
                    File.Delete(bset.module_load_path);
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Get imported modules name
        /// </summary>
        private static void get_import_modules_name()
        {
            try
            {
                using (StreamReader sr = new StreamReader(bset.source_path))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.TrimStart(' ');
                        if (line.StartsWith("import") | line.StartsWith("from"))
                        {
                            bset.base_module.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new GetImportDataError(ex.Message);
            }
        }

        /// <summary>
        /// Make module loader file
        /// </summary>
        private static void make_module_loader()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(bset.module_load_path))
                {
                    foreach (string module_line in bset.base_module)
                    {
                        sw.WriteLine(module_line);
                    }

                    // Make module loader.
                    if (int.Parse(bset.py_version) >= 21)
                    {
                        sw.WriteLine(
                            "import sys\r\n" +
                            "module_list = sys.modules.keys()\r\n" +
                            "import inspect\r\n" +
                            "for module in module_list:\r\n" +
                            "    try:\r\n" +
                            "        #exec(\"import \"+ module+ \";print(' + '+ \"+ module +\".__file__)\")\r\n" +
                            "        exec(\"import \"+ module+ \", inspect;print(' + '+ inspect.getfile(\"+ module +\"))\")\r\n" +
                            "        print(' * '+ module)\r\n" +
                            "    except:\r\n" +
                            "        print(' - '+ module)\r\n");
                    }
                    else
                    {
                        sw.WriteLine(
                            "import sys\r\n" +
                            "module_list = sys.modules.keys()\r\n" +
                            "for module in module_list:\r\n" +
                            "    try:\r\n" +
                            "        exec(\"import \"+ module+ \";print(' + '+ \"+ module +\".__file__)\")\r\n" +
                            "        print(' * '+ module)\r\n" +
                            "    except:\r\n" +
                            "        print(' - '+ module)\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MakeModuleLoaderError(ex.Message);
            }
        }

        /// <summary>
        /// Get imported file path
        /// </summary>
        private static void get_import_file_path()
        {
            if (bset.all_encodes)
            {
                try
                {
                    // Use all encodings.
                    foreach (string enc_path in Directory.EnumerateFiles(bset.python_lib_path + @"\encodings", "*.py", SearchOption.AllDirectories))
                    {
                        Console.WriteLine(" + " + enc_path);
                        bset.module_path.Add(enc_path);
                    }
                } catch { }
            }
            if (bset.add_modules.Count != 0)
            {
                // Include selected module paths.
                foreach (string add_modules in bset.add_modules)
                {
                    try
                    {
                        foreach (string add_modules_path in Directory.EnumerateFiles(add_modules, "*", SearchOption.AllDirectories))
                        {
                            Console.WriteLine(" + " + add_modules_path);
                            bset.module_path.Add(add_modules_path);
                        }
                    }
                    catch { }
                }
            }
            int before_module_count = 0;
            try
            {
                before_module_count = bset.module_path.Count;
                get_module_path_by_process(); // Start module loader.
            }
            catch (Exception ex)
            {
                throw new GetModulesByProcessError(ex.Message);
            }
            if (bset.module_path.Count == before_module_count)
            {
                throw new GetImportFilePathError();
            }
        }

        /// <summary>
        /// Search modules by module-hooks
        /// </summary>
        private static void check_copy_hooks()
        {
            // Search module-hooks.
            List<string> tmp_module_list = new List<string>();
            foreach (string hook_file in Directory.EnumerateFiles(bset.hooks_path, "*", SearchOption.AllDirectories))
            {
                foreach (string module_name in bset.module_hooks_list)
                {
                    if (Path.GetFileNameWithoutExtension(hook_file) == module_name)
                        tmp_module_list.Add(hook_file);
                }
            }

            // Include found module-hooks module.
            foreach (string module_list in tmp_module_list)
            {
                using (StreamReader sr = new StreamReader(module_list))
                {
                    while (sr.Peek() >= 0)
                    {
                        try
                        {
                            string raw_data = sr.ReadLine();
                            string data = raw_data.Replace(" ", "").Replace("[", "").Replace("]", "");
                            string name = data.Split(',')[1];
                            string cmd = data.Split(',')[0].ToLower();
                            List<int> pyver = new List<int>();
                            bool is_version = false;
                            string ver_tmp = "";

                            try
                            {
                                ver_tmp = data.Split(',')[2];
                                if (bset.py_version[0] + "x" != ver_tmp) pyver.Add(0); // 3x or 2x
                                try
                                {
                                    for (int i = int.Parse(ver_tmp.Split('-')[0]); i < int.Parse(ver_tmp.Split('-')[1]); i++)
                                        pyver.Add(i); // ex) 2.1-2.7
                                }
                                catch
                                {
                                    pyver.Add(int.Parse(ver_tmp));
                                }
                            } catch { }

                            foreach (int ver in pyver)
                                if (ver == int.Parse(bset.py_version))
                                    is_version = true;

                            if (pyver.Count == 0) is_version = true;
                            if (is_version)
                            {
                                string[] names = name.Split('/');
                                if (cmd == "file") bset.module_path.AddRange(names);
                                else if (cmd == "dir") bset.add_modules.AddRange(names);
                                else if (cmd == "pyfile") bset.import_pyfile_list.AddRange(names);
                                else if (cmd == "pydir") bset.import_pydir_list.AddRange(names);
                                else if (cmd == "pymodule") bset.pymodule_to_pyfiledir(name);
                                else if (cmd == "exclude")
                                {
                                    string argv = raw_data.Substring(raw_data.IndexOf(',') + 1).TrimEnd(']').TrimStart(' ');
                                    bset.except_file_list.Add(argv);
                                }
                                else if (cmd == "--no-rev") bset.add_fixed = false;
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        // Get pydir in folders and files
        private static void get_pydirfile(string folder_path)
        {           
            foreach (string add_modules_path in Directory.EnumerateFileSystemEntries(folder_path, "*", SearchOption.AllDirectories))
            {
                if (Directory.Exists(add_modules_path))
                {
                    get_pydirfile(add_modules_path);
                }
                else
                {
                    Console.WriteLine(" + " + add_modules_path);
                    bset.module_path.Add(add_modules_path);
                }
            }
        }

        private static void import_pyfilecmd()
        {
            List<Task> pyfile_task = new List<Task>();
            foreach (string regex_data in distinct_list(bset.import_pyfile_list))
            {
                pyfile_task.Add(Task.Factory.StartNew(new Action(() =>
                {
                    foreach (string file in Directory.EnumerateFiles(bset.python_path, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            if (Regex.IsMatch(file, regex_data))
                            {
                                bset.module_path.Add(file);
                                Console.WriteLine(" + " + file);
                                if (!bset.all_search) break;
                            }
                        }
                        catch (Exception ex)
                        {
                            bset.copy_error.Add(ex.Message);
                            break;
                        }
                    }
                })));
            }
            Task.WaitAll(pyfile_task.ToArray());
            GC.Collect();
        }

        private static void import_pydircmd()
        {
            List<Task> pydir_task = new List<Task>();
            foreach (string regex_data in distinct_list(bset.import_pydir_list))
            {
                pydir_task.Add(Task.Factory.StartNew(new Action(() =>
                {
                    foreach (string folder in Directory.EnumerateDirectories(bset.python_path, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            if (Regex.IsMatch(folder, regex_data))
                            {
                                get_pydirfile(folder);
                                if (!bset.all_search) break;
                            }
                        }
                        catch (Exception ex)
                        {
                            bset.copy_error.Add(folder + "\r\n   @" + ex.Message);
                            break;
                        }
                    }
                })));
            }
            Task.WaitAll(pydir_task.ToArray());
            GC.Collect();
        }

        private static List<string> distinct_list(List<string> dist_list)
        {
            return new List<string>(dist_list.Distinct());
        }

        // Start module loader
        private static void get_module_path_by_process()
        {
            // Build process class
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += p_OutputDataReceived;

            p.StartInfo.FileName = bset.python_path + @"\python.exe";
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.Arguments = "\""+ bset.module_load_path +"\"";

            // Start Python process
            p.Start();
            p.BeginOutputReadLine();

            p.WaitForExit();
            p.Close();
        }

        // Get any Python module paths and module names
        private static void p_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {         
            try
            {
                string filepath = e.Data.Remove(0, 3);
                if (filepath != bset.module_load_path)
                {
                    if (e.Data.StartsWith(" + "))
                    {
                        bset.module_path.Add(filepath);
                        Console.WriteLine(e.Data);
                    }
                    else if (e.Data.StartsWith(" * "))
                    {
                        bset.module_hooks_list.Add(filepath);
                    }
                    
                    else if (e.Data.StartsWith(" - "))
                    {
                        if (bset.search_files)
                        {
                            bset.import_pyfile_list.Add(filepath + ".py");
                            bset.import_pyfile_list.Add(filepath + ".pyd");
                        }
                        else if (bset.check_only)
                        {
                            bset.error_modules_list.Add(filepath);
                        }
                    }                   
                }
            }
            catch { }           
        }

        /// <summary>
        /// Exclude Python files
        /// </summary>
        private static void exc_pyfile()
        {
            List<string> tmp_module_path = new List<string>();
            bool include = true;

            foreach (string file_name in bset.module_path)
            {
                foreach (string regex_data in bset.except_file_list)
                {
                    try
                    {
                        if (Regex.IsMatch(file_name, regex_data))
                        {
                            Console.WriteLine(" - " + file_name);
                            include = false;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        bset.copy_error.Add(regex_data + "\r\n   @" + ex.Message);
                        break;
                    }
                }
                if (include)
                    tmp_module_path.Add(file_name);
                else
                    include = true;
            }
            bset.module_path = new List<string>(tmp_module_path);
        }

        /// <summary>
        /// Search pyd files
        /// </summary>
        /// <returns>Found?</returns>
        private static bool extract_pyd()
        {
            foreach (string module_path in bset.module_path)
            {
                if (module_path.EndsWith(".pyd"))
                {
                    bset.pyd_files.Add(module_path);
                }
            }

            if (bset.pyd_files.Count == 0) return false;
            else return true;
        }

        /// <summary>
        /// Open pyd file and Get DLL file names
        /// </summary>
        private static void analysis_pyd()
        {
            List<Task> extract = new List<Task>();
            foreach (string dll_file in bset.pyd_files)
            {
                extract.Add(Task.Factory.StartNew(new Action(() =>
                {
                    // Open dll file.
                    FileStream fs = new FileStream(dll_file, FileMode.Open, FileAccess.Read);
                    byte[] file_data = new byte[fs.Length];
                    fs.Read(file_data, 0, (int)fs.Length);
                    fs.Close();
                    string[] data = Encoding.UTF8.GetString(file_data).Split(Convert.ToChar(0x00));
                    file_data = null;

                    for (int i = 0; i < data.Length; i++)
                    {
                        // Search *.dll file names
                        if (data[i].EndsWith(".dll"))
                            bset.dll_modules.Add(data[i]);
                    }                   
                    GC.Collect(); // Free data.
                })));
            }
            
            Task.WaitAll(extract.ToArray());         
            bset.dll_modules = new List<string>(bset.dll_modules.Distinct().ToArray());
            GC.Collect(); // All free binary data.
        }

        /// <summary>
        /// Search DLL files
        /// </summary>
        private static void search_dll_in_folder()
        { 
            List<string> pyd_folders = new List<string>();
            List<string> all_folders = new List<string>();

            foreach (string pyd_file in bset.pyd_files)
            {
                if (Path.GetDirectoryName(pyd_file) != bset.python_lib_path + @"\site-packages")
                    pyd_folders.Add(Path.GetDirectoryName(pyd_file));
            }

            bset.pyd_files = null;
            pyd_folders = new List<string>(pyd_folders.Distinct().ToArray());
            all_folders.AddRange(pyd_folders);
            
            foreach (string pyd_folder in pyd_folders)
            {
                foreach (string folder_path in Directory.EnumerateDirectories(pyd_folder, "*", SearchOption.AllDirectories))
                {
                    all_folders.Add(folder_path);
                }
            }
            foreach (string folder_path in all_folders)
            {
                foreach (string dll_name in bset.dll_modules)
                {
                    if (File.Exists(folder_path + @"\" + dll_name))
                    {
                        bset.module_path.Add(folder_path + @"\" + dll_name);
                        Console.WriteLine(" + " + folder_path + @"\" + dll_name);
                    }
                }
            }
            bset.dll_modules = null;
        }       
    }
}
