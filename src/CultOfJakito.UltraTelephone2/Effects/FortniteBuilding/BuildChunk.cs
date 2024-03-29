using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.FortniteBuilding;

public class BuildChunk : MonoBehaviour
{
    public Material DefaultMaterial;
    public Material EditEnabledMaterial;
    public Material EditDisabledMaterial;
    private Renderer _renderer;
    private bool _enabled;
    private bool _editedInThisLook = false;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void StartEditing()
    {
        gameObject.SetActive(true);
        _renderer.material = _enabled ? EditEnabledMaterial : EditDisabledMaterial;
    }

    public void StopEditing()
    {
        gameObject.SetActive(_enabled);
        _renderer.material = DefaultMaterial;
    }

    public void RefreshEditingMaterial()
    {
        _renderer.material = _enabled ? EditEnabledMaterial : EditDisabledMaterial;
    }

    private void Update()
    {
        if (LookingAt)
        {
            if (_editedInThisLook || !Input.GetMouseButton(0))
            {
                return;
            }

            _enabled = !_enabled;
            _editedInThisLook = true;
            RefreshEditingMaterial();
        }
        else
        {
            _editedInThisLook = false;
        }
    }

    // i feel like this is really slow but blehhh :3
    public bool LookingAt => Physics.RaycastAll(CameraController.Instance.transform.position, CameraController.Instance.transform.forward, 100).Any(hit => hit.collider.gameObject == gameObject);
}
