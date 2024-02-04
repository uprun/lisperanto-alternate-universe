
from add_entity import *
from dd_save_to_file import *
from database_remove_connection import *
  


def remove_connection(entity, connection, with_entity, knowledge, database, author):
    add_entity(entity, knowledge)
    entity_dd = knowledge[entity]
    
    if not connection in entity_dd:
        entity_dd[connection] = []

    if with_entity in entity_dd[connection]:
        list = entity_dd[connection]
        list = [e for e in list if e != with_entity]
        entity_dd[connection] = list
        print("removing knowledge, absence of knowledge is also knowledge, thanks")
        dd_save_to_file(entity_dd, "./knowledge/" + entity + ".dd")
        database = database_remove_connection(entity, connection, with_entity, database, author)
    else:
        print("there is no such knowledge")
    return database