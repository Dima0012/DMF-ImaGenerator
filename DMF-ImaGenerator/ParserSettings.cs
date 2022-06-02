namespace DMF_ImaGenerator;

using CommandLine;
using CommandLine.Text;

internal class ParserSettings
{
    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        HelpText helpText;
        if (errs.IsVersion()) //check if error is version request
            helpText = HelpText.AutoBuild(result);
        else
        {
            helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = true;
                // h.Heading = "DMF-ImaGenerator 0.2.0"; //change header
                // h.Copyright = "Copyright (c) 2022 DMF"; //change copyright text
                h.AutoVersion = false;
                h.AutoHelp = true;

                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e, false);
            Console.WriteLine(helpText);
        }
    }

    [Verb("demo-old", HelpText = "Debug demo command")]
    public class DemoOldOptions
    {
        [Option('w', "width", MetaValue = "INT", Default = 480, Separator = ' ',
            HelpText = "The width of the image in pixels.")]
        public int Width { get; set; }

        [Option('h', "height", MetaValue = "INT", Default = 480, Separator = ' ',
            HelpText = "The height of the image in pixels.")]
        public int Height { get; set; }

        [Option('o', "orthogonal", Default = false,
            HelpText = "Renders the image using an Orthogonal Camera instead of a Perspective Camera.")]
        public bool Camera { get; set; }

        [Option('d', "angle-deg", Default = 0f, Separator = ' ',
            HelpText = "Rotation angle for the camera / observer")]
        public float Angle { get; set; }

        [Option('p', "pfm-output", Default = "default", Separator = ' ',
            HelpText = "Name of the output PFM file.")]
        public string PfmName { get; set; } = null!;

        [Option('n', "png-output", Default = "default", Separator = ' ',
            HelpText = "Name of the output PNG file.")]
        public string PngName { get; set; } = null!;

        [Option('a', "algorithm", Default = "on-off", Separator = ' ',
            HelpText = "Algorithm used to render the image. Choose between 'on-off' and 'flat'")]
        public string Algorithm { get; set; } = null!;
        
        [Option('g', "gamma", MetaValue = "FLOAT", Default = 1.0f, Separator = ' ',
            HelpText = "The gamma correction to apply in the image conversion.")]
        public float Gamma { get; set; }

        [Option('f', "factor", MetaValue = "FLOAT", Default = 0.18f, Separator = ' ',
            HelpText = "The factor for the luminosity normalization of the image.")]
        public float Factor { get; set; }
        
        [Option('l', "luminosity", MetaValue = "FLOAT", Default = null, Separator = ' ',
            HelpText = "Luminosity value for tone mapping.")]
        public float? Luminosity { get; set; }

        [Option('s', "samples-per-pixels", MetaValue = "INT", Default = 0, Separator = ' ',
            HelpText = "number of samples per pixel for anti-aliasing. Must be a perfect square (eg. 16).")]
        public int PixelSamples { get; set; }
    }

    [Verb("demo", HelpText = "Generates a full realistic demo image, and saves it in PFM and PNG format.")]
    internal class DemoOptions
    {
        [Option('w', "width", MetaValue = "INT", Default = 480, Separator = ' ',
            HelpText = "The width of the image in pixels.")]
        public int Width { get; set; }

        [Option('h', "height", MetaValue = "INT", Default = 480, Separator = ' ',
            HelpText = "The height of the image in pixels.")]
        public int Height { get; set; }

        [Option('o', "orthogonal", Default = false,
            HelpText = "Renders the image using an Orthogonal Camera instead of a Perspective Camera.")]
        public bool Camera { get; set; }

        [Option('d', "angle-deg", Default = 0f, Separator = ' ',
            HelpText = "Rotation angle for the camera / observer")]
        public float Angle { get; set; }

        [Option('p', "pfm-output", Default = "default", Separator = ' ',
            HelpText = "Name of the output PFM file.")]
        public string PfmName { get; set; } = null!;

        [Option('n', "png-output", Default = "default", Separator = ' ',
            HelpText = "Name of the output PNG file.")]
        public string PngName { get; set; } = null!;

        [Option('a', "algorithm", Default = "path-tracer", Separator = ' ',
            HelpText = "Algorithm used to render the image. Choose between 'path-tracer' and 'point-light'.")]
        public string Algorithm { get; set; } = null!;
        
        [Option('g', "gamma", MetaValue = "FLOAT", Default = 1.0f, Separator = ' ',
            HelpText = "The gamma correction to apply in the image conversion.")]
        public float Gamma { get; set; }

        [Option('f', "factor", MetaValue = "FLOAT", Default = 0.18f, Separator = ' ',
            HelpText = "The factor for the luminosity normalization of the image.")]
        public float Factor { get; set; }
        
        [Option('l', "luminosity", MetaValue = "FLOAT", Default = null, Separator = ' ',
            HelpText = "Luminosity value for tone mapping.")]
        public float? Luminosity { get; set; }

        [Option('s', "samples-per-pixels", MetaValue = "INT", Default = 0, Separator = ' ',
            HelpText = "Number of samples per pixel for anti-aliasing. Must be a perfect square (eg. 16).")]
        public int PixelSamples { get; set; }
        
        [Option('i', "init-state", MetaValue = "INT", Default = 42UL,Separator = ' ',
            HelpText = "Initial state for the PCG.")]
        public ulong InitState { get; set; }
        
        [Option('q', "init-seq", MetaValue = "INT", Default = 54UL, Separator = ' ',
            HelpText = "Initial sequence for the PCG.")]
        public ulong InitSequence { get; set; }
        
        [Option('r', "num-of-rays", MetaValue = "INT", Default  = 10, Separator = ' ',
            HelpText = "Numbers of rays scattered in path tracer algorithm.")]
        public int NumberOfRays { get; set; }
        
        [Option('m', "max-depth", MetaValue = "INT", Default = 2, Separator = ' ',
            HelpText = "The max travelling depth distance for the rays.")]
        public int MaxDepth { get; set; }
        
        [Option('u', "roulette-limit", MetaValue = "INT", Default = 3, Separator = ' ',
            HelpText = "The minimum numbers of iterations before applying Russian roulette selection.")]
        public int RussianRouletteLimit { get; set; }
    }

    [Verb("pfm2png", HelpText = "Convert a file from PFM format to PNG format.")]
    internal class Pfm2PngOptions
    {
        [Value(0, MetaName = "PFM", HelpText = "The input file name, path based. Must be a PFM file.", Required = true)]
        public string InputFile { get; set; } = null!;

        [Value(1, MetaName = "PNG", HelpText = "The output file name, path based.", Default = "output_image.png")]
        public string OutputFile { get; set; } = null!;

        [Option('g', "gamma", MetaValue = "FLOAT", Default = 1.0f, Separator = ' ',
            HelpText = "The gamma correction to apply in the image conversion.")]
        public float Gamma { get; set; }

        [Option('f', "factor", MetaValue = "FLOAT", Default = 0.18f, Separator = ' ',
            HelpText = "The factor for the luminosity normalization of the image.")]
        public float Factor { get; set; }

        [Option('l', "luminosity", MetaValue = "FLOAT", Default = null, Separator = ' ',
            HelpText = "Luminosity value for tone mapping.")]
        public float? Luminosity { get; set; }
    }
}