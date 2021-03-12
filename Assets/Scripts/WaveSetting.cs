using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveSetting
{
    public delegate void Callable();

    //private Dictionary<GameObject, int> enemySets;
    

    public int EnemiesPerTick
    {
        get;
    }

    public Callable WaveCallable
    {
        get;
    }

    public string Name
    {
        get;
    }

    public Dictionary<GameObject, int> EnemySets
    {
        get;
    }

    public WaveSetting(string internalName, Dictionary<GameObject, int> enemySets, int enemiesPerTick, Callable runOnWave)
    {
        this.Name = internalName;
        this.EnemySets = enemySets;
        this.EnemiesPerTick = enemiesPerTick;
        this.WaveCallable = runOnWave;
    }

    
}
