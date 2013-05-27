@ECHO OFF
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO AvalonDock
ECHO AvalonDock.Themes.Aero
ECHO AvalonDock.Themes.Expression
ECHO AvalonDock.Theme.ExpressionDark
ECHO AvalonDock.Themes.Metro
ECHO AvalonDock.Theme.VS2010
ECHO Edi
ECHO EdiViews
ECHO Themes
ECHO ICSharpCode.AvalonEdit
ECHO.
ECHO MiniUML.Diagnostics
ECHO MiniUML.Framework
ECHO MiniUML.Model
ECHO MiniUML.View
ECHO MiniUML.Plugins.UmlClassDiagram
ECHO.
ECHO SimpleControls
ECHO Util
ECHO.
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO Deleting BIN and OBJ Folders in AvalonDock
ECHO.
RMDIR /S /Q AvalonDock\AvalonDock\bin
RMDIR /S /Q AvalonDock\AvalonDock\obj
REM DEL /F /Q /S /A:H Automation.Tasks.Core\StyleCop.Cache >> C:\TEMP\CleanFiles.txt

RMDIR /S /Q AvalonDock\AvalonDock.Themes.Aero\bin
RMDIR /S /Q AvalonDock\AvalonDock.Themes.Aero\obj

RMDIR /S /Q AvalonDock\AvalonDock.Theme.VS2010\bin
RMDIR /S /Q AvalonDock\AvalonDock.Theme.VS2010\obj

RMDIR /S /Q AvalonDock\AvalonDock.Themes.Expression\bin
RMDIR /S /Q AvalonDock\AvalonDock.Themes.Expression\obj

RMDIR /S /Q AvalonDock\AvalonDock.Theme.ExpressionDark\bin
RMDIR /S /Q AvalonDock\AvalonDock.Theme.ExpressionDark\obj

RMDIR /S /Q AvalonDock\AvalonDock.Themes.Metro\bin
RMDIR /S /Q AvalonDock\AvalonDock.Themes.Metro\obj

ECHO Deleting BIN and OBJ Folders in EDI folder
ECHO.
RMDIR /S /Q Edi\bin
RMDIR /S /Q Edi\obj

RMDIR /S /Q EdiViews\bin
RMDIR /S /Q EdiViews\obj

RMDIR /S /Q Themes\bin
RMDIR /S /Q Themes\obj

RMDIR /S /Q ICSharpCode.AvalonEdit\bin
RMDIR /S /Q ICSharpCode.AvalonEdit\obj

ECHO Deleting BIN and OBJ Folders in MiniUML folder
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

ECHO Deleting BIN and OBJ Folders in utilities
ECHO.

RMDIR /S /Q SimpleControls\bin
RMDIR /S /Q SimpleControls\obj

RMDIR /S /Q Util\bin
RMDIR /S /Q Util\obj

PAUSE

:EndOfBatch
