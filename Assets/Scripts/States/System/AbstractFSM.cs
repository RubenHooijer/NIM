using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractFSM : AbstractState {

    [SerializeField, ReadOnly] protected AbstractState currentState;

    public List<AbstractState> States { get; private set; }

    public void Initialize() {
        States = Resources.LoadAll<AbstractState>("States/").ToList();
    }

    public void GoToState<T>(StateData stateData = null) where T : AbstractState {
        GoToState(typeof(T), stateData);
    }

    public void GoToState(System.Type stateType, StateData stateData = null){
        if (!typeof(AbstractState).IsAssignableFrom(stateType)) { return; }
        AbstractState newState = GetOrCreateState(stateType);

        if (currentState != null) {
            currentState.Deactivate();
        }
        currentState = newState;
        Debug.Log($"<b>{this.GetType()}</b>: Entered <color=yellow>{stateType}</color>");
        currentState.Activate(stateData);
    }

    public T GetOrCreateState<T>() where T : AbstractState {
        return GetOrCreateState(typeof(T)) as T;
    }

    public AbstractState GetOrCreateState(System.Type stateType) {
        AbstractState state = States.Find(x => x.GetType() == stateType);

        if (state == null) {
            Debug.LogWarning($"State {stateType} not found, creating an instance through script");
            state = CreateInstance(stateType) as AbstractState;
            States.Add(state);
        }

        state.Initialize(this);
        return state;
    }

    public override void OnUpdate() {
        if (currentState == null) { return; }
        currentState.OnUpdate();
    }

    protected override void OnDeactivate() {
        if (currentState != null) {
            currentState.Deactivate();
        }
        currentState = null;
    }

}