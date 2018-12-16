using System;
using wManager.Wow.Class;
using GcdManage;

public class WoWSpell : Spell
{
    private Func<bool> UseCondition { get; }

    private bool SharesGcd { get; }

    private bool CheckDistance { get; }


    public WoWSpell(string spellNameEnglish, Func<bool> useCondition = null, bool sharesGcd = true, bool checkDistance = true) 
        : base(spellNameEnglish, true)
    {
        UseCondition = useCondition;
        SharesGcd = sharesGcd;
        CheckDistance = checkDistance;
    }

    public new void Launch()
    {
        if (SharesGcd)
            GcdManager.Update();

        base.Launch();
    }

    public new bool IsDistanceGood() => CheckDistance ? base.IsDistanceGood : true;

    public new bool IsSpellUsable()
    {
        if (SharesGcd && !GcdManager.GcdIsOff)
            return false;
        else if (!base.IsSpellUsable)
            return false;
        else if (UseCondition != null)
            return UseCondition();
        else
            return true;
    }
}


