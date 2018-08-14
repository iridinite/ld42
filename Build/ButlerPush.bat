@echo off
butler -V
butler push Staging/Win64 binaryspark/dark-colosseum:win-64
butler push Staging/Win32 binaryspark/dark-colosseum:win-32
pause
butler status binaryspark/dark-colosseum
pause