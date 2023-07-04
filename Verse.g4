grammar Verse;

/*
 * Lexer Rules
 */

NUMBER		: [0-9]+ ;
// VARIABLE    : 
WS			: ' ' -> skip ;

/*
 * Parser Rules
 */

expression	: NUMBER '+' NUMBER ;
