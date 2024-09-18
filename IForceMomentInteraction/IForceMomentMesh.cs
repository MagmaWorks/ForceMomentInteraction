﻿using MagmaWorks.Geometry;
using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public interface IForceMomentMesh : ICartesianMesh<IForceMomentVertex, IForceMomentTriFace, ICoordinate, Force, Moment, Moment>, IForceMomentInteraction { }
}
