using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhishingDetectionTowerBehavior : TowerBehavior
{
    [Header("Phishing Detection Tower Settings")]
    public float attackDelaySeconds = .5f;
    public float freezeTimeSeconds = 3;
    public string tagForEnemiesFrozen = "Exposed Enemy";
    public float attackIntervalSeconds = 2;

    public delegate void Callable();

    public PhishingDetectionTowerBehavior() : base()
    {
        OnPlaced += (rootObject) =>
        {
            Collider2D targetCollider = shootObjectDetection.gameObject.GetComponent<Collider2D>();
            float radius = targetCollider.bounds.size.x / 2;
            GameController.DrawSquare(rootObject, Vector3.zero, radius, 0.02f, Color.red, false);
        };
    }

    public override void Start()
    {
        base.Start();

        shootObjectDetection.CanShoot += (timeSinceLastShot, projectilesActive) =>
        {
            return timeSinceLastShot >= attackIntervalSeconds; 
        };

        shootObjectDetection.DoShot += () =>
        {
            StartCoroutine(AttackAfterDelay(attackDelaySeconds));
        };
    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator AttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        List<Callable> undoAffectsActions = new List<Callable>();

        shootObjectDetection.ValidEnemiesInRange.ForEach(e =>
        {
            if(e != null)
            {
                string priorTag = e.tag;
                e.tag = tagForEnemiesFrozen;

                FollowPointPathBehavior movement = e.GetComponentInChildren<FollowPointPathBehavior>();
                float priorSpeed = movement.speed;
                movement.speed = 0;

                undoAffectsActions.Add(() =>
                {
                    if (e != null)
                    {
                        e.tag = priorTag;
                        movement.speed = priorSpeed;
                    }
                });
            }
        });

        yield return new WaitForSeconds(freezeTimeSeconds);

        foreach(Callable c in undoAffectsActions)
        {
            c();
        }
    }
}
