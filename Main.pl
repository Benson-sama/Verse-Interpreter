:- [init, syntax, rewriting, tests].

% Development predicate.
not_implemented :-
    writeln('Not implemented yet.'),
    false.

interpret(Verse, Result) :-
    tokenise(Verse, Ts),
    rewrite(Ts, Result).

tokenise(Cs, Ts) :- phrase(p(Ts), Cs).

tokenise(Ts) :- tokenise_file('samples/main.verse', Ts).

tokenise_file(File, Ts) :-
    read_file_to_string(File, S, []),
    write('Tokenising: '),
    writeln(S),
    string_chars(S, Cs),
    phrase(p(Ts), Cs).
