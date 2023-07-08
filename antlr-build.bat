@echo off
echo Building lexer and parser...
call antlr4 -Dlanguage=CSharp -o ANTLR\C# Verse.g4
echo Done.
echo.
