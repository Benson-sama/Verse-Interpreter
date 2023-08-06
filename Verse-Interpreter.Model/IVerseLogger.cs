namespace Verse_Interpreter.Model;

public interface IVerseLogger
{
    void Log(string message);

    void LogRuleApplied(string message);
}
