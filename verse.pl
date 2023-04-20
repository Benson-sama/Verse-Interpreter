:- set_prolog_flag(double_quotes, chars).   % Makes strings and list of characters equal.
:- use_module(library(clpfd)).  % Constraint Logic Programming over Finite Domains.
:- table e//1.

interpret(VC, Result) :-
    desugar(VC, Result),
    phrase(p, Result).

desugar(VC, VC) :-
    \+ phrase(desugar(_), VC).
desugar(VC, Result) :-
    phrase(desugar(Result1), VC),
    desugar(Result1, Result).

% -- Syntax --

% Integers
zero('0') --> "0".
digit('1') --> "1".
digit('2') --> "2".
digit('3') --> "3".
digit('4') --> "4".
digit('5') --> "5".
digit('6') --> "6".
digit('7') --> "7".
digit('8') --> "8".
digit('9') --> "9".
digits([]) --> "".
digits([Z|Ds]) --> zero(Z), digits(Ds).
digits([D|Ds]) --> digit(D), digits(Ds).
integer(0) --> zero(_).
integer(I, [D0|D1]) --> digit(D0), digits(D1), { number_chars(I, [D0|D1]), I #< 2_147_483_648 }.

% Variables
variable("x") --> "x".
variable("y") --> "y".
variable("z") --> "z".
variable("f") --> "f".
variable("g") --> "g".

% Programs
p --> "one{", e(E), "}", { fvs(E, 0) }.

% Fake.
fvs(_, 0).

% Expressions
e("fail") --> "fail".
e(E) --> "one{", e(E1), "}",
{
    append(["one{", E1, "}"], E)
}.
e(E) --> "all{", e(E1), "}",
{
    append(["all{", E1, "}"], E)
}.
e(E) --> "E", variable(V), ". ", e(E1),
{
    append(["E", V, ". ", E1], E)
}.
e(X) --> v(X).
e(E) --> v(V1), v(V2),
{
    append([V1, V2], E)
}.
e(E) --> eq(Eq), "; ", e(E1),
{
    append([Eq, "; ", E1], E)
}.
e(E) --> e(E1), "|", e(E2),
{
    append([E1, "|", E2], E)
}.
eq(Eq) --> v(V), "=", e(E),
{
    append([V, "=", E], Eq)
}.
eq(X) --> e(X).

% Values
v(X) --> variable(X) | hnf(X).

% Head values
hnf(X) --> integer(_, X) | operator(X) | tuple(X) | lambda(X).

% Tuples
tuple_v("") --> "".
tuple_v(V) --> v(V).
tuple_v(V) --> v(V1), ", ", tuple_v(V2),
{
    append([V1, ", ", V2], V)
}.
tuple(T) --> "<", tuple_v(V), ">",
{
    append(["<", V, ">"], T)
}.

% Lambdas
lambda(L) --> "\\", variable(V), ". ", e(E),
{
    append(["\\", V, ". ", E], L)
}.

% Primops
operator("gt") --> "gt".
operator("add") --> "add".

% -- Desugar --

% Sequences 
seq([])     --> [].
seq([E|Es]) --> [E], seq(Es).

% Whitespaces
ws --> [].
ws --> " ", ws.

desugar(X) --> desugar_gt(X) | desugar_add(X) | desugar_variable_initialisation(X).

% e1 + e2 means add<e1, e2>
desugar_add(X) --> seq(Cs1), e(E1), ws, "+", ws, e(E2), seq(Cs2),
{
    append([Cs1, "add<", E1, ", ", E2, ">", Cs2], X)
}.

% e1 > e2 means gt<e1, e2>
desugar_gt(X) --> seq(Cs1), e(E1), ">", e(E2), seq(Cs2),
{
    append([Cs1, "gt<", E1, ", ", E2, ">", Cs2], X)
}.

% x := e1; e2 means Ex. x=e1; e2
desugar_variable_initialisation(X) -->
    seq(Cs1), variable(V), ws, ":=", ws, e(E1), "; ", e(E2), seq(Cs2),
    {
        append([Cs1, "E", V, ". ", V, "=", E1, "; ", E2, Cs2], X)
    }.

% Test
test_all :-
    test_desugar_add,
    test_desugar_gt,
    test_desugar_variable_initialisation.

test_desugar_add :-
    % Act
    phrase(desugar_add(X), "one{fail+fail}", []),
    % Assert
    X="one{add<fail, fail>}".

test_desugar_gt :-
    % Act
    phrase(desugar_gt(X), "one{fail>fail}", []),
    % Assert
    X="one{gt<fail, fail>}".

test_desugar_variable_initialisation :-
    % Act
    phrase(desugar_variable_initialisation(X), "one{x := fail; fail}", []),
    % Assert
    X="one{Ex. x=fail; fail}".
