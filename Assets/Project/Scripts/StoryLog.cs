using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Translations;

namespace Project
{
    public class StoryLog : StateMachineBehaviour
    {
        public const string PrependBeforeLog = "Log {0:00}: ";

        [SerializeField]
        private bool isEnd = false;

        [Header("Log")]
        [SerializeField]
        private TranslatedString prependBeforeLogKey = new TranslatedString("Log Header");
        [SerializeField]
        [Multiline]
        private string log;
        [SerializeField]
        private TranslatedString logKey = new TranslatedString("Log Entry 01-1");

        private readonly StringBuilder builder = new StringBuilder();

        public bool IsEnd
        {
            get => isEnd;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StoryHistory history = animator.GetComponent<StoryHistory>();
            if(history != null)
            {
                builder.Clear();
                builder.AppendFormat(prependBeforeLogKey.ToString(), history.Count);
                builder.AppendFormat(logKey.ToString());
                history.AddLog(this, builder.ToString());
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
