namespace MagmaWorks.ForceMomentInteraction
{
    public class ForceMomentVertex : IForceMomentVertex
    {
        public Force X { get; set; } = Force.Zero;
        public Moment Y { get; set; } = Moment.Zero;
        public Moment Z { get; set; } = Moment.Zero;
        public ICoordinate TextureCoordinate { get; set; } = new Coordinate();

        public ForceMomentVertex() { }

        public ForceMomentVertex(Force x, Moment y, Moment z)
        {
            X = x;
            Y = y;
            Z = z.ToUnit(y.Unit);
        }

        public ForceMomentVertex(Force x, Moment y, Moment z, ICoordinate coordinate)
        {
            X = x;
            Y = y;
            Z = z.ToUnit(y.Unit);
            TextureCoordinate = coordinate;
        }

        public ForceMomentVertex(double x, double y, double z, ForceUnit forceUnit, MomentUnit momentUnit)
        {
            X = new Force(x, forceUnit);
            Y = new Moment(y, momentUnit);
            Z = new Moment(z, momentUnit);
        }
    }
}
