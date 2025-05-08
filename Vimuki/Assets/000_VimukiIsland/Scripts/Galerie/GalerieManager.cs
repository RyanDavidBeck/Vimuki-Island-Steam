using System;
using System.Collections.Generic;
using Articy.Unity;
using Articy.Vimukisteam;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalerieManager : MonoBehaviour
{
    public delegate void GalleryManagement();
    public event GalleryManagement InitializeDia; 
    
    [SerializeField] private GameObject diaPrefab;
    [SerializeField] private GameObject diaPool;
    private List<Dia> _allDias = new List<Dia>();
    private List<GameObject> _allDiaObjects = new List<GameObject>(); 
    
    
    void Start()
    {
        _allDias = ArticyDatabase.GetAllOfType<Dia>();

        foreach (var dia in _allDias)
        {
            var tempDia = GameObject.Instantiate(diaPrefab, diaPool.transform);
            tempDia.GetComponent<GalerieDia>().diaRef = dia; 
            _allDiaObjects.Add(tempDia);
        }
        InitializeDia?.Invoke();
    }
    
    public void OnClickFilter(string islandAreaName)
    {
        var islandAreaType = islandAreaName switch
        {
            "Camp" => IslandAreaType.Camp,  
            "Cave"=> IslandAreaType.Hoehle,
            "Jungle" => IslandAreaType.Dschungel,
            "Temple" => IslandAreaType.Froschtempel,
            "Beach" => IslandAreaType.Strand,
            _ => throw new ArgumentOutOfRangeException(nameof(islandAreaName), islandAreaName, "Area is not defined")
        };

        foreach (var diaObject in _allDiaObjects)
        {
            var diaIslandAreaType = diaObject.GetComponent<GalerieDia>().diaRef.Template.DiaProperties.Area;
            diaObject.SetActive(diaIslandAreaType == islandAreaType);
        }
    }

    public void OnClickAll()
    {
        foreach (var diaObject in _allDiaObjects)
        {
            diaObject.SetActive(true);
        }
    }

    public void OnClickCollected()
    {
        foreach (var diaObject in _allDiaObjects)
        {
            diaObject.SetActive(diaObject.GetComponent<GalerieDia>().diaRef.Template.DiaProperties.IsInCollection);
        }
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
