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
}
