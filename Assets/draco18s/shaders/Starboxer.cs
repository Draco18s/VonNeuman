using UnityEngine;

public class Starboxer : MonoBehaviour
{
    public enum SkyboxResolutionLOD
    {
        Low = 128,
        Medium = 512,
        High = 1024,
        Ultra = 2048,
        _4K = 4096,
    }

    [Tooltip("The resolution of the skybox.")]
    public SkyboxResolutionLOD SkyboxLOD;

    [Tooltip("Scale of the Perlin noise.")]
    public float Scale;

    [Header("Origins")]
    [Tooltip("Check to set a fixed origin, or uncheck to generate a random origin every time.")]
    public bool FixedOrigin;

    [Tooltip("The X origin of the Perlin noise.")]
    public float FixedXOrigin;

    [Tooltip("The Y origin of the Perlin noise.")]
    public float FixedYOrigin;

    int skyboxResolution;

    void Start()
    {
        skyboxResolution = (int)SkyboxLOD;

        float xOrg = FixedOrigin ? FixedXOrigin : Random.Range(0, 1000);
        float yOrg = FixedOrigin ? FixedYOrigin : Random.Range(0, 1000);

        Material mat = new Material(Shader.Find("Skybox/6 Sided"));

        for (int i = 0; i < 6; i++)
        {
            Texture2D tex = new Texture2D(skyboxResolution, skyboxResolution);
            Color[] col = new Color[skyboxResolution * skyboxResolution];

            col.GetSkyboxFace(xOrg + i * Scale, yOrg - i * Scale, skyboxResolution, Scale);
            
            tex.SetPixels(col);
            tex.Apply();

            switch (i)
            {
                case 0:
                    mat.SetTexture("_FrontTex", tex);
                    break;
                case 1:
                    mat.SetTexture("_BackTex", tex);
                    break;
                case 2:
                    mat.SetTexture("_LeftTex", tex);
                    break;
                case 3:
                    mat.SetTexture("_RightTex", tex);
                    break;
                case 4:
                    mat.SetTexture("_UpTex", tex);
                    break;
                case 5:
                    mat.SetTexture("_DownTex", tex);
                    break;
            }
        }

        if (this.GetComponent<Skybox>() == null)
        {
            this.gameObject.AddComponent<Skybox>();
        }

        this.GetComponent<Skybox>().material = mat;
    }
}