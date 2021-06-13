using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/State/GameFSM")]
public class GameFSM : AbstractFSM {

    public static GameFSM Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<GameFSM>("GameFSM");
                if (instance == null) {
                    instance = CreateInstance<GameFSM>();
                    instance.GoToState<MenuState>();
                }
            }
            return instance;
        }
    }
    private static GameFSM instance;

    [SerializeField] private AbstractState startingState;

    protected override void OnActivate() {
        GoToState(startingState.GetType());
    }

}