using System.Text.RegularExpressions;

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

    public InputStream(Stream stream, SourceLocation sourceLocation, char? savedChar, SourceLocation savedLocation,
        int tabulations, Token? savedToken)
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
        switch (ch)
        {
            // Nothing to do!
            case null:
                return;
            case '\n':
                Location.LineNum += 1;
                Location.ColNum = 1;
                break;
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
        var ch = SavedChar;
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

            ch = Convert.ToChar(buffer.GetValue(0));
        }

        SavedLocation = Location; //without using copy, might generate problems (!)
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
        var ch = read_char();
        while (ch is ' ' or '\t' or '\n' or '\r' or '#')
        {
            if (ch == '#')
            {
                // It's a comment! Keep reading until the end of the line (include the case "", the end-of-file)
                while (read_char() is not '\r' or '\n' or null)
                {
                }
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

    /// <summary>
    /// Parse a string token from file.
    /// </summary>
    public StringToken ParseStringToken(SourceLocation tokeLocation)
    {
        var token = "";
        while (true)
        {
            var ch = read_char();

            if (ch is '"')
                break;

            if (ch is null)
                throw new GrammarError(tokeLocation, "Unterminated string");

            token += ch;
        }
        
        return new StringToken(tokeLocation, token);
    }

    public NumberToken ParseNumberToken(char? firstCh, SourceLocation tokenLocation)
    {
        var token = firstCh;

        while (true)
        {
            var ch = read_char();
            
            if (ch is not '.' or 'E' or 'e' || !float.TryParse(ch.ToString(), out _) )
            {
                unread_char(ch);
                break;
            }
            
            token += ch;
        }

        float value = 0;
        try
        {
            if (token != null) value = (float) token;
        }
        catch (Exception)
        {
            throw new GrammarError(tokenLocation, $"{token} is an invalid floating-point number.");
        }

        return new NumberToken(tokenLocation, (float) value);
    }

    public Token ParseKeywordOrIdentifier(string firstCh, SourceLocation tokenLocation)
    {
        var token = firstCh;
        while (true)
        {
            var ch = read_char();
            if (!Regex.IsMatch(ch.ToString() ?? string.Empty, @"^[a-zA-Z0-9_]+$"))
            {
                unread_char(ch);
                break;
            }

            token += ch;
        }

        try
        {
            //return new KeywordToken(tokenLocation);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new IdentifierToken(tokenLocation, token);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}