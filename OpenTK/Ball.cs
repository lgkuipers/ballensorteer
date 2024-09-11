using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK2
{
    public class Ball
    {
        public Point3D Position { get; set;}
        public Color Color { get; set; }
        private Random rnd = new Random();

        public Ball()
        {
            Position = new Point3D();

            int c = rnd.Next(1, 4);
            switch(c)
            {
                case 1:
                    Color = Color.Red;
                    break;
                case 2:
                    Color = Color.Green;
                    break;
                case 3:
                    Color = Color.Blue;
                    break;
            }
        }

        public Ball(Color aColor)
        {
            Position = new Point3D();
            Color = aColor;
        }

        public void Draw()
        {
            Game2.DrawSpere(Position.X, Position.Y, Position.Z, Color, 0.0, 0.5);
        }
        public void Copy(Ball aBall)
        {
            Position.X = aBall.Position.X;
            Position.Y = aBall.Position.Y;
            Position.Z  = aBall.Position.Z;
            Color = aBall.Color;
        }
    }
}
