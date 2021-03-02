using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialistEmployeeTower : TowerBehavior
{
    [Header("Shoot Settings")]
    public float shootInterval = 5f;
    public int maxProjectilesActive = -1;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public GameObject projectileTransform;
    public float projectileSpeed = 10f;
    public float projectileDamage = 35f;

    [Header("Tower Repair Settings")]
    public float repairTimeSeconds = 2;
    public bool isRepairingTower = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
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
            EnemyBehavior enemy = hitObject.GetComponent<EnemyBehavior>();
            // filter out hits on enemies that have already been destroyed
            if (enemy.IsDead)
                return false;
            enemy.Health -= projectileDamage;
            return true;
        };
        shootBehavior.OnTargetChange += (newTarget) => turnTowardsBehavior.target = newTarget;
    }

    public IEnumerator RepairTower(HackableTower tower)
    {
        if (isRepairingTower)
            throw new System.Exception("Cannot repair tower, already repairing!");
        LineRenderer renderer = GameController.DrawPath(tower.gameObject, new Vector3[] { tower.transform.position, transform.position }, .035f, Color.green, true);
        isRepairingTower = true;
        yield return new WaitForSeconds(repairTimeSeconds);
        isRepairingTower = false;
        tower.Hacked = false;
        Destroy(renderer);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
