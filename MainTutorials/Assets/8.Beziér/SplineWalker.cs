using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public bool lookForward;
    public BezierSpline spline;
    public float duration;
    public SplineWalkerMode mode;

    float progress;
    private bool goingForwaed = true;

    private void Update()
    {
        if (goingForwaed)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForwaed = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime/duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForwaed = true;
            }
        }
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = spline.GetPoint(progress);
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }

}
