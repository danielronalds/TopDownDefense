﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace TopDownDefense
{
    public partial class Form1 : Form
    {
        Graphics g;

        Random random = new Random();

        Angles angle = new Angles();

        Crystal crystal = new Crystal();

        // Spawn Points
        Point TopLeftCorner;
        Point TopRightCorner;
        Point BottomLeftCorner;
        Point BottomRightCorner;

        int MaxEnemies = 8;

        public List<Enemy> enemies = new List<Enemy>();

        public List<AmmoPack> ammopacks = new List<AmmoPack>();

        Player player = new Player(300,300,1,0);

        bool playerLeft, playerRight, playerUp, playerDown, playerFire;

        Point mouse;

        public Form1()
        {
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, Canvas, new object[] { true });

            ConfigureSpawnPoints();
        }

        private void ConfigureSpawnPoints()
        {
            TopLeftCorner = new Point(0 - 50, 0 - 50);
            TopRightCorner = new Point(Canvas.Width + 50, 0 - 50);
            BottomLeftCorner = new Point(0 - 50, Canvas.Height + 50);
            BottomRightCorner = new Point(Canvas.Width + 50, Canvas.Height + 50 );
        }


        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            checkCollisions(g);

            crystal.DrawCrystal(g);

            foreach(AmmoPack a in ammopacks)
            {
                a.drawAmmoPack(g);
            }

            player.DrawPlayer(g, mouse, playerFire);
            foreach (Projectile p in player.projectiles)
            {
                p.drawProjectile(g);
                p.moveProjectile(g);
            }

            EnemySpawnManagement();

            foreach (Enemy enemy in enemies)
            {
                enemy.moveEnemy(g, crystal.crystalRec, player.playerRec);
                enemy.DrawEnemy(g);
            }
        }

        private void updateTmr_Tick(object sender, EventArgs e)
        {
            player.MovePlayer(playerLeft, playerRight, playerUp, playerDown);
            Console.WriteLine("Mouse X = " + mouse.X + " Mouse Y = " + mouse.Y + " Angle = " + angle.CalculateAngle(player.rifleBarrel(), mouse));
            Canvas.Invalidate();
        }

        private void EnemySpawnManagement()
        {

            if (enemies.Count() < MaxEnemies)
            {
                Point EnemySpawnPoint;
                String EnemyObjective;


                if (random.Next(1, 100) <= 25)
                {
                    EnemyObjective = "Crystal";
                }
                else
                {
                    EnemyObjective = "Player";
                }

                int RandomSpawnSelection = random.Next(1, 4);

                switch (RandomSpawnSelection)
                {
                    case 1:
                        EnemySpawnPoint = TopLeftCorner;
                        break;

                    case 2:
                        EnemySpawnPoint = TopRightCorner;
                        break;

                    case 3:
                        EnemySpawnPoint = BottomLeftCorner;
                        break;

                    case 4:
                        EnemySpawnPoint = BottomRightCorner;
                        break;
                    default:
                        EnemySpawnPoint = TopLeftCorner;
                        break;
                }


                enemies.Add(new Enemy(EnemySpawnPoint, EnemyObjective));
            }
        }

        private void checkCollisions(Graphics g)
        {
            for (int i = 0; i < player.projectiles.Count(); i++)
            {
                for (int x = 0; x < enemies.Count(); x++)
                {
                    if(enemies[x].enemyRec.IntersectsWith(player.projectiles[i].projectileRec))
                    {
                        player.projectiles.Remove(player.projectiles[i]);

                        enemies[x].health -= player.bulletDamage;
                        enemies[x].enemyHit = true;
                        Console.WriteLine("HIT!!!");

                        if (enemies[x].health <= 0)
                        {
                            int ammoChance = random.Next(1, 100);

                            if(ammoChance <= 35)
                            {
                                ammopacks.Add(new AmmoPack(g, enemies[x].enemyRec.Location, random.Next(1, 3)));
                            }

                            enemies.Remove(enemies[x]);
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < ammopacks.Count(); i++)
            {
                if(player.playerRec.IntersectsWith(ammopacks[i].ammoRec))
                {
                    for (int x = 0; x < ammopacks[i].containedAmmo; x++)
                    {
                        if(player.Ammo < player.MaxAmmo)
                        {
                            player.Ammo++;
                        }
                        else
                        {
                            ammopacks.Remove(ammopacks[i]);
                            break;
                        }
                    }
                }
            }

            for(int x = 0; x < enemies.Count(); x++)
            {
                if(enemies[x].enemyRec.IntersectsWith(player.playerRec))
                {
                    player.Health -= enemies[x].Damage;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mouse = this.PointToClient(Cursor.Position);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    playerFire = true;
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    playerFire = false;
                    break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.A:
                    playerLeft = true;
                    break;

                case Keys.D:
                    playerRight = true;
                    break;

                case Keys.W:
                    playerUp = true;
                    break;

                case Keys.S:
                    playerDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    playerLeft = false;
                    break;

                case Keys.D:
                    playerRight = false;
                    break;

                case Keys.W:
                    playerUp = false;
                    break;

                case Keys.S:
                    playerDown = false;
                    break;
            }
        }

    }
}
