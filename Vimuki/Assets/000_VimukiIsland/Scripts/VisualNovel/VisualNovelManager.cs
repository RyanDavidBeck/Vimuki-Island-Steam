using System;
using System.Collections.Generic;
using System.Linq;
using Articy.Unity;
using Articy.Unity.Interfaces;
using Articy.Vimukisteam;
using ColorDebugLog;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VisualNovelManager : MonoBehaviour, IArticyFlowPlayerCallbacks, VimukiIslandInput.IVisualNovelActions
{
    #region Variables
    [Space]
    [Header("Debugging")] 
    [SerializeField] private ArticyRef supporterDialog; 
    [SerializeField] private DecisionHandler decisionHandler; 
    
    [Space]
    [Header("Character")]
    [SerializeField] private List<IngameCharacter> allCharacters = new List<IngameCharacter>(); //all character objects in the scene
    [ArticyTypeConstraint(typeof(SceneCharacter))] public List<ArticyRef> ignoredCharactersRefs = new List<ArticyRef>(); //Ignored chars like player, narrator, ...
    [SerializeField] public Color32 colorActiveSpeaker; 
    [SerializeField] public Color32 colorInactiveSpeaker;
    [SerializeField] public Color32 colorInvisible;
    [SerializeField] public float timeFadeInFadeOut = 0.5f; 
    
    [Space]
    [Header("Scene")] 
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image diaItemImage;
    [SerializeField] private Transform charPositionLeft;
    [SerializeField] private Transform charPositionRight;
    [SerializeField] private Transform charPositionCenter;

    [Space]
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Space]
    [Header("DialogueBox UI")]
    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueSpeakingText;
    
    private ArticyFlowPlayer _flowPlayer;
    private Branch _branch;
    private IList<Branch> _decisionBranches;
    private VimukiIslandInput _inputControls;
    private bool _isDecisionActive;
    private bool _decide; 
    private IngameCharacter _lastCharacter;
    private readonly List<SceneCharacter> _ignoredCharacters = new List<SceneCharacter>();
    
    #endregion

    #region Initialization

    private void OnEnable()
    {
        BranchHolder.BranchSelected += ContinueDecisionBranch; 
    }

    private void OnDisable()
    {
        BranchHolder.BranchSelected -= ContinueDecisionBranch; 
    }

    private void Start()
    {
        if (_inputControls == null)
        {
            _inputControls = new VimukiIslandInput();
            _inputControls.VisualNovel.Enable();
            _inputControls.VisualNovel.SetCallbacks(this);
        }
        _flowPlayer = GetComponent<ArticyFlowPlayer>();

        foreach (var articyRef in ignoredCharactersRefs)
        {
            _ignoredCharacters.Add((SceneCharacter)articyRef);
        }

        foreach (var character in allCharacters)
        {
            character.gameObject.GetComponent<SpriteRenderer>().color = colorInvisible;
            character.gameObject.SetActive(true);
        }
        
        diaItemImage.color = colorInvisible;
    }
    
    #endregion

    #region Debug
    
    /// <summary>
    /// Starts the supporter debug dialogue manually.
    /// </summary>
    [Button]
    public void StartSupporterDialog()
    {
        var dialogueObject = supporterDialog.GetObject();
        StartDialogue(dialogueObject);
    }
    
    #endregion
    
    #region Dialogue Actions
    
    /// <summary>
    /// Starts a dialogue from a given Articy object.
    /// </summary>
    private void StartDialogue(IArticyObject aObject) //TODO Must be public in the future
    {
        _flowPlayer.StartOn = aObject;
    }

    /// <summary>
    /// Callback when the flow player is paused on an object.
    /// Handles dialogue or instruction types.
    /// </summary>
    /// <param name="aObject">the current stopped flowObject</param>
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        switch (aObject)
        {
            case Instruction instructionFragment:
                switch (instructionFragment)
                {
                    case ChangeBackground:
                        ChangeSceneBackground(instructionFragment);
                        break; 
                    case EndScene:  
                        EndDialogue();
                        break;
                    case SetScene:  
                        SetSceneSettings(instructionFragment);
                        break;
                    case ShowHideChar:  
                        ShowHideCharacters(instructionFragment);
                        break;
                    case ShowObject:
                        ShowObjectInScene(instructionFragment);
                        break; 
                    case HideObject:
                        HideObjectInScene();
                        break;
                    case MoveCharacterToPosition:
                        MoveCharToPosition(instructionFragment); 
                        break; 
                }
                _flowPlayer.Play();
                break;
            
            case IDialogueFragment dialogueFragment:
                switch (dialogueFragment)
                {
                    case DialogueWithExpression dialogueWithExpression:
                        FillDialogueBox(dialogueWithExpression);
                        break; 
                }
                break;
        }
    }

    /// <summary>
    /// Called when the flow player updates the available dialogue branches.
    /// 
    /// </summary>
    /// <param name="aBranches">IList of all following branches</param>
    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        ColorDebug.Log($"{aBranches}", DebugType.Dialogue);
        if (aBranches.Count == 0) return; 
        
        _branch = aBranches[0];
        if (_branch.Target is Decision)
        {
            ColorDebug.Log($"Decision incoming: {aBranches.Count}", DebugType.Dialogue);
            _decide = true; 
            _decisionBranches = aBranches;
        }
        else
        {
            _branch = aBranches[0]; 
        }
    }

    private void FillDialogueBox(DialogueFragment dialogueFragment)
    {
        var dialogueWithExpression = (DialogueWithExpression)dialogueFragment;
        var currentSpeaker = (IObjectWithLocalizableDisplayName)dialogueFragment.Speaker;
        var currentCharacter = (SceneCharacter)currentSpeaker;
        dialogueSpeaker.text= currentSpeaker.DisplayName;
        dialogueSpeakingText.text = dialogueFragment.Text;

        var inGameChar = FindArticyCharacterInScene(currentCharacter);
        
        //Check if there is a new expression
        if (dialogueWithExpression.Template.DialogueExpression.Expression != null)
        {
            var charAsset = dialogueWithExpression.Template.DialogueExpression.Expression as Asset;
            inGameChar.gameObject.GetComponent<SpriteRenderer>().sprite = charAsset!.LoadAssetAsSprite(); 
        }
        
        //Check if there is a voice sound 
        if (dialogueWithExpression.Template.DialogueExpression.VoiceSound != null)
        {
            var soundAsset = dialogueWithExpression.Template.DialogueExpression.VoiceSound as Asset; 
            var soundClip = soundAsset!.LoadAsset<AudioClip>();
            sfxSource.clip = soundClip;
            sfxSource.Play();
        }
        
        //Deletes the highlight from the last speaker
        ToneSpeaker();
        _lastCharacter = inGameChar; 
        
        //Highlight the current speaker if they are not ignored
        if (_ignoredCharacters.Contains(currentCharacter)) return;
        HighlightSpeaker(inGameChar);
    }
    
    /// <summary>
    /// Continues on the by the player selected branch
    /// </summary>
    /// <param name="branch">the selected branch</param>
    private void ContinueDecisionBranch(Branch branch)
    {
        _decide = false;
        _isDecisionActive = false; 
        _flowPlayer.Play(branch);
        _flowPlayer.Play();
    }

    /// <summary>
    /// Highlight the current speaker
    /// </summary>
    /// <param name="inGameCharacter">The character object in the scene</param>
    private void HighlightSpeaker(IngameCharacter inGameCharacter)
    {
        //Highlight the current speaking character
        ColorDebug.Log($"Current speaking char: {inGameCharacter.name}", DebugType.Dialogue);
        inGameCharacter.gameObject.GetComponent<SpriteRenderer>().DOColor(colorActiveSpeaker, timeFadeInFadeOut);
    }

    /// <summary>
    /// Delete the highlight from the last speaker
    /// </summary>
    private void ToneSpeaker()
    {
        //If there was a last character speaking, fade them out
        if (_lastCharacter == null || _ignoredCharacters.Contains(_lastCharacter.sceneCharacter) || _lastCharacter.isHidden) return;
        
        _lastCharacter.gameObject.GetComponent<SpriteRenderer>().DOColor(colorInactiveSpeaker, timeFadeInFadeOut);
    }
    
    #endregion
    
    #region Instructions
    private void EndDialogue()
    {
        Debug.Log("Dialog Ends!");
    }
    
    private void ChangeSceneBackground(Instruction instruction)
    {
        var changeBackgroundInstruction = (ChangeBackground)instruction;
        var newBackground = changeBackgroundInstruction.Template.ChangeBackgroundProperties.NewBackground as Asset;
        var backgroundFadeSeq = DOTween.Sequence();
        backgroundFadeSeq.Append(backgroundImage.DOColor(colorInvisible, timeFadeInFadeOut).
            OnComplete(() => backgroundImage.overrideSprite = newBackground!.LoadAssetAsSprite()));
        backgroundFadeSeq.Append(backgroundImage.DOColor(colorActiveSpeaker, timeFadeInFadeOut));
    }

    /// <summary>
    /// Sets up the scene with background, music, and characters.
    /// </summary>
    private void SetSceneSettings(Instruction instruction)
    {
        //Read Object
        var setSceneInstruction = (SetScene)instruction;
        var setSceneProperties = setSceneInstruction.Template.SetSceneProperties;
        var sceneCharacters = setSceneProperties.InSceneCharacters;
        var startMusicAsset = setSceneProperties.SceneMusic as Asset; 
        var startMusic = startMusicAsset!.LoadAsset<AudioClip>();
        
        //Set Background
        var backgroundAsset = setSceneProperties.BackgroundSprite as Asset;
        backgroundImage.overrideSprite = backgroundAsset!.LoadAssetAsSprite();
        backgroundImage.DOColor(colorActiveSpeaker, 4f);
        
        //Set Music
        musicSource.clip = startMusic;
        musicSource.Play();
        
        //Remove all unused characters from scene
        var charactersToRemove = allCharacters
            .Where(c => !sceneCharacters.Contains((ArticyObject)c.sceneCharacterRef))
            .ToList();
        
        allCharacters.RemoveAll(c => charactersToRemove.Contains(c));
        foreach (var character in charactersToRemove)
        {
            Destroy(character.gameObject);
        }
    }

    /// <summary>
    /// Moves a character to a defined position or coordinates.
    /// </summary>
    private void MoveCharToPosition(Instruction instruction)
    {
        var moveCharInstruction = (MoveCharacterToPosition)instruction;
        var moveCharProperties = moveCharInstruction.Template.MoveCharToPositionProperties;
        var charToMove = moveCharProperties.CharacterToMove;
        var movementType = moveCharProperties.MovementType;
        var inGameCharToMove = FindArticyCharacterInScene(charToMove);
        //Move by position
        if (movementType == MovementType.ByPosition)
        {
            switch (moveCharProperties.MovementPosition)
            {
                case MovementPosition.Left:
                    inGameCharToMove.gameObject.transform.DOMove(new Vector3(charPositionLeft.position.x, inGameCharToMove.transform.position.y), 3); 
                    break;
                case MovementPosition.Right:
                    inGameCharToMove.gameObject.transform.DOMove(new Vector3(charPositionRight.position.x, inGameCharToMove.transform.position.y), 3); 
                    break;
                case MovementPosition.Center:
                    inGameCharToMove.gameObject.transform.DOMove(new Vector3(charPositionCenter.position.x, inGameCharToMove.transform.position.y), 3); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //Move by coordinate
        else
        {
            var xCoordinate = moveCharProperties.XCoordinate; 
            var yCoordinate = moveCharProperties.YCoordinateAddition;
            inGameCharToMove.gameObject.transform.DOMove(new Vector3(xCoordinate, inGameCharToMove.transform.position.y + yCoordinate), 3); 
        }
    }
    
    /// <summary>
    /// Shows and/or hides characters based on instruction.
    /// </summary>
    private void ShowHideCharacters(Instruction instruction)
    {
        var showHideCharInstruction = (ShowHideChar)instruction;
        var charsToHide = showHideCharInstruction.Template.ShowHideCharProperties.CharsToHide; 
        var charsToShow = showHideCharInstruction.Template.ShowHideCharProperties.CharsToShow;
        
        //Hide Chars 
         allCharacters
            .Where(c => charsToHide.Contains((ArticyObject)c.sceneCharacterRef))
            .ToList()
            .ForEach(HideCharacter);
        
        //Show Chars
        allCharacters
            .Where(c => charsToShow.Contains((ArticyObject)c.sceneCharacterRef))
            .ToList()
            .ForEach(ShowCharacter);
    }

    /// <summary>
    /// Fades in a specified character and marks them as visible.
    /// </summary>
    /// <param name="character">The character object in the scene</param>
    private void ShowCharacter(IngameCharacter character)
    {
        character.gameObject.GetComponent<SpriteRenderer>().DOColor(colorInactiveSpeaker, timeFadeInFadeOut);
        character.isHidden = false; 
    }

    /// <summary>
    /// Fades out a specified character and marks them as hidden.
    /// </summary>
    /// <param name="character">The character object in the scene</param>
    private void HideCharacter(IngameCharacter character)
    {
        character.gameObject.GetComponent<SpriteRenderer>().DOColor(colorInvisible, timeFadeInFadeOut);
        character.isHidden = true; 
    }

    /// <summary>
    /// Displays a scene object from an Articy entity reference. Dia, Item or Artefact.
    /// </summary>
    private void ShowObjectInScene(Instruction instruction)
    {
        //Load the sprite from the articy Asset
        var showObjectInstruction = (ShowObject)instruction;
        var objectEntity = showObjectInstruction.Template.ShowObjectProperties.ObjectToShow as Entity;
        Assert.IsNotNull(objectEntity);
        diaItemImage.overrideSprite = objectEntity.PreviewImage.Asset.LoadAssetAsSprite();
        
        //Fade In 
        diaItemImage.DOColor(colorActiveSpeaker, timeFadeInFadeOut);
    }   

    private void HideObjectInScene()
    {
        diaItemImage.DOColor(colorInvisible, timeFadeInFadeOut);
    }
    
    #endregion
    
    #region Helper

    /// <summary>
    /// Find the InGameCharacter object in the scene from an articy object entity
    /// </summary>
    /// <param name="articyObject">The Entity of the character</param>
    /// <returns>The character object in the scene</returns>
    private IngameCharacter FindArticyCharacterInScene(ArticyObject articyObject)
    {
       return allCharacters.Find(character => (ArticyObject)character.sceneCharacterRef == articyObject);
    }
    
    #endregion
    
    #region InputActions

    public void OnContinueDialogue(InputAction.CallbackContext context)
    {
        switch (context.performed)
        {
            case true when _isDecisionActive:
                break;
            case true when !_decide:
                _flowPlayer.Play(_branch);
                break;
            case true when _decide:
                _isDecisionActive = true;
                decisionHandler.AssignBranches(_decisionBranches, _flowPlayer);
                break;
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Open Escape Menu");
        }
    }

    #endregion
    
}