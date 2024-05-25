using UnityEngine;

public class ParticleColor : MonoBehaviour
{
    [SerializeField] private Color playerColor;

    private void Start()
    {
        ApplyColorToAllChildParticleSystems();
        
    }
    
    private void ApplyColorToAllChildParticleSystems()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in particleSystems)
        {
            var colorModule = ps.colorOverLifetime;
            colorModule.enabled = true;

            Gradient originalGradient = colorModule.color.gradient;
            Gradient newGradient = new Gradient();
            
            GradientColorKey[] colorKeys = originalGradient.colorKeys;
            GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;
            
            if (colorKeys.Length > 0 && colorKeys[0].time == 0f)
            {
                colorKeys[0].color = playerColor;
            }
            else
            {
                GradientColorKey newKey = new GradientColorKey(playerColor, 0f);
                colorKeys = new GradientColorKey[colorKeys.Length + 1];
                colorKeys[0] = newKey;
                System.Array.Copy(originalGradient.colorKeys, 0, colorKeys, 1, originalGradient.colorKeys.Length);
            }

            newGradient.SetKeys(colorKeys, alphaKeys);
            colorModule.color = newGradient;
        }
    }
}
