using System.Threading.Tasks;
using MagmaWorks.ForceMomentInteraction.Utility;
using MagmaWorks.Geometry;
using MagmaWorks.Taxonomy.Materials;
using MagmaWorks.Taxonomy.Materials.StandardMaterials.En;
using MagmaWorks.Taxonomy.Profiles;
using MagmaWorks.Taxonomy.Sections;
using TriangleNet.Geometry;

namespace MagmaWorks.ForceMomentInteraction
{
    public class InteractionDiagram
    {
        public IForceMomentMesh Mesh { get; set; }

        private LengthUnit _unit = LengthUnit.Millimeter;
        private double _yLength;
        private double _zLength;
        private double _baryY;
        private double _baryZ;
        List<List<AnalyticalFace>> _rebarMeshes;
        List<AnalyticalFace> _concreteMesh;
        ILinearElasticMaterial _concreteMaterial;
        List<ILinearElasticMaterial> _rebarMaterials;
        public InteractionDiagram(IConcreteSection section) : this(section, new DiagramSettings()) { }

        public InteractionDiagram(IConcreteSection section, DiagramSettings settings)
        {
            Initialise(section, settings);
            var vertices = new List<IForceMomentVertex>();
            Parallel.For(0, settings.Steps, (i) =>
            {
                double alpha = i * 2 * Math.PI / (settings.Steps - 1);

                for (int j = 0; j < settings.Steps; j++)
                {
                    double theta = -Math.PI / 2 + j * Math.PI / (settings.Steps - 1);

                    double delta = Math.Cos(theta) * Math.Cos(alpha);
                    double khiy = Math.Cos(theta) * Math.Sin(alpha) / _zLength;
                    double khiz = Math.Sin(theta) / _yLength;

                    double fx = 0;
                    double myy = 0;
                    double mzz = 0;

                    double fc = _concreteMaterial.Strength.Megapascals;

                    foreach (AnalyticalFace face in _concreteMesh)
                    {
                        double deformation = delta - face.Z * khiy - face.Y * khiz;
                        if (deformation > 0)
                        {
                            continue;
                        }

                        fx += -fc * face.Area / 1E+03;
                        myy += (face.Z - _baryZ) * fc * face.Area / 1E+06;
                        mzz += (face.Y - _baryY) * fc * face.Area / 1E+06;
                    }

                    for (int k = 0; k < _rebarMeshes.Count; k++)
                    {
                        double fy = _rebarMaterials[k].Strength.Megapascals;
                        foreach (AnalyticalFace face in _rebarMeshes[k])
                        {
                            double deformation = delta - face.Z * khiy - face.Y * khiz;
                            if (deformation < 0)
                            {
                                fx += -fy * face.Area / 1E+03;
                                myy += (face.Z - _baryZ) * fy * face.Area / 1E+06;
                                mzz += (face.Y - _baryY) * fy * face.Area / 1E+06;
                            }
                            else
                            {
                                fx += fy * face.Area / 1E+03;
                                myy += -(face.Z - _baryZ) * fy * face.Area / 1E+06;
                                mzz += -(face.Y - _baryY) * fy * face.Area / 1E+06;
                            }
                        }
                    }

                    vertices.Add(new ForceMomentVertex(fx, myy, mzz, ForceUnit.Kilonewton, MomentUnit.KilonewtonMeter));
                }
            });

            vertices = vertices.Distinct().ToList();
            IList<IForceMomentTriFace> faces = Meshing.CreateHull(vertices);
            Mesh = new ForceMomentMesh(vertices, faces);
        }

        private (double yLength, double zLength) GetBounds(IPerimeter perimeter, LengthUnit unit)
        {
            double ymax = perimeter.OuterEdge.Points.Max(p => p.Y, unit).As(unit);
            double zmax = perimeter.OuterEdge.Points.Max(p => p.Z, unit).As(unit);
            double ymin = perimeter.OuterEdge.Points.Min(p => p.Y, unit).As(unit);
            double zmin = perimeter.OuterEdge.Points.Min(p => p.Z, unit).As(unit);
            return (ymax - ymin, zmax - zmin);
        }

        private (double y, double z) GetCentroid(IPerimeter perimeter, LengthUnit unit)
        {
            ILocalPoint2d centroid = ((LocalPolyline2d)perimeter.OuterEdge).GetBarycenter();
            return (centroid.Y.As(unit), centroid.Z.As(unit));
        }

        private void Initialise(IConcreteSection section, DiagramSettings settings)
        {
            var rebarVoidOutlines = new List<Contour>();
            (_rebarMeshes, rebarVoidOutlines) = MeshRebars(section, settings, _unit);
            IPerimeter perimeter = new Perimeter(section.Profile);
            _concreteMesh = MeshConcrete(perimeter, settings, rebarVoidOutlines);
            (_yLength, _zLength) = GetBounds(perimeter, _unit);
            (_baryY, _baryZ) = GetCentroid(perimeter, _unit);
            _concreteMaterial = AnalysisMaterialFactory.CreateLinearElastic((IEnConcreteMaterial)section.Material);
            _rebarMaterials =
                section.Rebars.Select(r => AnalysisMaterialFactory.CreateLinearElastic((IEnRebarMaterial)r.Rebar.Material)).ToList();
        }

        private List<AnalyticalFace> MeshConcrete(IPerimeter perimeter, DiagramSettings settings, List<Contour> voids)
        {
            return Meshing.Create(perimeter.OuterEdge.Points, settings.ConcreteMaximumFaceArea, settings.ConcreteMinimumAngle, voids);
        }

        private (List<List<AnalyticalFace>> meshes, List<Contour> outlines) MeshRebars(IConcreteSection section, DiagramSettings settings, LengthUnit unit)
        {
            var rebarMeshes = new List<List<AnalyticalFace>>();
            var rebarVoidOutlines = new List<Contour>();
            foreach (var rebar in section.Rebars)
            {
                IList<ILocalPoint2d> barPerimeter =
                    new Perimeter(new Circle(rebar.Rebar.Diameter), settings.RebarDivisions).OuterEdge.Points;
                rebarMeshes.Add(
                    Meshing.Create(barPerimeter, settings.RebarMaximumFaceArea, settings.RebarMinimumAngle));
                rebarVoidOutlines.Add(
                    new Contour(barPerimeter.Select(
                        p => new TriangleNet.Geometry.Vertex(p.Y.As(unit), p.Z.As(unit)))));
            }

            return (rebarMeshes, rebarVoidOutlines);
        }
    }
}
