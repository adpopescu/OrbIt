﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace OrbItProcs {
    public static class Utils {

        
        /*
        public static void cloneObject<T>(T obj, T newobj) //they must be the same type
        {
            //dynamic returnval;
            List<FieldInfo> fields = obj.GetType().GetFields().ToList();
            List<PropertyInfo> properties = obj.GetType().GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(newobj, property.GetValue(obj, null), null);
            }
            foreach (FieldInfo field in fields)
            {
                //Console.WriteLine("fieldtype: " + field.FieldType);
                if (field.FieldType.ToString().Contains("Dictionary"))
                {
                    //Console.WriteLine(field.Name + " is a dictionary.");
                    if (field.FieldType.ToString().Contains("[System.Object,System.Boolean]"))//must be props
                    {
                        //Console.WriteLine("PROPS");
                        dynamic node = obj;
                        
                        Dictionary<dynamic, bool> dict = node.props;
                        Dictionary<dynamic, bool> newdict = new Dictionary<dynamic, bool>();
                        foreach (dynamic key in dict.Keys)
                        {
                            newdict.Add(key, dict[key]);
                        }
                        field.SetValue(newobj, newdict);

                    }
                    else if (field.FieldType.ToString().Contains("[OrbItProc.comp,System.Object]"))//must be comps
                    {
                        //Console.WriteLine("COMPS");
                        dynamic node = obj;
                        Dictionary<comp,dynamic> dict = node.comps;
                        dynamic newNode = newobj;
                        foreach (comp key in dict.Keys)
                        {
                            newNode.addComponent(key, true);
                            cloneObject<Component>(dict[key], newNode.comps[key]);
                            
                        }
                        //Console.WriteLine(newNode.comps[comp.randinitialvel].multiplier);
                    }
                }
                else if ((field.FieldType.ToString().Equals("System.Int32"))
                    || (field.FieldType.ToString().Equals("System.Single"))
                    || (field.FieldType.ToString().Equals("System.Boolean"))
                    || (field.FieldType.ToString().Equals("System.String")))
                {
                    field.SetValue(newobj, field.GetValue(obj));
                }
                else if (field.FieldType.ToString().Contains("Vector2"))
                {
                    Vector2 vect = (Vector2)field.GetValue(obj);
                    Vector2 newvect = new Vector2(vect.X, vect.Y);
                    field.SetValue(newobj, newvect);
                }
                else if (field.Name.Equals("parent"))
                {
                    //do nothing
                }
                else
                { 
                    //this would be an object field
                    //Console.WriteLine(field.Name + " is an object of some kind.");
                    if (field.Name.Equals("room") || field.Name.Equals("texture"))
                    {
                        field.SetValue(newobj, field.GetValue(obj));
                    }
                }

                //field.SetValue(newobj, field.GetValue(obj));
            }
            
        }
        */

        public static Random random = new Random((int)DateTime.Now.Millisecond);

        public static Color randomColor()
        {
            return new Color((float)Utils.random.Next(255) / (float)255, (float)Utils.random.Next(255) / (float)255, (float)Utils.random.Next(255) / (float)255);
        }

        public static void printDictionary(Dictionary<dynamic,dynamic> dict, string s = "")
        {
            if (dict == null)
            { //Console.WriteLine("Dict is null"); return; }
            }
            Console.WriteLine(s);
            foreach (KeyValuePair<dynamic, dynamic> kvp in dict)
            {
                //Console.WriteLine("Key = {0}, Value = {1}",
                //    kvp.Key, kvp.Value);
            }
        }

        public static void ensureContains(Dictionary<dynamic, dynamic> props, Dictionary<dynamic, dynamic> defProps)
        {
            foreach (dynamic p in defProps.Keys)
            {
                if (!props.ContainsKey(p)) props.Add(p, defProps[p]);
                else props[p] = props[p] ?? defProps[p];
            }
            
        }

        public static bool checkCollision(Node o1, Node o2)
        {

            if (Vector2.DistanceSquared(o1.position, o2.position) <= ((o1.radius + o2.radius) * (o1.radius + o2.radius)))
            {
                return true;
            }
            return false;
        }

        public static void resolveCollision(Node o1, Node o2)
        {

            /*Console.WriteLine("Collision Occured.");
            o1.IsActive = false;
            o2.IsActive = false;
             */

            //Console.WriteLine(o1.mass + " " + o2.mass);
            
            //ELASTIC COLLISION RESOLUTION --- FUCK YEAH
            //float orbimass = 1, orbjmass = 1;
            //float orbRadius = 25.0f; //integrate this into the orb class
            float distanceOrbs = (float)Vector2.Distance(o1.position, o2.position);
            if (distanceOrbs < 10) distanceOrbs = 10; //prevent /0 error
            Vector2 normal = (o2.position - o1.position) / distanceOrbs;
            float pvalue = 2 * (o1.velocity.X * normal.X + o1.velocity.Y * normal.Y - o2.velocity.X * normal.X - o2.velocity.Y * normal.Y) / (o1.mass + o2.mass);
            //if (!test) 
            //return;
            o1.velocity.X = o1.velocity.X - pvalue * normal.X * o2.mass;
            o1.velocity.Y = o1.velocity.Y - pvalue * normal.Y * o2.mass;
            o2.velocity.X = o2.velocity.X + pvalue * normal.X * o1.mass;
            o2.velocity.Y = o2.velocity.Y + pvalue * normal.Y * o1.mass;

            float loss1 = 0.99999f;
            float loss2 = 0.99999f;
            //o1.velocity *= loss1;
            //o2.velocity *= loss2;

            //if (game1.fixCollisionOn)
            fixCollision(o1, o2);


        }

        //make sure that if the orbs are stuck together, they are separated.
        public static void fixCollision(Node o1, Node o2)
        {
            //float orbRadius = 25.0f; //integrate this into the orb class
            //if the orbs are still within colliding distance after moving away (fix radius variables)
            //if (Vector2.DistanceSquared(o1.position + o1.velocity, o2.position + o2.velocity) <= ((o1.radius * 2) * (o2.radius * 2)))
            if (Vector2.DistanceSquared(o1.position + o1.velocity, o2.position + o2.velocity) <= ((o1.radius + o2.radius) * (o1.radius + o2.radius)))
            {

                Vector2 difference = o1.position - o2.position; //get the vector between the two orbs
                float length = Vector2.Distance(o1.position, o2.position);//get the length of that vector
                difference = difference / length;//get the unit vector
                //fix the below statement to get the radius' from the orb objects
                length = (o1.radius + o2.radius) - length; //get the length that the two orbs must be moved away from eachother
                difference = difference * length; // produce the vector from the length and the unit vector
                if (o1.comps.ContainsKey(comp.movement) && o1.comps[comp.movement].pushable
                    && o2.comps.ContainsKey(comp.movement) && o2.comps[comp.movement].pushable)
                {
                    o1.position += difference / 2;
                    o2.position -= difference / 2;
                }
                else if (o1.comps.ContainsKey(comp.movement) && !o1.comps[comp.movement].pushable)
                {
                    o2.position -= difference;
                }
                else if (o2.comps.ContainsKey(comp.movement) && !o2.comps[comp.movement].pushable)
                {
                    o1.position += difference;
                }
            }
            else return;
        }



    } // end of class
}
