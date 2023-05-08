% This file still applies to a previous version where strings were parsed.
% Therefore this is not working anymore.

test_all :-
    test_desugar_add.

test_desugar_add :-
    % Act
    phrase(p(X), "one{fail+fail}", []),
    % Assert
    X=one(e(application(add, tuple(e(fail), e(fail))))).

test_desugar_gt :-
    % Act
    phrase(p(X), "one{fail>fail}", []),
    % Assert
    X="one{gt<fail, fail>}".

test_desugar_variable_initialisation :-
    % Act
    phrase(p(X), "one{x := fail; fail}", []),
    % Assert
    X="one{Ex. x=fail; fail}".
