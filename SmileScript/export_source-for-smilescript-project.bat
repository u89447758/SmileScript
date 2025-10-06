@echo off
setlocal enabledelayedexpansion

REM --- Configuration ---
REM Set the name for the output text file
set "OutputFile=Exported_Source_Code.txt"

REM List the folders you want to export (separated by spaces)
REM MODIFIED: Added the 'Services' and 'ViewComponents' folders
set "FoldersToExport=wwwroot Areas Controllers Data Models Views ViewModels Enums"

REM List the individual files you want to export (separated by spaces)
set "FilesToExport=appsettings.json Program.cs"

REM --- Script Start ---

REM Delete the output file if it already exists to start fresh
if exist "%OutputFile%" (
    del "%OutputFile%"
    echo Previous output file '%OutputFile%' has been deleted.
)

echo Starting the export of your source code...
echo.

REM --- Process Folders ---
echo Exporting directories... >> "%OutputFile%"
echo ================================== >> "%OutputFile%"
echo. >> "%OutputFile%"

for %%D in (%FoldersToExport%) do (
    if exist "%%D" (
        echo Processing folder: %%D
        REM --- MODIFIED METHOD: Added a findstr to exclude the 'uploads' folder ---
        for /f "delims=" %%F in ('dir /s /b /a-d "%%D\*.*" ^| findstr /v /i "\\lib\\" ^| findstr /v /i "\\images\\" ^| findstr /v /i /r "\\favicon\.ico$"') do (
            echo -------------------------------------------------- >> "%OutputFile%"
            echo   File: %%F >> "%OutputFile%"
            echo -------------------------------------------------- >> "%OutputFile%"
            echo. >> "%OutputFile%"
            type "%%F" >> "%OutputFile%"
            echo. >> "%OutputFile%"
            echo. >> "%OutputFile%"
        )
    ) else (
        echo [WARNING] Folder "%%D" not found.
    )
)

REM --- Process Individual Files ---
echo. >> "%OutputFile%"
echo Exporting individual files... >> "%OutputFile%"
echo ================================== >> "%OutputFile%"
echo. >> "%OutputFile%"

for %%I in (%FilesToExport%) do (
    if exist "%%I" (
        echo Processing file: %%I
        echo -------------------------------------------------- >> "%OutputFile%"
        echo   File: %%I >> "%OutputFile%"
        echo -------------------------------------------------- >> "%OutputFile%"
        echo. >> "%OutputFile%"
        type "%%I" >> "%OutputFile%"
        echo. >> "%OutputFile%"
        echo. >> "%OutputFile%"
    ) else (
        echo [WARNING] File "%%I" not found.
    )
)

echo.
echo ======================================================
echo  Source code export is complete!
echo  All content has been saved to: %OutputFile%
echo ======================================================
echo.

endlocal
pause