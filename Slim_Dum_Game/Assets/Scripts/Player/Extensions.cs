using UnityEngine;

public static class Extensions
{
    public static bool Includes(this LayerMask mask, int layer)
    {
        return (mask & 1 << layer) > 0;
    }
}

