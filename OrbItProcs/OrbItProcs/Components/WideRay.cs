﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace OrbItProcs.Components
{
    public class WideRay : Component
    {
        //public Queue<Vector2> positions;
        //public Queue<float> angles;
        //public Queue<float> scales;
        private int _queuecount = 10;
        public int queuecount { get { return _queuecount; } set { _queuecount = value; } }

        private int timer = 0, _timerMax = 1;
        public int timerMax { get { return _timerMax; } set { _timerMax = value; } }

        public double angle = 0;
        public float rayscale = 20;
        public int width = 3;

        public WideRay() : this(null) { }
        public WideRay(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            com = comp.wideray;
            methods = mtypes.affectself | mtypes.draw;
            InitializeLists();  
        }

        public override void AfterCloning()
        {
            if (!parent.comps.ContainsKey(comp.queuer)) parent.addComponent(comp.queuer, true);
            //if (parent.comps.ContainsKey(comp.queuer)) 
            parent.comps[comp.queuer].qs = parent.comps[comp.queuer].qs | queues.scale | queues.position | queues.angle;
            //int i = 0;
        }

        public override void InitializeLists()
        {
            //positions = new Queue<Vector2>();
            //angles = new Queue<float>();
            //scales = new Queue<float>();
        }

        public override void Initialize(Node parent)
        {
            this.parent = parent;
            
        }

        public override void AffectOther(Node other)
        {

        }
        public override void AffectSelf()
        {
            /*
            angle = Math.Atan2(parent.velocity.Y, parent.velocity.X) +(Math.PI / 2);

            timer++;
            if (timer % timerMax == 0)
            {
                if (positions.Count < queuecount)
                {
                    positions.Enqueue(parent.position);
                    angles.Enqueue((float)angle);
                    scales.Enqueue((float)parent.scale);
                }
                else
                {
                    positions.Dequeue();
                    positions.Enqueue(parent.position);
                    angles.Dequeue();
                    angles.Enqueue((float)angle);
                    scales.Dequeue();
                    scales.Enqueue((float)parent.scale);
                }
            }
            */

        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Room room = parent.room;
            float mapzoom = room.mapzoom;

            Queue<float> scales = parent.comps[comp.queuer].scales;
            Queue<float> angles = parent.comps[comp.queuer].angles;
            Queue<Vector2> positions = ((Queue<Vector2>)(parent.comps[comp.queuer].positions));
            

            Vector2 screenPos = parent.position / mapzoom;
            Vector2 centerTexture = new Vector2(0.5f, 0.5f);

            int count = 0;
            Vector2 scalevect = new Vector2(rayscale, width);
            foreach (Vector2 pos in positions)
            {
                scalevect.X = scales.ElementAt(count) * 50;
                spritebatch.Draw(parent.getTexture(textures.whitepixel), pos/mapzoom, null, parent.color, angles.ElementAt(count), centerTexture, scalevect, SpriteEffects.None, 0);
                count++;
            }

            float testangle = (float)(Math.Atan2(parent.velocity.Y, parent.velocity.X) + (Math.PI / 2));
            scalevect.X = parent.scale * 50;
            spritebatch.Draw(parent.getTexture(textures.whitepixel), parent.position / mapzoom, null, parent.color, testangle, centerTexture, scalevect, SpriteEffects.None, 0);
            
        }

        public void onCollision(Dictionary<dynamic, dynamic> args)
        {
        }

    }
}
