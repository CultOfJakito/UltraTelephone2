using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.MovingWindow;

public class DvdScreensaver : MovementMode
{
    private Vector2 _direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * UnityEngine.Random.Range(0.15f * Screen.width, 0.3f * Screen.width);

    public override Vector2 Move(Vector2Int resolution, Vector2Int currentWindowPoint)
    {
        if (currentWindowPoint.x <= 0 || currentWindowPoint.x + resolution.x >= Screen.currentResolution.width)
        {
            _direction.x *= -1;
        }

        if (currentWindowPoint.y <= 0 || currentWindowPoint.y + resolution.y >= Screen.currentResolution.height)
        {
            _direction.y *= -1;
        }

        return _direction;
    }
}
