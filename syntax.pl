:- set_prolog_flag(double_quotes, chars).   % Makes strings and list of characters equal.
:- use_module(library(clpfd)).  % Constraint Logic Programming over Finite Domains.
:- table e//1.  % Change execution strategy for phrasing expressions. (Due to problem of nontermination)

% Integers
digit('0') --> "0".
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
digits([D|Ds]) --> digit(D), digits(Ds).
integer(0) --> digit('0').
integer(I) --> digit(D0), digits(D1),
{
    number_chars(I, [D0|D1]),
    dif(D0, '0'),               % Thanks for the advise.
    I #< 2_147_483_648
}.

% Variables
variable(x) --> "x".
variable(y) --> "y".
variable(z) --> "z".
variable(f) --> "f".
variable(g) --> "g".

% Programs
p(one(E)) --> "one{", e(E), "}", { fvs(E, 0) }.

fvs(_, 0). % TODO.

% Expressions
e(fail) --> "fail".
e(one(E)) --> "one{", e(E), "}".
e(all(E)) --> "all{", e(E), "}".
e(exists(V, E)) --> "E", variable(V), ". ", e(E).
e(V) --> v(V).
e(application(V1, V2)) --> v(V1), " ", v(V2).
e(eqe(Eq, E)) --> eq(Eq), "; ", e(E).
e(choice(E1, E2)) --> e(E1), "|", e(E2).
eq(eq(V, E)) --> v(V), "=", e(E).
eq(E) --> e(E).

% Values
v(variable(V)) --> variable(V).
v(hnf(H)) --> hnf(H).

% Head values
hnf(integer(I)) --> integer(I).
hnf(operator(O)) --> operator(O).
hnf(tuple(T)) --> tuple(T).
hnf(lambda(L)) --> lambda(L).

% TODO: Change tuple implementation
% Tuples
tuple_v("") --> "".
tuple_v(V) --> v(V).
tuple_v(V) --> v(V1), ", ", tuple_v(V2),
{
    append([V1, ", ", V2], V)
}.
tuple(tuple(T)) --> "<", tuple_v(T), ">".

% Lambdas
lambda(lambda(V, E)) --> "\\", variable(V), ". ", e(E).

% Primops
operator(gt) --> "gt".
operator(add) --> "add".
