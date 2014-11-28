@ECHO OFF
cls

set bin="..\binr"

xcopy *.* %bin%\ /s /y /i /EXCLUDE:exclude.lst

ping -n 2 127.1 >nul 2>nul

:END
