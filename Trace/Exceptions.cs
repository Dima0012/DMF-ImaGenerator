namespace Trace;

/// <summary>
/// Rudimentary implementation for a class exception for PFM file reading.
/// </summary>
public partial class InvalidPfmFileFormat : FormatException
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
public class RuntimeError : FormatException
{
    public RuntimeError(string message)
        : base(message)
    {
    }

    public RuntimeError(string message, Exception err)
        : base(message, err)
    {
    }
}