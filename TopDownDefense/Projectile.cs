﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TopDownDefense
{
    class Projectile
    {
        private int x, y, width, height;
        private int projectileRotated;
        private int projectileSpeed = 45;

        private double xSpeed, ySpeed;

        private Image projectileImage;

        public Rectangle projectileRec;
        public Matrix projectileMatrix;

        Point projectileCentre;

        public Projectile(Rectangle rifleBarrel, int projectileAngle)
        {
            width = 42;
            height = 6;
            x = 20000;
            y = 20000;
            projectileImage = Properties.Resources.bullet_2;
            projectileRec = new Rectangle(x, y, width, height);

            xSpeed = projectileSpeed * (Math.Cos((projectileAngle) * Math.PI / 180));
            ySpeed = projectileSpeed * (Math.Sin((projectileAngle) * Math.PI / 180));

            x = rifleBarrel.X + (rifleBarrel.Width / 2);
            y = rifleBarrel.Y + (rifleBarrel.Height / 2);

            projectileRotated = projectileAngle;
        }

        public void drawProjectile(Graphics g)
        {
            projectileCentre = new Point(x, y);
            projectileMatrix = new Matrix();

            projectileMatrix.RotateAt(projectileRotated, projectileCentre);
            g.Transform = projectileMatrix;
            g.DrawImage(projectileImage, projectileRec);
        }

        public void moveProjectile(Graphics g)
        {
            x += (int)xSpeed;
            y += (int)ySpeed;
            projectileRec.Location = new Point(x, y);
        }
    }
}
