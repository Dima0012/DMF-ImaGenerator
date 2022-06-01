namespace Trace;

/// <summary>
/// Rudimentary implementation for a class exception for PFM file reading.
/// </summary>
public class InvalidPfmFileFormat : FormatException
{
    public InvalidPfmFileFormat(string message) : base(message)
    {
    }

    public InvalidPfmFileFormat(string message, Exception err) : base(message, err)
    {
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
    /// <summary>
    /// The Location of the error.
    /// </summary>
    public SourceLocation Location { get; set; }

    public GrammarError(SourceLocation sourceLocation, string message) : base(message)
    {
        Location = sourceLocation;
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

