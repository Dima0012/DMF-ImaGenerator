namespace Trace;

/// <summary>
/// Rudimentary implementation for a class exception for PFM file reading.
/// </summary>
public class InvalidPfmFileFormat : FormatException
{
    public InvalidPfmFileFormat()
    {
    }

    public InvalidPfmFileFormat(string message)
    {
        Console.WriteLine("Error: "+message);
        Environment.Exit(-1);
    }

    public InvalidPfmFileFormat(string message, Exception err)
    {

        Console.WriteLine("Error: "+message);
        Environment.Exit(-1);
    }
}

/// <summary>
/// Rudimentary implementation for a class exception for reading input parameters.
/// </summary>
public class InputError : FormatException
{
    public InputError(string message)
        : base(message)
    {
    }

    public InputError(string message, Exception err)
        : base(message, err)
    {
    }
    
}

/// <summary>
/// An error found by the lexer/parser while reading a scene file.
/// </summary>
public class GrammarError : Exception
{
    public SourceLocation Location { get; set; }
    public new string Message { get; set; }

    public GrammarError(SourceLocation sourceLocation, string message)
    {
        Location = sourceLocation;
        Message = message;
        
        Console.WriteLine($"\nError: {Message} at line {Location.LineNum}:{Location.ColNum} in file {Location.FileName}.");
        Console.WriteLine("Exiting application");
        Environment.Exit(-1);
    }

}

/// <summary>
/// Rudimentary implementation for a class exception for reading input stream (related to scene files).
/// </summary>
public class InputStreamError : FormatException
{
    public InputStreamError(string message)
        : base(message)
    {
    }

    public InputStreamError(string message, Exception err)
        : base(message, err)
    {
    }
    
}

