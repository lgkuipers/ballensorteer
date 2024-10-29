// C# example program to demonstrate OpenTK
//
// Steps:
// 1. Create an empty C# console application project in Visual Studio
// 2. Place OpenTK.dll in the directory of the C# source file
// 3. Add System.Drawing and OpenTK as References to the project
// 4. Paste this source code into the C# source file
// 5. Run. You should see a colored triangle. Press ESC to quit.
//
// Copyright (c) 2013 Ashwin Nanjappa
// Released under the MIT License

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using OpenTK.Input;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing.Imaging;

namespace OpenTK2
{
    /*
     * mesh is een verzameling bij elkaar horende punten uitgedrukt in driehoeken (posities van vertices en de indices van de hoekpunten van de driehoeken)
     * vertix is 1 punt van de verzameling punten
     * index is de index van het punt
     * face is een driehoek uitgedrukt in de indices van de hoekpunten
     */
    class Game2 : GameWindow
    {
        private double ivX = -1.0;
        public static double ivBall = 0.0;
        private double ivY = -1.0;
        Vector2 lastMousePos = new Vector2();
        private double myVar = 0.0;
        private bool UpdateExamples = false;

        private Ball startBall = new Ball();

        private Bitmap bitmap;
        //private Bitmap bmp;
        private int height = 512;
        private int width = 512;
        //private Graphics gfx;
        private int texture;

        private Graphics graphics;
        private Font arialFont;

        private int pos = 0;

        public Game2()
          : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;
        }

        private byte[] BmpToBytes_MemStream(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            bmp.Save(ms, ImageFormat.Jpeg);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            //bmp.Dispose();
            ms.Close();

            return bmpBytes;
        }

        protected override void OnLoad(EventArgs aE)
        {
            base.OnLoad(aE);

            startBall.Position = new Point3D(-2.5, 0.5, 4.0);

            Machine.Instance().AddBall(startBall);
            Machine.Instance().AddSchuif(2.5);
            Machine.Instance().AddSensor();
            Machine.Instance().SensorColorOff();

            Title = "Hallo Lolke";
            lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            float[] mat_diffuse = { 1.0f, 1.0f, 1.0f };
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, mat_diffuse);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.DepthTest);


            /*
            //ImageResult image = ImageResult.FromStream(File.OpenRead("../../container.jpg"), ColorComponents.RedGreenBlueAlpha);
            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            
            string imageFilePath = @"../../container.jpg";
            bitmap = (Bitmap)System.Drawing.Image.FromFile(imageFilePath);//load the image file

            graphics = Graphics.FromImage(bitmap);
            arialFont = new Font("Arial", 50);
            graphics.DrawString("Demo", arialFont, Brushes.White, new PointF(1, 1));
            
            //bitmap.Save(imageFilePath);
            var bmpBytes = BmpToBytes_MemStream(bitmap);
            ImageResult image = ImageResult.FromMemory(bmpBytes);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
            */




            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            string imageFilePath = @"../../container.jpg";
            //bitmap = (Bitmap)System.Drawing.Image.FromFile(imageFilePath);//load the image file
            graphics = Graphics.FromImage(bitmap);
            arialFont = new Font("Arial", 50);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            FontFamily fontFamily = new FontFamily("Arial");
            Font font = new Font(
                fontFamily,
                50,
                FontStyle.Regular,
                GraphicsUnit.Pixel);
            graphics.DrawString("Test", font, new SolidBrush(Color.BlueViolet), new PointF(1, 1));

            //MemoryStream stream = new MemoryStream();
            //bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //stream.ToArray();
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, stream.ToArray());

            var bmpBytes = BmpToBytes_MemStream(bitmap);
            ImageResult image = ImageResult.FromMemory(bmpBytes);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);

        }

        protected override void OnResize(EventArgs aE)
        {
            base.OnResize(aE);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs aE)
        {
            base.OnUpdateFrame(aE);

            if (Keyboard.GetState()[Key.Escape])
                Exit();
        }

        private void DrawCube(float aX, float aY, float aZ, float aOri, Color aColor, double aL = 0.5, double aW = 0.5, double aH = 0.5)
        {
            Color cLeft = Color.Blue, cRicht = Color.Yellow, cFront = Color.Orange, cBack = Color.HotPink, cUp = Color.Red, cDown = Color.Green;
            GL.PushMatrix();

            if (aColor != Color.Empty)
            {
                cLeft = aColor;
                cRicht = aColor;
                cFront = aColor;
                cBack = aColor;
                cUp = aColor;
                cDown = aColor;
            }

            GL.Translate(aX, aY, aZ);
            GL.Rotate(aOri /*+ ivX*/, 0, 0, 1);

            GL.Begin(PrimitiveType.Quads);

            GL.Normal3(new Vector3(0.0f, 0.0f, -1.0f));
            GL.Color3(cLeft);
            GL.Vertex3(aL, 0, -aH); GL.Vertex3(0, 0, -aH);
            GL.Vertex3(0, aW, -aH); GL.Vertex3(aL, aW, -aH);

            GL.Normal3(new Vector3(0.0f, -1.0f, 0.0f));
            GL.Color3(cRicht);
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, -aH);
            GL.Vertex3(aL, 0, -aH); GL.Vertex3(aL, 0, 0);

            GL.Normal3(new Vector3(0.0f, 0.0f, 1.0f));
            GL.Color3(cFront);
            GL.Vertex3(0, 0, 0); GL.Vertex3(aL, 0, 0);
            GL.Vertex3(aL, aW, 0); GL.Vertex3(0, aW, 0);

            GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
            GL.Color3(cBack);
            GL.Vertex3(0, aW, 0); GL.Vertex3(aL, aW, 0);
            GL.Vertex3(aL, aW, -aH); GL.Vertex3(0, aW, -aH);

            GL.Normal3(new Vector3(1.0f, 0.0f, 0.0f));
            GL.Color3(cUp);
            GL.Vertex3(aL, 0, 0); GL.Vertex3(aL, 0, -aH);
            GL.Vertex3(aL, aW, -aH); GL.Vertex3(aL, aW, 0);

            GL.Normal3(new Vector3(-1.0f, 0.0f, 0.0f));
            GL.Color3(cDown);
            GL.Vertex3(0, 0, -aH); GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, aW, 0); GL.Vertex3(0, aW, -aH);

            GL.End();

            GL.PopMatrix();
        }
        // ReSharper disable once UnusedMember.Local
        private void DrawScene()
        {
            // magazijn
            DrawCube(-3.0f, 1.0f, 3.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(-3.0f, 1.0f, 2.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);

            // magazijn schijf
            DrawCube(-3.0f, 0.0f + Machine.Instance().PositionSchuif, 2.5f, 0.0f, Color.Aqua, 1.0, 1.0, 0.2);

            // lopende band
            DrawCube(-3.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(-2.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(-1.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(0.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(1.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(2.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);
            DrawCube(3.0f, 0.0f, 1.0f, 0.0f, Color.Aqua, 1.0, 1.0, 1.0);

            // color sensor
            DrawCube(-1.0f, 1.0f, 1.5f, 0.0f, Machine.Instance().SensorColor, 1.0, 0.1, 1.0);

            // stoter
            DrawCube(1.0f, 0.0f + Machine.Instance().PositionStoter[0], 1.0f, 0.0f, Color.Aqua, 1.0, 2.0, 1.0);
            DrawCube(2.0f, 0.0f + Machine.Instance().PositionStoter[1], 1.0f, 0.0f, Color.Aqua, 1.0, 2.0, 1.0);
            DrawCube(3.0f, 0.0f + Machine.Instance().PositionStoter[2], 1.0f, 0.0f, Color.Aqua, 1.0, 2.0, 1.0);
        }

        public Vector3 Position = Vector3.Zero + new Vector3(0.0f,0.0f,-6.0f);
        public Vector3 Orientation = new Vector3(0f, 0f, 0.1f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        void ResetCursor()
        {
            OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
            lastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == 27)
            {
                Exit();
            }

            switch (e.KeyChar)
            {
                case 'w':
                    Move2(0f, 0.1f, 0f);
                    break;
                case 'a':
                    Move2(-0.1f, 0f, 0f);
                    break;
                case 's':
                    //Move2(0f, -0.1f, 0f);
                    Machine.Instance().CommandOpenSchuif = true;
                    break;
                case 'd':
                    Move2(0.1f, 0f, 0f);
                    break;
                case 'q':
                    //Move2(0f, 0f, 0.1f);
                    UpdateExamples = false;
                    break;
                case 'e':
                    UpdateExamples = true;
                    //Move2(0f, 0f, -0.1f);
                    break;
                case 'n':
                    AddRotation(0.1f, 0f);
                    break;
                case 'h':
                    AddRotation(-0.1f, 0f);
                    break;
                case 'b':
                    //startBall.Position = new Point3D(-2.5, 0.5, 4.0);
                    Machine.Instance().AddBall(startBall);
                    break;
                case 'p':
                    myVar+=0.1;
                    break;
                case 'm':
                    myVar-=0.1;
                    break;
            }
        }

        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }

        public void Move2(float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }

        protected override void OnRenderFrame(FrameEventArgs aE)
        {
            base.OnRenderFrame(aE);

            //         GL.LoadIdentity();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero + new Vector3(0.0f,-10.0f,10.0f) , new Vector3(0.0f, (float) myVar + 20.0f, -10.0f), Vector3.UnitY);
            //Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            //Matrix4 modelview = GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            DoMotion();

            //DrawFirstTriangle();

            DrawLegend();

            DrawBottom();


            //         GL.Translate(0.4,0.4,-0.25);

            //DrawScene();

            Machine.Instance().UpdateState();

            //Machine.Instance().GetBalls().ForEach(b => b.Draw());

            //DrawSpere(-2.5 + ivBall / 60, 0.5, 1.0, Color.Blue, 0.0, 0.5);

            //DrawAxis();

            //         DrawScene();
            //         GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            //         GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);

            if (UpdateExamples)
            {
                DrawCube(0.5f, 0.5f, 3.0f, (float)ivX, Color.Empty);
                DrawCube(-0.5f, 0.5f, 3.0f, (float)ivX, Color.Empty, 2.0, 1.0, 0.5);

                DrawSpere(0.0, -0.5, 6.0, Color.Aqua, ivX, 0.5);

                DrawObjFile();
            }


            SwapBuffers();

            /*
            if (Focused)
            {
                Vector2 delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
                lastMousePos += delta;

                AddRotation(delta.X, delta.Y);
                ResetCursor();
            }
            */
        }

        private void DrawBottom()
        {
            float sizef = 6.0f;
            GL.Begin(PrimitiveType.Quads);

            GL.Normal3(new Vector3(0.0f, 0.0f, 1.0f));
            GL.Color3(sizef, 0.0f, 0.0f);
            GL.Vertex3(-sizef, -sizef, 0.0f);
            GL.Vertex3(sizef, -sizef, 0.0f);
            GL.Vertex3(sizef, sizef, 0.0f);
            GL.Vertex3(-sizef, sizef, 0.0f);

            GL.End();
        }

        private void DrawLegend()
        {
            float b = 1.0f;
            float x = 5.0f;
            float y = 3.0f;
            float z = 1.0f;
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(new Vector3(0.0f, 0.0f, 1.0f));
            GL.Color3(1.0, 1.0, 1.0);
            GL.Vertex3(x - b, y, z - b);
            GL.Vertex3(x + b, y, z - b);
            GL.Vertex3(x + b, y, z + b);
            GL.Vertex3(x - b, y, z + b);
            GL.End();

            bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bitmap);
            graphics.DrawString("Legend", arialFont, Brushes.White, new PointF(pos++, 1));
            if (pos == 250) pos = 1;
            var bmpBytes = BmpToBytes_MemStream(bitmap);
            ImageResult image = ImageResult.FromMemory(bmpBytes);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
            
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord3(0.0f, 0.0f, 0f); GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord3(1.0f, 0.0f, 0f); GL.Vertex3(10, 0f, 0f);
            GL.TexCoord3(1.0f, 1.0f, 0f); GL.Vertex3(10, 10, 0f);
            GL.TexCoord3(0.0f, 1.0f, 0f); GL.Vertex3(0f, 10, 0f);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

        private void DoMotion()
        {
            ivX += 1.0f;
            ivY += 1.5f;
            ivBall += 1.0f;
            if (ivBall == 6.0 *60) ivBall = 0.0f;
        }

        // ReSharper disable once UnusedMember.Local
        private static void DrawFirstTriangle()
        {
            GL.Begin(PrimitiveType.Triangles);

            GL.Normal3(new Vector3(0.0f, 0.0f, -1.0f));
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, 0.0f);
            //GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(1.0f, -1.0f, 0.0f);
            //GL.Color3(0.2f, 0.9f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f);

            GL.End();
        }

        private static void DrawAxis()
        {
            GL.PushMatrix();
            GL.Begin(PrimitiveType.Quads);

            //GL.Material(MaterialFace.Front, MaterialParameter.Ambient, Color4.Orange);

            GL.Color3(Color.Aquamarine);
            GL.Normal3(new Vector3(0.0f, 0.0f, -1.0f));
            GL.Vertex3(0, 1.0, 3.0);
            GL.Vertex3(1.0, 1.0, 3.0);
            GL.Vertex3(1.0, 0, 3.0);
            GL.Vertex3(0, 0, 3.0);

            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 3.0);
            GL.Vertex3(-0.1, 0, 3.0);
            GL.Vertex3(-0.1, -0.1, 3.0);
            GL.Vertex3(0, -0.1, 3.0);

            GL.End();
            GL.PopMatrix();
        }

        // ReSharper disable once UnusedMember.Local
        public static void DrawSpere(double aX, double aY, double aZ, Color aColor, double aRotate, double aR)
        {
            IcoSphereCreator isc = new IcoSphereCreator();
            MeshGeometry3D mg3D = isc.Create(2, aR);

            Point3D p3D = new Point3D(aX, aY, aZ);
            GlDrawMeshGeometry3D(mg3D, p3D, aRotate, aColor);
        }

        private static void GlDrawMeshGeometry3D(MeshGeometry3D aMg3D, Point3D aP3D, double pX, Color pColor)
        {
            GL.PushMatrix();
            GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, Color4.Blue);
            GL.Rotate(pX, 0, 0, 1);
            GL.Translate(aP3D.X, aP3D.Y, aP3D.Z);
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(pColor);
            for (int i = 0; i < aMg3D.TriangleIndices.Count; i += 3)
            {
                Vector3 a = new Vector3(
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i]].X,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i]].Y,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i]].Z);
                Vector3 b = new Vector3(
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 1]].X,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 1]].Y,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 1]].Z);
                Vector3 c = new Vector3(
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 2]].X,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 2]].Y,
                   (float)aMg3D.Positions[aMg3D.TriangleIndices[i + 2]].Z);
                var dir = Vector3.Cross(b - a, c - a);
                var norm = Vector3.Normalize(dir);
                GL.Normal3(norm);
                GL.Vertex3(a);
                GL.Vertex3(b);
                GL.Vertex3(c);
            }
            GL.End();
            GL.PopMatrix();
        }

        private void DrawObjFile()
        {
            ObjFileCreator of = new ObjFileCreator();
            MeshGeometry3D mg3D = of.Create();

            Point3D p3D = new Point3D(0.0, -0.5, 2.0);
            GlDrawMeshGeometry3D(mg3D, p3D, ivY, Color.Yellow);
        }

        /*
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
        */
    }
}