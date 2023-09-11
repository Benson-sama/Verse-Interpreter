//------------------------------------------------------------------------------
// <copyright file="ISyntaxTreeNodeVisitor.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>
// Contains the ISyntaxTreeNodeVisitor and ISyntaxTreeNodeVisitor<T> interfaces.
// </summary>
//------------------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

/// <summary>
/// Interface <see cref="ISyntaxTreeNodeVisitor"/> defines methods to process various types of <see cref="Expression"/>.
/// </summary>
public interface ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// This method processes a specific <see cref="Variable"/>.
    /// </summary>
    /// <param name="variable"><c>variable</c> represents the <see cref="Variable"/> to process.</param>
    void Visit(Variable variable);

    /// <summary>
    /// This method processes a specific <see cref="Integer"/>.
    /// </summary>
    /// <param name="integer"><c>integer</c> represents the <see cref="Integer"/> to process.</param>
    void Visit(Integer integer);

    /// <summary>
    /// This method processes a specific <see cref="VerseString"/>.
    /// </summary>
    /// <param name="verseString"><c>verseString</c> represents the <see cref="VerseString"/> to process.</param>
    void Visit(VerseString verseString);

    /// <summary>
    /// This method processes a specific <see cref="Operator"/>.
    /// </summary>
    /// <param name="verseOperator"><c>verseOperator</c> represents the <see cref="Operator"/> to process.</param>
    void Visit(Operator verseOperator);

    /// <summary>
    /// This method processes a specific <see cref="VerseTuple"/>.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="VerseTuple"/> to process.</param>
    void Visit(VerseTuple verseTuple);

    /// <summary>
    /// This method processes a specific <see cref="Lambda"/>.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Lambda"/> to process.</param>
    void Visit(Lambda lambda);

    /// <summary>
    /// This method processes a specific <see cref="Equation"/>.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Equation"/> to process.</param>
    void Visit(Equation equation);

    /// <summary>
    /// This method processes a specific <see cref="Eqe"/>.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Eqe"/> to process.</param>
    void Visit(Eqe eqe);

    /// <summary>
    /// This method processes a specific <see cref="Exists"/>.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Exists"/> to process.</param>
    void Visit(Exists exists);

    /// <summary>
    /// This method processes a specific <see cref="Fail"/>.
    /// </summary>
    /// <param name="fail"><c>fail</c> represents the <see cref="Fail"/> to process.</param>
    void Visit(Fail fail);

    /// <summary>
    /// This method processes a specific <see cref="Choice"/>.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Choice"/> to process.</param>
    void Visit(Choice choice);

    /// <summary>
    /// This method processes a specific <see cref="Application"/>.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Application"/> to process.</param>
    void Visit(Application application);

    /// <summary>
    /// This method processes a specific <see cref="One"/>.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="One"/> to process.</param>
    void Visit(One one);

    /// <summary>
    /// This method processes a specific <see cref="All"/>.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="All"/> to process.</param>
    void Visit(All all);
}

/// <summary>
/// Interface <see cref="ISyntaxTreeNodeVisitor{T}"/> defines methods to visit various forms of <see cref="Expression"/>.
/// </summary>
public interface ISyntaxTreeNodeVisitor<T>
{
    /// <summary>
    /// This method processes a specific <see cref="Variable"/>.
    /// </summary>
    /// <param name="variable"><c>variable</c> represents the <see cref="Variable"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Variable variable);

    /// <summary>
    /// This method processes a specific <see cref="Integer"/> and returns the result of it.
    /// </summary>
    /// <param name="integer"><c>integer</c> represents the <see cref="Integer"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Integer integer);

    /// <summary>
    /// This method processes a specific <see cref="VerseString"/> and returns the result of it.
    /// </summary>
    /// <param name="verseString"><c>verseString</c> represents the <see cref="VerseString"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(VerseString verseString);

    /// <summary>
    /// This method processes a specific <see cref="VerseTuple"/> and returns the result of it.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="VerseTuple"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(VerseTuple verseTuple);

    /// <summary>
    /// This method processes a specific <see cref="Lambda"/> and returns the result of it.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Lambda"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Lambda lambda);

    /// <summary>
    /// This method processes a specific <see cref="Add"/> and returns the result of it.
    /// </summary>
    /// <param name="add"><c>add</c> represents the <see cref="Add"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Add add);

    /// <summary>
    /// This method processes a specific <see cref="Sub"/> and returns the result of it.
    /// </summary>
    /// <param name="sub"><c>sub</c> represents the <see cref="Sub"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Sub sub);

    /// <summary>
    /// This method processes a specific <see cref="Mult"/> and returns the result of it.
    /// </summary>
    /// <param name="mult"><c>mult</c> represents the <see cref="Mult"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Mult mult);

    /// <summary>
    /// This method processes a specific <see cref="Div"/> and returns the result of it.
    /// </summary>
    /// <param name="div"><c>div</c> represents the <see cref="Div"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Div div);

    /// <summary>
    /// This method processes a specific <see cref="Gt"/> and returns the result of it.
    /// </summary>
    /// <param name="gt"><c>gt</c> represents the <see cref="Gt"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Gt gt);

    /// <summary>
    /// This method processes a specific <see cref="Lt"/> and returns the result of it.
    /// </summary>
    /// <param name="lt"><c>lt</c> represents the <see cref="Lt"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Lt lt);

    /// <summary>
    /// This method processes a specific <see cref="Equation"/> and returns the result of it.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Equation"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Equation equation);

    /// <summary>
    /// This method processes a specific <see cref="Eqe"/> and returns the result of it.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Eqe"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Eqe eqe);

    /// <summary>
    /// This method processes a specific <see cref="Exists"/> and returns the result of it.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Exists"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Exists exists);

    /// <summary>
    /// This method processes a specific <see cref="Fail"/> and returns the result of it.
    /// </summary>
    /// <param name="fail"><c>fail</c> represents the <see cref="Fail"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Fail fail);

    /// <summary>
    /// This method processes a specific <see cref="Choice"/> and returns the result of it.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Choice"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Choice choice);

    /// <summary>
    /// This method processes a specific <see cref="Application"/> and returns the result of it.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Application"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(Application application);

    /// <summary>
    /// This method processes a specific <see cref="One"/> and returns the result of it.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="One"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(One one);

    /// <summary>
    /// This method processes a specific <see cref="All"/> and returns the result of it.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="All"/> to process.</param>
    /// <returns>The result of the processing defined by the generic argument of the <see cref="ISyntaxTreeNodeVisitor{T}"/>.</returns>
    T Visit(All all);
}
