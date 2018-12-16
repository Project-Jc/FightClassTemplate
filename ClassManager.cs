using System.Collections.Generic;
using System.Threading;
using System.Linq;

using wManager;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

using static robotManager.Helpful.Logging;

namespace ClassManage
{
    public static class ClassManager
    {
        static ClassManager()
        {
        }


        public static bool HasBandage()
        {
            return Bag.GetBagItem().FirstOrDefault(f => f.Name.EndsWith("Bandage")) != null ? true : false;
        }

        public static bool HasBandage(out string bandage)
        {
            bandage = Bag.GetBagItem().FirstOrDefault(f => f.Name.EndsWith("Bandage"))?.Name;

            return bandage != null ? true : false;
        }

        public static void ApplyBandage(string bandage)
        {
            WriteDebug($"[{ FightClassSettings.Name }] applying bandage \"{ bandage }\"");

            ItemsManager.UseItem(ItemsManager.GetIdByName(bandage));
            Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();
        }


        public static List<WoWSpell> Spells { get; private set; }

        public static void LoadAbilities()
        {
            Spells = new List<WoWSpell>();

            foreach (var spellName in FightClassSettings.CurrentSetting.Rotation) {

                var spell = SpellManager.SpellBook().FirstOrDefault(f => f.Name == spellName);

                if (spell == null)
                    continue;

                if (!spell.IsInActionBar)
                    Misc.PlaceSpellInActionBar(spell.NameInGame);

                var classSpell = default(WoWSpell);

                switch (spellName) {

                    case "Fireball": {
                            classSpell = new WoWSpell(spellName, sharesGcd: false);
                        } break;

                    //// Buffs.
                    //case "Battle Shout": {
                    //        classSpell = new WoWSpell(abilityName, () => !ObjectManager.Me.HaveBuff(abilityName));
                    //    }
                    //    break;

                    //case "Blood Fury": {
                    //        classSpell = new WoWSpell(abilityName, () => ObjectManager.Me.InCombat, false);
                    //    }
                    //    break;
                }

                Spells.Add(classSpell ?? new WoWSpell(spellName));
            }
        }


        public static List<WoWUnit> Multiples { get; private set; }

        public static void AddMultiples(List<WoWUnit> units)
        {
            Multiples.Clear();
            Multiples.AddRange(units);
        }

        public static void ClearMultiplesIfExists(ulong guid)
        {
            if (Multiples.Exists(e => e.Guid == guid)) {
                Multiples.Clear();
            }
        }


        public static bool ForcedResting => wManagerSetting.CurrentSetting.FoodPercent == 90 && wManagerSetting.CurrentSetting.FoodMaxPercent == 95;

        public static void ForceRegeneration()
        {
            WriteDebug($"[{ FightClassSettings.Name }] forcing resting");

            Regeneration.NewFoodValues(90, 95);
        }

        public static void ResetRegeneration()
        {
            WriteDebug($"[{ FightClassSettings.Name }] resetting resting");

            Regeneration.LoadUserDeclaredValues();
        }

        private static class Regeneration
        {
            private static int UserDeclaredFoodPercent { get; }
            private static int UserDeclaredFoodMaxPercent { get; }

            static Regeneration()
            {
                UserDeclaredFoodPercent = wManagerSetting.CurrentSetting.FoodPercent;
                UserDeclaredFoodMaxPercent = wManagerSetting.CurrentSetting.FoodMaxPercent;
            }

            public static void NewFoodValues(int foodPercent, int foodMaxPercent)
            {
                wManagerSetting.CurrentSetting.FoodPercent = foodPercent;
                wManagerSetting.CurrentSetting.FoodMaxPercent = foodMaxPercent;
            }

            public static void LoadUserDeclaredValues()
            {
                wManagerSetting.CurrentSetting.FoodPercent = UserDeclaredFoodPercent;
                wManagerSetting.CurrentSetting.FoodMaxPercent = UserDeclaredFoodMaxPercent;
            }
        }
    }
}


