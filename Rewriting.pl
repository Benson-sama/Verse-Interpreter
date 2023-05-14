rewrite(X, X) :- \+ rewrite(X, _).
rewrite(X, Z) :-
    rewrite(X, Y),
    rewrite(Y, Z).

% -- Application --

% APP-ADD
rewrite(application(v(hnf(operator(add))),
        v(hnf(
            tuple([v(hnf(integer(K1))),
                   v(hnf(integer(K2)))])))), integer(K3)) :-
    K3 is K1 + K2.

% APP-GT
rewrite(application(v(hnf(operator(gt))),
        v(hnf(
            tuple([v(hnf(integer(K1))),
                   v(hnf(integer(K2)))])))), integer(K1)) :-
    K1 > K2.

% APP-GT-FAIL
rewrite(application(v(hnf(operator(gt))),
        v(hnf(
            tuple([v(hnf(integer(K1))),
                   v(hnf(integer(K2)))])))), e(fail)) :-
    K1 =< K2.

% APP-TUP-0
rewrite(application(v(hnf(tuple([]))), v(_)), e(fail)).

% -- Unification --

% U-LIT
rewrite(eqe(eq(integer(K1), integer(K2)), e(E)), e(E)) :-
    K1 = K2.

% -- Elimination --

% VAL-ELIM
rewrite(eqe(v(_), e(E)), e(E)).

% -- Normalisation --

% -- Choice --

% ONE-FAIL
rewrite(one(e(fail)), fail).

% ONE-VALUE
rewrite(one(v(X)), v(X)).

% ONE-CHOICE
rewrite(one(choice(v(V), e(_))), v(V)).

% -- Traversals --

rewrite(one(X), one(Y)) :-
    rewrite(X, Y).

rewrite(e(X), e(Y)) :-
    rewrite(X, Y).

rewrite(hnf(X), hnf(Y)) :-
    rewrite(X, Y).

rewrite(v(X), v(Y)) :-
    rewrite(X, Y).

rewrite((X), (Y)) :-
    rewrite(X, Y).

rewrite((X), (Y)) :-
    rewrite(X, Y).
