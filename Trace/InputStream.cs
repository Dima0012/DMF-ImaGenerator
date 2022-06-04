using System.Diagnostics;
using System.Text.RegularExpressions;
using Trace.Cameras;
using Trace.Geometry;


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
                char? cha;
                do
                {
                    cha = read_char();
                } while (cha != '\r' && cha != '\n' && cha != null);
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
                throw new GrammarError(tokenLocation, "unterminated string");

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
/// <summary>
/// Read a token from the stream.
/// </summary>
public Token ReadToken()
    {
        if (SavedToken != null)
        {
            var result = SavedToken;
            SavedToken = null;
            return result;
        }
        
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
        throw new GrammarError(Location, $"Invalid character '{ch}'");
    }

    /// <summary>
    /// Make as if `token` were never read from `input_file`
    /// </summary>
    public void unread_token(Token token)
    {
        if (SavedToken != null)
        {
            throw new InputStreamError("The saved token is not empty, you should not unread_token");
        }

        SavedToken = token;
    }

    /// <summary>
    /// Returns the expected floating point number from the stream.
    /// </summary>
    public float expect_number(Scene scene)
    {
        var token = ReadToken();

        if (token.GetType() == typeof(NumberToken))
        {
            return token.Number;
        }

        if (token.GetType() == typeof(IdentifierToken))
        {
            var varName = token.Identifier;
            if (!scene.FloatVariables.ContainsKey(varName))
            {
                throw new GrammarError(token.Location, $"unknown variable '{varName}'");
            }
        }

        throw new GrammarError(token.Location, $"got '{token}' instead of a number");
    }

    /// <summary>
    /// Read a token from the stream and check if it is a string, then return its value as a string object.
    /// </summary>
    public string expect_string()
    {
        var token = ReadToken();

        if (token.GetType() != typeof(StringToken))
        {
            throw new GrammarError(token.Location, $"got '{token}' instead of a string");
        }

        return token.String;
    }

    /// <summary>
    /// Read a token from `input_file` and check that it matches `symbol`.
    /// </summary>
    public void expect_symbol(char symbol)
    {
        var token = ReadToken();

        if (token.Symbol != symbol || token.GetType() != typeof(SymbolToken))
        {
            throw new GrammarError(Location, $"got '{token.Symbol}' instead of '{symbol}");
        }
    }

    /// <summary>
    /// Read a token from `input_file` and check that it is one of the keywords in `keywords`.
    /// Return the keyword as a 'KeywordEnum' object.
    /// </summary>
    public KeywordEnum expect_keywords(List<KeywordEnum> keywords)
    {
        var token = ReadToken();
        if (token.GetType() != typeof(KeywordToken))
        {
            throw new GrammarError(token.Location, $"expected a keyword instead of '{token.GetType()}'");
        }

        if (!keywords.Contains(token.Keyword))
        {
            throw new GrammarError(token.Location,
                $"expected one of the keywords {string.Join(',', keywords)} instead of '{token}'");
        }

        return token.Keyword;
    }

    /// <summary>
    /// Read a token from `input_file` and check that it is an identifier.
    /// Return the name of the identifier.
    /// </summary>
    public string expect_identifier()
    {
        var token = ReadToken();
        if (token.GetType() != typeof(IdentifierToken))
        {
            throw new GrammarError(token.Location, $"expected an identifier instead of '{token.GetType()}'");
        }

        return token.Identifier;
    }

    /// <summary>
    /// Parse a vector form the scene and returns it as a Vec.
    /// </summary>
    public Vec parse_vector(Scene scene)
    {
        float x = 0, y = 0, z = 0;
        
        try
        {
            expect_symbol('[');
            x = expect_number(scene);
            expect_symbol(',');
            y = expect_number(scene);
            expect_symbol(',');
            z = expect_number(scene);
            expect_symbol(']');
        }
        catch (GrammarError ex)
        {
            Console.WriteLine($"Error: {ex.Message} at line {ex.Location.LineNum}:{ex.Location.ColNum} in file {ex.Location.FileName}");
        }

        return new Vec(x, y, z);
    }
    

    /// <summary>
    /// Parse a color from the scene and returns it as a Color.
    /// </summary>
    public Color parse_color(Scene scene)
    {
        float red = 0, green = 0, blue = 0;
        try
        {
            expect_symbol('<');
            red = expect_number(scene);
            expect_symbol(',');
            green = expect_number(scene);
            expect_symbol(',');
            blue = expect_number(scene);
            expect_symbol('>');

        }
        catch(GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return new Color(red, green, blue);
    }

    /// <summary>
    /// Parse a pigment from the scene, and return it as a Pigment
    /// </summary>
    public IPigment parse_pigment(Scene scene)
    {
        KeywordEnum keyword = 0;
        IPigment result = new UniformPigment();
        try
        {
            keyword = expect_keywords(new List<KeywordEnum>()
                {KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image});
            expect_symbol('(');
        }
        catch (GrammarError ex)
        {
            //Console.WriteLine($"Error: {ex.Message} at line {ex.Location.LineNum}:{ex.Location.ColNum} in file {ex.Location.FileName}");
            throw new GrammarError(ex.Location, ex.Message);
        }

        if (keyword == KeywordEnum.Uniform)
        {
            var color = parse_color(scene);
            result = new UniformPigment(color);
        }
        else if (keyword == KeywordEnum.Checkered)
        {
            try
            {
                var color1 = parse_color(scene);
                expect_symbol(',');
                var color2 = parse_color(scene);
                expect_symbol(',');
                var numOfSteps = (int) (expect_number(scene));
                result = new CheckeredPigment(color1, color2, numOfSteps);
            }
            catch (GrammarError ex)
            {
                throw new GrammarError(ex.Location, ex.Message);
            }
        }
        else if (keyword == KeywordEnum.Image)
        {
            try
            {
                var fileName = expect_string();
                var image = new HdrImage(fileName);
                result = new ImagePigment(image);
            }
            catch (GrammarError ex)
            {
                throw new GrammarError(ex.Location, ex.Message);
            }
        }
        else
        {
            throw new InputStreamError("This line should be unreachable");
        }

        try
        {
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Parse a BRDF from the scene, and return it as a Brdf.
    /// </summary>
    public Brdf parse_brdf(Scene scene)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Parse a material from the scene, and return it as a Material
    /// </summary>
    public (string, Material) parse_material(Scene scene)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Parse a transformation from the scene, and return it as a Transformation.
    /// </summary>
    public Transformation parse_transformation(Scene scene)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse a sphere from the scene, and return it as a Sphere.
    /// </summary>
    public Sphere parse_sphere(Scene scene)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Parse a plane from the scene, and return it as a Plane.
    /// </summary>
    public Plane parse_plane(Scene scene)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Parse a camera from the scene, and return it as a ICamera.
    /// </summary>
    public ICamera parse_camera(Scene scene)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse a light source in the scene for point light algorithm, and return it as a PointLight.
    /// </summary>
    public PointLight parse_pointlight(Scene scene)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Parse the scene from the input file.
    /// </summary>
    public Scene parse_scene()
    {
        throw new NotImplementedException();
    }

}