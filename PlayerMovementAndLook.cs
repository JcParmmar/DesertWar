using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementAndLook : MonoBehaviour
{
    //private variable which spacialy create for get the input from new Input System
    [Header("Get Input")]
    [SerializeField] private Vector3 MovementVelocity;
    [SerializeField] private Vector2 LookAxis;
    [SerializeField] private bool Running;
    [SerializeField] private bool Jump;

    //co-componest which will perform process 
    [Header("other Scripts and componets")]
    [SerializeField] CharacterController cc; //Movement
    [SerializeField] PlayerRigController rigController; //Rig animation
    [SerializeField] PlayerCinemachinePovExtantion camExtantion; //Look
    [SerializeField] PlayerAnimationsController animationsController; //Movement Animations
    [SerializeField] PlayerShooting shooting; //Shooting

    //Define variabel which will help to perform Movement and Look Opretion
    [Header("Look And Move")]
    public Transform PlayerCam;
    [SerializeField] [Range(2f,5f)] float movementSpeed = 2f;
    [SerializeField] [Range(5f, 8f)] float runningSpeed = 5f;
    [SerializeField] float JumpHeight = 2f; 
    [SerializeField] float Gravity=-9.8f;
    private float Speed;
    private bool isJump=false;
    private float JumpVelocityY;

    //variable which help to check graound
    [Header("Player Grounded")]
    public bool Grounded;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.5f;
    public LayerMask GroundLayers;

    //variable which will help to set camera movement
    [Header("camera Componets")]
    [Range(.5f,3f)] public float RotationSpeed = 1.0f;
    float pitch = 0f;
    [Range(1f, 90f)] public float maxPitch = 85f;//how much look Down
    [Range(-1f, -90f)] public float minPitch = -85f;//how much look Up


    //get the input from the new input system
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

    public void onPlayerMove(InputAction.CallbackContext value)
    {
        var inputMove = value.ReadValue<Vector2>();
        MovementVelocity = new Vector3(inputMove.x,0f,inputMove.y);
    }

    public void OnPlayerLook(InputAction.CallbackContext value)
    {
        var inputLook = value.ReadValue<Vector2>();
        LookAxis = new Vector2(inputLook.x, inputLook.y);
    }

    public void OnPressShift(InputAction.CallbackContext value)
    {
        var inputShift = value.ReadValue<float>();
        if (inputShift == 0)
        {
            Running = false;
        }
        else
        {
            Running = true;
        }
    }

    public void OnPlayerJump(InputAction.CallbackContext value)
    {
        var inputJump = value.ReadValue<float>();
        if (inputJump == 0)
        {
            Jump = false;
        }
        else
        {
            Jump = true;
        }
    }

    #endregion

    private void Awake()
    {
        //Get All Co-Componets
        cc = GetComponent<CharacterController>();
        rigController = GetComponentInChildren<PlayerRigController>();
        camExtantion = GetComponentInChildren<PlayerCinemachinePovExtantion>();
        animationsController = GetComponentInChildren<PlayerAnimationsController>();
        shooting = GetComponent<PlayerShooting>();
    }

    private void Update()
    {
        //Sepret Method of Movement, Ground Check, Jump, Look
        Move();
        GroundedCheck();
        GravityAndJump();
        Look();

        //If Running is True And MovementVelocity.Z (Player Left Right Movement ) then do nothing (It Is Running State) but is not
        if (Running && MovementVelocity.z > 0)
        {
        }
        else
        {
            //the player is not Running so Player Rig is Move To Aim Mode
            //and
            //Player Able To Shoot 
            rigController.isRunning = false;
            shooting.CallShooting();
        }
    }

    //Look Method Is Perform Cam Movements 
    void Look()
    {
        //If Mouse move More then .01 then 
        if (LookAxis.sqrMagnitude >= .01)
        {
            //Input , Max and Mini Pitch , Rotation Speed Will send to CamExtantion
            camExtantion.CamMove = LookAxis;
            camExtantion.maxLook = maxPitch;
            camExtantion.miniLook = minPitch;
            camExtantion.speed = RotationSpeed;

            //Get the Input Of Mouse Left To Right Movement So Player Can adjust Direction
            var x = LookAxis.x*RotationSpeed*Time.deltaTime;
            transform.Rotate(Vector3.up * x);
        }
        else
        {
            //If Mouse is not Moving then Set Speed to 0 So Any Unnessary movement Will ignore
            camExtantion.speed = 0f;
        }
    }

    //Move Method Is For Performing Movement Opretions
    private void Move()
    {
        //If MovementVelocity Is Not Zero Then
        if (MovementVelocity != Vector3.zero)
        {
            //If Running is True And MovementVelocity.Z (Player Left Right Movement ) then
            if (Running && MovementVelocity.z > 0)
            {
                //It Is Running State So Set Speed To Running Speed
                //and change Rig Mode to Running
                Speed = runningSpeed;
                rigController.isRunning = true;
            }
            else
            {
                //It Is Not Running State So Set Speed To Movement Speed
                //and change Rig Mode to Aim
                Speed = movementSpeed;
                rigController.isRunning = false;
            }

            //Calculate Velocity
            Vector3 velocity =
                ((PlayerCam.right * MovementVelocity.x) +
                (PlayerCam.forward * MovementVelocity.z));

            //set Velocity.y To 0 So Player is not Move Upward or Downward
            velocity.y = 0f;

            //Perform Movement By Using cc(CharacterController)
            cc.Move(velocity.normalized * Speed * Time.deltaTime );
            
            //set animations Mode To Movement
            animationsController.OnChangeAnimationMode(1);
        }
        else 
        {
            //if Player is Not Moving Then set animations Mode To Idle
            animationsController.OnChangeAnimationMode(0); 
        }

        //Define Which Movement Animations Will Play
        //animationsController.OnMoveAnimations(Foreward/Backward , Left/Right , Bool Running);
        animationsController.OnMoveAnimations(MovementVelocity.x, MovementVelocity.z, Running);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    //GravityAndJump Method is perform the Jump and apply Gravity 
    void GravityAndJump()
    {
        //Check player is Grounded or Not
        if (Grounded)
        {
            //If it is then it Is Not in Jump State
            isJump = false;
            
            //If Player Get Input Jump Then 
            if (Jump)
            {
                //Player Is In Jump State
                isJump = true;

                //Call Sound System And Play Jump Sound
                gameObject.GetComponentInChildren<SoundSystem>().Jump();
                
                //Add Jump Movement
                JumpVelocityY = Mathf.Sqrt(JumpHeight*-2f*Gravity);
            }
        }
        else 
        {
            //if Player Is Not Grounded Then It Main Player Is In Jump Of Falling State
            isJump = true; 
        }

        //If Player Is In Jump State Then
        if (isJump)
        {
            //Apply Gravity
            JumpVelocityY += Gravity * Time.deltaTime;
        }

        //Perform jump and Gravity
        cc.Move(new Vector3(0f, JumpVelocityY, 0f) * Time.deltaTime);
    }


    //Draw Sphere
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

}
