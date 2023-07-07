grammar Verse;

// Lexer Rules

INTEGER		: [0-9]+ ;
// VARIABLE    : 
WS			: ' ' -> skip ;

// Parser Rules

expression	:
	| INTEGER
	| expression '+' expression;
