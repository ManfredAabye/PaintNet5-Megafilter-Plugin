@echo off

REM Build Script for Megafilter Paint.NET Plugin
REM This script compiles the Megafilter project using dotnet CLI

dotnet restore
dotnet build -c Release

echo Build completed. The compiled files are located in bin\Release\net9.0-windows\
REM Kopie Release to MEGAFILTER/upload
xcopy /Y /E ".\bin\Release\net9.0-windows\" ".\upload\"
echo Files copied to MEGAFILTER\upload
pause
