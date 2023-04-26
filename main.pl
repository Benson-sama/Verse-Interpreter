:- [syntax, desugaring, rewriting, tests].

not_implemented :-
    writeln('Not implemented yet.'),
    false.

interpret(VC, Result) :-
    not_implemented,
    desugar(VC, D),         % TODO.
    tokenise(D, Ts),
    rewrite(Ts, Result).    % TODO.

desugar(VC, VC) :-
    not_implemented,
    \+ phrase(desugar(_), VC).
desugar(VC, Result) :-
    not_implemented,
    phrase(desugar(Result1), VC),
    desugar(Result1, Result).

tokenise(Cs, Ts) :- phrase(p(Ts), Cs).

% Sample queries:

% tokenise("one{Ex. Ey. x=5; y=3; 10}", Ts).           
% Ts = one(exists(x, exists(y, eqe(eq(variable(x), hnf(integer(5))),
% eqe(eq(variable(y), hnf(integer(3))), hnf(integer(10))))))) .

% tokenise("one{Ex. Ey. Ez. x=5; y=3; z=x|y; z}", Ts).
% Ts = one(exists(x, exists(y, exists(z, eqe(eq(variable(x), hnf(integer(5))),
% eqe(eq(variable(y), hnf(integer(3))), eqe(eq(variable(z), choice(variable(...),
% variable(...))), variable(z)))))))) .
