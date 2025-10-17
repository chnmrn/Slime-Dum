using Pathfinding;
using UnityEngine;
using Path = Pathfinding.Path;

public class EnemyAI : MonoBehaviour
{

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private FireController fireController;

    private int currentWaypoint = 0;

    private Vector3 originalPosition;

    private Vector2 destPoint;
    private bool walkPointSet;
    private float approximateValue;


    [Header("Patrol")]

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField, Range(1,6)] private float walkRangePath;

    //"Range Patrol shouldn't be above 5";

    [SerializeField] private float rangePatrol;
    [SerializeField] private bool returnEnabled;

    [Space]

    [Header("Pathfinding")]
    private Transform target;
    [SerializeField] private float activateDistance = 50f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float nextWaypointDistance = 3f;

    [SerializeField] private Vector3 startOffset;
    //[SerializeField] private RaycastHit2D isGrounded;

    [Space]

    [Header("Custom Behavior")]
    [SerializeField] private bool directionLookEnabled = true;


    private float _fireTimer;
    private float timer;



    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        fireController = GetComponent<FireController>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        approximateValue = CalculateApproximateValue();
        InvokeRepeating(nameof(UpdateTargetPath), 0f, pathUpdateSeconds);
        
    }

    private void FixedUpdate()
    {
        if (GetCurrentState() == "Player")
        {
            timer += Time.deltaTime;
            PathFollowPlayerAI();
            if (timer > 0.5)
            {
                timer = 0;
                FireOnAttack();
            }
        }
        else if (GetCurrentState() == "Patrol")
        {
            PatrolEnemyAI();
        }
    }


    private void PatrolEnemyAI()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        // Calculate the distance between startOffset and the original position of the object
        float _distanceToOriginal = Vector2.Distance((Vector2)startOffset, (Vector2)originalPosition);
        if (_distanceToOriginal > rangePatrol || _distanceToOriginal < -rangePatrol)
        {
            returnEnabled = true;
        }
        if (Mathf.Abs(_distanceToOriginal - 1) < 0.1f)
        {
            returnEnabled = false;
        }
        if (returnEnabled)
        {
            MovingEnemy();
            return;
        }
        // Search Destination
        if (!walkPointSet) SearchForDest();
        if (Vector2.Distance(transform.position, destPoint) <10 )walkPointSet=false;  
        
    }
    
    private void SearchForDest()
    {
        float x = Random.Range(-walkRangePath, walkRangePath);
        float y = Random.Range(-walkRangePath, walkRangePath);

        destPoint = new Vector2(transform.position.x + x, transform.position.y + y);

        if (Physics2D.Raycast(destPoint, Vector2.down, _groundLayer))
        {
            walkPointSet=true;

            MovingEnemy();
        }
    }

    private void FireOnAttack()
    {
        if (fireController != null)
        {
            _fireTimer -= Time.deltaTime;
            fireController.Fire();
            _fireTimer = fireController.fireRate;
        }
        
    }

    private void MovingEnemy()
    {
        startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y, transform.position.z);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * (speed * 1000);

        // Movement
        rb.AddForce(force);

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {

            currentWaypoint++;
        }

        DirectionalLook();
    }

    private Vector2 ObstacleEnemyReturn()
    {

         return (Vector2)originalPosition + new Vector2(approximateValue, approximateValue);

    }

    private void UpdateTargetPath()
    {
        if (seeker.IsDone() != true) return;

        if (!TargetInDistance())
        {
            if (returnEnabled)
            {
                Vector2 targetPosition = ObstacleEnemyReturn() + new Vector2(Random.Range(-3, 4), Random.Range(-3, 4));
                seeker.StartPath(rb.position, targetPosition, OnPathComplete);
                returnEnabled =false;
            }
            else if (!returnEnabled && !walkPointSet)
            {
                seeker.StartPath(rb.position, destPoint, OnPathComplete);
            }
        }
        else
        {
            seeker.StartPath(rb.position, (Vector2)target.position, OnPathComplete);
        }




    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void PathFollowPlayerAI()
    {

        if (path == null || currentWaypoint >= path.vectorPath.Count) return;
        
        MovingEnemy();
    }

    private void DirectionalLook()
    {   // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }
        }
    }

    private bool TargetInDistance()
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }


    // Method to obtain the current state of the enemy
    public string GetCurrentState()
    {
        if (TargetInDistance())
        {
            return "Player";
        }
        else
        {
            return "Patrol";
        }
    }

    private float CalculateApproximateValue()
    {
        // Get the absolute value of the x position
        float absoluteX = Mathf.Abs(transform.position.x);

        // Calculate the half value between x position and 0
        float halfX = absoluteX / 2;

        // Get a random value within the range of 0 to half of the absolute value of x
        float randomOffset = Random.Range(0, halfX);

        // Round the half value of x minus the random offset
        float roundedValue = Mathf.Round(halfX - randomOffset);

        // Return half of the rounded value
        return roundedValue / 2;
    }

    private void OnValidate()
    {
        // Check if rangePatrol is less than 1 and adjust its value if so
        if (rangePatrol < 1)
        {
            Debug.LogWarning("The value of rangePatrol cannot be less than 1. It will be automatically adjusted.");
            // Set rangePatrol to 1
            rangePatrol = 1;
        }
        // Check if walkRange is outside the range 1 to 6 and adjust its value if so
        if (walkRangePath < 1 || walkRangePath > 6)
        {
            // Display a warning message in the console
            Debug.LogWarning("The value of walkRange must be between 1 and 6. It will be automatically adjusted.");
            // Clamp walkRange between 1 and 6
            walkRangePath = Mathf.Clamp(walkRangePath, 1, 6);
        }
    }
}


