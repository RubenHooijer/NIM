using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HighscoreScreen : AbstractPopUp<HighscoreScreen> {

    private const string GETHIGHSCORES_URL = "https://studenthome.hku.nl/~ruben.hooijer/GDEV/GetHighscoreList.php";

    [SerializeField] private HighscoreItem highscoreItemTemplate;
    [SerializeField] private RectTransform layoutGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button backgroundButton;

    private ItemContainer<HighscoreItem, HighscoreInfo> highscoreContainer;

    protected override void Awake() {
        base.Awake();
        highscoreContainer = new ItemContainer<HighscoreItem, HighscoreInfo>(highscoreItemTemplate);
    }

    protected override void OnShow() {
        CoroutineHelper.Instance.StartCoroutine(UpdateHighscoresRoutine());

        gameObject.SetActive(true);

        closeButton.onClick.AddListener(Hide);
        backgroundButton.onClick.AddListener(Hide);
    }

    protected override void OnHide() {
        closeButton.onClick.RemoveListener(Hide);
        backgroundButton.onClick.RemoveListener(Hide);

        gameObject.SetActive(false);
        ClosePopUpEvent.Invoke();
    }

    private IEnumerator UpdateHighscoresRoutine() {
        UnityWebRequest httpRequest = UnityWebRequest.Get(GETHIGHSCORES_URL);
        httpRequest.timeout = 15;
        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.Success) {
            string data = httpRequest.downloadHandler.text;
            string[] players = data.Split('_');

            List<HighscoreInfo> highscoreInfos = new List<HighscoreInfo>();
            for (int i = 0; i < players.Length; i++) {
                string[] playerData = players[i].Split('-');
                if (playerData.Length != 2) { continue; }
                highscoreInfos.Add(new HighscoreInfo(i + 1, playerData[0], int.Parse(playerData[1])));
            }

            highscoreContainer.UpdateContainer(highscoreInfos);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
        } else {
            Debug.Log("Data could not be retrieved");
        }
    }

}