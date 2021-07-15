using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Ui_AdjustControl : MonoBehaviour
{
    //Componets How Define the Objects
    public Ui_ManuController manuController;
    public int adjustIndex;
    bool KeyDown = false;

    //Set Some Value To Slider
    public float Value = 10;
    public float MaxValue = 10f;
    public Slider AdjustSlider;
    
    //Play Some Sound
    [SerializeField] AudioClip ClickSound;
    AudioSource audioSource;
    
    [SerializeField] GameObject Arrow;

    //Get Some Input
    bool moveLeft;
    bool moveRight;

    //Get Input Form The New Input System
    #region GetInput

    /*
    public Void Method(value We Get From New Input System)
    {
        Create Variable and Set Value = Convert Input System Value In To Normal Variable Value (Float)
    
        if(Some Condiations As Per Variabel){
         Set Variable Input
        }
    }

    */

    public void OnPressLeft(InputAction.CallbackContext v)
    {
        var des = v.ReadValue<float>();

        if (des == 0) 
        {
            moveLeft = false;
        }
        else
        {
            moveLeft=true;
        }
    }

    public void OnPressRight(InputAction.CallbackContext v)
    {
        var des = v.ReadValue<float>();

        if (des == 0)
        {
            moveRight = false;
        }
        else
        {
            moveRight = true;
        }
    }

    #endregion

    private void Awake()
    {
        //set All Componets
        manuController = GetComponentInParent<Ui_ManuController>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    private void Update()
    {
        //Get The Index From Manu Controller If Index Is Same As AdjustIndex Then it Main Player Is Nevigate To The Slider
        if (adjustIndex == manuController.Index)
        {
            //Slier In Selected State
            Arrow.SetActive(true);

            //if We Get Some Input Then
            if (moveLeft || moveRight)
            {
                //Check If Key Is Down
                if (!KeyDown)
                {
                    //as perWhich Input We Get And Perform Operation To Incress And Decress Slider Value
                    if (moveLeft)
                    {
                        if (Value == 0)
                            return;
                        else
                            Value--;
                    }
                    else if (moveRight)
                    {
                        if (Value == MaxValue)
                            return;
                        else
                            Value++;
                    }

                    //Set KeyDown To True
                    KeyDown = true;
                }
            }
            else KeyDown = false;
            //if We Don't Get Any Input Then Set KeyDown To False
        }
        else Arrow.SetActive(false);
        //Slier In Selected Idle
    }

    private void LateUpdate()
    {
        //Set Slider Value As Same As Value Variable
        AdjustSlider.value = Value;
    }

    //Play Sound From Value Change
    public void Click()
    {
        audioSource.PlayOneShot(ClickSound);
    }

}