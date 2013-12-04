﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TomShane.Neoforce.Controls;
using System.Reflection;

using OrbItProcs;
using OrbItProcs.Processes;

using Component = OrbItProcs.Components.Component;
using System.IO;

namespace OrbItProcs.Interface {
    public class UserInterface {

        #region /// Essentials ///

        public Game1 game;
        public Room room;

        #endregion

        #region /// Hardware Variables ///

        
        KeyboardState oldKeyBState;
        MouseState oldMouseState;
        

        #endregion

        #region /// ToBeRemoved ///

        string currentSelection = "placeNode";//
        int oldMouseScrollValue = 0;//
        bool hovertargetting = false;//
        int rightClickCount = 0;//
        int rightClickMax = 1;//
        public int sWidth = 1000;////
        public int sHeight = 600;////
        bool isShiftDown = false;
        bool isTargeting = false;
        public Vector2 spawnPos;



        #endregion


        public Node editNode, spawnerNode;
        public Sidebar sidebar;

        public UserInterface(Game1 game)
        {
            this.game = game;
            this.room = game.room;
            

            Initialize();
        }

        public void Initialize()
        {
            sidebar = new Sidebar(game, this);
            sidebar.Initialize();



        }

        


        public void Update(GameTime gameTime)
        {
            ProcessKeyboard();
            ProcessMouse();

        }

        public void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();

            if (keybState.IsKeyDown(Keys.Y))
            {
                hovertargetting = true;
            }
            else
                hovertargetting = false;

            if (keybState.IsKeyDown(Keys.D1))
                currentSelection = "placeNode";
            if (keybState.IsKeyDown(Keys.Q))
                currentSelection = "targeting";


            if (keybState.IsKeyDown(Keys.LeftShift))
            {
                if (!isShiftDown)
                { 
                    MouseState ms = Mouse.GetState();
                    spawnPos = new Vector2(ms.X * room.mapzoom, ms.Y * room.mapzoom);
                }
                isShiftDown = true;
            }
            else
            {
                isShiftDown = false;
            }




            if (keybState.IsKeyDown(Keys.F) && currentSelection.Equals("pause") && !oldKeyBState.IsKeyDown(Keys.F))
                currentSelection = "placeNode";
            else if (keybState.IsKeyDown(Keys.F) && !oldKeyBState.IsKeyDown(Keys.F))
                currentSelection = "pause";

            oldKeyBState = Keyboard.GetState();
        }

        public void ProcessMouse()
        {
            MouseState mouseState = Mouse.GetState();
            //ignore mouse clicks outside window
            if (mouseState.X >= sWidth || mouseState.X < 0 || mouseState.Y >= sHeight || mouseState.Y < 0)
                return;

            //make sure clicks inside the ui are ignored by game logic
            if (mouseState.X >= sWidth - sidebar.Width - 5)
            {
                if (mouseState.Y > sidebar.lstMain.Top + 24 && mouseState.Y < sidebar.lstMain.Top + sidebar.lstMain.Height + 24)
                {
                    if (mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
                    {
                        //lstMain.
                        sidebar.lstMain_ChangeScrollPosition(4);

                    }
                    else if (mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
                    {
                        sidebar.lstMain_ChangeScrollPosition(-4);
                    }
                }
                if (mouseState.Y > sidebar.lstComp.Top + 24 && mouseState.Y < sidebar.lstComp.Top + sidebar.lstComp.Height + 24)
                {
                    if (mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
                    {
                        //lstMain.
                        sidebar.lstComp_ChangeScrollPosition(4);

                    }
                    else if (mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
                    {
                        sidebar.lstComp_ChangeScrollPosition(-4);
                    }


                }

                oldMouseState = mouseState;
                return;
            }

            int worldMouseX = (int)(mouseState.X * room.mapzoom);
            int worldMouseY = (int)(mouseState.Y * room.mapzoom);

            if (isTargeting)
            {
                
            }


            if (currentSelection.Equals("placeNode"))
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    //new node
                    
                        game.spawnNode(worldMouseX, worldMouseY);
                    

                }
                // rapid placement of nodes
                if (mouseState.RightButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    if (isShiftDown)
                    {
                        rightClickCount++;
                        if (rightClickCount % rightClickMax == 0)
                        {
                            //Vector2 positionToSpawn = new Vector2(Game1.sWidth, Game1.sHeight);
                            Vector2 positionToSpawn = spawnPos;
                            //positionToSpawn /= (game.room.mapzoom * 2);
                            //positionToSpawn /= (2);
                            Vector2 diff = new Vector2(mouseState.X, mouseState.Y);
                            diff *= room.mapzoom;
                            diff = diff - positionToSpawn;
                            //diff.Normalize();

                            //new node(s)
                            Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
                                { node.position, positionToSpawn },
                                { node.velocity, diff },
                                //{ node.texture, textures.whitecircle },
                                //{ node.radius, 12 },
                                { comp.randcolor, true },
                                { comp.movement, true },
                                //{ comp.randvelchange, true },
                                { comp.randinitialvel, true },
                                //{ comp.gravity, false },
                                { comp.lifetime, true },
                                //{ comp.transfer, true },
                                //{ comp.lasertimers, true },
                                //{ comp.laser, true },
                                //{ comp.wideray, true },
                                //{ comp.hueshifter, true },
                                //{ comp.phaseorb, true },
                                //{ comp.collision, false },
                            };

                            game.spawnNode(userP);
                            rightClickCount = 0;
                        }
                    }
                    else
                    {
                        if (rightClickCount > rightClickMax)
                        {
                            //new node(s)
                            int rad = 100;
                            for (int i = 0; i < 10; i++)
                            {
                                int rx = Utils.random.Next(rad * 2) - rad;
                                int ry = Utils.random.Next(rad * 2) - rad;
                                game.spawnNode(worldMouseX + rx, worldMouseY + ry);
                            }

                            rightClickCount = 0;
                        }
                        else
                        {
                            rightClickCount++;
                        }
                    }

                }
            }
            else if (currentSelection.Equals("targeting"))
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    bool found = false;
                    for (int i = room.nodes.Count - 1; i >= 0; i--)
                    {
                        Node n = (Node)room.nodes.ElementAt(i);
                        // find node that has been clicked, starting from the most recently placed nodes
                        if (Vector2.DistanceSquared(n.position, new Vector2(worldMouseX, worldMouseY)) < n.radius * n.radius)
                        {
                            //--
                            //room.nodes.Remove(n);
                            //break;
                            //--
                            //game.targetNode = n;
                            sidebar.SetTargetNode(n);
                            //room.processManager.Add(new TripSpawnOnCollide(game.targetNode));
                            found = true;
                            break;
                        }
                    }
                    if (!found) game.targetNode = null;
                }
                else if (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released)
                {
                    //bool found = false;
                    for (int i = room.nodes.Count - 1; i >= 0; i--)
                    {
                        Node n = (Node)room.nodes.ElementAt(i);
                        // find node that has been clicked, starting from the most recently placed nodes
                        if (Vector2.DistanceSquared(n.position, new Vector2(worldMouseX, worldMouseY)) < n.radius * n.radius)
                        {
                            //--
                            //room.nodes.Remove(n);
                            //break;
                            //--
                            if (game.targetNode != null && game.targetNode.comps.ContainsKey(comp.flow))
                            {
                                game.targetNode.comps[comp.flow].AddToOutgoing(n);
                                sidebar.SetTargetNode(n);
                                break;
                            }
                            //found = true;

                        }
                    }
                    //if (!found) game.targetNode = null;
                }
                else if (mouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released)
                {
                    //bool found = false;
                    for (int i = room.nodes.Count - 1; i >= 0; i--)
                    {
                        Node n = (Node)room.nodes.ElementAt(i);
                        // find node that has been clicked, starting from the most recently placed nodes
                        if (Vector2.DistanceSquared(n.position, new Vector2(worldMouseX, worldMouseY)) < n.radius * n.radius)
                        {
                            //--
                            //room.nodes.Remove(n);
                            //break;
                            //--
                            if (n.comps.ContainsKey(comp.flow))
                            {
                                n.comps[comp.flow].activated = !n.comps[comp.flow].activated;
                                break;
                            }
                            //found = true;

                        }
                    }
                    //if (!found) game.targetNode = null;
                }
                //oldMouseScrollValue = mouseState.ScrollWheelValue;
                oldMouseState = mouseState;
                return;
            }
            


            if (hovertargetting)
            {
                if (true || mouseState.LeftButton == ButtonState.Pressed)
                {
                    bool found = false;
                    for (int i = room.nodes.Count - 1; i >= 0; i--)
                    {
                        Node n = (Node)room.nodes.ElementAt(i);
                        // find node that has been clicked, starting from the most recently placed nodes
                        if (Vector2.DistanceSquared(n.position, new Vector2(worldMouseX, worldMouseY)) < n.radius * n.radius)
                        {
                            game.targetNode = n;
                            found = true;
                            break;
                        }
                    }
                    if (!found) game.targetNode = null;
                }
            }

            if (mouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed)
            {
                rightClickCount = 0;
            }

            if (mouseState.ScrollWheelValue < oldMouseScrollValue)
            {
                room.mapzoom += 0.2f;
            }
            else if (mouseState.ScrollWheelValue > oldMouseScrollValue)
            {
                room.mapzoom -= 0.2f;
            }

            oldMouseScrollValue = mouseState.ScrollWheelValue;
            oldMouseState = mouseState;
        }

        //DEPRECIATED
        //void btnSaveNode_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    PopupWindow saveNodes = new PopupWindow(game, "saveNode");

        //}


    }
}
/*
    List<Y> listOfY = listOfX.Cast<Y>().ToList() 
*/
