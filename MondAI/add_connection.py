from add_entity import *
from dd_save_to_file import *
from database_add_connection import *
  


def add_connection(entity, connection, with_entity, knowledge, database, author):
    add_entity(entity, knowledge)
    entity_dd = knowledge[entity]
    
    if not connection in entity_dd:
        entity_dd[connection] = []

    if not with_entity in entity_dd[connection]:
        print("this is a new knowledge, thanks")
        entity_dd[connection].append(with_entity)
        dd_save_to_file(entity_dd, "./knowledge/" + entity + ".dd")
        database_add_connection(entity, connection, with_entity, database, author)
    else:
        print("such knowledge already exists")