using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEmployeeTowerBehavior : TowerBehavior
{
    public GameObject projectilePrefab;
    public GameObject projectileTransform;

    public float shootInterval = 5f;
    public int maxProjectilesActive = -1;

    public float projectileSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        TurnTowardsBehavior turnTowardsBehavior = GetComponentInParent<TurnTowardsBehavior>();

        shootBehavior.CanShoot += (timeSinceLastShot, projectilesActive) => {
            return timeSinceLastShot > shootInterval && (maxProjectilesActive < 0 || projectilesActive < maxProjectilesActive);
        };
        shootBehavior.CreateProjectile += () => {
            GameObject projectile = Instantiate<GameObject>(projectilePrefab);
            projectile.transform.position = projectileTransform.transform.position;
            projectile.transform.rotation = projectileTransform.transform.rotation;
            Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
            rigidbody.velocity = projectile.transform.right * projectileSpeed;
            return projectile;
        };
        shootBehavior.OnHit += (projectile, hitObject) =>
        {
            //Debug.Log("Projectile Hit");
            Destroy(hitObject);
            return true;
        };
        shootBehavior.OnTargetChange += (newTarget) => turnTowardsBehavior.target = newTarget;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
