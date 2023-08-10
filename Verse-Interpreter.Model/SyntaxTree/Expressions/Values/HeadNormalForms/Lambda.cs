﻿namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Lambda : HeadNormalForm
{
    public required Variable? Parameter { get; set; }

    public required Expression E { get; set; }

    public override string ToString() => $"({Parameter}) => {E}";
}
