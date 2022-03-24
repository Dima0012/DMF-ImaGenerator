namespace Trace;
/// <summary>
/// Rudimentary implementation for a class exception.
/// </summary>
public partial class InvalidPfmFileFormat: FormatException
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
