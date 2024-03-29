﻿using CultOfJakito.UltraTelephone2.Assets;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

public class MusicalRocket : MonoBehaviour
{
    private AudioSource _source;

    public void Start()
    {
        _source = new GameObject("Rocket Music").AddComponent<AudioSource>();
        _source.clip = UT2Assets.GetAsset<AudioClip>("ridingtunes");
        _source.gameObject.transform.parent = transform;
    }

    public void Play()
    {
        if (_source.time > 0)
        {
            _source.UnPause();
            return;
        }

        _source.Play();
    }

    public void Pause()
    {
        _source.Pause();
    }
}