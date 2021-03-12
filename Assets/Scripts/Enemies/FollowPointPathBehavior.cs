using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPointPathBehavior : MonoBehaviour
{
    public float speed;

    public int pathPointIndex = 0;

    public float moveUpdateRate = .02f;
    // Start is called before the first frame update
    void Start()
    {
        speed *= Mathf.Pow(GameController.instance.speedScaleBase, GameController.instance.waveNum - 1);

        if (GameController.instance.enemyPath.Length > 0)
            transform.position = GameController.instance.enemyPath[0].transform.position;
        StartCoroutine("MoveToTargets");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveToTargets()
    {
        for (pathPointIndex = 0; pathPointIndex < GameController.instance.enemyPath.Length; pathPointIndex++)
        {
            while (true)
            {
                Vector3 toMove = GameController.instance.enemyPath[pathPointIndex].transform.position - transform.position;

                if (toMove.magnitude < speed * moveUpdateRate)
                {
                    transform.position = GameController.instance.enemyPath[pathPointIndex].transform.position;
                    break;
                }
                else
                {
                    toMove = toMove.normalized * speed * moveUpdateRate;
                    transform.position += toMove;
                }

                yield return new WaitForSeconds(moveUpdateRate);
            }
        }

        OnMoveFinished();
    }

    void OnMoveFinished()
    {
        GameController.instance.OnEnemyExited?.Invoke(this.gameObject);
        Destroy(gameObject);
    }
}
