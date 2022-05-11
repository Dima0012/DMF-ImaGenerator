using Xunit;

namespace Trace.Tests;

public class PcgTests
{
    [Fact]
    public void TestRandom()
    {
        var pcg = new Pcg();

        Assert.True(pcg.State == 1753877967969059832);
        Assert.True(pcg.Inc == 109);

        uint[] testRndVec =
        {
            2707161783, 2068313097,
            3122475824, 2211639955,
            3215226955, 3421331566
        };

        foreach (var expected in testRndVec)
        {
            var result = pcg.random();
            Assert.True(expected == result);
        }
    }
}