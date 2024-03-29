using CultOfJakito.UltraTelephone2.Assets;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.FortniteBuilding;

public class BuildingControls : MonoSingleton<BuildingControls>
{
    private bool _currentlyBuilding = false;

    private static readonly KeyValuePair<KeyCode, BuildTypes>[] s_keyToType =
    {
        new(KeyCode.Z, BuildTypes.Wall),
        new(KeyCode.X, BuildTypes.Floor),
        new(KeyCode.C, BuildTypes.Ramp),
        new(KeyCode.V, BuildTypes.Cone)
    };

    private static readonly Dictionary<BuildTypes, GameObject> s_typeToBuild = new()
    {
        { BuildTypes.Wall, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Wall.prefab") },
        { BuildTypes.Floor, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Floor.prefab") },
        { BuildTypes.Ramp, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Ramp.prefab") },
        { BuildTypes.Cone, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Cone.prefab") }
    };

    private static readonly Dictionary<BuildTypes, GameObject> s_typeToPreviewPrefab = new()
    {
        { BuildTypes.Wall, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Preview/Wall 1.prefab") },
        { BuildTypes.Floor, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Preview/Floor 1.prefab") },
        { BuildTypes.Ramp, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Preview/Ramp 1.prefab") },
        { BuildTypes.Cone, UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Preview/Cone 1.prefab") }
    };

    private Dictionary<BuildTypes, GameObject> _typeToPreview = new();
    private Dictionary<BuildTypes, List<Vector2>> _typeToPositions = new()
    {
        { BuildTypes.Floor, new List<Vector2>() },
        { BuildTypes.Ramp, new List<Vector2>() },
        { BuildTypes.Cone, new List<Vector2>() }
    };
    private List<string> _placedWallIdentifiers = new(); //this fucking sucks but im tired
    private BuildTypes _currentBuild;
    private const int VoxelSize = 8;

    private void Start()
    {
        foreach (KeyValuePair<BuildTypes, GameObject> kvp in s_typeToPreviewPrefab)
        {
            _typeToPreview.Add(kvp.Key, Instantiate(kvp.Value));
            s_typeToPreviewPrefab[kvp.Key].SetActive(false);
        }
    }

    private void Update()
    {
        if (!BuildingEffect.CurrentlyActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentlyBuilding = !_currentlyBuilding;

            if (_currentlyBuilding)
            {
                GunControl.Instance.NoWeapon();
            }
            else
            {
                GunControl.Instance.YesWeapon();
            }
        }

        foreach (KeyValuePair<KeyCode, BuildTypes> keyAndBuild in s_keyToType)
        {
            if (Input.GetKeyDown(keyAndBuild.Key))
            {
                _currentBuild = keyAndBuild.Value;

                foreach (KeyValuePair<BuildTypes, GameObject> kvp in _typeToPreview)
                {
                    kvp.Value.SetActive(kvp.Key == _currentBuild);
                }
            }
        }

        Vector3 spawnPos = RoundToNearestVoxel(NewMovement.Instance.transform.position + NewMovement.Instance.transform.forward * VoxelSize / 2f);
        _typeToPreview[_currentBuild].transform.position = spawnPos;
        _typeToPreview[_currentBuild].transform.rotation = RoundRotation(NewMovement.Instance.transform.rotation);

        if (Input.GetMouseButton(0))
        {
            if (_currentBuild != BuildTypes.Wall)
            {
                if (_typeToPositions[_currentBuild].Contains(spawnPos))
                {
                    return;
                }

                _typeToPositions[_currentBuild].Add(spawnPos);
            }
            else
            {
                string thisIdentifier = spawnPos.ToString() + "|" + RoundRotation(NewMovement.Instance.transform.rotation).eulerAngles;
                if (_placedWallIdentifiers.Contains(thisIdentifier))
                {
                    return;
                }
                _placedWallIdentifiers.Add(thisIdentifier);
            }

            Instantiate(s_typeToBuild[_currentBuild], spawnPos, RoundRotation(NewMovement.Instance.transform.rotation));
        }
    }

    private Vector3 RoundToNearestVoxel(Vector3 position)
    {
        return new Vector3(Mathf.RoundToInt(position.x / VoxelSize) * VoxelSize, Mathf.RoundToInt(position.y / VoxelSize) * VoxelSize, Mathf.RoundToInt(position.z / VoxelSize) * VoxelSize);
    }

    private Quaternion RoundRotation(Quaternion quaternion)
    {
        return Quaternion.Euler(0, Mathf.RoundToInt(quaternion.eulerAngles.y / 90) * 90, 0);
    }
}
