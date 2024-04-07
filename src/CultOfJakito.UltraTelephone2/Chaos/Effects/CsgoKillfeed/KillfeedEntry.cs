using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.CsgoKillfeed;

public unsafe class KillfeedEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _victimText;
    [SerializeField] private Image _weaponImage;
    [SerializeField] private Graphic[] _graphicsToFade;
    private Color[] _fadeGraphicStartColours;
    private float _timeAlive;
    private float _fadedOut = 1;

    private void Start()
    {
        _fadeGraphicStartColours = new Color[_graphicsToFade.Length];

        for (int i = 0; i < _graphicsToFade.Length; i++)
        {
            _fadeGraphicStartColours[i] = _graphicsToFade[i].color;
        }
    }
    public void SetValues(string victim, Sprite weapon = null)
    {
        _victimText.text = victim;

        if (weapon != null)
        {
            _weaponImage.sprite = weapon;
        }
    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;

        if (_timeAlive < 2)
        {
            return;
        }

        _fadedOut = Mathf.MoveTowards(_fadedOut, 0, Time.deltaTime * 3);

        if (_fadedOut == 0)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < _graphicsToFade.Length; i++)
        {
            _graphicsToFade[i].color = _fadedOut * _fadeGraphicStartColours[i];
        }
    }
}
