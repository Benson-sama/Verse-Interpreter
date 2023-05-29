# Verse Interpreter
The executable is used without command line arguments and instead prompts for input on startup.
This input is then wrapped by the interpreter into a "one()" expression.
Entering "10+20" for example results in "one(10+20)" to ensure the program returns exactly one value.

Implementation of rewriting is currently still at the beginning (unfortunately),
therefore only APP-ADD, APP-GT, APP-GT-FAIL, ONE-FAIL and ONE-VALUE have been tested in a very limited scope. (Example above)

## Collection of Verse programs
As development has not reached a satisfying state where programs can be tested, this collection is highly trivial.

* 1+2
* 1>2
* 10>2

## Deviations from the Verse Syntax
Many desugaring rules are missing and therefore the code is more based towards the Verse-Calculus.
Other than that, there are no own creations except for the string type presented in the LP.

## Build command using SWI-Prolog
`swipl -o VerseInterpreter.exe -c Main.pl --goal=main`
