namespace Trace;
/// <summary>
/// A specific position in a source file
/// This class has the following fields:
/// - file_name: the name of the file, or the empty string if there is no file associated with this location
///     (e.g., because the source code was provided as a memory stream, or through a network connection)
/// - line_num: number of the line (starting from 1)
/// - col_num: number of the column (starting from 1)
/// </summary>
public struct SourceLocation
{
    public string FileName;
    public int LineNum;
    public int ColNum;
    
    public SourceLocation()
    {
        FileName = "";
        LineNum = 0;
        ColNum = 0;
    }

    public SourceLocation(string fileName, int lineNum, int colNum)
    {
        FileName = fileName;
        LineNum = lineNum;
        ColNum = colNum;
    }
}