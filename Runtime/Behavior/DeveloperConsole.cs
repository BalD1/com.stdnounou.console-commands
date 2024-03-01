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
    public class DeveloperConsole : PersistentSingleton<DeveloperConsole>
    {
        [SerializeField] private string prefix;
        [SerializeField] private SO_ConsoleCommandsHolder commandsHolder;

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI ghostInput;
        private int ghostArgumentsStartIdx = -1;
        [SerializeField] private TextMeshProUGUI consoleField;
        [SerializeField] private TextMeshProUGUI selectedItemTMP;
        [SerializeField] private Scrollbar consoleScrollbar;

        [SerializeField] private Button inspectBtn; 

        [SerializeField] private bool showCommandsInConsole;
        [SerializeField] private bool controlTimescale;

        public GameObject SelectedObject { get; private set; }

        private string propositionGhostCommand = "";
        private const string SELECTED_OBJECT_FORMAT = "Selected : {0}";
        private string oldInput = string.Empty;

        private float pausedTimedScale;

        [SerializeField] private int cmdHistoryMaxSize = 10;
        private List<Tuple<IConsoleCommand, string[]>> commandsHistory;
        private int positionInHistory = -1;

        private readonly Dictionary<char, char> charactersToDouble = new Dictionary<char, char>
        {
            { '"', '"' }, { '\'', '\'' }, { '%', '%' }, { '{', '}' }, { '(', ')' }, { '[', ']' }, { '<', '>' }
        };

        private void OnValidate()
        {
            selectedItemTMP.text = string.Format(SELECTED_OBJECT_FORMAT, "NONE");
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
            commandsHistory = new List<Tuple<IConsoleCommand, string[]>>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                RaycastToAnyObj();
            if (Input.GetMouseButtonDown(1))
                UnselectObject();
        }

        private void RaycastToAnyObj()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                if (hitObject == SelectedObject)
                    UnselectObject();
                else
                    SelectObject(hitObject);
            }
        }

        public void SelectObject(GameObject obj)
        {
            SelectedObject = obj;
            selectedItemTMP.text = string.Format(SELECTED_OBJECT_FORMAT, SelectedObject.name);
            StringBuilder sb = new StringBuilder("DEBUG_ITEM : <color=#0080ff>");
            sb.Append(obj.name);
            sb.Append("</color> \n");
            sb.Append("> ID : ");
            sb.Append(obj.GetInstanceID());
            AddTextToConsole(sb.ToString());
            inspectBtn.gameObject.SetActive(true);
            this.OnSelectedObject_Call(obj);
        }
        private void UnselectObject()
        {
            SelectedObject = null;
            selectedItemTMP.text = string.Format(SELECTED_OBJECT_FORMAT, "NONE");
            inspectBtn.gameObject.SetActive(false);
            this.OnUnselectedObject_Call();
        }

        private void OnActivatedCommand(IConsoleCommand cmd, string[] args)
        {
            commandsHistory.Add( new Tuple<IConsoleCommand, string[]>(cmd, args));
            if (commandsHistory.Count > cmdHistoryMaxSize)
                commandsHistory.RemoveAt(commandsHistory.Count - 1);
            if (!showCommandsInConsole) return;
            AddTextToConsole(prefix + cmd.CommandKey);
            this.OnPerformedCommand_Call(cmd, args);
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
                UnselectObject();
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
            ParseCommand(inputValue);
            inputField.text = prefix;
            inputField.ActivateInputField();
            inputField.selectionFocusPosition = prefix.Length;
        }

        public void ParseCommand(string inputValue)
        {
            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            LookForCommand(commandInput, args);
        }
        public void LookForCommand(string commandInput, string[] args)
        {
            foreach (var item in commandsHolder.Commands)
            {
                if (!commandInput.Equals(item.CommandKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Process(args))
                {
                    OnActivatedCommand(item, args);
                    return;
                }
            }
        }

        public void OnInputFieldValueChanged(string currentString)
        {
            if (currentString.Length > oldInput.Length)
                TryDoubleLastCharacter();
            ProcessGhostCommand(currentString);
            oldInput = currentString;
        }

        private bool TryDoubleLastCharacter()
        {
            char lastCharOfInput = inputField.text.Last();
            if (!charactersToDouble.TryGetValue(lastCharOfInput, out char characterToAdd))
                return false;
            if (lastCharOfInput == characterToAdd &&
                inputField.text.Length > 1 && 
                inputField.text[inputField.text.Length - 2] == lastCharOfInput)
                return false;

            inputField.text += characterToAdd;
            return true;
        }

        private void ProcessGhostCommand(string currentString)
        {
            if (currentString.Length <= prefix.Length)
            {
                ghostInput.text = "";
                return;
            }
            IEnumerable<IConsoleCommand> matchingCommands = commandsHolder.Commands.Where(c => c.CommandKey.StartsWith(currentString.Remove(0, prefix.Length), StringComparison.OrdinalIgnoreCase));
            if (matchingCommands.Count() == 0)
            {
                ghostInput.text = "";
                return;
            }
            StringBuilder sb = new StringBuilder(prefix);
            sb.Append(matchingCommands.First().CommandKey);
            sb.Append(" ");
            ghostArgumentsStartIdx = sb.Length;
            sb.Append(matchingCommands.First().CommandArguments);

            propositionGhostCommand = sb.Remove(0, currentString.Length).Insert(0, currentString).ToString();
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
