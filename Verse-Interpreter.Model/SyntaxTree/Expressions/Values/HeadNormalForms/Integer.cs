//-----------------------------------------------------------
// <copyright file="Integer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Integer class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

/// <summary>
/// Class <see cref="Integer"/> serves as a <see cref="HeadNormalForm"/> for <see cref="int"/> values in the Verse syntax.
/// </summary>
public class Integer : HeadNormalForm
{
    /// <summary>
    /// Initialises a new instance of the <see cref="Integer"/> class.
    /// </summary>
    /// <param name="value"><c>value</c> represents the actual <see cref="int"/> value.</param>
    public Integer(int value) =>
        Value = value;

    /// <summary>
    /// Property <c>Value</c> represents the actual <see cref="int"/> value of this <see cref="Integer"/>.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// This method determines the equality of this <see cref="Integer"/> with another <see cref="Integer"/>.
    /// </summary>
    /// <param name="other"><c>other</c> is the other <see cref="Integer"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="other"/> <see cref="Integer"/>.</returns>
    public bool Equals(Integer? other)
    {
        if (other is null)
            return false;

        if (Value == other.Value)
            return true;

        return false;
    }

    /// <summary>
    /// This method tries to determine the equality of this <see cref="Integer"/> with another <see cref="Integer"/>.
    /// </summary>
    /// <param name="obj"><c>obj</c> is the other <see cref="object"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="obj"/> <see cref="object"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Integer integer)
            return Equals(integer);

        return false;
    }

    /// <summary>
    /// Calculates the equality of two <see cref="Integer"/> instances by using the <seealso cref="Equals(Integer?)"/> method.
    /// </summary>
    /// <param name="firstInteger"><c>firstInteger</c> is the first <see cref="Integer"/> to compare.</param>
    /// <param name="secondInteger"><c>secondInteger</c> is the second <see cref="Integer"/> to compare.</param>
    /// <returns>A value indicating whether or not the two <see cref="Integer"/> instances are equal.</returns>
    public static bool operator ==(Integer firstInteger, Integer secondInteger)
        => firstInteger.Equals(secondInteger);

    /// <summary>
    /// Calculates the inequality of two <see cref="Integer"/> instances by using the <seealso cref="Equals(Integer?)"/> method.
    /// </summary>
    /// <param name="firstInteger"><c>firstInteger</c> is the first <see cref="Integer"/> to compare.</param>
    /// <param name="secondInteger"><c>secondInteger</c> is the second <see cref="Integer"/> to compare.</param>
    /// <returns>A value indicating whether or not the two <see cref="Integer"/> instances are inequal.</returns>
    public static bool operator !=(Integer firstInteger, Integer secondInteger)
        => !firstInteger.Equals(secondInteger);

    /// <summary>
    /// This method calculates the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
        => Value.GetHashCode();

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
    /// This method creates a string representation of this <see cref="Integer"/>.
    /// </summary>
    /// <returns>A string representing the <c>Value</c>.</returns>
    public override string ToString() => $"{Value}";
}
