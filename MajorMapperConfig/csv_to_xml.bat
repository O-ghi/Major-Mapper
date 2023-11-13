echo off

echo "Started convert csv to xml!"
start /MIN csv_to_xml\build\exe.win-amd64-3.9\csv_to_xml.exe
echo "Refresh and check folder named 'xml' "

:exitpoint
pause
