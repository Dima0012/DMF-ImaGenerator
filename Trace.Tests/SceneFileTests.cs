using System.IO;
using Xunit;
using System.Text;
using System;
using System.Security.Cryptography;
using Trace.Cameras;
using Trace.Geometry;


namespace Trace.Tests;

public class SceneFileTests
{
    [Fact]
    public void TestInputFile()
    {
        var buf = Encoding.ASCII.GetBytes("abc   \nd\nef");
        using var memStream = new MemoryStream(buf);
        var stream = new InputStream(memStream);

        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 1);

        Assert.True(stream.read_char() == 'a');
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 2);

        stream.unread_char('A');
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 1);

        Assert.True(stream.read_char() == 'A');
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 2);

        Assert.True(stream.read_char() == 'b');
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 3);

        Assert.True(stream.read_char() == 'c');
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColNum == 4);

        stream.skip_whitespaces_and_comments();

        Assert.True(stream.read_char() == 'd');
        Assert.True(stream.Location.LineNum == 2);
        Assert.True(stream.Location.ColNum == 2);

        Assert.True(stream.read_char() == '\n');
        Assert.True(stream.Location.LineNum == 3);
        Assert.True(stream.Location.ColNum == 1);

        Assert.True(stream.read_char() == 'e');
        Assert.True(stream.Location.LineNum == 3);
        Assert.True(stream.Location.ColNum == 2);

        Assert.True(stream.read_char() == 'f');
        Assert.True(stream.Location.LineNum == 3);
        Assert.True(stream.Location.ColNum == 3);

        Assert.True(stream.read_char() == null);
    }

    [Fact]
    public void TestLexer()
    {
        var buf = Encoding.ASCII.GetBytes(
            "# This is a comment" +
            "# This is another comment" +
            "\nnew material sky_material(" +
            "diffuse(image(\"my file.pfm\"))," +
            "<5.0, 500.0, 300.0>" +
            ") # Comment at the end of the line"
        );
        using var memStream = new MemoryStream(buf);
        var stream = new InputStream(memStream);

        
        Assert.True(stream.ReadToken().Keyword == KeywordEnum.New);
        Assert.True(stream.ReadToken().Keyword == KeywordEnum.Material);
        Assert.True(stream.ReadToken().Identifier == "sky_material");
        Assert.True(stream.ReadToken().Symbol == '(');
        Assert.True(stream.ReadToken().Keyword == KeywordEnum.Diffuse);
        Assert.True(stream.ReadToken().Symbol == '(');
        Assert.True(stream.ReadToken().Keyword == KeywordEnum.Image);
        Assert.True(stream.ReadToken().Symbol == '(');
        Assert.True(stream.ReadToken().String == "my file.pfm");
        Assert.True(stream.ReadToken().Symbol == ')');
    }

    [Fact]
    public void TestParser()
    {
        var buf = Encoding.ASCII.GetBytes(
            "float clock(150)" + 
            "material sky_material(diffuse(uniform(<0, 0, 0>)), " +
                                    "uniform(<0.7, 0.5, 1>))\n" +
            "# Here is a comment\n" + 
            "material ground_material(diffuse(checkered(<0.3, 0.5, 0.1>,<0.1, 0.2, 0.5>, 4)), \n" +
                                    "uniform(<0, 0, 0>))" +
    
            "material sphere_material(specular(uniform(<0.5, 0.5, 0.5>)), \n" +
                                    "uniform(<0, 0, 0>))" +
            
            "plane (sky_material, translation([0, 0, 100]) * rotation_y(clock))\n" +
        
            "plane (ground_material, identity)\n" +
    
            "sphere(sphere_material, translation([0, 0, 1]))\n" +
    
            "camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)"
        );
        using var memStream = new MemoryStream(buf);
        var stream = new InputStream(memStream);

        var scene = stream.parse_scene();
        
        //Check that the float variables are ok

        Assert.True(scene.FloatVariables.Count == 1);
        Assert.True(scene.FloatVariables.ContainsKey("clock"));
        Assert.True(scene.FloatVariables["clock"] == 150.0);
        
        //Check that the materials are ok
        Assert.True(scene.Materials.Count == 3);
        Assert.True(scene.Materials.ContainsKey("sphere_material"));
        Assert.True(scene.Materials.ContainsKey("sky_material"));
        Assert.True(scene.Materials.ContainsKey("ground_material"));

        var sphere_material = scene.Materials["sphere_material"];
        var sky_material = scene.Materials["sky_material"];
        var ground_material = scene.Materials["ground_material"];
        
        Assert.True(sky_material.Brdf.GetType() == typeof(DiffuseBrdf));
        Assert.True(sky_material.Brdf.Pigment.GetType() == typeof(UniformPigment));
        Assert.True(sky_material.Brdf.Pigment.Color.is_close(new Color(0, 0, 0)));
        
        Assert.True(ground_material.Brdf.GetType() == typeof(DiffuseBrdf));
        Assert.True(ground_material.Brdf.Pigment.GetType() == typeof(CheckeredPigment));
        Assert.True(ground_material.Brdf.Pigment.Color1.is_close(new Color(0.3f, 0.5f, 0.1f)));
        Assert.True(ground_material.Brdf.Pigment.Color2.is_close(new Color(0.1f, 0.2f, 0.5f)));
        Assert.True(ground_material.Brdf.Pigment.NumOfSteps == 4);
        
        Assert.True(sphere_material.Brdf.GetType() == typeof(SpecularBrdf));
        Assert.True(sphere_material.Brdf.Pigment.GetType() == typeof(UniformPigment));
        Assert.True(sphere_material.Brdf.Pigment.Color.is_close(new Color(0.5f, 0.5f, 0.5f)));
        
        Assert.True(sky_material.EmittedRadiance.GetType() == typeof(UniformPigment));
        Assert.True(sky_material.EmittedRadiance.Color.is_close(new Color(0.7f, 0.5f, 1.0f)));
        Assert.True(ground_material.EmittedRadiance.GetType() == typeof(UniformPigment));
        Assert.True(ground_material.EmittedRadiance.Color.is_close(new Color(0, 0, 0)));
        Assert.True(sphere_material.EmittedRadiance.GetType() == typeof(UniformPigment));
        Assert.True(sphere_material.EmittedRadiance.Color.is_close(new Color(0, 0, 0)));
        
        //Check that the shapes are ok

        Assert.True(scene.World.Shapes.Count == 3);
        Assert.True(scene.World.Shapes[0].GetType() == typeof(Plane));
        Assert.True(scene.World.Shapes[0].Transformation
            .is_close(Transformation.translation(new Vec(0, 0, 100)) * Transformation.rotation_y(150.0f)));
        Assert.True(scene.World.Shapes[1].GetType() == typeof(Plane));
        Assert.True(scene.World.Shapes[1].Transformation.is_close(new Transformation()));
        Assert.True(scene.World.Shapes[2].GetType() == typeof(Sphere));
        Assert.True(scene.World.Shapes[2].Transformation.is_close(Transformation.translation(new Vec(0, 0, 1))));
        
        //Check that the camera is ok

        Assert.True(scene.Camera.GetType() == typeof(PerspectiveCamera));
        Assert.True(scene.Camera.Transformation.is_close(Transformation.rotation_z(30) * Transformation.translation(new Vec(-4, 0, 1))));
        Assert.True(Math.Abs(1.0 - scene.Camera.AspectRatio) < 1e-5); 
        Assert.True(Math.Abs(2.0 - scene.Camera.Distance) < 1e-5);
    }

    [Fact]
    //Check that unknown materials raises a GrammarError
    public void TestParserUndefinedMaterial()
    {
        var buf = Encoding.ASCII.GetBytes(
            "plane(this_material_does_not_exist, identity)");
        using var memStream = new MemoryStream(buf);
        var stream = new InputStream(memStream);
        try
        {
            var scene = stream.parse_scene();
            Assert.True(1==0, "the code did not throw an exception");
        }
        catch (GrammarError ex)
        {}
    }
    
    [Fact]
    //Check that defining two cameras in the same file raises a GrammarError
    public void TestParserDoubleCamera()
    {
        var buf = Encoding.ASCII.GetBytes(
            "camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)\n" +
            "camera(orthogonal, identity, 1.0, 1.0)");
        using var memStream = new MemoryStream(buf);
        var stream = new InputStream(memStream);

        try
        {
            var scene = stream.parse_scene();
            Assert.True(1==0, "the code did not throw an exception");
        }
        catch (GrammarError ex)
        {}
    }
    
    
}