using System.Linq;
using UnityEngine;

public static class RaycastingHelper
{
    private static RaycastHit tempHit1;

    public static void SortByDistanceNonAlloc(ref RaycastHit[] hits, Vector3 toPoint, int nonAllocHits)
    {
        if (nonAllocHits < 2) return;

        int elements = (nonAllocHits > hits.Length) ? hits.Length : nonAllocHits;
        int i = 0;
        while (i < elements - 1)
        {
            if (hits[i].transform == null || hits[i + 1].transform == null) break;

            float distanceToCurrent = Vector3.Distance(hits[i].point, toPoint);
            float distanceToNext = Vector3.Distance(hits[i+1].point, toPoint);

            if (distanceToNext < distanceToCurrent)
            {
                // Swap elements
                tempHit1 = hits[i];
                hits[i] = hits[i + 1];
                hits[i + 1] = tempHit1;
                if (i > 0) i--;
            }
            else
                i++;
        }
    }
    
    /// <returns>True if a valid hit is found, in which case validHit will be set.</returns>
    public static bool GetFirstValidHitNonAlloc(out RaycastHit validHit, ref RaycastHit[] hits, Ray ray, float maxDistance, 
        LayerMask layermask, string[] validTags)
    {
        int hitCount = Physics.RaycastNonAlloc(ray, hits, maxDistance, layermask);
        if (hitCount > 0)
        {
            SortByDistanceNonAlloc(ref hits, ray.origin, hitCount);

            foreach (var hit in hits)
            {
                if (validTags.Contains(hit.transform.tag))
                {
                    validHit = hit;
                    return true;
                }
            }
        }

        validHit = default(RaycastHit);
        return false;
    }
}
