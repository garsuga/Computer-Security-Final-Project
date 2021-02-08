using System.Collections;
using System;


public interface UI_Effect
{
    IEnumerator Execute();
    event Action<UI_Effect> OnComplete;
}

