using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLoc
    {
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }

    public enum eType { center, inset, outset };

    [Header("Inscribed")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic")]
    public eScreenLoc screenLocs = eScreenLoc.onScreen;
    public float camWidth;
    public float camHeight;

    public bool isOnScreen
    {
        get { return (screenLocs == eScreenLoc.onScreen); }
    }

    public bool LocIs(eScreenLoc checkLoc)
    {
        if (checkLoc == eScreenLoc.onScreen) return isOnScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }
    
    void LateUpdate()
    {
        float checkRadius = 0;

        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;

        screenLocs = eScreenLoc.onScreen;

        if (pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
            screenLocs |= eScreenLoc.offRight;
        }
        if (pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
            screenLocs |= eScreenLoc.offLeft;
        }
        if (pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
            screenLocs |= eScreenLoc.offUp;
        }
        if (pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
            screenLocs |= eScreenLoc.offDown;
        }
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            screenLocs = eScreenLoc.onScreen;
        }
    }


}
