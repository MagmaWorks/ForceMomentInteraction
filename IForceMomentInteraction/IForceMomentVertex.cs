using MagmaWorks.Geometry;
using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public interface IForceMomentVertex : ICartesianVertex<ICoordinate, Force, Moment, Moment>, IForceMomentInteraction { }
}
