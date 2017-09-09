using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "StarForceRecon/New Gun", order = 1)]
public class GunData : ScriptableObject
{
    #region Variables

    [Tooltip("Modifier for amount of damage done per shot. For multi-shot weapons, damage will be divided by the number of projectiles per shot.")]
    [Range(0.1f, 10.0f), SerializeField]        private float _damageModifier = 1.0f;
    [Range(0.0f, 10.0f), SerializeField]        private float _spread = 0.0f;

    [SerializeField]                            private bool _semiAuto = false;
    [Tooltip("Firing speed in Rounds per Minute")]
    [Range(30.0f, 900.0f), SerializeField]      private float _fireRateRPM = 450.0f;

    [SerializeField]                            private bool _bottomlessClip = false;
    [SerializeField]                            private bool _infiniteAmmo = false;

    [Range(1, 5), SerializeField]               private uint _ammoPerShot = 1;
    [Range(5, 100), SerializeField]             private uint _clipSize = 30;

    [Tooltip("Time in seconds taken to reload.")]
    [Range(0.0f, 10.0f), SerializeField]        private float _reloadTime = 0.0f;

    [SerializeField]                            private bool _useHeat = false;
    [SerializeField]                            private bool _heatLocksReload = true;

    [Tooltip("Can't begin firing if heat is over this amount. Firing may persist past this threshold if the trigger is not released.")]
    [Range(0.5f, 1.0f), SerializeField]         private float _overheatThreshold = 0.8f;
    [Tooltip("Firing will become available again when heat falls below this amount.")]
    [Range(0.0f, 0.5f), SerializeField]         private float _coolThreshold = 0.5f;

    [Range(0.01f, 0.5f), SerializeField]        private float _heatPerShot = 0.05f;
    [Range(0.1f, 1.0f), SerializeField]         private float _heatLossPerSecond = 0.3f;

    [Tooltip("The time in seconds the gun will need to be idle for before beginning to cool.")]
    [SerializeField]                            private AnimationCurve _coolingPauseTime = AnimationCurve.EaseInOut(0, 0.8f, 1, 2.2f);

    #endregion

    #region Getter properties

    /// <summary>Damage per fire event. Value is divided by number of projectiles in a shot.</summary>
    public float damageModifier { get { return _damageModifier; } }
    public float spread { get { return _spread; } }

    public bool semiAuto { get { return _semiAuto; } }

    /// <summary>Fire rate in Rounds Per Minute.</summary>
    public float fireRate { get { return _fireRateRPM; } }

    public bool bottomlessClip { get { return _bottomlessClip; } }
    public bool infiniteAmmo { get { return _infiniteAmmo; } }

    public uint ammoPerShot { get { return _ammoPerShot; } }
    public uint clipSize { get { return _clipSize; } }

    public float reloadTime { get { return _reloadTime; } }

    public bool usesHeat { get { return _useHeat; } }
    public bool heatLocksReloading { get { return _heatLocksReload; } }

    public float overheatThreshold { get { return _overheatThreshold; } }
    public float coolingThreshold { get { return _coolThreshold; } }

    public float heatPerShot { get { return _heatPerShot; } }
    public float heatLossPerSecond { get { return _heatLossPerSecond; } }

    public AnimationCurve coolingPauseTime { get { return _coolingPauseTime; } }

    #endregion
}
