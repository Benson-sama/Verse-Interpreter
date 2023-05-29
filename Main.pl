:- [init, syntax, rewriting, tests].

% The entry point of the application.
main :-
    prompt_user_input('Please enter the Verse code to interpret:', Cs),
    interpret(Cs, _).

% Retrieves a list of characters from the user by displaying the given prompt.
prompt_user_input(Prompt, Cs) :-
    writeln(Prompt),
    read_line_to_string(user_input, S),
    string_chars(S, Cs).

% Tokenises and rewrites the given list of characters.
interpret(Verse, Result) :-
    write('Tokenising... '),
    tokenise(Verse, Ts),
    writeln(Ts),
    write('Rewriting... '),
    apply_rewrite(Ts, E),
    result(E, Result),
    write('\nResult: '),
    writeln(Result).

result(e(v(hnf(integer(I)))), I).
result(e(v(hnf(tuple(T)))), T).
result(e(fail), fail).

% Tokenises the given list of characters.
tokenise(Cs, Ts) :- phrase(p(Ts), Cs).

% Helper predicate to quickly tokenise the Main.verse file via the console.
tokenise(Ts) :- tokenise_file('Samples/Main.verse', Ts).

% The conversion using phrase was necessary as alternatives lead to unexpected failure.
% This is because of the prolog flag double_quotes chars.
tokenise_file(File, Ts) :-
    read_file_to_string(File, I, []),
    writeln('Tokenising:'),
    writeln(I),
    string_chars(I, Cs),
    phrase(lines(Ls), Cs),
    phrase(seqq(Ls), S),
    phrase(p(Ts), S).

% Development predicate.
not_implemented :-
    writeln('Not implemented yet.'),
    false.
