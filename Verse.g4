grammar Verse;

// -- Parser Rules --

program		: e ;
e			: '(' e ')'								#parenthesisExp
			| VARIABLE ':' TYPE ';' e				#bringIntoScopeExp
			| VARIABLE ASSIGN e ';' e				#assignmentExp
			| v EQUALS e ';' e						#equalityExp
			| v (ASTERISK | SLASH) v				#multOrDivExp
			| v (PLUS | MINUS) v					#plusOrMinusExp
			| v (GREATERTHAN | LESSTHAN) v			#comparisonExp
			| v v									#valueApplicationExp
			| v										#valueExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| <assoc=right> e CHOICE e				#choiceExp
			| 'if' '(' e '):' e 'else:' e			#ifElseExp
			| 'for' '(' e ')' 'do' e				#forExp
			// | e e								#expApplicationExp
			// | '[' e (',' e)+ ']'					#expTupleExp
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
