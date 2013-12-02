﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
namespace OrbItProcs.Components
{
    public class MaxVel : Component {

        private float _maxvel = 100f;
        public float maxvel { get { return _maxvel; } set { _maxvel = value; } }

        public MaxVel() : this(null) { }
        public MaxVel(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            com = comp.movement; 
            methods = mtypes.affectself; 
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
            if ((Math.Pow(parent.velocity.X, 2) + Math.Pow(parent.velocity.Y, 2)) > Math.Pow(maxvel, 2))
            {
                parent.velocity.Normalize();
                parent.velocity *= maxvel;
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {

        }
    }
}