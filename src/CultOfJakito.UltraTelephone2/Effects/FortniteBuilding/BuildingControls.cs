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
    private List<string> _placedWallIdentifiers = new(); //this fucking sucks but im tired
    private List<string> _placedFloorIdentifiers = new();
    private List<string> _placedRampConeIdentifiers = new();
    private BuildTypes _currentBuild;
    private const int VoxelSize = 10;

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

        Vector3 spawnPos = RoundToNearestVoxel(NewMovement.Instance.transform.position + CameraController.Instance.transform.forward * VoxelSize / 1.75f);
        Quaternion spawnRot = RoundRotation(NewMovement.Instance.transform.rotation);
        _typeToPreview[_currentBuild].transform.position = spawnPos;
        _typeToPreview[_currentBuild].transform.rotation = spawnRot;

        if (Input.GetMouseButton(0))
        {
            if (!CanBuildHere(_currentBuild, spawnPos, spawnRot))
            {
                return;
            }

            Instantiate(s_typeToBuild[_currentBuild], spawnPos, spawnRot);
        }
    }

    private bool CanBuildHere(BuildTypes type, Vector3 position, Quaternion rotation)
    {
        switch (type)
        {
            case BuildTypes.Wall:
                string thisIdentifier = position + "|" + rotation.eulerAngles;
                if (_placedWallIdentifiers.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedWallIdentifiers.Add(thisIdentifier);
                return true;

            case BuildTypes.Floor:
                thisIdentifier = position.ToString();
                if (_placedFloorIdentifiers.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedFloorIdentifiers.Add(thisIdentifier);
                return true;

            case BuildTypes.Cone or BuildTypes.Ramp:
                thisIdentifier = position.ToString();
                if (_placedRampConeIdentifiers.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedRampConeIdentifiers.Add(thisIdentifier);
                return true;
        }

        return false;
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
