using UnityEngine;

public class ZedPlush : MonoBehaviour
{
    [SerializeField]
    private Texture2D[] _faces;
    [SerializeField]
    private Material _screenMaterial;

    public void ChangeFace()
    {
        _screenMaterial.mainTexture = _faces[UnityEngine.Random.Range(0, _faces.Length)];
    }
}
