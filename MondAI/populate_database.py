def populate_database(knowledge):
    database = []
    for (entity_key, entity) in knowledge.items():
        for (connection_key, connection) in entity.items():
            for to_entity in connection:
                database.append( (entity_key, connection_key, to_entity) )
    return database