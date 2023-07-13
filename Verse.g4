grammar Verse;

// Lexer Rules

fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

EQUALS		: '=' ;
ONE			: 'one' ;
ALL			: 'all' ;
CHOICE		: '|' ;
FAIL		: 'fail' ;
ADD			: 'add' ;
PLUS		: '+' ;
MINUS		: '-' ;
ASTERISK	: '*' ;
SLASH		: '/' ;
GT			: '>' ;
INTEGER		: [0-9]+ ;
VARIABLE    : LOWERCASE (LOWERCASE | UPPERCASE)* ;
NEWLINE		: [\r\n] -> skip ;
WS			: ' ' -> skip ;

// Parser Rules

comment		: '#' ~NEWLINE* NEWLINE ;
program		: e ;
e			:
	| '(' e ')'
	| comment
	| v
	| 'E' VARIABLE '. ' e
	| FAIL
	| e CHOICE e
	| v v
	| ONE '{' e '}'
	| ALL '{' e '}'
	| VARIABLE
	| INTEGER
	| v EQUALS e ';' e
	| e ';' e
	;
// eq			: v EQUALS e | e ;
v 			: VARIABLE | hnf ;
hnf			: INTEGER | op | tuple | lambda ;
tuple		: '(' elements? ')' ;
elements	: v (', ' elements)* ;
lambda		: '\\' VARIABLE '. ' e ;
op 			: GT | ADD ;
