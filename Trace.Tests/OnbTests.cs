using System;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class OnbTests
{
    [Fact]
    public void TestOnb()
    {
        var pcg = new Pcg();
        const int nTest = 1000;

        for (var i = 0; i < nTest; i++)
        {
            var normalizedVec = new Vec(
                pcg.random_float(),
                pcg.random_float(),
                pcg.random_float()
            ).normalize();

            var onb = new Onb(normalizedVec);

            // Verify that the z axis is aligned with the normal
            Assert.True(onb.E3.is_close(normalizedVec));
            
            const double tolerance = 10e-1;
            // Verify that the base is orthogonal
            var temp = onb.E1 * onb.E2;
            Assert.True(temp < tolerance);
            
            var temp2 = onb.E2 * onb.E3;
            Assert.True(temp2 < tolerance);
            
            
            Assert.True(onb.E3 * onb.E1 < tolerance);

            // Verify that each component is normalized
            
            Assert.True(Math.Abs(onb.E1.squared_norm() - 1f) < tolerance);
            Assert.True(Math.Abs(onb.E2.squared_norm() - 1f) < tolerance);

            var squaredNorm = onb.E3.squared_norm() - 1f;
            Assert.True(Math.Abs(squaredNorm) < tolerance);

            // Verify that the base is right-oriented
            // Assert.True(onb.E1.vec_prod(onb.E2).is_close(onb.E3, tolerance));
        }
    }
}