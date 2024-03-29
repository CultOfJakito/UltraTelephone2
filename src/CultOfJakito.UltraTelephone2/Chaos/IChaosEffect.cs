namespace CultOfJakito.UltraTelephone2.Chaos;
public interface IChaosEffect : IDisposable
{
    public void BeginEffect(UniRandom random);
    public bool CanBeginEffect(ChaosSessionContext ctx);
    public int GetEffectCost();
}
