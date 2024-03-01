using TMPro;
using StdNounou.Core;
using UnityEngine;
using UnityEngine.UI;

namespace StdNounou.ConsoleCommands
{
    public class ComponentTitle : MonoBehaviour
    {
        [SerializeField] private ComponentDescr componentDescrPF;
        private ComponentDescr currentDescr;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image arrow;
        [SerializeField] private SO_InspectorObjectDataContainer inspectorObjectDataContainer;

        private LTDescr arrowTween;

        public void Setup<T>(T component) where T : Component
        {
            title.text = component.GetType().Name;
            if (!inspectorObjectDataContainer.Data.TryGetValue(component.GetType().Name, out var res))
            {
                arrow.SetAlpha(0);
                return;
            }
            arrow.SetAlpha(1);
            currentDescr = componentDescrPF?.Create(this.transform.parent);
            currentDescr.Setup(res, component);
            currentDescr.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
        }

        public void ChangeDescrState()
        {
            if (currentDescr == null) return;
            currentDescr.FlipState();
            if (arrowTween != null)
                LeanTween.cancel(arrowTween.uniqueId);

            if (currentDescr.IsVisible)
                arrowTween = LeanTween.rotateZ(arrow.gameObject, 0, .15f).setIgnoreTimeScale(true);
            else
                arrowTween = LeanTween.rotateZ(arrow.gameObject, 90, .15f).setIgnoreTimeScale(true);
        }

        private void OnDestroy()
        {
            if (currentDescr != null)
                Destroy(currentDescr.gameObject);
        }
    }
}
