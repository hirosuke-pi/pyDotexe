import sys, os
try:
    f = open(os.path.dirname(sys.argv[0]) +'\\cache.sys', "r")
    sys.argv[0] = f.readline().strip()
    os.system("title "+ sys.argv[0])
    f.close()
except:
    pass