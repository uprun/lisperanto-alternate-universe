def dd_parse_file(file_path):
    result = {}
    last_key = ""
    file1 = open(file_path, 'r')
    for line in file1:
        just_text = line.strip()
        if just_text.startswith("--"):
            result[just_text] = []
            last_key = just_text
        else:
            result[last_key].append(just_text)
    return result