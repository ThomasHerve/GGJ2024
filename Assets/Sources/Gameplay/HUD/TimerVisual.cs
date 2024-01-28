using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2024
{
    public class TimerVisual : MonoBehaviour
    {
        [SerializeField] private Image m_Fill;
        [SerializeField] private Image m_Foreground;

        public void SetTimer(float remainingTime, float maxTime)
        {
            m_Fill.fillAmount = remainingTime / maxTime;
            if (remainingTime < 5.0f)
            {
                m_Foreground.color = Color.Lerp(Color.white, Color.red, 1 - (remainingTime / 5.0f));
            }
        }

        internal void Reset()
        {
            m_Fill.fillAmount = 1.0f;
            m_Foreground.color = Color.white;
        }
    }
}