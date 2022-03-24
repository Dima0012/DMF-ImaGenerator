// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp.Memory;
using Trace;

Console.WriteLine("Hello, world!");

static void Main(string[] args)
{
    Parameters parameters = new Parameters();
    try
    {
        parameters = new Parameters(args);
    }
    catch (RuntimeError err)
    {
        Console.WriteLine($"Error: {err}");
        throw;
    }
    
    using (Stream fileStreamIn = File.OpenRead(parameters.InputPfmFilename))
    {
        // this constructor is missing atm
       // HdrImage img = new HdrImage(fileStreamIn);
    }

    Console.WriteLine($"File {parameters.InputPfmFilename} has been read from disk.");
    
    HdrImage img = new HdrImage(2,1);//just to continue, must be removed later
    img.normalize_image(parameters.Factor);
    img.clamp_image();

    img.write_ldr_image(parameters.OutputPngFilename, parameters.Gamma);
    
    Console.WriteLine($"File {parameters.OutputPngFilename} has been written to disk.");

    }
