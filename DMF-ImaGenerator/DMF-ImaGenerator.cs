using CommandLine;
using CommandLine.Text;
using Trace;
using Trace.Cameras;
using Trace.Geometry;

namespace DMF_ImaGenerator;

internal static class DfmImaGenerator
{
    private static void Main(string[] args)
    {
        Console.WriteLine();
        
        var parserSettings = new ParserSettings();
        var parser = new Parser(with => with.HelpWriter = null);
        var result = parser.ParseArguments<ParserSettings.Pfm2PngOptions, ParserSettings.DemoOptions>(args);

        result.WithParsed<ParserSettings.Pfm2PngOptions>(Pfm2Png);
        result.WithParsed<ParserSettings.DemoOptions>(Demo);

        result.WithNotParsed(errs => ParserSettings.DisplayHelp(result, errs));

        Console.WriteLine();
    }

    private static void Pfm2Png(ParserSettings.Pfm2PngOptions parsed)
    {
        Console.WriteLine();
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();
        
        var gamma = parsed.Gamma;
        var factor = parsed.Factor;
        var inputFile = parsed.InputFile;
        var outputFile = parsed.OutputFile;

        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Could not find the specified file.");
            Console.WriteLine("File name: " + inputFile);
            Console.WriteLine();
            Console.WriteLine("Exiting application.");
            Console.WriteLine();
            return;
        }

        using Stream fileStreamIn = File.OpenRead(inputFile);
        var img = new HdrImage(fileStreamIn);
        Console.WriteLine($"File {inputFile} has been read from disk.");

        // Adjusting image
        img.normalize_image(factor);
        img.clamp_image();

        img.write_ldr_image(outputFile, gamma);

        Console.WriteLine($"File {outputFile} has been written to disk.");
    }

    private static void Demo(ParserSettings.DemoOptions parsed)
    {
        Console.WriteLine();
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();
        
        var width = parsed.Width;
        var height = parsed.Height;
        var cam = parsed.Camera;
        var angle = parsed.Angle;
        var name = parsed.Name;
        var algorithm = parsed.Algorithm;

        List<string> algs = new List<string> {"onoff", "flat"};

        if (!algs.Contains(algorithm))
        {
            Console.WriteLine("Unknown render option. Choose between: ");
            Console.WriteLine("onoff\n"+"flat\n");
            Console.WriteLine("Exiting application.");
            return;
        }

        var world = new World();

        var scalingFactor = 1 / 10f;
        var scaling = Transformation.scaling(new Vec(scalingFactor,scalingFactor,scalingFactor));
        var material = new Material(new DiffuseBrdf(new UniformPigment(new Color(1, 0, 0))));

        // Add sphere to cube vertexes
        for (var x = 0.5f; x >= -0.5f; x -= 1.0f)
        {
            for (var y = 0.5f; y >= -0.5f; y -= 1.0f)
            {
                for (var z = 0.5f; z >= -0.5f; z -= 1.0f)
                {
                    var translation = Transformation.translation(new Vec(x, y, z));
                    var transformation = translation * scaling;

                    var sphere = new Sphere(transformation, material);

                    world.add(sphere);
                }
            }
        }
        
        // Add sphere to cube faces
        var transl = Transformation.translation(new Vec(0f, 0.5f , 0f));
        var transf = transl * scaling;
        var materiaf1 = new Material(new DiffuseBrdf(new UniformPigment()));
        var spheref1 = new Sphere(transf, materiaf1);
        world.add(spheref1);
        
        transl = Transformation.translation(new Vec(0f, 0f , -0.5f));
        transf = transl * scaling;

        var green = new Color(0.3f, 0.9f, 0.5f);
        var yellow = new Color(0.9f, 0.9f, 0.4f);
        
        var materialf2 = new Material(new DiffuseBrdf(new CheckeredPigment(green, yellow,2)));
        var spheref2 = new Sphere(transf, materialf2);
        world.add(spheref2);

        // Define camera with rotation angle
        var camTransformation = Transformation.rotation_z(angle)*Transformation.translation(new Vec(-2.0f, 0,0));
        ICamera camera;
        string s;
        
        if (!cam)
        {
            camera = new PerspectiveCamera(1.0f, (float) width/height, camTransformation);
            s = "perspective";
        }
        else
        {
            camera = new OrthogonalCamera((float) width/height, camTransformation);
            s = "orthogonal";
        }

        var img = new HdrImage(width, height);
        var imageTracer = new ImageTracer(img, camera);

        switch (algorithm)
        {
            case "onoff":
            {
                Console.WriteLine($"Rendering demo image with {algorithm} renderer.\n");
                var renderer = new OnOffRenderer(world);
                // Trace image with flat method
                imageTracer.fire_all_rays(renderer);
                break;
            }
            case "flat":
            {
                Console.WriteLine($"Rendering demo image with {algorithm} renderer.\n");
                var renderer = new FlatRenderer(world);
                // Trace image with on-off method
                imageTracer.fire_all_rays(renderer);
                break;
            }
        }

        // Save image in PFM format
        using Stream outputFilePfm = File.OpenWrite(s+"_demo.pfm");
        imageTracer.Image.write_pfm(outputFilePfm, -1.0);
        
        // Save image in PNG format

        // Adjusting image
        imageTracer.Image.normalize_image(0.18f, 0.5f);
        imageTracer.Image.clamp_image();

        if (name == "default")
        {
            name = s + "_demo.png";
            Console.WriteLine("File "+name+" has been written to disk.");
        }
        
        imageTracer.Image.write_ldr_image(name, 1.0f);
        
    }
}