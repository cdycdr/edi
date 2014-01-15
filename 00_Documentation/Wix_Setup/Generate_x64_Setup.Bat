@ECHO OFF

REM 
REM Setup of Wix Installer version 3.8.1128 or later is required for this setup scripts to work
REM 
ECHO.
ECHO Generating MSI file please wait...
ECHO.
ECHO.

SET WIX=C:\Program Files (x86)\WiX Toolset v3.8\bin\

REM "%WIX%heat.exe" dir Release -gg -cg EdiCore -out 10_SetupScripts\Edi.wxs

"%WIX%candle.exe" 10_SetupScripts\Edi.wxs 10_SetupScripts\EdiProject.wxs -out .\30_temp_x64\

"%WIX%light.exe" .\30_temp_x64\Edi.wixobj .\30_temp_x64\EdiProject.wixobj -out Edi_x64.msi

del Edi_x64.wixpdb

PAUSE
