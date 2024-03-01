using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    public class Inspector : MonoBehaviour
    {
        [SerializeField] private Hierarchy relatedHierarchy;
        [SerializeField] private ComponentTitle componentTitlePF;
        [SerializeField] private RectTransform componentTitleContainer;
        private ComponentTitle[] componentTitles;

        private void Awake()
        {
            relatedHierarchy.OnOpenedHierarchy += OnOpenedHierarchy;
            relatedHierarchy.OnClosedHierarchy += OnClosedHierarchy;
            this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            relatedHierarchy.OnOpenedHierarchy -= OnOpenedHierarchy;
            relatedHierarchy.OnClosedHierarchy -= OnClosedHierarchy;
        }

        private void OnOpenedHierarchy(GameObject targetObj)
        {
            this.gameObject.SetActive(true);
            if (componentTitles != null)
            {
                for (int i = 0; i < componentTitles.Length; i++)
                {
                    componentTitles[i].gameObject.Destroy();
                }
            }

            Component[] targetComponents = targetObj.GetComponents(typeof(Component));
            componentTitles = new ComponentTitle[targetComponents.Length];
            if (targetComponents.Length == 0) return;

            for (int i = 0; i < targetComponents.Length; i++)
            {
                componentTitles[i] = componentTitlePF?.Create(componentTitleContainer);
                componentTitles[i].Setup(targetComponents[i]);
            }
        }

        private void OnClosedHierarchy()
        {
            if (componentTitles != null)
            {
                for (int i = 0; i < componentTitles.Length; i++)
                {
                    componentTitles[i].gameObject.Destroy();
                }
            }
            componentTitles = null;
            this.gameObject.SetActive(false);
        }
    }
}
