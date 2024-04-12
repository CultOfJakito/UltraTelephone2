namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerActivatedEvent : PlayerEvent
    {
        public string LevelName { get;}

        public PlayerActivatedEvent(string levelName, NewMovement player) : base(player)
        {
            LevelName = levelName;
        }
    }
}
