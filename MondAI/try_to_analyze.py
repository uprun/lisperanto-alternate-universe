from query import *
from add_connection import *

def try_to_analyze(words, knowledge, database):
    if len(words) == 3:
        print("query", words)
        query(words[0], words[1], words[2], database)
        return
    if len(words) == 4 and words[0] == "+":
        print("adding connection", words[1:])
        add_connection(words[1], words[2], words[3], knowledge, database)
        return
    print("unknown command")