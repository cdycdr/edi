@ECHO OFF

REM 
REM Setup of Wix Installer version 3.8.1128 or later is required for this setup scripts to work
REM 

ECHO.
ECHO Generating MSI file please wait...
ECHO.
ECHO.

SET WIX=C:\Program Files (x86)\WiX Toolset v3.8\bin\

REM "%WIX%heat.exe" dir Release -gg -cg EdiCore -out 20_SetupScripts_x86\Edi.wxs

"%WIX%candle.exe" 20_SetupScripts_x86\Edi.wxs 20_SetupScripts_x86\EdiProject.wxs -out .\40_temp_x86\

"%WIX%light.exe" .\40_temp_x86\Edi.wixobj .\40_temp_x86\EdiProject.wixobj -out Edi_x86.msi

del Edi_x86.wixpdb

PAUSE
