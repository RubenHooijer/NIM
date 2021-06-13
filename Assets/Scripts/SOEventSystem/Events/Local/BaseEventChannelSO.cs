using System;
using UnityEngine;

public class BaseEventChannelSO<T1> : ScriptableObject {

    public Action<T1> OnEventRaised;

    public void Raise(T1 arg1) {
        if (OnEventRaised == null) { return; }

        OnEventRaised.Invoke(arg1);
    }

}

public class BaseEventChannelSO<T1, T2> : ScriptableObject {

    public Action<T1, T2> OnEventRaised;

    public void Raise(T1 arg1, T2 arg2) {
        if (OnEventRaised == null) { return; }

        OnEventRaised.Invoke(arg1, arg2);
    }

}

public class BaseEventChannelSO<T1, T2, T3> : ScriptableObject {

    public Action<T1, T2, T3> OnEventRaised;

    public void Raise(T1 arg1, T2 arg2, T3 arg3) {
        if (OnEventRaised == null) { return; }

        OnEventRaised.Invoke(arg1, arg2, arg3);
    }

}