//------------------------------------------------------------
// <copyright file="Variable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Variable class.</summary>
//------------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

/// <summary>
/// Class <see cref="Variable"/> serves as an yet-unknown-logical <see cref="Value"/> in the Verse syntax.
/// </summary>
public class Variable : Value, IEquatable<Variable>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="Variable"/> class.
    /// </summary>
    /// <param name="name"><c>name</c> is the name of the <see cref="Variable"/>.</param>
    public Variable(string name) =>
        Name = name;

    /// <summary>
    /// Property <c>Name</c> represents the name of this <see cref="Variable"/>.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// This method determines the equality of this <see cref="Variable"/> with another <see cref="Variable"/>.
    /// </summary>
    /// <param name="other"><c>other</c> is the other <see cref="Variable"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="other"/> <see cref="Variable"/>.</returns>
    public bool Equals(Variable? other)
    {
        if (other is null)
            return false;

        if (Name == other.Name)
            return true;

        return false;
    }

    /// <summary>
    /// This method tries to determine the equality of this <see cref="Variable"/> with another <see cref="Variable"/>.
    /// </summary>
    /// <param name="obj"><c>obj</c> is the other <see cref="object"/> to compare with.</param>
    /// <returns>A value indicating whether or not this instance is equal to the <paramref name="obj"/> <see cref="object"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Variable variable)
            return Equals(variable);

        return false;
    }

    /// <summary>
    /// Calculates the equality of two <see cref="Variable"/> instances by using the <seealso cref="Equals(Variable?)"/> method.
    /// </summary>
    /// <param name="firstVariable"><c>firstVariable</c> is the first <see cref="Variable"/> to compare.</param>
    /// <param name="secondVariable"><c>secondVariable</c> is the second <see cref="Variable"/> to compare.</param>
    /// <returns>A value indicating whether or not the two <see cref="Variable"/> instances are equal.</returns>
    public static bool operator ==(Variable firstVariable, Variable secondVariable)
        => firstVariable.Equals(secondVariable);

    /// <summary>
    /// Calculates the inequality of two <see cref="Variable"/> instances by using the <seealso cref="Equals(Variable?)"/> method.
    /// </summary>
    /// <param name="firstVariable"><c>firstVariable</c> is the first <see cref="Variable"/> to compare.</param>
    /// <param name="secondVariable"><c>secondVariable</c> is the second <see cref="Variable"/> to compare.</param>
    /// <returns>A value indicating whether or not the two <see cref="Variable"/> instances are inequal.</returns>
    public static bool operator !=(Variable firstVariable, Variable secondVariable)
        => !firstVariable.Equals(secondVariable);

    /// <summary>
    /// This method calculates the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
        => Name.GetHashCode();

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
    /// This method creates a string representation of this <see cref="Variable"/>.
    /// </summary>
    /// <returns>The <c>Name</c> of this variable limited to a maximum of 5 characters,
    /// marked with ² in case the <c>Name</c> is longer.</returns>
    public override string ToString()
    {
        return Name.Length <= 5 ?
            Name
            : string.Concat(Name.Take(5).Concat("²"));
    }
}
