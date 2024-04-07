using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.CsgoKillfeed;

public class Killfeed : MonoSingleton<Killfeed>
{
    [SerializeField] private GameObject _entry;

    public void CreateKillfeedEntry(EnemyIdentifier enemy, GameObject sourceWeapon)
    {
        Sprite icon = sourceWeapon?.GetComponent<WeaponIcon>()?.weaponDescriptor?.icon;
        GameObject entry = Instantiate(_entry, transform);
        entry.GetComponent<KillfeedEntry>().SetValues(enemy.FullName, icon);
        entry.transform.SetSiblingIndex(0);
    }
}
