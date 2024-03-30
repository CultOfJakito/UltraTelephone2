using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MovingWindow;

public class Circles : MovementMode
{
    private float _angle;
    private float _movementSpeed = UnityEngine.Random.Range(0.15f * Screen.width, 0.3f * Screen.width);
    private float _rotationSpeed = UnityEngine.Random.Range(75, 150);

    public override Vector2Int StartPosition => new(Screen.width / 2, 0);

    public override Vector2 Move(Vector2Int resolution, Vector2Int currentWindowPoint)
    {
        if (currentWindowPoint.x <= 0 || currentWindowPoint.x + resolution.x >= Screen.currentResolution.width)
        {
            _movementSpeed *= 1;
        }

        if (currentWindowPoint.y <= 0 || currentWindowPoint.y + resolution.y >= Screen.currentResolution.height)
        {
            _movementSpeed *= 1;
        }

        _angle += Time.unscaledDeltaTime * _rotationSpeed;
        _movementSpeed += (Time.unscaledDeltaTime / 40) * Screen.width;
        return new Vector2((float)Math.Cos(_angle * Mathf.Deg2Rad), (float)Math.Sin(_angle * Mathf.Deg2Rad)) * _movementSpeed;
    }
}
