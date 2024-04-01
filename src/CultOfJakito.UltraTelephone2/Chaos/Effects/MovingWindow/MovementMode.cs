using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MovingWindow;

public abstract class MovementMode
{
    public abstract Vector2 Move(Vector2Int resolution, Vector2Int currentWindowPoint);
}
