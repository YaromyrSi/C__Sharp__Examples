using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using System.Threading.Tasks;


public class Player : MonoBehaviour
{
    public float horizontalInput;
    //jump 
    private bool jumpIsAllow;
    private float jumpReloadTime = 1.1f;
    private bool jumpReloadTimeTrigger = true;//if false jump is not allow
    public float speedPlayer = 5;
    [HideInInspector] public Rigidbody2D rigidbody;
    public Animator animator;
    public float move;
    [SerializeField] private float gravityScale = 5;
    [SerializeField] private float fallingGravityScale = 7;
    [SerializeField] private int playerjumpforce = 25;
    //points spawn ammo
    [Header("ammoForEnemy")]
    public GameObject pointSpawnAmmoForGunOne;
    public GameObject pointSpawnAmmoForGunTwo;
    public GameObject pointSpawnAmmoForGunThree;
    public GameObject currentSpawnPointAmmo;
    // ammo for player
    [Header("ammoForPlayer)]
    public GameObject AmmoForGunOne;
    public GameObject AmmoForGunTwo;
    public GameObject AmmoForGunThree;
    public GameObject CurrentAmmo;
    public GameObject gun;

    private float timeToReloadShot = 2;
    private bool triggerReloadShot = true;
    //life variable
    public float lifePlayer = 100;
    private float maxPlayerLife = 100;
    public Image imageLifePlayer;
    private float currentLife;
    private float speedlifeChange = 2;
    //enemy GameObject
    public GameObject enemy;
    //random player speed 
    float timePlayerspeedRandom = 0.8f;
    // for environment Script 
    public GameObject spawnPointEnvironment;//point object spawn(right)
    public GameObject[] environment;
    public GameObject lastPoint;//point object delete (left)
    private float TimeToSpawnEnvironment = 3;
    public bool triggerSpeedBonus;//speed bonus trigger
    //Explorers Assets
    public GameObject explorerBullet;
    public GameObject explorerEnemy;
    // player gameObject 
    public GameObject gameObjectPlayer;
    //thorns back derection 
    private Vector3 thornsBackDerection;
    //reload time damage thorns 
    private float reloadDamageThorns = 0;
    //text life 
    public TextMeshProUGUI playerLifeText;
    [SerializeField] private GameObject deadlyPoint;
    [SerializeField] private GameObject deadlyPointUp;
    // current collision player 
    [HideInInspector] public string CurrentPlayerCollisionName;
    //Ammo limit
    [HideInInspector] public float ammoCount;
    public TextMeshProUGUI ammoCountText;
    //heal
    private int heal = 20;
    private float currentHeal;
    //color change
    private SpriteRenderer playerSpriteRenderer;
    //color changes 
    private float timeColorChange = 0.4f;
    private bool boolColorChange = false;
    private float currentLifeForChangeColor = 100;
    //scripts
    private Gun scrGun;
    //grab button variable 
    public GameObject grabGunButton;
    [SerializeField] GameObject grabGunButtonBackground;
    private float grabButtonTransparency;
    [HideInInspector] public GameObject grabButtonCanvas;
    //guns icons 
    /*[HideInInspector]*/ public GameObject gunIconOne;
    [HideInInspector] public GameObject gunIconTwo;
    [HideInInspector] public GameObject gunIconThree;
    private LevelManager scrLevelManager;

    [HideInInspector] public bool IsReversGravity;

    // time player exit акfrom collision
    public float TimeCollisionExitHold = 0.1f;
    public bool TimeCollisionExitHoldBool = false;
    // Boss Parf variable
    [HideInInspector] public bool IsBossZone;
    //for camera
    [Header("camera")]
    public CinemachineVirtualCamera vCam;
    public GameObject pointCameraLock;
    public GameObject hedgeForBossDragon;
    private bool IspperPlatform = false;
    private float reloadRodAttack = 0.5f;
    private bool reloadRodAttackTrigger = true;
    float BufferAmmoCount;

    


    void Start()
    {
       // hedgeForBossDragon = GameObject.Find("hedge");

        vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>(); 
        //currentLifeForChangeColor = lifePlayer;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ammoCount = 12;
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        scrGun = GameObject.Find("gunSlot").GetComponent<Gun>();
        grabButtonCanvas = GameObject.Find("Canvas button");
        pointSpawnAmmoForGunTwo = GameObject.Find("pointSpawnAmmoForGunTwo");
        pointSpawnAmmoForGunOne = GameObject.Find("pointSpawnAmmoForGunOne");
        pointSpawnAmmoForGunThree = GameObject.Find("pointSpawnAmmoForGunThree");
        gunIconOne = GameObject.Find("icon gun one");
        gunIconTwo = GameObject.Find("icon gun two");
        gunIconThree = GameObject.Find("icon gun three");
        scrLevelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        if (scrLevelManager.currentLevel == 6)
        {
            Debug.Log("if is working");
            hedgeForBossDragon = GameObject.Find("hedge");
            if (hedgeForBossDragon != null)
            {
                hedgeForBossDragon.SetActive(false);
            }
        }
        if (gunIconTwo != null)
        {
            gunIconTwo.SetActive(false);
        }
        if (gunIconThree != null)
        {
            gunIconThree.SetActive(false);
        }
    }
    void PlayerIsDied()
    {
        SceneManager.LoadScene("deadScreen");
    }

    void ChangeGravity()
    {
        if (IsReversGravity)
        {
            rigidbody.AddForce(Vector3.up *  100.5f, ForceMode2D.Force);
        }
    }
    void BossPart()
    {
        
        if (IsBossZone)
        {
            if (!float.IsInfinity(ammoCount))
            {
                BufferAmmoCount = ammoCount;
            }
            ammoCount = float.PositiveInfinity;
            playerjumpforce = 32;
            vCam.m_Lens.OrthographicSize = 15.2f;
            if (pointCameraLock != null)
            {
                vCam.Follow = pointCameraLock.transform;
            }
            hedgeForBossDragon.SetActive(true);
        }
        else
        {
            if (float.IsInfinity(ammoCount))
            {
                ammoCount = BufferAmmoCount;
            }
            playerjumpforce = 25;
            vCam.m_Lens.OrthographicSize = 6.7f;
            vCam.Follow = gameObjectPlayer.transform;
        }
    }
    void ColorChanges()
    {
            playerSpriteRenderer.color = new Color(0.973f, 0.431f, 0.431f);
            timeColorChange -= Time.deltaTime;
            if (timeColorChange <= 0)
            {
                boolColorChange = false;
                timeColorChange = 0.4f;
                playerSpriteRenderer.color = new Color(0.933f, 0.933f, 0.933f);
            }
    }
    void ColorGrabButtonChange()
    {
        if (Vector3.Distance(transform.position, grabGunButton.transform.position) < 5 && Vector3.Distance(transform.position, grabGunButton.transform.position) > 2.5f)
        {
            grabButtonCanvas.SetActive(true);
            grabButtonTransparency = Mathf.InverseLerp(7, 2, Vector3.Distance(transform.position, grabGunButton.transform.position));
            grabGunButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, grabButtonTransparency);
            grabGunButtonBackground.GetComponent<Image>().color = new Color(0.302f, 0.298f, 0.298f, grabButtonTransparency);
        }
        else if (Vector3.Distance(transform.position, grabGunButton.transform.position) > 5)
        {
            grabButtonCanvas.SetActive(false);
        }
        if (Vector3.Distance(transform.position, grabGunButton.transform.position) < 2.5f)
        {
            grabGunButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            grabGunButtonBackground.GetComponent<Image>().GetComponent<Image>().color = new Color(0.302f, 0.298f, 0.298f, 1f);
        }
    }
    void SpawnPointAmmoAndGunChanges()
    {
        switch (scrGun.currentGun)
        {
            case Gun.AllGun.GunOne:
                currentSpawnPointAmmo = pointSpawnAmmoForGunOne;
                CurrentAmmo = AmmoForGunOne;
                break;
            case Gun.AllGun.GunTwo:
                currentSpawnPointAmmo = pointSpawnAmmoForGunTwo;
                CurrentAmmo = AmmoForGunTwo;
                break;
            case Gun.AllGun.GunThree:
                currentSpawnPointAmmo = pointSpawnAmmoForGunThree;
                CurrentAmmo = AmmoForGunThree;
                break;
        }
    }
    void FixedUpdate()
    {
        ChangeGravity();
    }
    void FlipPlayer()
    {
        //flip player 
        if (horizontalInput < 0)
        {
            if (!IsReversGravity)
            {
                gameObject.transform.localScale = new Vector3(-9, 9, 1);//becouse normal scale player 9 9 1
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-9, -9, 1);
            }
            triggerSpeedBonus = false;
            thornsBackDerection = new Vector3(1, 1, 0);
        }
        if (horizontalInput > 0)
        {
            if (!IsReversGravity)
            {
                gameObject.transform.localScale = new Vector3(9, 9, 1);//becouse normal scale player 9 9 1
            }
            else
            {
                gameObject.transform.localScale = new Vector3(9, -9, 1);
            }
            triggerSpeedBonus = true;
            thornsBackDerection = new Vector3(-1, 1, 0);
        }
    }
    // Update is called once per frame
    void Update() 
    {
        if (!reloadRodAttackTrigger)
        {
            if (reloadRodAttack >= 0)
            {
                reloadRodAttack -= Time.deltaTime;
            }
            else
            {
                reloadRodAttack = 0.5f;
                reloadRodAttackTrigger = true;
            }
        }
        FlipPlayer();
        BossPart();
       // AmmoGunChanges();
        SpawnPointAmmoAndGunChanges();
        //color changes player
        if (scrGun.currentfilling < 0.989f && grabButtonCanvas !=null && grabGunButtonBackground != null && grabGunButton != null)
        {
            ColorGrabButtonChange();
        } 
        //color changes player
        if (lifePlayer < currentLifeForChangeColor)
        {
            boolColorChange = true;
            currentLifeForChangeColor = lifePlayer;
        }
        if (boolColorChange)
        {
            ColorChanges();
        }
        //Showing ammo count 
        ammoCountText.text = " : " + ammoCount.ToString();
        //reload time damage thorns 
        if (reloadDamageThorns > 0)
        {
            reloadDamageThorns -= Time.deltaTime;
        }
        //player life text
        playerLifeText.text = lifePlayer.ToString();

        //player is died
        if (lifePlayer <= 0)
        {
            PlayerIsDied();
        }
        //Player is fell
        if (transform.position.y < deadlyPoint.transform.position.y)
        {
            lifePlayer = 0;
        }
        if (deadlyPointUp != null)
        {
            if (transform.position.y > deadlyPointUp.transform.position.y)
                lifePlayer = 0;
        }
       
        //Life Ribbon 
        if (currentLife != lifePlayer)
        {
            //boolColorChange = true;
            currentLife = Mathf.Lerp(currentLife, lifePlayer, Time.deltaTime * speedlifeChange);
        }

        imageLifePlayer.fillAmount = currentLife  / 100;

        //Player Random Speed
        if (timePlayerspeedRandom >= 0)
        {
            timePlayerspeedRandom -= Time.deltaTime;
        }else
        {
            speedPlayer = Random.Range(4.5f, 5);
            timePlayerspeedRandom = 0.8f;
        }
        //speedPlayer = Mathf.Lerp(3,5,(Time.deltaTime * speed));
        //Animation
        move = Input.GetAxisRaw("Horizontal");
        if (move >= 0.01f || move <= -0.01f)
        {
            animator.SetBool("isRuning", true);
        }
        else
        {
            animator.SetBool("isRuning", false);
        }
        //player move
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * speedPlayer * horizontalInput);
        //player jump 
        if (Input.GetKeyDown(KeyCode.Space) && jumpIsAllow && !IspperPlatform && jumpReloadTimeTrigger)
        {
            if (!IsReversGravity)
            {
                rigidbody.AddForce(Vector2.up * playerjumpforce, ForceMode2D.Impulse);
            }
            else
            {
                rigidbody.AddForce(Vector2.up * -playerjumpforce, ForceMode2D.Impulse);
            }
            jumpIsAllow = false;
            jumpReloadTimeTrigger = false;
        }
        if (rigidbody.velocity.y >= 0)
        {
            rigidbody.gravityScale = gravityScale;
        }
        else if (rigidbody.velocity.y < 0)
        {
            rigidbody.gravityScale = fallingGravityScale;
        }
        if (!jumpReloadTimeTrigger)
        {
            jumpReloadTime -= Time.deltaTime;
            if (jumpReloadTime <= 0)
            {
                jumpReloadTime = 0.8f;
                jumpReloadTimeTrigger = true;
            }
        }
        
        //Spawn Ammo
        if (Input.GetKeyDown(KeyCode.E) && triggerReloadShot && ammoCount > 0)
        {
           // scrGun.gunAnimationBool = true;
            ammoCount--;
            Instantiate(CurrentAmmo, currentSpawnPointAmmo.transform.position, gun.transform.rotation);
            triggerReloadShot = false;
        }
        //reload Shot 
        if (triggerReloadShot == false)
        {
            timeToReloadShot -= Time.deltaTime;
            if (timeToReloadShot <= 0)
            {
                triggerReloadShot = true;
                timeToReloadShot = 1.1f;
            }
        }

        //ping void "Spawn environment" in time
        if (TimeToSpawnEnvironment <= 0)
        {
            TimeToSpawnEnvironment = Random.Range(2,5);
            SpawnEnvironment();
        }
        else
        {
            TimeToSpawnEnvironment -= Time.deltaTime;
        }
        //points spawn to follow to player
        if (!jumpIsAllow)
        {
            animator.SetBool("isJump", true);
        }
        //Collsion exit time
        if (TimeCollisionExitHoldBool)
        {
            if (TimeCollisionExitHold >=0)
            {
                TimeCollisionExitHold -= Time.deltaTime;
            }
        }
    }
    //Spawn environment
    void SpawnEnvironment()
    {
        int indexEnvironment;
        indexEnvironment = Random.Range(0, environment.Length);
        Instantiate(environment[indexEnvironment], spawnPointEnvironment.transform);
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "bateryHeal")
        {
            if (lifePlayer <= 80)
            {
                lifePlayer += heal;
            }
            else
            {
                currentHeal = maxPlayerLife - lifePlayer;
                lifePlayer += currentHeal;
            }
            Destroy(coll.gameObject);
        }
        if (coll.gameObject.tag == "ground" || coll.gameObject.tag == "Jump Platform" || coll.gameObject.tag == "thorns" || coll.gameObject.tag == "UpperPlatformForBoss")
        {
            CurrentPlayerCollisionName = coll.gameObject.name;
            TimeCollisionExitHoldBool = true;
            if (!jumpIsAllow) {
                animator.SetBool("isJump", false);
                jumpIsAllow = true;
            }
            if (coll.gameObject.tag == "UpperPlatformForBoss") {
                IspperPlatform = true;
            }

        }
        if (coll.gameObject.tag == "Jump Platform")
        {
            jumpIsAllow = false;
            rigidbody.AddForce(Vector3.zero * 29, ForceMode2D.Impulse);
            rigidbody.AddForce(Vector3.up * 29, ForceMode2D.Impulse);
        }
        if (coll.gameObject.tag == "portal between level ")
        {
            if (scrLevelManager.recordlevel <= scrLevelManager.currentLevel)
            {
                scrLevelManager.recordlevel = scrLevelManager.currentLevel;
                PlayerPrefs.SetInt("recordLevel", scrLevelManager.recordlevel);
            }
            if (scrLevelManager.currentLevel == 6)
            {
                SceneManager.LoadScene("theEndScene");
            }
            else
            {
                SceneManager.LoadScene("levelChange");
            }
        }
        if (coll.gameObject.tag == "thorns")
        {
            if(reloadDamageThorns <= 0)
            {
                lifePlayer -= 15;
                reloadDamageThorns = 2;
            }
                 rigidbody.AddForce(thornsBackDerection * 7, ForceMode2D.Impulse);
                jumpIsAllow = false;
        }

    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "ground" || coll.gameObject.tag == "Jump Platform" || coll.gameObject.tag == "thorns" || coll.gameObject.tag == "UpperPlatformForBoss")
        {
            if(TimeCollisionExitHold <= 0)
            {
                CurrentPlayerCollisionName = null;
                TimeCollisionExitHold = 0.1f;
            }
            TimeCollisionExitHoldBool = false;
            if (coll.gameObject.tag == "UpperPlatformForBoss")
            {
                IspperPlatform = false;
            }
        }

       
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "minus gravity zone")
        {
            IsReversGravity = false;
        }
        if (collider.gameObject.tag == "BossFightZone")
        {
             IsBossZone = false;
        }
       
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "minus gravity zone")
        {
            IsReversGravity = true;
        }
        if (collider.gameObject.tag == "BossFightZone")
        {
            IsBossZone = true;
        }
        if (collider.gameObject.tag == "FireAmmoForDragon" && !collider.gameObject.GetComponent<FireAmmoForDragon>().isHit)
        {
            collider.gameObject.GetComponent<FireAmmoForDragon>().isHit = true;
            lifePlayer -= 20;
        }
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "FireRodForDragon" && reloadRodAttackTrigger)
        {
            reloadRodAttackTrigger = false;
            lifePlayer -= 7;
        }
    }
}
