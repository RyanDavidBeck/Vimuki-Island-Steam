using System.Collections.Generic;
using System.Linq;
using Articy.Unity;
using Articy.Vimukisteam;
using ColorDebugLog;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecisionHandler : MonoBehaviour
{
    [Header("Decisions")] 
    [SerializeField] private GameObject[] icons;
    [SerializeField] private GameObject[] buttons;

    private IList<Branch> _currentBranches;
   

    public void AssignBranches(IList<Branch> branches, ArticyFlowPlayer flowPlayer)
    {
        ColorDebug.Log("Try to assign branches", DebugType.Dialogue);
        _currentBranches = branches;
        var index = 0;
        
        foreach (var branch in _currentBranches)
        {
            var targetDecision = (Decision)branch.Target;
            if (targetDecision.Template.Decision.Icon != null) //Icons
            {
                var iconSpriteAsset = targetDecision.Template.Decision.Icon as Asset;
                GameObject nextIcon = null; 

                if (index == _currentBranches.Count-1 && _currentBranches.Count % 2 == 1) //Last one and odd
                {
                    nextIcon = icons[^1];
                }
                else //Not last one or even
                {
                    nextIcon = icons.FirstOrDefault(x => !x.activeSelf);
                }

                if (nextIcon != null && iconSpriteAsset != null)
                {
                    nextIcon.GetComponent<Image>().overrideSprite = iconSpriteAsset.LoadAssetAsSprite();
                    nextIcon.GetComponent<BranchHolder>().SetBranch(branch, flowPlayer);
                    nextIcon.GetComponent<EventTrigger>().enabled = true; 
                    nextIcon.SetActive(true);
                }
            }
            else //Text
            {
                var nextButton = buttons.FirstOrDefault(x => !x.activeSelf);
                if (nextButton != null)
                {
                    var buttonText = nextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    buttonText.text = targetDecision.MenuText;
                    nextButton.GetComponent<BranchHolder>().SetBranch(branch, flowPlayer);
                    nextButton.GetComponent<EventTrigger>().enabled = true; 
                    nextButton.SetActive(true);
                }
            }
            index++; 
        }
        ColorDebug.Log("Branches assigned", DebugType.Dialogue);
    }
}
