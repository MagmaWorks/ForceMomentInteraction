using System;
using OasysUnits;
using OasysUnits.Units;

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

        /// <summary>
        /// Surface area of the Tri-Face will be returned as a unitless Ratio calculated
        /// using the units the Force-Moment vertex where constructed with.
        /// </summary>
        public IQuantity Area
        {
            get
            {
                _area ??= GetArea(A, B, C);
                return _area;
            }
        }

        private IForceMomentVertex _center = null;
        private IQuantity _area = null;

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

        private static Ratio GetArea(IForceMomentVertex p1, IForceMomentVertex p2, IForceMomentVertex p3)
        {
            double a = GetLength(p1, p2);
            double b = GetLength(p2, p3);
            double c = GetLength(p3, p1);

            double s = (a + b + c) / 2;
            return new Ratio(Math.Sqrt(s * (s - a) * (s - b) * (s - c)), RatioUnit.DecimalFraction);
        }

        private static double GetLength(IForceMomentVertex p1, IForceMomentVertex p2)
        {
            return Math.Sqrt(Math.Pow(p1.X.Value - p2.X.Value, 2)
                + Math.Pow(p1.Y.Value - p2.Y.Value, 2)
                + Math.Pow(p1.Z.Value - p2.Z.Value, 2));
        }
    }
}
