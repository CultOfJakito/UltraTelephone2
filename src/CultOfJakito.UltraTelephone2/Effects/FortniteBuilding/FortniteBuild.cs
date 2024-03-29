using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.FortniteBuilding;

public class FortniteBuild : MonoBehaviour
{
    public const KeyCode EditBind = KeyCode.G;
    public BuildChunk[] Edits;
    private bool _editing;

    private void Update()
    {
        if (Input.GetKeyDown(EditBind) && Edits.Any(chunk => chunk.LookingAt))
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
        foreach (BuildChunk chunk in Edits)
        {
            chunk.StopEditing();
        }
    }
}
