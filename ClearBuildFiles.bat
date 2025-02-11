@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO Deleting all build output files from the ScreenSketcher project...
ECHO.

ECHO Deleting the entire BIN folder for ScreenSketcher...
RMDIR /S /Q ".\src\ScreenSketcher\bin"
ECHO BIN folder deleted.
ECHO.

ECHO Deleting the entire OBJ folder for ScreenSketcher...
RMDIR /S /Q ".\src\ScreenSketcher\obj"
ECHO OBJ folder deleted.
ECHO.

ECHO Deleting the project .vs folder...
RMDIR /S /Q ".\src\ScreenSketcher\.vs"
ECHO Project .vs folder deleted.
ECHO.

ECHO Deleting the root .vs folder...
RMDIR /S /Q ".\.vs"
ECHO Root .vs folder deleted.
ECHO.

ECHO Deleting the setup_output folder...
RMDIR /S /Q ".\setup_output"
ECHO setup_output folder deleted.
ECHO.

ECHO All specified directories have been deleted.
ECHO.
pause