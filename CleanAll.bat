@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO Edi
ECHO Edi.Core
ECHO EdiApp
ECHO EdiDialogs
ECHO EdiDocuments
ECHO EdiViews
ECHO EdiTools
ECHO Themes
ECHO ICSharpCode.AvalonEdit
ECHO.
ECHO Log4NetTools
ECHO.
ECHO MiniUML.Diagnostics
ECHO MiniUML.Framework
ECHO MiniUML.Model
ECHO MiniUML.View
ECHO MiniUML.Plugins.UmlClassDiagram
ECHO.
ECHO Settings
ECHO SettingsView
ECHO SimpleControls
ECHO Util
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

ECHO Deleting BIN and OBJ Folders in EDI folder
ECHO.
RMDIR /S /Q Edi\bin
RMDIR /S /Q Edi\obj

RMDIR /S /Q EdiDialogs\bin
RMDIR /S /Q EdiDialogs\obj

RMDIR /S /Q EdiDocuments\bin
RMDIR /S /Q EdiDocuments\obj

RMDIR /S /Q EdiTools\bin
RMDIR /S /Q EdiTools\obj

RMDIR /S /Q Themes\bin
RMDIR /S /Q Themes\obj

RMDIR /S /Q ICSharpCode.AvalonEdit\bin
RMDIR /S /Q ICSharpCode.AvalonEdit\obj

ECHO Deleting BIN and OBJ Folders in Log4NetTools folders
ECHO.
RMDIR /S /Q .\Tools\Log4NetTools\bin
RMDIR /S /Q .\Tools\Log4NetTools\obj

ECHO Deleting BIN and OBJ Folders in MiniUML folders
ECHO.
RMDIR /S /Q .\MiniUML\MiniUML.Diagnostics\bin
RMDIR /S /Q .\MiniUML\MiniUML.Diagnostics\obj

RMDIR /S /Q .\MiniUML\MiniUML.Framework\bin
RMDIR /S /Q .\MiniUML\MiniUML.Framework\obj

RMDIR /S /Q .\MiniUML\MiniUML.Model\bin
RMDIR /S /Q .\MiniUML\MiniUML.Model\obj

RMDIR /S /Q .\MiniUML\MiniUML.View\bin
RMDIR /S /Q .\MiniUML\MiniUML.View\obj

RMDIR /S /Q .\MiniUML\Plugins\src\MiniUML.Plugins.UmlClassDiagram\bin
RMDIR /S /Q .\MiniUML\Plugins\src\MiniUML.Plugins.UmlClassDiagram\obj

ECHO Deleting BIN and OBJ Folders in Util
ECHO.
RMDIR /S /Q Util\bin
RMDIR /S /Q Util\obj

ECHO Deleting BIN and OBJ Folders in Settings
ECHO.
RMDIR /S /Q Settings\bin
RMDIR /S /Q Settings\obj

ECHO Deleting BIN and OBJ Folders in SettingsView
ECHO.
RMDIR /S /Q SettingsView\bin
RMDIR /S /Q SettingsView\obj

ECHO Deleting BIN and OBJ Folders in SimpleControls
ECHO.
RMDIR /S /Q SimpleControls\bin
RMDIR /S /Q SimpleControls\obj

ECHO Deleting BIN and OBJ Folders in EdiApp
ECHO.
RMDIR /S /Q EdiApp\bin
RMDIR /S /Q EdiApp\obj

ECHO Deleting BIN and OBJ Folders in Edi.Core
ECHO.
RMDIR /S /Q Edi.Core\bin
RMDIR /S /Q Edi.Core\obj

PAUSE

:EndOfBatch
