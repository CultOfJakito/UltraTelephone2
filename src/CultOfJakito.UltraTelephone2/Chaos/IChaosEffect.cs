namespace CultOfJakito.UltraTelephone2.Chaos;

internal interface IChaosEffect {
	public void BeginEffect(Random random);
	public bool CanBeginEffect(ChaosSessionContext ctx);
	public int GetEffectCost();
}
