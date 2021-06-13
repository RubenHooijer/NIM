using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractPopUp<T> : AbstractScreen<T> where T : Component {

    public readonly UnityEvent ClosePopUpEvent = new UnityEvent();

}