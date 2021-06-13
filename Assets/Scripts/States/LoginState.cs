using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/State/LoginState")]
public class LoginState : AbstractState {

    protected override void OnActivate() {
        LoginScreen.Instance.Show();

        LoginScreen.Instance.OnRegisterButtonClick.AddListener(GoToRegister);
        LoginScreen.Instance.OnCorrectLogin.AddListener(GoToMenu);
    }

    protected override void OnDeactivate() {
        LoginScreen.Instance.Hide();

        LoginScreen.Instance.OnRegisterButtonClick.RemoveListener(GoToRegister);
        LoginScreen.Instance.OnCorrectLogin.RemoveListener(GoToMenu);
    }

    private void GoToRegister() {
        fsm.GoToState<RegisterState>();
    }

    private void GoToMenu() {
        //AccountInformation.Username = "yeet";
        fsm.GoToState<MenuState>();
    }

}