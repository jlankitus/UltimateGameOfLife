using UnityEngine;

[CreateAssetMenu(fileName = "NewLifePattern", menuName = "LifePattern")]
public class LifePattern : ScriptableObject
{
    public string patternName;
    public GameObject patternPrefab;

    // Cool trick to default the patternName to the object name.
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(patternName))
        {
            patternName = name;
        }
    }
}
