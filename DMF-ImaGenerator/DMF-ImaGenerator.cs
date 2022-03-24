using Trace;

static void Main(string[] args)
{
    Parameters parameters;
    try
    {
        parameters = new Parameters(args);
    }
    catch (RuntimeError err)
    {
        Console.WriteLine($"Error: {err}");
        throw;
    }

    using Stream fileStreamIn = File.OpenRead(parameters.InputPfmFilename);
    var img = new HdrImage(fileStreamIn);
    Console.WriteLine($"File {parameters.InputPfmFilename} has been read from disk.");
    
    // Adjusting image
    img.normalize_image(parameters.Factor);
    img.clamp_image();
    
    img.write_ldr_image(parameters.OutputPngFilename, parameters.Gamma);

    Console.WriteLine($"File {parameters.OutputPngFilename} has been written to disk.");
}
