:- [init, syntax, rewriting, tests].

not_implemented :-
    writeln('Not implemented yet.'),
    false.

interpret(Verse, Result) :-
    tokenise(Verse, Ts),
    rewrite(Ts, Result).

% desugar(VC, VC) :-
%     not_implemented,
%     \+ phrase(desugar(_), VC).
% desugar(VC, Result) :-
%     not_implemented,
%     phrase(desugar(Result1), VC),
%     desugar(Result1, Result).

tokenise(Cs, Ts) :- phrase(p(Ts), Cs).

tokenise(Ts) :- tokenise_file('samples/main.verse', Ts).

tokenise_file(File, Ts) :-
    read_file_to_string(File, S, []),
    string_chars(S, Cs),
    phrase(p(Ts), Cs).
