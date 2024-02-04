from add_entity import *
from dd_save_to_file import *
from database_replace_connection import *
  


def replace_connection(entity, connection, with_entity, knowledge, database, author):
    add_entity(entity, knowledge)
    entity_dd = knowledge[entity]
    
    if not connection in entity_dd:
        entity_dd[connection] = []
    if len(entity_dd[connection]) == 1 and entity_dd[connection][0] == with_entity:
        print("this knowledge is already present")
        return

    print("this is a new knowledge, thanks")
    entity_dd[connection]=[with_entity]
    dd_save_to_file(entity_dd, "./knowledge/" + entity + ".dd")
    database = database_replace_connection(entity, connection, with_entity, database, author)
    return database