using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.MovingWindow;

public abstract class MovementMode
{
    public virtual Vector2Int StartPosition
    {
        get => new(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
    }

    public abstract Vector2 Move(Vector2Int resolution, Vector2Int currentWindowPoint);
}
