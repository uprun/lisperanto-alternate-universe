def query(entity, connection, to_entity, database):
    for triple in database:
        if entity != '?' and triple[0] != entity:
            continue
        if connection != '?' and triple[1] != connection:
            continue
        if to_entity != '?' and triple[2] != to_entity:
            continue
        print(triple)