def dd_save_to_file(dictionary_of_lists, file_path):
    file1 = open(file_path, 'w')
    for e in dictionary_of_lists.items():
        key = e[0] + '\n'
        if key.startswith('--') == False:
            key = '--' + key
        #print('Processing key', key)
        file1.write(key)
        for line in e[1]:
            file1.write(line + '\n')
    file1.close()