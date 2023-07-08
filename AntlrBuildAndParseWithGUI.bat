@echo off
echo Building lexer and parser...
call antlr4 -o ANTLR\Java Verse.g4
echo Compiling Java files...
cd ANTLR\Java
call javac *.java
echo Parsing the Input.verse file...
call grun Verse expression ..\..\Input.verse -gui
