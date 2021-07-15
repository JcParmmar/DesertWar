using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{

    //Check Some State
    [Header("bools")]
    [SerializeField] bool isRadyToShoot;
    [SerializeField] bool isShooting;
    public bool isReload;
    [SerializeField] bool isFireHold; // This Bool Is Set From Inspecter
                                                      // If Is True Then Player Can Able To Hold Button Or
                                                      // If Is Not Then Player Need To Use Taps

    [Header("floats")]
    public int maxBullets; // Set Max Bullets
    public int remaningBullets; //Remaing Bullets
    [SerializeField] float nextBulletsTime; //Time Between Bullets
    [SerializeField] float shootForce; // This Variable is Use For Speed
    [SerializeField] [Range(00f,5f)] float impectForce;//This Variabe is Use For Apply Bullet Drop
    float BulletTime;//This Variable Help For Calculate Next Bullet Time
    [SerializeField] float reloadTime;//Time Tack For Reload

    //Some Componets For Calculation
    [Header("Componets")]
    [SerializeField] Camera cam;
    [SerializeField] Transform FirePoint;
    [SerializeField] GameObject BulletObject;
    [SerializeField] ParticleSystem MuzzalFlash;


    [Header("Sound")]
    [SerializeField] AudioClip FiringSound;
    [SerializeField] AudioClip ReloadingSound;
    AudioSource audioSource;

    //Get Input From New Input System
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
    public void OnFireHold(InputAction.CallbackContext value)
    {
        if (isFireHold)
        {
            var fire = value.ReadValue<float>();

            if (fire == 0) { isShooting = false; }
            else { isShooting = true; }
        }
    }

    public void OnFireTap(InputAction.CallbackContext value)
    {
        if (!isFireHold)
        {
            var fire = value.ReadValue<float>();

            if (fire == 0) { isShooting = false; }
            else { isShooting = true; }
        }
    }

    public void OnReload(InputAction.CallbackContext value)
    {
        var reload = value.ReadValue<float>();

        if (reload == 0) return;
        else
        {
            if (remaningBullets != maxBullets)
            {
                audioSource.PlayOneShot(ReloadingSound);
                isReload = true;
                Invoke("ReloadBullets", reloadTime);
            }
        }
    }

    #endregion

    private void Awake()
    {
        //Set Componets
        cam = GetComponentInChildren<Camera>();
        audioSource = GetComponent<AudioSource>();

        //Set Remaing Bullets As MaxBullets
        remaningBullets = maxBullets;

        //Set Player Is Ready for Shoot
        isRadyToShoot = true;
    }   

    public void CallShooting()
    {
        //Calculate Bullet Time
        BulletTime -= Time.deltaTime;

        //If Bullet Time Is Less Then 0 Then Player Is Ready To Shoot 
        if (BulletTime <= 0)
        {
            isRadyToShoot = true;
        }

        //If Player Is Rady To Shoot 
            // && Get The Input For Shoot
            // && Player Is Not Reloading
            //&& Player Is Not Out Of Bullets
        if (isRadyToShoot && isShooting && !isReload && remaningBullets > 0)
        {
            //Then Shoot
            Shoot();    
        }
    }

    //Perform Sooting
    void Shoot()
    {
        //Now Set Player Is Not Ready For Shoot
        isRadyToShoot = false;
        
        //Decress Reaming Bullets
        remaningBullets--;

        //Set Bullet Time
        BulletTime = nextBulletsTime;
        
        //Emit Muzzal Flash And Play Sound
        MuzzalFlash.Emit(1);
        audioSource.PlayOneShot(FiringSound);

        //Create Some Variable For Help to Find The Target
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
        RaycastHit hit;
        Vector3 TargetPos;

        //If Ray Hit Something the Set Target That Positon Other Wise Set Farest Position 
        if (Physics.Raycast(ray, out hit))
            TargetPos = hit.point;
        else
            TargetPos = ray.GetPoint(100f);

        //Calculate Direaction
        Vector3 direction = TargetPos - FirePoint.position;

        //Create Bullet And Set It Rotetion
        GameObject CurrentBulllet = Instantiate(BulletObject, FirePoint.position, Quaternion.identity);
        CurrentBulllet.transform.forward = direction.normalized;
        CurrentBulllet.GetComponent<PlayerBullets>().effectForward = hit.normal;

        //Add Foreces
        CurrentBulllet.GetComponent<Rigidbody>().AddForce(direction.normalized*shootForce,ForceMode.Impulse); // For Bullet Move Forward
        CurrentBulllet.GetComponent<Rigidbody>().AddForce(-cam.transform.up*impectForce,ForceMode.Impulse); //For Bullet Drop

        //If Player Out Of Bullets
        if (remaningBullets <= 0) 
        {            
            audioSource.PlayOneShot(ReloadingSound); //Play Reload Sound
            isReload = true;//Set Player Is Reloading
            Invoke("ReloadBullets", reloadTime); //Complet Reloading
        }
    }

    //Perform Reloading
    void ReloadBullets()
    {
        isReload = false;
        Debug.Log("Bullets Reloading");
        remaningBullets = maxBullets;
    }
}