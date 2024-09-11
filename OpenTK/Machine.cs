using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenTK2
{
    public class Machine
    {
        private static Machine instance = null;
        private static List<Ball> balls = new List<Ball>();
        private double SchuifZLevel = 0.0;
        public float PositionSchuif { get; set; }
        public bool CommandOpenSchuif { get; set; }
        private bool OpeningSchuif { get; set; }
        private bool ClosingSchuif { get; set; }
        public Color SensorColor { get; set; }
        private bool[] OpeningStoter { get; set; }
        private bool[] ClosingStoter { get; set; }

        public float[] PositionStoter { get; set; }
        private int WaitSchuif;
        private int WaitSensor;
        public static Machine Instance()
        {
            if (instance == null)
            {
                instance = new Machine();
            }
            return instance;
        }

        public Machine()
        {
            OpeningStoter = new bool[3];
            ClosingStoter = new bool[3];
            PositionStoter = new float[3];
        }

        public bool ForeSensor(Ball aBall)
        {
            if ((Math.Abs(aBall.Position.X + 0.5) <= 0.02))
                return true;
            else
                return false;
        }

        public bool ForeStoter(Color aColor, Ball aBall)
        {
            if (aColor == Color.Red)
            {
                if ((Math.Abs(aBall.Position.X - 1.5) <= 0.02) && (Math.Abs(aBall.Position.Y - 0.5) <= 0.02))
                    return true;
                else
                    return false;
            }
            else if (aColor == Color.Green)
            {
                if ((Math.Abs(aBall.Position.X - 2.5) <= 0.02) && (Math.Abs(aBall.Position.Y - 0.5) <= 0.02))
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Math.Abs(aBall.Position.X - 3.5) <= 0.02) && (Math.Abs(aBall.Position.Y - 0.5) <= 0.02))
                    return true;
                else
                    return false;
            }

        }
        public bool OnStoter(Color aColor, Ball aBall)
        {
            if (aColor == Color.Red)
            {
                if ((Math.Abs(aBall.Position.X - 1.5) <= 0.02) && ((aBall.Position.Y < 0.5) && (aBall.Position.Y > -1.5)))
                    return true;
                else
                    return false;
            }
            else if (aColor == Color.Green)
            {
                if ((Math.Abs(aBall.Position.X - 2.5) <= 0.02) && ((aBall.Position.Y < 0.5) && (aBall.Position.Y > -1.5)))
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Math.Abs(aBall.Position.X - 3.5) <= 0.02) && ((aBall.Position.Y < 0.5) && (aBall.Position.Y > -1.5)))
                    return true;
                else
                    return false;
            }

        }

        public bool OnTransporter(Ball aBall)
        {
            if ((aBall.Position.X > 4.5) || (Math.Abs(aBall.Position.Z - 1.0) >= 0.02) || (Math.Abs(aBall.Position.Y - 0.5) >= 0.02))
                return false;
            else
                return true;
        }


        public bool OnBottom(Ball aBall)
        {
            if (Math.Abs(aBall.Position.Z - 0.0) >= 0.02)
                return false;
            else
                return true;
        }

        public bool OnSchuif(Ball aBall)
        {
            if ((Math.Abs(aBall.Position.Z - SchuifZLevel - 0.5) >= 0.02) ||
                (Math.Abs(PositionSchuif - 1.0f) < 0.02))
                    return false;
            else
                return true;
        }

        public float SchuifStep()
        {
            return 1.0f / 60;
        }
        public float StoterStep()
        {
            return 3.0f / 60;
        }

        public Point3D TransportStep()
        {
            return new Point3D(1.0 / 60, 0.0, 0.0);
        }

        public void SensorColorOff()
        {
            SensorColor = Color.Aqua;
        }

        public void UpdateState()
        {
            if (WaitSchuif != 0) // wait seconds before closing schuif
            {
                WaitSchuif--;
                if (WaitSchuif == 0)
                {
                    // close schuif
                    ClosingSchuif = true;
                }
            }

            if (WaitSensor != 0)
            {
                WaitSensor--;
                if (WaitSensor == 0)
                {
                    // close color
                    SensorColorOff();
                }
            }

            if (CommandOpenSchuif)
            {
                CommandOpenSchuif = false;
                OpeningSchuif = true;
            }

            for (int i = 0; i<3; i++)
            {
                if (OpeningStoter[i])
                {
                    PositionStoter[i] -= Machine.Instance().StoterStep();
                    if (Math.Abs(PositionStoter[i] + 1.0f) < 0.04)
                    {
                        OpeningStoter[i] = false;
                        ClosingStoter[i] = true;
                    }
                }

                if (ClosingStoter[i])
                {
                    PositionStoter[i] += Machine.Instance().StoterStep();
                    if (Math.Abs(PositionStoter[i] - 0.0f) < 0.04)
                    {
                        ClosingStoter[i] = false;
                    }
                }

            }

            if (OpeningSchuif)
            {
                PositionSchuif += Machine.Instance().SchuifStep();
                if (Math.Abs(PositionSchuif - 1.0f) < 0.02)
                {
                    OpeningSchuif = false;
                    WaitSchuif = 60 * 2;
                }
            }

            if (ClosingSchuif)
            {
                PositionSchuif -= Machine.Instance().SchuifStep();
                if (Math.Abs(PositionSchuif - 0.0f) < 0.001)
                {
                    ClosingSchuif = false;
                }
            }

            foreach (var ball in balls)
            {
                if (Machine.Instance().ForeSensor(ball))
                {
                    SensorColor = ball.Color;
                    WaitSensor = 60 * 10;
                }

                if( (ball.Color == SensorColor ) && (ForeStoter(SensorColor, ball)))
                {
                    if (SensorColor == Color.Red)
                    {
                        OpeningStoter[0] = true;
                    }
                    else if (SensorColor == Color.Green)
                    {
                        OpeningStoter[1] = true;
                    }
                    else
                    {
                        OpeningStoter[2] = true;
                    }
                    ball.Position.Y -= Machine.Instance().StoterStep();
                }

                if (Machine.Instance().OnTransporter(ball))
                {
                    ball.Position.X += Machine.Instance().TransportStep().X;
                    ball.Position.Y += Machine.Instance().TransportStep().Y;
                    ball.Position.Z += Machine.Instance().TransportStep().Z;
                }
                else
                {
                    if (Machine.Instance().OnStoter(ball.Color, ball))
                    {
                        ball.Position.Y -= Machine.Instance().StoterStep();
                    }
                    else if (Machine.Instance().OnBottom(ball))
                    {
                        //ball.Position = new Point3D(-2.5, 0.5, 3.0);
                    }
                    else if (Machine.Instance().OnSchuif(ball))
                    {
                        // no movement
                    }
                    else
                    {
                        ball.Position.Z -= 1.0 / 60;
                    }
                }
            }
        }

        public void AddSchuif(double aZLevel)
        {
            SchuifZLevel = aZLevel;
            PositionSchuif = 0.0f;
            CommandOpenSchuif = false;
            WaitSchuif = 0;
        }
        public void AddSensor()
        {
            WaitSensor = 0;
        }

        public void AddBall(Ball aBall)
        {
            Ball newBall = new Ball();

            Color c = newBall.Color; // save the random color
            newBall.Copy(aBall);
            newBall.Color = c; // restore the random color
            balls.Add(newBall);
        }
        public List<Ball> GetBalls()
        {
            return balls;
        }
    }
}
