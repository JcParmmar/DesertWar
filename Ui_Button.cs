using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Ui_Button : MonoBehaviour
{
    //Componets How Define the Objects
    [SerializeField] Ui_ManuController manuController;
    [SerializeField] int ButtonIndex;
    
    //Co Componets For Performing Operations
    [SerializeField] Animator anim;
    [SerializeField] Button btn;
    
    //Componets For Macking Sound
    [SerializeField] AudioClip ClickSound;
    [SerializeField] AudioClip PressSound;
    AudioSource audioSource;
    
    //Get Some Input
    bool Tap;

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

    public void OnTapButton(InputAction.CallbackContext value)
    {
        var v = value.ReadValue<float>();

        if (v == 0) Tap = false;
        else Tap = true;
    }

    public void OnReleaseButton(InputAction.CallbackContext value)
    {
        var v = value.ReadValue<float>();

        if (v == 0) Tap = false;
        else Tap = true;
    }

    #endregion

    private void Awake()
    {
        //set All Componets
        btn = GetComponent<Button>();
        anim = GetComponent<Animator>();
        manuController = GetComponentInParent<Ui_ManuController>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    private void Update()
    {
        //Get The Index From Manu Controller If Index Is Same As ButtonIndex Then it Main Player Is Nevigate To The Button
        if (ButtonIndex == manuController.Index)
        {
            //Button State Selected

            anim.SetBool("Selected", true);//Selected Animation
            
            //Get Input Of Enter
            if (Tap)
            {
                //Then Button State Is Press

                anim.SetTrigger("Press"); // Press Animation
                Press();//Play Press Sound
                btn.onClick.Invoke();//Activet All Operation Which Button Perform
            }
        }
        else anim.SetBool("Selected", false); //If Index Is Not Same As ButtonIndex Then State Is Idel
    }

    //This Method Is Use For Jump TO Next Panel
    //This Method Call From Button Componet
    public void OnChangePanel(GameObject NextPanel)
    {
        manuController.gameObject.SetActive(false);
        NextPanel.SetActive(true);
    }

    //Play Click Sound
    //This Method Call From Animation
    private void Click()
    {
        audioSource.PlayOneShot(ClickSound);
    }

    //Play Press Sound
    private void Press()
    {
        audioSource.PlayOneShot(PressSound);
    }
}
