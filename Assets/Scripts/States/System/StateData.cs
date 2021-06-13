using UnityEngine;

public class StateData {

    private object[] parameters;

    public StateData(params object[] parameters) {
        this.parameters = parameters;
    }

    public bool HasParameterAtIndex(int index) {
        return parameters.Length > index;
    }

    public T Get<T>(int index) {
        if (index < 0 || index >= parameters.Length) {
            Debug.LogError("Tried getting parameters of type " + typeof(T) + " but at index " + index + " but the index is out of bounds.");
            return default(T);
        }
        if (parameters[index] is T == false) {
            Debug.LogError("Tried getting parameters of type " + typeof(T) + " but found " + parameters[index].GetType() + " at index " + index);
        }

        return (T)parameters[index];
    }

}