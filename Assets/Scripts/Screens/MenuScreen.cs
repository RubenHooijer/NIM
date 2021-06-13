using DG.Tweening;
using NaughtyAttributes;
using Oasez.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : AbstractScreen<MenuScreen> {

    [Header("Settings")]
    [SerializeField, TextArea(2, 3)] private string loggedOutText;

    [Header("General")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button highscoresButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private Button accountButton;
    [SerializeField] private TextMeshProUGUI accountFeedbackTextField;

    [Header("Play button dropdown")]
    [HorizontalLine]
    [SerializeField] private RectTransform dropdownTransform;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button hostServerButton;
    [SerializeField] private Button joinServerButton;
    [SerializeField] private TextMeshProUGUI feedbackTextField;

    [Header("Account info dropdown")]
    [HorizontalLine]
    [SerializeField] private RectTransform infoBox;
    [SerializeField] private GameObject accountInfo;
    [SerializeField] private TextMeshProUGUI winsTextField;
    [SerializeField] private TextMeshProUGUI lossesTextField;
    [SerializeField] private TextMeshProUGUI winrateTextField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button logoutButton;

    [Button]
    void DebugAddWin() {
        Account.AddGame(true);
    }

    protected override void OnShow() {
        gameObject.SetActive(true);

        playButton.onClick.AddListener(OnPlayButtonClicked);
        accountButton.onClick.AddListener(OnAccountButtonClicked);
        highscoresButton.onClick.AddListener(OnHighscoresButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        logoutButton.onClick.AddListener(OnLogoutButtonClicked);

        hostServerButton.onClick.AddListener(OnHostServerClicked);
        joinServerButton.onClick.AddListener(OnJoinServerClicked);

        Account.LoggedIn.AddListener(OnLoggedIn);
        Account.LoggedOut.AddListener(OnLoggedOut);
        Account.StatsUpdated.AddListener(OnStatsUpdated);
    }

    protected override void OnHide() {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        accountButton.onClick.RemoveListener(OnAccountButtonClicked);
        highscoresButton.onClick.RemoveListener(OnHighscoresButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);

        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        registerButton.onClick.RemoveListener(OnRegisterButtonClicked);
        logoutButton.onClick.RemoveListener(OnLogoutButtonClicked);

        hostServerButton.onClick.RemoveListener(OnHostServerClicked);
        joinServerButton.onClick.RemoveListener(OnJoinServerClicked);

        Account.LoggedIn.RemoveListener(OnLoggedIn);
        Account.LoggedOut.RemoveListener(OnLoggedOut);
        Account.StatsUpdated.RemoveListener(OnStatsUpdated);

        gameObject.SetActive(false);
    }

    private void OnPlayButtonClicked() {
        if (dropdownTransform.localScale.y > 0) {
            dropdownTransform.DOScaleY(0, 0.1f).OnComplete(() => dropdownTransform.gameObject.SetActive(false));
            highscoresButton.transform.DOMove(playButton.transform.position + Vector3.down * 110, 0.1f);
            quitButton.transform.DOMove(playButton.transform.position + Vector3.down * 210, 0.1f);
        } else {
            dropdownTransform.gameObject.SetActive(true);
            dropdownTransform.DOScaleY(1, 0.2f).SetEase(Ease.OutBack);
            highscoresButton.transform.DOMove(playButton.transform.position + Vector3.down * 490, 0.2f).SetEase(Ease.OutBack);
            quitButton.transform.DOMove(playButton.transform.position + Vector3.down * 590, 0.2f).SetEase(Ease.OutBack);
        }
    }

    private void OnAccountButtonClicked() {
        UpdateAccountInfo();
        SetAccountInfoBoxVisible(!(infoBox.localScale.y > 0));
    }

    private void OnHighscoresButtonClicked() {
        HighscoreScreen.Instance.Show();
    }

    private void OnLoginButtonClicked() {
        SetAccountInfoBoxVisible(false);
        LoginScreen.Instance.Show();
    }

    private void OnRegisterButtonClicked() {
        SetAccountInfoBoxVisible(false);
        RegisterScreen.Instance.Show();
    }

    private void OnLogoutButtonClicked() {
        Account.Logout();
        UpdateAccountInfo();
    }

    private void OnQuitButtonClicked() {
        Application.Quit();
    }

    private void OnHostServerClicked() {
        TransportNetwork.CreateServer(ipInputField.text);
    }

    private void OnJoinServerClicked() {
        TransportNetwork.JoinServer(ipInputField.text);
    }

    private void OnLoggedIn() {
        accountFeedbackTextField.text = Account.Username;
        Account.UpdateStats();
    }

    private void OnLoggedOut() {
        accountFeedbackTextField.text = loggedOutText;
        UpdateAccountInfo();
    }

    private void OnStatsUpdated() {
        winsTextField.text = $"Wins: {Account.Stats.Wins}";
        lossesTextField.text = $"Losses: {Account.Stats.Losses}";
        string winrate = (Account.Stats.Winrate * 100).ToString();
        if (winrate.Length > 4) {
            winrate = winrate.Substring(0, 4);
        }
        winrateTextField.text = $"Win rate: {winrate}%";
    }

    private void UpdateAccountInfo() {
        accountInfo.SetActive(Account.IsLoggedIn);
        loginButton.gameObject.SetActive(!Account.IsLoggedIn);
        registerButton.gameObject.SetActive(!Account.IsLoggedIn);
        logoutButton.gameObject.SetActive(Account.IsLoggedIn);
    }

    private void SetAccountInfoBoxVisible(bool isVisible) {
        if (isVisible) {
            infoBox.gameObject.SetActive(true);
            infoBox.DOScaleY(1, 0.2f).SetEase(Ease.OutBack);
        } else {
            infoBox.DOScaleY(0, 0.1f).OnComplete(() => infoBox.gameObject.SetActive(false)).SetEase(Ease.OutBack);
        }
    }

}