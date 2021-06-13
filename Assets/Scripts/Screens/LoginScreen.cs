using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginScreen : AbstractPopUp<LoginScreen> {

    public readonly UnityEvent OnCorrectLogin = new UnityEvent();
    public readonly UnityEvent OnRegisterButtonClick = new UnityEvent();

    [Header("Feedback Text")]
    [SerializeField, TextArea(2, 3)] private string succesfulLogin;
    [SerializeField, TextArea(2, 3)] private string failedLogin;

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backgroundButton;
    [SerializeField] private TextMeshProUGUI feedbackTextField;

    protected override void OnShow() {
        gameObject.SetActive(true);

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);

        exitButton.onClick.AddListener(Hide);
        backgroundButton.onClick.AddListener(Hide);
    }

    protected override void OnHide() {
        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        registerButton.onClick.RemoveListener(OnRegisterButtonClicked);

        exitButton.onClick.RemoveListener(Hide);
        backgroundButton.onClick.RemoveListener(Hide);

        gameObject.SetActive(false);
        ClosePopUpEvent.Invoke();
    }

    private void OnLoginButtonClicked() {
        Account.LoggedIn.AddListener(OnLoggedIn);
        Account.Login(usernameInputField.text, passwordInputField.text);
    }

    private void OnRegisterButtonClicked() {
        Hide();
        RegisterScreen.Instance.Show();
    }

    private void OnLoggedIn() {
        Account.LoggedIn.RemoveListener(OnLoggedIn);
        feedbackTextField.text = succesfulLogin;
        CoroutineHelper.Delay(0.4f, () => {
            if (gameObject.activeInHierarchy) {
                Hide();
            }
        });
    }

}