
using Articy.Vimukisteam;
using Articy.Vimukisteam.GlobalVariables;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GalerieDia : MonoBehaviour
{
    public delegate void GalerieManagement(Dia diaReferenz);
    public static event GalerieManagement OpenInfobox; 
    
    public Dia diaRef;
    [SerializeField] private Image diaImage;
    [SerializeField] private Color inactive; 
    [SerializeField] private Color active;
    [SerializeField] private float activeScaling; 
    [SerializeField] private float activeScalingDuration; 
    
    private GalerieManager _galerieManager;
    private bool _isInCollection; 
    private void Awake()
    {
        _galerieManager = FindFirstObjectByType<GalerieManager>();
    }

    private void OnEnable()
    {
        _galerieManager.InitializeDia += Initialize;
    }

    private void OnDisable()
    {
        _galerieManager.InitializeDia -= Initialize; 
    }

    private void Initialize()
    {
        diaImage.overrideSprite = diaRef.PreviewImage.Asset.LoadAssetAsSprite();
        
        //Get the collectionInfo from the DatabaseSaves
        diaRef.Template.DiaProperties.IsInCollection = ArticyGlobalVariables.Default.
            GetVariableByString<bool>(diaRef.Template.DiaProperties.BindedVariable.RawScript);

        _isInCollection = diaRef.Template.DiaProperties.IsInCollection;
        
        diaImage.color = _isInCollection ? active : inactive;
    }

    public void OnClickDia()
    {
        OpenInfobox?.Invoke(diaRef);
    }

    public void OnHoverDia()
    {
        Debug.Log("Hover dia");
        if (_isInCollection)
        {
            gameObject.transform.DOScale(activeScaling, activeScalingDuration); 
        }
        
    }

    public void OnExit()
    {
        if (_isInCollection)
        {
            gameObject.transform.DOScale(1, activeScalingDuration); 
        }
        
    }
}