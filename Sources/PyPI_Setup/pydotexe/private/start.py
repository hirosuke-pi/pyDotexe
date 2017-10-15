import os, sys

def pydotexe_cmd(cmd, upgrade=False):
    if (upgrade): 
        os.system("pydotexe.exe -hooks --upgrade-clean")
    os.system("pydotexe "+ cmd)


