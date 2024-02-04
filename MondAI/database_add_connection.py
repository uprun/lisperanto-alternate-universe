import datetime

def database_add_connection(entity, connection, with_entity, database, author):
    database.append((entity, connection, with_entity))
    # log
    with open("./knowledge/" + entity + ".ddlog",'a+') as f:
        current_time = datetime.datetime.now()
        str_date_time = current_time.strftime("%Y-%m-%d--%H:%M:%S")
        to_log = '+ ' + entity + ' ' + connection + ' ' + with_entity + ' ' + str_date_time + ' ' + author + '\n'
        f.write(to_log)