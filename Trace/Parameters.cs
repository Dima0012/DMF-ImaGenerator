namespace Trace;

public struct Parameters
{
    public string InputPfmFilename { get; set; } = "";
    public float Factor { get; set; } = 0.2f;
    public float Gamma { get; set; } = 1.0f;
    public string OutputPngFilename { get; set; } = "";

    public Parameters(string[] args)
    {
        if (args.Length != 4) 
    }

}
