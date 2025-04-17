
using TriangleNet.Topology;

namespace MagmaWorks.ForceMomentInteraction.Utility
{
    internal struct AnalyticalFace
    {
        public double Area { get; }
        public double Y { get; }
        public double Z { get; }

        public AnalyticalFace(Triangle tri)
        {
            Y = (tri.GetVertex(0).X + tri.GetVertex(1).X + tri.GetVertex(2).X) / 3;
            Z = (tri.GetVertex(0).Y + tri.GetVertex(1).Y + tri.GetVertex(2).Y) / 3;
            Area = tri.Area;
        }

        public static List<AnalyticalFace> CreateFromTriangleNetMesh(TriangleNet.Mesh m)
        {
            return m.Triangles.AsParallel().Select(t => new AnalyticalFace(t)).ToList();
        }
    }
}
