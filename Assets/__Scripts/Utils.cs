using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{

//== Bezier Curves ===================================================================================\\
    static public Vector3 Bezier(float u, params Vector3[] points)
    {
        Vector3[,] vArr = new Vector3[points.Length, points.Length];

        int r = points.Length - 1;
        for (int i = 0; i < points.Length; ++i)
        {
            vArr[r, i] = points[i];
        }

        for (--r; r >= 0; --r)
        {
            for (int i = 0; i <= r; ++i)
            {
                vArr[r, i] = Vector3.LerpUnclamped(vArr[r + 1, i], vArr[r + 1, i + 1], u);
            }
        }
        return vArr[0, 0];
    }

//== Materials Functions =============================================================================\\
    static public Material[] GetAllMaterials(GameObject go)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        Material[] mats = new Material[rends.Length];
        for (int i = 0; i < rends.Length; ++i)
        {
            mats[i] = rends[i].material;
        }

        return mats;
    }
}
