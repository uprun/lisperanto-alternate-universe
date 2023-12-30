import re
from int_abstract_out import *
print('light_intelligence')

while True:
     print("you:")
     input_string = input()
     print(">>>", input_string)
     words = re.split('\s+', input_string)
     abstract_version = abstract_out_remove_random_word(words)
     print(abstract_version)
