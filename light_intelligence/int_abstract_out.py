import random
def abstract_out_remove_random_word(input_words):
    words = input_words[:] #shallow copy of the list
    print("input words", words)
    length = len(words)
    index_to_remove = random.randrange(0, length)
    print("removing index:", index_to_remove)
    removed = words.pop(index_to_remove)
    print("after removal:", words)
    return words

def test_run():
    for i in range(1,30):
        input = ['hello', 'words']
        abstract_out_remove_random_word(input)
        print("afer method call", input)