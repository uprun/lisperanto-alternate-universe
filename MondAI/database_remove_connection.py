import datetime

def database_remove_connection(entity, connection, with_entity, database, author):
    database = [c for c in database if c[0] != entity or c[1] != connection or c[2] != with_entity]
    # log
    with open("./knowledge/" + entity + ".ddlog",'a+') as f:
        current_time = datetime.datetime.now()
        str_date_time = current_time.strftime("%Y-%m-%d--%H:%M:%S")
        to_log = '- ' + entity + ' ' + connection + ' ' + with_entity + ' ' + str_date_time + ' ' + author + '\n'
        f.write(to_log)

    return database