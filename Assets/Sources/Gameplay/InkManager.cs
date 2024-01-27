using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ2024
{
    public class InkManager : MonoBehaviour
    {
        public static InkManager Instance { get; private set; }

        [field: SerializeField]
        public InkMap InkMap { get; private set; }
        
        [field: SerializeField]
        public Rect WorldRect { get; private set; }

        [SerializeField] private Material m_Material;
        private List<PlayerController> m_Players = new List<PlayerController>(4);
        private List<InkMap.PlayerData> m_PlayerDatas = new List<InkMap.PlayerData>(4);

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

            Assert.IsNotNull(m_Material);
            Assert.IsNotNull(InkMap);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            m_PlayerDatas.Clear();
            foreach (var player in m_Players) 
            {
                var playerPos = (Vector2) player.transform.position;
                var normalizedPosition = new Vector2 (
                    (playerPos.x - WorldRect.xMin) / WorldRect.width,
                    (playerPos.y - WorldRect.yMin) / WorldRect.height
                );

                m_PlayerDatas.Add(new InkMap.PlayerData
                {
                    normalizedPosition = normalizedPosition,
                    isInking = player.IsInking
                });
            }

            InkMap.UpdateMap(m_PlayerDatas);
            m_Material.mainTexture = InkMap.Texture;
        }

        public void RegisterPlayer(PlayerController player)
        {
            m_Players.Add(player);
        }

        public void UnregisterPlayer(PlayerController player)
        {
            m_Players.Remove(player);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(WorldRect.center, WorldRect.size);
        }
    }
}
