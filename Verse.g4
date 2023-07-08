grammar Verse;

// Lexer Rules

NEWLINE		: [\r\n]+ ;
INTEGER		: [0-9]+ ;
// VARIABLE    : 
WS			: ' ' -> skip ;

// Parser Rules

program		: (expression NEWLINE)* ;
expression	:
	| expression ('*'|'/') expression
	| expression ('+'|'-') expression
	| INTEGER
	| '(' expression ')'
	;
