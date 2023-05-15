test_all :-
    test_app_add,
    test_app_gt,
    test_desugar_variable_initialisation.

test_app_add :-
    phrase(p(X), "10+20"),
    phrase(p(X), "add <10, 20>"),
    X = one(
        e(application(
            v(hnf(operator(add))),
            v(hnf(tuple([
                v(hnf(integer(10))),
                v(hnf(integer(20)))
            ])))
        ))
    ).

test_app_gt :-
    phrase(p(X), "10>20"),
    phrase(p(X), "gt <10, 20>"),
    X = one(
        e(application(
            v(hnf(operator(gt))),
            v(hnf(tuple([
                v(hnf(integer(10))),
                v(hnf(integer(20)))
            ])))
        ))
    ).

test_desugar_variable_initialisation :-
    phrase(p(X), "Epeter. peter=fail; fail"),
    phrase(p(X), "peter:=fail; fail"),
    X=one(e(exists(variable("peter"), eqe(eq(v(variable("peter")), e(fail)), e(fail))))).
