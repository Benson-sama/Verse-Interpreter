% Core rewrite loop.
apply_rewrite(X, X) :- \+ rewrite(X, _).
apply_rewrite(X, Z) :-
    rewrite(X, Y),
    apply_rewrite(Y, Z).

% Most of the rewrite rules still require adjustments and many are still missing.

% -- Application --

% APP-ADD
rewrite(
    e(
        application(
            v(hnf(operator(add))),
            v(hnf(tuple([
                v(hnf(integer(K1))),
                v(hnf(integer(K2)))
            ])))
        )
    ),
    e(v(hnf(integer(K3))))
) :- K3 is K1 + K2,
     write('~[APP-ADD]').

% APP-GT
rewrite(
    e(
        application(
            v(hnf(operator(gt))),
            v(hnf(tuple([
                v(hnf(integer(K1))),
                v(hnf(integer(K2)))
            ])))
        )
    ),
    e(v(hnf(integer(K1))))
) :- K1 > K2,
     write('~[APP-GT]').

% APP-GT-FAIL
rewrite(
    e(
        application(
            v(hnf(operator(gt))),
            v(hnf(tuple([
                v(hnf(integer(K1))),
                v(hnf(integer(K2)))
            ])))
        )
    ),
    e(fail)
) :- K1 =< K2,
     write('~[APP-GT-FAIL]').

% APP-TUP-0
rewrite(
    application(
        v(hnf(tuple([]))),
        v(_)
    ),
    e(fail)
).

% -- Unification --

% U-LIT
rewrite(eqe(eq(integer(K1), integer(K2)), e(E)), e(E)) :-
    K1 = K2,
    write('~[u-lit]').

% -- Elimination --

% VAL-ELIM
rewrite(eqe(v(_), e(E)), e(E)).

% -- Normalisation --

% -- Choice --

% ONE-FAIL
rewrite(one(e(fail)), e(fail)) :- write('~[ONE-FAIL]').

% ONE-VALUE
rewrite(one(e(v(hnf(integer(X))))), e(v(hnf(integer(X))))) :- write('~[ONE-VALUE]').

% ONE-CHOICE
rewrite(one(choice(v(V), e(_))), v(V)).

% -- Traversals --

rewrite(one(X), one(Y)) :-
    rewrite(X, Y).

rewrite(all(X), all(Y)) :-
    rewrite(X, Y).

rewrite(exists(V, E1), exists(V, E2)) :-
    rewrite(E1, E2).

rewrite(v(X), v(Y)) :-
    rewrite(X, Y).

rewrite(application(V1, V2), application(V3, V4)) :-
    rewrite(V1, V3),
    rewrite(V2, V4).

rewrite(eqe(Eq1, E1), eqe(Eq2, E2)) :-
    rewrite(Eq1, Eq2),
    rewrite(E1, E2).

rewrite(choice(E1, E2), choice(E3, E4)) :-
    rewrite(E1, E3),
    rewrite(E2, E4).

rewrite(e(X), e(Y)) :-
    rewrite(X, Y).

rewrite(hnf(X), hnf(Y)) :-
    rewrite(X, Y).
