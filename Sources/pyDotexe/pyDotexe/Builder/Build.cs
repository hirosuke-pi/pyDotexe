using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace pyDotexe.Builder
{
    class Build
    {
        private static BuildSet bset = null;

        private class CreateTmpError : Exception
        {
            public CreateTmpError(string Message) { }
        }
        private class ImportDllCompilerError : Exception
        {
            public ImportDllCompilerError(string Message) { }
        }
        private class ReplaceCompileFileError : Exception
        {
            public ReplaceCompileFileError(string Message) { }
        }
        private class CompressModulesError : Exception
        {
            public CompressModulesError(string Message) { }
        }
        private class MargeApplicationError : Exception
        {
            public MargeApplicationError(string Message) { }
        }
        private class AllModulesCopyError : Exception
        {
            public AllModulesCopyError(string Message) { }
        }

        /// <summary>
        /// Start build and marge
        /// </summary>
        /// <param name="BuildSettings">BuildSet Object</param>
        /// <returns></returns>
        public static bool Start(ref BuildSet BuildSettings)
        {
            try
            {
                bset = BuildSettings;
                if (bset.standalone)
                {
                    if (bset.all_imports)
                    {
                        Console.WriteLine("\r\n[+] Copying all modules file...");
                        copy_all_modules(); // Import all modules.
                    }
                    else
                    {
                        Console.WriteLine("\r\n[+] Copying Python libraries and compiler...");
                        import_dll_compiler(); // Copy python libraries to TMP folder.         
                    }
                }

                Console.WriteLine("\r\n[+] Copying imported modules...");
                copy_module_file(); // Copy selected files to TMP folder.

                if ((bset.resource_folder.Count != 0) | (bset.resource_file.Count != 0))
                {
                    Console.WriteLine("[+] Copying selected resources data...");
                    copy_resources(); // Copy delected resource files to TMP folder.
                    Console.WriteLine();
                }

                if (bset.debug) // Debug mode only.
                {
                    Console.WriteLine("\r\n");
                    Analysis.show_module_list();
                    Console.WriteLine("\r\n[+] Debug path and command-options extracted. You can start in Command-Lines.");
                    string startCMD = "";
                    string startApp = "";

                    if (bset.standalone)
                    {
                        startApp = "\"" + bset.tmp_module_path + @"\" + Path.GetFileName(bset.default_python_bin) + "\"";
                        startCMD = "\"" + bset.tmp_module_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + bset.default_src_ex + "\"";
                    }
                    else
                    {
                        startApp = "python";
                        startCMD = "\"" + bset.tmp_module_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + bset.default_src_ex + "\"";
                    }

                    Console.WriteLine("[*] "+ startApp +" "+ startCMD);                   
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(bset.tmp_folder_path + @"\pydotexe_debug.bat"))
                        {
                            sw.WriteLine(startApp +" "+ startCMD);
                            sw.WriteLine("pause");
                        }
                        Process.Start(bset.tmp_folder_path + @"\pydotexe_debug.bat");
                    } catch { }
                    return false;
                }

                if (!bset.zip_out & bset.add_fixed) add_fixes_argv_path(); // Add file path fixed.

                if (bset.optimize)
                {
                    Console.WriteLine("[+] Optimizing Python modules...");
                    optimize_code_start();
                }

                if (bset.standalone)
                {
                    Console.WriteLine("[+] Compiling imported modules...");
                    replace_compile_file(); // Replace default source codes file to compiled file.
                }

                if (bset.one_file)
                {
                    Console.WriteLine("[+] Creating image file...");
                    compress_modules(); // Compress TMP folder.
                }
                else
                {
                    Console.WriteLine("[+] Copying to out-folder path...");
                    string dir_path = Path.GetDirectoryName(bset.output_path);
                    if (!Directory.Exists(dir_path)) new DirectoryInfo(dir_path).Create();
                    copy_folder(bset.tmp_module_path, dir_path);
                }

                if (bset.zip_out) // Zip-out mode only
                {
                    try
                    {
                        File.Copy(bset.zip_path, bset.source_folder_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + ".zip", true);
                    }
                    catch (Exception ex) { throw new CompressModulesError(ex.Message); }

                    Console.WriteLine("\a\r\n[+] File compression completed.");
                    Console.WriteLine("[*] " + bset.source_folder_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + ".zip");
                    try
                    {
                        Directory.Delete(bset.tmp_folder_path, true);
                    }
                    catch { }
                    return false;
                }

                marge_application(); // Merge application binary.
                return true;

            }
            catch (CreateTmpError)
            {
                Console.WriteLine("[-] Could not make temporary folder.");
            }
            catch (ImportDllCompilerError)
            {
                Console.WriteLine("\r\n[-] Could not copy the file: python.exe, pythonw.exe, python" + bset.py_version + ".dll or Source code.");
            }
            catch (ReplaceCompileFileError)
            {
                Console.WriteLine("[-] Could not compile python sources.");
            }
            catch (CompressModulesError)
            {
                Console.WriteLine("[-] Could not create image file.");
            }
            catch (MargeApplicationError)
            {
            }
            return false;
        }

        /// <summary>
        /// Copy default Python libraries
        /// </summary>
        private static void import_dll_compiler()
        {           
            try
            {
                Directory.CreateDirectory(bset.tmp_module_path);
            }
            catch (Exception ex)
            {
                throw new CreateTmpError(ex.Message);
            }

            try
            {
                foreach (string default_modules in bset.default_import)
                {
                    Console.WriteLine(" + "+ default_modules);
                    File.Copy(default_modules, bset.tmp_module_path + @"\" + Path.GetFileName(default_modules), true);
                }
            }
            catch (Exception ex)
            {
                throw new ImportDllCompilerError(ex.Message);
            }
        }

        /// <summary>
        /// Copy all modules
        /// </summary>
        private static void copy_all_modules()
        {
            try
            {
                copy_folder(bset.python_path, bset.tmp_module_path);
                File.Copy(bset.source_path, bset.tmp_module_path + @"\" + Path.GetFileName(bset.source_path), true);
            }
            catch (Exception ex)
            {
                throw new AllModulesCopyError(ex.Message);
            }
        }

        /// <summary>
        /// Copy selected files
        /// </summary>
        private static void copy_module_file()
        {
            foreach (string module_path in bset.module_path)
            {
                if (module_path.ToLower().StartsWith(bset.python_lib_path.ToLower()))
                    // Python's base library files only.
                    copy_folder_ex(bset.python_lib_path, module_path, bset.tmp_module_path, @"\lib");
                else if (module_path.ToLower().StartsWith(bset.python_path.ToLower()))
                    // Python's base library folders only. (ex: \site-packages)
                    copy_folder_ex(bset.python_path, module_path, bset.tmp_module_path);
                else if (module_path.StartsWith(bset.source_folder_path))
                    // Current directory's module files only.
                    copy_folder_ex(bset.source_folder_path, module_path, bset.tmp_module_path);
                else
                    // pkg_resources and Eggs module files only.
                    copy_folder_ex("", module_path, bset.tmp_module_path, "", true);
            }

            // python binary icon change
            try
            {
                if (bset.icon_path != "")
                {
                    IconChanger icon_change = new IconChanger();
                    var r = icon_change.ChangeIcon(bset.tmp_module_path + @"\" + Path.GetFileName(bset.default_python_bin), bset.icon_path);
                    r.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Could not change the python binary icon\r\n[!] " + ex.Message + "\r\n");
            }
        }

        /// <summary>
        /// Copy files to TMP folder 
        /// </summary>
        /// <param name="base_path">Base path</param>
        /// <param name="trim_path">Trim data</param>
        /// <param name="copy_to_path">Copy to</param>
        /// <param name="add_folder">Want to add folder path</param>
        private static void copy_folder_ex(string base_path, string trim_path, string copy_to_path, string add_folder = @"", bool pkg_res = false)
        {
            try
            {
                if (pkg_res)
                {
                    foreach (string module in bset.module_hooks_list)
                    {
                        string base_m = module.Replace(".", @"\");
                        if (trim_path.EndsWith(base_m + ".py"))
                        {
                            base_path = copy_to_path + "\\" + base_m + ".py";
                            new DirectoryInfo(Path.GetDirectoryName(base_path)).Create(); // Create selected folder path.
                            File.Copy(trim_path, base_path, true);
                        }
                    }
                }
                else
                {
                    string create_path = copy_to_path + add_folder + trim_path.Remove(trim_path.LastIndexOf(@"\")).Remove(0, base_path.Length);
                    new DirectoryInfo(create_path).Create(); // Create selected folder path.
                    File.Copy(trim_path, create_path + @"\" + Path.GetFileName(trim_path), true);

                }
            }
            catch (Exception ex)
            {
                bset.copy_error.Add(trim_path + "\r\n   " + ex.Message);
            }
        }

        /// <summary>
        /// Copy folder
        /// </summary>
        /// <param name="sourceDirName">Source folder path</param>
        /// <param name="destDirName">Want copying file path</param>
        private static void copy_folder(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }

            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                destDirName = destDirName + Path.DirectorySeparatorChar;

            foreach (string file in Directory.EnumerateFiles(sourceDirName, "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                }
                catch (Exception ex)
                {
                    bset.copy_error.Add(file + "\r\n   " + ex.Message);
                }
            }
            
            foreach (string dir in Directory.EnumerateDirectories(sourceDirName))
                copy_folder(dir, destDirName + Path.GetFileName(dir));
        }

        /// <summary>
        /// Add file path fixed
        /// </summary>
        private static void add_fixes_argv_path()
        {
            try
            {
                string src_data = "";
                using (FileStream fs = new FileStream(bset.tmp_module_path + @"\" + Path.GetFileName(bset.source_path), FileMode.Open, FileAccess.Read))
                {
                    StreamReader sr = new StreamReader(fs);
                    src_data = sr.ReadToEnd();
                    sr.Close();
                }
                using (FileStream fs = new FileStream(bset.tmp_module_path + @"\" + Path.GetFileName(bset.source_path), FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    if (int.Parse(bset.py_version) >= 26)
                        // Python 2.6.x ~ Python 2.7.x, Python 3.x.x ~
                        sw.WriteLine(Encoding.ASCII.GetString(Properties.Resource.merge_py));
                    else // ~ Python 2.5
                        sw.WriteLine(Encoding.ASCII.GetString(Properties.Resource.merge_py_old)); // Use old mode
                    sw.WriteLine(src_data);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Could not set start-path: "+ ex.Message +"\r\n");
            }
        }

        private static void optimize_code_start()
        {
            List<Task> tmp_taskList = new List<Task>();
            foreach (string tmp_path in Directory.EnumerateFiles(bset.tmp_folder_path, "*.py", SearchOption.AllDirectories))
            {
                tmp_taskList.Add(Task.Factory.StartNew(() =>
                {
                   Codes.Optimize.Start(tmp_path);
                }));
            }
            Task.WaitAll(tmp_taskList.ToArray());
        }

        /// <summary>
        /// Start source codes compile
        /// </summary>
        /// <param name="argv"></param>
        private static void compile_python_module(string argv)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = bset.python_binary_path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = argv;

            // Start Python binary.
            p.Start(); 

            string results = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
        }

        /// <summary>
        /// Replace source files to complied files
        /// </summary>
        private static void replace_compile_file()
        {
            try
            {
                if (int.Parse(bset.py_version) >= 35) // Python 3.5 ~
                {
                    compile_python_module("-c \"import compileall; compileall.compile_dir('" + bset.tmp_module_path.Replace(@"\", @"\\") + "', maxlevels=100, optimize=2)\"");
                    start_replace_py3();
                }
                else if (int.Parse(bset.py_version) >= 32) // Python 3.2 ~ Python 3.4
                {
                    compile_python_module("-c \"import compileall; compileall.compile_dir('" + bset.tmp_module_path.Replace(@"\", @"\\") + "', maxlevels=100)\"");
                    start_replace_py3();
                }
                else // Python 2.x, Python 3.0, 3.1
                {
                    compile_python_module("-c \"import compileall; compileall.compile_dir('" + bset.tmp_module_path.Replace(@"\", @"\\") + "', maxlevels=100)\"");
                    start_replace_py2();
                }
            }
            catch (Exception ex)
            {
                throw new ReplaceCompileFileError(ex.Message);
            }
        }

        /// <summary>
        /// Replace Python 2.x mode
        /// </summary>
        private static void start_replace_py2()
        {
            foreach (string folder_path in Directory.EnumerateDirectories(bset.tmp_folder_path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    foreach (string compiled_file in Directory.EnumerateFiles(folder_path, "*.pyc", SearchOption.AllDirectories))
                    {
                        bset.default_src_ex = Path.GetExtension(compiled_file);
                        string delete_file = Path.GetDirectoryName(compiled_file) + @"\" + Path.GetFileNameWithoutExtension(compiled_file) + ".py";
                        File.Delete(delete_file);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Replace Python 3.x mode
        /// </summary>
        private static void start_replace_py3()
        {
            foreach (string folder_path in Directory.EnumerateDirectories(bset.tmp_module_path, "*", SearchOption.AllDirectories))
            {
                if (folder_path.EndsWith("__pycache__")) // find cache folder path.
                {
                    try
                    {
                        foreach (string compiled_file in Directory.EnumerateFiles(folder_path, "*", SearchOption.AllDirectories))
                        {
                            bset.default_src_ex = Path.GetExtension(compiled_file);
                            string copyto = folder_path.Replace("__pycache__", "") + Path.GetFileName(compiled_file).Split('.')[0] + Path.GetExtension(compiled_file);
                            File.Copy(compiled_file, copyto, true);
                            File.Delete(Path.GetDirectoryName(copyto) + @"\" + Path.GetFileNameWithoutExtension(copyto) + ".py");
                        }
                        Directory.Delete(folder_path, true);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Copy resource files
        /// </summary>
        private static void copy_resources()
        {
            foreach (string resource_name in bset.resource_file)
            {
                try
                {
                    File.Copy(bset.source_folder_path + @"\" + resource_name, bset.tmp_module_path + @"\" + resource_name);
                    Console.WriteLine(" + " + bset.source_folder_path + @"\" + resource_name);
                }
                catch (Exception ex)
                {
                    bset.copy_error.Add(bset.source_folder_path + @"\" + resource_name + "\r\n   " + ex.Message);
                }
            }
            foreach (string resource_folder in bset.resource_folder)
            {
                try
                {
                    copy_folder(bset.source_folder_path + @"\" + resource_folder, bset.tmp_module_path + @"\" + resource_folder);
                    Console.WriteLine(" + " + bset.source_folder_path + @"\" + resource_folder);
                }
                catch (Exception ex)
                {
                    bset.copy_error.Add(bset.source_folder_path + @"\" + resource_folder + "\r\n   " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Complress modules data
        /// </summary>
        private static void compress_modules()
        {            
            if ((bset.compress != 1) & (bset.compress != 0)) bset.compress = 2;      

            bset.zip_path = bset.tmp_folder_path + @"\" + Path.GetFileNameWithoutExtension(bset.source_path) + ".zip";
            try
            {
                if (File.Exists(bset.zip_path)) File.Delete(bset.zip_path);
                compress_zip();
                GC.Collect();
                //ZipFile.CreateFromDirectory(bset.tmp_module_path, bset.zip_path, (CompressionLevel)bset.compress, true);
            }
            catch (Exception ex)
            {               
                throw new CompressModulesError(ex.Message);
            }
        }

        private static void compress_zip()
        {
            string zipPath = bset.zip_path;
            string zipFolder = bset.tmp_folder_path;

            //Write ZIP Stream.
            FileStream writer = new FileStream(zipPath, FileMode.Create, FileAccess.Write);

            //Build ZipOutputStream.
            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zos = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(writer);

            //Set compress levels.
            if ((0 <= bset.compress) | (9 >= bset.compress))
                zos.SetLevel(bset.compress);
            else
                zos.SetLevel(9);

            //Get folders.
            ICSharpCode.SharpZipLib.Zip.ZipNameTransform nameTrans =
                new ICSharpCode.SharpZipLib.Zip.ZipNameTransform(zipFolder);

            foreach (string file in Directory.EnumerateFiles(zipFolder, "*", System.IO.SearchOption.AllDirectories))
            {
                if (file == bset.zip_path) continue;

                // Set file name.
                string f = nameTrans.TransformFile(file);
                ICSharpCode.SharpZipLib.Zip.ZipEntry ze =
                    new ICSharpCode.SharpZipLib.Zip.ZipEntry(f);

                // Set file informations.
                FileInfo fi = new System.IO.FileInfo(file);
                ze.DateTime = fi.LastAccessTime;
                ze.ExternalFileAttributes = (int)fi.Attributes;
                ze.Size = fi.Length;
                ze.IsUnicodeText = true;
                zos.PutNextEntry(ze);

                // Load files.
                try
                {
                    FileStream fs = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[2048];
                    int len;
                    while ((len = fs.Read(buffer, 0, buffer.Length)) > 0)
                        zos.Write(buffer, 0, len);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" - Error: "+ file +" ["+ ex.Message+"]");
                    continue;
                }
            }
            // Close objects.
            zos.Finish();
            zos.Close();
            writer.Close();
        }

        /// <summary>
        /// Merge application binary
        /// </summary>
        private static void marge_application()
        {
            
            if (!Merger.Merge.Start(bset)) throw new MargeApplicationError(""); // Start Merge

            // Removing Tmp files
            if (bset.dll_out)
            {
                string dll_copyto = Path.GetDirectoryName(bset.output_path) + @"\" + Path.GetFileNameWithoutExtension(bset.output_path) + ".dll";
                if (!File.Exists(dll_copyto))
                    File.Copy(bset.python_path + @"\python" + bset.py_version + ".dll", dll_copyto, true);
            }
            try
            {
                Directory.Delete(bset.tmp_folder_path, true);
            }
            catch { }
        }
    }
}
