grammar Verse;

// Lexer Rules

fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

EQUALS		: '=' ;
ONE			: 'one' ;
ALL			: 'all' ;
CHOICE		: '|' ;
FAIL		: 'false?' ;
ADD			: 'add' ;
PLUS		: '+' ;
ASSIGN		: ':=' ;
MINUS		: '-' ;
ASTERISK	: '*' ;
SLASH		: '/' ;
GREATERTHAN	: '>' ;
LESSTHAN	: '<' ;
LAMBDA		: '=>' ;
INTEGER		: [0-9]+ ;
VARIABLE    : LOWERCASE (LOWERCASE | UPPERCASE)* ;
NEWLINE		: [\r\n] -> skip ;
TAB			: '\t' -> skip ;
WS			: ' ' -> skip ;

// Parser Rules

comment		: '#' ~NEWLINE* NEWLINE ;
program		: e ;
e			: '(' e ')'								#parenthesisExp
			| comment e?							#commentExp
			| e (ASTERISK | SLASH) e				#multDivExp
			| e (PLUS | MINUS) e					#plusMinusExp
			| e GREATERTHAN e						#greaterThanExp
			| e LESSTHAN e							#lessThanExp
			| VARIABLE ASSIGN e (';' e)?			#assignmentExp
			| v										#valueExp
			// | 'E' VARIABLE '. ' e				#existsExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| e CHOICE e							#choiceExp
			| v v									#valueApplicationExp
			| e e									#expApplicationExp
			| e EQUALS e							#expEquationExp
			| 'if' '(' e ')' ':' e 'else:' e		#ifElseExp
			| 'for' '(' e ')' 'do' e				#forExp
			// | ONE '{' e '}'						#oneExp
			// | ALL '{' e '}'						#allExp
			| v EQUALS e (';' e)?					#eqeExp
			;
v 			: VARIABLE | hnf ;
hnf			: INTEGER | tuple | lambda ;
tuple		: '(' elements? ')' ;
elements	: v (', ' elements)* ;
lambda		: '\\' VARIABLE '. ' e ;
