using Cinemachine;
using UnityEngine;

public class PlayerCinemachinePovExtantion : CinemachineExtension
{
    //Get Input from PlayerMovementAndLook Script
    [HideInInspector] public Vector2 CamMove;
    [HideInInspector] public float maxLook;
    [HideInInspector] public float miniLook;
    [HideInInspector] public float speed;

    private Vector3 camRoteion;

    //When We Extand CinemachineEXtension Then we need to Call Back PostPipelineStageCallback
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        //Check VCam Is In Follow Mode
        if (vcam.Follow)
        {
            //Stage Is In Aim Mode
            if (stage==CinemachineCore.Stage.Aim)
            {
                //Set X Rotetion
                camRoteion.x += CamMove.x * speed * Time.deltaTime;

                //Set Y Rotetion
                camRoteion.y += CamMove.y*speed * Time.deltaTime;
                //Clamp Y Rotetion
                camRoteion.y = Mathf.Clamp(camRoteion.y, miniLook, maxLook);

                //Perform Rotetion
                state.RawOrientation = Quaternion.Euler(-camRoteion.y,camRoteion.x,0f);

            }
        }
    }
}
