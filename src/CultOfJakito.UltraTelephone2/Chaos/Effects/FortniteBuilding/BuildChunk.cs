using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.FortniteBuilding;

public class BuildChunk : MonoBehaviour
{
    public Material DefaultMaterial;
    public Material EditEnabledMaterial;
    public Material EditDisabledMaterial;
    private Renderer _renderer;
    public bool Enabled { get; private set; } = true;
    private bool _editing = false;
    private bool _editedInThisLook = false;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void StartEditing()
    {
        gameObject.SetActive(true);
        _renderer.material = Enabled ? EditEnabledMaterial : EditDisabledMaterial;
        _editing = true;
    }

    public void StopEditing()
    {
        gameObject.SetActive(Enabled);
        _renderer.material = DefaultMaterial;
        _editing = false;
    }

    public void RefreshEditingMaterial()
    {
        _renderer.material = Enabled ? EditEnabledMaterial : EditDisabledMaterial;
    }

    private void Update()
    {
        if (_editing && LookingAt)
        {
            if (_editedInThisLook || !Input.GetMouseButton(0))
            {
                return;
            }

            Enabled = !Enabled;
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
