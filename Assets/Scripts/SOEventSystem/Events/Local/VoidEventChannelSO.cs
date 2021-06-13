using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/Events/Local/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject {

    public Action OnEventRaised;

    public void Raise() {
        if (OnEventRaised == null) { return; }

        OnEventRaised.Invoke();
    }
    
}