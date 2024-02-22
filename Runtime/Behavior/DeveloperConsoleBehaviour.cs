using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using StdNounou.Core;

namespace StdNounou.ConsoleCommands
{
    public class DeveloperConsoleBehaviour : PersistentSingleton<DeveloperConsoleBehaviour>
    {
        [SerializeField] private string prefix;
        [SerializeField] private SO_ConsoleCommand[] commands;

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI ghostInput;
        private int ghostArgumentsStartIdx = -1;
        [SerializeField] private TextMeshProUGUI consoleField;
        [SerializeField] private Scrollbar consoleScrollbar;

        [SerializeField] private bool showCommandsInConsole;
        [SerializeField] private bool controlTimescale;

        private GameObject selectedObject;

        private string propositionGhostCommand = "";

        private float pausedTimedScale;

        [SerializeField] private int cmdHistoryMaxSize = 10;
        private List<Tuple<IConsoleCommand, string[]>> commandsHistory;
        private int positionInHistory = -1;

        private DeveloperConsole developerConsole;
        public DeveloperConsole DeveloperConsole
        {
            get
            {
                if (developerConsole != null) return developerConsole;
                developerConsole = new DeveloperConsole(prefix, commands);
                commandsHistory = new List<Tuple<IConsoleCommand, string[]>>();
                return developerConsole;
            }
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            DeveloperConsole.OnActivatedCommand += OnActivatedCommand;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                RaycastToAnyObj();
        }

        private void RaycastToAnyObj()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                StringBuilder sb = new StringBuilder("DEBUG_ITEM : <color=#0080ff>");
                sb.Append(hitObject.name);
                sb.Append("</color> \n");
                sb.Append("> ID : ");
                sb.Append(hitObject.GetInstanceID());
                AddTextToConsole(sb.ToString());
                selectedObject = hitObject;
            }
        }

        public UnityEngine.Object FindObjectFromInstanceID(int iid)
        {
            return (UnityEngine.Object)typeof(UnityEngine.Object)
            .GetMethod("FindObjectFromInstanceID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { iid });
        }

        private void OnActivatedCommand(IConsoleCommand cmd, string[] args)
        {
            commandsHistory.Add( new Tuple<IConsoleCommand, string[]>(cmd, args));
            if (commandsHistory.Count > cmdHistoryMaxSize)
                commandsHistory.RemoveAt(commandsHistory.Count - 1);
            if (!showCommandsInConsole) return;
            AddTextToConsole(prefix + cmd.CommandKey);
        }

        public void Toggle(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (uiCanvas.activeSelf)
            {
                if (controlTimescale)
                    Time.timeScale = pausedTimedScale;
                uiCanvas.SetActive(false);
                positionInHistory = -1;
                this.enabled = false;
                selectedObject = null;
            }
            else
            {
                if (controlTimescale)
                {
                    pausedTimedScale = Time.timeScale;
                    Time.timeScale = 0;
                }
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
                this.enabled = true;
            }
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix))
            {
                AddTextToConsole(inputValue);
                return;
            }
            DeveloperConsole.ProcessCommand(inputValue);
            inputField.text = prefix;
            inputField.ActivateInputField();
            inputField.selectionFocusPosition = prefix.Length;
        }

        public void OnInputFieldValueChanged(string currentString)
        {
            if (currentString.Length <= prefix.Length)
            {
                ghostInput.text = "";
                return;
            }
            IEnumerable<IConsoleCommand> matchingCommands = developerConsole.commands.Where(c => c.CommandKey.StartsWith(currentString.Remove(0, prefix.Length), StringComparison.OrdinalIgnoreCase));
            if (matchingCommands.Count() == 0)
            {
                ghostInput.text = "";
                return;
            }
            StringBuilder sb = new StringBuilder(prefix);
            sb.Append(matchingCommands.First().CommandKey);
            propositionGhostCommand = sb.ToString();
            sb.Append(" ");
            ghostArgumentsStartIdx = sb.Length;
            sb.Append(matchingCommands.First().CommandArguments);

            propositionGhostCommand = sb.ToString();
            ghostInput.text = propositionGhostCommand;
        }

        public void AcceptGhostCommand(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            inputField.text = propositionGhostCommand;
            inputField.selectionAnchorPosition = ghostArgumentsStartIdx;
            inputField.selectionFocusPosition = propositionGhostCommand.Length;
            Canvas.ForceUpdateCanvases();
        }

        public void ValidateInput(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            positionInHistory = -1;
            ProcessCommand(inputField.text);
        }

        public void AddTextToConsole(string txt)
        {
            StringBuilder sb = new StringBuilder("\n ");
            sb.Append("[");
            sb.Append(DateTime.Now.ToString("HH:mm:ss:ff"));
            sb.Append("] > ");
            sb.Append(txt);
            consoleField.text += sb.ToString();
            inputField.text = string.Empty;
            inputField.ActivateInputField();
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = 0;
            Canvas.ForceUpdateCanvases();
            consoleScrollbar.value = 0;
        }

        public void NavigateInHistoryToPosition(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (commandsHistory.Count == 0) return;

            if (positionInHistory == -1)
            {
                int dir = (int)ctx.ReadValue<Vector2>().y;
                if (dir == 1)
                    positionInHistory = 0;
                else
                    positionInHistory = commandsHistory.Count - 1;
            }
            else
            {
                positionInHistory = positionInHistory + (int)ctx.ReadValue<Vector2>().y;
                positionInHistory = Mathf.Clamp(positionInHistory, 0, commandsHistory.Count - 1);
            }

            StringBuilder sb = new StringBuilder(prefix);
            sb.Append(commandsHistory[positionInHistory].Item1.CommandKey);
            foreach (var item in commandsHistory[positionInHistory].Item2)
            {
                sb.Append(" ");
                sb.Append(item);
            }
            inputField.text = sb.ToString();
        }
    }
}
