using UnitsNet;

namespace ForceMomentInteractionTests.Utility
{
    internal static class TestUtility
    {
        internal static void TestQuantitiesAreEqual<T>(T expected, T actual) where T : IQuantity
        {
            Assert.Equal(expected.Value, actual.Value);
            Assert.Equal(expected.Unit, actual.Unit);
        }
    }
}
