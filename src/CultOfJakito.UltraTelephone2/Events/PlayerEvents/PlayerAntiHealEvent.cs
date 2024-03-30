namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerAntiHealEvent : PlayerEvent
    {
        public int AntiHealthGained { get; }
        public PlayerAntiHealEvent(NewMovement player, int antiGained) : base(player)
        {
            AntiHealthGained = antiGained;
        }
    }
}
