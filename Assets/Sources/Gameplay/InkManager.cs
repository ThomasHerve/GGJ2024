using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ2024
{
    public class InkManager : MonoBehaviour
    {
        public const int kSampleCountPerFrame = 4;

        public static InkManager Instance { get; private set; }

        [field: SerializeField]
        public InkMap InkMap { get; private set; }
        
        [field: SerializeField]
        public Rect WorldRect { get; private set; }

        [SerializeField] private Material m_Material;
        private List<PlayerController> m_Players = new List<PlayerController>(4);

        private InkMap.PlayerData[] m_PlayerDatas = new InkMap.PlayerData[4];
        private InkMap.PlayerData[] m_LastPlayerDatas = new InkMap.PlayerData[4];

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
            for (int i = 1; i <= kSampleCountPerFrame; i++)
            {
                foreach (var player in m_Players)
                {
                    var lastNormalizedPos = m_LastPlayerDatas[player.ID].normalizedPosition;

                    var playerPos = (Vector2)player.transform.position;
                    var normalizedPosition = new Vector2(
                        (playerPos.x - WorldRect.xMin) / WorldRect.width,
                        (playerPos.y - WorldRect.yMin) / WorldRect.height
                    );

                    normalizedPosition = Vector2.Lerp(lastNormalizedPos, normalizedPosition, (float) i / kSampleCountPerFrame);

                    m_PlayerDatas[player.ID] = new InkMap.PlayerData
                    {
                        normalizedPosition = normalizedPosition,
                        isInking = player.IsInking
                    };
                }

                InkMap.UpdateMap(m_PlayerDatas);
            }

            for (int i = 0; i < m_PlayerDatas.Length; i++)
            {
                m_PlayerDatas[i].isInking = false;
                m_LastPlayerDatas[i] = m_PlayerDatas[i];
            }

            InkMap.DiffuseMap();

            m_Material.mainTexture = InkMap.Texture;
            m_Material.SetTexture("_InkMap", InkMap.Texture);
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
