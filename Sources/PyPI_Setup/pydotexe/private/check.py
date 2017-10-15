import sys, os

def set_current_dir(current_directory = ""):
        # Set current directory.
        if (current_directory == ""):
            os.chdir(os.path.dirname(sys.argv[0]))
        else:
            os.chdir(os.path.dirname(current_directory))  

def check_pydotexe_file():
    is_found = False
    scripts_dir = ""
    for path in sys.path:
        if (os.path.isfile(path + "\\Scripts\\pip.exe")):
            scripts_dir = path + "\\Scripts"
        if (os.path.isfile(path +"\\Scripts\\pydotexe.exe")):
            is_found = True
            break
    return is_found, scripts_dir