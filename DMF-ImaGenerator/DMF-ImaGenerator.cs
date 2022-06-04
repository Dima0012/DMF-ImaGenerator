using System.Diagnostics;
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
        var parser = new Parser();
        var result =
            parser
                .ParseArguments<ParserSettings.Pfm2PngOptions, ParserSettings.DemoOptions,
                    ParserSettings.RenderOptions>(args);

        result.WithParsed<ParserSettings.Pfm2PngOptions>(Pfm2Png);
        result.WithParsed<ParserSettings.DemoOptions>(Demo);
        result.WithParsed<ParserSettings.RenderOptions>(Render);

        result.WithNotParsed(errs => ParserSettings.DisplayHelp(result, errs));

        Console.WriteLine();
    }

    private static void Pfm2Png(ParserSettings.Pfm2PngOptions parsed)
    {
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();

        var gamma = parsed.Gamma;
        var factor = parsed.Factor;
        var luminosity = parsed.Luminosity;
        var inputFile = parsed.InputFile;
        var outputFile = parsed.OutputFile;

        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Could not find the specified file.");
            Console.WriteLine("File name: " + inputFile);
            Console.WriteLine("\nExiting application.");
            return;
        }

        using Stream fileStreamIn = File.OpenRead(inputFile);
        var img = new HdrImage(fileStreamIn);
        Console.WriteLine($"File {inputFile} has been read from disk.");

        // Adjusting image
        img.normalize_image(factor, luminosity);
        img.clamp_image();

        img.write_ldr_image(outputFile, gamma);

        Console.WriteLine($"File {outputFile} has been written to disk.");
    }

    private static void Demo(ParserSettings.DemoOptions parsed)
    {
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();

        var width = parsed.Width;
        var height = parsed.Height;
        var cam = parsed.Camera;
        var angle = parsed.Angle;
        var outputName = parsed.OutputName;
        var algorithm = parsed.Algorithm;
        var factor = parsed.Factor;
        var gamma = parsed.Gamma;
        var luminosity = parsed.Luminosity;
        var pixelSamples = parsed.PixelSamples;
        var initState = parsed.InitState;
        var initSeq = parsed.InitSequence;
        var numberOfRays = parsed.NumberOfRays;
        var maxDepth = parsed.MaxDepth;
        var rouletteLimit = parsed.RussianRouletteLimit;
        var imageResolution = parsed.ImageResolution;

        var algorithms = new List<string> {"flat", "path-tracer", "point-light"};
        var resolutions = new List<string> {"SD", "HD", "FHD"};

        if (!algorithms.Contains(algorithm, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Unknown render option '{algorithm}'. Choose between: ");
            Console.WriteLine(string.Join("\n", algorithms));
            Console.WriteLine("\nExiting application.");
            return;
        }

        // This line makes the render name all lower-case, if user used upper-case
        algorithm = algorithms.Find(x => algorithm.Equals(x, StringComparison.OrdinalIgnoreCase));

        if (!resolutions.Contains(imageResolution, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Unknown resolution '{imageResolution}'. Choose between: ");
            Console.WriteLine("SD (720x480)\nHD (1280x720)\nFHD (1920x1080)");
            Console.WriteLine("\nExiting application.");
            return;
        }

        switch (imageResolution)
        {
            case "SD":
                width = 720;
                height = 480;
                break;

            case "HD":
                width = 1280;
                height = 720;
                break;

            case "FHD":
                width = 1920;
                height = 1080;
                break;
        }

        var samplesPerSide = MathF.Sqrt(pixelSamples);
        if (Math.Abs(samplesPerSide * samplesPerSide - pixelSamples) > 10 - 4)
        {
            Console.WriteLine(
                $"Error: samples per pixels {pixelSamples} to use in anti-aliasing is not a perfect square.");
            Console.WriteLine("\nExiting application.");
            return;
        }

        // Create scene and define camera transformation
        var world = new World();
        ICamera camera;
        Transformation camTransformation;
        string s;

        // Complex algorithm, complex scene
        if (algorithm is "path-tracer" or "point-light")
        {
            var skyMaterial = new Material(new DiffuseBrdf(new UniformPigment(new Color(0, 0, 0))),
                new UniformPigment(new Color(1.0f, 0.9f, 0.5f)));

            var groundMaterial = new Material(new DiffuseBrdf(new CheckeredPigment(
                new Color(0.3f, 0.5f, 0.1f),
                new Color(0.1f, 0.2f, 0.5f))));

            var sphereMaterial = new Material(new DiffuseBrdf(new UniformPigment(new Color(0.3f, 0.4f, 0.8f))));
            var mirrorMaterial = new Material(new SpecularBrdf(new UniformPigment(new Color(0.6f, 0.2f, 0.3f))));

            world.add(new Sphere(Transformation.scaling(new Vec(200, 200, 200))
                                 * Transformation.translation(new Vec(0, 0, 0.4f)),
                skyMaterial));
            world.add(new Plane(groundMaterial));
            world.add(new Sphere(Transformation.translation(new Vec(0, 0, 1)), sphereMaterial));
            world.add(new Sphere(Transformation.translation(new Vec(1, 2.5f, 0)), mirrorMaterial));

            camTransformation = Transformation.rotation_z(angle) * Transformation.translation(new Vec(-1.0f, 0, 1));
        }
        else // Algorithm is flat, simple scene
        {
            const float scalingFactor = 1 / 10f;
            var scaling = Transformation.scaling(new Vec(scalingFactor, scalingFactor, scalingFactor));
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
            var transl = Transformation.translation(new Vec(0f, 0.5f, 0f));
            var transf = transl * scaling;
            var materiaf1 = new Material(new DiffuseBrdf(new UniformPigment()));
            var spheref1 = new Sphere(transf, materiaf1);
            world.add(spheref1);

            transl = Transformation.translation(new Vec(0f, 0f, -0.5f));
            transf = transl * scaling;

            var green = new Color(0.3f, 0.9f, 0.5f);
            var yellow = new Color(0.9f, 0.9f, 0.4f);

            var materialf2 = new Material(new DiffuseBrdf(new CheckeredPigment(green, yellow, 2)));
            var spheref2 = new Sphere(transf, materialf2);
            world.add(spheref2);

            camTransformation = Transformation.rotation_z(angle) * Transformation.translation(new Vec(-2.0f, 0, 0));
        }

        // Choose camera
        if (!cam)
        {
            camera = new PerspectiveCamera((float) width / height, camTransformation);
            s = "perspective";
        }
        else
        {
            camera = new OrthogonalCamera((float) width / height, camTransformation);
            s = "orthogonal";
        }

        var img = new HdrImage(width, height);
        var imageTracer = new ImageTracer(img, camera, (int) samplesPerSide);
        var stopWatch = new Stopwatch();

        // Choose algorithm and render
        Console.WriteLine($"Rendering demo image with {algorithm} renderer.");
        Console.WriteLine($"Using a {s} camera.");
        Console.WriteLine($"Resolution is {width}x{height}\n");

        switch (algorithm)
        {
            case "path-tracer":
            {
                var renderer = new PathTracer(world, new Pcg(initState, initSeq), numberOfRays, maxDepth,
                    rouletteLimit);
                // Trace image with path-trace method
                stopWatch.Start();
                imageTracer.fire_all_rays(renderer);
                stopWatch.Stop();
                break;
            }
            case "point-light":
            {
                world.add_light(new PointLight(new Point(-30, 30, 30), new Color(1, 1, 1)));

                var renderer = new PointLightRenderer(world);
                // Trace image with point-light method
                stopWatch.Start();
                imageTracer.fire_all_rays(renderer);
                stopWatch.Stop();
                break;
            }
            default:
            {
                var renderer = new FlatRenderer(world);
                // Trace image with flat method
                stopWatch.Start();
                imageTracer.fire_all_rays(renderer);
                stopWatch.Stop();
                break;
            }
        }

        var ts = stopWatch.Elapsed;
        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

        // Save image in PFM format
        var pfm = outputName;
        var png = outputName;

        if (outputName == "default")
        {
            pfm = s + "_demo.pfm";
        }

        Console.WriteLine($"Rendering of {pfm} complete.");
        Console.WriteLine("Time elapsed: " + elapsedTime);

        using Stream outputFilePfm = File.OpenWrite(pfm);
        imageTracer.Image.write_pfm(outputFilePfm, -1.0);
        Console.WriteLine("\nFile " + pfm + " has been written to disk.");

        // Save image in PNG format

        // Adjusting image
        imageTracer.Image.normalize_image(factor, luminosity);
        imageTracer.Image.clamp_image();

        if (png == "default")
        {
            png = s + "_demo.png";
        }

        imageTracer.Image.write_ldr_image(png, gamma);
        Console.WriteLine("File " + png + " has been written to disk.");
        Console.WriteLine("\nExiting application.");
    }

    private static void Render(ParserSettings.RenderOptions parsed)
    {
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();

        var width = parsed.Width;
        var height = parsed.Height;
        var angle = parsed.Angle;
        var outputName = parsed.OutputName;
        var algorithm = parsed.Algorithm;
        var factor = parsed.Factor;
        var gamma = parsed.Gamma;
        var luminosity = parsed.Luminosity;
        var pixelSamples = parsed.PixelSamples;
        var initState = parsed.InitState;
        var initSeq = parsed.InitSequence;
        var numberOfRays = parsed.NumberOfRays;
        var maxDepth = parsed.MaxDepth;
        var rouletteLimit = parsed.RussianRouletteLimit;
        var imageResolution = parsed.ImageResolution;
        var sceneFile = parsed.SceneFile;

        var algorithms = new List<string> {"path-tracer", "point-light"};
        var resolutions = new List<string> {"SD", "HD", "FHD"};

        if (!algorithms.Contains(algorithm, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Unknown render option '{algorithm}'. Choose between: ");
            Console.WriteLine(string.Join("\n", algorithms));
            Console.WriteLine("\nExiting application.");
            return;
        }

        // This line makes the render name all lower-case, if user used upper-case
        algorithm = algorithms.Find(x => algorithm.Equals(x, StringComparison.OrdinalIgnoreCase));

        if (!resolutions.Contains(imageResolution, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Unknown resolution '{imageResolution}'. Choose between: ");
            Console.WriteLine("SD (720x480)\nHD (1280x720)\nFHD (1920x1080)");
            Console.WriteLine("\nExiting application.");
            return;
        }

        switch (imageResolution)
        {
            case "SD":
                width = 720;
                height = 480;
                break;

            case "HD":
                width = 1280;
                height = 720;
                break;

            case "FHD":
                width = 1920;
                height = 1080;
                break;
        }

        var samplesPerSide = MathF.Sqrt(pixelSamples);
        if (Math.Abs(samplesPerSide * samplesPerSide - pixelSamples) > 10 - 4)
        {
            Console.WriteLine(
                $"Error: samples per pixels {pixelSamples} to use in anti-aliasing is not a perfect square.");
            Console.WriteLine("\nExiting application.");
            return;
        }

        // Read scene from file
        if (!File.Exists(sceneFile))
        {
            Console.WriteLine("Could not find the specified file.");
            Console.WriteLine("File name: " + sceneFile);
            Console.WriteLine("\nExiting application.");
            return;
        }

        using Stream sceneStream = File.OpenRead(sceneFile);
        var img = new HdrImage(sceneStream);
        Console.WriteLine($"File {sceneFile} has been read from disk.");

        Scene scene;
        var inputStream = new InputStream(sceneStream, sceneFile);


        Console.WriteLine("Processing the scene ... ");

        try
        {
            scene = inputStream.parse_scene();
        }
        catch (GrammarError e)
        {
            var loc = e.Location;
            Console.WriteLine($"\nError in file: {loc.FileName} at line {loc.LineNum}:{loc.ColNum}: \n {e.Message}");
            Console.WriteLine("\nExiting application.");
            return;
        }

        Console.WriteLine("Scene processed successfully.\n");

        // Model scene 
        var world = scene.World;
        var camera = scene.Camera;
        var camName = nameof(camera);
        var aspectRatio = (float) width / height;

        // Check if camera is defined
        if (camera is null)
        {
            Console.WriteLine(
                "Warning: the camera has not been defined in the scene. Using a default perspective camera centered in [-5,0,0]\n");
            var camTransformation = Transformation.rotation_z(angle) * Transformation.translation(new Vec(-5.0f, 0, 0));
            camera = new PerspectiveCamera(aspectRatio, camTransformation);
        }

        var image = new HdrImage(width, height);
        var imageTracer = new ImageTracer(image, camera, (int) samplesPerSide);
        var stopWatch = new Stopwatch();


        // Choose algorithm and render
        Console.WriteLine($"Rendering demo image with {algorithm} renderer.");
        Console.WriteLine($"Using a {camName}.");
        Console.WriteLine($"Resolution is {width}x{height}\n");

        switch (algorithm)
        {
            case "path-tracer":
            {
                var renderer = new PathTracer(world, new Pcg(initState, initSeq), numberOfRays, maxDepth,
                    rouletteLimit);
                // Trace image with path-trace method
                stopWatch.Start();
                imageTracer.fire_all_rays(renderer);
                stopWatch.Stop();
                break;
            }
            case "point-light":
            {
                var renderer = new PointLightRenderer(world);
                // Trace image with point-light method
                stopWatch.Start();
                imageTracer.fire_all_rays(renderer);
                stopWatch.Stop();
                break;
            }
        }
        
        var ts = stopWatch.Elapsed;
        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

        // Save image in PFM format
        var pfm = outputName;
        var png = outputName;

        if (outputName == "")
        {
            pfm = camName + "_image.pfm";
        }

        Console.WriteLine($"Rendering of {pfm} complete.");
        Console.WriteLine("Time elapsed: " + elapsedTime);

        using Stream outputFilePfm = File.OpenWrite(pfm);
        imageTracer.Image.write_pfm(outputFilePfm, -1.0);
        Console.WriteLine("\nFile " + pfm + " has been written to disk.");

        // Save image in PNG format

        // Adjusting image
        imageTracer.Image.normalize_image(factor, luminosity);
        imageTracer.Image.clamp_image();

        if (png == "")
        {
            png = camName + "_demo.png";
        }

        imageTracer.Image.write_ldr_image(png, gamma);
        Console.WriteLine("File " + png + " has been written to disk.");
        Console.WriteLine("\nExiting application.");
    }
}