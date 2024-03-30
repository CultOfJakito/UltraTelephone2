namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerParryEvent : PlayerEvent
    {
        public Punch Fist { get; }
        public PlayerParryEvent(NewMovement player, Punch fist) : base(player)
        {
            Fist = fist;
        }
    }
}
