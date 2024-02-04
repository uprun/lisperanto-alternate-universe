from parse_ddlog import *
import pathlib

def parse_knowledge():
    files = [f for f in pathlib.Path("./knowledge/").iterdir() if f.is_file() and f.suffix == ".ddlog"]

    knowledge = {}

    for file in files:
        knowledge[file.stem] = parse_ddlog('./knowledge/' + file.name)
    return knowledge