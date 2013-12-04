﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using OrbItProcs.Processes;

namespace OrbItProcs.Components
{
    public class Waver : Component
    {
        public bool _reflective = true;
        public bool reflective { get { return _reflective; } set { _reflective = value; } }
        public Queue<Vector2> metapositions = new Queue<Vector2>();
        public Queue<Vector2> reflectpositions = new Queue<Vector2>();
        public int _queuecount = 10;
        public int queuecount { get { return _queuecount; } set { _queuecount = value;  } }

        public float _amp = 100;
        public float amp
        {
            get { return _amp; } 
            set 
            {
                _amp = value;
                DelegateManager.ChangeArg(parent, "waver", "amp", value);

            } 
        }

        

        public float _period = 10;
        public float period
        {
            get { return _period; }
            set
            {
                _period = value;
                DelegateManager.ChangeArg(parent, "waver", "period", value);
            }
        }

        public float _composite = 1;
        public float composite
        {
            get { return _composite; }
            set
            {
                _composite = value;
                DelegateManager.ChangeArg(parent, "waver", "composite", value);
            }
        }


        public Waver() : this(null) { }
        public Waver(Node parent = null)
        {
            if (parent != null)
            {
                this.parent = parent;


            }
            com = comp.waver;
            methods = mtypes.affectself | mtypes.draw;
            //InitializeLists();

        }

        public override void AfterCloning()
        {
            //if (!parent.comps.ContainsKey(comp.queuer)) parent.addComponent(comp.queuer, true);
            if (!parent.comps.ContainsKey(comp.lifetime)) 
                parent.addComponent(comp.lifetime, true);
            if (!parent.comps.ContainsKey(comp.modifier)) 
                parent.addComponent(comp.modifier, true);
            //if (parent.comps.ContainsKey(comp.queuer)) 
            //parent.comps[comp.queuer].qs = parent.comps[comp.queuer].qs | queues.scale | queues.position;

            parent.comps[comp.lifetime].immortal = true;

            //MODIFIER
            ModifierInfo modinfo = new ModifierInfo();
            //modinfo.AddFPInfoFromString("o1", "scale", parent);
            //modinfo.AddFPInfoFromString("m1", "position", parent);
            modinfo.AddFPInfoFromString("v1", "position", parent);
            modinfo.AddFPInfoFromString("m1", "timer", parent.comps[comp.lifetime]);

            //modinfo.args.Add("mod", 4.0f);
            modinfo.args.Add("amp", amp);
            modinfo.args.Add("period", period);
            modinfo.args.Add("composite", composite);
            modinfo.args.Add("vshift", 0.0f);
            modinfo.args.Add("yval", 0.0f);

            //modinfo.delegateName = "Mod";
            //modinfo.delegateName = "Triangle";
            //modinfo.delegateName = "VelocityToOutput";
            //modinfo.delegateName = "VectorSine";
            modinfo.delegateName = "VectorSineComposite";

            parent.comps[comp.modifier].modifierInfos["waver"] = modinfo;


            
        }

        public override void InitializeLists()
        {
            metapositions = new Queue<Vector2>();
            reflectpositions = new Queue<Vector2>();
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
            float yval = 0;
            if (parent.comps[comp.modifier].modifierInfos["waver"].args.ContainsKey("yval"))
            {
                yval = parent.comps[comp.modifier].modifierInfos["waver"].args["yval"];
            }

            Vector2 metapos = new Vector2(parent.velocity.Y, -parent.velocity.X);
            metapos.Normalize();
            metapos *= yval;
            Vector2 metaposfinal = parent.position + metapos;


            if (metapositions.Count > queuecount)
            {
                metapositions.Dequeue();
            }
            metapositions.Enqueue(metaposfinal);

            if (reflective)
            {
                Vector2 reflectfinal = parent.position - metapos;
                if (reflectpositions.Count > queuecount)
                {
                    reflectpositions.Dequeue();
                }
                reflectpositions.Enqueue(reflectfinal);
            }

        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Room room = parent.room;
            float mapzoom = room.mapzoom;

            int count = 0;
            //Queue<float> scales = parent.comps[comp.queuer].scales;
            //Queue<Vector2> positions = ((Queue<Vector2>)(parent.comps[comp.queuer].positions));

            foreach (Vector2 metapos in metapositions)
            {
                spritebatch.Draw(parent.getTexture(), metapos / mapzoom, null, parent.color, 0, parent.TextureCenter(), parent.scale / mapzoom, SpriteEffects.None, 0);
                count++;
            }
            foreach (Vector2 relectpos in reflectpositions)
            {
                spritebatch.Draw(parent.getTexture(), relectpos / mapzoom, null, parent.color, 0, parent.TextureCenter(), parent.scale / mapzoom, SpriteEffects.None, 0);
                count++;
            }

            //spritebatch.Draw(parent.getTexture(), parent.position / mapzoom, null, parent.color, 0, parent.TextureCenter(), parent.scale / mapzoom, SpriteEffects.None, 0);

        }

        public void onCollision(Dictionary<dynamic, dynamic> args)
        {
        }

    }
}
