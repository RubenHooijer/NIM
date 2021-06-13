using UnityEngine;

public abstract class AbstractState : ScriptableObject {

    protected StateData data;
    protected AbstractFSM fsm { get; private set; }

    public void Initialize(AbstractFSM fsm) {
        this.fsm = fsm;
    }

    public void Activate(StateData stateData = null) {
        if(stateData != null) {
            data = stateData;
        }
        OnActivate();
    }

    public void Deactivate() {
        OnDeactivate();
    }

    protected virtual void OnActivate() { }

    public virtual void OnUpdate() { }

    protected virtual void OnDeactivate() { }

}