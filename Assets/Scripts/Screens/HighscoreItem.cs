using TMPro;
using UnityEngine;

public class HighscoreItem : ContainerItem<HighscoreInfo> {

    [SerializeField] private TextMeshProUGUI numberTextField;
    [SerializeField] private TextMeshProUGUI nameTextField;
    [SerializeField] private TextMeshProUGUI winsTextField;

    protected override void OnSetup(HighscoreInfo data) {
        numberTextField.text = data.Number.ToString();
        nameTextField.text = data.Name;
        winsTextField.text = data.Wins.ToString();
    }

    protected override void OnDispose() { }

}