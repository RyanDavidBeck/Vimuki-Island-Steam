using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DisplayCaseManager : MonoBehaviour
{
    [SerializeField] private Light2D lightHairball; 
    [SerializeField] private Light2D lightPearl; 
    [SerializeField] private Light2D lightLantern; 
    [SerializeField] private Light2D lightBrush; 
    [SerializeField] private Light2D lightRomance;

    [SerializeField] private DisplayCase hairball; 
    [SerializeField] private DisplayCase pearl; 
    [SerializeField] private DisplayCase lantern; 
    [SerializeField] private DisplayCase brush; 
    [SerializeField] private DisplayCase romance;

    [SerializeField] private GameObject infoPopUp; 
    
    [SerializeField] private float activeLightIntensity; 
    [SerializeField] private float inactiveLightIntensity; 
    
    public void OnClickHairball()
    {
        
    }

    public void OnHoverHairball()
    {
        lightHairball.intensity = activeLightIntensity; 
    }

    public void OnClickPearl()
    {
        
    }

    public void OnHoverPearl()
    {
        lightPearl.intensity = activeLightIntensity; 
    }
    
    public void OnClickLantern()
    {
        
    }

    public void OnHoverLantern()
    {
        lightLantern.intensity = activeLightIntensity; 
    }
    
    public void OnClickBrush()
    {
        
    }

    public void OnHoverBrush()
    {
        lightBrush.intensity = activeLightIntensity; 
    }
    
    public void OnClickRomance()
    {
        
    }

    public void OnHoverRomance()
    {
        lightRomance.intensity = activeLightIntensity; 
    }

    public void ResetLight()
    {
        lightHairball.intensity = inactiveLightIntensity; 
        lightBrush.intensity = inactiveLightIntensity; 
        lightLantern.intensity = inactiveLightIntensity; 
        lightPearl.intensity = inactiveLightIntensity; 
        lightRomance.intensity = inactiveLightIntensity; 
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
