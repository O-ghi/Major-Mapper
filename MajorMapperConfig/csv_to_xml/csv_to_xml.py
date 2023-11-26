import csv
import os
from pathlib import Path

def convert_row(title_row, row):
    rs="<Row "
    for t in (title_row):
        if t != "" and row[title_row.index(t)] != "''":
            rs += """{}="{}" """.format(t, row[title_row.index(t)].strip("'"))
    rs += "/>"
    return rs

def convert_table(title_row, rows):
    rs="<Table>\n"
    for r in rows:
        if len(r) == 0:
            continue
        else:
            if len(r[0]) >=2 and r[0][0:2] == '//':
                continue
            else:
                rs += "  " + convert_row(title_row, r) + "\n"
    rs += "</Table>"
    return rs

def convert_pro(title_row, row):
    rs="<Property "
    for t in (title_row):
        if t != "" and row[title_row.index(t)] != "''":
            rs += """{}="{}" """.format(t, row[title_row.index(t)].strip("'"))
    rs += "/>"
    return rs

def convert_object(title_row, rows):
    rs="<Object>\n"
    for r in rows:
        if len(r) == 0:
            continue
        else:
            if len(r[0]) >=2 and r[0][0:2] == '//':
                continue
            else:
                rs += "  " + convert_pro(title_row, r) + "\n"
    rs += "</Object>"
    
    return rs


def csv_to_xml(csv_file, xml_file):
    if csv_file in list_ignore_files:
        return

    with open(csv_file, 'r', encoding='utf-8-sig') as f:
        reader = csv.reader(f)
        try:
            title_row = next(reader)
        except StopIteration:
            print("Empty file")
            title_row = []
        rows = [row for row in reader]
    
    dir_path = os.path.split(xml_file)[0]
    Path(dir_path).mkdir(parents=True, exist_ok=True)

    if csv_file in list_object_files:
        with open(xml_file, 'w+', encoding='utf-8') as f:
            f.write(convert_object(title_row, rows))
    else:
        with open(xml_file, 'w+', encoding='utf-8') as f:
            f.write(convert_table(title_row, rows))

    os.system("echo convert file {} ok!".format(csv_file))

if __name__ == '__main__':
    import pathlib
    import glob

    # list_ignore_files.extend(["csv\\item_main_cfg.csv", "csv\\task\\task_main_cfg.csv"])
    for i in remove_list1:
        list_ignore_files.remove(i)
    list_object_files = []
    list_ignore_files = []
    #list_ignore_files.extend(glob.glob("csv/string_"+"*.csv"))

    if len(remove_list) > 0:
        list_object_files.remove(remove_list[0])


    list_file = []
    dept_search = 5 # dept in b92 maybe < 5 
    error_file = []
    for i in range(dept_search):
        list_file.extend(glob.glob( "csv/" + "*/"*i + "*.csv"))
    for file_path in list_file:
        xml_name = file_path.replace("csv", "xml")
        with pathlib.Path(file_path) as csv_file:
            if csv_file.is_file():
                csv_to_xml(file_path, xml_name)

    os.system("echo =====================================================")
    os.system("echo =====================================================")
    os.system("echo ===Finish===")
    os.system("echo FOLDER:./xml")
    os.system("echo Num file .csv processed: {}".format(len(list_file)-len(error_file)))
    os.system("echo Num file error: {}".format(len(error_file)))
    for file_path in error_file:
        os.system("echo Error file: {}".format(file_path))
    os.system("echo List {} ignore file:".format(len(list_ignore_files)))
    for file_path in list_ignore_files:
        os.system("echo Ignore file: {}".format(file_path))
    os.system("echo Have {} Object-Property files:".format(len(list_object_files)))
    os.system("echo Have {} Table_Row files:".format(len(list_file)-len(error_file)-len(list_ignore_files)-len(list_object_files)))

    os.system("pause")
    
    