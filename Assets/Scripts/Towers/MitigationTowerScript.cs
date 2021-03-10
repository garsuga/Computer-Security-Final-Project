using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitigationTowerScript : TowerBehavior
{
    [Header("Mitigation Tower Settings")]
    public float attackDelaySeconds = .5f;
    public float attackIntervalSeconds = 2;

    public MitigationTowerScript() : base()
    {
        OnPlaced += (rootObject) =>
        {
            Collider2D targetCollider = shootObjectDetection.gameObject.GetComponent<Collider2D>();
            float radius = targetCollider.bounds.size.x / 2;
            GameController.DrawSquare(rootObject, Vector3.zero, radius, 0.02f, Color.red, false);
        };
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        shootObjectDetection.CanShoot += (timeSinceLastShot, numProjectiles) =>
        {
            return timeSinceLastShot > attackIntervalSeconds;
        };

        shootObjectDetection.DoShot += () =>
        {
            StartCoroutine(AttackAfterDelay());
        };
    }

    IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDelaySeconds);

        List<GameObject> valid = shootObjectDetection.ValidEnemiesInRange.FindAll(go => go != null);
        Debug.Log("Mitigation destroying " + valid.Count + " enemies.");
        valid.ForEach(go => Destroy(go));
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
