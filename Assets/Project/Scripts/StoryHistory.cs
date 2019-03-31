using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Animator))]
    public class StoryHistory : MonoBehaviour
    {
        [SerializeField]
        private string nextLogTrigger = "Next Log";
        [SerializeField]
        private string randomFloatField = "Random";

        private readonly List<KeyValuePair<StoryLog, string>> allGeneratedLogs = new List<KeyValuePair<StoryLog, string>>();
        private Animator animator;

        public int Count
        {
            get => allGeneratedLogs.Count;
        }

        public KeyValuePair<StoryLog, string> this[int index]
        {
            get => allGeneratedLogs[index];
        }

        public Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        public string LatestLog
        {
            get
            {
                if(Count > 0)
                {
                    return this[Count - 1].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public void GetNextLog()
        {
            Animator.SetFloat(randomFloatField, Random.value);
            Animator.SetTrigger(nextLogTrigger);
        }

        public void AddLog(StoryLog log, string fullText)
        {
            allGeneratedLogs.Add(new KeyValuePair<StoryLog, string>(log, fullText));
        }
    }
}
