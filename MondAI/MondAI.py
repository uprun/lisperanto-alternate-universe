from parse_knowledge import *
from split_words import *
from populate_database import *
from try_to_analyze import *

_knowledge = parse_knowledge()
print(_knowledge['MondAI']['name'], 'welcomes you')

_database = populate_database(_knowledge)

while True:
    print("you:")
    input_string = input()
    words = split_words(input_string)
    print(words)
    try_to_analyze(words, _knowledge, _database)
