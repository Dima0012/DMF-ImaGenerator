namespace Trace;
/// <summary>
/// A high-level wrapper around a stream, used to parse scene files
/// This class implements a wrapper around a stream, with the following additional capabilities:
/// - It tracks the line number and column number;
/// - It permits to "un-read" characters and tokens.
/// </summary>
public class InputStream
{
    public Stream Stream;
    public SourceLocation Location;
    public char? SavedChar;
    public SourceLocation SavedLocation;
    public int Tabulations;
    public Token? SavedToken;
    
    public InputStream(Stream stream, SourceLocation sourceLocation, char? savedChar, SourceLocation savedLocation, int tabulations, Token? savedToken)
    {
        Stream = stream;
        Location = sourceLocation;
        SavedChar = savedChar;
        SavedLocation = savedLocation;
        Tabulations = tabulations;
        SavedToken = savedToken;
    }

    public InputStream(Stream stream, string filename = "")
    {
        Stream = stream;
        Location = new SourceLocation(filename, 1, 1);
        SavedChar = null;
        SavedLocation = Location;
        Tabulations = 8;
        SavedToken = null;
    }

    /// <summary>
    /// Update `location` after having read `ch` from the stream
    /// </summary>
    public void update_pos(char? ch)
    {
        if (ch == null) // Nothing to do!
        {
            return;
        }

        if (ch == '\n')
        {
            Location.LineNum += 1;
            Location.ColNum = 1;
        }

        if (ch == '\t')
        {
            Location.ColNum += Tabulations;
        }
        else
        {
            Location.ColNum += 1; 
        }
    }

    /// <summary>
    /// Read a new character from the stream.
    /// </summary>
    public char? read_char()
    {
        char? ch = SavedChar;
        if (SavedChar != null)
        {
            SavedChar = null;
        }
        else
        {
            var buffer = new byte[1];
            if (Stream.Read(buffer, 0, 1) == 0)
            {
                return null;
            }

            ch = Convert.ToChar(buffer);

        }

        SavedLocation = Location;       //without using copy, might generate problems (!)
        update_pos(ch);
        
        return ch;
    }
}