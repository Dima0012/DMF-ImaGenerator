using DMF_ImaGenerator;
using Trace;

// Parameters parameters;
// try
// {
//     parameters = new Parameters(args);
// }
// catch (RuntimeError err)
// {
//     Console.WriteLine($"Error: {err}");
//     return;
// }
//
// using Stream fileStreamIn = File.OpenRead(parameters.InputPfmFilename);
// var img = new HdrImage(fileStreamIn);
// Console.WriteLine($"File {parameters.InputPfmFilename} has been read from disk.");
//
// // Adjusting image
// img.normalize_image(parameters.Factor);
// img.clamp_image();
//
// img.write_ldr_image(parameters.OutputPngFilename, parameters.Gamma);
//
// Console.WriteLine($"File {parameters.OutputPngFilename} has been written to disk.");
using CommandLine;

internal static class DfmImaGenerator
{
    private static void Main(string[] args)
    {
        // (1) default options
        var result = Parser.Default.ParseArguments<Pfm2PngOptions, DemoOptions>(args);

        // or (2) build and configure instance
        // var parser = new Parser(with => with.EnableDashDash = true);
        // var result = parser.ParseArguments<Options>(args);

        result.WithParsed<Pfm2PngOptions>(Pfm2Png);
        result.WithParsed<DemoOptions>(Demo);
    }

    private static void Pfm2Png(Pfm2PngOptions parsed)
    {
        var gamma = parsed.Gamma;
        var factor = parsed.Factor;
        var inputFile = parsed.FileNames!.ElementAt(0);
        var outputFile = parsed.FileNames!.ElementAt(1);

        // var s = $"gamma: {gamma}, factor {factor}, " + inputFile + " " + outputFile;
        // Console.WriteLine(s);
        

        using Stream fileStreamIn = File.OpenRead(inputFile);
        var img = new HdrImage(fileStreamIn);
        Console.WriteLine($"File {inputFile} has been read from disk.");

        // Adjusting image
        img.normalize_image(factor);
        img.clamp_image();

        img.write_ldr_image(outputFile, gamma);

        Console.WriteLine($"File {outputFile} has been written to disk.");
    }

    private static void Demo(DemoOptions parsed)
    {
        Console.WriteLine("Hello world!");
    }
}

[Verb("pfm2png", HelpText = "Convert a file from PFM format to PNG format.")]
internal class Pfm2PngOptions
{
    [Option('g', "gamma", Default = 1.0f, Separator = ' ',
        HelpText = "The gamma correction to apply in the image conversion.")]
    public float Gamma { get; set; }

    [Option('f', "factor", Default = 0.18f, Separator = ' ',
        HelpText = "The factor for the luminosity normalization of the image.")]
    public float Factor { get; set; }

    [Value(0, Min = 1, Max = 2)] public IEnumerable<string>? FileNames { get; set; }
}

[Verb("demo", HelpText = "Generates a demo image")]
public class DemoOptions
{
}