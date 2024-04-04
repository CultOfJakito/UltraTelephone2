using Configgy;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.FortniteBuilding;

public class FortniteBuild : MonoBehaviour
{
    [Configgable("Chaos/Effects/Fortnite Building", "Edit key")] private static ConfigInputField<KeyCode> EditKey = new(KeyCode.G);
    public BuildChunk[] Edits;
    [field: SerializeField] public BuildTypes BuildType { get; private set; }
    private bool _editing;

    private void Update()
    {
        if (Input.GetKeyDown(EditKey.Value) && Edits.Any(chunk => chunk.LookingAt))
        {
            if (!_editing)
            {
                StartEditing();
                _editing = true;
            }
            else
            {
                StopEditing();
                _editing = false;
            }
        }

        if (Vector3.SqrMagnitude(NewMovement.Instance.transform.position - transform.position) > Mathf.Pow(25, 2))
        {
            StopEditing();
        }
    }

    public void StartEditing()
    {
        foreach (BuildChunk chunk in Edits)
        {
            chunk.StartEditing();
        }
    }

    public void StopEditing()
    {
        bool hasEnabledChunks = false;

        foreach (BuildChunk chunk in Edits)
        {
            chunk.StopEditing();
            hasEnabledChunks = hasEnabledChunks || chunk.Enabled;
        }

        if (Edits.Length != 0 && !hasEnabledChunks)
        {
            GetComponent<Breakable>().Break();
        }
    }

    private void OnDestroy()
    {
        BuildingControls.Instance.RemoveBuildFromList(this);
    }
}
