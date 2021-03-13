using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHookBehavior : MonoBehaviour
{
    public enum DestroyReason {
        HIT, TIMEOUT
    }
    public delegate bool OnHitHandler(GameObject other, Collider2D otherCollider);
    public delegate void OnDestroyHandler(DestroyReason reason);

    public float maxTimeAliveSeconds = 12;

    private float timeCreatedSeconds;

    public OnHitHandler OnHit
    {
        get;set;
    }

    public OnDestroyHandler OnDestroy
    {
        get;set;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeCreatedSeconds = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeCreatedSeconds > maxTimeAliveSeconds)
        {
            Destroy(this.gameObject);
            OnDestroy?.Invoke(DestroyReason.TIMEOUT);
        }
    }

    private bool destroyed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destroyed)
            return;
        bool? destroy = OnHit?.Invoke(collision.gameObject, collision);
        if(destroy.GetValueOrDefault(false))
        {
            Destroy(this.gameObject);
            destroyed = true;
            OnDestroy?.Invoke(DestroyReason.HIT);
        }
    }
}
