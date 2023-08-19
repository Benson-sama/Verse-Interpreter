grammar Verse;

// -- Parser Rules --

program		: e ;
e			: '(' e ')'								#parenthesisExp
			| VARIABLE ':' TYPE ';' e				#bringIntoScopeExp
			| v										#valueExp
			| e ASTERISK e							#multExp
			| e SLASH e								#DivExp
			| e PLUS e								#plusExp
			| e MINUS e								#minusExp
			| e GREATERTHAN e						#greaterThanExp
			| e LESSTHAN e							#lessThanExp
			| VARIABLE ASSIGN e ';' e				#assignmentExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| <assoc=right> e CHOICE e				#choiceExp
			| e e									#expApplicationExp
			| e EQUALS e ';' e						#eqeExp
			| 'if' '(' e ')' ':' e 'else:' e		#ifElseExp
			| 'for' '(' e ')' 'do' e				#forExp
			| '[' e (',' e)+ ']'					#expTupleExp
			;
v 			: VARIABLE								#variableValue
			| hnf									#hnfValue
			;
hnf			: INTEGER								#integerHnf
			| tuple									#tupleHnf
			| lambda								#lambdaHnf
			;
tuple		: '[' elements? ']' ;
elements	: v (',' elements)* ;
lambda		: tuple LAMBDA e ;

// -- Lexer Rules --

fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

TYPE		: 'any' ;
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
