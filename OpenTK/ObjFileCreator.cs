using OpenTK;
using System;
using System.Globalization;
using System.IO;

namespace OpenTK2
{
   class ObjFileCreator
   {
      private MeshGeometry3D ivGeometry = new MeshGeometry3D();

      public Matrix3d NewRotateAroundX(double aRadians)
      {
         var matrix = new Matrix3d
         {
            [0, 0] = 1.0f,
            [1, 1] = Math.Cos(aRadians),
            [1, 2] = Math.Sin(aRadians),
            [2, 1] = -(Math.Sin(aRadians)),
            [2, 2] = Math.Cos(aRadians)
         };
         return matrix;
      }
      public Matrix3d NewRotateAroundY(double aRadians)
      {
         var matrix = GetMatrix(aRadians);
         return matrix;
      }

      private static Matrix3d GetMatrix(double aRadians)
      {
         return new Matrix3d
         {
            [1, 1] = 1.0f,
            [0, 0] = Math.Cos(aRadians),
            [0, 2] = -(Math.Sin(aRadians)),
            [2, 0] = Math.Sin(aRadians),
            [2, 2] = Math.Cos(aRadians)
         };
      }

      public Matrix3d NewRotateAroundZ(double aRadians)
      {
         var matrix = new Matrix3d
         {
            [2, 2] = 1.0f,
            [0, 0] = Math.Cos(aRadians),
            [0, 1] = Math.Sin(aRadians),
            [1, 0] = -(Math.Sin(aRadians)),
            [1, 1] = Math.Cos(aRadians)
         };
         return matrix;
      }

      public Vector3d NewVector(Matrix3d aM3D, Vector3d aV3D)
      {
         Vector3d r = new Vector3d
         {
            [0] = aM3D[0, 0] * aV3D[0] + aM3D[0, 1] * aV3D[1] + aM3D[0, 2] * aV3D[2],
            [1] = aM3D[1, 0] * aV3D[0] + aM3D[1, 1] * aV3D[1] + aM3D[1, 2] * aV3D[2],
            [2] = aM3D[2, 0] * aV3D[0] + aM3D[2, 1] * aV3D[1] + aM3D[2, 2] * aV3D[2]
         };
         return r;
      }

      public Matrix3d NewMatrix(Matrix3d aM1, Matrix3d aM2)
      {
         Matrix3d r = new Matrix3d();
         for (int row = 0; row < 3; row++)
         {
            for (int col = 0; col < 3; col++)
            {
               // Multiply the row of A by the column of B to get the row, column of product.  
               for (int inner = 0; inner < 3; inner++)
               {
                  r[row,col] += aM1[row,inner] * aM2[inner,col];
               }
            }
         }
         return r;
      }
      public MeshGeometry3D Create()
      {
         ivGeometry = new MeshGeometry3D();

         ReadObjFile("teapot.obj");

         Matrix3d lm3Dx = NewRotateAroundX(0.0 * 2 * Math.PI / 360.0f);
         Matrix3d lm3Dy = NewRotateAroundY(135.0 * 2 * Math.PI / 360.0f);
         Matrix3d lm3Dz = NewRotateAroundZ(0.0 * 2 * Math.PI / 360.0f);
         Matrix3d lm3D = NewMatrix(NewMatrix(lm3Dx, lm3Dy), lm3Dz);
         Vector3d lv3D = new Vector3d();

         for (int i = 0; i < ivGeometry.Positions.Count; i++)
         {
            lv3D.X = ivGeometry.Positions[i].X;
            lv3D.Y = ivGeometry.Positions[i].Y;
            lv3D.Z = ivGeometry.Positions[i].Z;
            lv3D = NewVector(lm3D, lv3D);
            ivGeometry.Positions[i].X = lv3D.X;
            ivGeometry.Positions[i].Y = lv3D.Y;
            ivGeometry.Positions[i].Z = lv3D.Z;
         }


         return ivGeometry;
      }

      public void ReadObjFile(string aFileName)
      {
         string line = "";
         try
         {

            using (StreamReader streamReader = new StreamReader(aFileName))
            {
               while (!streamReader.EndOfStream)
               {
                  var readLine = streamReader.ReadLine();
                  if (readLine != null) line = readLine.Trim();
                  string[] strArrayRead = line.Split();
                  if (strArrayRead.Length >= 0)
                  {
                     switch (strArrayRead[0].ToLower())
                     {
                        case "v"://Vertex
                           Point3D vertex = new Point3D();

                           //double dx, dy, dz;
                           float f;
                           float.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en-US"), out f);
                           vertex.X = f;
                           float.TryParse(strArrayRead[4], NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en-US"), out f);
                           vertex.Y = f;
                           float.TryParse(strArrayRead[6], NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en-US"), out f);
                           vertex.Z = f;

                           AddVertex(vertex);
                           break;

                        case "f":
                           int i;
                           int.TryParse(strArrayRead[2], out i);
                           ivGeometry.TriangleIndices.Add(i-1);
                           int.TryParse(strArrayRead[4], out i);
                           ivGeometry.TriangleIndices.Add(i-1);
                           int.TryParse(strArrayRead[6], out i);
                           ivGeometry.TriangleIndices.Add(i-1);
                           break;
                     }
                  }
               }

            }
         }
         catch (Exception err)
         {
            System.Windows.Forms.MessageBox.Show(@"Error reading obj file - Vertices: " + line + @" ; " + err.Message);
         }

      }
      private void AddVertex(Point3D aP)
      {
         double fact = 0.1f;
         ivGeometry.Positions.Add(new Point3D(aP.X * fact, aP.Y * fact, aP.Z * fact));
      }
   }
}
