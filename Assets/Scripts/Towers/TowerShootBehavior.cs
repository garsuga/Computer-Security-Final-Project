using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShootBehavior : MonoBehaviour
{
    /// <summary>
    /// Event called to create new projectile
    /// </summary>
    /// <returns>Projectile object</returns>
    public delegate GameObject CreateProjectileHandler();
    /// <summary>
    /// Event which reports on a projectile hit and returns completion status
    /// </summary>
    /// <param name="projectile">Reference to projectile object which did hit</param>
    /// <param name="hitObject">Reference to object which was hit</param>
    /// <returns>True if hit should end propogation</returns>
    public delegate bool OnHitHandler(GameObject projectile, GameObject hitObject);
    /// <summary>
    /// Event called to check for creation of new projectile
    /// </summary>
    /// <param name="timeSinceLastSpawn">Time in seconds since last projectile spawn</param>
    /// <param name="numProjectilesActive">Number of projectiles not currently destroyed</param>
    /// <returns>True if next projectile should be spawned</returns>
    public delegate bool CanShootHandler(float timeSinceLastSpawn, int numProjectilesActive);
    /// <summary>
    /// Event called when a new target is set to the current target
    /// </summary>
    /// <param name="target">Reference to the new target's GameObject</param>
    public delegate void OnTargetChangeHandler(GameObject target);

    /// <summary>
    /// Tags which can be targeted
    /// </summary>
    public string[] validTargetTags = new string[0];

    private List<string> _validTagsList = null;
    public List<string> ValidTags
    {
        get {
            if (_validTagsList == null)
                _validTagsList = new List<string>(validTargetTags);
            return _validTagsList;
        }
    }

    /// <summary>
    /// List of colliders to ignore (includes <see cref="rangeCollider">rangeCollider</see> by default)
    /// </summary>
    public Collider2D[] ignoredColliders = new Collider2D[0];

    private List<Collider2D> _ignoredColliders = null;
    public List<Collider2D> IgnoredColliders
    {
        get
        {
            if (_ignoredColliders == null)
                _ignoredColliders = new List<Collider2D>(ignoredColliders);
            return _ignoredColliders;
        }
    }


    public CreateProjectileHandler CreateProjectile
    {
        get;set;
    }

    public OnHitHandler OnHit
    {
        get;set;
    }

    public CanShootHandler CanShoot
    {
        get;set;
    }

    public OnTargetChangeHandler OnTargetChange
    {
        get;set;
    }

    

    private GameObject currentTarget = null;
    private List<GameObject> withinRange = new List<GameObject>();
    private float timeLastShot;
    private int projectilesActive = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeLastShot = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        // check required events
        if(CreateProjectile == null || CanShoot == null || OnHit == null)
        {
            return;
        }
        if(currentTarget == null)
        {
            SetNextTargetIfPossible();
        }

        if(currentTarget != null)
        {
            if(CanShoot(Time.realtimeSinceStartup - timeLastShot, projectilesActive))
            {
                timeLastShot = Time.realtimeSinceStartup;
                GameObject projectile = CreateProjectile();
                ProjectileHookBehavior projectileHook = projectile.AddComponent<ProjectileHookBehavior>();
                projectileHook.enabled = true;
                projectileHook.OnHit += (hitObject, hitCollider) => { 
                    if (IsValidTarget(hitObject, hitCollider)) 
                        return OnHit(projectile, hitObject);

                    return false;
                };
                projectileHook.OnDestroy += (reason) => projectilesActive--;
            }
        }
    }

    private void SetNextTargetIfPossible()
    {
        while(withinRange.Count > 0)
        {
            GameObject candidate = withinRange[withinRange.Count - 1];
            withinRange.RemoveAt(withinRange.Count - 1);
            if (candidate != null && candidate.activeSelf)
            {
                currentTarget = candidate;
                OnTargetChange?.Invoke(candidate);
                return;
            }
        }

        OnTargetChange?.Invoke(null);
    }

    private bool IsValidTarget(GameObject hit, Collider2D collision)
    {
        // filter self hits
        if (hit == this)
            return false;

        // filter ignored colliders
        if (IgnoredColliders.Contains(collision))
            return false;

        // filter null hits and invalid tags
        if (hit == null || !ValidTags.Contains(hit.tag))
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision has entered tower collider");
        GameObject hit = collision.gameObject;

        if (!IsValidTarget(hit, collision))
            return;

        withinRange.Add(hit.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Collision has exited tower collider");
        if (withinRange.Contains(collision.gameObject))
            withinRange.Remove(collision.gameObject);

        if(currentTarget == collision.gameObject)
        {
            currentTarget = null;
            SetNextTargetIfPossible();
        }
    }
}
