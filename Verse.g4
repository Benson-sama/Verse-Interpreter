grammar Verse;

// -- Parser Rules --

program		: e ;
e			: '(' e ')'								#parenthesisExp
			| VARIABLE ':' TYPE ';' e				#bringIntoScopeExp
			| VARIABLE ASSIGN e ';' e				#assignmentExp
			| v EQUALS e ';' e						#equalityExp
			| v (ASTERISK | SLASH) v				#multOrDivExp
			| v (PLUS | MINUS) v					#plusOrMinusExp
			| v (GREATERTHAN | LESSTHAN) v (';' e)?	#comparisonExp
			| (v v | v '(' v ')') (';' e)?			#valueApplicationExp
			| v										#valueExp
			| FAIL									#failExp
			| INTEGER '..' INTEGER					#rangeChoiceExp
			| <assoc=right> e CHOICE e				#choiceExp
			| 'if' '(' e '):' e 'else:' e (';' e)?	#ifElseExp
			| 'for' '{' e '}'						#forExp
			| 'for' '(' e ')' 'do' e				#forDoExp
			;
v 			: VARIABLE								#variableValue
			| hnf									#hnfValue
			;
hnf			: INTEGER								#integerHnf
			| string								#stringHnf
			| tuple									#tupleHnf
			| lambda								#lambdaHnf
			;
string		: DOUBLEQUOTES content DOUBLEQUOTES ;
content		: ~DOUBLEQUOTES* ;
tuple		: '[' elements? ']' ;
elements	: v (',' elements)* ;
lambda		: tuple LAMBDA e ;

// -- Lexer Rules --

fragment NUMBER		:	[0-9] ;
fragment LOWERCASE	:	[a-z] ;
fragment UPPERCASE	:	[A-Z] ;

DOUBLEQUOTES: '"' ;
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
INTEGER		: NUMBER+ ;
VARIABLE    : LOWERCASE (LOWERCASE | UPPERCASE | NUMBER)* ;
NEWLINE		: [\r?\n] -> skip ;
TAB			: '\t' -> skip ;
WS			: ' ' -> skip ;
COMMENT		: '#' ~[\r?\n]* [\r?\n] -> skip;
