using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TextMeshProResizer : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> labels = new List<TMP_Text>();
    [SerializeField] private bool executeOnUpdate;
    private int _currentIndex;

    private void Update()
    {
        if (executeOnUpdate) Execute();

        OnUpdateCheck();
    }

    private void Execute()
    {
        if (labels.Count == 0) return;

        var count = labels.Count;

        var index = 0;
        float maxLength = 0;

        for (var i = 0; i < count; i++)
        {
            float length = labels[i].GetParsedText().Length;
            if (!(length > maxLength)) continue;
            
            maxLength = length;
            index = i;
        }

        if (_currentIndex != index)
        {
            OnChanged(index);
        }
    }
    private void OnChanged(int index)
    {
        // Disable auto size on previous
        labels[_currentIndex].enableAutoSizing = false;

        _currentIndex = index;

        // Force an update of the candidate text object so we can retrieve its optimum point size.
        labels[index].enableAutoSizing = true;
        labels[index].ForceMeshUpdate();
    }
    private void OnUpdateCheck()
    {
        var optimumPointSize = labels[_currentIndex].fontSize;

        // Iterate over all other text objects to set the point size
        var count = labels.Count;

        for (var i = 0; i < count; i++)
        {
            if (_currentIndex == i) continue;

            labels[i].enableAutoSizing = false;

            labels[i].fontSize = optimumPointSize;
        }
    }
}
