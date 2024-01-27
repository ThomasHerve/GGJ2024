using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ2024
{
    public class InkMap : MonoBehaviour
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct PlayerInfo
        {
            public uint x;
            public uint y;
            public uint isInking;
            public uint unused;
        }

        public struct PlayerData
        {
            public Vector2 normalizedPosition;
            public bool isInking;
        }

        [SerializeField] private ComputeShader m_UpdateInkMapCompute;

        public RenderTexture Texture { get; private set; }
        private ComputeBuffer m_ConstantBuffer;
        private PlayerInfo[] m_PlayerInfos = new PlayerInfo[kPlayerCount];

        [SerializeField][Min(1)] private int m_Width = 256;
        [SerializeField][Min(1)] private int m_Height = 256;

        private const int kPlayerCount = 4;

        private static readonly int ConstantBufferNameID = Shader.PropertyToID("Players");
        private static readonly int InkMapNameID = Shader.PropertyToID("InkMap");

        void Start()
        {
            Assert.IsNotNull(m_UpdateInkMapCompute);
        }

        private void OnEnable()
        {
            Texture = new RenderTexture(new RenderTextureDescriptor(m_Width, m_Height, RenderTextureFormat.ARGB64)
            {
                enableRandomWrite = true,
            });

            m_ConstantBuffer = new ComputeBuffer(1, 16 * kPlayerCount);
        }

        private void OnDisable()
        {
            Texture?.Release();
            Texture = null;

            m_ConstantBuffer?.Release();
            m_ConstantBuffer = null;
        }

        public void UpdateMap(PlayerData[] playerDatas)
        {
            if (m_UpdateInkMapCompute != null)
            {
                for (int i = 0; i < kPlayerCount; i++)
                {
                    var coords = ToTextureCoords(playerDatas[i].normalizedPosition);
                    m_PlayerInfos[i] = new PlayerInfo
                    {
                        x = (uint)coords.x,
                        y = (uint)coords.y,
                        isInking = playerDatas[i].isInking ? 1u : 0u,
                    };
                }

                m_ConstantBuffer.SetData(m_PlayerInfos);

                m_UpdateInkMapCompute.SetConstantBuffer(ConstantBufferNameID, m_ConstantBuffer, 0, 64);
                m_UpdateInkMapCompute.SetTexture(0, InkMapNameID, Texture);
                m_UpdateInkMapCompute.Dispatch(0, GroupCount(Texture.width), GroupCount(Texture.height), 1);
            }
        }

        public void DiffuseMap()
        {
            m_UpdateInkMapCompute.SetTexture(1, InkMapNameID, Texture);
            m_UpdateInkMapCompute.Dispatch(1, GroupCount(Texture.width), GroupCount(Texture.height), 1);
        }

        private Vector2Int ToTextureCoords(Vector2 position)
        {
            return new Vector2Int(
                (int)(position.x * Texture.width),
                (int)(position.y * Texture.height)
            );
        }

        private static int GroupCount(int size)
        {
            return Mathf.FloorToInt((size - 1) / 8.0f);
        }
    }
}