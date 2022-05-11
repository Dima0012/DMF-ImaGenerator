namespace Trace;

/// <summary>
/// PCG Uniform Pseudo-random Number Generator
/// </summary>
public class Pcg
{
    public ulong State { get; set; }
    public ulong Inc { get; set; }

    public Pcg(ulong initState = 42, ulong initSeq = 54)
    {
        State = 0;
        Inc = (initSeq << 1) | 1;
        random();       //Throw a random number and discard it.
        State += initState;
        random();       //Throw a random number and discard it.

    }

    /// <summary>
    /// Return a new random unsigned 32-bit integer and advance PCG's internal state.
    /// </summary>
    public uint random()
    {
        // 64-bit
        var oldState = State;
        State = oldState * 6364136223846793005 + Inc;
        
        // 32-bit
        var xorShifted = (uint) (((oldState >> 18) ^ oldState) >> 27);
        var rot = (uint) (oldState >> 59);

        return xorShifted >> (int) rot | (xorShifted << (int) (-rot & 31));
    }
    
    /// <summary>
    /// Returns a new floating-point random number in [0,1) and
    /// advance PCG's internal state.
    /// </summary>
    public float random_float()
    {
        return (float) random() / 0xffffffff;
    }
}