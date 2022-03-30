namespace Trace.Geometry;

public struct Vec
{
    public float X { get; set; } = 0.0f;
    public float Y { get; set; } = 0.0f;
    public float Z { get; set; } = 0.0f;

    public Vec(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }


    public string to_string()
    {
        return $"< x: {X}, y: {Y}, z: {Z} >";
    }


    public bool is_close(Vec v, double epsilon = 10e-5)
    {
        return Math.Abs(X - v.X) < epsilon && Math.Abs(Y - v.Y) < epsilon && Math.Abs(Z - v.Z) < epsilon;
    }

    public static Vec operator+(Vec v, Vec u)
    {
        return new Vec(v.X + u.X, v.Y + u.Y, v.Z + u.Z);
    }
    
    public static Vec operator-(Vec v, Vec u)
    {
        return new Vec(v.X - u.X, v.Y - u.Y, v.Z - u.Z);
    }
    
    
    
}