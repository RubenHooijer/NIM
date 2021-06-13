using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class Account {

    public class AccountStats {
        
        public float Winrate => (float)Wins / (float)TotalGames;
        public int Losses => TotalGames - Wins;

        public int Wins = 0;
        public int TotalGames = 0;

        public AccountStats() { }

        public AccountStats(int wins, int totalGames) {
            Wins = wins;
            TotalGames = totalGames;
        }

    }

    private const string ACCOUNTLOGIN_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/AccountLogin.php";
    private const string ACCOUNTLOGOUT_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/AccountLogout.php";
    private const string ACCOUNTREGISTER_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/RegisterAccount.php";
    private const string ADDGAME_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/AddGameToAccount.php";
    private const string GETSTATS_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/GetAccountInfo.php";

    public static UnityEvent LoggedIn { get; private set; } = new UnityEvent();
    public static UnityEvent LoggedOut { get; private set; } = new UnityEvent();
    public static UnityEvent LoginError { get; private set; } = new UnityEvent();

    public static UnityEvent RegistrationSuccess { get; private set; } = new UnityEvent();
    public static UnityEvent RegistrationFailed { get; private set; } = new UnityEvent();

    public static UnityEvent StatsUpdated { get; private set; } = new UnityEvent();

    public static bool IsLoggedIn { get; private set; } = false;
    public static string Username { get; private set; } = string.Empty;
    public static AccountStats Stats { get; private set; } = new AccountStats();

    public static void Login(string username, string password) {
        if (!IsValidLogin(username, password)) { return; }
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        CoroutineHelper.Instance.StartCoroutine(SimpleWebRequestRoutine(
            ACCOUNTLOGIN_URL,
            () => {
                IsLoggedIn = true;
                Username = username;
                LoggedIn.Invoke();
            },
            LoginError.Invoke,
            form));
    }

    public static void Logout() {
        WWWForm form = new WWWForm();
        form.AddField("username", Username);

        CoroutineHelper.Instance.StartCoroutine(SimpleWebRequestRoutine(
            ACCOUNTLOGOUT_URL,
            () => {
                IsLoggedIn = false;
                Username = string.Empty;
                LoggedOut.Invoke();
            },
            null,
            form));
    }

    public static void Register(string username, string password) {
        if (!IsValidRegistration(username, password)) { return; }
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        CoroutineHelper.Instance.StartCoroutine(SimpleWebRequestRoutine(
            ACCOUNTREGISTER_URL,
            RegistrationSuccess.Invoke,
            RegistrationFailed.Invoke,
            form));
    }

    public static void AddGame(bool isWin) {
        if (!IsLoggedIn) { return; }

        WWWForm form = new WWWForm();
        form.AddField("username", Username);
        form.AddField("isWin", isWin ? 1 : 0);

        CoroutineHelper.Instance.StartCoroutine(SimpleWebRequestRoutine(
            ADDGAME_URL,
            () => Debug.Log("Successfully added game"),
            () => Debug.Log("Failed to add game"),
            form));
    }

    public static void UpdateStats() {
        CoroutineHelper.Instance.StartCoroutine(UpdateStatsRoutine());
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() {
        Application.quitting += OnApplicationQuit;
    }

    private static bool IsValidLogin(string username, string password) {
        if (username.Length <= 0) {
            LoginError.Invoke();
            return false;
        } else if (password.Length <= 0) {
            LoginError.Invoke();
            return false;
        }

        return true;
    }

    private static bool IsValidRegistration(string username, string password) {
        if (username.Length < 4) {
            RegistrationFailed.Invoke();
            return false;
        } else if (username.Length > 24) {
            RegistrationFailed.Invoke();
            return false;
        } else if (password.Length < 6) {
            RegistrationFailed.Invoke();
            return false;
        } else if (password.Length > 40) {
            RegistrationFailed.Invoke();
            return false;
        }

        return true;
    }

    private static void OnApplicationQuit() {
        if (IsLoggedIn) {
            Logout();
        }
    }

    private static IEnumerator UpdateStatsRoutine() {
        WWWForm form = new WWWForm();
        form.AddField("username", Username);

        UnityWebRequest httpRequest = UnityWebRequest.Post(GETSTATS_URL, form);
        httpRequest.timeout = 15;
        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.Success) {
            string statsInfo = httpRequest.downloadHandler.text;
            string[] stats = statsInfo.Split('_');

            if (!int.TryParse(stats[0], out int wins)) {
                wins = 0;
            }

            if (!int.TryParse(stats[1], out int totalGames)) {
                totalGames = 0;
            }

            Stats.Wins = wins;
            Stats.TotalGames = totalGames;

            StatsUpdated.Invoke();
        } else {
            Debug.Log($"Could not retrieve account info");
        }
    }

    private static IEnumerator SimpleWebRequestRoutine(string url, Action onSuccess = null, Action onFail = null, WWWForm form = null) {
        UnityWebRequest httpRequest = UnityWebRequest.Post(url, form);
        httpRequest.timeout = 15;
        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.Success) {
            onSuccess?.Invoke();
        } else {
            onFail?.Invoke();
        }
    }

}