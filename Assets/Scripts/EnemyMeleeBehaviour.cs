using UnityEngine;
using UnityEngine.AI;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Enemy patrol, chase, damage and audio
*/

public class EnemyBehaviour : MonoBehaviour
{
    //Player position
    public Transform Player;
    //Layers for ground recognition and Player recognition
    public LayerMask whatIsGround, whatIsPlayer;
    //Patrolling Variables
    //Walkpoint cooridinates
    public Vector3 walkPoint;
    //Check whether walk point is set
    bool walkPointSet;
    //Allow setting of range of walk point
    public float walkPointRange;
    //Allow easy setting of enemy health
    public float EnemyHealth;
    public float MaxEnemyHealth;
    //HealthBar input field
    [SerializeField]
    FloatingHealthBar healthBar;
    //Time between walks
    private float walkTime = 0f;
    //States
    //Setting of range of enemy sight
    public float sightRange;
    //Checks whether player within sightRange
    public bool playerInSight;
    //Navigation agent
    public NavMeshAgent agent;
    //Loot object input field
    [SerializeField]
    GameObject MutagenLoot;
    //Loot spawn location field
    [SerializeField]
    Transform LootSpawn;
    //Check for whether loot is spawned-prevent multiple instances
    private bool isLooted = false;
    //Attack audio
    AudioSource AttackAudioSource;
    //Check whether audio has been played, default is not played
    private bool SoundPlayed = false;
    //Initialise before Start
    private void Awake()
    {
        Player = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(EnemyHealth, MaxEnemyHealth);
    }
    void Start()
    {
        //Set Enemy health to Maximum
        EnemyHealth = MaxEnemyHealth;
        //Variable for audio
        AttackAudioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        //Check if player in sight
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (!playerInSight)
        {
            Patrolling();
        }
        else ChasePlayer();
    }
    /// <Patrolling summary>
    /// Enemy behaviour when player not in sightRange
    /// </summary>
    private void Patrolling()
    {
        //Search for walkpoint if not set
        if (!walkPointSet)
            SearchWalkPoint();
        if (walkPointSet)
        {
            //Move to walkpoint
            agent.SetDestination(walkPoint);
            //track time spent moving to walkpoint
            walkTime += Time.deltaTime;
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint Reached
        //In cases where walkpoint is reached or unreached after 5s,new walkpoint set. Prevent getting stuck
        if (distanceToWalkPoint.magnitude < 1f || walkTime >= 5f)
        {
            walkPointSet = false;
            walkTime = 0f;
        }
    }
    /// <SearchWalkPoint summary>
    /// Set random walkpoint to go to
    /// </summary>
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        walkPointSet = true;
    }
    /// <ChasePlayer summary>
    /// Set navmesh destination to player transform-Chases the player
    /// </summary>
    private void ChasePlayer()
    {
        Debug.Log("Sound played" + SoundPlayed);
        PlaySound();
        transform.LookAt(Player);
        agent.SetDestination(Player.position);

    }
    /// <TakeDamage summary>
    /// minus HP when this function is called and update on Healthbar,Calls Loot function
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        EnemyHealth -= damage;
        Debug.Log("Enemy:" + EnemyHealth);
        Debug.Log("Max: " + MaxEnemyHealth);
        healthBar.UpdateHealthBar(EnemyHealth, MaxEnemyHealth);
        if (EnemyHealth <= 0 && !isLooted)
        {
            isLooted = true;
            Invoke(nameof(Loot), 0.5f);
        }
    }
    /// <Loot summary>
    /// Creates collectible item when dead
    /// </summary>
    private void Loot()
    {
        Destroy(gameObject);
        GameObject newMutagenLoot = Instantiate(MutagenLoot, LootSpawn.position, LootSpawn.rotation);

    }
    /// <summary>
    /// Debugging to see sight range 
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    /// <PlaySound summary>
    /// Play audio only once to not overlap
    /// </summary>
    private void PlaySound()
    {
        if (SoundPlayed != true)
        {
            AttackAudioSource.Play();
            SoundPlayed = true;
        }

    }
}
