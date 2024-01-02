def dd_parse_file(file_path):
    result = {}
    last_key = ""
    file1 = open(file_path, 'r')
    for line in file1:
        just_text = line
        if just_text.startswith("--"):
            last_key = just_text[2:].strip()
            result[last_key] = []
        else:
            result[last_key].append(just_text.strip())
    file1.close()
    return result