from query import *

def print_query(entity, connection, to_entity, database):
    result = query(entity, connection, to_entity, database)
    for entry in result:
        print(entry)