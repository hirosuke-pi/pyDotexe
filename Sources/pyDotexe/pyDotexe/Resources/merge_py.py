# -*- coding: utf-8 -*-
import sys, os, ctypes, codecs
try:
    with (codecs.open(os.path.dirname(sys.argv[0]) +'\\cache.sys', "r", "utf-8")) as f:
        sys.argv[0] = f.readline().rstrip('\n')
        ctypes.windll.kernel32.SetConsoleTitleW(sys.argv[0])
except:
    pass