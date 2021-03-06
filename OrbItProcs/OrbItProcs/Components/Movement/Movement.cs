﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System.Reflection;



namespace OrbItProcs
{
    public enum movemode
    {
        free,
        wallbounce,
        screenwrap,
        falloff,
    };

    public class Movement : Component {

        private bool _pushable = true;
        public bool pushable { get { return _pushable; } set { _pushable = value; } }

        private float _VelocityModifier = 1f;
        public float VelocityModifier { get { return _VelocityModifier; } set { _VelocityModifier = value; } }

        public Vector2 tempPosition = new Vector2(0, 0);
        
        private movemode _movementmode = movemode.wallbounce;
        public movemode movementmode { get { return _movementmode; } set { _movementmode = value; } }

        public Movement() : this(null) { }
        public Movement(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            com = comp.movement; 
            methods = mtypes.affectself;

        }

        public override void Initialize(Node parent)
        {
            this.parent = parent;
            //movementmode = movemode.wallbounce;
        }


        public override void AffectSelf()
        {
            //if (parent.comps[comp.collision].active == false) return;

            parent.transform.effvelocity = parent.transform.position - tempPosition;
            tempPosition = parent.transform.position;

            parent.transform.position.X += parent.transform.velocity.X * VelocityModifier;
            parent.transform.position.Y += parent.transform.velocity.Y * VelocityModifier;

            //test (holy SHIT that looks cool)
            //PropertyInfo pi = parent.GetType().GetProperty("scale");
            //pi.SetValue(parent, parent.transform.position.X % 4.0f, null);

            if (movementmode == movemode.screenwrap) screenWrap();
            if (movementmode == movemode.wallbounce) wallBounce();
            if (movementmode == movemode.falloff)    fallOff();
            
        }

        public override void Draw(SpriteBatch spritebatch)
        {
        }

        public void fallOff()
        {
            int levelwidth = parent.room.game.worldWidth;
            int levelheight = parent.room.game.worldHeight;

            Vector2 pos = parent.transform.position;

            if (parent.comps.ContainsKey(comp.queuer) && (parent.comps[comp.queuer].qs & queues.position) == queues.position)
            {
                Queue<Vector2> positions = ((Queue<Vector2>)(parent.comps[comp.queuer].positions));
                pos = positions.ElementAt(0);
            }


            if (pos.X >= (levelwidth + parent.transform.radius))
            {
                parent.IsDeleted = true;
            }
            else if (pos.X < parent.transform.radius * -1)
            {
                parent.IsDeleted = true;
            }


            if (pos.Y >= (levelheight + parent.transform.radius))
            {
                parent.IsDeleted = true;
            }
            else if (pos.Y < parent.transform.radius * -1)
            {
                parent.IsDeleted = true;
            }
        }

        public void checkForQueue()
        {
            
        }

        public void wallBounce()
        {
            //if (room.PropertiesDict["wallBounce"])
            //float levelwidth = room.game...;
            int levelwidth = parent.room.game.worldWidth;
            int levelheight = parent.room.game.worldHeight;



            if (parent.transform.position.X >= (levelwidth - parent.transform.radius))
            {
                parent.transform.position.X = levelwidth - parent.transform.radius;
                parent.transform.velocity.X *= -1;
                parent.OnCollidePublic(null);

            }
            if (parent.transform.position.X < parent.transform.radius)
            {
                parent.transform.position.X = parent.transform.radius;
                parent.transform.velocity.X *= -1;
                parent.OnCollidePublic(null);
            }
            if (parent.transform.position.Y >= (levelheight - parent.transform.radius))
            {
                parent.transform.position.Y = levelheight - parent.transform.radius;
                parent.transform.velocity.Y *= -1;
                parent.OnCollidePublic(null);
            }
            if (parent.transform.position.Y < parent.transform.radius)
            {
                parent.transform.position.Y = parent.transform.radius;
                parent.transform.velocity.Y *= -1;
                parent.OnCollidePublic(null);
            }


        }

        public void screenWrap()
        {
            //if (room.PropertiesDict["wallBounce"])
            //float levelwidth = room.game...;
            int levelwidth = parent.room.game.worldWidth;
            int levelheight = parent.room.game.worldHeight;

            //hitting top/bottom of screen
            //teleport node
            if (parent.transform.position.X >= levelwidth)
            {
                parent.transform.position.X = 1;
            }
            else if (parent.transform.position.X < 0)
            {
                parent.transform.position.X = levelwidth - 1;
            }
            //show half texture on other side
            if (parent.transform.position.X >= (levelwidth - parent.transform.radius))
            {
                //
            }
            else if (parent.transform.position.X < parent.transform.radius)
            {
                //
            }

            //hitting sides
            //teleport node
            if (parent.transform.position.Y >= levelheight)
            {
                parent.transform.position.Y = 1;
            }
            else if (parent.transform.position.Y < 0)
            {
                parent.transform.position.Y = levelheight - 1;
            }
            //show half texture on other side
            if (parent.transform.position.Y >= (levelheight - parent.transform.radius))
            {
                //
            }
            else if (parent.transform.position.Y < parent.transform.radius)
            {
                //
            }



        }

    }
}
