using System.Numerics;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class TransformationTests
{
    [Fact]
    public void TestIsClose()
    {
        var m = new Matrix4x4(
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f);

        var invm = new Matrix4x4(
            -3.75f, 2.75f, -1.0f, 0f,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);

        var t1 = new Transformation(m, invm);

        Assert.True(t1.is_consistent());

        var t2 = new Transformation(t1.M, t1.InvM);
        Assert.True(t1.is_transf_close(t2));

        Matrix4x4 m3 = t1.M;
        m3.M33 += 1.0f; 
        var t3 = new Transformation(m3, t1.InvM);
        Assert.False(t1.is_transf_close(t3));
        
        Matrix4x4 invm4 = t1.InvM;
        invm4.M33 += 1.0f;
        var t4 = new Transformation(t1.M, invm4);
        Assert.False(t1.is_transf_close(t4));
    }

    [Fact]
    public void TestMultiplication()
    {
        var m = new Matrix4x4(
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f);

        var invm = new Matrix4x4(
            -3.75f, 2.75f, -1.0f, 0f,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);

        var t1 = new Transformation(m, invm);

        Assert.True(t1.is_consistent());

        var m2 = new Matrix4x4(
            3.0f, 5.0f, 2.0f, 4.0f,
            4.0f, 1.0f, 0.0f, 5.0f,
            6.0f, 3.0f, 2.0f, 0.0f,
            1.0f, 4.0f, 2.0f, 1.0f);

        var invm2 = new Matrix4x4(
            0.4f, -0.2f, 0.2f, -0.6f,
            2.9f, -1.7f, 0.2f, -3.1f,
            -5.55f, 3.15f, -0.4f, 6.45f,
            -0.9f, 0.7f, -0.2f, 1.1f);

        var t2 = new Transformation(m2, invm2);

        Assert.True(t2.is_consistent());

        var mExp = new Matrix4x4(
            33.0f, 32.0f, 16.0f, 18.0f,
            89.0f, 84.0f, 40.0f, 58.0f,
            118.0f, 106.0f, 48.0f, 88.0f,
            63.0f, 51.0f, 22.0f, 50.0f);
        
        var invmExp = new Matrix4x4(
            -1.45f, 1.45f, -1.0f, 0.6f,
            -13.95f, 11.95f, -6.5f, 2.6f,
            25.525f, -22.025f, 12.25f, -5.2f,
            4.825f, -4.325f, 2.5f, -1.1f);


        var tExp = new Transformation(mExp, invmExp);

        Assert.True(tExp.is_consistent());
        Assert.True(tExp.is_transf_close(t1*t2));
    }

    [Fact]
    public void TestVecPointMultiplication()
    {
        var m = new Matrix4x4(
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        
        var invm = new Matrix4x4(
            -3.75f, 2.75f, -1f, 0f,
            5.75f, -4.75f, 2.0f, 1.0f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        var t = new Transformation(m, invm);
        Assert.True(t.is_consistent());

        var expV = new Vec(14.0f, 38.0f, 51.0f);
        Assert.True(expV.is_close(t* new Vec(1f,2f,3f )));
        
        var expP = new Point(18.0f, 46.0f, 58.0f);
        Assert.True(expP.is_close(t* new Point(1.0f, 2.0f, 3.0f)));
        
        var expN = new Normal(-8.75f, 7.75f, -3.0f);
        Assert.True(expN.is_close(t* new Normal(3.0f, 2.0f, 4.0f)));
        
    }

    [Fact]
    public void TestInverse()
    {
        var m = new Matrix4x4(
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f);
        
        var invm = new Matrix4x4(
            -3.75f, 2.75f, -1f, 0.0f,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);

        var t1 = new Transformation(m, invm);
        var t2 = t1.inverse();
        Assert.True(t2.is_consistent());

        var prod = t1 * t2;
        Assert.True(prod.is_consistent());
        Assert.True(prod.is_transf_close(new Transformation()));
    }

    [Fact]
    public void TestTranslations()
    {
        var t = new Transformation();
        var tr1 = t.translation(new Vec(1.0f, 2.0f, 3.0f));
        Assert.True(tr1.is_consistent());
        
        var tr2 = t.translation(new Vec(4.0f, 6.0f, 8.0f));
        Assert.True(tr2.is_consistent());

        var prod = tr1 * tr2;
        Assert.True(prod.is_consistent());
        
        var expTr = t.translation(new Vec(5.0f, 8.0f, 11.0f));
        Assert.True(prod.is_transf_close(expTr));

    }

    [Fact]
    public void TestRotations()
    {
        var t = new Transformation();
        Assert.True(t.rotation_x(0.1f).is_consistent());
        Assert.True(t.rotation_y(0.1f).is_consistent());
        Assert.True(t.rotation_z(0.1f).is_consistent());

        var vecX = new Vec(1f, 0f, 0f);
        var vecY = new Vec(0f, 1f, 0f);
        var vecZ = new Vec(0f, 0f, 1f);
        
        Assert.True((t.rotation_x(90f) * vecY).is_close(vecZ));
        Assert.True((t.rotation_y(90f) * vecZ).is_close(vecX));
        Assert.True((t.rotation_z(90f) * vecX).is_close(vecY));
    }

    [Fact]
    public void TestScalings()
    {
        var t = new Transformation();
        var tr1 = t.scaling(new Vec(2.0f, 5.0f, 10.0f));
        Assert.True(tr1.is_consistent());
        
        var tr2 = t.scaling(new Vec(3.0f, 2.0f, 4.0f));
        Assert.True(tr2.is_consistent());
        
        var expTr = t.scaling(new Vec(6.0f, 10.0f, 40.0f));
        Assert.True(expTr.is_transf_close(tr1*tr2));
    }
    


}