import re

def parse_ddlog(file_path):
    result = {}
    last_key = ""
    file1 = open(file_path, 'r')
    for line in file1:
        words = re.split('\s+', line)
        command = words[0]
        connection = words[2]
        to_entity = words[3]
        if command == '+':
            if not connection in result:
                result[connection] = []
            result[connection].append(to_entity)

        if command == '=':
            if not connection in result:
                result[connection] = []
            result[connection] = [to_entity]

        if command == '-':
            if not connection in result:
                result[connection] = []
            if to_entity in result[connection]:
                result[connection] = [e for e in result[connection] if e != to_entity]
    file1.close()
    return result