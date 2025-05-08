using ColorDebugLog;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public void OnClick()
    {
        ColorDebug.Log("Clicked Button", DebugType.UserInteraction);
    }
}
