namespace Trace;

/// <summary>
/// A lexical token, used when parsing a scene file.
/// </summary>
public abstract class Token
{
    public SourceLocation Location { get; set; }
    public KeywordEnum Keyword { get; set; }
    public string Identifier { get; set; }
    public string String { get; set; }
    public float Number { get; set; }
    public char Symbol { get; set; }

    protected Token(SourceLocation sourceLocation)
    {
        Location = sourceLocation;
    }


}

/// <summary>
/// Enumeration for all the possible keywords recognized by the lexer.
/// </summary>
public enum KeywordEnum : ushort
{
    New = 1,
    Material = 2,
    Plane = 3,
    Sphere = 4,
    Diffuse = 5,
    Specular = 6,
    Uniform = 7,
    Checkered = 8,
    Image = 9,
    Identity = 10,
    Translation = 11,
    RotationX = 12,
    RotationY = 13,
    RotationZ = 14,
    Scaling = 15,
    Camera = 16,
    Orthogonal = 17,
    Perspective = 18,
    Float = 19
}

/// <summary>
/// A Token containing a keyword.
/// </summary>
public class KeywordToken : Token
{

    public Dictionary<ushort, string> Keywords = Enum.GetValues(typeof(KeywordEnum))
    .Cast<KeywordEnum>()
    .ToDictionary(t => (ushort)t, t => t.ToString() );


    public KeywordToken(SourceLocation sourceLocation, KeywordEnum keyword) : base(sourceLocation)
    {
        Keyword = keyword;
    }

}



/// <summary>
/// A Token containing an identifier, a string s.
/// </summary>
public class IdentifierToken : Token
{
    

    public IdentifierToken(SourceLocation sourceLocation, string s) : base(sourceLocation)
    {
        Identifier = s;
    }
}

/// <summary>
/// A Token containing a literal string s.
/// </summary>
public class StringToken : Token
{

    public StringToken(SourceLocation sourceLocation, string s) : base(sourceLocation)
    {
        String = s;
    }
}

/// <summary>
/// A Token containing a floating-point number.
/// </summary>
public class NumberToken : Token
{

    public NumberToken(SourceLocation sourceLocation, float f) : base(sourceLocation)
    {
        Number = f;
    }
}

/// <summary>
/// A Token containing a Symbol, a char ch.
/// </summary>
public class SymbolToken : Token
{
    public SymbolToken(SourceLocation sourceLocation, char ch) : base(sourceLocation)
    {
        Symbol = ch;
    }
}

/// <summary>
/// A Token signalling the end of a file.
/// </summary>
public class StopToken : Token
{
    public StopToken(SourceLocation sourceLocation) : base(sourceLocation)
    {
    }
}