//---------------------------------------------------------------
// <copyright file="VerseString.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseString class.</summary>
//---------------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

/// <summary>
/// Class <see cref="VerseString"/> serves as a <see cref="HeadNormalForm"/> for <see cref="string"/> values in the Verse syntax.
/// </summary>
public class VerseString : HeadNormalForm, IEquatable<VerseString>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="VerseString"/> class.
    /// </summary>
    /// <param name="text"><c>text</c> represents the actual <see cref="string"/> value.</param>
    public VerseString(string text) =>
        Text = text;

    /// <summary>
    /// Property <c>Text</c> represents the actual <see cref="string"/> value of this <see cref="VerseString"/>.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// This method determines the equality of this <see cref="VerseString"/> with another <see cref="VerseString"/>.
    /// </summary>
    /// <param name="other"><c>other</c> is the other <see cref="VerseString"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="other"/> <see cref="VerseString"/>.</returns>
    public bool Equals(VerseString? other)
    {
        if (other is null)
            return false;

        if (Text == other.Text)
            return true;

        return false;
    }

    /// <summary>
    /// This method tries to determine the equality of this <see cref="VerseString"/> with another <see cref="VerseString"/>.
    /// </summary>
    /// <param name="obj"><c>obj</c> is the other <see cref="object"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="obj"/> <see cref="object"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is VerseString text)
            return Equals(text);

        return false;
    }

    /// <summary>
    /// This method calculates the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
        => Text.GetHashCode();

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
    /// This method creates a string representation of this <see cref="VerseString"/>.
    /// </summary>
    /// <returns>A string in the format "{Text}".</returns>
    public override string ToString() => $"\"{Text}\"";
}
