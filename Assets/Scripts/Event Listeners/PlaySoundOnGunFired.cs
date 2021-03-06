﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class PlaySoundOnGunFired : MonoBehaviour
{
    [SerializeField]    private Gun _gun = null;
    [SerializeField]    private AudioPlayer _audioPlayer = null;
    [SerializeField]    private string _clipName = "";

    private void Awake()
    {
        if (!_audioPlayer)
            _audioPlayer = GetComponentInChildren<AudioPlayer>();

        if (!_gun)
            _gun = GetComponentInParent<Gun>();

        if (_gun)
            _gun.OnGunFired += _gun_OnShotFired;
    }

    private void _gun_OnShotFired(Gun sender)
    {
        if (_audioPlayer)
            _audioPlayer.PlayOneShot(_clipName);
    }
}
