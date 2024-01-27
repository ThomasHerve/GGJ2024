using System.Collections.Generic;
using UnityEngine;

namespace GGJ2024.Samples
{
    [RequireComponent(typeof(InkMap))]
    public class InkSample : MonoBehaviour
    {
        public Material material;

        void Update()
        {
            var inkMap = GetComponent<InkMap>();

            for (int i = 0; i < 1; i++)
            {
                var lastTime = Time.time - Time.deltaTime;
                var time = Time.time;

                var t = Mathf.Lerp(lastTime, time, i * 1f) * 0.1f;

                var t0 = Mathf.PingPong(t, 0.8f);
                var t1 = Mathf.PingPong(t + 0.5f, 0.8f);

                //inkMap.UpdateMap(new List<InkMap.PlayerData>()
                //{
                //    new Vector2(0.1f + t0, 0.5f + Mathf.Sin(t * Mathf.PI * 2.1f) * 0.4f),
                //    new Vector2(0.1f + t1, 0.5f + Mathf.Sin(t * Mathf.PI * 1.4f) * 0.4f),
                //});
            }

            material.mainTexture = inkMap.Texture;
        }
    }
}