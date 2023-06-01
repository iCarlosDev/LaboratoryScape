using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuLevelManager : MonoBehaviour
{
    public static MainMenuLevelManager instance;
    
    [Header("--- EVENT SYSTEM STUFF ---")]
    [Space(10)]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private ReselectLastSelectedOnInput reselectLastSelectedOnInput;

    [Header("--- ENUMS ---")]
    [Space(10)]
    [SerializeField] private MenuType _menuTypeEnum;
    
    private enum MenuType
    {
        Background,
        MainMenu,
        RepeatTutorial,
        Options,
        Credits
    }

    [SerializeField] private OptionsBTN _optionsBtnEnum;
    private enum OptionsBTN
    {
        Graphics,
        Controls,
        Audio
    }

    [Header("--- MOUSE CONTROL ---")]
    [Space(10)]
    [SerializeField] private Vector3 lastMousePos;
    [SerializeField] private GameObject blockMousePanel;

    [Header("--- ALL CANVAS ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject background_Canvas;
    [SerializeField] private GameObject mainMenu_Canvas;
    [SerializeField] private GameObject repeatTutorial_Canvas;
    [SerializeField] private GameObject options_Canvas;
    [SerializeField] private GameObject graphics_Canvas;
    [SerializeField] private GameObject controls_Canvas;
    [SerializeField] private GameObject audio_Canvas;
    [SerializeField] private GameObject credits_Canvas;

    [Header("--- CANVAS BTNS ---")] 
    [Space(10)] 
    [SerializeField] private bool canNavigate;
    [SerializeField] private GameObject pressAnyButtonBTN;
    [SerializeField] private GameObject startBTN;
    [SerializeField] private GameObject firstRepeatTutorialBTN;
    [SerializeField] private Animator graphicsBTN_Animator;
    [SerializeField] private GameObject firstGraphicsBTN;
    [SerializeField] private Animator controlsBTN_Animator;
    [SerializeField] private GameObject firstControlsBTN;
    [SerializeField] private Animator audioBTN_Animator;
    [SerializeField] private GameObject firstAudioBTN;

    [Header("--- GRAPHICS OPTIONS BTNS VALUES ---")] 
    [Space(10)]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TextMeshProUGUI resolutionTMP;
    [SerializeField] private TextMeshProUGUI maxFrameTMP;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider brightSlider;
    [SerializeField] private TextMeshProUGUI antialiasingTMP;
    [SerializeField] private TextMeshProUGUI shadowQualityTMP;
    
    [Header("--- CONTROLS OPTIONS BTNS VALUES ---")] 
    [Space(10)]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle invertMouseVertical;
    [SerializeField] private Toggle invertMouseHorizontal;
    
    [Header("--- AUDIO OPTIONS BTNS VALUES ---")] 
    [Space(10)]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;

    [Header("--- VOLUME PARAMS ---")] 
    [Space(10)] 
    [SerializeField] private float timeBetweenColors;
    [SerializeField] private Volume _volume;
    [SerializeField] private Fog fog;
    [SerializeField] private Color purpleColor;
    [SerializeField] private Color redColor;
    [SerializeField] private Color yellowColor;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private Color whiteColor;
    
    [Header("--- Credits anim ---")]
    [SerializeField] private Animation credits;
    
    private Coroutine changeMenuColor;

    //GETTERS && SETTERS//
    public EventSystem EventSystem => eventSystem;

    ////////////////////////////////////////////////
    
    private void Awake()
    {
        instance = this;
        
        eventSystem = FindObjectOfType<EventSystem>();
        reselectLastSelectedOnInput = eventSystem.GetComponent<ReselectLastSelectedOnInput>();
        _animator = GetComponent<Animator>();
        _volume = GetComponentInChildren<Volume>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        
        SetVolumeColors();
        SetOptions();
        GoBackGround();
        _animator.ResetTrigger("RemoveBlur");
        canNavigate = true;
    }

    private void Update()
    {
        //Comprobamos si hemos pulsado cualquier boton del mouse;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            ReselectLastButton();
        }
        
        //Comprobamos si pulsamos cualquier tecla y estamos en el Menú Background;
        if (Input.anyKeyDown && _menuTypeEnum == MenuType.Background)
        {
            GoMenu();
            _animator.SetTrigger("AddBlur");
        }

        //Comprobamos que le hemos dado al "Escape";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Comprobamos que estamos en cualquier menu que no sea el principal;
            if (_menuTypeEnum != MenuType.MainMenu)
            {
                GoMenu();
                OptionsManager.instance.SaveOptions();
                OptionsManager.instance.SetAllOptions();
            }
            else
            {
                GoBackGround();
            }
        }

        //Comprobamos que estamos en las opciones;
        if (_menuTypeEnum == MenuType.Options && canNavigate)
        {
            //Si le damos a la Q...;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                NavigateLeft();
                canNavigate = false;
            }
            //Si le damos a la E...;
            if (Input.GetKeyDown(KeyCode.E))
            {
                NavigateRight();
                canNavigate = false;
            }

            //Si le damos a la G...
            if (Input.GetKeyDown(KeyCode.G))
            {
                OptionsManager.instance.SetOptionsDefault();
                SetOptions();
            }
        }

        //Comprobamos si estamos tocando alguna de las siguientes teclas para bloquear el mouse y no generar conflicto a la hora de elegir opciones en el menú
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            blockMousePanel.SetActive(true);
            lastMousePos = Input.mousePosition;
            Cursor.visible = false;
        }

        //Comprobamos si el mouse se ha movido para volver a activarlo;
        if (Input.mousePosition != lastMousePos)
        {
            Cursor.visible = true;
            blockMousePanel.SetActive(false);
        }
    }

    //Método para activar la navegación entre categorías del menú opciones;
    private void ActivateCanNavigate()
    {
        canNavigate = true;
    }

    //Método para setear las opciones cargadas en la UI;
    private void SetOptions()
    {
        Debug.Log("Options Setted");

        fullscreenToggle.isOn = OptionsManager.instance.Fullscreen;
        resolutionTMP.text = $"{OptionsManager.instance.WidthResolutionsList[OptionsManager.instance.ResolutionIndex]}x{OptionsManager.instance.HeightResolutionsList[OptionsManager.instance.ResolutionIndex]}";
        maxFrameTMP.text = $"{OptionsManager.instance.FrameRateList[OptionsManager.instance.FrameRateIndex]}";
        vsyncToggle.isOn = OptionsManager.instance.Vsync;
        brightSlider.value = OptionsManager.instance.Bright;
        
        if (OptionsManager.instance.AntialiasingIndex == 0)
        {
            antialiasingTMP.text = $"DISABLED";
        }
        else
        {
            antialiasingTMP.text = $"x{OptionsManager.instance.AntialiasingList[OptionsManager.instance.AntialiasingIndex]}";
        }
        
        shadowQualityTMP.text = $"{OptionsManager.instance.ShadowsQualityList[OptionsManager.instance.ShadowQualityIndex]}";

        mouseSensitivitySlider.value = OptionsManager.instance.MouseSensitivity;
        invertMouseVertical.isOn = OptionsManager.instance.InvertVertical;
        invertMouseHorizontal.isOn = OptionsManager.instance.InvertHorizontal;

        masterVolumeSlider.value = OptionsManager.instance.MasterVolume;
        musicVolumeSlider.value = OptionsManager.instance.MusicVolume;
        effectVolumeSlider.value = OptionsManager.instance.EffectsVolume;
    }
    
    #region - MAIN MENU -

    public void StartGame()
    {
        if (!CheckpointsManager.instance.TutorialCompleted)
        {
            SceneManager.LoadScene(1);
            return;
        }
        
        GoRepeatTutorial();
    }

    //Método para ir al Menú Background;
    private void OpenBackground()
    {
        eventSystem.SetSelectedGameObject(pressAnyButtonBTN);
        reselectLastSelectedOnInput.LastSelectedObject = pressAnyButtonBTN;
        
        _menuTypeEnum = MenuType.Background;
        
        background_Canvas.SetActive(true);
        mainMenu_Canvas.SetActive(false);
        repeatTutorial_Canvas.SetActive(false);
        options_Canvas.SetActive(false);
        credits_Canvas.SetActive(false);
    }
    
    //Método para abrir el Menú Principal;
    private void OpenMainMenu()
    {
        eventSystem.SetSelectedGameObject(startBTN);
        reselectLastSelectedOnInput.LastSelectedObject = startBTN;
        
        _menuTypeEnum = MenuType.MainMenu;
        
        background_Canvas.SetActive(false);
        mainMenu_Canvas.SetActive(true);
        repeatTutorial_Canvas.SetActive(false);
        options_Canvas.SetActive(false);
        credits_Canvas.SetActive(false);
    }
    
    //Método para abrir las Opciones;
    private void OpenOptions()
    {
        OpenGraphicsMenu();
        
        _menuTypeEnum = MenuType.Options;
        
        mainMenu_Canvas.SetActive(false);
        repeatTutorial_Canvas.SetActive(false);
        options_Canvas.SetActive(true);
        credits_Canvas.SetActive(false);
        
        SelectGraphicsBTN();
    }
    
    //Método para abrir los Creditos;
    private void OpenCredits()
    {
        GetVolumeParameters(whiteColor);
        _menuTypeEnum = MenuType.Credits;
        
        mainMenu_Canvas.SetActive(false);
        repeatTutorial_Canvas.SetActive(false);
        options_Canvas.SetActive(false);
        credits_Canvas.SetActive(true);
        credits.Play();
    }

    //Método para salir del juego;
    public void Quit()
    {
        Application.Quit();
    }
    
    #endregion

    #region - REPEAT TUTORIAL -

    public void OpenRepeatTutorial()
    {
        eventSystem.SetSelectedGameObject(firstRepeatTutorialBTN);
        reselectLastSelectedOnInput.LastSelectedObject = firstRepeatTutorialBTN;

        _menuTypeEnum = MenuType.RepeatTutorial;
        
        background_Canvas.SetActive(false);
        mainMenu_Canvas.SetActive(false);
        repeatTutorial_Canvas.SetActive(true);
        options_Canvas.SetActive(false);
        credits_Canvas.SetActive(false);
    }
    
    public void RepeatTutorial()
    {
        CheckpointsManager.instance.TutorialCompleted = false;
        CheckpointsManager.instance.SaveCheckpoint();
        SceneManager.LoadScene(1);
    }

    public void NoRepeatTutorial()
    {
        SceneManager.LoadScene(1);
    }

    #endregion

    #region - OPTIONS -

    //Método para navegar hacía la izquierda por los botones del encabezado;
    private void NavigateLeft()
    {
        if (_optionsBtnEnum == OptionsBTN.Graphics)
        {
            GoAudio();
            return;
        }
        
        if (_optionsBtnEnum == OptionsBTN.Controls)
        {
            GoGraphics();
            return;
        }
        
        if (_optionsBtnEnum == OptionsBTN.Audio)
        {
           GoControls();
        }
    }
    
    //Método para navegar hacía la derecha por los botones del encabezado;
    private void NavigateRight()
    {
        if (_optionsBtnEnum == OptionsBTN.Graphics)
        {
            GoControls();
            return;
        }
        
        if (_optionsBtnEnum == OptionsBTN.Controls)
        {
            GoAudio();
            return;
        }
        
        if (_optionsBtnEnum == OptionsBTN.Audio)
        {
            GoGraphics();
        }
    }

    //Método para abrir las Opciones de Graficos;
    private void OpenGraphicsMenu()
    {
        GetVolumeParameters(greenColor);
        SelectGraphicsBTN();
        
        eventSystem.SetSelectedGameObject(firstGraphicsBTN);
        reselectLastSelectedOnInput.LastSelectedObject = firstGraphicsBTN;

        _optionsBtnEnum = OptionsBTN.Graphics;
        
        graphics_Canvas.SetActive(true);
        controls_Canvas.SetActive(false);
        audio_Canvas.SetActive(false);
    }
    
    
    //Método para abrir las Opciones de Controles;
    private void OpenControlsMenu()
    {
        GetVolumeParameters(yellowColor);
        SelectControlsBTN();
        
        eventSystem.SetSelectedGameObject(firstControlsBTN);
        reselectLastSelectedOnInput.LastSelectedObject = firstControlsBTN;
        
        _optionsBtnEnum = OptionsBTN.Controls;
        
        graphics_Canvas.SetActive(false);
        controls_Canvas.SetActive(true);
        audio_Canvas.SetActive(false);
    }
    
    //Método para abrir las Opciones de Audio;
    private void OpenAudioMenu()
    {
        GetVolumeParameters(redColor);
        SelectAudioBTN();
        
        eventSystem.SetSelectedGameObject(firstAudioBTN);
        reselectLastSelectedOnInput.LastSelectedObject = firstAudioBTN;
        
        _optionsBtnEnum = OptionsBTN.Audio;
        
        graphics_Canvas.SetActive(false);
        controls_Canvas.SetActive(false);
        audio_Canvas.SetActive(true);
    }

    #endregion

    #region - ANIMATIONS CONTROL -

    private void GoBackGround()
    {
        GetVolumeParameters(purpleColor);
        _animator.SetTrigger("RemoveBlur");
        _animator.SetTrigger("RemoveMenu");
        _animator.SetTrigger("GoBackground");
    }
    
    public void GoMenu()
    {
        _animator.SetTrigger("GoMenu");
        GetVolumeParameters(blueColor);
        
        switch (_menuTypeEnum)
        {
            case MenuType.Background:
                _animator.SetTrigger("RemoveBackground");
                return;
            case MenuType.RepeatTutorial:
                _animator.SetTrigger("RemoveRepeatTutorial");
                break;
            case MenuType.Options:
                _animator.SetTrigger("RemoveOptions");
                return;
            case MenuType.Credits:
                _animator.SetTrigger("RemoveCredits");
                break;
            case MenuType.MainMenu:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void GoRepeatTutorial()
    {
        _animator.SetTrigger("RemoveMenu");
        _animator.SetTrigger("GoRepeatTutorial");
    }

    public void GoOptions()
    {
        _animator.SetTrigger("RemoveMenu");
        _animator.SetTrigger("GoOptions");
    }

    public void GoCredits()
    {
        _animator.SetTrigger("RemoveMenu");
        _animator.SetTrigger("GoCredits");
    }

    public void GoGraphics()
    {
        _animator.SetTrigger("GoGraphics");

        if (_optionsBtnEnum == OptionsBTN.Controls)
        {
            _animator.SetTrigger("RemoveControls");
            return;
        }

        if (_optionsBtnEnum == OptionsBTN.Audio)
        {
            _animator.SetTrigger("RemoveAudio");
        }
    }

    private void SelectGraphicsBTN()
    {
        graphicsBTN_Animator.SetTrigger("Selected");
        controlsBTN_Animator.SetTrigger("Normal");
        audioBTN_Animator.SetTrigger("Normal");
    }
    
    private void SelectControlsBTN()
    {
        graphicsBTN_Animator.SetTrigger("Normal");
        controlsBTN_Animator.SetTrigger("Selected");
        audioBTN_Animator.SetTrigger("Normal");
    }
    
    private void SelectAudioBTN()
    {
        graphicsBTN_Animator.SetTrigger("Normal");
        controlsBTN_Animator.SetTrigger("Normal");
        audioBTN_Animator.SetTrigger("Selected");
    }
    
    public void GoControls()
    {
        _animator.SetTrigger("GoControls");

        if (_optionsBtnEnum == OptionsBTN.Graphics)
        {
            _animator.SetTrigger("RemoveGraphics");
            return;
        }

        if (_optionsBtnEnum == OptionsBTN.Audio)
        {
            _animator.SetTrigger("RemoveAudio");
        }
    }
    
    public void GoAudio()
    {
        _animator.SetTrigger("GoAudio");

        if (_optionsBtnEnum == OptionsBTN.Controls)
        {
            _animator.SetTrigger("RemoveControls");
            return;
        }

        if (_optionsBtnEnum == OptionsBTN.Graphics)
        {
            _animator.SetTrigger("RemoveGraphics");
        }
    }

    #endregion

    #region - CHILD UI CONTROL -

    //Método para activar o desactivar el Toggle de un botón;
    public void ToggleControl()
    {
        Toggle toggle = eventSystem.currentSelectedGameObject.GetComponentInChildren<Toggle>();
        toggle.isOn = !toggle.isOn;
    }

    #endregion

    #region - CHANGE OPTIONS VALUES WITH BUTTONS -

    #region - GRAPHICS OPTIONS -
    
    //Método para cambiar la opción Fullscreen;
    public void ChangeFullScreen(bool isOn)
    {
        OptionsManager.instance.Fullscreen = isOn;
    }

    //Método para cambiar el texto con las resoluciones disponibles;
    public void ChangeResolution(bool isLeft)
    {
        //Comprobamos si el botón clicado es el izquierdo;
        if (isLeft)
        {
            OptionsManager.instance.ResolutionIndex--;

            //Comprobamos si el indice de la lista es menor a "0" para volver al final de la lista;
            if (OptionsManager.instance.ResolutionIndex < 0)
            {
                OptionsManager.instance.ResolutionIndex = OptionsManager.instance.WidthResolutionsList.Count - 1;
            }
        }
        else
        {
            OptionsManager.instance.ResolutionIndex++;
            
            //Comprobamos si el indice de la lista es mayor al último indice de la lista para volver al principio;
            if (OptionsManager.instance.ResolutionIndex > (OptionsManager.instance.WidthResolutionsList.Count - 1))
            {
                OptionsManager.instance.ResolutionIndex = 0;
            }
        }
        
        resolutionTMP.text = $"{OptionsManager.instance.WidthResolutionsList[OptionsManager.instance.ResolutionIndex]}x{OptionsManager.instance.HeightResolutionsList[OptionsManager.instance.ResolutionIndex]}";
    }

    //Método para cambiar el texto con los FrameRates disponibles;
    public void ChangeFrameRate(bool isLeft)
    {
        //Comprobamos si el botón clicado es el izquierdo;
        if (isLeft)
        {
            OptionsManager.instance.FrameRateIndex--;

            //Comprobamos si el indice de la lista es menor a "0" para volver al final de la lista;
            if (OptionsManager.instance.FrameRateIndex < 0)
            {
                OptionsManager.instance.FrameRateIndex = OptionsManager.instance.FrameRateList.Count - 1;
            }
        }
        else
        {
            OptionsManager.instance.FrameRateIndex++;
            
            //Comprobamos si el indice de la lista es mayor al último indice de la lista para volver al principio;
            if (OptionsManager.instance.FrameRateIndex > (OptionsManager.instance.FrameRateList.Count - 1))
            {
                OptionsManager.instance.FrameRateIndex = 0;
            }
        }
        
        maxFrameTMP.text = $"{OptionsManager.instance.FrameRateList[OptionsManager.instance.FrameRateIndex]}";
    }

    //Método para cambiar el Vsync;
    public void ChangeVsync(bool isOn)
    {
        OptionsManager.instance.Vsync = isOn;
    }

    //Método para cambiar el brillo;
    public void ChangeBright(float brightValue)
    {
        OptionsManager.instance.Bright = brightValue;
    }

    //Método para cambiar el texto con el antialiasing disponible;
    public void ChangeAntialiasing(bool isleft)
    {
        //Comprobamos si el botón clicado es el izquierdo;
        if (isleft)
        {
            OptionsManager.instance.AntialiasingIndex--;

            //Comprobamos si el indice de la lista es menor a "0" para volver al final de la lista;
            if (OptionsManager.instance.AntialiasingIndex < 0)
            {
                OptionsManager.instance.AntialiasingIndex = OptionsManager.instance.AntialiasingList.Count - 1;
            }
        }
        else
        {
            OptionsManager.instance.AntialiasingIndex++;
            
            //Comprobamos si el indice de la lista es mayor al último indice de la lista para volver al principio;
            if (OptionsManager.instance.AntialiasingIndex > (OptionsManager.instance.AntialiasingList.Count - 1))
            {
                OptionsManager.instance.AntialiasingIndex = 0;
            }
        }

        //Comprobamos si el indice de la lista antialiasing es 0 para cambiar el texto a "DISABLED";
        if (OptionsManager.instance.AntialiasingIndex == 0)
        {
            antialiasingTMP.text = $"DISABLED";
        }
        else
        {
            antialiasingTMP.text = $"x{OptionsManager.instance.AntialiasingList[OptionsManager.instance.AntialiasingIndex]}";
        }
    }

    //Método para cambiar el texto con el ShadowQuality disponible;
    public void ChangeShadowQuality(bool isLeft)
    {
        //Comprobamos si el botón clicado es el izquierdo;
        if (isLeft)
        {
            OptionsManager.instance.ShadowQualityIndex--;

            //Comprobamos si el indice de la lista es menor a "0" para volver al final de la lista;
            if (OptionsManager.instance.ShadowQualityIndex < 0)
            {
                OptionsManager.instance.ShadowQualityIndex = OptionsManager.instance.ShadowsQualityList.Count - 1;
            }
        }
        else
        {
            OptionsManager.instance.ShadowQualityIndex++;
            
            //Comprobamos si el indice de la lista es mayor al último indice de la lista para volver al principio;
            if (OptionsManager.instance.ShadowQualityIndex > (OptionsManager.instance.ShadowsQualityList.Count - 1))
            {
                OptionsManager.instance.ShadowQualityIndex = 0;
            }
        }

        shadowQualityTMP.text = $"{OptionsManager.instance.ShadowsQualityList[OptionsManager.instance.ShadowQualityIndex]}";
    }
    
    #endregion

    #region - CONTROLS OPTIONS -

    public void ChangeMouseSensitivity(float mouseSensitivitySlider)
    {
        OptionsManager.instance.MouseSensitivity = mouseSensitivitySlider;
    }

    public void InvertMouseHorizontal(bool isOn)
    {
        OptionsManager.instance.InvertHorizontal = isOn;
    }
    
    public void InvertMouseVertical(bool isOn)
    {
        OptionsManager.instance.InvertVertical = isOn;
    }

    #endregion

    #region - AUDIO OPTIONS -

    public void ChangeMasterVolume(float masteVolumeSlider)
    {
        OptionsManager.instance.MasterVolume = masteVolumeSlider;
    }

    public void ChangeMusicVolume(float musicVolumeSlider)
    {
        OptionsManager.instance.MusicVolume = musicVolumeSlider;
    }

    public void ChangeEffectsVolume(float effectsVolumeSlider)
    {
        OptionsManager.instance.EffectsVolume = effectsVolumeSlider;
    }

    #endregion

    #endregion

    #region - VOLUME PARAMETERS -

    private void SetVolumeColors()
    {
        purpleColor = new Color(0.89f, 0.55f, 1f, 1f);
        redColor = new Color(1f, 0.28f, 0.17f, 1f);
        yellowColor = new Color(1f, 0.73f, 0.17f,1f);
        greenColor = new Color(0.3f, 1f, 0.3f, 1f);
        blueColor = new Color(0.3f, 0.53f, 1f, 1f);
        whiteColor = new Color(1f, 1f, 1f, 1f);
    }
    
    private void GetVolumeParameters(Color color)
    {
        _volume.profile.TryGet(out fog);

        if (changeMenuColor != null)
        {
            StopCoroutine(changeMenuColor);
            changeMenuColor = null;
        }
        
        changeMenuColor = StartCoroutine(ChangeMenuColor_Coroutine(color));
    }

    private IEnumerator ChangeMenuColor_Coroutine(Color color)
    {
        while (fog.albedo.value != color)
        {
            fog.albedo.value = Color.Lerp(fog.albedo.value, color, timeBetweenColors);
            yield return null;
        }
    }

    #endregion
    
    //Método para cuando se hace click fuera de un botón no se deseleccione;
    public void ReselectLastButton()
    {
        eventSystem.SetSelectedGameObject(reselectLastSelectedOnInput.LastSelectedObject);
    }
}
