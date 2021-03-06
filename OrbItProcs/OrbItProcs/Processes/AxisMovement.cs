﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    public class AxisMovement : Process
    {
        public Player player { get; set; }
        public float speed { get; set; }

        public bool constantMovement { get; set; }

        public AxisMovement(Player player, float speed = 10f) : base()
        {
            this.player = player;
            this.speed = speed;
            constantMovement = true;

            addProcessKeyAction("w", KeyCodes.W, OnHold: MoveW);
            addProcessKeyAction("s", KeyCodes.S, OnHold: MoveS);
            addProcessKeyAction("d", KeyCodes.D, OnHold: MoveD);
            addProcessKeyAction("a", KeyCodes.A, OnHold: MoveA);
            addProcessKeyAction("x", KeyCodes.X, OnPress: ToggleMode);

            
        }

        public void ToggleMode()
        {
            constantMovement = !constantMovement;
        }

        public void MoveW()
        {
            if (constantMovement)
                player.transform.position.Y -= speed;
            else
            {
                
            }
        }
        public void MoveS()
        {
            if (constantMovement)
                player.transform.position.Y += speed;
            else
            {

            }
        }
        public void MoveD()
        {
            if (constantMovement)
                player.transform.position.X += speed;
            else
            {

            }
        }
        public void MoveA()
        {
            if (constantMovement)
                player.transform.position.X -= speed;
            else
            {

            }

        }

    }
}
