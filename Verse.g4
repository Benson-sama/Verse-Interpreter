grammar Verse;

// Lexer Rules

fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

ASSIGN		: ':=' ;
EQUALS		: '=' ;
CHOICE		: '|' ;
FAIL		: 'false?' ;
PLUS		: '+' ;
MINUS		: '-' ;
ASTERISK	: '*' ;
SLASH		: '/' ;
GREATERTHAN	: '>' ;
LESSTHAN	: '<' ;
LAMBDA		: '=>' ;
INTEGER		: [0-9]+ ;
VARIABLE    : LOWERCASE (LOWERCASE | UPPERCASE)* ;
NEWLINE		: [\r?\n] -> skip ;
TAB			: '\t' -> skip ;
WS			: ' ' -> skip ;
COMMENT		: '#' ~[\r?\n]* [\r?\n] -> skip;

// Parser Rules

program		: e ;
e			: '(' e ')'								#parenthesisExp
			| e ASTERISK e							#multExp
			| e SLASH e								#DivExp
			| e PLUS e								#plusExp
			| e MINUS e								#minusExp
			| e GREATERTHAN e						#greaterThanExp
			| e LESSTHAN e							#lessThanExp
			| VARIABLE ASSIGN e (';' e)?			#assignmentExp
			| v										#valueExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| e CHOICE e							#choiceExp
			| v v									#valueApplicationExp
			| e e									#expApplicationExp
			| e EQUALS e							#expEquationExp
			| 'if' '(' e ')' ':' e 'else:' e		#ifElseExp
			| 'for' '(' e ')' 'do' e				#forExp
			| v EQUALS e (';' e)?					#eqeExp
			;
v 			: VARIABLE | hnf ;
hnf			: INTEGER | tuple | lambda ;
tuple		: '(' elements? ')' ;
elements	: v (', ' elements)* ;
lambda		: '\\' VARIABLE '. ' e ;
