using System;
using System.IO;
using System.Configuration;
using System.ComponentModel;

using robotManager;
using robotManager.Helpful;

using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

[Serializable]
public class FightClassSettings : Settings
{
    public static string Name => "FightClass";

    public static string UserNameAndRealm => $"{ ObjectManager.Me.Name }.{ Usefuls.RealmName }";


    private FightClassSettings()
    {
        DisableAutoAttack = true;

        Rotation = new string[] {
           "Fireball"
        };

        ConfigWinForm(new System.Drawing.Point(450, 600), $"{ Name }{ Translate.Get("Settings") }");
    }

    public static FightClassSettings CurrentSetting { get; set; }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName(Name, UserNameAndRealm));
        }
        catch (Exception e)
        {
            Logging.WriteError($"{ Name } > Save(): { e }");
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName(Name, UserNameAndRealm)))
            {
                CurrentSetting =
                    Load<FightClassSettings>(AdviserFilePathAndName(Name, UserNameAndRealm));
                return true;
            }
            CurrentSetting = new FightClassSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError($"{ Name } > Load(): { e }");
        }
        return false;
    }


    [Setting]
    [Category("Rotation")]
    [DisplayName("Rotation")]
    [Description("Define your own rotation by changing the order of the ability names in the list. " +
                 "If your character does not know the ability name, you can leave it in the list. It will simply won't be used.")]
    public string[] Rotation { get; set; }

    [Setting]
    [Category("Misc")]
    [DisplayName("Disable Auto Attack")]
    [Description("Disable auto attacking.")]
    public bool DisableAutoAttack { get; set; }
}


