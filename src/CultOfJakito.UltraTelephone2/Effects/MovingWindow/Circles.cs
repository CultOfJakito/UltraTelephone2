using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.MovingWindow;

public class Circles : MovementMode
{
    private float _angle;
    private float _movementSpeed = UnityEngine.Random.Range(0.15f * Screen.width, 0.3f * Screen.width);
    private float _rotationSpeed = UnityEngine.Random.Range(90, 180);

    public override Vector2Int StartPosition => new(Screen.width / 2, 0);

    public override Vector2 Move(Vector2Int resolution, Vector2Int currentWindowPoint)
    {
        _angle += Time.unscaledDeltaTime * _rotationSpeed;
        _movementSpeed += (Time.unscaledDeltaTime / 10) * Screen.width;
        return new Vector2((float)Math.Cos(_angle * Mathf.Deg2Rad), (float)Math.Sin(_angle * Mathf.Deg2Rad)) * _movementSpeed;
    }
}
