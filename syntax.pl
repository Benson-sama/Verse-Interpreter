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
integer(integer(I)) --> digit(D0), digits(D1),
{
    number_chars(I, [D0|D1]),
    dif(D0, '0'),               % Thanks for the advise.
    I #< 2_147_483_648
}.

% Variables
variable(variable(x)) --> "x".
variable(variable(y)) --> "y".
variable(variable(z)) --> "z".
variable(variable(f)) --> "f".
variable(variable(g)) --> "g".

% Programs
p(one(E)) --> "one{", e(E), "}", { fvs(E, 0) }.

fvs(_, 0). % TODO.

% Expressions
e(e(fail)) --> "fail".
e(e(one(E))) --> "one{", e(E), "}".
e(e(all(E))) --> "all{", e(E), "}".
e(e(exists(V, E))) --> "E", variable(V), ". ", e(E).
e(e(V)) --> v(V).
e(e(application(V1, V2))) --> v(V1), " ", v(V2).
e(eqe(Eq, E)) --> eq(Eq), "; ", e(E).
e(choice(E1, E2)) --> e(E1), "|", e(E2).

e(e(application(add, tuple(E1, E2)))) --> e(E1), "+", e(E2).    % Sugar.
e(e(application(gt, tuple(E1, E2)))) --> e(E1), ">", e(E2). % Sugar.
e(e(exists(V, e(eqe(eq(V, E1), E2))))) --> variable(V), ":=", e(eqe(E1, E2)).  % Sugar.

eq(eq(V, E)) --> v(V), "=", e(E).
eq(eq(E)) --> e(E).

% Values
v(v(V)) --> variable(V).
v(v(H)) --> hnf(H).

% Head values
hnf(hnf(I)) --> integer(I).
hnf(hnf(O)) --> operator(O).
hnf(hnf(T)) --> tuple(T).
hnf(hnf(L)) --> lambda(L).

% Tuples
tuple_v("") --> "".
tuple_v([V]) --> v(V).
tuple_v([V|Vs]) --> v(V), ", ", tuple_v(Vs).
tuple(tuple(T)) --> "<", tuple_v(T), ">".

% Lambdas
lambda(lambda(V, E)) --> "\\", variable(V), ". ", e(E).

% Primops
operator(operator(gt)) --> "gt".
operator(operator(add)) --> "add".
