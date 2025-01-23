using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Threading;

namespace spaceRace
{
    public partial class Form1 : Form
    {
        Random randGen = new Random();

        SoundPlayer wrapPlayer = new SoundPlayer(Properties.Resources.zip);
        SoundPlayer expPlayer = new SoundPlayer(Properties.Resources.Explosion);
        SoundPlayer victoryPlayer = new SoundPlayer(Properties.Resources.VVVVVV);

        Rectangle player1 = new Rectangle(100, 0, 20, 20);
        Rectangle player2 = new Rectangle(300, 0, 20, 20);

        bool wPressed = false;
        bool sPressed = false;

        bool upPressed = false;
        bool downPressed = false;

        bool escPressed = false;

        int playerSpeed = 5;
        int asteroidSize = 5;

        int p1Score = 0;
        int p2Score = 0;

        List<int> asteroidSpeeds = new List<int>();
        List<Rectangle> asteroids = new List<Rectangle>();

        SolidBrush violetBrush = new SolidBrush(Color.Violet);
        SolidBrush blueBrush = new SolidBrush(Color.RoyalBlue);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Pen whitePen = new Pen(Color.White, 10);


        public Form1()
        {
            InitializeComponent();
            player1.Y = this.Height - 20;
            player2.Y = this.Height - 20;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check for when a key is pressed
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = true;
                    break;
                case Keys.S:
                    sPressed = true;
                    break;

                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;

                case Keys.Escape:
                    escPressed = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check for when a key stops being pressed
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = false;
                    break;
                case Keys.S:
                    sPressed = false;
                    break;

                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move p1
            if (wPressed == true)
            {
                player1.Y -= playerSpeed;
            }
            if (sPressed == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += playerSpeed;
            }

            //wrap p1, add a point, play the sound
            if (player1.Y < 0)
            {
                player1.Y = this.Height - player2.Height;
                p1Score += 1;

                wrapPlayer.Play();
            }

            //move p2
            if (upPressed == true)
            {
                player2.Y -= playerSpeed;
            }

            if (downPressed == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y += playerSpeed;
            }

            //wrap p2, add a point, and play the sound
            if (player2.Y < 0)
            {
                player2.Y = this.Height - player2.Height;
                p2Score += 1;

                wrapPlayer.Play();
            }
            
            
            //spawn asteroid, decide speed and position
            
            int randValue = randGen.Next(0, 101);

            if (randValue < 10)
            {
                int yGen;
                yGen = randGen.Next(0, this.Height - 50);

                Rectangle newAsteroid = new Rectangle(0, yGen, asteroidSize, asteroidSize);
                asteroids.Add(newAsteroid);
                asteroidSpeeds.Add(randGen.Next(4, 16));
            }

            else if (randValue < 20)
            {
                int yGen;
                yGen = randGen.Next(0, this.Height - 50);

                Rectangle newAsteroid = new Rectangle(this.Width - asteroidSize, yGen, asteroidSize, asteroidSize);
                asteroids.Add(newAsteroid);

                int asteroidSpeed = randGen.Next(4, 10);

                asteroidSpeeds.Add(-asteroidSpeed);
            }
            
            //move the asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                int asteroidX = asteroids[i].X + asteroidSpeeds[i];
                asteroids[i] = new Rectangle(asteroidX, asteroids[i].Y, asteroidSize, asteroidSize);

                if (player1.IntersectsWith(asteroids[i]))
                {
                    asteroids.Remove(asteroids[i]);
                    asteroidSpeeds.Remove(asteroidSpeeds[i]);
                    //play sound
                    expPlayer.Play();

                    //reset player position
                    player1.Y = this.Height - 20;
                }

                if (player2.IntersectsWith(asteroids[i]))
                {
                    asteroids.Remove(asteroids[i]);
                    asteroidSpeeds.Remove(asteroidSpeeds[i]);
                    //play sound
                    expPlayer.Play();

                    //reset player position
                    player2.Y = this.Height - 20;
                }
            }


            //remove from beyond borders
            for (int i = 0; i < asteroids.Count; i++)
            {
                if (asteroids[i].X < 0 || asteroids[i].X > this.Width)
                {
                    asteroids.Remove(asteroids[i]);
                    asteroidSpeeds.Remove(asteroidSpeeds[i]);
                   
                }

            }

            //end the game when someone wins
            if (p1Score == 3)
            {
                victoryPlayer.Play();
                gameTimer.Stop();
                victoryLabel.Text = "player 1 wins!";
                Refresh();
                Thread.Sleep(10000);
                Close();
            }

            if (p2Score == 3)
            {
                victoryPlayer.Play();
                gameTimer.Stop();
                victoryLabel.Text = "player 2 wins!";
                Refresh();
                Thread.Sleep(10000);
                Close();
            }
            //redraw the screen
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw the players
            e.Graphics.FillRectangle(violetBrush, player1);
            e.Graphics.FillRectangle(blueBrush, player2);

            //draw relevant variables
            player1ScoreLabel.Text = $"{p1Score}";
            player2ScoreLabel.Text = $"{p2Score}";

            //draw the asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                e.Graphics.FillRectangle(redBrush, asteroids[i]);
            }
        }
    }
}
