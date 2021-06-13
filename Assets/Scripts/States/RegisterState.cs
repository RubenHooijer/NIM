using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/State/RegisterState")]
public class RegisterState : AbstractState {

    protected override void OnActivate() {
        RegisterScreen.Instance.Show();

        RegisterScreen.Instance.ClosePopUpEvent.AddListener(GoToMenu);
    }

    protected override void OnDeactivate() {
        RegisterScreen.Instance.Hide();

        RegisterScreen.Instance.ClosePopUpEvent.RemoveListener(GoToMenu);
    }

    private void GoToMenu() {
        fsm.GoToState<LoginState>();
    }

}