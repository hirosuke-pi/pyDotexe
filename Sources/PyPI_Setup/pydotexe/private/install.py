import os, sys
try:
    import urllib.request
except ImportError:
    import urllib
import zipfile

def install_pydotexe(save_dir, version):
    tmp_zipfile = save_dir +"\\pydotexe_tmp.zip"

    # Check version information.
    try:
        try:
            urllib.request.urlretrieve("https://github.com/betacode-project/pyDotexe/raw/master/Sources/PyPI_Setup/version.ini", save_dir +"\\version.ini")
        except AttributeError:
            urllib.urlretrieve("https://github.com/betacode-project/pyDotexe/raw/master/Sources/PyPI_Setup/version.ini", save_dir +"\\version.ini")
        f = open(save_dir + "\\version.ini", "r")
        sp_ver = f.readline().split('.')
        now_sp_ver = version.split('.')
        f.close()

        if ((sp_ver[0] == now_sp_ver[0]) and (sp_ver[1] == now_sp_ver[1])):
            print("[+] pydotexe version detected: "+ ".".join(sp_ver))
        else:
            print("[-] pydotexe version mismatch: "+ ".".join(sp_ver))
            print("    Please upgrade this setup_pydotexe module with pip.")
            return False
            
    except:
        return False

    # Start download pydotexe's ZIP data.
    try:
        try:
            urllib.request.urlretrieve("https://github.com/betacode-project/pyDotexe/raw/master/Binary/Latest/pyDotexe.zip", tmp_zipfile)
        except AttributeError:
            urllib.urlretrieve("https://github.com/betacode-project/pyDotexe/raw/master/Binary/Latest/pyDotexe.zip", tmp_zipfile)
        zf = zipfile.ZipFile(tmp_zipfile, "r")
        zf.extractall(path=save_dir)
        zf.close()
        try:
            os.remove(tmp_zipfile)
        except:
            pass
        print("[+] pyDotexe installation complteted.\r\n")
        return True
    except:
        return False


