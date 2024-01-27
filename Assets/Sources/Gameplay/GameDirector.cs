using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GGJ2024
{
    public class GameDirector : MonoBehaviour
    {
        [Serializable]
        public struct LevelDescriptor
        {
            public float time;
            public float drawFadeoutTime;
            public Sprite drawSprite;
        }

        public int Level { get; set; }
        public float Timer { get; set; }

        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer m_DrawSpriteRenderer;
        [SerializeField] private TextMeshProUGUI m_TimerText;
        [SerializeField] private TextMeshProUGUI m_LevelText;

        [Header("Levels")]
        [SerializeField] private LevelDescriptor[] m_Levels;

        public static GameDirector Instance { get; private set; }

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
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator Start()
        {
            // Start first level.
            yield return LevelCoroutine(0);
        }

        private IEnumerator LevelCoroutine(int level)
        {
            Debug.Log($"Begin level {level}");
            var levelDescriptor = m_Levels[level];

            Level = level;
            Timer = levelDescriptor.time;

            m_TimerText.text = TimeSpan.FromSeconds(Timer).ToString(@"mm\:ss");
            m_LevelText.text = $"{level + 1} / {m_Levels.Length}";

            m_DrawSpriteRenderer.sprite = levelDescriptor.drawSprite;
            m_DrawSpriteRenderer.color = new Color(1, 1, 1, 1);

            while (Timer > 0)
            {
                yield return null;
                Timer -= Time.deltaTime;
                Timer = Mathf.Max(Timer, 0);

                // Fadeout the drawing picture
                var elapsedTime = (levelDescriptor.time - Timer);
                var fadeout = elapsedTime / levelDescriptor.drawFadeoutTime;
                var alpha = Mathf.Lerp(1, 0, fadeout);
                m_DrawSpriteRenderer.color = new Color(1, 1, 1, alpha);

                // Update the timer
                m_TimerText.text = TimeSpan.FromSeconds(Timer).ToString(@"mm\:ss");
            }

            level++;
            if (level < m_Levels.Length)
            {
                yield return LevelCoroutine(level);
            }
            yield return RecapCoroutine();
        }

        private IEnumerator RecapCoroutine()
        {
            Debug.Log("Finished !");
            yield return null;
        }
    }
}
