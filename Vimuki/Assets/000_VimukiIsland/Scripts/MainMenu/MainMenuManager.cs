using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionPrompt;
    [SerializeField] private GameObject descriptionObject;
    [SerializeField] private GameObject settingsCanvas; 
    void Start()
    {
        EmptyPrompt();
    }

    public void OnClickGalerie()
    {
        SceneManager.LoadScene("Galerie"); 
    }

    public void OnHoverGalerie()
    {
        descriptionObject.SetActive(true);
        descriptionPrompt.text = "Schaue dir deine gesammelten Dias an!"; 
    }

    public void OnClickDisplayCase()
    {
        SceneManager.LoadScene("DisplayCases");
    }

    public void OnHoverDisplayCase()
    {
        descriptionObject.SetActive(true);
        descriptionPrompt.text = "Schaue dir deine Sammlerstücke an!"; 
    }

    public void OnClickFullGame()
    {
        //Start Supporter Game
        
    }

    public void OnHoverFullGame()
    {
        descriptionObject.SetActive(true);
        descriptionPrompt.text = "Komplettes Spiel: Ergattere dir eine:n Unterstützer:in, pack deinen Koffer und mach dich auf die Reise nach Vimuki Island!"; 
    }
    
    public void OnClickFastGame()
    {
        //Activate 3 Random Items and the Multitool
        InventoryManager.instance.AddRandomItems(3);
        
        //Start Main Game
        
    }

    public void OnHoverFastGame()
    {
        descriptionObject.SetActive(true);
        descriptionPrompt.text = "Schnelles Spiel: Schippere direkt nach Vimuki Island!"; 
    }

    public void OnClickSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void OnClickSettingsClose()
    {
        settingsCanvas.SetActive(false);
    }

    public void EmptyPrompt()
    {
        descriptionObject.SetActive(false);
        descriptionPrompt.text = ""; 
    }
}
