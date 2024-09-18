﻿using MagmaWorks.Geometry;
using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public interface IForceMomentTriFace : ICartesianTriFace<IForceMomentVertex, ICoordinate, Force, Moment, Moment>, IForceMomentInteraction { }
}