using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool isRepairingTower = false;

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

        GameController.instance.OnRoundEnd += (roundNum) =>
        {
            if (this != null && this.isPlaced)
            {
                foreach (HackableTower ht in FindObjectsOfType<HackableTower>().OrderBy(e => Random.value))
                {
                    if(ht != null && ht.Hacked)
                    {
                        StartCoroutine(RepairTower(ht));
                        return;
                    }
                }
            }
        };
    }

    public IEnumerator RepairTower(HackableTower tower)
    {
        if (isRepairingTower)
            throw new System.Exception("Cannot repair tower, already repairing!");

        tower.Hacked = false;

        LineRenderer renderer = GameController.DrawPath(tower.gameObject, Vector3.zero, new Vector3[] { tower.transform.position, transform.position }, .035f, Color.green, true);
        isRepairingTower = true;
        yield return new WaitForSeconds(repairTimeSeconds);
        isRepairingTower = false;
        Destroy(renderer);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
