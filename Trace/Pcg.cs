using System.Data;

namespace Trace;

public class Pcg
{
    public ulong State { get; set; }
    public ulong Inc { get; set; }

    public Pcg(ulong init_state = 42, ulong init_seq = 54)
    {
        State = 0;
        Inc = (init_seq << 1) | 1;
        random();       //Throw a random number and discard it
        State += init_state;
        random();       //Throw a random number and discard it

    }


    public uint random()
    {
        // 64-bit
        ulong oldState = State;
        State = (ulong) (oldState * 6364136223846793005 + Inc);
        
        // 32-bit
        uint xorShifted = (uint) (((oldState >> 18) ^ oldState) >> 27);
        uint rot = (uint) (oldState >> 59);

        return (uint) (xorShifted >> rot) | (xorShifted << ((-rot) & 31));
    }
    
    /// <summary>
    /// Returns a new floating-point random number in [0,1) and
    /// advance PCG's internal state.
    /// </summary>
    public float random_float()
    {
        return random() / 0xffffffff;
    }
}