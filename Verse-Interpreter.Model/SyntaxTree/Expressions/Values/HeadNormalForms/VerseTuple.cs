//--------------------------------------------------------------
// <copyright file="VerseTuple.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseTuple class.</summary>
//--------------------------------------------------------------

using System.Collections;
using System.Text;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

/// <summary>
/// Class <see cref="VerseTuple"/> serves as a <see cref="HeadNormalForm"/> for collections of
/// <see cref="Value"/> instances in the Verse syntax.
/// </summary>
public class VerseTuple : HeadNormalForm, IEnumerable<Value>
{
    /// <summary>
    /// Field <c>_empty</c> represents an empty <see cref="VerseTuple"/>.
    /// </summary>
    private static readonly VerseTuple _empty = new(Array.Empty<Value>());

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseTuple"/> instance with a single <paramref name="value"/>.
    /// </summary>
    /// <param name="value"><c>value</c> is the single <see cref="Value"/> to insert.</param>
    public VerseTuple(Value value) => Values = Enumerable.Repeat(value, 1);

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseTuple"/> instance with two values.
    /// </summary>
    /// <param name="firstValue"><c>firstValue</c> is the first <see cref="Value"/> to insert.</param>
    /// <param name="secondValue"><c>secondValue</c> is the second <see cref="Value"/> to insert.</param>
    public VerseTuple(Value firstValue, Value secondValue) => Values = new Value[] { firstValue, secondValue };

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseTuple"/> instance with given <paramref name="values"/>.
    /// </summary>
    /// <param name="values"> represents the collection of values to insert.</param>
    public VerseTuple(IEnumerable<Value> values) => Values = values;

    /// <summary>
    /// Property <c>Empty</c> represents an empty <see cref="VerseTuple"/>.
    /// </summary>
    public static VerseTuple Empty { get => _empty; }

    /// <summary>
    /// Property <c>Values</c> represents the elements of this <see cref="VerseTuple"/>.
    /// </summary>
    public IEnumerable<Value> Values { get; set; }

    /// <summary>
    /// This method retrieves an enumerator that iterates through this <see cref="VerseTuple"/>.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through this <see cref="VerseTuple"/>.</returns>
    public IEnumerator<Value> GetEnumerator() => Values.GetEnumerator();

    /// <summary>
    /// This method retrieves an enumerator that iterates through this <see cref="VerseTuple"/>.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through this <see cref="VerseTuple"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor"/> in order to call it back.
    /// </summary>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor"/> to call back.</param>
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor{T}"/> in order to call it back.
    /// </summary>
    /// <typeparam name="T">The return type of this operation.</typeparam>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor{T}"/> to call back.</param>
    /// <returns>The result of the <c>visitor</c> callback.</returns>
    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="VerseTuple"/>.
    /// </summary>
    /// <returns>A string in the format "[]" for empty tuples or [v1, v2, ..., vn] for n > 0.</returns>
    public override string ToString()
    {
        if (!Values.Any())
            return "[]";

        StringBuilder sb = new("[");
        sb.Append($"{Values.First()}");

        foreach (Value value in Values.Skip(1))
        {
            sb.Append($", {value}");
        }

        sb.Append(']');
        return sb.ToString();
    }
}
