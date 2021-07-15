using UnityEngine;
using TMPro;

public class Ui_InputField : MonoBehaviour
{
    //Componets How Define the Objects
    [SerializeField] Ui_ManuController manuController;
    [SerializeField] int InputFieldIndex;

    //Co Componets For Performing Operations
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Animator anim;
    
    //Use For Play Sound
    [SerializeField] AudioClip ClickSound;
    AudioSource audioSource;
    
    //Display Text In Inspector
    public string InputFieldText;

    // Start is called before the first frame update
    void Start()
    {
        //set All Componets
        anim = GetComponent<Animator>();
        manuController = GetComponentInParent<Ui_ManuController>();
        inputField = GetComponent<TMP_InputField>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get The Index From Manu Controller If Index Is Same As InputFieldIndex Then it Main Player Is Nevigate To The InputField
        if (InputFieldIndex == manuController.Index)
        {
            //Input Field State Is Selected
            anim.SetBool("Selected", true);//Perform Animation
            inputField.Select();//Perform Input Field Select
        }
        else 
        {
            //Input Field State Is Idle
            anim.SetBool("Selected", false);
        }
    }

    //This Method Get Text From The InputField And Show In Inspector
    public void TextChange(string text)
    {
        InputFieldText = text;
    }

    //Play Click Sound
    //This Method Call From Animation
    private void Click()
    {
        audioSource.PlayOneShot(ClickSound);
    }
}
