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
        : base(message)
    {
    }

    public InvalidPfmFileFormat(string message, Exception err)
        : base(message, err)
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
/// An error found by the lexer/parser while reading a scene file. <br />
/// The fields of this type are the following: <br />
/// - `file_name`: the name of the file, or the empty string if there is no real file <br />
/// - `line_num`: the line number where the error was discovered (starting from 1) <br />
/// - `col_num`: the column number where the error was discovered (starting from 1) <br />
/// - `message`: a user-friendly error message
/// </summary>
public class GrammarError : Exception
{
    public SourceLocation Location { get; set; } = new();
    public new string Message { get; set; } = "";

    public GrammarError(SourceLocation sourceLocation, string message)
    {
        Location = sourceLocation;
        Message = message;
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

