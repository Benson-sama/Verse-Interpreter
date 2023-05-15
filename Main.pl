:- [init, syntax, rewriting, tests].

main :-
    tokenise(Ts),
    writeln(Ts).

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
    read_file_to_string(File, I, []),
    writeln('Tokenising:'),
    writeln(I),
    string_chars(I, Cs),
    phrase(lines(Ls), Cs),
    phrase(seqq(Ls), S),
    phrase(p(Ts), S).
