using OpenTK;
using System.Collections.Generic;

namespace OpenTK2
{
   public class MeshGeometry3D
   {
      public List<Point3D> Positions { get; set; }
      public List<int> TriangleIndices { get; set; }

      public MeshGeometry3D()
      {
         Positions = new List<Point3D>();
         TriangleIndices = new List<int>();
      }
   }
}
