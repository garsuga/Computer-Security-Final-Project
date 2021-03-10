using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEmployeeTowerBehavior : HackableTower
{
    [Header("Shoot Settings")]
    public float shootInterval = 5f;
    public int maxProjectilesActive = -1;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public GameObject projectileTransform;
    public float projectileSpeed = 10f;
    public float projectileDamage = 35f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        TurnTowardsBehavior turnTowardsBehavior = GetComponentInParent<TurnTowardsBehavior>();

        GameObject target = null;

        shootObjectDetection.CanShoot += (timeSinceLastShot, projectilesActive) => {
            return timeSinceLastShot > shootInterval && (maxProjectilesActive < 0 || projectilesActive < maxProjectilesActive);
        };
        shootObjectDetection.CreateProjectile += () => {
            GameObject projectile = Instantiate<GameObject>(projectilePrefab);
            projectile.transform.position = projectileTransform.transform.position;
            projectile.transform.rotation = projectileTransform.transform.rotation;
            Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
            rigidbody.velocity = (target.transform.position - projectile.transform.position).normalized * projectileSpeed;
            return projectile;
        };
        shootObjectDetection.OnHit += (projectile, hitObject) =>
        {
            //Debug.Log("Projectile Hit");
            EnemyBehavior enemy = hitObject.GetComponent<EnemyBehavior>();
            // filter out hits on enemies that have already been destroyed
            if (enemy.IsDead)
                return false;
            enemy.Health -= projectileDamage;
            return true;
        };
        shootObjectDetection.OnTargetChange += (newTarget) => 
        {
            turnTowardsBehavior.target = newTarget;
            target = newTarget;
        };

        OnHacked += (hackedBy) =>
        {
            shootObjectDetection.enabled = false;
        };

        OnUnhacked += () =>
        {
            shootObjectDetection.enabled = true;
        };
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); 
    }
}
