using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JakePerry;
using StarForceRecon;

public class Gun : MonoBehaviour, Equipment.IEquipment
{
    #region Delegates & Events

    public delegate void GunEventDelegate(Gun sender);
    public delegate void GunShotEventDelegate(Gun sender, Vector3 origin, Vector3 direction, Transform hitTransform);

    public event GunEventDelegate OnReloadSuccess;
    public event GunEventDelegate OnReloadFailed;

    /// <summary>Called once when the gun is fired.</summary>
    public event GunEventDelegate OnGunFired;

    /// <summary>
    /// Called per shot with directional information. May be called more than once
    /// when the gun is fired if ammoPerShot is greater than one.
    /// </summary>
    public event GunShotEventDelegate OnGunShotFired;

    #endregion

    #region General

    [SerializeField]
    private Transform _gunOrigin = null;

    [Tooltip("All excluded layers will be completely ignored, having no effect on the bullet.")]
    [SerializeField]
    private LayerMask _layerMask = (LayerMask)1;

    [Tooltip("A Mask defining which damage layers are affected by this gun.")]
    [SerializeField]
    private DamageLayer.Mask _damageMask = 0;
    [SerializeField]
    private DamageLayer.Modifier _damageModifier;

    [SerializeField]
    private GunData _gunData = null;

    #endregion

    #region Firing Variables
    
    public bool semiAuto
    {
        get { return _gunData.semiAuto; }
    }
    private int _semiAutoFireFrameCount = 0;
    private float _timeSinceLastFire = 0.0f;
    
    private float _fireWaitTime = 0.0f;

    #endregion

    #region Ammo Variables

    [SerializeField]    private uint _startAmmo = 100;
    private uint _currentAmmo = 0;
    public uint remainingAmmoUnloaded
    {
        get { return _currentAmmo; }
    }
    
    private uint _currentClip = 0;
    public uint ammoInClip
    { get { return _currentClip; } }
    
    private bool _isReloading = false;
    public bool reloadRequired
    {
        get { return (!_gunData.bottomlessClip && _currentClip == 0); }
    }
    private IEnumerator _reloadCoroutine = null;

    #endregion

    #region Heat Mechanics
    
    [SerializeField, HideInInspector]   private float _currentHeat = 0.0f;
    private bool _heatTriggerMustRelease = false;
    private bool _heatOverThresholdReleased = false;
    
    private bool _heatLockUp = false;

    #endregion

    #region NonAlloc Variables

    private RaycastHit _nonAllocHit;
    private RaycastHit[] _nonAllocHits = new RaycastHit[16];
    private Ray _nonAllocRay = new Ray();

    #endregion

    void Awake()
    {
        // Create clone of the specified gun data object
        if (_gunData != null)
        {
            GunData cloneData = Instantiate<GunData>(_gunData);
            _gunData = cloneData;
        }
        else
        {
            Destroy(this);
            throw new System.MissingFieldException("No GunData specified.");
        }

        // Initialize ammo
        _currentClip = (_gunData.clipSize < _startAmmo) ? _gunData.clipSize : _startAmmo;
        _currentAmmo = _startAmmo - _gunData.clipSize;

        // Get time to wait before each consecutive shot
        _fireWaitTime = 1 / (_gunData.fireRate / 60);
    }

    private void LateUpdate()
    {
        _timeSinceLastFire += Time.deltaTime;
        HandleCooling();
    }

    /// <summary>Safely raises the specified event if it has any registered listeners.</summary>
    private void RaiseEvent(GunEventDelegate e)
    {
        // Check there are any registered listeners before firing event
        GunEventDelegate del = e;
        if (del != null)
            e(this);
    }

    /// <summary>Safely raises the specified event if it has any registered listeners.</summary>
    private void RaiseEvent(GunShotEventDelegate e, Vector3 origin, Vector3 direction, Transform hitTransform)
    {
        // Check there are any registered listeners before firing event
        GunShotEventDelegate del = e;
        if (del != null)
            e(this, origin, direction, hitTransform);
    }
    
    private void HandleCooling()
    {
        if (_currentHeat > 0)
        {
            // Check if the gun has been idle long enough to start cooling down
            float timeBeforeCool = _gunData.coolingPauseTime.Evaluate(_currentHeat);
            if (_timeSinceLastFire >= timeBeforeCool)
            {
                _currentHeat -= Time.deltaTime * _gunData.heatLossPerSecond;
                if (_currentHeat < 0) _currentHeat = 0;

                if (_currentHeat < _gunData.coolingThreshold)
                    _heatLockUp = false;
            }
        }
    }

    private void ApplyHeat(uint shotCount)
    {
        float heatThisShot = (float)shotCount * _gunData.heatPerShot;
        _currentHeat += heatThisShot;

        if (_currentHeat > 1.0f)
            _currentHeat = 1.0f;

        if (_currentHeat > _gunData.overheatThreshold)
            _heatLockUp = true;
    }

    /// <summary>Fires the gun.</summary>
    /// <param name="usePlayerMechanics">Should semi-auto & heat mechanics be checked?</param>
    public void Fire(bool usePlayerMechanics)
    {
        // Has enough time elapsed between shots?
        bool fireRateQualified = (_timeSinceLastFire >= _fireWaitTime);

        // Has there been at least one frame where the gun was not fired? Semi-auto is ignored for AI
        bool semiAutoQualified = (Time.frameCount > _semiAutoFireFrameCount + 1)
                                    || (!usePlayerMechanics)
                                    || (!_gunData.semiAuto);

        // Trigger must be released before re-firing when heat reaches 100%
        if (_heatTriggerMustRelease)
            _heatTriggerMustRelease = !(Time.frameCount > _semiAutoFireFrameCount + 1);

        if ((_currentHeat >= _gunData.overheatThreshold) && !_heatOverThresholdReleased)
            _heatOverThresholdReleased = !(Time.frameCount == _semiAutoFireFrameCount + 1);

        // Has the gun been locked up due to heat? Firing may persist past this threshold if not cancelled prior
        bool heatQualified =    (!_heatLockUp || !_heatOverThresholdReleased)
                                && (_currentHeat < 1)
                                && (!_heatTriggerMustRelease);

        // Track this frame count for semi-auto functionality
        _semiAutoFireFrameCount = Time.frameCount;

        if (_currentHeat >= 1)
            _heatTriggerMustRelease = true;

        if (fireRateQualified && semiAutoQualified && heatQualified)
        {
            // Reset fire-rate tracking
            _timeSinceLastFire = 0.0f;
            _heatOverThresholdReleased = false;

            // Check ammo in clip
            if (_currentClip > 0 || _gunData.bottomlessClip)
            {
                uint ammoThisShot = _gunData.ammoPerShot;

                // Consume ammo
                if (!_gunData.bottomlessClip)
                {
                    // Only use ammo that is loaded in clip
                    if (_gunData.ammoPerShot > _gunData.clipSize)
                        ammoThisShot = _gunData.clipSize;

                    _currentClip -= ammoThisShot;
                }

                // Apply heat
                if (usePlayerMechanics && _gunData.usesHeat)
                    ApplyHeat(ammoThisShot);

                // Fire shots
                RaiseEvent(OnGunFired);
                for (int i = 0; i < ammoThisShot; i++)
                {
                    FireShot(_gunData.damage / _gunData.ammoPerShot);
                }
            }
        }
    }
    
    /// <returns>Vector3 direction for a random spread angle.</returns>
    private Vector3 GetSpreadDirection()
    {
        // Randomize a spread angle
        float angle = Random.Range(0, 360);
        float spread = Random.Range(0, _gunData.spread);
        float tanOfSpread = Mathf.Tan(spread * Mathf.Deg2Rad);
        
        float spreadX = Mathf.Sin(angle * Mathf.Deg2Rad) * tanOfSpread;
        float spreadY = Mathf.Cos(angle * Mathf.Deg2Rad) * tanOfSpread;

        // Get initial forward point
        Vector3 spreadPoint = _gunOrigin.forward + (_gunOrigin.up * spreadY) + (_gunOrigin.right * spreadX);

        return spreadPoint.normalized;
    }

    /// <summary>Used internally to fire a single shot using a raycast.</summary>
    private void FireShot(float damage)
    {
        Vector3 spreadDirection = GetSpreadDirection();

        // Raycast in this direction
        _nonAllocRay.origin = _gunOrigin.position;
        _nonAllocRay.direction = spreadDirection;
        int hits = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits, 1000.0f, (int)_layerMask);

        Transform hitTransform = null;
        if (hits > 0)
        {
            // Sort the hits array by distance from the shot origin to find the closest hit
            if (hits > 1)
            {
                for (int i = 0; i < hits - 1; i++)
                {
                    if (i == _nonAllocHits.Length) break;

                    // Get this hit & next hit
                    RaycastHit thisHit = _nonAllocHits[i];
                    RaycastHit nextHit = _nonAllocHits[i + 1];

                    // Compare hits, order by ascending distance
                    if (Vector3.Distance(thisHit.point, _gunOrigin.position) > Vector3.Distance(nextHit.point, _gunOrigin.position))
                    {
                        _nonAllocHits.SetValue(nextHit, i);
                        _nonAllocHits.SetValue(thisHit, i + 1);

                        // De-increment iterator
                        i = (i < 2) ? 0 : i - 2;
                    }
                }
            }

            _nonAllocHit = _nonAllocHits[0];
            Debug.DrawLine(_gunOrigin.position, _nonAllocHit.point, Color.blue, 0.5f);

            // Deal damage to the object hit
            DamageData damageData = new DamageData(this, damage, _damageMask, _damageModifier);
            hitTransform = _nonAllocHit.transform;
            IDamageable d = _nonAllocHit.transform.GetComponentInParent<IDamageable>();
            if (d != null)
                d.ApplyDamage(damageData);

        }
        else
            Debug.DrawRay(_gunOrigin.position, spreadDirection * 10, Color.blue, 0.5f);

        RaiseEvent(OnGunShotFired, _gunOrigin.position, spreadDirection, hitTransform);
    }

    /// <summary>Begins a reload for this gun.</summary>
    public void DoReload()
    {
        if (!_gunData.bottomlessClip
            && !(_currentClip == _gunData.clipSize)
            && !_isReloading)
        {
            // Check heat-lock state
            if (!(
                (_gunData.usesHeat && _currentHeat > _gunData.overheatThreshold)
                && _gunData.heatLocksReloading))
            {
                _reloadCoroutine = HandleReloadTimer();
                StartCoroutine(_reloadCoroutine);
            }
        }
    }
    
    private void CancelReload()
    {
        if (_reloadCoroutine != null
            && _isReloading)
            StopCoroutine(_reloadCoroutine);

        _isReloading = false;
    }

    private IEnumerator HandleReloadTimer()
    {
        uint reloadableAmmo = (_gunData.clipSize > _currentAmmo) ? _currentAmmo : _gunData.clipSize;

        // Does the gun have any ammo remaining for reload?
        if (reloadableAmmo > 0)
        {
            if (_gunData.reloadTime > 0)
            {
                _isReloading = true;
                yield return new WaitForSeconds(_gunData.reloadTime);
            }

            _isReloading = false;

            if (_gunData.infiniteAmmo)
                _currentClip = _gunData.clipSize;
            else
                _currentClip = reloadableAmmo;

            RaiseEvent(OnReloadSuccess);
        }
        else
        {
            // Raise event for reload failure
            RaiseEvent(OnReloadFailed);
        }
    }

    void Equipment.IEquipment.Use()
    {
        Fire(true);
    }
}
