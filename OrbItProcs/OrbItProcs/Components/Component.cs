﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;



using Component = OrbItProcs.Component;

namespace OrbItProcs
{
    [Flags]
    public enum mtypes
    {
        none = 0,
        initialize = 1,
        affectother = 2,
        affectself = 4,
        draw = 8,

        changereference = 16,
        minordraw = 32,
    };


    public abstract class Component {

        protected bool _active = false;
        public virtual bool active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                if (parent != null && parent.comps.ContainsKey(com))
                {
                    parent.triggerSortLists();
                }
            }
        }

        public int sentinel = -10;
        protected Node _parent;
        //*
        [Polenter.Serialization.ExcludeFromSerialization]
        public Node parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        //*/
        //flag as not editable in InspectorBox
        private comp _com;
        public comp com { get { return _com; } protected set { _com = value; } }
        private mtypes _methods;
        //flag as not editable in InspectorBox
        private bool _CallDraw = true;
        public bool CallDraw { get { return _CallDraw; } set { _CallDraw = value; } }
        public mtypes methods { get { return _methods; } protected set { _methods = value; } }

        public HashSet<Node> exclusions = new HashSet<Node>();

        public virtual void Initialize(Node parent) { this.parent = parent; }
        public virtual void AfterCloning() { }
        public virtual void OnSpawn() { }
        public virtual void AffectOther(Node other) { }
        public abstract void AffectSelf();
        public abstract void Draw(SpriteBatch spritebatch);

        public virtual void InitializeLists()
        { 
       
        }

        public virtual Texture2D getTexture()
        {
            if (parent != null)
            {
                return parent.getTexture();
            }
            return null;
        }

        
        

       public static void CloneComponent(Component sourceComp, Component destComp)
       {
           List<FieldInfo> fields = sourceComp.GetType().GetFields().ToList();
           fields.AddRange(sourceComp.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList());
           List<PropertyInfo> properties = sourceComp.GetType().GetProperties().ToList();
           foreach (PropertyInfo property in properties)
           {
               if (property.PropertyType == typeof(ModifierInfo)) continue;
               if (property.PropertyType == typeof(Node)) continue;
               //Console.WriteLine(destComp.GetType().ToString() + property.Name);
               property.SetValue(destComp, property.GetValue(sourceComp, null), null);
           }
           foreach (FieldInfo field in fields)
           {
               //Console.WriteLine("fieldtype: " + field.FieldType);
               if (field.FieldType == typeof(Dictionary<string,ModifierInfo>))
               {
                   Modifier mod = (Modifier) sourceComp;

                   Dictionary<string, ModifierInfo> newmodinfos = new Dictionary<string, ModifierInfo>();
                   foreach (KeyValuePair<string, ModifierInfo> kvp in mod.modifierInfos)
                   {
                       string key = kvp.Key;
                       ModifierInfo modifierInfo = kvp.Value;
                       Dictionary<string, FPInfo> newFpInfos = new Dictionary<string, FPInfo>();
                       Dictionary<string, object> newFpInfosObj = new Dictionary<string, object>();
                       foreach (string key2 in modifierInfo.fpInfos.Keys)
                       {
                           FPInfo fpinfo = new FPInfo(modifierInfo.fpInfos[key2]);

                           newFpInfos.Add(key2, fpinfo);
                           newFpInfosObj.Add(key2, null);
                       }
                       Dictionary<string, dynamic> newargs = new Dictionary<string, dynamic>();
                       foreach (string key2 in modifierInfo.args.Keys)
                       {
                           newargs.Add(key2, modifierInfo.args[key2]); //by reference (for now)
                       }


                       ModifierInfo modInfo = new ModifierInfo(newFpInfos, newFpInfosObj, newargs, modifierInfo.modifierDelegate);
                       modInfo.delegateName = modifierInfo.delegateName;
                       newmodinfos.Add(key, modInfo);
                   }
                   field.SetValue(destComp, newmodinfos);

               }

               if (field.FieldType.ToString().Contains("Dictionary"))
               {
                   //Console.WriteLine(field.Name + " is a dictionary.");
               }
               else if ((field.FieldType.ToString().Equals("System.Int32"))
                   || (field.FieldType.ToString().Equals("System.Single"))
                   || (field.FieldType.ToString().Equals("System.Boolean"))
                   || (field.FieldType.ToString().Equals("System.String")))
               {
                   field.SetValue(destComp, field.GetValue(sourceComp));
               }
               else if (field.FieldType.ToString().Equals("Microsoft.Xna.Framework.Vector2"))
               {
                   //Console.WriteLine("VECTOR: {0}", field.FieldType.ToString());
                   Vector2 vect = (Vector2)field.GetValue(sourceComp);
                   Vector2 newvect = new Vector2(vect.X, vect.Y);
                   field.SetValue(destComp, newvect);
               }
               else if (field.FieldType.ToString().Equals("Microsoft.Xna.Framework.Color"))
               {
                   Color col = (Color)field.GetValue(sourceComp);
                   Color newcol = new Color(col.R, col.G, col.B, col.A);
                   field.SetValue(destComp, newcol);
               }
               else if (field.Name.Equals("parent"))
               {
                   //do nothing, we don't want to overwrite the reference to the new parent
               }
               else
               {
                   //this would be an object field
                   //Console.WriteLine(field.Name + " is an object of some kind.");
                   if (field.Name.Equals("room") || field.Name.Equals("texture"))
                   {
                       field.SetValue(destComp, field.GetValue(sourceComp));
                   }

                   //NO IDEA IF THIS WILL FUCK SHIT UP
                   //field.SetValue(destComp, field.GetValue(sourceComp));
               }

               

               //field.SetValue(newobj, field.GetValue(obj));
           }
           destComp.InitializeLists();
       }
    }

    
}
