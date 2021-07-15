using UnityEngine;
using UnityEngine.InputSystem;

public class Ui_ManuController : MonoBehaviour
{
    public int Index; // Index Help Use To Nevigate Controls 
    [SerializeField] int MaxIndex; //Max Index Define How Manu Controls In The Panel
    [SerializeField] bool KeyDown=false; //KeyDown Help To Stop Messing With The Keys

    //Get Some Inputs
    bool moveUp;
    bool moveDown;

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

    public void OnMoveUp(InputAction.CallbackContext value)
    {
        var v = value.ReadValue<float>();

        if (v == 0f) moveUp = false;
        else moveUp = true;
    }

    public void OnMoveDown(InputAction.CallbackContext value)
    {
        var v = value.ReadValue<float>();

        if (v == 0f) moveDown = false;
        else moveDown = true;
    }

    #endregion

    private void Update()
    {
        //if We Get Some Input Then
        if(moveUp || moveDown){ 
            
            //Check If Key Is Down
            if (!KeyDown)
            {
                //as perWhich Input We Get And Perform Operation To Incress And Decress Index
                if (moveUp) {
                    if (Index == 0)
                        Index = MaxIndex;
                    else
                        Index--;
                }
                else if (moveDown) {
                    if (Index == MaxIndex)
                        Index = 0;
                    else
                        Index++;
                }

                //Set KeyDown To True
                KeyDown = true;
            } 
        }
        else KeyDown = false;
        //if We Don't Get Any Input Then Set KeyDown To False
    }
}
