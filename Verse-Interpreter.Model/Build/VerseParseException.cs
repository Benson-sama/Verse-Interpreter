using System.Runtime.Serialization;

namespace Verse_Interpreter.Model.Build;

public class VerseParseException : Exception
{
    public VerseParseException()
    {
    }

    public VerseParseException(string? message) : base(message)
    {
    }

    public VerseParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected VerseParseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
