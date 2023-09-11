# Verse Interpreter
This solution describes an implemented interpreter for the functional-logic language Verse,
proposed by Epic Games.

## How to Use
The application call needs command-line arguments according to the following pattern:
```
    {renderMode} {resultMode} {command}

    {renderMode}: -default | -silent | -debug
    {resultMode}: -one | -all
    {command}: -code "your code here" | -interactive | -file "your file path here"
```

## Implemented Features
For detailed information about the supported expressions take a quick look into the "Verse.g4" file.
Some of them are:

- Comments are supported beginning with a '#'.
- Assignment using "x := e"
- Choice as "e1 | e2"
- Failing choice as "false?"
- Choice as range expressions using "Integer..Integer" can be used to create choices with all numbers between the range.

## Missing Features
- Recursion is not working.
- ForDo uses flatMap instead of map, therefore not behaving entirely correct.
- Type system is missing.
- Strings support only lower case characters without special characters and only support concatenation
  via addition and comparison by its length.

## Collection of Verse Programs
Visit the Verse-Interpreter.Test project for samples with expected results.

## Deviations from the Verse Syntax or Verse Paper
- Rule Exi-Swap is implemented but not used because the execution strategy infinite-loops when using it.
- one\{\} equivalent to for\{\} can be used to wrap expressions for retrieving a single value.
- Functions are not implemented as they are only syntactical sugar for lambdas.
