from print_query import *
from add_connection import *
from replace_connection import *
from remove_connection import *

def try_to_analyze(words, knowledge, database):
    if len(words) == 1:
        print("triple query", words)
        print_query(words[0], '?', '?', database)
        print_query('?', words[0], '?', database)
        print_query('?', '?', words[0], database)
        return database
    if len(words) == 3:
        print("query", words)
        print_query(words[0], words[1], words[2], database)
        return database
    if len(words) == 4 and words[0] == "+":
        print("adding connection", words[1:])
        add_connection(words[1], words[2], words[3], knowledge, database, 'user')
        return database
    if len(words) == 4 and words[0] == "=":
        print("replacing connection", words[1:])
        database = replace_connection(words[1], words[2], words[3], knowledge, database, 'user')
        return database
    if len(words) == 4 and words[0] == "-":
        print("removing connection", words[1:])
        database = remove_connection(words[1], words[2], words[3], knowledge, database, 'user')
        return database
    print("unknown command")