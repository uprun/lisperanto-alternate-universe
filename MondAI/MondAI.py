import re

from dd_save_to_file import *
from parse_knowledge import *
from split_words import *
from add_entity import *
from add_connection import *

def populate_database(knowledge):
    database = []
    for (entity_key, entity) in knowledge.items():
        for (connection_key, connection) in entity.items():
            for to_entity in connection:
                database.append( (entity_key, connection_key, to_entity) )
    return database

def query(entity, connection, to_entity, database):
    for triple in database:
        if entity != '?' and triple[0] != entity:
            continue
        if connection != '?' and triple[1] != connection:
            continue
        if to_entity != '?' and triple[2] != to_entity:
            continue
        print(triple)


def try_to_analyze(words):
    if len(words) == 3:
        print("query", words)
        query(words[0], words[1], words[2], _database)
        return
    if len(words) == 4 and words[0] == "+":
        print("adding connection", words[1:])
        add_connection(words[1], words[2], words[3], _knowledge)
        return
    print("unknown command")


_knowledge = parse_knowledge()
print(_knowledge['MondAI']['name'], 'welcomes you')

_database = populate_database(_knowledge)

while True:
    print("you:")
    input_string = input()
    words = split_words(input_string)
    print(words)
    try_to_analyze(words)
