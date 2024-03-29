using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.FortniteBuilding;

public class BuildingControls : MonoSingleton<BuildingControls>
{
    private static readonly KeyValuePair<KeyCode, BuildTypes>[] s_keyToType =
    {
        new(KeyCode.Z, BuildTypes.Wall),
        new(KeyCode.X, BuildTypes.Floor),
        new(KeyCode.C, BuildTypes.Ramp),
        new(KeyCode.V, BuildTypes.Cone)
    };

    private static readonly Dictionary<BuildTypes, GameObject> s_typeToBuild = new()
    {
        //new(BuildTypes.Wall, AssetLo)
    };

    private BuildTypes _currentBuild;

    private void Update()
    {
        foreach (KeyValuePair<KeyCode, BuildTypes> keyAndBuild in s_keyToType)
        {
            if (Input.GetKeyDown(keyAndBuild.Key))
            {
                _currentBuild = keyAndBuild.Value;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Instantiate()
        }
    }

    private Vector3 RoundToNearestVoxel(Vector3 position)
    {
        const int voxelSize = 4;
        return new Vector3(Mathf.RoundToInt(position.x / voxelSize) * voxelSize, Mathf.RoundToInt(position.y / voxelSize) * voxelSize, Mathf.RoundToInt(position.z / voxelSize) * voxelSize);
    }
}
