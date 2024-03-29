using CultOfJakito.UltraTelephone2.Data;

namespace CultOfJakito.UltraTelephone2.Hydra.FakePBank;

public static class FakeBank
{
    private static bool s_initialized;

    public static long GetCurrentMoney()
    {
        Initialize();
        return UT2Data.SaveData.FakePAmount;
    }

    public static void AddMoney(long amount)
    {
        Initialize();
        UT2Data.SaveData.FakePAmount += amount;
        UT2Data.Save();
    }

    public static void SetMoney(long amount)
    {
        Initialize();
        UT2Data.SaveData.FakePAmount = amount;
        UT2Data.Save();
    }

    private static void Initialize()
    {
        if (s_initialized)
        {
            return;
        }

        //Dont use GetMoney() here, it will cause a stack overflow
        int p = GameProgressSaver.GetGeneralProgress().money;
        int lastP = UT2Data.SaveData.LastRealPAmount;
        UT2Data.SaveData.LastRealPAmount = p;

        if (!UT2Data.SaveData.InitializedPAmount)
        {
            lastP = p;
            UT2Data.SaveData.FakePAmount = p;
            UT2Data.SaveData.InitializedPAmount = true;
            UT2Data.Save();
        }

        if (p != lastP)
        {
            UT2Data.SaveData.FakePAmount += p - lastP;
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
