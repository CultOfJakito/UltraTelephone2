namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerRespawnEvent : PlayerEvent
    {
        /// <summary>
        /// If true, the respawn was triggered manually by the player through checkpoint restart. If false, the played died and respawned.
        /// </summary>
        public bool IsManualRespawn { get; }
        public PlayerRespawnEvent(NewMovement player, bool manualRespawn) : base(player)
        {
            IsManualRespawn = manualRespawn;
        }
    }
}
