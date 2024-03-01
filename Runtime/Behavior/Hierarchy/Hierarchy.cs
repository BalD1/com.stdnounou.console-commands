using StdNounou.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StdNounou.ConsoleCommands
{
    public class Hierarchy : MonoBehaviourEventsHandler
    {
        [SerializeField] private HierarchyObject objPrefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private int childLeftPadding = 20;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private ScrollRect scrollrect;
        [SerializeField] private Button closeHierarchyBtn;
        private HierarchyObject[] objects;

        public event Action<GameObject> OnOpenedHierarchy;
        public event Action OnClosedHierarchy;

        private bool isActive = false;

        protected override void EventsSubscriber()
        {
            DeveloperConsoleEvents.OnUnSelectedObject += Close;
        }

        protected override void EventsUnSubscriber()
        {
            DeveloperConsoleEvents.OnUnSelectedObject -= Close;
        }

        public void BuildHierarchyFromConsoleSelected()
        {
            BuildHierarchy(DeveloperConsole.Instance.SelectedObject.transform);
        }
        public void BuildHierarchy(Transform targetObj)
        {
            closeHierarchyBtn.gameObject.SetActive(true);
            this.gameObject.SetActive(true);
            isActive = true;
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    Destroy(objects[i].gameObject);
                }
            }

            int arraySize = targetObj.parent == null ? 1 : 2;
            arraySize += targetObj.childCount;

            objects = new HierarchyObject[arraySize];
            int nextItemIdx = 0;
            int nextItemPadding = 0;

            if (targetObj.parent != null)
            {
                AddObjectToArray(0, targetObj.parent, true, nextItemPadding, "Parent - ");
                nextItemIdx = 1;
                nextItemPadding += childLeftPadding;
            }

            AddObjectToArray(nextItemIdx, targetObj, true, nextItemPadding, "Current - ");
            nextItemIdx++;
            nextItemPadding += childLeftPadding;

            for (int i = 0; i < targetObj.childCount; i++)
            {
                AddObjectToArray(nextItemIdx, targetObj.GetChild(i), false, nextItemPadding);
                nextItemIdx++;
            }

            OnOpenedHierarchy?.Invoke(targetObj.gameObject);
            StartCoroutine(WaitForNextFrame());
        }
        public void Close()
        {
            isActive = false;
            closeHierarchyBtn.gameObject.SetActive(false);
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    Destroy(objects[i].gameObject);
                }
            }
            objects = null;
            OnClosedHierarchy?.Invoke();
            this.gameObject.SetActive(false);
        }

        private IEnumerator WaitForNextFrame()
        {
            yield return new WaitForEndOfFrame();
            scrollbar.SetValueWithoutNotify(1);
            scrollrect.normalizedPosition = Vector2.one;
        }

        private void AddObjectToArray(int idx, Transform obj, bool isOpen, int leftPadding, string additionalPrefix = "")
        {
            objects[idx] = objPrefab.Create(container).Setup(obj.gameObject, leftPadding, additionalPrefix);
            objects[idx].OnClick += OnObjectClicked;
        }

        private void OnObjectClicked(HierarchyObject obj)
        {
            BuildHierarchy(obj.TargetObj.transform);
            DeveloperConsole.Instance.SelectObject(obj.TargetObj);
        }
    }
}
