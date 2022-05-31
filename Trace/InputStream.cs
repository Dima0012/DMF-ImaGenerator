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
            case '\t':
                Location.ColNum += Tabulations;
                break;
            default:
                Location.ColNum += 1;
                break;
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

        //SavedLocation = new SourceLocation(Location.FileName, Location.LineNum, Location.ColNum);

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
        // Location = new SourceLocation(SavedLocation.FileName, SavedLocation.LineNum, SavedLocation.ColNum);
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
                // It's a comment; keep reading until the end of the line (include the case null, the end-of-file)
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
    public StringToken ParseStringToken(SourceLocation tokenLocation)
    {
        var token = "";
        while (true)
        {
            var ch = read_char();

            if (ch is '"')
                break;

            if (ch is null)
                throw new GrammarError(tokenLocation, "Unterminated string");

            token += ch;
        }

        return new StringToken(tokenLocation, token);
    }

    /// <summary>
    /// Parse a number token from file .
    /// </summary>
    public NumberToken ParseNumberToken(char? firstCh, SourceLocation tokenLocation)
    {
        var token = firstCh;

        while (true)
        {
            var ch = read_char();

            if (ch is not '.' or 'E' or 'e' || !float.TryParse(ch.ToString(), out _))
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

        return new NumberToken(tokenLocation, value);
    }

    /// <summary>
    /// Parse a Keyword or Identifier token, whether the keyword is recognized by the lexer.
    /// </summary>
    public Token ParseKeywordOrIdentifier(string firstCh, SourceLocation tokenLocation)
    {
        // In this function, firstCh is a string because of some function here that requires string
        var token = firstCh;
        while (true)
        {
            var ch = read_char();
            // If is not alphanumeric or underscore
            if (!Regex.IsMatch(ch.ToString() ?? string.Empty, @"^[a-zA-Z0-9_]+$"))
            {
                unread_char(ch);
                break;
            }

            token += ch;
        }

        // if token is a keyword recognized by the lexer, return a KeywordToken
        if (Enum.TryParse<KeywordEnum>(token, true, out var keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        // Else return an IdentifierToken
        return new IdentifierToken(tokenLocation, token);
    }

    public Token ReadToken()
    {
        skip_whitespaces_and_comments();
        var ch = read_char();

        if (ch is null)
        {
            return new StopToken(Location);
        }

        var tokeLocation = Location;
        var symbols = new List<char> {'(', ')', '<', '>', '[', ']', ',', '*'};

        // A Symbol 
        if (symbols.Contains((char) ch))
        {
            return new SymbolToken(tokeLocation, (char) ch);
        }

        // A literal string
        if (ch == '"')
        {
            return ParseStringToken(tokeLocation);
        }

        // A floating-point number 
        if (char.IsDigit((char) ch) || new List<char> {'+', '-', '.'}.Contains((char) ch))
        {
            return ParseNumberToken(ch, tokeLocation);
        }

        // A keyword or an identifier (first char is alphabetic)
        if (char.IsLetter((char) ch) || ch == '_')
        {
            return ParseKeywordOrIdentifier(ch.ToString()!, tokeLocation);
        }

        // Else ia a weird character, not recognized
        throw new GrammarError(Location, $"Invalid character {ch}");
    }
}