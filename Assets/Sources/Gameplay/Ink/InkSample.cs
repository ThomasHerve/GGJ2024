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
                var t2 = Mathf.PingPong(t + 0.33f, 0.8f);
                var t3 = Mathf.PingPong(t * 0.9f + 0.117f, 0.8f);

                var p0 = new Vector2(0.1f + t0, 0.5f + Mathf.Sin(t * Mathf.PI * 2.478213f) * 0.4f);
                var p1 = new Vector2(0.1f + t1, 0.5f + Mathf.Sin(t * Mathf.PI * 3.67f) * 0.4f);
                var p2 = new Vector2(0.1f + t2, 0.5f + Mathf.Sin(t * Mathf.PI * 1.73f) * 0.4f);
                var p3 = new Vector2(0.1f + t3, 0.5f + Mathf.Sin(t * Mathf.PI * 9.14f) * 0.4f);

                inkMap.UpdateMap(new InkMap.PlayerData[]
                {
                    new InkMap.PlayerData { normalizedPosition = p0, isInking = true },
                    new InkMap.PlayerData { normalizedPosition = p1, isInking = true },
                    new InkMap.PlayerData { normalizedPosition = p2, isInking = true },
                    new InkMap.PlayerData { normalizedPosition = p3, isInking = true },
                });
            }

            material.mainTexture = inkMap.Texture;
            material.SetTexture("_InkMap", inkMap.Texture);
        }
    }
}