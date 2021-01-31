using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPointPathBehavior : MonoBehaviour
{

    GameController controller;

    public float speed;

    public float moveUpdateRate = .02f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        StartCoroutine("MoveToTargets");
        if(controller.enemyPath.Length > 0)
            transform.position = controller.enemyPath[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveToTargets()
    {
        for (int i = 0; i < controller.enemyPath.Length; i++)
        {
            while (true)
            {
                Vector3 toMove = controller.enemyPath[i].transform.position - transform.position;

                if (toMove.magnitude < speed * moveUpdateRate)
                {
                    transform.position = controller.enemyPath[i].transform.position;
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
        Destroy(gameObject);
    }
}
