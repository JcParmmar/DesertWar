using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRigController : MonoBehaviour
{
    //Get isRunning From PlayerMovmentAndLook Script
    [HideInInspector] public bool isRunning;

    //Get Rig
    [SerializeField] Rig AimRig;
    //Set Smooth Time So Rig Is Smoothly Convert
    [SerializeField] [Range(0,1)] float smoothTime=.2f;
    
    private void Update()
    {
        //Check Player Is Running
        if (isRunning)
        {
            //If It Is Running then Change Aim Weght To 0
            AimRig.weight -= Time.deltaTime / smoothTime;
        }
        else
        {
            //If It Is Running then Change Aim Weght To 1
            if (AimRig.weight != 1) AimRig.weight += Time.deltaTime / smoothTime;
        }
    }
}
