using BepInEx.Logging;
using CultOfJakito.UltraTelephone2.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.Chaos;

internal class DamageOnHoldingSkull : ChaosEffect
{
    [Inject]
    public ManualLogSource Logger { get; set; }

    private int _damage;

    private float _timer = 1f;
    private bool _isHoldingSkull;
    private bool _hasDamaged;

    public override void BeginEffect(Random random) {
        _damage = random.Next(10, 25);
        Logger.LogInfo($"{_damage} skull damage");
        EventBus.PlayerItemChanged += OnPlayerItemChanged;
    }

    private void Update() {
        if(!_isHoldingSkull) {
            _timer = 1f;
            return;
        }

        _timer -= UnityEngine.Time.deltaTime;
        if(_timer <= 0f) {
            Damage();
            _timer = 1f;
        }
    }

    private void Damage() {
        if(!_hasDamaged) {
            HudMessageReceiver.Instance.SendHudMessage("Your robotic flesh begins to melt at the touch of the skull.");
            _hasDamaged = true;
        }
        NewMovement.Instance.GetHurt(_damage, true);
    }

    private void OnPlayerItemChanged(object sender, PlayerItemChangedEventArgs e) {
        _isHoldingSkull = e.Item is not null && e.Item.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen;
        Logger.LogInfo($"Holding a skull: {_isHoldingSkull}");
    }

    private void OnDestroy() {
        EventBus.PlayerItemChanged -= OnPlayerItemChanged;
    }
}
