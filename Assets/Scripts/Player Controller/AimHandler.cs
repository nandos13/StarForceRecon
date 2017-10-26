using System.Collections.Generic;
using UnityEngine;

namespace StarForceRecon
{
    [DisallowMultipleComponent]
    public class AimHandler : MonoBehaviour
    {
        #region Variables

        #region Target Masking

        [SerializeField]
        private LayerMask aimableLayers = (LayerMask)1;

        [Tooltip("The number of iterations to make in each direction, positively and negatively, when searching for a better aim point.\nNOTE: Each extra iteration can result in up to 26 new raycast checks per frame (worst case scenario). Although less accurate, a lower value may slightly improve performance.")]
        [SerializeField, Range(2, 5)]
        private int smartAimIterations = 3;
        [Tooltip("Tags ignored by smart aim. If the cursor is over a transform with contained tag, finding an optimal aim point will not be attempted.")]
        [SerializeField]
        private string[] smartAimIgnoreTags = new string[] { "Untagged" };
        [Tooltip("Tags which can be aimed at. All other tags will be ignored.")]
        [SerializeField]
        private string[] aimTags = new string[] { "Untagged", "Enemy" };

        #endregion

        [Tooltip("Where the character's gun fires from. This should be an empty gameobject at the end of the gun barrel.")]
        [SerializeField]
        private Transform gunOrigin = null;

        #region Data Tracking

        Vector3 _lastAimPoint = Vector3.zero;
        public Vector3 aimPoint { get { return _lastAimPoint; } }

        #endregion

        RaycastHit[] rayHitsNonAlloc = new RaycastHit[16];

        #endregion

        #region Functionality

        private void Awake()
        {
            if (gunOrigin == null)
                throw new System.MissingFieldException("AimHandler requires a gun origin.");
        }

        /// <summary>Handles aiming based on a viewport cursor position for player's character.</summary>
        /// /// <returns>The final aim point.</returns>
        public Vector3 HandlePlayerAiming(Vector2 viewportCoordinates)
        {
            // Find the desired target point under the cursor (at viewportCoordinates)
            Vector3 desiredTarget;
            Collider colliderUnderCursor = null;
            bool cursorIsOverObject = GetDesiredTargetFromViewport(out desiredTarget, out colliderUnderCursor, viewportCoordinates);

            // If cursor is not over an object, raycast to character's horizontal plane
            bool targetFoundAtCharacterHeight = false;
            if (!cursorIsOverObject)
                targetFoundAtCharacterHeight = CharacterHorizonIntersect(viewportCoordinates, out desiredTarget);

            // Attempt to aim at the desired target point
            if (cursorIsOverObject || targetFoundAtCharacterHeight)
                return AimAtPoint(desiredTarget, colliderUnderCursor);

            return transform.position;
        }

        /// <summary>Attempts to aim at a target point in world space.</summary>
        /// <param name="desiredTarget">The world-space target to aim at.</param>
        /// <param name="targetCollider">Optional. 
        /// <para>If specified, Smart Aiming functionality will be used to attempt to find an optimal aim point on the collider.</para></param>
        /// <returns>The final aim point.</returns>
        public Vector3 AimAtPoint(Vector3 desiredTarget, Collider targetCollider = null)
        {
            Vector3 targetPoint;
            if (targetCollider != null) // Use Smart Aiming to find an optimal point
            {
                Collider ancestorCollider = GetAncestorWithSameTag(targetCollider);
                RaycastHit hit;
                if (JakePerry.SightLineOptimizer.FindOptimalViewablePoint(out hit, gunOrigin.position, desiredTarget,
                                ancestorCollider, true, (uint)smartAimIterations, aimTags, aimableLayers, smartAimIgnoreTags, 0.0f))
                    targetPoint = hit.point;
                else
                    targetPoint = desiredTarget;
            }
            else
            {
                Vector3 hit;
                if (SingleLineCheck(out hit, gunOrigin.position, desiredTarget - gunOrigin.position))
                    targetPoint = hit;
                else
                    targetPoint = desiredTarget;
            }

            _lastAimPoint = targetPoint;
            return targetPoint;
        }

        private bool SingleLineCheck(out Vector3 point, Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            int hits = Physics.RaycastNonAlloc(ray, rayHitsNonAlloc, 1000.0f, aimableLayers.value, QueryTriggerInteraction.Collide);
            if (hits > 0)
            {
                // Sort hits by distance
                RaycastingHelper.SortByDistanceNonAlloc(ref rayHitsNonAlloc, origin, hits);
                RaycastHit hit;
                if (FirstMatchingTag(out hit, rayHitsNonAlloc, aimTags, hits))
                {
                    point = hit.point;
                    return true;
                }
            }

            point = Vector3.zero;
            return false;
        }
        
        /// <returns>Ascends collider's heirarchy to find top parent collider with same tag.
        /// <para>Breaks search when a parent with differing tag is found.</para></returns>
        private Collider GetAncestorWithSameTag(Collider collider)
        {
            string tag = collider.transform.tag;
            Transform t = collider.transform;

            Stack<Transform> stack = new Stack<Transform>();
            stack.Push(t);

            // Ascend heirarchy & track all parent transforms with matching tag
            while (t.parent != null)
            {
                if (t.parent.tag == tag)
                {
                    t = t.parent;
                    stack.Push(t);
                }
                else break;
            }

            // Find first collider in stack
            Collider result = null;
            do
            {
                Transform topOfStack = stack.Pop();
                result = topOfStack.GetComponent<Collider>();

            } while (result == null && stack.Count > 0);

            // Return initial collider if no higher collider was found
            if (result == null)
                return collider;

            return result;
        }

        /// <summary>Finds where ray through viewport intersects with character's horizontal plane.</summary>
        /// <returns>True if a point was found. False if ray is parallel to horizontal plane or ray direction moves away from plane.</returns>
        private bool CharacterHorizonIntersect(Vector2 viewportCoordinates, out Vector3 intersect)
        {
            Plane characterHorizon = new Plane(Vector3.up, transform.position);
            Ray cursorRay = Camera.main.ViewportPointToRay(viewportCoordinates);

            float intersectDistance;
            if (characterHorizon.Raycast(cursorRay, out intersectDistance))
            {
                intersect = cursorRay.GetPoint(intersectDistance);
                return true;
            }

            intersect = Vector3.zero;
            return false;
        }

        /// <summary>Calculates an aim target from a viewport location.</summary>
        private bool GetDesiredTargetFromViewport(out Vector3 desiredTarget, out Collider colliderUnderCursor, Vector2 viewportCoordinates)
        {
            Ray cursorRay = Camera.main.ViewportPointToRay(viewportCoordinates);

            int hits = Physics.RaycastNonAlloc(cursorRay, rayHitsNonAlloc,
                            1000.0f, aimableLayers.value, QueryTriggerInteraction.Collide);

            if (hits > 0)
            {
                RaycastingHelper.SortByDistanceNonAlloc(ref rayHitsNonAlloc, Camera.main.transform.position, hits);

                RaycastHit hit;
                if (FirstMatchingTag(out hit, rayHitsNonAlloc, aimTags, hits))
                {
                    desiredTarget = hit.point;
                    colliderUnderCursor = hit.collider;
                    return true;
                }
            }

            desiredTarget = Vector3.zero;
            colliderUnderCursor = null;
            return false;
        }

        /// <summary>Outputs first hit in hitArray with a tag found in tags. Returns false if no match was found.</summary>
        /// <param name="hits">Number of elements in hitArray to check.</param>
        private bool FirstMatchingTag(out RaycastHit match, RaycastHit[] hitArray, string[] tags, int hits)
        {
            for (uint i = 0; i < hits; i++)
            {
                for (uint j = 0; j < aimTags.Length; j++)
                {
                    if (hitArray[i].transform.tag == aimTags[j])
                    {
                        match = hitArray[i];
                        return true;
                    }
                }
            }
            match = default(RaycastHit);
            return false;
        }

        #endregion
    }
}
