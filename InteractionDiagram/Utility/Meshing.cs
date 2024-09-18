using System;
using System.Collections.Generic;
using System.Linq;
using MagmaWorks.Geometry;
using MIConvexHull;
using OasysUnits;
using OasysUnits.Units;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TMesh = TriangleNet.Mesh;
using TVertex = TriangleNet.Geometry.Vertex;

namespace MagmaWorks.ForceMomentInteraction.Utility
{
    internal static class Meshing
    {
        internal static List<AnalyticalFace> Create(IList<ILocalPoint2d> pts, List<Contour> voids = null)
        {
            LengthUnit unit = LengthUnit.Millimeter;
            Area maxArea = Area.FromSquareMillimeters(Math.Min(
                pts.Max(p => p.Y, unit).As(unit) - pts.Min(p => p.Y, unit).As(unit),
                pts.Max(p => p.Z, unit).As(unit) - pts.Min(p => p.Z, unit).As(unit)) / 20);
            Angle minAngle = Angle.FromDegrees(25);
            return Create(pts, maxArea, minAngle, voids);
        }

        internal static List<AnalyticalFace> Create(IList<ILocalPoint2d> pts, Area maxArea, Angle minAngle, List<Contour> voids = null)
        {
            Polygon pol = new Polygon();
            List<TVertex> bounds = new List<TVertex>();
            foreach (var p in pts)
            {
                bounds.Add(new TVertex(p.Y.Millimeters, p.Z.Millimeters));
            }
            pol.Add(new Contour(bounds), false);

            if (voids != null)
            {
                for (int i = 0; i < voids.Count; i++)
                {
                    pol.Add(voids[i], true);
                }
            }

            QualityOptions qo = new QualityOptions();
            qo.MaximumArea = maxArea.SquareMillimeters;
            qo.MinimumAngle = minAngle.Radians;

            TMesh mesh = new GenericMesher().Triangulate(pol, qo) as TMesh;
            return AnalyticalFace.CreateFromTriangleNetMesh(mesh);
        }

        internal static IList<IForceMomentTriFace> CreateHull(IList<IForceMomentVertex> vertices)
        {
            var faces = new List<IForceMomentTriFace>();
            ConvexHullCreationResult<ConvexHullVertex, ConvexHullFace> convexHull =
                ConvexHull.Create<ConvexHullVertex, ConvexHullFace>(
                    vertices.Select(p => new ConvexHullVertex(p)).ToList());
            var convexHullFaces = convexHull.Result.Faces.ToList();
            for (int i = 0; i < convexHullFaces.Count; i++)
            {
                faces.Add(convexHullFaces[i].ToForceMomentTriFace());
            }

            return faces;
        }
    }
}
