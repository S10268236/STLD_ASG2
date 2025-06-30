using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.ProBuilder.Shapes;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Interactions of player with items and environment
*/

public class PlayerBehaviour : MonoBehaviour
{
    //Variables & Fields

    /// <Mutagen summary>
    /// Score points & amount required to pass level, collectible and heals
    /// </summary>
    //Mutagen Variables
    int mutagenScore = 0;
    private int totalMutagens = 10;
    int mutagensLeft;
    //Allow setting of heal amount for each mutagen
    [SerializeField]
    float mutagenHealAmt = 50f;
    //Player Health Variables
    public float maxPlayerHealth = 100f;
    float currentPlayerHealth;
    //Interact check
    bool canInteract = false;
    //Use to track time for damage
    private float damageTimer = 0f;
    //Breath Variables
    private float maxBreath = 5f;
    private float currentBreath;
    //Track whether gun is obtained
    bool gotGun = false;
    //Set a variable for Door, Mutagen, Gun, Exit, Oxygen and set it to null for future storage of Raycast collider
    DoorBehaviour currentDoor = null;
    MutagenBehaviour currentMutagen = null;
    GunBehaviour currentGun = null;
    ExitBehaviour currentExit = null;
    OxygenBehaviour currentOxygen = null;
    
    //Onscreen overlay for taking damage
    public Image HealthImpact;
    //Update player Health on UI
    [SerializeField]
    TextMeshProUGUI playerHealthText;

    //Update Mutagen obtained on UI
    [SerializeField]
    TextMeshProUGUI mutagenAmtText;
    //Aim reticle image input field
    [SerializeField]
    Image reticle;
    //Input field for Interact Message for all interactables
    [SerializeField]
    TextMeshProUGUI InteractMessage;
    //Input field for screen on death
    [SerializeField]
    GameObject DeathScreen;
    //Input field for text shown on death
    [SerializeField]
    TextMeshProUGUI DeathMessage;
    //Input field for screen on win
    [SerializeField]
    GameObject WinScreen;
    //Input field for text shown on win
    [SerializeField]
    TextMeshProUGUI WinMessage;
    //Input field for intro panel
    [SerializeField]
    GameObject IntroPanel;
    //Input field for intro message
    [SerializeField]
    TextMeshProUGUI IntroText;
    //Input field for respawn point
    [SerializeField]
    Transform respawnPoint;
    //Set how far your able to intereact
    [SerializeField]
    float interactionDistance = 5f;
    //Input field for acid DPS
    [SerializeField]
    float acidDPS = 40f;
    //Input field for smoke DPS
    [SerializeField]
    float smokeDPS = 10f;
    //Input field for enemy DPS
    [SerializeField]
    float enemyDPS = 10f;
    //Which gameobject to instantiate when firing
    [SerializeField]
    GameObject projectile;
    //Input field for where to spawn-Projectile
    [SerializeField]
    Transform spawnPoint;
    //Input field for Gun spawn
    [SerializeField]
    Transform GunLocation;
    //What to show when obtaining gun
    [SerializeField]
    GameObject GunModel;
    //Allow for gun to follow player
    [SerializeField]
    GameObject GunParentFollow;
    //Forward force for projectiles
    [SerializeField]
    float fireStrength = 200f;
    //Audio Variables
    public AudioSource ShootAudio;
    public AudioSource Coughing;
    public AudioSource AcidBubbling;
    //Gun Fire rate Variables
    private bool gunFired = false;
    void Start()
    {
        //Set Player health and breath to maximum values
        currentPlayerHealth = maxPlayerHealth;
        currentBreath = maxBreath;
        //Add Health and Mutagens collected to UI
        playerHealthText.text = "Health: " + currentPlayerHealth.ToString();
        mutagenAmtText.text = "Mutagens: " + mutagenScore.ToString();
        //Show intro message
        StartCoroutine(Introduction());
    }
    /// <Introduction summary>
    /// Shows Intro, disappears automatically
    /// </summary>
    /// <returns></returns>
    IEnumerator Introduction()
    {
        IntroText.text = "You awaken in a smoke-filled Surgery Room, with no recollection of how you got there.\n\n The last thing you remember is entering the Interview room for a job at the Parasol Corporation.\n\n\n Escape ";
        IntroPanel.SetActive(true);
        yield return new WaitForSeconds(8);
        IntroPanel.SetActive(false);
    }
    void Update()
    {
        //Show damage screen when damaged
        DamageScreen();
        //Raycasting for interactables
        RaycastHit hitInfo;
        //Show Raycast ray for debugging
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.red);
        //Raycast = true when hitting something
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
        {
            //If collectible is within interaction range
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                InteractMessage.text = "[E] Mutagen \n+" + mutagenHealAmt + "HP";
                //Set currentMutagen to the one in front of player and allow use of Interact
                currentMutagen = hitInfo.collider.gameObject.GetComponent<MutagenBehaviour>();
                canInteract = true;
            }
            //If door is within interaction range
            else if (hitInfo.collider.gameObject.CompareTag("Door"))
            {

                InteractMessage.text = "[E] Interact";
                //Set currentDoor to the one in front of player and allow use of Interact
                currentDoor = hitInfo.collider.gameObject.GetComponent<DoorBehaviour>();
                canInteract = true;
            }
            //if its gun
            else if (hitInfo.collider.gameObject.CompareTag("Gun"))
            {

                InteractMessage.text = "[E] Pickup Gun";
                currentGun = hitInfo.collider.gameObject.GetComponent<GunBehaviour>();
                canInteract = true;
            }
            //if its the exit
            else if (hitInfo.collider.gameObject.CompareTag("Win"))
            {

                InteractMessage.text = "[E] Exit";
                currentExit = hitInfo.collider.gameObject.GetComponent<ExitBehaviour>();
                canInteract = true;
            }
            //Reset variables to default if it hits non interactables
            else if (hitInfo.collider.gameObject.CompareTag("Untagged"))
            {
                ResetRaycast();
            }
        }
        //Reset all variables to default state when Raycast = false
        else 
        {
            ResetRaycast();
        }
    }
    /// <ResetRaycast summary>
    /// Reset all the stored variables when raycast hits untagged and when raycast is false
    /// </summary>
    void ResetRaycast()
    {
        currentMutagen = null;
        currentDoor = null;
        currentGun = null;
        canInteract = false;
        InteractMessage.text = null;

    }
    /// <Damage Screen summary>
    /// Show on screen damage when hp is reduced
    /// </summary>
    void DamageScreen()
    {
        //As damage is taken, transparency increases
        float transparency = 1f - (currentPlayerHealth / 100f);
        Color imageColor = Color.white;
        //Set alpha of imageColor to be transparency variable
        imageColor.a = transparency;
        HealthImpact.color = imageColor;

    }
    /// <Damage taken summary>
    /// Function to reduce HP and reset timer to control rate of damage taken
    /// </summary>
    /// <param name="dps"></param>
    private void DamageTaken(float dps)
    {
        currentPlayerHealth -= dps;
        //Updating the UI
        playerHealthText.text = "Health: " + currentPlayerHealth.ToString();
        //Reset damagetimer to not take more damage
        damageTimer = 0f;
        //Death Trigger
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
            StartCoroutine(Respawn());
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //Melee Enemy attack
        if (collision.gameObject.CompareTag("Enemy") && currentPlayerHealth > 0)
        {
            DamageTaken(enemyDPS);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        //when contact with enemy and HP not 0, count time to adjust damage tick
        if (collision.collider.gameObject.CompareTag("Enemy") && currentPlayerHealth > 0)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 2f)
            {
                DamageTaken(enemyDPS);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        //Reset damage timer
        damageTimer = 0f;
    }
    void OnTriggerEnter(Collider other)
    {
        //Obtaining O2 resets breath
        if (other.gameObject.CompareTag("Oxygen"))
        {
            currentBreath = maxBreath;
            currentOxygen = other.gameObject.GetComponent<OxygenBehaviour>();
            currentOxygen.Collect(this);
        }
        //Play smoke audio when entering it
        if (other.gameObject.CompareTag("Smoke") && currentPlayerHealth > 0)
        {
            Coughing.Play();
        }
        //Play acid audio when entering it
        if (other.gameObject.CompareTag("Acid") && currentPlayerHealth > 0)
        {
            AcidBubbling.Play();
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Acid Damage Over Time
        if (other.gameObject.CompareTag("Acid") && currentPlayerHealth > 0)
        {
            //Track time
            damageTimer += Time.deltaTime;
            if (damageTimer >= 2f)
            {
                DamageTaken(acidDPS);
            }
        }
        // Smoke-buffer time of held breath, then Damage Over Time
        else if (other.gameObject.CompareTag("Smoke") && currentPlayerHealth > 0)
        {
            //Reduce maxBreath time to signify breath running out
            currentBreath -= Time.deltaTime;

            //Once out, start taking damage per second
            if (currentBreath <= 0)
            {
                //use damageTimer to time seconds per tick damage
                damageTimer += Time.deltaTime;
                if (damageTimer >= 2f)
                {
                    DamageTaken(smokeDPS);
                }
            }
        }
    }
    /// <Respawn summary>
    /// Teleports player back to start, Shows death screen for a while, resets player HP and reflects on UI
    /// </summary>
    /// <returns></returns>
    IEnumerator Respawn()
    {
        //Activate the death screen
        DeathScreen.SetActive(true);
        DeathMessage.text = "You Are Dead";
        //Reset HP
        currentPlayerHealth = maxPlayerHealth;
        //Teleport back to start
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        //Wait 2 seconds or frame closes to 2 seconds
        yield return new WaitForSeconds(2);
        //deactivate death screen
        DeathScreen.SetActive(false);
        DeathMessage.text = null;
        //reflect on UI
        playerHealthText.text = "Health: " + currentPlayerHealth.ToString();
    }

    void OnTriggerExit(Collider other)
    {
        //Stop the audios for smoke and acid when leaving trigger zones
        Coughing.Pause();
        AcidBubbling.Pause();
        //Debug.Log("Leaving trigger:" + other.gameObject.name);
        //Resets damage timer if player leaves hazard zone
        damageTimer = 0f;
        //Reset Breath after leaving smoke zone
        currentBreath = maxBreath;
        //Closes door when leaving trigger zone
        if (other.gameObject.CompareTag("Door"))
        {
            currentDoor = other.gameObject.GetComponent<DoorBehaviour>();
            if (currentDoor.Closed != true)
            {
                Debug.Log("AutoClose");
                currentDoor.Interact();
            }

        }
        currentDoor = null;
    }

    //What to do when interact is pressed
    public void OnInteract()
    {
        //Check if able to interact
        if (canInteract)
        {
            //Check if Door or Collectible
            if (currentDoor != null)
            {
                currentDoor.Interact();
            }
            else if (currentMutagen != null)
            {
                currentMutagen.Collect(this);
                ModifyHealth(50f);
            }
            //Check if gun
            else if (currentGun != null)
            {
                currentGun.Collect(this);
                gotGun = true;
                GameObject showGun = Instantiate(GunModel, GunLocation.position, GunLocation.rotation);
                Transform showGunTransform = showGun.transform;
                showGunTransform.SetParent(GunParentFollow.transform);
            }
            //Check if exit
            else if (currentExit != null)
            {
                if (mutagenScore >= 10)
                {
                    //Win Screen
                    StartCoroutine(WinLoadNew());
                }
                else
                {
                    //Requirements screen
                    StartCoroutine(NeedMore());
                }
            }
        }
    }
    /// < WinLoadNew summary>
    /// Show Win screen,Teleport player back to start, Show restart message and Restart scene
    /// </summary>
    /// <returns></returns>
    IEnumerator WinLoadNew()
    {
        //Show win screen
        WinScreen.SetActive(true);
        DeathScreen.SetActive(true);
        //Teleport player
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        WinMessage.text = "Congratulations!\n\n You get this hideous Win Screen!";
        yield return new WaitForSeconds(3);
        //Show restart message
        WinMessage.text = "Game is Restarting";
        yield return new WaitForSeconds(3);
        WinScreen.SetActive(false);
        DeathScreen.SetActive(false);
        //Restart Scene
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    /// <NeedMore summary>
    /// When mutagen insufficient, request for more, with different messages depending on amount
    /// </summary>
    /// <returns></returns>
    IEnumerator NeedMore()
    {
        if (mutagensLeft > 1)
        {
            WinScreen.SetActive(true);
            WinMessage.text = "You're not strong enough, find " + mutagensLeft + " more Mutagens";
            yield return new WaitForSeconds(3);
            WinScreen.SetActive(false);
            WinMessage.text = null;
        }
        else
        {
            WinScreen.SetActive(true);
            WinMessage.text = "You're not strong enough, find the Last Mutagen";
            yield return new WaitForSeconds(3);
            WinScreen.SetActive(false);
            WinMessage.text = null;
        }

    }
    //Add to mutagen score when collected
    public void ModifyMutagenScore(int amt)
    {
        mutagenScore += amt;
        mutagensLeft = totalMutagens - mutagenScore;
        //Update new mutagen amount to UI
        mutagenAmtText.text = "Mutagens: " + mutagenScore.ToString();
    }
    //Mutagens heal by amount specified in function, sets health to max health if it goes over
    void ModifyHealth(float mutagenHealAmt)
    {
        //Heal goes over maxHP
        if (currentPlayerHealth + mutagenHealAmt > maxPlayerHealth)
        {
            //Prevent overhealing
            currentPlayerHealth = maxPlayerHealth;
        }
        else
        {
            currentPlayerHealth += mutagenHealAmt;
        }
        //Update on UI
        playerHealthText.text = "Health: " + currentPlayerHealth.ToString();
    }
    /// <OnFire summary>
    /// Fire a projectile with forward motion
    /// </summary>
    public void OnFire()
    {
        if (gotGun)
        {
            if (gunFired != true)
            {
                //Instantiate a new projectile at spawn point
                //Store projectile to newProjectile variable
                GameObject newProjectile = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
                Vector3 fireForce = spawnPoint.forward * fireStrength;
                newProjectile.GetComponent<Rigidbody>().AddForce(fireForce);
                ShootAudio.Play();
                StartCoroutine(FireRate());
            }

        }
    }
    /// <FireRate summary>
    /// Sets a fire rate of 1s for the gun
    /// </summary>
    /// <returns></returns>
    IEnumerator FireRate()
    {
        gunFired = true;
        yield return new WaitForSeconds(1);
        gunFired = false;
    }
}
