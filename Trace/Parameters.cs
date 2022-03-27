namespace Trace;

public struct Parameters
{
    public string InputPfmFilename { get; } = "";
    public float Factor { get; } = 0.2f;
    public float Gamma { get; } = 1.0f;
    public string OutputPngFilename { get; } = "";

    public Parameters(string[] args)
    {
        if (args.Length != 4)
        {
            throw new RuntimeError("Usage: dotnet run INPUT_PFM_FILE FACTOR GAMMA OUTPUT_PNG_FILE");
        }

        InputPfmFilename = args[0];
       
        try
        {
            Factor = float.Parse(args[1]);
        }
        catch (FormatException err)
        {
            throw new RuntimeError($"Invalid factor ('{args[1]}'), it must be a floating-point number.", err);
        }
        
        try
        {
            Gamma = float.Parse(args[2]);
        }
        catch (FormatException err)
        {
            throw new RuntimeError($"Invalid gamma ('{args[2]}'), it must be a floating-point number.", err);
        }
        
        OutputPngFilename = args[3];
    }

}
