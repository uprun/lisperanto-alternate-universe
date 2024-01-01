import re
from int_abstract_out import *
from dd_parse_file import *
print('MondAI welcomes you')
parsed = dd_parse_file("./self_knowledge/self.dd")
print (parsed)

def split_words(input_string):
    words = re.split('\s+', input_string)
    result = []
    symbols = ["?", ",", "!"]
    for word in words:
        if any(word.endswith(symb) for symb in symbols):
            result.append(word[:-1])
            result.append(word[-1])
        else:
            result.append(word)
    return result

while True:
    print("you:")
    input_string = input()
    print(">>>", input_string)
    words = split_words(input_string)
    abstract_version = abstract_out_remove_random_word(words)
    print(abstract_version)
