% This file still applies to a previous version where strings were parsed.
% Therefore this is not working anymore.

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
    append([Cs1, "gt <", E1, ", ", E2, ">", Cs2], X)
}.

% x := e1; e2 means Ex. x=e1; e2
desugar_variable_initialisation(X) -->
    seq(Cs1), variable(V), ws, ":=", ws, e(E1), "; ", e(E2), seq(Cs2),
    {
        append([Cs1, "E", V, ". ", V, "=", E1, "; ", E2, Cs2], X)
    }.
