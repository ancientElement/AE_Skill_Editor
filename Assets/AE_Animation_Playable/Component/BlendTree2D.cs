using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    [System.Serializable]
    public struct BlendClip2D
    {
        public AnimationClip clip;
        public Vector2 pos;
    }

    public class BlendTree2D : AnimBehaviour
    {
        private struct DataPair
        {
            public float x;
            public float y;
            public float output;
        }

        private AnimationMixerPlayable m_mixer;
        private Vector2 m_pointer;
        public Vector2 Pointer => m_pointer;
        private float m_total;
        private int m_clipCount;

        private ComputeShader m_computeShader;
        private ComputeBuffer m_computeBuffer;
        private DataPair[] m_datas;
        private int m_kernel;
        private int m_pointerX;
        private int m_pointerY;

        public BlendTree2D(PlayableGraph graph,
            BlendClip2D[] clips, float enterTime = 0f, float eps = 1e-5f) :
            base(graph, enterTime)
        {
            m_datas = new DataPair[clips.Length];

            m_mixer = AnimationMixerPlayable.Create(graph);
            m_adapterPlayable.AddInput(m_mixer, 0, 1f);
            for (int i = 0; i < clips.Length; i++)
            {
                m_mixer.AddInput(AnimationClipPlayable.Create(graph, clips[i].clip), 0);
                m_datas[i].x = clips[i].pos.x;
                m_datas[i].y = clips[i].pos.y;
            }

            m_computeShader = AnimHelper.GetComputeShader("BlendTree2D");
            m_computeBuffer = new ComputeBuffer(clips.Length, 12);
            m_kernel = m_computeShader.FindKernel("Compute");
            m_computeShader.SetBuffer(m_kernel, "dataBuffer", m_computeBuffer);
            m_computeShader.SetFloat("eps", eps);
            m_pointerX = Shader.PropertyToID("pointerX");
            m_pointerY = Shader.PropertyToID("pointerY");
            m_clipCount = clips.Length;
            m_pointer.Set(1, 1);
            SetPointer(0, 0);

            Disable();
        }

        public BlendTree2D(PlayableGraph graph, float enterTime) : base(graph, enterTime)
        {
        }

        public void SetPointer(Vector2 vector)
        {
            SetPointer(vector.x, vector.y);
        }

        public void SetPointer(float x, float y)
        {
            if (m_pointer.x == x && m_pointer.y == y)
            {
                return;
            }
            m_pointer.Set(x, y);

            int i;
            m_computeShader.SetFloat(m_pointerX, x);
            m_computeShader.SetFloat(m_pointerY, y);

            m_computeBuffer.SetData(m_datas);
            m_computeShader.Dispatch(m_kernel, m_clipCount, 1, 1);
            m_computeBuffer.GetData(m_datas);
            for (i = 0; i < m_clipCount; i++)
            {
                m_total += m_datas[i].output;
            }
            for (i = 0; i < m_clipCount; i++)
            {
                m_mixer.SetInputWeight(i, m_datas[i].output / m_total);
            }
            m_total = 0f;
        }

        public override void Enable()
        {
            base.Enable();

            SetPointer(0, 0);
            for (int i = 0; i < m_clipCount; i++)
            {
                m_mixer.GetInput(i).Play();
                m_mixer.GetInput(i).SetTime(0f);
            }
            m_mixer.SetTime(0f);
            m_mixer.Play();
            m_adapterPlayable.SetTime(0f);
            m_adapterPlayable.Play();
        }

        public override void Disable()
        {
            base.Disable();
            for (int i = 0; i < m_clipCount; i++)
            {
                m_mixer.GetInput(i).Pause();
            }
            m_mixer.Pause();
            m_adapterPlayable.Pause();
        }

        public override void Stop()
        {
            base.Stop();
            m_computeBuffer.Dispose();
        }
    }
}