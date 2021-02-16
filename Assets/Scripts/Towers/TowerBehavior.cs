using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerBehavior : MonoBehaviour
{
    [Header("General Tower Settings")]
    public int towerCost = 1000;
    /// <summary>
    /// Reference to the shoot behavior which handles boilerplate shooting mechanics
    /// </summary>
    public TowerShootBehavior shootBehavior;
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
