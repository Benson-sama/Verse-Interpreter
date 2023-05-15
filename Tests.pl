test_all :-
    test_desugar_add,
    test_desugar_gt,
    test_desugar_variable_initialisation.

test_desugar_add :-
    % Act
    phrase(p(X), "fail+fail", []),
    % Assert
    X=one(e(application(add, tuple(e(fail), e(fail))))).

test_desugar_gt :-
    % Act
    phrase(p(X), "fail>fail", []),
    % Assert
    X=one(e(application(gt, tuple(e(fail), e(fail))))).

test_desugar_variable_initialisation :-
    % Act
    phrase(p(X), "peter:=fail; fail", []),
    % Assert
    X=one(e(exists(variable("peter"), e(eqe(eq(variable("peter"), eq(e(fail))), e(fail)))))).
