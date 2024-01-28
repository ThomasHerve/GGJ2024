using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Linq;
using static GGJ2024.GameDirector;
using UnityEngine.SceneManagement;

namespace GGJ2024
{
    public class GameDirector : MonoBehaviour
    {
        [Serializable]
        public struct LevelDescriptor
        {
            public float time;
            public float drawFadeoutTime;
            public Texture[] drawTexture;
        }
        public static GameDirector Instance { get; private set; }


        public int Level { get; set; }
        public float Timer { get; set; }
        public RenderTexture[] ScreenshotTextures { get; private set; }

        [Header("Dependencies")]
        [SerializeField] private Material m_DrawPictureMaterial;
        [SerializeField] private TimerVisual m_Timer;
        [SerializeField] private TextMeshProUGUI m_LevelText;
        [SerializeField] private TextMeshProUGUI m_GoText;

        [Header("Transition")]
        [SerializeField] private Sprite[] m_TsunamiAnimation;
        [SerializeField] private float m_TsunamiAnimationSpeed;
        [SerializeField] private RectTransform m_TransitionPanel;
        [SerializeField] private Transform m_TransitionStartAnchor;
        [SerializeField] private Transform m_TransitionEndAnchor;
        [SerializeField] private Transform m_TransitionForeground;

        [Header("Recap")]
        [SerializeField] private RectTransform m_RecapPanel;
        [SerializeField] private RawImage[] m_ScreenshotImages;
        [SerializeField] private Transform[] m_ScreenshotTargetAnchors;

        [Header("Levels")]
        [SerializeField] private LevelDescriptor[] m_Levels;

        [Header("Positions")]
        [SerializeField] private Transform[] m_PlayerPositions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            foreach (var texture in ScreenshotTextures)
            {
                texture.Release();
            }
            ScreenshotTextures = null;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator Start()
        {
            // Start first level.
            ScreenshotTextures = new RenderTexture[m_Levels.Length];
            for (int i = 0; i < ScreenshotTextures.Length; i++)
            {
                var rt = new RenderTexture(new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGB32));
                ScreenshotTextures[i] = rt;
                rt.Create();
            }

            ResetPlayerPositions();
            SetAllInputsEnabled(false);
            m_RecapPanel.gameObject.SetActive(false);
            InitializeLevel(0);
            yield return LevelCoroutine();
        }

        private void InitializeLevel(int level)
        {
            Debug.Log($"Begin level {level}");
            var levelDescriptor = m_Levels[level];
            Level = level;
            Timer = levelDescriptor.time;

            InkManager.Instance.InkMap.Clear();
            ResetPlayerPositions();
            m_DrawPictureMaterial.mainTexture = levelDescriptor.drawTexture[UnityEngine.Random.Range(0, levelDescriptor.drawTexture.Length - 1)];
            m_DrawPictureMaterial.color = new Color(1, 1, 1, 1);

            //m_TimerText.text = TimeSpan.FromSeconds(Timer).ToString(@"mm\:ss");
            m_Timer.Reset();
            m_LevelText.text = $"{level + 1}/{m_Levels.Length}";

        }

        private IEnumerator LevelCoroutine()
        {
            var levelDescriptor = m_Levels[Level];
            float nextTimerPunch = Mathf.Min(Timer, 10.0f);
            var screenShotTime = UnityEngine.Random.Range(0.0f, 5.0f);
            bool isShaking = false;
            bool isScreenshotRendered = false;

            yield return new WaitForSeconds(3.0f);

            m_GoText.alpha = 1.0f;
            m_GoText.transform.localScale = Vector2.one * 4;
            m_GoText.transform.DOScale(1, 0.25f);
            m_GoText.DOFade(0, 1.0f);

            SetAllInputsEnabled(true);

            while (Timer > 0)
            {
                yield return null;
                Timer -= Time.deltaTime;
                Timer = Mathf.Max(Timer, 0);

                // Fadeout the drawing picture

                var elapsedTime = (levelDescriptor.time - Timer);
                var fadeout = elapsedTime / levelDescriptor.drawFadeoutTime;
                var alpha = Mathf.Lerp(1, 0, fadeout);
                m_DrawPictureMaterial.color = new Color(1, 1, 1, alpha);

                if (Timer < nextTimerPunch)
                {
                    m_Timer.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f);
                    nextTimerPunch -= 1.0f;
                }

                if (Timer < 3.0f && !isShaking)
                {
                    //m_TimerText.color = Color.red;
                    m_Timer.transform.DOShakePosition(10f, 3f, 50);
                    isShaking = true;
                }

                if (Timer < screenShotTime && !isScreenshotRendered)
                {
                    var request = new RenderPipeline.StandardRequest();
                    request.destination = ScreenshotTextures[Level];
                    RenderPipeline.SubmitRenderRequest(Camera.main, request);
                    isScreenshotRendered = true;
                }

                // Update the timer
                m_Timer.SetTimer(Timer, levelDescriptor.time);
            }

            // Play "sifflet tût tût"
            SetAllInputsEnabled(false);
            yield return new WaitForSeconds(2.0f);

            Level++;
            if (Level < m_Levels.Length)
            {
                yield return TransitionCoroutine(() => {
                    InitializeLevel(Level);
                });
                yield return LevelCoroutine();
            }
            else
            {
                yield return TransitionCoroutine(() => m_RecapPanel.gameObject.SetActive(true));
                yield return RecapCoroutine();
            }
        }

        private IEnumerator TransitionCoroutine(Action callback)
        {
            /*
            var rectTransform = m_TransitionPanel.GetComponent<RectTransform>();

            var sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOMove(m_TransitionEndAnchor.position, 1.5f).SetEase(Ease.OutBounce));
            sequence.AppendInterval(1.0f);
            sequence.AppendCallback(callback);
            sequence.Append(rectTransform.DOMove(m_TransitionStartAnchor.position, 1.5f).SetEase(Ease.InQuad));

            yield return sequence.WaitForCompletion();
            */
            m_TransitionForeground.gameObject.SetActive(true);
            var rawImage = m_TransitionForeground.GetComponent<RawImage>();
            for (int i = 0; i < m_TsunamiAnimation.Length; i++)
            {
                rawImage.texture = m_TsunamiAnimation[i].texture;
                yield return new WaitForSeconds(m_TsunamiAnimationSpeed);
            }
            yield return new WaitForSeconds(0.5f);
            callback?.Invoke();
            for (int i = m_TsunamiAnimation.Length-1; i >= 0; i--)
            {
                rawImage.texture = m_TsunamiAnimation[i].texture;
                yield return new WaitForSeconds(m_TsunamiAnimationSpeed);
            }
            m_TransitionForeground.gameObject.SetActive(false);

        }

        private IEnumerator RecapCoroutine()
        {
            Debug.Log("Finished !");
            yield return new WaitForSeconds(2.0f);

            for (int i  = 0; i < m_Levels.Length; i++)
            {
                m_ScreenshotImages[i].texture = ScreenshotTextures[i];
                m_ScreenshotImages[i].transform.parent.DOMove(m_ScreenshotTargetAnchors[i].position, 1.0f);
                m_ScreenshotImages[i].transform.parent.DORotateQuaternion(m_ScreenshotTargetAnchors[i].rotation, 1.0f);
                yield return new WaitForSeconds(3.0f);
            }

            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("IntroMenu");
        }

        private void SetAllInputsEnabled(bool enabled)
        {
            foreach (var player in InkManager.Instance.Players)
            {
                player.SetInputEnabled(enabled);
            }
        }

        private void ResetPlayerPositions()
        {
            var randomIndices = new List<int>() { 0, 1, 2, 3 }
                .OrderBy(i => UnityEngine.Random.value).ToArray();
            
            foreach (var player in InkManager.Instance.Players)
            {
                var index = randomIndices[player.ID];
                player.transform.position = m_PlayerPositions[index].position;
                player.transform.rotation = Quaternion.identity;
            }
        }
    }
}
