﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    public enum formationtype
    {
        AllToAll,
        NearestN,
        FurthestN,
        //Nearest,
        //Random,
        //Special,
    }

    public class Formation
    {
        public Room room;
        public Link link { get; set; }
        public formationtype FormationType { get { return link.FormationType; } set { link.FormationType = value; } }
        public bool Uninhabited { get; set; }
        
        private int _UpdateFrequency = -1;
        public int UpdateFrequency
        {
            get { return _UpdateFrequency; }
            set
            {
                if (_UpdateFrequency <= 0 && value > 0)
                {
                    room.AfterIteration += UpdateHandler;
                }
                else if (_UpdateFrequency > 0 && value <= 0)
                {
                    room.AfterIteration -= UpdateHandler;
                }
                _UpdateFrequency = value;
            }
        }
        public int Clock = 0;
        public int NearestNValue { get; set; }
        public Dictionary<Node, ObservableHashSet<Node>> AffectionSets { get; set; }

        public Formation(   Link link, 
                            formationtype FormationType = formationtype.AllToAll,
                            bool Uninhabited = false,
                            int UpdateFrequency = -1,
                            int NearestNValue = 1,
                            bool InitializeFormation = true)
        {
            this.room = Program.getRoom();
            this.link = link;
            //this.FormationType = FormationType;
            this.Uninhabited = Uninhabited;
            this.UpdateFrequency = UpdateFrequency;
            this.NearestNValue = NearestNValue;
            this.AffectionSets = new Dictionary<Node, ObservableHashSet<Node>>();
            
            if (InitializeFormation) UpdateFormation();
        }

        public Formation(Link link,
                            Formation form,
                            bool InitializeFormation = true)
        {
            this.room = Program.getRoom();
            this.link = link;
            //this.FormationType = FormationType;
            this.Uninhabited = form.Uninhabited;
            this.UpdateFrequency = form.UpdateFrequency;
            this.NearestNValue = form.NearestNValue;
            this.AffectionSets = new Dictionary<Node, ObservableHashSet<Node>>();

            if (InitializeFormation) UpdateFormation();
        }

        public void UpdateHandler(object sender, EventArgs e)
        {
            Update();
        }

        public void Update()
        {
            if (UpdateFrequency <= 0 || FormationType == formationtype.AllToAll) return;

            if (Clock++ % UpdateFrequency ==  0)
            {
                UpdateFormation();
            }
        }

        public void UpdateFormation()
        {
            AffectionSets = new Dictionary<Node, ObservableHashSet<Node>>();
            if (FormationType == formationtype.AllToAll)
            {
                AllToAll();
            }
            else if (FormationType == formationtype.NearestN || FormationType == formationtype.FurthestN)
            {
                NearestN();
            }
        }

        public void AllToAll()
        {
            if (link.sources != null)
            {
                link.sources.ToList().ForEach(delegate(Node source)
                {
                    AffectionSets[source] = link.targets;
                    //HashSet<Node> set = AffectionSets[source];
                    //set = new HashSet<Node>();
                    //link.targets.ToList().ForEach(delegate(Node target) {
                    //    set.Add(target);
                    //});
                });
            }
        }
        //used for NearestN or FurthestN
        public void NearestN()
        {
            if (link.sources != null)
            {
                //not effecient if NearestNValue == 1 because it sorts the entire list of distances
                HashSet<Node> AlreadyInhabited = new HashSet<Node>();

                link.sources.ToList().ForEach(delegate(Node source)
                {
                    AffectionSets[source] = new ObservableHashSet<Node>();
                    ObservableHashSet<Node> set = AffectionSets[source];

                    List<Tuple<float, Node>> DistancesList = new List<Tuple<float, Node>>();
                    Comparison<Tuple<float, Node>> comparer;
                    if (FormationType == formationtype.NearestN)
                    {
                        comparer = delegate(Tuple<float, Node> first, Tuple<float, Node> second)
                        {
                            if (first.Item1 < second.Item1) return -1;
                            else if (first.Item1 > second.Item1) return 1;
                            return 0;
                        };
                    }
                    else if (FormationType == formationtype.FurthestN)
                    {
                        comparer = delegate(Tuple<float, Node> first, Tuple<float, Node> second)
                        {
                            if (first.Item1 > second.Item1) return -1;
                            else if (first.Item1 < second.Item1) return 1;
                            return 0;
                        };
                    }
                    else return;

                    link.targets.ToList().ForEach(delegate(Node target)
                    {
                        if (source == target) return;
                        DistancesList.Add(new Tuple<float, Node>(Vector2.DistanceSquared(source.transform.position, target.transform.position), target));
                    });

                    DistancesList.Sort(comparer);

                    int min = Math.Min(NearestNValue, DistancesList.Count);

                    /*
                    for (int i = 0; i < min; i++)
                    {
                        set.Add(DistancesList.ElementAt(i).Item2);
                    }
                    */
                    int count = 0;
                    int it = 0;
                    while (count < min)
                    {
                        if (it >= DistancesList.Count) break;
                        Node nn = DistancesList.ElementAt(it).Item2;
                        if (Uninhabited)
                        {
                            if (AlreadyInhabited.Contains(nn))
                            {
                                it++;
                                continue;
                            }
                            else
                            {
                                AlreadyInhabited.Add(nn);
                            }
                        }

                        set.Add(nn);
                        count++;
                        it++;
                    }
                });
            }
        }

                         

    }
}
