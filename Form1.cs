using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;



namespace IA_C
{
    public partial class Form1 : Form
    {
        class EnableDirection
        {
            public bool goUp;
            public bool goDown;
            public bool goLeft;
            public bool goRight;

            public EnableDirection(bool up, bool down, bool left, bool right)
            {
                goUp = up;
                goDown = down;
                goLeft = left;
                goRight = right;
            }
        };
        class Direction
        {
            public double up;
            public double down;
            public double left;
            public double right;
            public EnableDirection enabled;
            public Direction(double mup, double mdown, double mleft, double mright, bool eup, bool edown, bool eleft, bool eright)
            {
                enabled = new EnableDirection(eup, edown, eleft, eright);
                up = mup;
                down = mdown;
                left = mleft;
                right = mright;
            }
        };

        int startX = 4, startY = 0;
        int greenX = 0, greenY = 4;
        int redX = 1, redY = 4;
        int x, y;
        double score = 1;
        Direction[,] Q = new Direction[5, 5];
        double alpha = 1;
        double reward = -0.04;
        bool restart = false;
        double discount = 0.3;
        Thread st;

        public Form1()
        {
            InitializeComponent();
            ThreadStart thrS = new ThreadStart(run);
            st = new Thread(thrS);
            st.Start();
        }

        private void onClose(object sender, FormClosingEventArgs e)
        {
            st.Abort();
        }
        void initQ()
        {
            x = startX;
            y = startY;
            Q[0, 0] = new Direction(1, 1, 1, 1, false, true, false, true);
            Q[0, 1] = new Direction(1, 1, 1, 1, false, false, true, true);
            Q[0, 2] = new Direction(1, 1, 1, 1, false, false, true, true);
            Q[0, 3] = new Direction(1, 1, 1, 1, false, true, true, true);
            Q[0, 4] = new Direction(1, 1, 1, 1, false, false, false, false);

            Q[1, 0] = new Direction(1, 1, 1, 1, true, true, false, false);
            Q[1, 1] = new Direction(1, 1, 1, 1, false, false, false, false);
            Q[1, 2] = new Direction(1, 1, 1, 1, false, false, false, false);
            Q[1, 3] = new Direction(1, 1, 1, 1, true, true, false, true);
            Q[1, 4] = new Direction(1, 1, 1, 1, false, false, false, false);

            Q[2, 0] = new Direction(1, 1, 1, 1, true, true, false, false);
            Q[2, 1] = new Direction(1, 1, 1, 1, false, false, false, false);
            Q[2, 2] = new Direction(1, 1, 1, 1, false, false, false, false);
            Q[2, 3] = new Direction(1, 1, 1, 1, true, true, false, true);
            Q[2, 4] = new Direction(1, 1, 1, 1, true, true, true, false);

            Q[3, 0] = new Direction(1, 1, 1, 1, true, true, false, true);
            Q[3, 1] = new Direction(1, 1, 1, 1, false, true, true, true);
            Q[3, 2] = new Direction(1, 1, 1, 1, false, true, true, true);
            Q[3, 3] = new Direction(1, 1, 1, 1, true, true, true, true);
            Q[3, 4] = new Direction(1, 1, 1, 1, true, true, true, false);

            Q[4, 0] = new Direction(1, 1, 1, 1, true, false, false, true);
            Q[4, 1] = new Direction(1, 1, 1, 1, true, false, true, true);
            Q[4, 2] = new Direction(1, 1, 1, 1, true, false, true, true);
            Q[4, 3] = new Direction(1, 1, 1, 1, true, false, true, true);
            Q[4, 4] = new Direction(1, 1, 1, 1, true, false, true, false);
        }

        public void getMaxAction(ref double val, ref int action)
        {
            val = Q[x, y].enabled.goUp ? Q[x, y].up : (Q[x, y].enabled.goDown ? Q[x, y].down : (Q[x, y].enabled.goLeft ? Q[x, y].left : Q[x, y].right));
            action = Q[x, y].enabled.goUp ? 0 : (Q[x, y].enabled.goDown ? 1 : (Q[x, y].enabled.goLeft ? 2 : 3));
            if (Q[x, y].enabled.goUp == true)
            {
                if (val < Q[x, y].up)
                {
                    val = Q[x, y].up;
                    action = 0;
                }
            }
            if (Q[x, y].enabled.goDown == true)
            {
                if (val < Q[x, y].down)
                {
                    val = Q[x, y].down;
                    action = 1;
                }
            }
            if (Q[x, y].enabled.goLeft == true)
            {
                if (val < Q[x, y].left)
                {
                    val = Q[x, y].left;
                    action = 2;
                }
            }
            if (Q[x, y].enabled.goRight == true)
            {
                if (val < Q[x, y].right)
                {
                    val = Q[x, y].right;
                    action = 3;
                }
            }
        }

        public void tryMove(int dx, int dy)
        {
            if (dx == -1 && Q[x, y].enabled.goUp == true)
            {
                x--;
                agent.Top -= 50;
            }
            if (dx == 1 && Q[x, y].enabled.goDown == true)
            {
                x++;
                agent.Top += 50;
            }
            if (dy == -1 && Q[x, y].enabled.goLeft == true)
            {
                y--;
                agent.Left -= 50;
            }
            if (dy == 1 && Q[x, y].enabled.goRight == true)
            {
                y++;
                agent.Left += 50;
            }
            score += reward;
            if (y == greenY && x == greenX)
            {
                score += 1.04;
                reward = 1;
                restart = true;
                if (score > 0)
                {
                    Console.WriteLine("Success - score: " + score.ToString());
                }
                else
                {
                    Console.WriteLine("Fail - score: " + score.ToString());
                }
            }
            if (y == redY && x == redX)
            {
                score -= 0.96;
                reward = -1;
                restart = true;
                if (score > 0)
                {
                    Console.WriteLine("Success - score: " + score.ToString());
                }
                else
                {
                    Console.WriteLine("Fail - score: " + score.ToString());
                }
            }
            scoreValue.Text = score.ToString();
        }


        public void doMove(int action)
        {
            switch (action)
            {
                case 0:
                    tryMove(-1, 0);
                    break;
                case 1:
                    tryMove(+1, 0);
                    break;
                case 2:
                    tryMove(0, -1);
                    break;
                case 3:
                    tryMove(0, 1);
                    break;
                default:
                    break;
            }
        }

        public void modifyLastStateActionValue(int firstAction, double val)
        {
            int lastX, lastY;
            switch (firstAction)
            {
                case 0:
                    lastX = x + 1;
                    Q[lastX, y].up = Q[lastX, y].up * (1.0 - alpha) + alpha * (reward + discount * val);
                    break;
                case 1:
                    lastX = x - 1;
                    Q[lastX, y].down = Q[lastX, y].down * (1.0 - alpha) + alpha * (reward + discount * val);
                    break;
                case 2:
                    lastY = y + 1;
                    Q[x, lastY].left = Q[x, lastY].left * (1.0 - alpha) + alpha * (reward + discount * val);
                    break;
                case 3:
                    lastY = y - 1;
                    Q[x, lastY].right = Q[x, lastY].right * (1.0 - alpha) + alpha * (reward + discount * val);
                    break;
                default:
                    break;
            }

        }

        public void run()
        {
            int firstAction = 0;
            int secondAction = 0;
            double maxVal = 0;
            double t = 1;

            initQ();

            while (true)
            {
                //maxVal = -100;
                getMaxAction(ref maxVal, ref firstAction);

                //Console.WriteLine(maxVal + " " + firstAction);
                //Console.WriteLine("Q " + x +" " + y +  "" + Q[x, y].up + " " + Q[x, y].down + " " + Q[x, y].left + " " + Q[x, y].right + " " + " " + alpha);

                doMove(firstAction);

                //maxVal = -100;
                getMaxAction(ref maxVal, ref secondAction);
                //Console.WriteLine(maxVal + " " + secondAction);
                //Console.WriteLine("");
                modifyLastStateActionValue(firstAction, maxVal);

                t++;

                if (restart == true)
                {
                    reward = -0.04;
                    t = 1;
                    score = 1;
                    restart = false;
                    x = startX;
                    y = startY;
                    agent.Location = new Point(0, 200);
                }

                alpha = Math.Pow(t, -0.1);

                Thread.Sleep(100);
            }
        }
    }
}
