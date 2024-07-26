using System.Collections.Generic;
using UnityEngine;

public class LifeGenerator : MonoBehaviour
{
    [SerializeField]
    private List<LifePattern> patterns;

    [SerializeField]
    private LifePattern defaultPattern;

    public void GeneratePattern(string patternName)
    {
        // Ignore case, and launch pattern, if a correct pattern name is supplied
        LifePattern pattern = patterns.Find(p => p.patternName.ToUpper() == patternName.ToUpper());
        if (pattern != null)
        {
            Debug.Log($"Generating pattern: {pattern.patternName}");
            if (pattern.patternPrefab != null)
            {
                RenderPattern(pattern);
            }
        }
        // If pattern name was not supplied, or incorrectly supplied, default to RANDOM
        else
        {
            Debug.LogWarning("Invalid pattern supplied, generating default pattern: " + defaultPattern.patternName);
            RenderPattern(defaultPattern);
        }
    }

    public void RenderPattern(LifePattern parsedPattern)
    {
        // TODO: replace with proper GOL logic
        Debug.Log("Rendering " +  parsedPattern.patternName);
    }
}
