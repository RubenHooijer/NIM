using Oasez.Extensions.Generics.Singleton;
using System;
using System.Collections;
using UnityEngine;

public class CoroutineHelper : GenericSingleton<CoroutineHelper, CoroutineHelper> {

    public void StartRoutine(IEnumerator routine) {
        StartCoroutine(routine);
    }

    public static void Delay(float delay, Action onEndDelay) {
        Instance.StartCoroutine(DelayRoutine(delay, onEndDelay));
    }

    private static IEnumerator DelayRoutine(float delay, Action onEndDelay) {
        yield return new WaitForSeconds(delay);
        onEndDelay?.Invoke();
    }

}