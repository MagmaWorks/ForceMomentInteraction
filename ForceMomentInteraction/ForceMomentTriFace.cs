using MagmaWorks.Geometry;
using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public class ForceMomentTriFace : IForceMomentTriFace
    {

        public IForceMomentVertex A { get; }
        public IForceMomentVertex B { get; }
        public IForceMomentVertex C { get; }
        public IForceMomentVertex Center
        {
            get
            {
                _center ??= GetCenter();
                return _center;
            }
        }

        public IQuantity Area => throw new System.NotImplementedException();

        private IForceMomentVertex _center = null;

        public ForceMomentTriFace(IForceMomentVertex a, IForceMomentVertex b, IForceMomentVertex c)
        {
            A = a;
            B = b;
            C = c;
        }

        private ForceMomentVertex GetCenter()
        {
            return new ForceMomentVertex()
            {
                X = (A.X + B.X + C.X) / 3,
                Y = (A.Y + B.Y + C.Y) / 3,
                Z = (A.Z + B.Z + C.Z) / 3,
            };
        }
    }
}
