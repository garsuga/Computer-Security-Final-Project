using System;
using System.Collections;
using UnityEngine;


public class ShakeRectEffect : UI_Effect
{
    public event Action<UI_Effect> OnComplete;
    private RectTransform RectTransform { get; }
    private float WiggleSpeed { get; }
    private float MaxRotation { get; }
    
    
    public ShakeRectEffect(RectTransform rectTransform, float maxRotation, float speed, Action<UI_Effect> onComplete = null)
    {
        RectTransform = rectTransform;
        WiggleSpeed = speed;
        MaxRotation = maxRotation;
        OnComplete += onComplete;
    }

    public IEnumerator Execute()
    {
        var rotateTo = new Quaternion
        {
            eulerAngles = new Vector3(0, 0, MaxRotation)
        };


        var currentRotation = RectTransform.rotation.z;
        var nextRotation = MaxRotation * -1f;

        var time = 0f;

        while (Mathf.Abs(nextRotation) > 0.15f)
        {
            time += Time.deltaTime * WiggleSpeed;
            var newRotation = Mathf.Lerp(currentRotation, nextRotation, time);
            rotateTo.eulerAngles = new Vector3(0, 0, newRotation);
            RectTransform.rotation = rotateTo;
            if (time >= 1)
            {
                currentRotation = nextRotation;
                nextRotation = (nextRotation * 0.9f) * -1;
                time = 0;
            }

            yield return null;
        }

        rotateTo.eulerAngles = new Vector3(0, 0, 0);
        RectTransform.rotation = rotateTo;

        OnComplete?.Invoke(this);
    }
}
