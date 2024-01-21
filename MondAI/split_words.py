import re

def split_words(input_string):
    words = re.split('\s+', input_string)
    result = []
    symbols = ["?", ",", "!"]
    for word in words:
        if any(word.endswith(symb) for symb in symbols) and len(word) > 1:
            result.append(word[:-1])
            result.append(word[-1])
        else:
            result.append(word)
    return result