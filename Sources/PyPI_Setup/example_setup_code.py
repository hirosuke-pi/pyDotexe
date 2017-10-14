import pydotexe.setup

# Some Settings
build_set = pydotexe.setup.build("yourcode.py")
build_set.icon = ""
build_set.hide_console = False
build_set.use_cache = True

# Show pyDotexe argv data.
# print(build_set.get_argv_data())

# Start pyDotexe process.
build_set.start_build()
