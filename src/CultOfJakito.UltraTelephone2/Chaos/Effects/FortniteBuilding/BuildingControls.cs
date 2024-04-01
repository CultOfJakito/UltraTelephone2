using CultOfJakito.UltraTelephone2.Assets;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.FortniteBuilding;

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
    private Dictionary<GameObject, string> _placedWallIdentifiers = new(); //this fucking sucks but im tired
    private Dictionary<GameObject, string> _placedFloorIdentifiers = new();
    private Dictionary<GameObject, string> _placedRampConeIdentifiers = new();
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

        if (!_currentlyBuilding)
        {
            return;
        }

        foreach (KeyValuePair<KeyCode, BuildTypes> keyAndBuild in s_keyToType)
        {
            if (Input.GetKeyDown(keyAndBuild.Key))
            {
                _currentBuild = keyAndBuild.Value;
                BuildingHud.Instance.SelectOutline(_currentBuild);

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
            TryBuild(_currentBuild, spawnPos, spawnRot);
        }
    }

    private bool TryBuild(BuildTypes type, Vector3 position, Quaternion rotation)
    {
        switch (type)
        {
            case BuildTypes.Wall:
                string thisIdentifier = position + "|" + rotation.eulerAngles;
                if (_placedWallIdentifiers.Values.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedWallIdentifiers.Add(Instantiate(s_typeToBuild[type], position, rotation), thisIdentifier);
                return true;

            case BuildTypes.Floor:
                thisIdentifier = position.ToString();
                if (_placedFloorIdentifiers.Values.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedFloorIdentifiers.Add(Instantiate(s_typeToBuild[type], position, rotation), thisIdentifier);
                return true;

            case BuildTypes.Cone or BuildTypes.Ramp:
                thisIdentifier = position.ToString();
                if (_placedRampConeIdentifiers.Values.Contains(thisIdentifier))
                {
                    return false;
                }
                _placedRampConeIdentifiers.Add(Instantiate(s_typeToBuild[type], position, rotation), thisIdentifier);
                return true;
        }

        return false;
    }

    public void RemoveBuildFromList(FortniteBuild build)
    {
        switch (build.BuildType)
        {
            case BuildTypes.Wall:
                _placedWallIdentifiers.Remove(build.gameObject);
                return;

            case BuildTypes.Floor:
                _placedFloorIdentifiers.Remove(build.gameObject);
                return;

            case BuildTypes.Cone or BuildTypes.Ramp:
                _placedRampConeIdentifiers.Remove(build.gameObject);
                return;
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
