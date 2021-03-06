﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TomShane.Neoforce.Controls;

namespace OrbItProcs
{
    public class LinkGeneratorWindow
    {
        public Manager manager;
        public Sidebar sidebar;
        public Window window;
        public int HeightCounter3;
        //LinkGenerator
        ComboBox cbLinkType, cbLinkPresets, cbLinkPalette, cbLinkFormation;
        Label lblGenerateLink, lblLinkType, lblLinkPresets, lblLinkPalette, lblLinkFormation;
        CheckBox chkEntangled;
        Button btnAddToPalette;

        public LinkGeneratorWindow(Manager manager, Sidebar sidebar)
        {
            Game1 game = Program.getGame();
            game.ui.GameInputDisabled = true;

            this.manager = manager;
            this.sidebar = sidebar;
            window = new Window(manager);
            window.Init();
            window.Left = game.ui.sidebar.master.Left;
            window.Width = game.ui.sidebar.master.Width;
            window.Top = 200;
            window.Height = 200;
            window.Text = "Link Generator";
            window.Closed += delegate { game.ui.GameInputDisabled = false; };
            window.ShowModal();
            manager.Add(window);

            //LinkGenerator.ExpandedHeight += 30;
            HeightCounter3 = 0;
            Window parent2 = window;
            int left = 0;
            int middle = 100;

            lblGenerateLink = new Label(manager);
            lblGenerateLink.Init();
            lblGenerateLink.Left = left + middle / 2;
            lblGenerateLink.Top = HeightCounter3; HeightCounter3 += lblGenerateLink.Height;
            lblGenerateLink.Text = "Generate Link";
            lblGenerateLink.Width += 40;
            lblGenerateLink.Parent = parent2;

            lblLinkType = new Label(manager);
            lblLinkType.Init();
            lblLinkType.Left = left;
            lblLinkType.Text = "Link Type";
            lblLinkType.Parent = parent2;
            lblLinkType.Top = HeightCounter3; HeightCounter3 += lblLinkType.Height;

            cbLinkType = new ComboBox(manager);
            cbLinkType.Init();
            cbLinkType.Left = left;
            cbLinkType.Width += 20;
            cbLinkType.Parent = parent2;
            cbLinkType.MaxItems = 15;
            cbLinkType.Top = HeightCounter3; HeightCounter3 += cbLinkType.Height;
            //cbLinkType.Items.AddRange(new List<object>() { });
            Link.GetILinkableEnumVals(cbLinkType.Items);

            /*
            foreach (comp key in Enum.GetValues(typeof(comp)))
            {
                Type compType = Game1.compTypes[key];

                if (!typeof(ILinkable).IsAssignableFrom(compType)) continue;

                cbLinkType.Items.Add(key);

                /*
                MethodInfo mInfo = compType.GetMethod("AffectOther");
                if (mInfo != null
                    && mInfo.DeclaringType == compType)
                {
                    cbLinkType.Items.Add(key);
                }
                //
            }
            */

            lblLinkFormation = new Label(manager);
            lblLinkFormation.Init();
            lblLinkFormation.Left = left;
            lblLinkFormation.Text = "Formation";
            lblLinkFormation.Parent = parent2;
            lblLinkFormation.Top = HeightCounter3; HeightCounter3 += lblLinkFormation.Height;

            cbLinkFormation = new ComboBox(manager);
            cbLinkFormation.Init();
            cbLinkFormation.Left = left;
            cbLinkFormation.Width += 20;
            cbLinkFormation.Parent = parent2;
            cbLinkFormation.Top = HeightCounter3; HeightCounter3 += cbLinkFormation.Height;

            foreach (formationtype f in Enum.GetValues(typeof(formationtype)))
            {
                cbLinkFormation.Items.Add(f);
            }
            cbLinkFormation.ItemIndex = 0;

            chkEntangled = new CheckBox(manager);
            chkEntangled.Init();
            chkEntangled.Left = left;
            chkEntangled.Width += 20;
            chkEntangled.Text = "Entangled";
            chkEntangled.Parent = parent2;
            chkEntangled.Top = HeightCounter3; HeightCounter3 += chkEntangled.Height;

            HeightCounter3 = lblGenerateLink.Height;

            lblLinkPresets = new Label(manager);
            lblLinkPresets.Init();
            lblLinkPresets.Left = left + middle;
            lblLinkPresets.Text = "Preset";
            lblLinkPresets.Parent = parent2;
            lblLinkPresets.Top = HeightCounter3; HeightCounter3 += lblLinkPresets.Height;

            cbLinkPresets = new ComboBox(manager);
            cbLinkPresets.Init();
            cbLinkPresets.Left = left + middle;
            //cbLinkPresets.Width += 20;
            cbLinkPresets.Parent = parent2;
            cbLinkPresets.Top = HeightCounter3; HeightCounter3 += cbLinkPresets.Height;

            cbLinkPresets.Items.Add("Default");
            cbLinkPresets.ItemIndex = 0;

            btnAddToPalette = new Button(manager);
            btnAddToPalette.Init();
            btnAddToPalette.Left = left + middle;
            btnAddToPalette.Width = middle - 20;
            btnAddToPalette.Text = "Add to\nPalette";
            btnAddToPalette.Height = btnAddToPalette.Height * 2 - 10;
            btnAddToPalette.Parent = parent2;
            btnAddToPalette.Top = HeightCounter3 + 10; HeightCounter3 += btnAddToPalette.Height + 10;
            btnAddToPalette.Click += btnAddToPalette_Click;
        }

        void btnAddToPalette_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (cbLinkType.ItemIndex < 0) return;

            if (cbLinkPresets.SelectedItem().Equals("Default"))
            {
                //try
                //{
                    string ltype = cbLinkType.SelectedItem();
                    comp c = (comp)Enum.Parse(typeof(comp), ltype);
                    //System.Console.WriteLine((int)c);
                    Type t = Game1.compTypes[c];

                    object linkComp = Activator.CreateInstance(t);

                    ILinkable l = (ILinkable)linkComp;

                    bool entangled = chkEntangled.Checked;

                    string ftype = cbLinkFormation.SelectedItem();
                    formationtype f = (formationtype)Enum.Parse(typeof(formationtype), ftype);

                    Link newLink = new Link(l, f);
                    newLink.IsEntangled = entangled;

                    sidebar.PaletteLinks.Add(newLink);

                    if (sidebar.cbLinkList.ItemIndex != -1)
                        sidebar.cbLinkList.ItemIndex = sidebar.cbLinkList.ItemIndex;


                    window.Close();
                //}
                    /*
                catch(Exception ex)
                {
                    string ltype = cbLinkType.SelectedItem();
                    comp c = (comp)Enum.Parse(typeof(comp), ltype);
                    //System.Console.WriteLine((int)c);
                    Type t = Game1.compTypes[c];
                    System.Console.WriteLine(t);
                    throw ex;
                }
                */
            }
        }
    }
}
