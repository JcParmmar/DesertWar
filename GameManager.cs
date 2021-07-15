using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static class DesertWarPlayer
    {
        public static string PlayerName = "PlayerName";
        public static int TotalPlay = 0;
    }

    [SerializeField] private bool cursorNotVisible;

    //Componets For Store Player And Enemy Prefabs Also Enemy Spawn Positions
    [Header("Player & Enemy Info")]
    public GameObject PlayerModel; //Player Prefab
    [SerializeField] GameObject Player; //Player Which Spawn
    [SerializeField] GameObject EnemyModel;//Enemy Prefab
    [SerializeField] Transform[] EnemySpawnPos;//Enemy Spawn Position
    
    //Ui Componets Which Help to Display Informations
    [Header("Ui Componets")]
    [SerializeField] TMP_Text EnemySpawnTimer;
    [SerializeField] TMP_Text StatusText;
    [SerializeField] TMP_Text EndScoreText;
    [SerializeField] TMP_Text PlayerInfoName;
    [SerializeField] TMP_Text BulletInfo;
    [SerializeField] TMP_Text ReloadInfo;
    [SerializeField] Slider HelathBar;
    //Ui Componets For Get Informations
    [SerializeField] Ui_InputField SetNameInputField;
    [SerializeField] Image HealthBarFill;
    [SerializeField] Gradient HealthBarColor;

    //Ui Panels And Lobby Camera
    [Header("Camera And Panels")]
    [SerializeField] Camera MainCam;
    [SerializeField] GameObject ManuPanel;
    [SerializeField] GameObject GamePanel;
    [SerializeField] GameObject EndGamePanel;
    [SerializeField] GameObject PausePanel;

    //Some Time Stemp
    [Header("Time Stemp")]
    float EnemySpawnTime = 10f; //Set Time For Spawning
    float EnemySpawnDeltaTime = 15f; //Help TO Calculate Spawn Time
    float DeltaPlayTime;//Total Time Player Spend In Game

    //Some States
    bool GameIsRunning;
    bool GameIsPause=false;

    private void Awake()
    {
        
        if (cursorNotVisible)
        {
            //Not Display Cursor
            Cursor.visible = cursorNotVisible;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //Display Cursor
            Cursor.visible = cursorNotVisible;
            Cursor.lockState = CursorLockMode.None;
        }


        //Display Some Text
        StatusText.text = "Total Play : " + DesertWarPlayer.TotalPlay;
        SetNameInputField.TextChange(DesertWarPlayer.PlayerName);

        //Desable And Enable
        ManuPanel.SetActive(true);
        MainCam.gameObject.SetActive(true);
        GamePanel.SetActive(false);
    }

    private void Update()
    {
        //if game is Running then
        //1. Incress Play Time
        //2. Find Shooting Script And Display Currant Bullet And Max Bullet
        //3. if Player is Relaoding Then show Reloading Text
        //4. Find Health Value Form Health System and set Health Bar Value And Color
        //5. Spawn Enemy At Enemy Spawn Position When the Timer Is Finish 
        //6. Display Remaning Time to Spawn Next Enemy
        //7. Check The Health  if it is bellow then 0 then perform end game process
        //71. find all Enemy and Destroy
        //72. Stop Running Game And Destroy Player
        //73. Enable Camera, End Panel & Desable Game Panel
        //74. Display Speend Time In Game in End Panel

        if (GameIsRunning)
        {
            //1
            DeltaPlayTime += Time.deltaTime;
            
            //2
            var bullet = Player.transform.GetComponent<PlayerShooting>();
            BulletInfo.text = bullet.remaningBullets.ToString() + " / " + bullet.maxBullets.ToString();

            //3
            if (bullet.isReload)
                ReloadInfo.gameObject.SetActive(true);
            else 
                ReloadInfo.gameObject.SetActive(false);

            //4
            var health = Player.transform.GetComponent<PlayerAndEnemyHealthSystem>();
            HelathBar.value = health.CurrantHealth;
            HealthBarFill.color = HealthBarColor.Evaluate(HelathBar.normalizedValue);

            //5
            if (EnemySpawnDeltaTime <= 0f)
            {
                EnemySpawnDeltaTime = EnemySpawnTime;
                var v = Random.Range(0, EnemySpawnPos.Length);
                Instantiate(EnemyModel, EnemySpawnPos[v]);
            }
            else
                EnemySpawnDeltaTime -= Time.deltaTime;

            //6
            EnemySpawnTimer.text = "Enemy Spawn In " + EnemySpawnDeltaTime.ToString();

            //7
            if(health.CurrantHealth == 0)
            {
                //71
                EnemyAI[] curEnemy = FindObjectsOfType<EnemyAI>();
                foreach(var a in curEnemy) {
                    Destroy(a.gameObject);
                }

                //72
                GameIsRunning = false;
                Destroy(Player.gameObject);

                //73
                MainCam.gameObject.SetActive(true);
                EndGamePanel.SetActive(true);
                GamePanel.SetActive(false);

                //74
                EndScoreText.text = "Player Server For Total "+ DeltaPlayTime.ToString() + " Time";
            }
            else gameObject.GetComponent<AudioListener>().enabled = true;
        }

    }

    //This Method Is For Spawn Player And Start The Timer For Spawn Enemy Also Some Smoall Other Stuff
    //This Method Call By Start Game Button
    public void StartGame()
    {
        //Set Game Is Running Now So Play Time Is 0
        GameIsRunning = true;
        DeltaPlayTime = 0f;

        //Incress Total Play And Set Player Name
        DesertWarPlayer.TotalPlay++;
        DesertWarPlayer.PlayerName = SetNameInputField.InputFieldText;
        
        //Spaen Player At Random Position with 0 Rotetion
        Player = Instantiate(PlayerModel,
            new Vector3(Random.Range(-10, 10), 10f, Random.Range(-10, 10)),
            Quaternion.identity);

        //Display Player Name And Diable Game Manager Audio Listener
        PlayerInfoName.text = DesertWarPlayer.PlayerName;
        gameObject.GetComponent<AudioListener>().enabled = false;
    }

    //this Method Call When Esc Key Is Press
    public void OnPressEsc()
    {
        //if Game Is Not Running then Return From Here
        if (Player == null) { return; }

        //Inverse The Pause State (If Pause Then Resume / If Play Then Pause)
        GameIsPause = !GameIsPause;

        //if Game Is Pause Then
        if (GameIsPause)
        {
            //Mack True Some States and Disable Some Componets
            PausePanel.SetActive(true);
            GameIsRunning = false;
            Player.GetComponent<PlayerMovementAndLook>().enabled =false ;

            EnemyAI[] curEnemy = FindObjectsOfType<EnemyAI>();
            foreach (var a in curEnemy)
            {
                a.enabled = false;
                a.ChangeAnimationsState(0);
                a.agent.SetDestination(a.transform.position);
            }
        }
        else
        {
            //If Game Is Running Then

            //Mack False Some States and Enable Some Componets
            PausePanel.SetActive(false);
            GameIsRunning = true;
            Player.GetComponent<PlayerMovementAndLook>().enabled = true;

            EnemyAI[] curEnemy = FindObjectsOfType<EnemyAI>();
            foreach (var a in curEnemy)
            {
                a.enabled = true;
            }
        }
    }


    //This Method is Help to Exit Form Mid Game
    public void OnPlayerExit()
    {
        //Disable Pause Panel
        PausePanel.SetActive(false);

        //Distroy All Enemy
        EnemyAI[] curEnemy = FindObjectsOfType<EnemyAI>();
        foreach (var a in curEnemy)
        {
            Destroy(a.gameObject);
        }

        //Stop Running Game And Destroy Player
        GameIsRunning = false;
        Destroy(Player.gameObject);

        //Active Some Componets
        MainCam.gameObject.SetActive(true);
        EndGamePanel.SetActive(true);
        GamePanel.SetActive(false);

        //Display Total Time Player Survive
        EndScoreText.text = "Player Server For Total " + DeltaPlayTime.ToString() + " Time";
    }

    //This Method Help to Close The Game
    public void OnQuitGame()
    {
        Application.Quit();
    }

    //By using This Method We Can Change Camera Sensitivity From Pause Panel
    //This Method Call From Camera Sensitivity Slider
    public void SetCamSensitivity(float value)
    {
        Player.GetComponent<PlayerMovementAndLook>().RotationSpeed = value;
    }
}
