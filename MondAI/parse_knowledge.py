from dd_parse_file import *
import pathlib

def parse_knowledge():
    files = [f for f in pathlib.Path("./knowledge/").iterdir() if f.is_file() and f.suffix == ".dd"]

    knowledge = {}

    for file in files:
        knowledge[file.stem] = dd_parse_file('./knowledge/' + file.name)
    return knowledge