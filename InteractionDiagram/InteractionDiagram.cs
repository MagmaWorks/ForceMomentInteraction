using System;
using System.Collections.Generic;
using System.Linq;
using MagmaWorks.ForceMomentInteraction.Utility;
using MagmaWorks.Geometry;
using MagmaWorks.Taxonomy.Materials;
using MagmaWorks.Taxonomy.Profiles;
using MagmaWorks.Taxonomy.Sections;
using OasysUnits;
using OasysUnits.Units;
using TriangleNet.Geometry;

namespace MagmaWorks.ForceMomentInteraction
{
    public class InteractionDiagram
    {
        public IForceMomentMesh Mesh { get; set; }

        public InteractionDiagram(IConcreteSection section) : this(section, new DiagramSettings()) { }

        public InteractionDiagram(IConcreteSection section, DiagramSettings settings)
        {
            settings = new DiagramSettings();

            IPerimeter perimeter = new Perimeter(section.Profile);
            LengthUnit unit = LengthUnit.Millimeter;
            double ymax = perimeter.OuterEdge.Points.Max(p => p.Y, unit).As(unit);
            double zmax = perimeter.OuterEdge.Points.Max(p => p.Z, unit).As(unit);
            double ymin = perimeter.OuterEdge.Points.Min(p => p.Y, unit).As(unit);
            double zmin = perimeter.OuterEdge.Points.Min(p => p.Z, unit).As(unit);

            var rebarMeshes = new List<List<AnalyticalFace>>();
            var rebarVoidOutlines = new List<Contour>();
            foreach (var rebar in section.Rebars)
            {
                IList<ILocalPoint2d> barPerimeter =
                    new Perimeter(new Circle(rebar.Rebar.Diameter), settings.RebarDivisions).OuterEdge.Points;
                rebarMeshes.Add(
                    Meshing.Create(barPerimeter, settings.RebarMaximumFaceArea, settings.RebarMinimumAngle));
                rebarVoidOutlines.Add(new Contour(
                    barPerimeter.Select(p => new TriangleNet.Geometry.Vertex(p.Y.As(unit), p.Z.As(unit)))));
            }

            List<AnalyticalFace> concreteMesh = Meshing.Create(
                perimeter.OuterEdge.Points, settings.ConcreteMaximumFaceArea, settings.ConcreteMinimumAngle, rebarVoidOutlines);

            ILocalPoint2d centroid = ((LocalPolygon2d)perimeter.OuterEdge).GetBarycenter();
            double baryY = centroid.Y.Millimeters;
            double baryZ = centroid.Z.Millimeters;

            ILinearElasticMaterial concreteMaterial = AnalysisMaterialFactory.CreateLinearElastic(section.Material);
            List<ILinearElasticMaterial> rebarMaterials =
                section.Rebars.Select(r => AnalysisMaterialFactory.CreateLinearElastic(r.Rebar.Material)).ToList();

            var vertices = new List<IForceMomentVertex>();
            for (int i = 0; i < settings.Steps; i++)
            {
                double alpha = i * 2 * Math.PI / (settings.Steps - 1);

                for (int j = 0; j < settings.Steps; j++)
                {
                    double theta = -Math.PI / 2 + j * Math.PI / (settings.Steps - 1);

                    var delta = Math.Cos(theta) * Math.Cos(alpha);
                    var khiy = Math.Cos(theta) * Math.Sin(alpha) / (zmax - zmin);
                    var khiz = Math.Sin(theta) / (ymax - ymin);

                    double fx = 0;
                    double myy = 0;
                    double mzz = 0;

                    foreach (AnalyticalFace face in concreteMesh)
                    {
                        if (delta - face.Z * khiy - face.Y * khiz > 0)
                        {
                            continue;
                        }

                        double fc = concreteMaterial.Strength.Megapascals;
                        fx += -fc * face.Area / 1E+03;
                        myy += (face.Z - baryZ) * fc * face.Area / 1E+06;
                        mzz += (face.Y - baryY) * fc * face.Area / 1E+06;
                    }

                    for(int k = 0; k < rebarMeshes.Count; k++)
                    {
                        double fy = rebarMaterials[k].Strength.Megapascals;
                        foreach (AnalyticalFace face in rebarMeshes[k])
                        {
                            if (delta - face.Z * khiy - face.Y * khiz < 0)
                            {
                                fx += -fy * face.Area / 1E+03;
                                myy += (face.Z - baryZ) * fy * face.Area / 1E+06;
                                mzz += (face.Y - baryY) * fy * face.Area / 1E+06;
                            }
                            else
                            {
                                fx += fy * face.Area / 1E+03;
                                myy += -(face.Z - baryZ) * fy * face.Area / 1E+06;
                                mzz += -(face.Y - baryY) * fy * face.Area / 1E+06;
                            }
                        }
                    }

                    vertices.Add(new ForceMomentVertex(fx, myy, mzz, ForceUnit.Kilonewton, MomentUnit.KilonewtonMeter));
                }
            }

            vertices = vertices.Distinct().ToList();
            IList<IForceMomentTriFace> faces = Meshing.CreateHull(vertices);
            Mesh = new ForceMomentMesh(vertices, faces);
        }
    }
}
