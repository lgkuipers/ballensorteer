namespace OpenTK
{
   public class Point3D
   {
      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; }

      public Point3D()
      {
         X = 0.0f;
         Y = 0.0f;
         Z = 0.0f;
      }
      public Point3D(double aX, double aY, double aZ)
      {
         X = aX;
         Y = aY;
         Z = aZ;
      }
   }
}
