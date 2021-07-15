using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("NaveMesh")]
    public NavMeshAgent agent; //NevMesh is Help To Move Arround to Find Player
    //TargetPlayer Transform Help to Find Where the player is Standing and Help to Follow the Player
    [SerializeField]Transform targetPlayer;
    //Check Layer Is Ground Or A Player
    [SerializeField]LayerMask WhatIsGround, WhatIsPlayer;

    //Set State Petroling
    [Header("Petroling")]
    Vector3 walkPoint;//walkPoint Where the player is going for petroling
    bool walkPointIsSet;//check walkpoint is set or not
    [SerializeField]float walkPoinRang; //how much far the player goes

    //Set State Attacking
    [Header("Attacking")]
    [SerializeField]float timeBetweenAttack;
    bool isAttack;

    //Check all States
    [Header("State")]
    [SerializeField]float sightRang;
    [SerializeField]float attackRang;
    bool PlayerInAttackRang;
    bool PlayerInSightRang;
    bool EnemyIsDie=false;

    //set animator for play animations
    [Header("Animations")]
    [SerializeField] Animator animator;

    //Set Variable for shooting
    [Header("Bullet shooting")]
    [SerializeField] GameObject GunObject;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] Transform gunMuzzel;
    [SerializeField] ParticleSystem MuzzalFlash;
    [SerializeField] float ShootForce;
    [SerializeField] GameObject BulletObject;

    //Set Variable For check the Ground
    [Header("Ground Check")]
    bool Grounded;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.5f;
    public LayerMask GroundLayers;
    float VelocityY;

    //Set Enemy Health Bar
    [Header("Health Bar System")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Image HealthBarFill;
    [SerializeField] Gradient HealthBarColor;
    [SerializeField] GameObject Canvas;
    [SerializeField] Camera PlayerCam;

    private void Awake()
    {
        //Get All Co-Componets
        rigBuilder = GetComponentInChildren<RigBuilder>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        PlayerCam = FindObjectOfType<Camera>();

        //Set Player Is Not Die
        animator.SetBool("Die", false);
        
        //Active Gun Objects
        GunObject.SetActive(true);
    }

    private void Update()
    {
        //Ground check
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        //If Enemy Is Not Grounded
        if (!Grounded)
        {
            //Apply gravity
            VelocityY += -9.8f * Time.deltaTime;
            gameObject.transform.GetComponent<CharacterController>().Move(new Vector3(0f, VelocityY, 0f) * Time.deltaTime);
        }
        else
        {
            //State Player Is in Grounded
            VelocityY = -2f;
        }

        //If Player is Die then return from here
        if (EnemyIsDie) return;

        //check player is in the Sight Rang
        PlayerInSightRang = Physics.CheckSphere(transform.position, sightRang, WhatIsPlayer);
        
        //Check Player is in the Attack Rang
        PlayerInAttackRang = Physics.CheckSphere(transform.position, attackRang, WhatIsPlayer);

        //If Player Is Not In Sight Rang then Enemy Is Petroling
        if (!PlayerInSightRang) { Petroling(); }
        else
        {
            //If Player Is In Sight Rang And Attack Rang then Enemy Is Attacking
            if (PlayerInAttackRang) AttackPlayer();
            //If Player Is In Sight Rang But Not In Attack Rang then Enemy Is In Chaseing The Player
            else ChasePlayer(); 
        }
    }

    private void LateUpdate()
    {
        //Set Health Bar
        HealthBarValue();
    }

    //Perform Petroling
    void Petroling()
    {
        //if WalkPoint Is not Set Then Set The WalkPoint
        if (!walkPointIsSet)
        {
            //Get Random Position 
            float x = Random.Range(-walkPoinRang , walkPoinRang);
            float z = Random.Range(-walkPoinRang , walkPoinRang);

            //Set WalkPoint
            walkPoint = new Vector3(transform.position.x + x, transform.position.y ,transform.position.z + z);

            //Check The walk Point Is Graound or Not
            if (Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsGround)) walkPointIsSet = true;
        }

        //If WalkPoint Is Set Then Give Comand The NavMesh Agent to Move At WalkPoint
        if (walkPointIsSet) agent.SetDestination(walkPoint);
        
        //Set Animations Mode To Movemet
        ChangeAnimationsState(1);

        //Calculate The Distance between the Currant Positon and Walk Point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //If Distance is Less Then 1m then Set WalkPoint To null
        if (distanceToWalkPoint.magnitude < 1f) walkPointIsSet = false;
    }

    //Perform Chaseing
    void ChasePlayer()
    {
        //set Animation Mode In to Movement
        ChangeAnimationsState(1);

        //Set Target Player
        targetPlayer = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //Give Comand To NavMesh Agent To Follow The TargetPlayer
        agent.SetDestination(targetPlayer.position);
    }

    //Perform Attacking
    void AttackPlayer()
    {
        //Stop Enemy Movement
        agent.SetDestination(transform.position );

        //set Where the Enemy is Looking
        transform.LookAt(new Vector3(targetPlayer.position.x, 0f , targetPlayer.position.z));

        //if Player Is Not Attacking Then Attack
        if (!isAttack)
        {
            //Set Player Is Attacking
            isAttack = true;

            //Perform Attack
            Attack();

            //Check Animation Mode To Attack
            ChangeAnimationsState(2);

            //Reset Attack after Some Time
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }
    }

    //Perform Attack
    void Attack()
    {
        //Find The Point Where the gun is Pointing
        RaycastHit hit;
        if (Physics.Raycast(gunMuzzel.position,gunMuzzel.forward,out hit))
        {
            //save the hit point and send to the ShootBullet Method
            ShootBullet(hit);
        }
    }

    //ResetAttack
    void ResetAttack()
    {
        isAttack = false;
        ChangeAnimationsState(0);
    }

    //Chenge Animations States
    // i = 0 is for idle
    // i = 1 is for movement
    // i = 2 is for Attack
    public void ChangeAnimationsState(int i)
    {
        animator.SetInteger("EnemyAnim", i);
    }

    //Perfrom Shooting
    //ShootBullet(Where the bullet hit)
    void ShootBullet(RaycastHit hitPos)
    {
        MuzzalFlash.Emit(1);//Flash Muzzal

        //set Target Position
        Vector3 TargetPos = hitPos.point;
        //Set Direction So Bullet Look at Target Position
        Vector3 direction = TargetPos - gunMuzzel.position;

        //Create Bullet located at Muzzal with 0 Rotetion
        GameObject CurrentBulllet = Instantiate(BulletObject, gunMuzzel.position, Quaternion.identity);
        //Make Bullet Look At Target Position
        CurrentBulllet.transform.forward = direction.normalized;
        //Get PlayerBullets From the Bullet And set EffectForward To hitpos.normal
        CurrentBulllet.GetComponent<PlayerBullets>().effectForward = hitPos.normal;

        //it is use to perform impect at player but we are not using the rigidbody so this line is not working
        CurrentBulllet.GetComponent<Rigidbody>().AddForce(direction.normalized * ShootForce, ForceMode.Impulse);
    }

    //Perform Death animations
    public void EnemyDie()
    {
        //in case Enemy is Moveing and die then stop at where he die 
        agent.SetDestination(transform.position);
        
        //Set EnemyIsDie to true so script can not perform any forther operations
        EnemyIsDie = true;

        //Desable some Componets
        rigBuilder.enabled = false;
        GunObject.SetActive(false);
        
        //Perform Die Animation
        animator.SetBool("Die",true);
    }

    //Controle Enemy Health bar 
    void HealthBarValue()
    {
        //Get Health From PlayerAndEnemyHealthSystem
        var health = GetComponent<PlayerAndEnemyHealthSystem>();

        //Set The Health Bat Value and Color
        healthBarSlider.value = health.CurrantHealth;
        HealthBarFill.color = HealthBarColor.Evaluate(healthBarSlider.normalizedValue);

        //Make Health Bar Is Look At Player
        Canvas.transform.LookAt(Canvas.transform.position + PlayerCam.transform.forward);
    }

    //Create Wire Sphere
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRang);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRang);
    }
}
