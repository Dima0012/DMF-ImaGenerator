using System.Numerics;

namespace Trace.Geometry;

public struct Transformation
{
    public Matrix4x4 M { get; set; } = Matrix4x4.Identity;
    public Matrix4x4 InvM { get; set; } = Matrix4x4.Identity;

    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        M = m;
        InvM = invM;
    }

    public static bool are_matr_close(Matrix4x4 ma, Matrix4x4 mb)
    {
        return Math.Abs(ma.M11 - mb.M11) < 1e-5 &&
               Math.Abs(ma.M12 - mb.M12) < 1e-5 &&
               Math.Abs(ma.M13 - mb.M13) < 1e-5 &&
               Math.Abs(ma.M14 - mb.M14) < 1e-5 &&
               Math.Abs(ma.M21 - mb.M21) < 1e-5 &&
               Math.Abs(ma.M22 - mb.M22) < 1e-5 &&
               Math.Abs(ma.M23 - mb.M23) < 1e-5 &&
               Math.Abs(ma.M24 - mb.M24) < 1e-5 &&
               Math.Abs(ma.M31 - mb.M31) < 1e-5 &&
               Math.Abs(ma.M32 - mb.M32) < 1e-5 &&
               Math.Abs(ma.M33 - mb.M33) < 1e-5 &&
               Math.Abs(ma.M34 - mb.M34) < 1e-5 &&
               Math.Abs(ma.M41 - mb.M41) < 1e-5 &&
               Math.Abs(ma.M42 - mb.M42) < 1e-5 &&
               Math.Abs(ma.M43 - mb.M43) < 1e-5 &&
               Math.Abs(ma.M44 - mb.M44) < 1e-5; //seriously?? didn't find a way to access an element as "matrix[i,j]"
    }

    /// <summary>
    /// Check if the transformation passed as argument represents the same transform.
    /// </summary>
    public bool is_transf_close(Transformation t)
    {
        return are_matr_close(M, t.M) && are_matr_close(InvM, t.InvM);
    }

    public bool is_consistent()
    {
        var prod = Matrix4x4.Multiply(M, InvM);
        //return prod.Equals(Matrix4x4.Identity);
        return are_matr_close(prod, Matrix4x4.Identity);
    }

    /// <summary>
    /// Return a :class:`.Transformation` object encoding a rigid translation.
    /// The parameter `vec` specifies the amount of shift to be applied along the three axes.
    /// /// </summary>
    public Transformation translation(Vec vec)
    {
        Matrix4x4 m = new Matrix4x4
        (1.0f, 0.0f, 0.0f, vec.X,
            0.0f, 1.0f, 0.0f, vec.Y,
            0.0f, 0.0f, 1.0f, vec.Z,
            0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 invM = new Matrix4x4
        (1.0f, 0.0f, 0.0f, -vec.X,
            0.0f, 1.0f, 0.0f, -vec.Y,
            0.0f, 0.0f, 1.0f, -vec.Z,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    /// Return a :class:`.Transformation` object encoding a scaling
    /// The parameter `vec` specifies the amount of scaling along the three directions X, Y, Z.
    /// /// </summary>
    public Transformation scaling(Vec vec)
    {
        Matrix4x4 m = new Matrix4x4
        (vec.X, 0.0f, 0.0f, 0.0f,
            0.0f, vec.Y, 0.0f, 0.0f,
            0.0f, 0.0f, vec.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 invM = new Matrix4x4
        (1 / vec.X, 0.0f, 0.0f, 0.0f,
            0.0f, 1 / vec.Y, 0.0f, 0.0f,
            0.0f, 0.0f, 1 / vec.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    /// Return a :class:`.Transformation` object encoding a rotation around the X axis
    ///The parameter `angle_deg` specifies the rotation angle (in degrees).
    /// The positive sign is given by the right-hand rule.
    /// /// </summary>
    public Transformation rotation_x(float angle_deg)
    {
        float sinang = (float) Math.Sin((Math.PI / 180) * angle_deg);
        float cosang = (float) Math.Cos((Math.PI / 180) * angle_deg);
        Matrix4x4 m = new Matrix4x4
        (1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, cosang, -sinang, 0.0f,
            0.0f, sinang, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 invM = new Matrix4x4
        (1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, cosang, sinang, 0.0f,
            0.0f, -sinang, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    /// Return a :class:`.Transformation` object encoding a rotation around the Y axis
    ///The parameter `angle_deg` specifies the rotation angle (in degrees).
    /// The positive sign is given by the right-hand rule.
    /// /// </summary>
    public Transformation rotation_y(float angle_deg)
    {
        float sinang = (float) Math.Sin((Math.PI / 180) * angle_deg);
        float cosang = (float) Math.Cos((Math.PI / 180) * angle_deg);
        Matrix4x4 m = new Matrix4x4
        (cosang, 0.0f, sinang, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            -sinang, 0.0f, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 invM = new Matrix4x4
        (cosang, 0.0f, -sinang, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            sinang, 0.0f, cosang, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    /// <summary>
    /// Return a :class:`.Transformation` object encoding a rotation around the Z axis
    ///The parameter `angle_deg` specifies the rotation angle (in degrees).
    /// The positive sign is given by the right-hand rule.
    /// /// </summary>
    public Transformation rotation_z(float angle_deg)
    {
        float sinang = (float) Math.Sin((Math.PI / 180) * angle_deg);
        float cosang = (float) Math.Cos((Math.PI / 180) * angle_deg);
        Matrix4x4 m = new Matrix4x4
        (cosang, -sinang, 0.0f, 0.0f,
            sinang, cosang, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 invM = new Matrix4x4
        (cosang, sinang, 0.0f, 0.0f,
            -sinang, cosang, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return new Transformation(m, invM);
    }

    public Transformation inverse()
    {
        return new Transformation(InvM, M);
    }

    public static Transformation operator *(Transformation ta, Transformation tb)
    {
        Matrix4x4 resultM = Matrix4x4.Multiply(ta.M, tb.M);
        Matrix4x4 resultInvM = Matrix4x4.Multiply(tb.InvM, ta.InvM);
        return new Transformation(resultM, resultInvM);

        // add exception as he did in python
    }

    public static Point operator *(Transformation t, Point p)
    {
        var m = t.M;
        var resultPoint = new Point(p.X * m.M11 + p.Y * m.M12 + p.Z * m.M13 + m.M14,
            p.X * m.M21 + p.Y * m.M22 + p.Z * m.M23 + m.M24,
            p.X * m.M31 + p.Y * m.M32 + p.Z * m.M33 + m.M34);
        var w = p.X * m.M41 + p.Y * m.M42 + p.Z * m.M43 + m.M44;

        if (Math.Abs(w - 1f) < 1e-5) //he used w == 1.0, what's better?
        {
            return resultPoint;
        }
        else
        {
            return new Point(resultPoint.X / w, resultPoint.Y / w, resultPoint.Z / w);
        }
    }

    public static Vec operator *(Transformation t, Vec vec)
    {
        var m = t.M;
        var resultVec = new Vec(vec.X * m.M11 + vec.Y * m.M12 + vec.Z * m.M13,
            vec.X * m.M21 + vec.Y * m.M22 + vec.Z * m.M23,
            vec.X * m.M31 + vec.Y * m.M32 + vec.Z * m.M33);

        return resultVec;
    }

    public static Normal operator *(Transformation t, Normal n)
    {
        var invM = t.InvM;
        var resultNormal = new Normal(n.X * invM.M11 + n.Y * invM.M21 + n.Z * invM.M31,
            n.X * invM.M12 + n.Y * invM.M22 + n.Z * invM.M32,
            n.X * invM.M13 + n.Y * invM.M23 + n.Z * invM.M33);

        return resultNormal;
    }
}