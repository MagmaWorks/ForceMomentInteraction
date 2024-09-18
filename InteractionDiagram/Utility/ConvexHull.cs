using MIConvexHull;
using OasysUnits.Units;

namespace MagmaWorks.ForceMomentInteraction.Utility
{
    internal class ConvexHullVertex : IVertex
    {
        public ForceUnit ForceUnit { get; set; } = ForceUnit.Kilonewton;
        public MomentUnit MomentUnit { get; set; } = MomentUnit.KilonewtonMeter;
        public double[] Position { get; set; }

        public ConvexHullVertex(IForceMomentVertex vertex)
        {
            Position = new double[] {
                    vertex.X.As(ForceUnit), vertex.Y.As(MomentUnit), vertex.Z.As(MomentUnit) };
        }

        public ForceMomentVertex ToForceMomentVertex()
        {
            return new ForceMomentVertex(Position[0], Position[1], Position[2], ForceUnit, MomentUnit);
        }
    }

    internal class ConvexHullFace : ConvexFace<ConvexHullVertex, ConvexHullFace>
    {
        public ForceMomentTriFace ToForceMomentTriFace()
        {
            return new ForceMomentTriFace(
                Vertices[0].ToForceMomentVertex(),
                Vertices[1].ToForceMomentVertex(),
                Vertices[2].ToForceMomentVertex());
        }
    }
}
