using System.Collections.Generic;
using Unity.MultiPlayerGame.Shared;
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
        public int skin = 0;

        private Vector2 m_MovementInput;

        public bool IsInking { get; private set; }
        public bool IsErasing { get; private set; }
        public int ID => m_PlayerInput.playerIndex;

        private GameManager gameManager;

        [Header("Sons")]
        public float minRandom;
        public float maxRandom;
        public float collisionTimout = 2;
        private float collisionTimoutTime = 0;
        private float currentRandom = 0;
        private float currentRandomTime = 0;

        public AudioClip[] biblical;
        public AudioClip[] collision;
        public AudioClip[] random;
        public AudioClip[] start;
        private AudioSource m_AudioSource;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            SetupInputs();
            ResetRandomSound();
            PlaySound(start);
        }

        private void ResetRandomSound()
        {
            currentRandom = Random.Range(minRandom, maxRandom);
            currentRandomTime = 0;
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
            m_PlayerInput.currentActionMap.FindAction("Erase").started += OnErase;
            m_PlayerInput.currentActionMap.FindAction("Erase").canceled += OnErase;
        }

        public void SetInputEnabled(bool enabled)
        {
            if (enabled)
            {
                m_PlayerInput.currentActionMap.Enable();
            }
            else
            {
                m_PlayerInput.currentActionMap.Disable();
            }

        }

        private void OnErase(InputAction.CallbackContext context)
        {
            IsErasing = !context.canceled;
        }

        private void OnInk(InputAction.CallbackContext context)
        {
            IsInking = !context.canceled;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            m_MovementInput = context.ReadValue<Vector2>().normalized;
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

            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity * dash.Evaluate(dashTime), Time.deltaTime * acceleration);
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

            // Sound
            currentRandomTime += Time.deltaTime;
            if(currentRandomTime > currentRandom)
            {
                PlaySound(random);
                ResetRandomSound();
            }
            if(collisionTimoutTime < collisionTimout) collisionTimoutTime += Time.deltaTime;
            if(collisionTimoutTime > collisionTimout)
            {
                collisionTimoutTime = collisionTimout;
            }

        }

        public void SetSprites(Sprite chargeSprite, Sprite dashSprite, Sprite stopSprite)
        {
            this.chargeSprite = chargeSprite;
            this.dashSprite = dashSprite;
            this.stopSprite = stopSprite;
        }

        private void PlaySound(AudioClip[] audioClips)
        {
            m_AudioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length - 1)]);
        }

        public void PlayBiblical()
        {
            PlaySound(biblical);
        }

        private void OnCollisionEnter2D()
        {
            if (collisionTimoutTime < collisionTimout) return;
            collisionTimoutTime = 0;
            PlaySound(collision);
        }
    }

}
