using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardsBehavior : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        LookAt2D(transform, target.transform.position);
    }

    // https://forum.unity.com/threads/2d-look-at-object-disappears.390105/
    public void LookAt2D(Transform transform, Vector3 target)
    {
        Vector3 current = transform.position;
        var direction = target - current;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
