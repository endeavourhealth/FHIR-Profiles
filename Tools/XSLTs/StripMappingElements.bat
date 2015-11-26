@echo off

echo WARNING - This will run in place on existing StructureDefinitions.  Ensure all changes are committed to source control before continuing.

echo.

pause

set CURRENT_DIR=%~dp0

for %%f in (%CURRENT_DIR%..\..\StructureDefinition\*.xml) do call :PROCESS_FILE %%f

pause

goto :EOF

:PROCESS_FILE

echo Processing %1
%CURRENT_DIR%xslt.exe %1 StripMappingElements.xsl %1
goto :EOF

