using CultOfJakito.UltraTelephone2.Data;

namespace CultOfJakito.UltraTelephone2.Hydra.FakePBank;

public static class FakeBank
{
    private static bool s_initialized;

    public static long GetCurrentMoney()
    {
        Initialize();
        return Ut2Data.SaveData.FakePAmount;
    }

    public static void AddMoney(long amount)
    {
        Initialize();
        Ut2Data.SaveData.FakePAmount += amount;
        Ut2Data.Save();
    }

    public static void SetMoney(long amount)
    {
        Initialize();
        Ut2Data.SaveData.FakePAmount = amount;
        Ut2Data.Save();
    }

    private static void Initialize()
    {
        if (s_initialized)
        {
            return;
        }

        //Dont use GetMoney() here, it will cause a stack overflow
        int p = GameProgressSaver.GetGeneralProgress().money;
        int lastP = Ut2Data.SaveData.LastRealPAmount;
        Ut2Data.SaveData.LastRealPAmount = p;

        if (!Ut2Data.SaveData.InitializedPAmount)
        {
            lastP = p;
            Ut2Data.SaveData.FakePAmount = p;
            Ut2Data.SaveData.InitializedPAmount = true;
            Ut2Data.Save();
        }

        if (p != lastP)
        {
            Ut2Data.SaveData.FakePAmount += p - lastP;
        }


        s_initialized = true;
    }

    public static string FormatMoney(long money)
    {
        return money switch
        {
            long.MaxValue => "∞",
            long.MinValue => "-∞",
            _ => money.ToString("N0")
        };
    }

    public static string PString(long money) => "<color=white>" + FormatMoney(money) + "</color>" + "<color=orange>P</color>";
}
