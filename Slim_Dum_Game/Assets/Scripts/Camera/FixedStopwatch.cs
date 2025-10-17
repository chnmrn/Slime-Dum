using System;
using UnityEngine;

namespace Common
{
    [Serializable]
    public class FixedStopwatch
    {
        public bool IsFinished => Elapsed > duration;
        public bool IsReady => Elapsed > cooldown;
        public float Completion => Mathf.Clamp01(Elapsed / duration);

        private float _timestamp;
        [SerializeField] private float duration;
        [SerializeField] private float cooldown;
        private int v1;
        private int v2;

        public FixedStopwatch(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        private float Elapsed => Time.fixedTime - _timestamp;

        public void Reset()
        {
            _timestamp = Time.fixedTime - cooldown - duration - 1;
        }

        public void Split()
        {
            _timestamp = Time.fixedTime;
        }
    }
}