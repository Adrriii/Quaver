﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Quaver.Graphics;
using Quaver.Graphics.Sprite;

namespace Quaver.Gameplay
{
    /// <summary>
    ///     This class Draws anything that will be shown to the player which is related to data
    /// </summary>
    internal class DataInterface
    {
        private static Sprite AccuracyBox { get; set; }

        private static Sprite LeaderboardBox { get; set; }

        private static Boundary Boundary { get; set; }

        internal static void Initialize()
        {
            // Create Boundary
            Boundary = new Boundary();

            // Create new Accuracy Box
            AccuracyBox = new Sprite()
            {
                Alignment = Alignment.TopRight,
                Size = new Vector2(230, 240),
                Parent = Boundary,
                Tint = Color.Black //todo: remove later and use skin image
            };

            // Create new Leaderboard Box
            LeaderboardBox = new Sprite()
            {
                Size = new Vector2(230, 340),
                Alignment = Alignment.MidLeft,
                Parent = Boundary,
                Tint = Color.Black //todo: remove later and use skin image
            };
        }

        internal static void Update(double dt)
        {
            Boundary.Update(dt);   
        }

        internal static void Draw()
        {
            Boundary.Draw();
        }

        internal static void UnloadContent()
        {
            
        }
    }
}