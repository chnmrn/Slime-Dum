using System;
using System.Collections;
using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private FireController fireController;    

        [Header("Camera World")]
        [SerializeField] private new Camera camera;

        [Header("Walking")] 
        [SerializeField] private float maxWalkCos = 0.5f;
        [SerializeField] private float walkSpeed = 7;
    
        [Header("Getting a Hit")]
        [SerializeField] private Vector2 bounceBackStrength = new Vector2(8, 12);
        [SerializeField] private FixedStopwatch hitStopwatch;

        [Header("Attacking")]
        [SerializeField] private FixedStopwatch attackStopwatch;

        [Header("Giving a dash")] 
        [SerializeField] private float dashSpeed = 12;
        [SerializeField] private FixedStopwatch dashStopwatch;

        

        public PlayerState State { get; private set; } = PlayerState.Movement;
        public Vector2 DesiredDirection { get; private set; }
        public int FacingDirection { get; private set; } = -1;

        public Vector2 Velocity => _rigidbody2D.velocity;
        public float DashCompletion => dashStopwatch.Completion;
        public float AttackCompletion => attackStopwatch.Completion;


        private Rigidbody2D _rigidbody2D;
        private ContactFilter2D _contactFilter;
        private ContactPoint2D? _wallContact;
        private readonly ContactPoint2D[] _contacts = new ContactPoint2D[16];
        private Transform _transform;

        private Vector2 _MousePos;




        private bool _canDash;
        private int _enemyLayer;
        private bool _canAttack;

        private float _fireTimer;
        private void Start()
        {
            fireController = GetComponent<FireController>();
            _transform = this.transform;
        }

        private void Awake()
        {
          
           
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _contactFilter = new ContactFilter2D();
            _enemyLayer = LayerMask.NameToLayer("Enemy");
            _contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        }
        
        #region Events

        public void OnMove(InputValue value)
        {
            DesiredDirection = value.Get<Vector2>();
        }

        public void OnDash(InputValue value)
        {
            // Verificar si la dirección deseada del jugador no es igual a cero en ningun componente
            if (DesiredDirection != Vector2.zero)
                EnterDashState();
        }

        public void OnMousePos(InputValue value)
        {
            _MousePos = camera.ScreenToWorldPoint(value.Get<Vector2>());
        }

        public void OnAttack(InputValue value)
        {
            EnterAttackState();
        }

        #endregion

        private void FindContacts()
        {
            _wallContact = null;

            // Modificar para considerar la dirección "y" del movimiento del jugador
            float wallProjection = maxWalkCos;

            // Modificar para que el suelo no tenga un collider específico
            int numberOfContacts = _rigidbody2D.GetContacts(_contactFilter, _contacts);

            for (var i = 0; i < numberOfContacts; i++)
            {
                var contact = _contacts[i];
                float projection = Vector2.Dot(Vector2.up, contact.normal);

                if (projection <= wallProjection)
                {
                    _wallContact = contact;
                    wallProjection = projection;
                }
            }
        }

        private void Update()
        {
            if (State == PlayerState.Attack)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    FireOnAttack();
                    EnterMovementState();
                }
            }
        }

        private void FixedUpdate()
        {
            FindContacts();
            switch (State)
            {
                case PlayerState.Movement:
                    UpdateMovementState();
                    break;
                case PlayerState.Dash:
                    UpdateDashState();
                    break;
                case PlayerState.Attack:
                    UpdateAttackState();
                    break;
            }
            
        }

        #region States

        public void MouseDirectionInWorld()
        {
            Vector2 direction = new Vector3(_MousePos.x, _MousePos.y, 0) - _transform.position;
            float angle = (Mathf.Atan2(direction.x, direction.y)*Mathf.Rad2Deg);
            int normalizedDirection = (angle > 0) ? 1 : -1;
            FacingDirection = normalizedDirection;

        }

        

        private void FireOnAttack()
        {
            _fireTimer -= Time.deltaTime;
            fireController.Fire();
            _fireTimer = fireController.fireRate;
        }

        private void EnterAttackState()
        {
            if (attackStopwatch.IsReady ||
                !_canAttack)
            {
                State = PlayerState.Attack;
                attackStopwatch.Split();
                _canAttack = false;
            }
        }
        private void UpdateAttackState()
        {
            UpdateMovementState();
            EnterAttackState();
            if (attackStopwatch.IsFinished || _wallContact.HasValue)
            {
                attackStopwatch.Split();
                EnterMovementState();
            }
        }

        private void EnterDashState()
        {
            if (State != PlayerState.Movement || 
                State != PlayerState.Attack || 
                !dashStopwatch.IsReady || 
                !_canDash)
            {
                State = PlayerState.Dash;
                dashStopwatch.Split();
                _canDash = false;
            }
        }


        private void UpdateDashState()
        {

            Vector2 dashMovementImpulse = (DesiredDirection.normalized * dashSpeed) - _rigidbody2D.velocity;
            _rigidbody2D.AddForce(dashMovementImpulse, ForceMode2D.Impulse);
            
            
            if (dashStopwatch.IsFinished || _wallContact.HasValue)
            {
                dashStopwatch.Split();
                EnterMovementState();
            }
        }

        private void EnterMovementState()
        {
            State = PlayerState.Movement;
        }


        private void UpdateMovementState()
        {
            var previousVelocity = _rigidbody2D.velocity;
            var velocityChange = Vector2.zero;

            Vector2 moveDirection = DesiredDirection.normalized;

            velocityChange.x = (DesiredDirection.x * walkSpeed - previousVelocity.x) / 4;
            velocityChange.y = (DesiredDirection.y * walkSpeed - previousVelocity.y) / 4;

            MouseDirectionInWorld();

            if (_wallContact.HasValue)
            {
                Vector2 wallDirection = (_wallContact.Value.point - (Vector2)transform.position).normalized;

                if (Vector2.Dot(moveDirection, wallDirection) > 0.9f)
                    velocityChange = Vector2.zero;

            }

            _rigidbody2D.AddForce(velocityChange, ForceMode2D.Impulse);
        }

        #endregion


    }
}