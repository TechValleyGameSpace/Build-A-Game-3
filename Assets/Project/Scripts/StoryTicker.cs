using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project
{
    public class StoryTicker : MonoBehaviour
    {
        [SerializeField]
        private int currentIndex = 0;
        [SerializeField]
        private float scrollSpeed = 10f;

        [Header("Required Components")]
        [SerializeField]
        private StoryHistory history;
        [SerializeField]
        private TextMeshProUGUI label;
        [SerializeField]
        private RectTransform parent;
        [SerializeField]
        private RectTransform movingChild;

        private bool isScrolling = false;
        private bool isGameFinished = false;
        private Vector2 initialChildPosition;
        private Vector2 currentChildPosition;

        private void Start()
        {
            initialChildPosition = movingChild.localPosition;
        }

        private void Update()
        {
            if (currentIndex < history.Count)
            {
                if (isScrolling == true)
                {
                    // FIXME: check if the text is done scrolling
                    if ((movingChild.localPosition.x + movingChild.rect.width) < parent.rect.xMin)
                    {
                        // Reset the position of the child
                        movingChild.localPosition = initialChildPosition;

                        // Indicate to stop scrolling
                        isScrolling = false;
                        ++currentIndex;
                    }
                    else
                    {
                        // FIXME: animate scrolling the text
                        currentChildPosition = movingChild.localPosition;
                        currentChildPosition.x -= scrollSpeed * Time.deltaTime;
                        movingChild.localPosition = currentChildPosition;
                    }
                }
                else
                {
                    // Update the label, and display the text
                    label.SetText(history[currentIndex].Value);
                    isScrolling = true;
                }
            }
            else if((isGameFinished == false) && (history.Count > 0) && (history[history.Count - 1].Key.IsEnd == true))
            {
                OmiyaGames.Singleton.Get<OmiyaGames.Scenes.SceneTransitionManager>().LoadCredits();
                isGameFinished = true;
            }
        }
    }
}
