from dd_save_to_file import *

def add_entity(word, knowledge):
    if not word in knowledge:
        print("[",word, "] is missing, adding entry")
        knowledge[word] = {}
        dd_save_to_file(knowledge[word], "./knowledge/" + word + ".dd")