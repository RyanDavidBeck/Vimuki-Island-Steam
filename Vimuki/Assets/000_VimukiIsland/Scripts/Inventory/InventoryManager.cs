using System.Collections.Generic;
using System.Linq;
using Articy.Unity;
using Articy.Vimukisteam;
using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; } 
    
    [ArticyTypeConstraint(typeof(Inventory))]
    public ArticyRef inventoryReference;

    [ArticyTypeConstraint(typeof(Item))]
    public ArticyRef multiToolReference;

    [SerializeField] private bool isDebugMode; 
    [ReadOnly] public List<ArticyObject> inventoryItems;
    
    private Inventory _inventory;
    private Item _multiTool; 
    private List<Item> _allItems; 
    
    private void Awake() 
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
            DontDestroyOnLoad(this);
        } 
    }
    
    private void Start()
    {
        _multiTool = (Item)multiToolReference; 
        _inventory = (Inventory)inventoryReference;
        inventoryItems = _inventory.Template.InventoryProperties.InventoryItems;
        _allItems = ArticyDatabase.GetAllOfType<Item>().Where(x => !x.Template.ItemProperties.IsSupporterItem).ToList();
    }

    public void AddItem(Item item)
    {
        if (IsInInventory(item)) return;
        
        inventoryItems.Add(item);
        Debug.Log($"Adding item: {item.Template.ItemProperties.ItemName}");
    }

    public bool IsInInventory(Item item)
    {
        return inventoryItems.Contains(item);
    }

    public void AddRandomItems(int count)
    {
        count = Mathf.Min(count, _allItems.Count);

        while (count > 0)
        {
            var tempItem = _allItems[Random.Range(0, _allItems.Count)];
            if (IsInInventory(tempItem)) continue;
            
            AddItem(tempItem);
            count--;
        }
        
        AddItem(_multiTool);
    }
}
