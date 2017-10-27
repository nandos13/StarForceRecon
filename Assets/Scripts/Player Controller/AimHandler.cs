using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RootMotion.FinalIK;

namespace StarForceRecon
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AimIK))]
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

        public LayerMask AimLayers { get { return aimableLayers; } }
        public string[] AimTags { get { return aimTags; } }

        #endregion

        #region Aim Rotation

        [Tooltip("When aiming, the character will turn towards aimer when the angle from forward exceeds this value.")]
        [SerializeField, Range(20.0f, 80.0f)]
        private float maxHipSwivel = 50.0f;

        [Tooltip("Character's turning speed in rotations/second.")]
        [SerializeField, Range(1.0f, 2.0f)]
        private float turningSpeed = 1.0f;

        private float closeAimRadius = 0.5f;
        public float CloseAimRadius
        {
            get { return closeAimRadius; }
            set { closeAimRadius = value; }
        }

        private bool isSwiveling = false;

        #endregion

        [Tooltip("Where the character's gun fires from. This should be an empty gameobject at the end of the gun barrel.")]
        [SerializeField]
        private Transform gunOrigin = null;

        private AimIK aimIK = null;
        private Transform aimIKTarget = null;
        
        public Vector3 AimPoint { get { return aimIKTarget.position; } }

        RaycastHit[] rayHitsNonAlloc = new RaycastHit[16];

        #endregion

        #region Functionality

        private void Awake()
        {
            if (gunOrigin == null)
                throw new System.MissingFieldException("AimHandler requires a gun origin.");
            
            InitializeIKSystem();
        }

        private void InitializeIKSystem()
        {
            aimIK = GetComponent<AimIK>();
            GameObject obj = new GameObject(string.Format("AimIK Target {0}", aimIK.GetInstanceID()));
            obj.hideFlags = HideFlags.HideAndDontSave;
            aimIKTarget = obj.transform;
            aimIK.solver.target = aimIKTarget;
            aimIK.solver.transform = gunOrigin;
        }

        /// <summary>Handles aiming based on a viewport cursor position for player's character.</summary>
        /// /// <returns>The final aim point.</returns>
        public void HandlePlayerAiming(Vector2 viewportCoordinates)
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
                AimAtPoint(desiredTarget, colliderUnderCursor);
        }

        /// <summary>Attempts to aim at a target point in world space.</summary>
        /// <param name="desiredTarget">The world-space target to aim at.</param>
        /// <param name="targetCollider">Optional. 
        /// <para>If specified, Smart Aiming functionality will be used to attempt to find an optimal aim point on the collider.</para></param>
        /// <returns>The final aim point.</returns>
        public void AimAtPoint(Vector3 desiredTarget, Collider targetCollider = null)
        {
            Vector3 targetPoint = desiredTarget;
            if (targetCollider != null)
            {
                Collider ancestorCollider = GetAncestorWithSameTag(targetCollider);
                RaycastHit hit;
                if (JakePerry.SightLineOptimizer.FindOptimalViewablePoint(out hit, gunOrigin.position, desiredTarget,
                                ancestorCollider, true, (uint)smartAimIterations, aimTags, aimableLayers, smartAimIgnoreTags, 0.0f))
                    targetPoint = hit.point;
            }
            
            aimIKTarget.position = targetPoint;
            DoSwivel(gunOrigin.forward);
        }

        /// <summary>Swivels character to face aim direction.</summary>
        private void DoSwivel(Vector3 direction)
        {
            Vector3 yLevelOffset = new Vector3(direction.x, transform.position.y, direction.z);
            float aimTheta = Vector3.Angle(transform.forward, yLevelOffset);
            
            if (isSwiveling || aimTheta > maxHipSwivel)
            {
                // Rotate to face aim point
                float turnSpeed = (turningSpeed * 360.0f) / aimTheta;
                Quaternion rotation = Quaternion.LookRotation(yLevelOffset);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            
                float swivelStopAngle = isSwiveling ? maxHipSwivel / 2 : maxHipSwivel;
                isSwiveling = Vector3.Angle(transform.forward, yLevelOffset) > swivelStopAngle;
            }
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
                if (tags.Contains(hitArray[i].transform.tag))
                {
                    match = hitArray[i];
                    return true;
                }
            }
            match = default(RaycastHit);
            return false;
        }

        #endregion
    }
}
