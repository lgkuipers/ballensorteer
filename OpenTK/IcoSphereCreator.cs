using OpenTK;
using System;
using System.Collections.Generic;

namespace OpenTK2
{

   public class IcoSphereCreator
   {
      private MeshGeometry3D ivGeometry = new MeshGeometry3D();
      private int ivIndex;
      private Dictionary<long, int> ivMiddlePointIndexCache;

      private struct TriangleIndices
      {
         public readonly int V1;
         public readonly int V2;
         public readonly int V3;

         public TriangleIndices(int aV1, int aV2, int aV3)
         {
            V1 = aV1;
            V2 = aV2;
            V3 = aV3;
         }
      }

      // add vertex to mesh, fix position to be on unit sphere, return index
      private int AddVertex(Point3D aP, double aR)
      {
         double length = Math.Sqrt(aP.X * aP.X + aP.Y * aP.Y + aP.Z * aP.Z) / aR;
         ivGeometry.Positions.Add(new Point3D(aP.X / length, aP.Y / length, aP.Z / length));
         return ivIndex++;
      }

      // return index of point in the middle of p1 and p2
      private int GetMiddlePoint(int aP1, int aP2, double aR)
      {
         // first check if we have it already
         bool firstIsSmaller = aP1 < aP2;
         long smallerIndex = firstIsSmaller ? aP1 : aP2;
         long greaterIndex = firstIsSmaller ? aP2 : aP1;
         long key = (smallerIndex << 32) + greaterIndex;

         int ret;
         if (ivMiddlePointIndexCache.TryGetValue(key, out ret))
         {
            return ret;
         }

         // not in cache, calculate it
         Point3D point1 = ivGeometry.Positions[aP1];
         Point3D point2 = ivGeometry.Positions[aP2];
         Point3D middle = new Point3D(
             (point1.X + point2.X) / 2.0,
             (point1.Y + point2.Y) / 2.0,
             (point1.Z + point2.Z) / 2.0);

         // add vertex makes sure point is on unit sphere
         int i = AddVertex(middle, aR);

         // store it, return index
         ivMiddlePointIndexCache.Add(key, i);
         return i;
      }

        /// <summary>
        /// Create a sphere with 0,0,0 as centre
        /// </summary>
        /// <param name="aRecursionLevel">Number of times that the triangles are split up.</param>
        public MeshGeometry3D Create(int aRecursionLevel, double aR)
      {
         ivGeometry = new MeshGeometry3D();
         ivMiddlePointIndexCache = new Dictionary<long, int>();
         ivIndex = 0;

         // create 12 vertices of a icosahedron
         var t = (1.0 + Math.Sqrt(5.0)) / 2.0;

         AddVertex(new Point3D(-1, t, 0), aR);
         AddVertex(new Point3D(1, t, 0), aR);
         AddVertex(new Point3D(-1, -t, 0), aR);
         AddVertex(new Point3D(1, -t, 0), aR);

         AddVertex(new Point3D(0, -1, t), aR);
         AddVertex(new Point3D(0, 1, t), aR);
         AddVertex(new Point3D(0, -1, -t), aR);
         AddVertex(new Point3D(0, 1, -t), aR);

         AddVertex(new Point3D(t, 0, -1), aR);
         AddVertex(new Point3D(t, 0, 1), aR);
         AddVertex(new Point3D(-t, 0, -1), aR);
         AddVertex(new Point3D(-t, 0, 1), aR);


         // create 20 triangles of the icosahedron
         var faces = new List<TriangleIndices>
         {
         // 5 faces around point 0
            new TriangleIndices(0, 11, 5),
            new TriangleIndices(0, 5, 1),
            new TriangleIndices(0, 1, 7),
            new TriangleIndices(0, 7, 10),
            new TriangleIndices(0, 10, 11),
         // 5 adjacent faces 
            new TriangleIndices(1, 5, 9),
            new TriangleIndices(5, 11, 4),
            new TriangleIndices(11, 10, 2),
            new TriangleIndices(10, 7, 6),
            new TriangleIndices(7, 1, 8),
         // 5 faces around point 3
            new TriangleIndices(3, 9, 4),
            new TriangleIndices(3, 4, 2),
            new TriangleIndices(3, 2, 6),
            new TriangleIndices(3, 6, 8),
            new TriangleIndices(3, 8, 9),
         // 5 adjacent faces 
            new TriangleIndices(4, 9, 5),
            new TriangleIndices(2, 4, 11),
            new TriangleIndices(6, 2, 10),
            new TriangleIndices(8, 6, 7),
            new TriangleIndices(9, 8, 1)
         };

         // refine triangles
         for (int i = 0; i < aRecursionLevel; i++)
         {
            var faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
               // replace triangle by 4 triangles
               int a = GetMiddlePoint(tri.V1, tri.V2, aR);
               int b = GetMiddlePoint(tri.V2, tri.V3, aR);
               int c = GetMiddlePoint(tri.V3, tri.V1, aR);

               faces2.Add(new TriangleIndices(tri.V1, a, c));
               faces2.Add(new TriangleIndices(tri.V2, b, a));
               faces2.Add(new TriangleIndices(tri.V3, c, b));
               faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
         }

         // done, now add triangles to mesh
         foreach (var tri in faces)
         {
            ivGeometry.TriangleIndices.Add(tri.V1);
            ivGeometry.TriangleIndices.Add(tri.V2);
            ivGeometry.TriangleIndices.Add(tri.V3);
         }

         return ivGeometry;
      }
   }
}
