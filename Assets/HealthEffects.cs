using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthEffects : MonoBehaviour
{
    private MonoBehaviour Owner { get; }
    private readonly List<UI_Effect> _effects = new List<UI_Effect>();

    private int _completedEffects = 0;

    public event Action OnAllEffectsComplete;

    public HealthEffects(MonoBehaviour owner) { Owner = owner; }

    public HealthEffects AddEffect(UI_Effect effect)
    {
        _effects.Add(effect);
        effect.OnComplete += OnEffectComplete;
        return this;
    }
    public void ExecuteEffects()
    {
        Owner.StopAllCoroutines();
        foreach (var effect in _effects)
        {
            Owner.StartCoroutine(effect.Execute());
        }
    }
    //Helper Functions
    private void OnEffectComplete(UI_Effect effect)
    {
        _completedEffects += 1;
        if (_completedEffects < _effects.Count) return;
        OnAllEffectsComplete();
    }
    private void AllEffectsComplete()
    {
        _completedEffects = 0;
        OnAllEffectsComplete?.Invoke();
    }
}
