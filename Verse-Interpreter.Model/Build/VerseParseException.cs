//-----------------------------------------------------------------------
// <copyright file="VerseParseException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseParseException class.</summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Verse_Interpreter.Model.Build;

/// <summary>
/// Class <see cref="VerseParseException"/> serves as an <see cref="Exception"/> for parsing Verse code.
/// </summary>
public class VerseParseException : Exception
{
    /// <summary>
    /// Initialises a new instance of the <see cref="VerseParseException"/> class.
    /// </summary>
    public VerseParseException()
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseParseException"/> class.
    /// </summary>
    /// <param name="message"><c>message</c> is the message representing the cause of this exception.</param>
    public VerseParseException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseParseException"/> class.
    /// </summary>
    /// <param name="message"><c>message</c> is the message representing the cause of this exception.</param>
    /// <param name="innerException"><c>innerException</c> is the exception that was thrown prior to this exception.</param>
    public VerseParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseParseException"/> class.
    /// </summary>
    /// <param name="info"><c>info</c> is the additional serialization info for this exception.</param>
    /// <param name="context"><c>context</c> is the additional streaming context for this exception.</param>
    protected VerseParseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
