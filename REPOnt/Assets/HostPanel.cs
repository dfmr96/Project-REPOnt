using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HostPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idText;
    public void GetButtonId() { idText.text += ConnectionManager.Instance.GetRunId(); }
}
