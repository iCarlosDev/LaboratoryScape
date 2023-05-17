using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeselectButtonNotPressed : MonoBehaviour, IMoveHandler
{

    [SerializeField] private Button _button;
    [SerializeField] private Slider slider;
    [SerializeField] private Button leftBTN;
    [SerializeField] private Button rightBTN;

    private void Awake()
    {
        _button = GetComponent<Button>();
        slider = GetComponentInChildren<Slider>() ? GetComponentInChildren<Slider>() : null;

        //Comprobamos que haya un botón como hijo del objeto que tenga este script;
        if (GetComponentInChildren<Button>())
        {
            //Recorremos todos los botones que tenga de hijos;
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                //Comprobamos que el botón recorrido se llame "Left_BTN"
                if (button.name.Equals("Left_BTN"))
                {
                    leftBTN = button;
                }
                //Comprobamos que el botón recorrido se llame "Right_BTN"
                else if (button.name.Equals("Right_BTN"))
                {
                    rightBTN = button; 
                }
            } 
        }
    }

    //Método para seleccionar el boton que tenga este script;
    public void SelectButton()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            MainMenuLevelManager.instance.EventSystem.SetSelectedGameObject(gameObject);
        }
        else
        {
            PauseMenuManager.instance.EventSystem.SetSelectedGameObject(gameObject);
        }
    }

    //Método para hacer interactuable o no el botón que tiene este script;
    public void SetInteractable(int isInteractable)
    {
        if (isInteractable.Equals(0))
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }

    //Método que se llama siempre que un botón esté seleccionado y detecte un input de dirección;
    public void OnMove(AxisEventData eventData)
    {
        //Comprobamos que no estémos tecleando arriba o abajo porque si no modificariamos el slider;
        if (eventData.moveDir == MoveDirection.Down || eventData.moveDir == MoveDirection.Up) return;
        
        //Comprobamos que exista un slider;
        if (slider != null)
        {
            SliderControl(eventData);
        }

        if (leftBTN != null && rightBTN != null)
        {
            OptionListControl(eventData);
        }
    }

    //Método para controlar los sliders con las teclas aún teniendo seleccionado el botón padre;
    private void SliderControl(AxisEventData eventData)
    {
        float direction = eventData.moveDir == MoveDirection.Left ? -0.1f : 0.1f;
        slider.value += direction;
    }

    //Método para controlar los botónes con las teclas aún teniendo seleccionado el botón padre;
    private void OptionListControl(AxisEventData eventData)
    {
        //Comprobamos que estémos pulsando la tecla "D"
        if (eventData.moveDir == MoveDirection.Left)
        {
            leftBTN.OnSubmit(eventData);
        }
        else
        {
            rightBTN.OnSubmit(eventData);
        }
    }
}
