@echo off
echo Building lexer and parser...
call antlr4 -no-listener -no-visitor -Dlanguage=CSharp -o Verse-Interpreter.Model/ANTLR Verse.g4
echo Done.
echo.
