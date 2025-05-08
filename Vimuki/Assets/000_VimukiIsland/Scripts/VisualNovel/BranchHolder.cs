using Articy.Unity;
using ColorDebugLog;
using UnityEngine;
using UnityEngine.EventSystems;

public class BranchHolder : MonoBehaviour
{
    public delegate void BranchManagement(Branch branch);
    public static event BranchManagement BranchSelected; 
    
    private Branch _branch;
    private ArticyFlowPlayer _flowPlayer;

    private void OnEnable()
    {
        BranchSelected += DisableDecision; 
    }

    private void OnDisable()
    {
        BranchSelected -= DisableDecision; 
    }

    private void DisableDecision(Branch branch)
    {
        GetComponent<EventTrigger>().enabled = false;
        gameObject.SetActive(false);
    }
    
    public void SetBranch(Branch branch, ArticyFlowPlayer flowPlayer)
    {
        _branch = branch;
        _flowPlayer = flowPlayer; 
    }

    public void OnClickDecision()
    {
        ColorDebug.Log("Clicked Decision", DebugType.UserInteraction);
        BranchSelected?.Invoke(_branch);
    }
}
