using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    [Header("--- OPTIONS TO SHOW ---")]
    [Space(10)]
    [SerializeField] private List<int> widthResolutionsList;
    [SerializeField] private List<int> heightResolutionsList;
    [SerializeField] private List<int> frameRateList;
    [SerializeField] private List<int> antialiasingList;
    [SerializeField] private List<string> shadowsQualityList;

    [Header("--- GRAPHIC OPTIONS IN USE")] 
    [Space(10)] 
    [SerializeField] private bool fullscreen;
    [SerializeField] private int resolutionIndex;
    [SerializeField] private int frameRateIndex;
    [SerializeField] private bool vsync;
    [SerializeField] private float bright;
    [SerializeField] private int antialiasingIndex;
    [SerializeField] private int shadowQualityIndex;
    
    [Header("--- CONTROLS OPTIONS IN USE")] 
    [Space(10)] 
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private bool invertHorizontal;
    [SerializeField] private bool invertVertical;
    
    [Header("--- AUDIO OPTIONS IN USE")] 
    [Space(10)] 
    [SerializeField] private float masterVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float effectsVolume;
    
    //GETTERS && SETTERS//
    public List<int> WidthResolutionsList => widthResolutionsList;
    public List<int> HeightResolutionsList => heightResolutionsList;
    public List<int> FrameRateList => frameRateList;
    public List<int> AntialiasingList => antialiasingList;
    public List<string> ShadowsQualityList => shadowsQualityList;

    public bool Fullscreen
    {
        get => fullscreen;
        set => fullscreen = value;
    }
    public int ResolutionIndex
    {
        get => resolutionIndex;
        set => resolutionIndex = value;
    }
    public int FrameRateIndex
    {
        get => frameRateIndex;
        set => frameRateIndex = value;
    }
    public bool Vsync
    {
        get => vsync;
        set => vsync = value;
    }
    public float Bright
    {
        get => bright;
        set => bright = value;
    }
    public int AntialiasingIndex
    {
        get => antialiasingIndex;
        set => antialiasingIndex = value;
    }
    public int ShadowQualityIndex
    {
        get => shadowQualityIndex;
        set => shadowQualityIndex = value;
    }
    public float MouseSensitivity
    {
        get => mouseSensitivity;
        set => mouseSensitivity = value;
    }
    public bool InvertHorizontal
    {
        get => invertHorizontal;
        set => invertHorizontal = value;
    }
    public bool InvertVertical
    {
        get => invertVertical;
        set => invertVertical = value;
    }
    public float MasterVolume
    {
        get => masterVolume;
        set => masterVolume = value;
    }
    public float MusicVolume
    {
        get => musicVolume;
        set => musicVolume = value;
    }
    public float EffectsVolume
    {
        get => effectsVolume;
        set => effectsVolume = value;
    }

    ////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GetResolutions();
        
        //Comprobamos que existe le key "FullScreen" para saber si tiene alguna configuración guardada, si no, ponemos las opciones a default;
        if (!PlayerPrefs.HasKey("FullScreen"))
        {
            SetOptionsDefault();
        }
     
        LoadOptions();
        SetAllOptions();
    }

    //Método para obtener todas las resoluciones disponibles para el monitor del usuario;
    private void GetResolutions()
    {
        Resolution[] resolution = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray(); 
        widthResolutionsList.Clear();
        heightResolutionsList.Clear();
        for (int i = 0; i < resolution.Length; i++)
        {
            widthResolutionsList.Add(resolution[i].width);
            heightResolutionsList.Add(resolution[i].height);
        }
    }

    //Método para setear todas las opciones;
    public void SetAllOptions()
    {
        SetFullScreen();
        SetFrameRate();
        SetResolution();
        SetVsync();
        SetBright();
        SetAntialiasing();
        SetShadowQuality();
        FindObjectOfType<PlayerMovement>()?.SetSensitivityOptions();
        FindObjectOfType<SoldierFP_Controller>()?.CameraPivot.GetComponent<EnemyMouseLook>().SetSensitivityOptions();
        FindObjectOfType<PlayerFPLook>()?.SetSensitivityOptions();
    }
    
    #region - OPTIONS SET -

    //Método para setear la pantalla completa;
    private void SetFullScreen()
    {
        Screen.fullScreen = fullscreen;
    }
    
    //Método para setear la resolución;
    private void SetResolution()
    {
        Screen.SetResolution(widthResolutionsList[resolutionIndex], heightResolutionsList[resolutionIndex], fullscreen, frameRateList[frameRateIndex]);
    }

    //Método para setear el Frame Rate;
    private void SetFrameRate()
    {
        Application.targetFrameRate = frameRateList[frameRateIndex];
    }

    //Método para setear la sincronización vertical;
    private void SetVsync()
    {
        //Comprobamos is está activo o no el botón de vsync;
        if (vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    //Método para setear el brillo de la pantalla;
    private void SetBright()
    {
        Screen.brightness = bright;
    }

    //Método para setear el antialiasing;
    private void SetAntialiasing()
    {
        QualitySettings.antiAliasing = antialiasingList[antialiasingIndex];
    }

    //Método para setear la calidad de las sombras;
    private void SetShadowQuality()
    {
        //Cambiamos a una configuración distinta dependiendo del indice de la variable "shadowQualityIndex";
        switch (shadowQualityIndex)
        {
            case 0: QualitySettings.shadowResolution = ShadowResolution.Low; break;
            case 1: QualitySettings.shadowResolution = ShadowResolution.Medium; break;
            case 2: QualitySettings.shadowResolution = ShadowResolution.High; break;
            case 3: QualitySettings.shadowResolution = ShadowResolution.VeryHigh; break;
        }
    }

    #endregion
    
    //Método para setear las opciones a Default;
    public void SetOptionsDefault()
    {
        Debug.Log("Options Set Defautlt");
        
        fullscreen = true;
        resolutionIndex = widthResolutionsList.Count - 1;
        frameRateIndex = 1;
        vsync = true;
        bright = 0.5f;
        antialiasingIndex = antialiasingList.Count - 1;
        shadowQualityIndex = shadowsQualityList.Count - 1;

        mouseSensitivity = 1f;
        invertVertical = false;
        invertHorizontal = false;

        masterVolume = 1f;
        musicVolume = 1f;
        effectsVolume = 1f;
        
        SaveOptions();
    }

    //Método para guardar la configuración de las opciones;
    public void SaveOptions()
    {
        Debug.Log("Options Saved");
        
        if (fullscreen)
        {
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }
        
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.SetInt("FrameRate", frameRateIndex);

        if (vsync)
        {
            PlayerPrefs.SetInt("Vsync", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Vsync", 0);
        }
        
        PlayerPrefs.SetFloat("Bright", bright);
        PlayerPrefs.SetInt("Antialiasing", antialiasingIndex);
        PlayerPrefs.SetInt("ShadowQuality", shadowQualityIndex);
        
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        
        if (invertHorizontal)
        {
            PlayerPrefs.SetInt("InvertHorizontal", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertHorizontal", 0);
        }

        if (invertVertical)
        {
            PlayerPrefs.SetInt("InvertVertical", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertVertical", 0);
        }
        
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        
    }

    //Método para cargar la configuración guardada;
    private void LoadOptions()
    {
        Debug.Log("Options Loaded");
        
        if (PlayerPrefs.GetInt("FullScreen").Equals(1))
        {
            fullscreen = true;
        }
        else
        {
            fullscreen = false;
        }

        if (resolutionIndex > widthResolutionsList.Count)
        {
            resolutionIndex = widthResolutionsList.Count - 1;
        }
        else
        {
            resolutionIndex = PlayerPrefs.GetInt("Resolution");
        }
        
        frameRateIndex = PlayerPrefs.GetInt("FrameRate");

        if (PlayerPrefs.GetInt("Vsync").Equals(1))
        {
            vsync = true;
        }
        else
        {
            vsync = false; 
        }
        
        bright = PlayerPrefs.GetFloat("Bright");
        antialiasingIndex = PlayerPrefs.GetInt("Antialiasing");
        shadowQualityIndex = PlayerPrefs.GetInt("ShadowQuality");

        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");

        if (PlayerPrefs.GetInt("InvertHorizontal").Equals(1))
        {
            invertHorizontal = true;
        }
        else
        {
            invertHorizontal = false;
        }

        if (PlayerPrefs.GetInt("InvertVertical").Equals(1))
        {
            invertVertical = true;
        }
        else
        {
            invertVertical = false;
        }

        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
    }
}
