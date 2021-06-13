using UnityEngine;

public class FSMRunner : MonoBehaviour {

    [SerializeField] private GameFSM gameFSM;

    private void Start() {
        gameFSM.Initialize();
        gameFSM.Activate();
    }

    private void Update() {
        gameFSM.OnUpdate();
    }

}