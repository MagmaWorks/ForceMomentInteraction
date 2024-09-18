using System;
using System.Collections.Generic;
using MagmaWorks.Geometry;
using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public class ForceMomentMesh : IForceMomentMesh
    {
        public IList<int[]> MeshIndices { get; set; }
        public IList<IForceMomentVertex> Verticies { get; set; }
        public IList<IForceMomentTriFace> Faces { get; set; }
        public double Opacity { get; set; }
        public IBrush Brush { get; set; }

        public ForceMomentMesh()
        {
            MeshIndices = new List<int[]>();
            Verticies = new List<IForceMomentVertex>();
            Brush = new Brush(128, 128, 0);
            Opacity = 1;
        }

        public ForceMomentMesh(IList<IForceMomentVertex> verticies, IList<IForceMomentTriFace> faces)
        {
            Verticies = verticies;
            Faces = faces;
            Brush = new Brush(128, 128, 0);
            Opacity = 1;
        }

        public void AddVertex<C>(Force x, Moment y, Moment z, C coordinate) where C : ICoordinate
        {
            Verticies.Add(new ForceMomentVertex(x, y, z, coordinate));
        }

        public void AddVertex<V>(V vertex) where V : ICartesian3d<Force, Moment, Moment>
        {
            Verticies.Add(new ForceMomentVertex(vertex.X, vertex.Y, vertex.Z));
        }

        public void SetIndices(IList<int[]> indices)
        {
            MeshIndices = new List<int[]>();
            Faces = new List<IForceMomentTriFace>();
            foreach (int[] item in indices)
            {
                if (item.Length != 3)
                {
                    throw new ArgumentException($"There must three indices per mesh face but {item.Length} was provided");
                }

                Faces.Add(new ForceMomentTriFace(Verticies[item[0]], Verticies[item[1]], Verticies[item[2]]));
                MeshIndices.Add(item);
            }
        }

        public List<int[]> GetUniqueEdges()
        {
            List<int[]> returnList = new List<int[]>();
            foreach (int[] triangle in MeshIndices)
            {
                List<int[]> edges = new List<int[]>
                {
                    new int[] { triangle[0], triangle[1] },
                    new int[] { triangle[1], triangle[2] },
                    new int[] { triangle[2], triangle[0] }
                };

                foreach (int[] edge in edges)
                {
                    bool add = true;
                    foreach (int[] edge2 in returnList)
                    {
                        if ((edge[0] == edge2[0] && edge[1] == edge2[1])
                           || (edge[0] == edge2[1] && edge[1] == edge2[0]))
                        {
                            add = false;
                        }
                    }

                    if (add)
                    {
                        returnList.Add(edge);
                    }
                }
            }

            return returnList;
        }

        public List<int[]> GetAllEdges()
        {
            List<int[]> returnList = new List<int[]>();
            foreach (int[] triangle in MeshIndices)
            {
                List<int[]> edges = new List<int[]>
                {
                    new int[] { triangle[0], triangle[1] },
                    new int[] { triangle[1], triangle[2] },
                    new int[] { triangle[2], triangle[0] }
                };

                foreach (int[] edge in edges)
                {
                    returnList.Add(edge);
                }
            }
            return returnList;
        }

        public List<int[]> GetOuterEdges()
        {
            List<int[]> allEdges = GetAllEdges();
            var returnList = new List<int[]>();
            for (int i = 0; i < allEdges.Count; i++)
            {
                int[] edge = allEdges[i];
                bool isOuterEdge = true;
                for (int j = 0; j < allEdges.Count; j++)
                {
                    int[] edge2 = allEdges[j];
                    if (((edge[0] == edge2[0] && edge[1] == edge2[1])
                        || (edge[0] == edge2[1] && edge[1] == edge2[0]))
                        && (i != j))
                    {
                        isOuterEdge = false;
                    }
                }

                if (isOuterEdge)
                {
                    returnList.Add(new int[] { edge[0], edge[1] });
                }
            }

            return returnList;
        }

        public List<List<int>> GetMeshOutlines()
        {
            List<int[]> outerEdges = GetOuterEdges();
            List<bool> alreadyUsed = new List<bool>();
            foreach (int[] item in outerEdges)
            {
                alreadyUsed.Add(false);
            }

            var returnList = new List<List<int>>();
            for (int i = 0; i < outerEdges.Count; i++)
            {
                if (alreadyUsed[i] == false)
                {
                    List<int> currentList = new List<int>();
                    bool nothingAdded = false;
                    int prevID = outerEdges[i][1];
                    while (!nothingAdded)
                    {
                        bool somethingAdded = false;
                        for (int j = 0; j < outerEdges.Count; j++)
                        {
                            if (!alreadyUsed[j] && prevID == outerEdges[j][0])
                            {
                                somethingAdded = true;
                                currentList.Add(outerEdges[j][1]);
                                prevID = outerEdges[j][1];
                                alreadyUsed[j] = true;
                            }
                            else if (!alreadyUsed[j] && prevID == outerEdges[j][1])
                            {
                                somethingAdded = true;
                                currentList.Add(outerEdges[j][0]);
                                prevID = outerEdges[j][0];
                                alreadyUsed[j] = true;
                            }
                        }

                        if (!somethingAdded)
                        {
                            nothingAdded = true;
                        }
                    }

                    returnList.Add(currentList);
                }

            }
            return returnList;
        }

        public void ReverseFaceDirections()
        {
            foreach (int[] face in MeshIndices)
            {
                int temp = face[0];
                face[0] = face[2];
                face[2] = temp;
            }
        }
    }
}
