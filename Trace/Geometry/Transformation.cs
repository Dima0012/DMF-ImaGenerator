using System.Numerics;

namespace Trace.Geometry;

/// <summary>
///     An affine Transformation.
/// </summary>
public struct Transformation
{
    public Matrix4x4 M { get; set; }
    public Matrix4x4 InvM { get; set; }

    /// <summary>
    ///     Initialize a Transformation with the specified matrix and inverse.
    /// </summary>
    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        M = m;
        InvM = invM;
    }

    /// <summary>
    ///     Initialize the identity Transformation.
    /// </summary>
    public Transformation()
    {
        M = Matrix4x4.Identity;
        InvM = Matrix4x4.Identity;
    }

    /// <summary>
    ///     Check if two matrices are equal with an epsilon precision.
    /// </summary>
    private static bool are_matr_close(Matrix4x4 ma, Matrix4x4 mb, double epsilon = 1e-5)
    {
        // return Math.Abs(ma.M11 - mb.M11) < epsilon &&
        //        Math.Abs(ma.M12 - mb.M12) < epsilon &&
        //        Math.Abs(ma.M13 - mb.M13) < epsilon &&
        //        Math.Abs(ma.M14 - mb.M14) < epsilon &&
        //        Math.Abs(ma.M21 - mb.M21) < epsilon &&
        //        Math.Abs(ma.M22 - mb.M22) < epsilon &&
        //        Math.Abs(ma.M23 - mb.M23) < epsilon &&
        //        Math.Abs(ma.M24 - mb.M24) < epsilon &&
        //        Math.Abs(ma.M31 - mb.M31) < epsilon &&
        //        Math.Abs(ma.M32 - mb.M32) < epsilon &&
        //        Math.Abs(ma.M33 - mb.M33) < epsilon && 
        //        Math.Abs(ma.M34 - mb.M34) < epsilon &&
        //        Math.Abs(ma.M41 - mb.M41) < epsilon &&
        //        Math.Abs(ma.M42 - mb.M42) < epsilon &&
        //        Math.Abs(ma.M43 - mb.M43) < epsilon &&
        //        Math.Abs(ma.M44 - mb.M44) < epsilon; //seriously?? didn't find a way to access an element as "matrix[i,j]"


        // This piece of code uses reflection to cycle over each matrix element (seen as a field)
        // to check if matrices are equal.

        var typeA = ma.GetType();
        var typeB = mb.GetType();

        var propertiesA = typeA.GetFields();
        var propertiesB = typeB.GetFields();

        foreach (var (first, second) in propertiesA.Zip(propertiesB))
        {
            var elemA = (float) (first.GetValue(ma) ?? 0.0f);
            var elemB = (float) (second.GetValue(mb) ?? 0.0f);

            if (Math.Abs(elemA - elemB) > epsilon) return false;
        }

        return true;
    }

    /// <summary>
    ///     Check if the transformation passed as argument represents the same transformation.
    /// </summary>
    public bool is_close(Transformation t)
    {
        return are_matr_close(M, t.M) && are_matr_close(InvM, t.InvM);
    }

    /// <summary>
    ///     Check if the Transformation is consistent (M and invM product to identity) with
    ///     an epsilon precision.
    /// </summary>
    public bool is_consistent(double epsilon = 1e-5)
    {
        var prod = Matrix4x4.Multiply(M, InvM);
        return are_matr_close(prod, Matrix4x4.Identity, epsilon);
    }

    /// <summary>
    ///     Return a Transformation object encoding a rigid translation.
    ///     The parameter `vec` specifies the amount of shift to be applied along the three axes.
    /// </summary>
    public static Transformation translation(Vec vec)
    {
        var m = new Matrix4x4(
            1.0f, 0.0f, 0.0f, vec.X,
            0.0f, 1.0f, 0.0f, vec.Y,
            0.0f, 0.0f, 1.0f, vec.Z,
            0.0f, 0.0f, 0.0f, 1.0f);

        var invM = new Matrix4x4(
            1.0f, 0.0f, 0.0f, -vec.X,
            0.0f, 1.0f, 0.0f, -vec.Y,
            0.0f, 0.0f, 1.0f, -vec.Z,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    ///     Return a Transformation object encoding a scaling.
    ///     The parameter vec specifies the amount of scaling along the three directions X, Y, Z.
    /// </summary>
    public static Transformation scaling(Vec vec)
    {
        var m = new Matrix4x4(
            vec.X, 0.0f, 0.0f, 0.0f,
            0.0f, vec.Y, 0.0f, 0.0f,
            0.0f, 0.0f, vec.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        var invM = new Matrix4x4(
            1 / vec.X, 0.0f, 0.0f, 0.0f,
            0.0f, 1 / vec.Y, 0.0f, 0.0f,
            0.0f, 0.0f, 1 / vec.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    ///     Return a Transformation object encoding a rotation around the X axis
    ///     The parameter `angle_deg` specifies the rotation angle (in degrees).
    ///     The positive sign is given by the right-hand rule.
    /// </summary>
    public static Transformation rotation_x(float angleDeg)
    {
        var sinang = (float) Math.Sin(Math.PI / 180 * angleDeg);
        var cosang = (float) Math.Cos(Math.PI / 180 * angleDeg);

        var m = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, cosang, -sinang, 0.0f,
            0.0f, sinang, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        var invM = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, cosang, sinang, 0.0f,
            0.0f, -sinang, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    ///     Return a Transformation object encoding a rotation around the Y axis
    ///     The parameter angle_deg specifies the rotation angle (in degrees).
    ///     The positive sign is given by the right-hand rule.
    /// </summary>
    public static Transformation rotation_y(float angleDeg)
    {
        var sinang = (float) Math.Sin(Math.PI / 180 * angleDeg);
        var cosang = (float) Math.Cos(Math.PI / 180 * angleDeg);

        var m = new Matrix4x4(
            cosang, 0.0f, sinang, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            -sinang, 0.0f, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        var invM = new Matrix4x4(
            cosang, 0.0f, -sinang, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            sinang, 0.0f, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    ///     Return a Transformation object encoding a rotation around the Z axis
    ///     The parameter angleDeg specifies the rotation angle (in degrees).
    ///     The positive sign is given by the right-hand rule.
    /// </summary>
    public static Transformation rotation_z(float angleDeg)
    {
        var sinang = (float) Math.Sin(Math.PI / 180 * angleDeg);
        var cosang = (float) Math.Cos(Math.PI / 180 * angleDeg);

        var m = new Matrix4x4(
            cosang, -sinang, 0.0f, 0.0f,
            sinang, cosang, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        var invM = new Matrix4x4(
            cosang, sinang, 0.0f, 0.0f,
            -sinang, cosang, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    ///     Returns the inverse Transformation, switching M and invM
    /// </summary>
    public Transformation inverse()
    {
        return new Transformation(InvM, M);
    }

    public static Transformation operator *(Transformation ta, Transformation tb)
    {
        Matrix4x4 resultM;
        Matrix4x4 resultInvM;

        try
        {
            resultM = Matrix4x4.Multiply(ta.M, tb.M);
            resultInvM = Matrix4x4.Multiply(tb.InvM, ta.InvM);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new Transformation(resultM, resultInvM);
    }

    /// <summary>
    ///     Overloading of multiplication for Transformation of a Point.
    /// </summary>
    public static Point operator *(Transformation t, Point p)
    {
        var m = t.M;
        var resultPoint = new Point(
            p.X * m.M11 + p.Y * m.M12 + p.Z * m.M13 + m.M14,
            p.X * m.M21 + p.Y * m.M22 + p.Z * m.M23 + m.M24,
            p.X * m.M31 + p.Y * m.M32 + p.Z * m.M33 + m.M34);

        var w = p.X * m.M41 + p.Y * m.M42 + p.Z * m.M43 + m.M44;

        return Math.Abs(w - 1.0f) < 1e-5
            ? resultPoint
            : new Point(resultPoint.X / w, resultPoint.Y / w, resultPoint.Z / w);
    }

    /// <summary>
    ///     Overloading of multiplication for Transformation of a Vector.
    /// </summary>
    public static Vec operator *(Transformation t, Vec vec)
    {
        var m = t.M;
        var resultVec = new Vec(
            vec.X * m.M11 + vec.Y * m.M12 + vec.Z * m.M13,
            vec.X * m.M21 + vec.Y * m.M22 + vec.Z * m.M23,
            vec.X * m.M31 + vec.Y * m.M32 + vec.Z * m.M33);

        return resultVec;
    }

    /// <summary>
    ///     Overloading of multiplication for Transformation of a Normal.
    /// </summary>
    public static Normal operator *(Transformation t, Normal n)
    {
        var invM = t.InvM;
        var resultNormal = new Normal(
            n.X * invM.M11 + n.Y * invM.M21 + n.Z * invM.M31,
            n.X * invM.M12 + n.Y * invM.M22 + n.Z * invM.M32,
            n.X * invM.M13 + n.Y * invM.M23 + n.Z * invM.M33);

        return resultNormal;
    }
}