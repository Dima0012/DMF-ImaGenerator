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
        var token = firstCh.ToString();

        while (true)
        {
            var ch = read_char();

            if (ch is not '.' or 'E' or 'e' && !float.TryParse(ch.ToString(), out _))
            {
                unread_char(ch);
                break;
            }

            token += ch;
        }

        float value = 0;
        try
        {
            if (token != null) value = Convert.ToSingle(token);
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

        else if (token.GetType() == typeof(IdentifierToken))
        {
            var varName = token.Identifier;
            if (!scene.FloatVariables.ContainsKey(varName))
            {
                throw new GrammarError(token.Location, $"unknown variable '{varName}'");
            }

            return scene.FloatVariables[varName];
        }

        throw new GrammarError(token.Location, $"got '{token.Symbol}' instead of a number");
    }

    /// <summary>
    /// Read a token from the stream and check if it is a string, then return its value as a string object.
    /// </summary>
    public string expect_string()
    {
        var token = ReadToken();

        if (token.GetType() != typeof(StringToken))
        {
            throw new GrammarError(token.Location, $"got '{token.String}' instead of a string");
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
            throw new GrammarError(Location, $"got '{token.Identifier}' instead of '{symbol}'");
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
                $"expected one of the keywords {string.Join(',', keywords)} instead of '{token.Symbol}'");
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
    /// Parse a point form the scene and returns it as a Point.
    /// </summary>
    public Point parse_point(Scene scene)
    {
        float x = 0, y = 0, z = 0;

        try
        {
            expect_symbol('(');
            x = expect_number(scene);
            expect_symbol(',');
            y = expect_number(scene);
            expect_symbol(',');
            z = expect_number(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return new Point(x, y, z);
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
            //Console.WriteLine($"Error: {ex.Message} at line {ex.Location.LineNum}:{ex.Location.ColNum} in file {ex.Location.FileName}");
            throw new GrammarError(ex.Location, ex.Message);
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
        catch (GrammarError ex)
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
        KeywordEnum keyword;
        IPigment result;
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
        KeywordEnum brdfKeyword;
        IPigment pigment;
        try
        {
            brdfKeyword = expect_keywords(new List<KeywordEnum>()
                {KeywordEnum.Diffuse, KeywordEnum.Specular});
            expect_symbol('(');
            pigment = parse_pigment(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        switch (brdfKeyword)
        {
            case KeywordEnum.Diffuse:
                return new DiffuseBrdf(pigment);
            case KeywordEnum.Specular:
                return new SpecularBrdf(pigment);
        }


        throw new InputStreamError("This line should be unreachable");

    }

    /// <summary>
    /// Parse a material from the scene, and return it as a Material
    /// </summary>
    public Tuple<string, Material> parse_material(Scene scene)
    {
        string name;
        Brdf brdf;
        IPigment emittedRadiance;
        try
        {
            name = expect_identifier();

            expect_symbol('(');
            brdf = parse_brdf(scene);
            expect_symbol(',');
            emittedRadiance = parse_pigment(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }


        return Tuple.Create(name, new Material(brdf, emittedRadiance));
    }

    /// <summary>
    /// Parse a transformation from the scene, and return it as a Transformation.
    /// </summary>
    public Transformation parse_transformation(Scene scene)
    {
        Transformation result = new Transformation();
        KeywordEnum transformationKw;
        while (true)
        {
            try
            {
                transformationKw = expect_keywords(new List<KeywordEnum>()
                {
                    KeywordEnum.Identity,
                    KeywordEnum.Translation,
                    KeywordEnum.Rotation_X,
                    KeywordEnum.Rotation_Y,
                    KeywordEnum.Rotation_Z,
                    KeywordEnum.Scaling,
                });

                if (transformationKw == KeywordEnum.Identity)
                {
                    // Do nothing (this is a primitive form of optimization!) 
                }
                else if (transformationKw == KeywordEnum.Translation)
                {
                    expect_symbol('(');
                    result *= Transformation.translation(parse_vector(scene));
                    expect_symbol(')');
                }
                else if (transformationKw == KeywordEnum.Rotation_X)
                {
                    expect_symbol('(');
                    result *= Transformation.rotation_x(expect_number(scene));
                    expect_symbol(')');
                }
                else if (transformationKw == KeywordEnum.Rotation_Y)
                {
                    expect_symbol('(');
                    result *= Transformation.rotation_y(expect_number(scene));
                    expect_symbol(')');
                }
                else if (transformationKw == KeywordEnum.Rotation_Z)
                {
                    expect_symbol('(');
                    result *= Transformation.rotation_z(expect_number(scene));
                    expect_symbol(')');
                }
                else if (transformationKw == KeywordEnum.Scaling)
                {
                    expect_symbol('(');
                    result *= Transformation.scaling(parse_vector(scene));
                    expect_symbol(')');
                }


            }
            catch (GrammarError ex)
            {
                throw new GrammarError(ex.Location, ex.Message);
            }

            //We must peek the next token to check if there is another transformation that is being
            //chained or if the sequence ends. Thus, this is a LL(1) parser.   
            var nextKw = ReadToken();
            if (nextKw.GetType() != typeof(SymbolToken) || nextKw.Symbol != '*')
            {
                //Pretend you never read this token and put it back!
                unread_token(nextKw);
                break;
            }

        }

        return result;

    }

    /// <summary>
    /// Parse a sphere from the scene, and return it as a Sphere.
    /// </summary>
    public Sphere parse_sphere(Scene scene)
    {
        Transformation transformation;
        string materialName;
        try
        {
            expect_symbol('(');

            materialName = expect_identifier();
            if (!scene.Materials.ContainsKey(materialName))
            {
                // We raise the exception here because input_file is pointing to the end of the wrong identifier
                throw new GrammarError(Location, $"unknown material {materialName}");
            }

            expect_symbol(',');
            transformation = parse_transformation(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return new Sphere(transformation, scene.Materials[materialName]);
    }

    /// <summary>
    /// Parse a plane from the scene, and return it as a Plane.
    /// </summary>
    public Plane parse_plane(Scene scene)
    {
        Transformation transformation;
        string materialName;
        try
        {
            expect_symbol('(');

            materialName = expect_identifier();
            if (!scene.Materials.ContainsKey(materialName))
            {
                // We raise the exception here because input_file is pointing to the end of the wrong identifier
                throw new GrammarError(Location, $"unknown material {materialName}");
            }

            expect_symbol(',');
            transformation = parse_transformation(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return new Plane(transformation, scene.Materials[materialName]);
    }

    /// <summary>
    /// Parse a camera from the scene, and return it as a ICamera.
    /// </summary>
    public ICamera parse_camera(Scene scene)
    {
        KeywordEnum typeKw;
        Transformation transformation;
        float aspectRatio;
        float distance;
        ICamera result = new PerspectiveCamera(new Transformation());
        try
        {
            expect_symbol('(');
            typeKw = expect_keywords(new List<KeywordEnum>()
            {
                KeywordEnum.Perspective,
                KeywordEnum.Orthogonal
            });

            expect_symbol(',');
            transformation = parse_transformation(scene);
            expect_symbol(',');
            aspectRatio = expect_number(scene);
            expect_symbol(',');
            distance = expect_number(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        switch (typeKw)
        {
            case KeywordEnum.Perspective:
                result = new PerspectiveCamera(distance, aspectRatio, transformation);
                break;
            case KeywordEnum.Orthogonal:
                result = new OrthogonalCamera(aspectRatio, transformation);
                break;
        }

        return result;
    }

    /// <summary>
    /// Parse a light source in the scene for point light algorithm, and return it as a PointLight.
    /// </summary>
    public PointLight parse_pointlight(Scene scene)
    {
        var point = new Point();
        var color = new Color();
        var linearRadius = 0f;

        try
        {
            expect_symbol('(');
            point = parse_point(scene);
            expect_symbol(',');
            color = parse_color(scene);
            expect_symbol(',');
            linearRadius = expect_number(scene);
            expect_symbol(')');
        }
        catch (GrammarError ex)
        {
            throw new GrammarError(ex.Location, ex.Message);
        }

        return new PointLight(point, color, linearRadius);
    }

    
    /// <summary>
    /// Parse the scene from the input file (read a scene description from a stream).
    /// </summary>
    public Scene parse_scene(Dictionary<string, float>? variables =  null)
    {
        if(variables == null) variables = new Dictionary<string,float>();
        var scene = new Scene();
        scene.FloatVariables = variables;
        foreach (var s in variables.Keys)
        {
            scene.OverriddenVariables.Add(s);
        }

        string variableName;
        SourceLocation variableLoc;
        float variableValue;
        Tuple<string, Material> tuple;

        while (true)
        {
            var what = ReadToken();
            if (what.GetType() == typeof(StopToken))
            {
                break;
            }

            if (what.GetType() != typeof(KeywordToken))
            {
                throw new GrammarError(what.Location, $"expected a keyword instead of '{what}'");
            }

            switch (what.Keyword)
            {
                case KeywordEnum.Float:
                {
                    try
                    {
                        variableName = expect_identifier();
                        // Save this for the error message
                        variableLoc = Location;
                        expect_symbol('(');
                        variableValue = expect_number(scene);
                        expect_symbol(')');
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }

                    if (scene.FloatVariables.ContainsKey(variableName) && !scene.OverriddenVariables.Contains(variableName))
                    {
                        throw new GrammarError(variableLoc, $"variable «{variableName}» cannot be redefined");
                    }

                    if (!scene.OverriddenVariables.Contains(variableName))
                    {
                        // Only define the variable if it was not defined by the user *outside* the scene file
                        // (e.g., from the command line)
                        scene.FloatVariables[variableName] = variableValue;
                    }

                    break;
                }
                case KeywordEnum.Sphere:
                    try
                    {
                        scene.World.add(parse_sphere(scene));
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }

                    break;
                case KeywordEnum.Plane:
                    try
                    {
                        scene.World.add(parse_plane(scene));
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }

                    break;
                case KeywordEnum.Camera when scene.Camera != null:
                    throw new GrammarError(what.Location, "You cannot define more than one camera");
                case KeywordEnum.Camera:
                    try
                    {
                        scene.Camera = parse_camera(scene);
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }

                    break;
                case KeywordEnum.Material:
                    try
                    {
                        tuple = parse_material(scene);
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }

                    scene.Materials[tuple.Item1] = tuple.Item2;
                    break;
                case KeywordEnum.PointLight:
                    try
                    {
                        scene.World.add_light(parse_pointlight(scene));
                    }
                    catch (GrammarError ex)
                    {
                        throw new GrammarError(ex.Location, ex.Message);
                    }
                    break;
            }
        }

        return scene;
    }
}