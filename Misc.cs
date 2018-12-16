using robotManager.Helpful;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public static class Misc
{
    /// <summary>
    /// Returns -1 if there are no enemies.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public static int EnemiesAttackingCount(float range)
    {
        var count = ObjectManager.GetWoWUnitHostile().Where(w => w.IsTargetingMe && w.GetDistance <= range).Count();
        return count > 0 ? count : -1;
    }

    public static uint UnitAggroDistance(uint unitLevel)
    {
        const uint defaultInGameAggroDistance = 18;

        return unitLevel > ObjectManager.Me.Level ? 
            defaultInGameAggroDistance + (unitLevel - ObjectManager.Me.Level) : 
            defaultInGameAggroDistance - (ObjectManager.Me.Level - unitLevel);
    }

    public static void PlaceSpellInActionBar(string spellName)
    {
        if (ActionBarFreeSlot(out var freeSlot)) {

            Logging.WriteDebug($"[{ FightClassSettings.Name }] placing spell \"{ spellName }\" on slot \"{ freeSlot }\" of the action bar");

            Lua.LuaDoString(
                    @"
                            local spellNameInGame = '#SPELLNAME#';
                            local freeslot = '#FREESLOT#';

                            local i = 1;
                            while true do
                                local spellName, spellRank = GetSpellName(i, BOOKTYPE_SPELL)
                                if not spellName then break; end
                                if spellName == spellNameInGame then PickupSpell(i, BOOKTYPE_SPELL); PlaceAction(freeslot); ClearCursor() end
                                    i = i + 1
                            end
                        ".Replace("#SPELLNAME#", spellName.Replace("'", "\\'"))
                         .Replace("#FREESLOT#", freeSlot.ToString()));
        }
    }

    public static bool ActionBarFreeSlot(out int freeSlot)
    {
        freeSlot = Lua.LuaDoString<int>(
            @"
                local freeslot = -1;
                for i = 1, 120, 1 do
                    if not HasAction(i) then freeslot = i; break; end
                end

                return freeslot;");

        return freeSlot != -1;
    }
}


