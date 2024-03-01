using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StdNounou.ConsoleCommands
{
    public class ComponentDescr : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private int characterWrapLimit = 80;

        private SO_InspectorObjectData objectData;
        private Component component;

        private LTDescr currentTween;
        private float maxHeight = 0;

        private bool isVisible;
        public bool IsVisible { get => isVisible; }

        public void Setup(SO_InspectorObjectData objectData, Component component)
        {
            this.objectData = objectData;
            this.component = component;

            tmp.text = objectData.Process(component);
            tmp.rectTransform.sizeDelta = new Vector2(tmp.rectTransform.sizeDelta.x, tmp.preferredHeight);
            layoutElement.enabled = (tmp.text.Length > characterWrapLimit);
            StartCoroutine(SetMaxHeight());
        }

        private IEnumerator SetMaxHeight()
        {
            yield return new WaitForEndOfFrame();
            maxHeight = (this.transform as RectTransform).rect.height;

            isVisible = false;
            layoutElement.preferredHeight = 0;
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isVisible) return;
            tmp.text = objectData.Process(component);
        }

        public void FlipState()
        {
            if (this.gameObject.activeSelf)
            {
                if (currentTween != null)
                    LeanTween.cancel(currentTween.uniqueId);

                currentTween = LeanTween.value(maxHeight, 0, .25f)
                                        .setOnUpdate((float val) =>
                                        {
                                            layoutElement.preferredHeight = val;
                                        })
                                        .setIgnoreTimeScale(true)
                                        .setOnComplete(() => this.gameObject.SetActive(false));
                isVisible = false;
                return;
            }

            if (currentTween != null)
                LeanTween.cancel(currentTween.uniqueId);
            this.gameObject.SetActive(true);
            currentTween = LeanTween.value(0, maxHeight, .25f)
                                    .setOnUpdate((float val) =>
                                    {
                                        layoutElement.preferredHeight = val;
                                    })
                                    .setIgnoreTimeScale(true);
            isVisible = true;
        }
    }
}
