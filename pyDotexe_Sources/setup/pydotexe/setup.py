import os, sys

__version__ = "1.0.0"

class build(object):
    ''' Build Python program setup-class. '''

    def __init__(self, source_path):
        # Set default data
        self.__src = source_path
        self.out = ""
        self.argv = ""
        self.icon = ""
        self.module_loader = ""

        # Set file or folder lists
        self.import_list = []
        self.pymodule = []

        self.file_list = []
        self.dir_list = []
        self.pyfile = []
        self.pydir = []
        self.pydirfile = []
        self.pydirdir = []
        self.pyfile_regex = []
        self.pydir_regex = []
        self.exclude = []
        self.resfile = []
        self.resdir = []

        # Set number or bool
        self.compress_levels = 9
        self.upgrade_hooks_clean = False
        self.hide_console = False
        self.all_encodes = True
        self.zip_out = False
        self.dll_out = False 
        self.take_logs = True     
        self.use_cache = True
        self.use_hooks = True
        self.add_path_revision = True
        self.wait_key_input = False
        self.debug_mode = False
        self.check_modules_only = False
        self.research_modules = False
        self.optimize = False
        self.one_file = True
        self.all_includes = False

    def get_argv_data(self):
        ''' Get pydotexe's argv string data '''
        argv = "-build -src "+ self.__src
        argv = argv +" -comp="+ str(self.compress_levels)
        if (self.out != ""): argv = argv + " -out "+ self.out
        if (self.icon != ""): argv = argv + " -icon "+ self.icon
        if (self.argv != ""): argv = argv + " -argv "+ self.argv
        if ((self.module_loader != "" ) and (os.path.isfile(self.module_loader))): argv = argv + " -loader "+ self.module_loader

        if (len(self.import_list) != 0): argv = argv + " -import "+ ",".join(self.import_list)
        if (len(self.pymodule) != 0): argv = argv + " -pymodule "+ ",".join(self.pymodule)
        if (len(self.file_list) != 0): argv = argv + " -file "+ ",".join(self.file_list)
        if (len(self.dir_list) != 0): argv = argv + " -dir "+ ",".join(self.dir_list)
        if (len(self.pyfile) != 0): argv = argv + " -pyfile "+ ",".join(self.pyfile)
        if (len(self.pydir) != 0): argv = argv + " -pydir "+ ",".join(self.pydir)
        if (len(self.pydirfile) != 0): argv = argv + " -pydirfile "+ ",".join(self.pydirfile)
        if (len(self.pydirdir) != 0): argv = argv + " -pydirdir "+ ",".join(self.pydirdir)
        if (len(self.pyfile_regex) != 0): argv = argv + " -pyfile-regex "+ " -pyfile-regex ".join(self.pyfile_regex)
        if (len(self.pydir_regex) != 0): argv = argv + " -pydir-regex "+ " -pydir-regex ".join(self.pydir_regex)
        if (len(self.pydir_regex) != 0): argv = argv + " -exclude "+ " -exclude ".join(self.pydir_regex)
        if (len(self.resfile) != 0): argv = argv + " -resfile "+ ",".join(self.resfile)
        if (len(self.resdir) != 0): argv = argv + " -resdir "+ ",".join(self.resdir)

        if (self.hide_console): argv = argv + " --hide"
        if (not(self.all_encodes)): argv = argv + " --part-enc"
        if (self.zip_out): argv = argv + " --zip-out"
        if (self.dll_out): argv = argv + " --dll-out"
        if (not(self.take_logs)): argv = argv + " --no-log"
        if (not(self.use_cache)): argv = argv + " --no-cache"
        if (not(self.use_hooks)): argv = argv + " --no-hooks"
        if (not(self.add_path_revision)): argv = argv + " --no-rev"
        if (self.wait_key_input): argv = argv + " --wait-key"
        if (self.debug_mode): argv = argv + " --debug"
        if (self.check_modules_only): argv = argv + " --check-only"
        if (self.research_modules): argv = argv + " --research"
        if (self.optimize): argv = argv + " --optimize"
        if (not(self.one_file)): argv = argv + " --no-onefile"
        if (self.all_includes): argv = argv + " --all-includes"

        return argv

    def start_build(self, current_directory=""):
        ''' Start program '''

        if (current_directory == ""):
            os.chdir(os.path.dirname(sys.argv[0]))
        else:
            os.chdir(os.path.dirname(current_directory))           
        print("[+] Set current directory: "+ os.getcwd())
        
        is_found = False
        for path in sys.path:
            if (os.path.isfile(path +"\\Scripts\\pydotexe.exe")):
                is_found = True
                break

        if (is_found):
            print("[+] Starting pyDotexe process...")
            if (self.upgrade_hooks_clean):
                os.system("pydotexe.exe -hooks --upgrade-clean")
            os.system("pydotexe.exe "+ self.get_argv_data())
        else:
            print("[-] 'pydotexe.exe' is not found in Scripts directory. Please install pyDotexe.")
    