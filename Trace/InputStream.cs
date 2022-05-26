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
        //Note that we start counting lines/columns from 1, not from 0
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
                                        //maybe I solved turning SourceLocation into a struct
        update_pos(ch);
        
        return ch;
    }

    /// <summary>
    /// Push a character back to the stream.
    /// </summary>
    public void unread_char(char? ch)
    {
        if (SavedChar != null)
        {
            throw new InputStreamError("The saved char is not empty, you should not unread_char");
        }

        SavedChar = ch;
        Location = SavedLocation;
    }

    /// <summary>
    /// Keep reading characters until a non-whitespace/non-comment character is found.
    /// </summary>
    public void skip_whitespaces_and_comments()
    {
        char? ch = read_char();
        while (ch is ' ' or '\t' or '\n' or '\r' or '#')
        {
            if (ch == '#')
            {
                // It's a comment! Keep reading until the end of the line (include the case "", the end-of-file)
                while (read_char() is not '\r' or '\n' or null){}
                
            }

            ch = read_char();
            if (ch == null)
            {
                return;
            }
            
        }
        
       //Put the non-whitespace character back
        unread_char(ch);
    }
    
    //public StringToken 
}