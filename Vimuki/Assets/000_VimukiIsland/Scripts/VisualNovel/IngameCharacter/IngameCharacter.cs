using System;
using Articy.Unity;
using Articy.Vimukisteam;
using UnityEngine;

public class IngameCharacter : MonoBehaviour
{
    [ArticyTypeConstraint(typeof(SceneCharacter))]
    public ArticyRef sceneCharacterRef;
    
    public SceneCharacter sceneCharacter {get; private set; }
    public bool isHidden; 
    private void Start()
    {
        sceneCharacter = (SceneCharacter)sceneCharacterRef;
    }
}
