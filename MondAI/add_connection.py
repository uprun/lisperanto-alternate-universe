from add_entity import *
from dd_save_to_file import *
import datetime;
  


def add_connection(entity, connection, with_entity, knowledge, database, author):
    add_entity(entity, knowledge)
    entity_dd = knowledge[entity]
    
    if not connection in entity_dd:
        entity_dd[connection] = []

    if not with_entity in entity_dd[connection]:
        print("this is a new knowledge, thanks")
        entity_dd[connection].append(with_entity)
        dd_save_to_file(entity_dd, "./knowledge/" + entity + ".dd")
        database.append((entity, connection, with_entity))
        # log
        with open("./knowledge/" + entity + ".ddlog",'a+') as f:
            current_time = datetime.datetime.now()
            str_date_time = current_time.strftime("%Y-%m-%d--%H:%M:%S")
            to_log = '+ ' + entity + ' ' + connection + ' ' + with_entity + ' ' + str_date_time + ' ' + author + '\n'
            f.write(to_log)
    else:
        print("such knowledge already exists")