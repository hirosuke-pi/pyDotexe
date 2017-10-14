using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Reflection;


namespace pyDotexe
{
    class Program
    {
        static string version = "v1.0.0-Hydrogen";
        static string py_surpport = "1.6.x, 2.1.x-2.7.x, 3.0.x-3.6.x";
        static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // Local Path S
        static string my_path = Assembly.GetExecutingAssembly().Location;
        static bool wait_key = false;
        static bool logger = true;
        static BuildSet bset = null;

        [STAThread]
        // Main
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                string cmd_args = args[0].ToLower();
                if (cmd_args == "-build") Main_build(args);　// Start build
                else if (cmd_args == "-merge") Main_merge(args); // Start merge
                else if (cmd_args == "-hooks") Main_hooks(args); // Start Module-Search
                else if (cmd_args == "-help") System.Diagnostics.Process.Start("https://github.com/betacode-project/pyDotexe");
                else if (cmd_args == "-version") Console.WriteLine(version + "\r\n" + py_surpport);
                else Main_short_help();              
            }
            else Main_short_help(); // 
            if (wait_key)
            {
                Console.WriteLine("\r\n[+] Press any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Start Build
        /// </summary>
        /// <param name="args">Default Args</param>
        static void Main_build(string[] args)
        {           
            bset = new BuildSet(path);
            if (path.EndsWith(@"\Scripts") & File.Exists(path.Replace(@"\Scripts", "") + @"\python.exe"))
                bset.python_path = path.Replace(@"\Scripts", "");
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (args[i] == "-src" | args[i] == "-s") bset.source_path = args[++i]; // Set source code file path.
                    else if (args[i] == "-py" | args[i] == "-p") bset.python_path = args[++i]; // Set Python folder path.
                    else if (args[i] == "-out" | args[i] == "-o") bset.output_path = args[++i]; // Set binary saving path.
                    else if (args[i] == "-icon" | args[i] == "-I") bset.icon_path = args[++i]; // Set icon path.
                    else if (args[i] == "-argv" | args[i] == "-a") bset.default_argv = args[++i]; // Set fixed argv data.
                    else if (args[i] == "-file" | args[i] == "-f") bset.module_path.Add(args[++i]); // Set added file path.
                    else if (args[i] == "-dir" | args[i] == "-d") bset.add_modules.Add(args[++i]); // Set added folder path.
                    else if (args[i] == "-import" | args[i] == "-i") bset.base_module.Add("import " + args[++i]); // Set imported Python modules.
                    else if (args[i] == "-pyfile" | args[i] == "-pf") bset.import_pyfile_list.AddRange(args[++i].Split(',')); // Set Python file name.
                    else if (args[i] == "-pydir" | args[i] == "-pd") bset.import_pydir_list.AddRange(args[++i].Split(',')); // Set Python folder name.
                    else if (args[i] == "-pyfile-regex" | args[i] == "-pfr") bset.import_pyfilecmd_list.Add(args[++i]); // Set file search options.
                    else if (args[i] == "-pydir-regex" | args[i] == "-pdr") bset.import_pydircmd_list.Add(args[++i]); // Set folder search options.
                    else if (args[i] == "-pydirdir" | args[i] == "-pdd") bset.import_pydirdir_list.AddRange(args[++i].Split(',')); // Set only Python folder in folders name. 
                    else if (args[i] == "-pydirfile" | args[i] == "-pdf") bset.import_pydirfile_list.AddRange(args[++i].Split(',')); // Set only Python folder in files name.
                    else if (args[i] == "-pymodule" | args[i] == "-pm") bset.add_pymodules_list.AddRange(args[++i].Split(',')); // Set Python module name.
                    else if (args[i] == "-resfile" | args[i] == "-rf") bset.resource_file.AddRange(args[++i].Split(',')); // Set resource file name.
                    else if (args[i] == "-resdir" | args[i] == "-rd") bset.resource_folder.AddRange(args[++i].Split(',')); // Set resource folder name.
                    else if (args[i] == "-exclude" | args[i] == "-e") bset.except_file_list.Add(args[++i]); // Set excluding Python module file.
                    else if (args[i] == "-loader" | args[i] == "-l") bset.except_file_list.Add(args[++i]); // Set own module loader
                    else if (args[i] == "-pyver" | args[i] == "-pv") bset.pyver = args[++i];
                    else if (args[i].StartsWith("-comp")) bset.compress = int.Parse(args[i].Split('=')[1]); // Set compression levels.

                    else if (args[i] == "--no-cache") bset.cache = false; // Do not use cache.
                    else if (args[i] == "--debug" ) bset.debug = true; // Use debug mode.
                    else if (args[i] == "--dll-out") bset.dll_out = true; // Dll file out mode.
                    else if (args[i] == "--all-includes") bset.all_imports = true; // Include all python modules.
                    else if (args[i] == "--hide") bset.show_window = false; // Do not show Console.
                    else if (args[i] == "--research") bset.search_files = true; // More search modules mode.
                    else if (args[i] == "--zip-out") bset.zip_out = true; // Only output zip file.
                    else if (args[i] == "--wait-key") wait_key = true; // Wait for pressed any keys.
                    else if (args[i] == "--no-rev") bset.add_fixed = false; // Do not fix the file path.
                    else if (args[i] == "--part-enc") bset.all_encodes = false; // Only use part of encodings.
                    else if (args[i] == "--no-log") logger = false; // Do not take a log.
                    else if (args[i] == "--no-hooks") bset.hooks = false; // Do not use module-hooks.
                    else if (args[i] == "--no-onefile") bset.one_file = false; // Do not make one file.
                    else if (args[i] == "--optimize") bset.optimize = true; // Optimize Python files
                    else if (args[i] == "--check-only") bset.check_only = true; // Check module list only
                    else if (args[i] == "-build") { }
                    else Console.WriteLine("[!] No named command options: " + args[i]);
                    }
                catch (Exception) { }
            }        
            bset.Decision(); // Get python version and get any paths.
            Console.Title = "pyDotexe "+ version +" - Build: "+ bset.source_path;
            if (bset.check_only) Console.Title = Console.Title + " (Path)";
            else if (bset.debug) Console.Title = Console.Title + " (Debug)";
            else if (bset.zip_out) Console.Title = Console.Title + " (ZIP-Out)";
            if (logger) Main_make_log(args, "pyDotexe_Build_log.txt"); // Write logs.

            // check data, analysis and build.
            if (!Builder.CheckFiles.Start(ref bset)) return;
            if (!Builder.Analysis.Start(ref bset)) return;
            if (!Builder.Build.Start(ref bset)) return;
            GC.Collect();

            // Build successful.
            Console.WriteLine("\a\r\n[+] Build succeeded! (Python "+ bset.py_version_raw +".x)");
            Console.WriteLine("[*] " + bset.output_path);
            if (bset.copy_error.Count > 0)
            {
                Console.WriteLine("\r\n[-] Some files could not be copied.");
                foreach (string error_data in bset.copy_error)
                {
                    Console.WriteLine(" - " + error_data);
                }
            }
            if (bset.delete_error_list.Count > 0)
            {
                Console.WriteLine("\r\n[-] Some files could not be deleted.");
                foreach (string error_data in bset.copy_error)
                {
                    Console.WriteLine(" - " + error_data);
                }
            }            
        }

        /// <summary>
        /// Start Merge
        /// </summary>
        /// <param name="args"></param>
        static void Main_merge(string[] args)
        {
            MergeSet mset = new MergeSet();
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (args[i] == "-src" | args[i] == "-s") mset.src_zip = args[++i];
                    else if (args[i] == "-out" | args[i] == "-o") mset.output_path = args[++i];
                    else if (args[i] == "-ver" | args[i] == "-v") mset.version = args[++i];
                    else if (args[i] == "-icon" | args[i] == "-I") mset.icon_path = args[++i];
                    else if (args[i] == "-start-bin" | args[i] == "-sb") mset.start_bin = args[++i];
                    else if (args[i] == "-start-src" | args[i] == "-ss") mset.start_src = args[++i];
                    else if (args[i] == "-start-argv" | args[i] == "-sa") mset.start_argv = args[++i];
                    else if (args[i] == "--no-cache") mset.use_cache = false;
                    else if (args[i] == "--no-onefile") mset.onefile = false;
                    else if (args[i] == "--wait-key") wait_key = true;
                    else if (args[i] == "--no-log") logger = false;
                    else if (args[i] == "-merge") { }
                    else Console.WriteLine("[!] No named command options: " + args[i]);
                }
                catch { }
            }

            Console.Title = "pyDotexe " + version + " - Merge: " + mset.src_zip;
            if (logger) Main_make_log(args, "pyDotexe_Merge_log.txt");
            if (!mset.Decision()) return;
            if (!Merger.Merge.Start(ref mset)) return;

            Console.WriteLine("\a\r\n[+] Merging succeeded!");
            Console.WriteLine("[*] "+ mset.output_path);      
        }

        static void Main_hooks(string[] args)
        {
            Console.Title = "pyDotexe " + version + " - Hooks";
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (args[i] == "-update" | args[i] == "-u") Hooker.Hook.Update(args[++i], path);
                    else if (args[i] == "-add" | args[i] == "-a") Hooker.Hook.Add(args[++i], args[++i], args[++i], path);
                    else if (args[i] == "--upgrade-clean" | args[i] == "--upc") Hooker.Hook.Upgrade(path, true);
                    else if (args[i] == "--upgrade" | args[i] == "--up") Hooker.Hook.Upgrade(path);
                    else if (args[i] == "--clear" | args[i] == "--c") Directory.Delete(path +@"\module-hooks", true);
                    else if (args[i] == "--wait-key") wait_key = true;
                    else if (args[i] == "--no-log") logger = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[!] "+ ex.Message);
                }
            }
            if (logger) Main_make_log(args, "pyDotexe_Hook_log.txt");
            Console.WriteLine("\a\r\n[+] Completed Changing Module-Hook files data.");
        }

        static void Main_short_help()
        {
            Console.WriteLine("\r\n" +
                "pydotexe -build -src [SOURCE_PATH] (-py [PYTHON_FOLDER_PATH])\r\n" +
                "         (-out [OUT_PATH]) (-icon [ICON_PATH]) (-argv [FIXED_ARGV(string)])\r\n" +
                "         (-file [FILE_PATH]) (-dir [FOLDER_PATH]) (-import [MODULE_NAME])\r\n" +
                "         (-pyfile [PYTHON_FILE_NAME]) (-pydir [PYTHON_FOLDER_NAME])\r\n" +
                "         (-pydirfile [PYTHON_FOLDER_NAME]) (-pydirdir [PYTHON_FOLDER_NAME])\r\n"+
                "         (-pyfile-regex [REGEX_DATA]) (-pydir-regex [REGEX_DATA]\r\n" +
                "         (-resfile [RESOUCE_FILE_NAME]) (-resdir [RESOURCE_FOLDER_NAME])\r\n" +
                "         (-exclude [EXCLUDE_MODULE_REGEX]) (-pymodule [PYTHON_MODULE_NAME])\r\n" +
                "         (-loader [SOURCE_PATH]) (-comp=[COMPRESS_LEVELS([LOW]0~9[HIGH])])\r\n" +
                "         (--part-enc) (--zip-out) (--no-cache) (--check-only) (--no-rev)\r\n" +
                "         (--debug) (--dll-out) (--no-hooks) (--all-includes) (--hide) \r\n" +
                "         (--no-log) (--research) (--optimize) (--no-onefile) (--wait-key)\r\n\r\n" +
                "         -merge -src [SOURCE_PATH(ZIP,FOLDER_PATH)] -start-bin [FILE_NAME(EXE)]\r\n" +
                "         (-start-src [FILE_NAME]) (-icon [ICON_PATH]) (-ver [VERSION(int)])\r\n" +
                "         (-start-argv [FIXED_ARGV(string)]) (-out [OUT_PATH])\r\n" +
                "         (--no-cache) (--wait-key) (--no-log) (--no-onefile)\r\n\r\n" +
                "         -hooks (-update [FOLDER_PATH]) \r\n" +
                "         (-add [MODULE_NAME] [ADD_COMMAND] [MODULE_DATA])\r\n" +
                "         (--upgrade) (--upgrade-clean) (--clear) (--wait-key) (--no-log)\r\n\r\n" +
                "         -help (Show pyDotexe help.)\r\n\r\n" +
                "         -version (Show pyDotexe version    : " + version + ")\r\n" +
                "                  (Supported Python version : " + py_surpport + ")\r\n" +
                "                  (Project: https://github.com/betacode-project/pyDotexe)");
        }

        static void Main_make_log(string[] args, string filename)
        {
            string argv_path = path + @"\"+ filename;
            int max_line = 30;
            string data = "";
            string save_argv = string.Join(" ", args);
            int count = 0;
            List<string> line_data = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(argv_path))
                {
                    while ((data = sr.ReadLine()) != null)
                    {
                        line_data.Add(data);
                        count++;
                        if (count == max_line)
                        {
                            line_data.RemoveAt(0);
                            count--;
                            break;
                        }
                    }
                }
                if (line_data[count - 1] != save_argv)
                    line_data.Add(save_argv);
            }
            catch
            {
                line_data.Add(save_argv);
            }


            using (StreamWriter sw = new StreamWriter(argv_path))
            {
                foreach (string save_data in line_data)
                {
                    sw.WriteLine(save_data);
                }
            }
        }
    }
}
