using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StdNounou.ConsoleCommands
{
    public class HierarchyObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI objectNameTMP;
        [SerializeField] private HorizontalLayoutGroup group;

        public GameObject TargetObj { get; private set; }

        public event Action<HierarchyObject> OnClick;

        public HierarchyObject Setup(GameObject targetObj, int leftPadding, string additionalPrefix = "")
        {
            this.TargetObj = targetObj;
            objectNameTMP.text = additionalPrefix + targetObj.name;
            group.padding.left = leftPadding;
            return this;
        }

        public void OnBtnClick()
            => OnClick?.Invoke(this);
    }
}
