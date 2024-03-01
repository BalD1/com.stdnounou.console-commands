using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    public class ConsoleCamera : MonoBehaviourEventsHandler
    {
        [SerializeField] private Camera cam;
        [SerializeField] private RectTransform cameraRenderParent;
        [SerializeField] private Vector3 defaultTargetOffset;

        [SerializeField] private float orbitalRotationSpeed;
        [SerializeField] private float zoomSpeed;

        private Vector3 lastMousePosition;

        private Transform target;

        protected override void Awake()
        {
            base.Awake();
            cam.enabled = false;
            cameraRenderParent.gameObject.SetActive(false);
        }

        protected override void EventsSubscriber()
        {
            DeveloperConsoleEvents.OnSelectedObject += OnSelectedObject;
            DeveloperConsoleEvents.OnUnSelectedObject += OnUnselectedObject;
        }

        protected override void EventsUnSubscriber()
        {
            DeveloperConsoleEvents.OnSelectedObject -= OnSelectedObject;
            DeveloperConsoleEvents.OnUnSelectedObject -= OnUnselectedObject;
        }

        private void OnSelectedObject(GameObject obj)
        {
            this.target = obj.transform;
            Vector3 pos = target.position;
            pos -= defaultTargetOffset;
            this.transform.position = pos;

            this.enabled = true;
            cameraRenderParent.gameObject.SetActive(true);
            cam.enabled = true;

            this.transform.parent = obj.transform;
        }

        private void OnUnselectedObject()
        {
            this.enabled = false;
            cameraRenderParent.gameObject.SetActive(false);
            cam.enabled = false;

            this.transform.parent = null;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                lastMousePosition = Input.mousePosition;
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
                Orbit();
            if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl))
                Zoom();
        }

        private void Orbit()
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

            float mouseX = deltaMouse.x * orbitalRotationSpeed * Time.unscaledDeltaTime;
            float mouseY = -deltaMouse.y * orbitalRotationSpeed * Time.unscaledDeltaTime;

            mouseY = Mathf.Clamp(mouseY, -80f, 80f);

            this.transform.LookAt(target);
            this.transform.RotateAround(target.position, Vector3.up, mouseX);
            this.transform.RotateAround(target.position, this.transform.right, mouseY);

            lastMousePosition = Input.mousePosition;
        }

        private void Zoom()
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

            float mouseY = -deltaMouse.y * zoomSpeed * Time.unscaledDeltaTime;
            this.transform.Translate(0,0, mouseY, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
    }
}
