﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class PresetService : DeneirsService
    {
        public List<RaceViewModel> GetRaces()
        {
            var races = new List<RaceViewModel>();
            try
            {
                using (DBReset())
                {
                    races = DB.Races.Select(x => new RaceViewModel
                    {
                        Name = x.Name,
                        RaceKey = x.RaceKey
                    }).OrderBy(x => x.Name).ToList();

                    var none = races.FirstOrDefault(x => x.Name == "None");
                    races.Remove(none);
                    races.Insert(0, none);
                }
            }
            catch (Exception ex) { }

            return races;
        }

        public List<ClassViewModel> GetClasses()
        {
            var classes = new List<ClassViewModel>();
            try
            {
                using (DBReset())
                {
                    classes = DB.Classes.Select(x => new ClassViewModel
                    {
                        Name = x.Name,
                        ClassKey = x.ClassKey
                    }).OrderBy(x => x.Name).ToList();

                    var none = classes.FirstOrDefault(x => x.Name == "None");
                    classes.Remove(none);
                    classes.Insert(0, none);
                }
            }
            catch (Exception ex) { }

            return classes;
        }

        public List<BackgroundViewModel> GetBackgrounds()
        {
            var backgrounds = new List<BackgroundViewModel>();
            try
            {
                using (DBReset())
                {
                    backgrounds = DB.Backgrounds.Select(x => new BackgroundViewModel
                    {
                        Name = x.Name,
                        BackgroundKey = x.BackgroundKey
                    }).OrderBy(x => x.Name).ToList();

                    var none = backgrounds.FirstOrDefault(x => x.Name == "None");
                    backgrounds.Remove(none);
                    backgrounds.Insert(0, none);
                }
            }
            catch (Exception ex) { }

            return backgrounds;
        }

        public Dictionary<string, string> GetAlignments()
        {
            var items = new Dictionary<string, string>();
            items.Add("LG", "Lawful Good");
            items.Add("NG","Neutral Good");
            items.Add("CG","Chaotic Good");
            items.Add("LN","Lawful Neutral");
            items.Add("NN","Neutral");
            items.Add("CN","Chaotic Neutral");
            items.Add("LE","Lawful Evil");
            items.Add("NE","Neutral Evil");
            items.Add("CE","Chaotic Evil");

            return items;
        }
    }
}
