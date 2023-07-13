@echo off
call antlr4-parse .\Verse.g4 program .\Input.verse -tokens
call antlr4-parse .\Verse.g4 program .\Input.verse -gui
