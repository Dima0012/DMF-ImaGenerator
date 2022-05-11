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
}