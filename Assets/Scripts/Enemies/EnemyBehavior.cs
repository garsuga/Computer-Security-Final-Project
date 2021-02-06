using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    public float maxHealth = 20;

    private float _health;
    public float Health
    {
        get {
            return _health;
        }

        set {
            if(value <= 0)
            {
                isEarlyDestroyed = true;
                Destroy(this.gameObject);
            }
            _health = value;
        }
    }

    private bool isEarlyDestroyed = false;

    public bool IsDead
    {
        get
        {
            return Health <= 0 || isEarlyDestroyed;
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        _health = maxHealth;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(IsDead && !isEarlyDestroyed)
        {
            isEarlyDestroyed = true;
            Destroy(this.gameObject);
        }
    }
}
