using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittedTextBehavior : MonoBehaviour
{
    public float lifetimeSeconds = 1.5f;
    public string text = "null";
    public Color color = Color.white;
    public int fontSize = 50;
    public Vector3 velocity = new Vector3(0, .1f);
    public TextMesh textMesh;

    private float bornAt;
    

    // Start is called before the first frame update
    void Start()
    {
        textMesh.text = text;
        textMesh.color = color;
        textMesh.fontSize = fontSize;

        bornAt = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > bornAt + lifetimeSeconds)
        {
            Destroy(gameObject);
        } else
        {
            transform.position += velocity * Time.deltaTime;
            color.a = 1 - Mathf.Clamp((Time.time - bornAt) / lifetimeSeconds, 0, 1);
            textMesh.color = color;
        }
    }
}
