using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreen : AbstractPopUp<RegisterScreen> {

    [Header("Feedback Text")]
    [SerializeField, TextArea(2, 3)] private string successfulLogin;
    [SerializeField, TextArea(2, 3)] private string failedLogin;

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button backgroundButton;
    [SerializeField] private TextMeshProUGUI feedbackTextField;

    protected override void OnShow() {
        gameObject.SetActive(true);

        registerButton.onClick.AddListener(OnRegisterButtonClicked);

        closeButton.onClick.AddListener(Hide);
        backgroundButton.onClick.AddListener(Hide);
    }

    protected override void OnHide() {
        registerButton.onClick.RemoveListener(OnRegisterButtonClicked);

        closeButton.onClick.RemoveListener(Hide);
        backgroundButton.onClick.RemoveListener(Hide);

        gameObject.SetActive(false);
        ClosePopUpEvent.Invoke();
    }

    private void OnRegisterButtonClicked() {
        Account.RegistrationSuccess.AddListener(OnRegistrationSuccess);
        Account.Register(usernameInputField.text, passwordInputField.text);
    }

    private void OnRegistrationSuccess() {
        Account.RegistrationSuccess.RemoveListener(OnRegistrationSuccess);
        feedbackTextField.text = successfulLogin;
        CoroutineHelper.Delay(0.4f, () => {
            if (gameObject.activeInHierarchy) {
                Hide();
            }
        });
    }

}