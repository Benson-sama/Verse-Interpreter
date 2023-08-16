grammar Verse;

// -- Parser Rules --

program		: e ;
e			: v										#valueExp
			| '(' e ')'								#parenthesisExp
			| VARIABLE ':' TYPE ';' e				#bringIntoScopeExp
			| e ASTERISK e							#multExp
			| e SLASH e								#DivExp
			| e PLUS e								#plusExp
			| e MINUS e								#minusExp
			| e GREATERTHAN e						#greaterThanExp
			| e LESSTHAN e							#lessThanExp
			| VARIABLE ASSIGN e (';' e)?			#assignmentExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| <assoc=right> e CHOICE e				#choiceExp
			| e e									#expApplicationExp
			| e EQUALS e							#expEquationExp
			| 'if' '(' e ')' ':' e 'else:' e		#ifElseExp
			| 'for' '(' e ')' 'do' e				#forExp
			| v EQUALS e (';' e)?					#eqeExp
			| '(' e (',' e)+ ')'					#expTupleExp
			;
v 			: VARIABLE								#variableValue
			| hnf									#hnfValue
			;
hnf			: INTEGER								#integerHnf
			| tuple									#tupleHnf
			| lambda								#lambdaHnf
			;
tuple		: '(' elements? ')' ;
elements	: v (',' elements)* ;
lambda		: tuple LAMBDA e ;

// -- Lexer Rules --

fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

TYPE		: 'int' ;
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
