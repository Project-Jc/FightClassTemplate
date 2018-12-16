using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Linq;

using robotManager.Helpful;

using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

using ClassManage;
using wManager.Wow.Enums;

public class Main : ICustomClass
{
    public float Range => 32.5f;

    private bool Launched;


    public void Dispose()
    {
        Launched = false;

        if (ClassManager.ForcedResting)
            ClassManager.ResetRegeneration();

        FightEvents.OnFightStart -= FightEvents_OnFightStart;
        FightEvents.OnFightEnd -= FightEvents_OnFightEnd;
        Radar3D.OnDrawEvent -= Radar3D_OnDrawEvent;

        robotManager.Events.FiniteStateMachineEvents.OnRunState -= FiniteStateMachineEvents_OnRunState;

        Logging.Write($"[{ FightClassSettings.Name }] unloaded");
    }

    public void ShowConfiguration()
    {
        FightClassSettings.Load();
        FightClassSettings.CurrentSetting.ToForm();
        FightClassSettings.CurrentSetting.Save();
    }

    public void Initialize()
    {
        FightClassSettings.Load();

        ClassManager.LoadAbilities();

        FightEvents.OnFightStart += FightEvents_OnFightStart;
        FightEvents.OnFightEnd += FightEvents_OnFightEnd;
        Radar3D.OnDrawEvent += Radar3D_OnDrawEvent;

        EventsLua.AttachEventLua(LuaEventsId.PLAYER_TARGET_CHANGED, e => EventsLua_OnEventLuaPlayerTargetChanged());

        robotManager.Events.FiniteStateMachineEvents.OnRunState += FiniteStateMachineEvents_OnRunState;

        Logging.Write($"[{ FightClassSettings.Name }] loaded");

        Process();
    }

    private void EventsLua_OnEventLuaPlayerTargetChanged()
    {
        if (Lua.LuaDoString<bool>("return IsCurrentAction(" + (SpellManager.GetSpellSlotId(SpellListManager.SpellIdByName("Attack")) + 1) + ")")) {
            SpellManager.CastSpellByNameLUA("Attack");
        }
    }

    private void FiniteStateMachineEvents_OnRunState(robotManager.FiniteStateMachine.Engine engine, robotManager.FiniteStateMachine.State state, CancelEventArgs cancelable)
    {
        //Logging.WriteDebug($"FiniteStateMachineEvents_OnRunState: { state.DisplayName } NeedToRun: { state.NeedToRun }");
    }

    private void FightEvents_OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
    {     
    }

    private void FightEvents_OnFightEnd(ulong guid)
    {
    }

    private void Radar3D_OnDrawEvent()
    {
    }


    internal void Process()
    {
        Launched = true;

        try {
            for (; Launched; Thread.Sleep(10)) {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause) {
                    if (Fight.InFight) {
                        Rotation();
                    }
                }
            }
        }
        catch (Exception e) {
            Logging.Write($"[{ FightClassSettings.Name }] error: { e }");
        }
    }

    internal void Rotation()
    {
        foreach (var ability in ClassManager.Spells) {

            if (!ability.IsDistanceGood())
                continue;

            if (!ability.IsSpellUsable())
                continue;

            ability.Launch();

            break;
        }
    }
}


