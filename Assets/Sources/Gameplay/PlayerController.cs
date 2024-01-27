using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2024
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("La physique")]
        public float speed;
        public float acceleration;
        public float dashDuration;
        public AnimationCurve dash;

        private Vector2 targetVelocity;
        private Vector2 currentVelocity;
        private Rigidbody2D m_Rigidbody;
        private PlayerInput m_PlayerInput;
        private float dashTime = 0;

        [Header("Animation")]
        private Sprite chargeSprite;
        private Sprite dashSprite;
        private Sprite stopSprite;
        public SpriteRenderer m_SpriteRenderer;
        public float startedTime;
        private float started = 0;

        private Vector2 m_MovementInput;

        public bool IsInking { get; private set; }

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.addPlayer(this);
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_PlayerInput = GetComponent<PlayerInput>();
            SetupInputs();
        }

        private void OnEnable()
        {
            InkManager.Instance.RegisterPlayer(this);
        }

        private void OnDisable()
        {
            InkManager.Instance?.UnregisterPlayer(this);
        }

        private void SetupInputs()
        {
            m_PlayerInput.currentActionMap.FindAction("Move").performed += OnMove;
            m_PlayerInput.currentActionMap.FindAction("Move").canceled += OnMove;
            m_PlayerInput.currentActionMap.FindAction("Ink").started += OnInk;
            m_PlayerInput.currentActionMap.FindAction("Ink").canceled += OnInk;
        }

        private void OnInk(InputAction.CallbackContext context)
        {
            IsInking = !context.canceled;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            m_MovementInput = context.ReadValue<Vector2>();
        }

        // Update is called once per frame
        void Update()
        {
            // Velocity
            targetVelocity = m_MovementInput * speed;
            float angle = Mathf.Atan2(m_MovementInput.y, m_MovementInput.x) - Mathf.PI / 2;

            if (m_MovementInput != Vector2.zero)
            {
                transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            } 
                

            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, Time.deltaTime * acceleration) * dash.Evaluate(dashTime);
            if (m_MovementInput != Vector2.zero && started < startedTime)
            {
                started += Time.deltaTime;
                m_SpriteRenderer.sprite = chargeSprite;
            }
            else if (m_MovementInput != Vector2.zero)
            {
                m_SpriteRenderer.sprite = dashSprite;
            }
            else
            {
                m_SpriteRenderer.sprite = stopSprite;
            }
            dashTime += Time.deltaTime;
            if (dashTime > dashDuration)
            {
                dashTime = 0;
                started = 0;
            }

            //if (buffspeed) currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity * buffspeedmultiplier, Time.deltaTime * acceleration);
            m_Rigidbody.velocity = currentVelocity;

        }

        public void SetSprites(Sprite chargeSprite, Sprite dashSprite, Sprite stopSprite)
        {
            this.chargeSprite = chargeSprite;
            this.dashSprite = dashSprite;
            this.stopSprite = stopSprite;
        }
    }

}
