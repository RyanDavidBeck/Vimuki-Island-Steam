using Articy.Unity;
using Articy.Vimukisteam;
using Articy.Vimukisteam.GlobalVariables;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCase : MonoBehaviour
{
    [ArticyTypeConstraint(typeof(Artefact))]
    public ArticyRef artefactReference;

    [SerializeField] private Color inactive; 
    [SerializeField] private Color active;
    private Image _displayImage;
    private Artefact _artefact; 
    
    void Start()
    {
        _artefact = (Artefact)artefactReference; 
        _displayImage = GetComponent<Image>();
        
        //Get the collectionInfo from the DatabaseSaves
        _artefact.Template.ArtefactProperties.IsInCollection = ArticyGlobalVariables.Default.
            GetVariableByString<bool>(_artefact.Template.ArtefactProperties.BindedVariable.RawScript);
        
        _displayImage.color = _artefact.Template.ArtefactProperties.IsInCollection ? active : inactive;
    }
}
