using ForceMomentInteractionTests.Utility;
using MagmaWorks.ForceMomentInteraction;
using MagmaWorks.ForceMomentInteraction.Serialization.Extensions;
using MagmaWorks.Geometry;
using OasysUnits;
using OasysUnits.Units;

namespace ForceMomentInteractionTests.UnitTests
{
    public class MeshTests
    {
        [Fact]
        public void CreateMeshTest()
        {
            // Assemble
            var x = new Force(2.3, ForceUnit.Kilonewton);
            var y = new Moment(5.4, MomentUnit.KilonewtonMeter);
            var z = new Moment(6.8, MomentUnit.KilonewtonMeter);

            // Act
            var m = new ForceMomentMesh();
            m.AddVertex(x, y, z, new Coordinate());

            // Assert
            Assert.Single(m.Verticies);
            TestUtility.TestQuantitiesAreEqual(x, m.Verticies[0].X);
            TestUtility.TestQuantitiesAreEqual(y, m.Verticies[0].Y);
            TestUtility.TestQuantitiesAreEqual(z, m.Verticies[0].Z);
        }

        [Fact]
        public void MeshSurvivesJsonRoundtripTest()
        {
            // Assemble
            var x = new Force(2.3, ForceUnit.Kilonewton);
            var y = new Moment(5.4, MomentUnit.KilonewtonMeter);
            var z = new Moment(6.8, MomentUnit.KilonewtonMeter);

            // Act
            var m = new ForceMomentMesh();
            m.AddVertex(new ForceMomentVertex(x, y, z));
            string json = m.ToJson();
            IForceMomentMesh meshDeserialized = json.FromJson<ForceMomentMesh>();

            // Assert
            Assert.Single(m.Verticies);
            TestUtility.TestQuantitiesAreEqual(x, meshDeserialized.Verticies[0].X);
            TestUtility.TestQuantitiesAreEqual(y, meshDeserialized.Verticies[0].Y);
            TestUtility.TestQuantitiesAreEqual(z, meshDeserialized.Verticies[0].Z);
        }
    }
}
