using CultOfJakito.UltraTelephone2.Data;

namespace CultOfJakito.UltraTelephone2.Fun.FakePBank;

public static class FakeBank
{
    private static bool s_initialized;
    public static event Action<long> OnMoneyChanged;

    public static long GetCurrentMoney()
    {
        Initialize();
        return UT2SaveData.SaveData.FakePAmount;
    }

    public static void AddMoney(long amount)
    {
        Initialize();
        UT2SaveData.SaveData.FakePAmount += amount;
        UT2SaveData.MarkDirty();
        OnMoneyChanged?.Invoke(amount);
    }

    public static void SetMoney(long amount)
    {
        Initialize();
        long lastP = UT2SaveData.SaveData.FakePAmount;
        long difference = amount - lastP;
        UT2SaveData.SaveData.FakePAmount = amount;
        UT2SaveData.MarkDirty();
        OnMoneyChanged?.Invoke(amount);
    }

    private static void Initialize()
    {
        if (s_initialized)
        {
            return;
        }

        //Dont use GetMoney() here, it will cause a stack overflow
        int currentRealP = GameProgressSaver.GetGeneralProgress().money;
        int lastRealP = UT2SaveData.SaveData.LastRealPAmount;
        UT2SaveData.SaveData.LastRealPAmount = currentRealP;

        if (!UT2SaveData.SaveData.InitializedPAmount)
        {
            lastRealP = currentRealP;
            UT2SaveData.SaveData.FakePAmount = currentRealP;
            UT2SaveData.SaveData.InitializedPAmount = true;
            UT2SaveData.Save();
        }

        if (currentRealP != lastRealP)
        {
            //Apply the difference between the last real P and the current real P to the fake P
            UT2SaveData.SaveData.FakePAmount += currentRealP - lastRealP;
        }

        UT2SaveData.MarkDirty();
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
