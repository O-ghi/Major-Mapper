from cx_Freeze import setup, Executable

base = None    

executables = [Executable("csv_to_xml.py", base=base)]

packages = ["idna", "os", "csv", "pathlib", "glob"]
options = {
    'build_exe': {    
        'packages':packages,
    },    
}

setup(
    name = "csv_to_xml",
    options = options,
    version = "1.1",
    description = 'Convert csv to xml for B92',
    executables = executables
)