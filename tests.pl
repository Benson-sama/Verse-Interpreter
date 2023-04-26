% This file still applies to a previous version where strings were parsed.
% Therefore this is not working anymore.

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
