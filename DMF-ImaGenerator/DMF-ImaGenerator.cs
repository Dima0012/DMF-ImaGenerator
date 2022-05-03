using CommandLine;
using CommandLine.Text;
using Trace;

namespace DMF_ImaGenerator;

internal static class DfmImaGenerator
{
    private static void Main(string[] args)
    {
        var parserSettings = new ParserSettings();
        var parser = new Parser(with => with.HelpWriter = null);
        var result = parser.ParseArguments<ParserSettings.Pfm2PngOptions, ParserSettings.DemoOptions>(args);

        Console.WriteLine();
        Console.WriteLine(HeadingInfo.Default);
        Console.WriteLine();

        result.WithParsed<ParserSettings.Pfm2PngOptions>(Pfm2Png);
        result.WithParsed<ParserSettings.DemoOptions>(Demo);

        result.WithNotParsed(errs => ParserSettings.DisplayHelp(result, errs));
        
        Console.WriteLine();
    }

    private static void Pfm2Png(ParserSettings.Pfm2PngOptions parsed)
    {
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

        // var s = $"gamma: {gamma}, factor {factor}, " + inputFile + " " + outputFile;
        // Console.WriteLine(s);
    }

    private static void Demo(ParserSettings.DemoOptions parsed)
    {
        Console.WriteLine("Hello world!");
    }
}