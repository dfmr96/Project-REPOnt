using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalConditionPanel : MonoBehaviour
{
    public void ExitGame() { UIManager.Instance.ExitGame(); }
    public void ReturnMainMenu() { UIManager.Instance.BackToMainMenu(); }
}
