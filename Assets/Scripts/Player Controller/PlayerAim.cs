using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JakePerry;

/* This script should be placed on each playable squad member in the scene. 
 * Handles player aiming. */
[DisallowMultipleComponent()]
public class PlayerAim : MonoBehaviour
{
    [Tooltip("Where the character's gun fires from. This should be an empty gameobject at the end of the gun barrel.")]
    [SerializeField]    private Transform _gunOrigin;
    private LogOnce _gunOriginWarningMessage = null;

    [SerializeField]    private LayerMask _aimableLayers = (LayerMask)1;

    [Tooltip("The minimum distance for an aim point. Prevents player from aiming at their feet and causing strange animations.")]
    [Range(0.0f, 2.0f), SerializeField] private float _minimumAimRange = 0.6f;

    [Header("Smart Aim")]
    [Range(2, 5), Tooltip("The number of iterations to make in each direction, positively and negatively, when searching for a better aim point.\nNOTE: Each extra iteration can result in up to 26 new raycast checks per frame (worst case scenario). Although less accurate, a lower value may slightly improve performance.")]
    [SerializeField]    private int _smartAimIterations = 3;
    [Tooltip("An array of tags that will be ignored by the smart aim functionality. If the cursor point is over a transform with a tag found in this array, the script will not attempt to find an optimal aim point.")]
    [SerializeField]    private string[] _smartAimIgnoreTags = new string[] { "Untagged" };

    [Tooltip("A list of tags the player can aim at. The player will aim at the first object under the mouse which has a tag included in this list.")]
    [SerializeField]    private string[] _aimTags = new string[] { "Untagged", "Enemy" };

    private Vector3 _aimMousePoint;
    private bool _aimingAtGeometry = false;     // Tracks whether or not the mouse is over aimable geometry
    private bool _pointFoundAtPlayerHeight = false;
    private Collider _mousePointCollider = null;
    private Vector3 _aimPoint = Vector3.zero;
    private Vector3 _previousValidAimPoint = Vector3.zero;

    /// <summary>
    /// Returns the point under the mouse. For the actual aim point, use GetAimPoint
    /// </summary>
    public Vector3 GetAimMousePoint
    {
        get { return _aimMousePoint; }
    }

    /// <summary>
    /// Returns the aim point. For the point under the mouse, use GetAimMousePoint
    /// </summary>
    public Vector3 GetAimPoint
    {
        get { return _aimPoint; }
    }
    public bool IsAiming
    {
        get { return _aimingAtGeometry || _pointFoundAtPlayerHeight; }
    }

    void Awake()
    {
        _gunOriginWarningMessage = new LogOnce("Warning: No gun origin transform specified on PlayerAim script.", LogOnce.LogType.Warning, gameObject);

        if (_aimableLayers == 0)
            Debug.LogWarning("Warning: Aim script on " + gameObject.name + " has Aimable Layers mask with no selected layers.");
    }
    
	void Start ()
    {
    }
	
	void Update ()
    {
        // Get the point under the mouse pointer
        Vector3 underMouse;
        _aimingAtGeometry = GetPointUnderMouse(out underMouse, out _mousePointCollider);
        
        // If mouse is not over geometry, find point on last valid hit's horizontal plane
        if (!_aimingAtGeometry)
            _pointFoundAtPlayerHeight = GetPointAtLastHeight(out underMouse);

        if (_aimingAtGeometry || _pointFoundAtPlayerHeight)
        {
            _aimMousePoint = underMouse;
            bool trackAsPrevious = false;

            // Find an optimal aim-point
            if (_gunOrigin)
            {
                if (_mousePointCollider)
                {
                    Collider target = _mousePointCollider.TopParentMatchingTag();
                    RaycastHit hit;
                    if (SightLineOptimizer.FindOptimalViewablePoint(out hit, _gunOrigin.position, _aimMousePoint,
                            target, true, (uint)_smartAimIterations, _aimTags, _aimableLayers, _smartAimIgnoreTags, 0.0f))
                    {
                        _aimPoint = hit.point;
                        trackAsPrevious = true;
                    }
                    else
                    {
                        _aimPoint = _aimMousePoint;
                        trackAsPrevious = true;
                    }
                }
                else
                {
                    // No collider under mouse (Point has been found off of geometry, on previous valid point's plane)
                    Vector3 hit;
                    if (RaycastForValidHit(out hit, _gunOrigin.position, _aimMousePoint - _gunOrigin.position))
                    {
                        _aimPoint = hit;
                        trackAsPrevious = true;
                    }
                    else
                    {
                        _aimPoint = underMouse;
                    }
                }
            }
            else
                _gunOriginWarningMessage.Log();

            // Ensure minimum aiming distance
            Vector3 toAimPoint = new Vector3(_aimPoint.x, transform.position.y, _aimPoint.z) - transform.position;
            if (toAimPoint.magnitude < _minimumAimRange)
            {
                toAimPoint = toAimPoint.normalized * _minimumAimRange;
                Vector3 newPoint = transform.position + toAimPoint;

                _aimPoint.x = newPoint.x;
                _aimPoint.z = newPoint.z;
            }

            if (trackAsPrevious)
                _previousValidAimPoint = _aimPoint;
        }
    }

    private bool RaycastForValidHit(out Vector3 point, Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin, direction);

        // Get an array of all raycasthits for this ray
        RaycastHit[] rayHits = Physics.RaycastAll(ray, 1000.0f, _aimableLayers.value, QueryTriggerInteraction.Ignore);

        if (rayHits.Length > 0)
        {
            // Sort by distance from camera
            rayHits.SortByDistance(origin);

            // Loop through each hit & compare to allowed tags to find the first valid hit
            RaycastHit firstHit = new RaycastHit();
            for (uint i = 0; i < rayHits.Length; i++)
            {
                RaycastHit hit = rayHits[i];

                // Find the hit's transform tag in the aimTags list
                string found = null;
                found = System.Array.Find(_aimTags, delegate (string s) {
                    return s == hit.transform.tag;
                });

                if (found != null)
                {
                    firstHit = hit;
                    break;
                }
            }

            // Check if a valid hit was found
            if (firstHit.transform)
            {
                point = firstHit.point;
                return true;
            }
        }

        point = Vector3.zero;
        return false;
    }

    /// <summary>Finds where the cursor's screenpoint-ray intersects the player's horizontal plane.</summary>
    /// <param name="point">Out parameter for the resulting point. Only use if function returns true.</param>
    /// <returns>Returns true if a point is found. Will only return false if the ray is parallel to the horizon.</returns>
    private bool GetPointAtLastHeight(out Vector3 point)
    {
        // Get ray through main camera @ mouse position
        Camera mainCam = Camera.main;
        Vector3 mainCamPos = mainCam.transform.position;
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, _previousValidAimPoint);
        float rayDist;
        if (plane.Raycast(mouseRay, out rayDist))
        {
            point = mouseRay.GetPoint(rayDist);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    /// <summary>Shoots a ray into the screen to find the first valid hit under the mouse pointer.</summary>
    /// <param name="point">Out parameter for the resulting point. Only use if function returns true.</param>
    /// <param name="collider">Out parameter for the collider hit by the raycast. Null if no collider is under the mouse.</param>
    /// <returns>Returns true if there is valid geometry under the mouse pointer</returns>
    private bool GetPointUnderMouse(out Vector3 point, out Collider collider)
    {
        // Get ray through main camera @ mouse position
        Camera mainCam = Camera.main;
        Vector3 mainCamPos = mainCam.transform.position;
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

        // Get an array of all raycasthits for this ray
        RaycastHit[] rayHits = Physics.RaycastAll(mouseRay, 1000.0f, _aimableLayers.value, QueryTriggerInteraction.Ignore);

        if (rayHits.Length > 0)
        {
            // Sort by distance from camera
            rayHits.SortByDistance(mainCamPos);

            // Loop through each hit & compare to allowed tags to find the first valid hit
            RaycastHit firstHit = new RaycastHit();
            for (uint i = 0; i < rayHits.Length; i++)
            {
                RaycastHit hit = rayHits[i];

                // Find the hit's transform tag in the aimTags list
                string found = null;
                found = System.Array.Find(_aimTags, delegate (string s) {
                    return s == hit.transform.tag;
                });

                if (found != null)
                {
                    firstHit = hit;
                    break;
                }
            }

            // Check if a valid hit was found
            if (firstHit.transform)
            {
                point = firstHit.point;
                collider = firstHit.collider;
                return true;
            }
        }

        point = Vector3.zero;
        collider = null;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (enabled && _aimingAtGeometry || _pointFoundAtPlayerHeight)
        {
            // Draw aiming ray
            if (_gunOrigin)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_gunOrigin.position, _aimMousePoint);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_gunOrigin.position, _aimPoint);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _aimMousePoint);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _aimPoint);
            }

            // Draw aim object bounds
            if (_mousePointCollider)
            {
                Gizmos.color = Color.red;
                Bounds b = _mousePointCollider.TopParentMatchingTag().GetGroupedBounds();
                Gizmos.DrawWireCube(b.center, b.size);
            }

            // Draw aim point + mouse point
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_aimPoint, 0.21f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_aimMousePoint, 0.19f);

            // Draw previous point for casting to plane
            Gizmos.DrawWireSphere(_previousValidAimPoint, 0.12f);
            Gizmos.DrawWireCube(_aimMousePoint, new Vector3(1, 0, 1));
        }
    }
}
