#if UNITY_TMPRO && UNITY_UI

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FlexalonCopilot.Editor
{
    internal class ChatWindow : EditorWindow
    {
        [SerializeField]
        private ChatLog _chatLog;

        [SerializeField]
        private PrefabSet _prefabSet;

        [SerializeField]
        private List<UnityEngine.Object> _contextObjects = new List<UnityEngine.Object>();

        [SerializeField]
        private GameObject _rootGameObject;
        public GameObject RootGameObject
        {
            get => _rootGameObject;
            set => _rootGameObject = value;
        }

        [SerializeField]
        private string _promptText;

        [SerializeField]
        private string _errorText;

        [SerializeField]
        private int _undoGroup = 0;

        private GUIStyle _errorStyle;
        private GUIStyle _promptStyle;
        private GUIStyle _logStyle;
        private GUIStyle _promptLogStyle;
        private GUIStyle _promptProcessingStyle;
        private GUIStyle _responseStyle;
        private GUIStyle _promptContextStyle;
        private GUIStyle _chatBoxStyle;
        private GUIStyle _chatBoxAreaStyle;
        private GUIStyle _errorAreaStyle;
        private GUIStyle _statusAreaStyle;
        private GUIStyle _sendButtonStyle;
        private GUIStyle _selectingStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _feedbackStyle;
        private CancellationTokenSource _cancellationTokenSource;
        private Vector2 _scrollPosition = new Vector2(0, 999999);
        private bool _processing;
        private GameObject _lastRootGameObject;

        [MenuItem("Tools/Flexalon Copilot/Chat Window", false, 11)]
        public static void ShowChatWindow()
        {
            ChatWindow window = GetWindow<ChatWindow>(false, "Flexalon Copilot Chat", true);
            window.Show();
        }

        [MenuItem("GameObject/Flexalon Copilot/Set as Root GameObject", false, 9999)]
        public static void SetAsRootGameObject()
        {
            var selected = Selection.activeGameObject;
            if (selected != null)
            {
                var window = GetWindow<ChatWindow>(false, "Flexalon Copilot Chat", true);
                Undo.RecordObject(window, "Set Root GameObject");
                window._rootGameObject = selected;
            }
        }

        [MenuItem("GameObject/Flexalon Copilot/Add To Context", false, 9999)]
        public static void AddGameObjectToPromptContext()
        {
            var selected = Selection.gameObjects;
            if (selected != null)
            {
                foreach (var gameObject in selected)
                {
                    var window = GetWindow<ChatWindow>(false, "Flexalon Copilot Chat", true);
                    Undo.RecordObject(window, "Add GameObject to Context");
                    if (!window._contextObjects.Contains(gameObject))
                    {
                        window._contextObjects.Add(gameObject);
                    }
                }
            }
        }

        private static Color Gray(int gray)
        {
            return new Color(gray / 255f, gray / 255f, gray / 255f);
        }

        private static readonly int _fontSize = 14;

        private void Initialize()
        {
            if (_promptStyle != null) return;

            _promptStyle = new GUIStyle(EditorStyles.textArea);
            _promptStyle.wordWrap = true;

            _logStyle = new GUIStyle(EditorStyles.helpBox);

            _promptLogStyle = new GUIStyle(EditorStyles.textArea);
            _promptLogStyle.wordWrap = true;
            _promptLogStyle.normal.background = new Texture2D(1, 1);
            _promptLogStyle.normal.background.SetPixel(0, 0, Gray(35));
            _promptLogStyle.normal.background.Apply();
            _promptLogStyle.hover.background = _promptLogStyle.normal.background;
            _promptLogStyle.active.background = _promptLogStyle.normal.background;
            _promptLogStyle.focused.background = _promptLogStyle.normal.background;
            _promptLogStyle.fontSize = _fontSize;
            _promptLogStyle.normal.textColor = Gray(170);
            _promptLogStyle.hover.textColor = _promptLogStyle.normal.textColor;
            _promptLogStyle.active.textColor = _promptLogStyle.normal.textColor;
            _promptLogStyle.focused.textColor = _promptLogStyle.normal.textColor;
            _promptLogStyle.margin = new RectOffset(0, 0, 0, 0);
            _promptLogStyle.padding = new RectOffset(20, 20, 15, 15);

            _promptProcessingStyle = new GUIStyle(_promptLogStyle);
            _promptProcessingStyle.normal.background = new Texture2D(1, 1);
            _promptProcessingStyle.normal.background.SetPixel(0, 0, new Color(0.12f, 0.1f, 0.13f));
            _promptProcessingStyle.normal.background.Apply();
            _promptProcessingStyle.hover.background = _promptProcessingStyle.normal.background;
            _promptProcessingStyle.active.background = _promptProcessingStyle.normal.background;
            _promptProcessingStyle.focused.background = _promptProcessingStyle.normal.background;
            _promptProcessingStyle.normal.textColor = Gray(170);

            _responseStyle = new GUIStyle(EditorStyles.textArea);
            _responseStyle.wordWrap = true;
            _responseStyle.normal.background = new Texture2D(1, 1);
            _responseStyle.normal.background.SetPixel(0, 0, Gray(45));
            _responseStyle.normal.background.Apply();
            _responseStyle.hover.background = _responseStyle.normal.background;
            _responseStyle.active.background = _responseStyle.normal.background;
            _responseStyle.focused.background = _responseStyle.normal.background;
            _responseStyle.fontSize = _fontSize;
            _responseStyle.normal.textColor = Gray(170);
            _responseStyle.hover.textColor = _responseStyle.normal.textColor;
            _responseStyle.active.textColor = _responseStyle.normal.textColor;
            _responseStyle.focused.textColor = _responseStyle.normal.textColor;
            _responseStyle.margin = new RectOffset(0, 0, 0, 0);
            _responseStyle.padding = new RectOffset(20, 20, 15, 15);

            _promptContextStyle = new GUIStyle(EditorStyles.helpBox);
            _promptContextStyle.padding = new RectOffset(20, 20, 10, 10);
            _promptContextStyle.normal.background = new Texture2D(1, 1);
            _promptContextStyle.normal.background.SetPixel(0, 0, new Color(0.25f, 0.25f, 0.28f));
            _promptContextStyle.normal.background.Apply();

            _chatBoxStyle = new GUIStyle(EditorStyles.textField);
            _chatBoxStyle.wordWrap = true;
            _chatBoxStyle.normal.background = new Texture2D(1, 1);
            _chatBoxStyle.normal.background.SetPixel(0, 0, new Color(0.12f, 0.1f, 0.13f));
            _chatBoxStyle.normal.background.Apply();
            _chatBoxStyle.hover.background = _chatBoxStyle.normal.background;
            _chatBoxStyle.active.background = _chatBoxStyle.normal.background;
            _chatBoxStyle.focused.background = _chatBoxStyle.normal.background;
            _chatBoxStyle.fontSize = _fontSize;
            _chatBoxStyle.normal.textColor = new Color(236 / 255f, 236 / 255f, 241 / 255f);
            _chatBoxStyle.hover.textColor = _chatBoxStyle.normal.textColor;
            _chatBoxStyle.active.textColor = _chatBoxStyle.normal.textColor;
            _chatBoxStyle.focused.textColor = _chatBoxStyle.normal.textColor;
            _chatBoxStyle.padding = new RectOffset(10, 10, 10, 10);
            _chatBoxStyle.border = new RectOffset(5, 5, 5, 5);

            _chatBoxAreaStyle = new GUIStyle(EditorStyles.helpBox);
            _chatBoxAreaStyle.padding = new RectOffset(20, 20, 10, 10);
            _chatBoxAreaStyle.normal.background = new Texture2D(1, 1);
            _chatBoxAreaStyle.normal.background.SetPixel(0, 0, new Color(0.27f, 0.25f, 0.28f));
            _chatBoxAreaStyle.normal.background.Apply();

            _statusAreaStyle = new GUIStyle(EditorStyles.helpBox);
            _statusAreaStyle.padding = new RectOffset(20, 20, 10, 10);
            _statusAreaStyle.normal.background = new Texture2D(1, 1);
            _statusAreaStyle.normal.background.SetPixel(0, 0, Gray(45));
            _statusAreaStyle.normal.background.Apply();

            _errorAreaStyle = new GUIStyle(EditorStyles.helpBox);
            _errorAreaStyle.padding = new RectOffset(20, 20, 10, 10);
            _errorAreaStyle.normal.background = new Texture2D(1, 1);
            _errorAreaStyle.normal.background.SetPixel(0, 0, new Color(0.35f, 0.25f, 0.25f));
            _errorAreaStyle.normal.background.Apply();

            _errorStyle = new GUIStyle(EditorStyles.label);
            _errorStyle.wordWrap = true;

            _sendButtonStyle = new GUIStyle(EditorStyles.label);
            _sendButtonStyle.fontSize = _fontSize;
            _sendButtonStyle.padding.top = 10;
            _sendButtonStyle.padding.left = 10;
            _sendButtonStyle.padding.right = 10;
            _sendButtonStyle.padding.bottom = 10;
            _sendButtonStyle.normal.background = new Texture2D(1, 1);
            _sendButtonStyle.normal.background.SetPixel(0, 0, new Color(0.12f, 0.1f, 0.13f));
            _sendButtonStyle.normal.background.Apply();
            _sendButtonStyle.hover.background = _sendButtonStyle.normal.background;
            _sendButtonStyle.active.background = _sendButtonStyle.normal.background;
            _sendButtonStyle.focused.background = _sendButtonStyle.normal.background;
            _sendButtonStyle.wordWrap = false;
            _sendButtonStyle.stretchWidth = false;

            _selectingStyle = new GUIStyle(EditorStyles.label);
            _selectingStyle.normal.textColor = Color.yellow;
            _selectingStyle.fontSize = _fontSize;

            _labelStyle = new GUIStyle(EditorStyles.label);
            _labelStyle.fontSize = _fontSize;

            _feedbackStyle = new GUIStyle();
            _feedbackStyle.padding.bottom = 10;
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
            Selection.selectionChanged += Repaint;
            EditorApplication.quitting += OnQuitting;
            EditorApplication.playModeStateChanged += ResetStyle;
            _promptStyle = null;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Selection.selectionChanged -= Repaint;
            EditorApplication.playModeStateChanged -= ResetStyle;
        }

        private void ResetStyle(PlayModeStateChange mode)
        {
            _promptStyle = null;
        }

        private void OnQuitting()
        {
            _undoGroup = 0;
        }

        private void OnUndoRedo()
        {
            _scrollPosition = new Vector2(0, 1000000);
            Repaint();
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Flexalon Copilot is currently not available in play mode.", EditorStyles.boldLabel);
                return;
            }

            Initialize();

            FXGUI.DisableGroup(_processing, () =>
            {
                DrawChatLogSelection();
                DrawPrefabSet();
            });

            EditorGUILayout.Separator();

            if (!_chatLog)
            {
                GUILayout.Label("Please select a chat log.", EditorStyles.boldLabel);
                return;
            }

            if (!_prefabSet)
            {
                GUILayout.Label("Please select a prefab set.", EditorStyles.boldLabel);
                return;
            }

            DrawPromptEntries();

            FXGUI.DisableGroup(_processing, () =>
            {
                DrawPromptContext();
                DrawChatBox();
            });
        }

        private void RepaintAndScrollDown()
        {
            Repaint();
            _scrollPosition = new Vector2(0, 1000000);
            Repaint();
        }

        private Prompt CreatePrompt(ChatLogEntry entry)
        {
            var prompt = new Prompt();
            prompt.Id = entry.Id;
            var gids = new GameObjectIdMap();

            var lastEntry = _chatLog.Entries.Count > 1 ? _chatLog.Entries[_chatLog.Entries.Count - 2] : null;
            if (lastEntry != null)
            {
                prompt.PreviousId = lastEntry.Id;
            }

            prompt.PromptContext = PromptContextFactory.Create(_rootGameObject, _contextObjects, gids);
            var log = new UpdateLog();
            prompt.SceneUpdater = new SceneUpdater(_rootGameObject.transform, _prefabSet, log, gids);
            prompt.SceneUpdater.EnableAnimations();
            _cancellationTokenSource = new CancellationTokenSource();
            prompt.CancellationToken = _cancellationTokenSource.Token;
            prompt.OnUpdate += () =>
            {
                UpdateChatEntryResponse(entry, log);
                RepaintAndScrollDown();
            };

            return prompt;
        }

        private void StartNewUndoGroup()
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Flexalon Copilot Prompt");
            _undoGroup = Undo.GetCurrentGroup();
            _lastRootGameObject = _rootGameObject;
            Undo.RecordObject(this, "Flexalon Copilot Prompt");
        }

        private void AddChatEntry(ChatLogEntry entry)
        {
            Undo.RecordObject(_chatLog, "Flexalon Copilot Prompt");
            _chatLog.Entries.Add(entry);
            RepaintAndScrollDown();
        }

        private void SetProcessing()
        {
            _promptText = "";
            _errorText = "";
            _processing = true;
        }

        public void CreateNewChatLog(bool save)
        {
            _chatLog = ScriptableObject.CreateInstance<ChatLog>();
            _undoGroup = 0;
            _errorText = "";

            if (save)
            {
                var time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var path = Path.Combine("Assets", "FlexalonCopilotChatLogs", "ChatLog_" + time + ".asset");
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                AssetDatabase.CreateAsset(_chatLog, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public async Task SendPromptAsync(string prompt)
        {
            ShowChatWindow();
            _promptText = prompt;
            await SendPromptAsync();
        }

        private void CleanupLastError()
        {
            if (!string.IsNullOrEmpty(_errorText) && _chatLog.Entries.Count > 0 && string.IsNullOrEmpty(_chatLog.Entries.Last().Response))
            {
                _chatLog.Entries.RemoveAt(_chatLog.Entries.Count - 1);
            }
        }

        private async Task SendPromptAsync()
        {
            try
            {
                // Select the Hierarchy window to avoid selecting the prompt anymore, which messes up undo/redo.
                EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");

                StartNewUndoGroup();

                CleanupLastError();

                var entry = new ChatLogEntry()
                {
                    Id = Guid.NewGuid().ToString(),
                    Prompt = _promptText,
                    Response = ""
                };

                AddChatEntry(entry);

                SetProcessing();

                var prompt = CreatePrompt(entry);
                await prompt.SendAsync(entry.Prompt);

                OnPromptComplete(entry, prompt);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private async void SendRepeatAsync()
        {
            try
            {
                var lastChatEntry = _chatLog.Entries.LastOrDefault();
                await UndoAsync();

                StartNewUndoGroup();

                lastChatEntry.Response = "";
                AddChatEntry(lastChatEntry);

                SetProcessing();

                var prompt = CreatePrompt(lastChatEntry);
                prompt.PreviousId = _chatLog.Entries.Count > 1 ? _chatLog.Entries[_chatLog.Entries.Count - 2].Id : null;
                await prompt.SendRepeatAsync(lastChatEntry.Id);

                OnPromptComplete(lastChatEntry, prompt);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private void OnPromptComplete(ChatLogEntry entry, Prompt prompt)
        {
            Undo.RecordObject(this, "Flexalon Copilot Prompt");
            _errorText = prompt.ErrorMessage;
            _processing = false;
            _cancellationTokenSource = null;
            _scrollPosition = new Vector2(0, 1000000);

            Undo.CollapseUndoOperations(_undoGroup);
            Repaint();
        }

        private void UpdateChatEntryResponse(ChatLogEntry entry, UpdateLog log)
        {
            Undo.RecordObject(_chatLog, "Flexalon Copilot Prompt");
            var response = log.TakeLogs();
            if (!string.IsNullOrWhiteSpace(response))
            {
                if (!string.IsNullOrEmpty(entry.Response))
                {
                    entry.Response += "\n";
                }

                entry.Response += response;
            }
        }

        private async Task UndoAsync()
        {
            // Avoid undo in OnGUI because it calls OnGUI.
            await Task.Delay(1);

            Undo.RevertAllDownToGroup(_undoGroup);
            _undoGroup = 0;
            _errorText = "";
        }

        private void FlexibleSelectableLabel(string text, GUIStyle style)
        {
            GUIContent content = new GUIContent(text);
            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(text, style, GUILayout.Height(height));
        }

        private void DrawChatLogSelection()
        {
            EditorGUILayout.Space();
            FXGUI.Horizontal(() =>
            {
                var newChatLog = (ChatLog)EditorGUILayout.ObjectField("Chat Log", _chatLog, typeof(ChatLog), false);
                if (newChatLog != _chatLog)
                {
                    _chatLog = newChatLog;
                    _undoGroup = 0;
                    RepaintAndScrollDown();
                }

                if (GUILayout.Button("New", GUILayout.Width(75)))
                {
                    CreateNewChatLog(true);
                }
            });
        }

        private void DrawPrefabSet()
        {
            _prefabSet = (PrefabSet)EditorGUILayout.ObjectField("Prefab Set", _prefabSet, typeof(PrefabSet), false);
            if (_prefabSet == null)
            {
                _prefabSet = PrefabSet.Default;
            }
        }

        private static readonly string _noResponse = "Something went wrong interpretting Copilot's response. Please try again.";

        private void DrawPromptEntries()
        {
            _scrollPosition = FXGUI.Scroll(_scrollPosition, _logStyle, () =>
            {
                foreach (var entry in _chatLog.Entries.Take(_chatLog.Entries.Count - 1))
                {
                    FlexibleSelectableLabel(entry.Prompt.Trim(), _promptLogStyle);
                    var response = entry.Response.Trim();
                    FlexibleSelectableLabel(!string.IsNullOrEmpty(response) ? response : _noResponse, _responseStyle);
                    DrawFeedback(entry);
                }

                if (_chatLog.Entries.Count > 0)
                {
                    FlexibleSelectableLabel(_chatLog.Entries.Last().Prompt.Trim(), _processing ? _promptProcessingStyle : _promptLogStyle);
                    var newResponse = _chatLog.Entries.Last().Response.Trim();
                    if (!_processing || !string.IsNullOrEmpty(newResponse))
                    {
                        FlexibleSelectableLabel(!string.IsNullOrEmpty(newResponse) ? newResponse : _noResponse, _responseStyle);
                    }

                    if (!_processing && !string.IsNullOrEmpty(newResponse))
                    {
                        DrawFeedback(_chatLog.Entries.Last());
                    }
                }

                DrawProcessing();
                DrawRetry();
                DrawError();
            });
        }

        private void DrawFeedback(ChatLogEntry entry)
        {
            FXGUI.Horizontal(_feedbackStyle, () =>
            {
                GUILayout.FlexibleSpace();
                if (FXGUI.ImageButton("d77f7e6041730b8448081abf9df10a90", 32, 32))
                {
                    FeedbackWindow.ShowFeedbackWindow(entry.Id, true);
                }

                if (FXGUI.ImageButton("a2567f3ff7b0ebc4db2d208eaaea7604", 32, 32))
                {
                    FeedbackWindow.ShowFeedbackWindow(entry.Id, false);
                }
            });
        }

        private void DrawPromptContext()
        {
            FXGUI.Vertical(_promptContextStyle, () =>
            {
                FXGUI.Horizontal(() =>
                {
                    EditorGUILayout.LabelField("Prompt Context");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear All", GUILayout.Width(75)))
                    {
                        _rootGameObject = null;
                        _contextObjects.Clear();
                        Selection.activeObject = null;
                    }
                });

                EditorGUI.indentLevel++;
                {
                    GUILayout.Space(10);
                    _rootGameObject = (GameObject)EditorGUILayout.ObjectField("Root GameObject", _rootGameObject, typeof(GameObject), true);
                    GUILayout.Space(10);

                    if (_contextObjects.Any())
                    {
                        for (int i = 0; i < _contextObjects.Count; i++)
                        {
                            FXGUI.Horizontal(() =>
                            {
                                _contextObjects[i] = (UnityEngine.Object)EditorGUILayout.ObjectField(_contextObjects[i], typeof(UnityEngine.Object), true);
                                if (GUILayout.Button("Remove", GUILayout.Width(75)))
                                {
                                    _contextObjects.RemoveAt(i);
                                    i--;
                                }
                            });
                        }
                    }

                    FXGUI.Horizontal(() =>
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add", GUILayout.Width(75)))
                        {
                            _contextObjects.Add(null);
                        }
                    });

                    if (_rootGameObject)
                    {
                        var selectedGameObjectsUnderRoot = Selection.gameObjects.Where(go => go != _rootGameObject && go.transform.IsChildOf(_rootGameObject.transform)).Select(go => go.name);
                        if (selectedGameObjectsUnderRoot.Any())
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Selecting GameObjects: " + string.Join(", ", selectedGameObjectsUnderRoot), _selectingStyle);
                        }

                        var selectedAssets = Selection.objects.Where(o => PromptContextFactory.IsSupportedAsset(o));
                        if (selectedAssets.Any())
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Selecting Assets: " + string.Join(", ", selectedAssets), _selectingStyle);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            });
        }

        private void DrawProcessing()
        {
            if (!_processing)
            {
                return;
            }

            FXGUI.Horizontal(_statusAreaStyle, () =>
            {
                GUILayout.Label("Processing...");
                if (GUILayout.Button("Stop", GUILayout.Width(75)))
                {
                    _cancellationTokenSource?.Cancel();
                    _processing = false;
                }
            });

        }

        private void DrawRetry()
        {
            if (_processing || !_lastRootGameObject || _undoGroup == 0 || _chatLog.Entries.Count == 0)
            {
                return;
            }

            FXGUI.Horizontal(_statusAreaStyle, () =>
            {
                GUILayout.Label("Caution: This will undo everything since the prompt was sent.");
                if (GUILayout.Button("Retry", GUILayout.Width(75)))
                {
                    UndoAsync().ConfigureAwait(false);
                }

                #if FLEXALON_COPILOT_TEST
                    if (GUILayout.Button("Repeat", GUILayout.Width(75)))
                    {
                        SendRepeatAsync();
                    }
                #endif
            });

        }

        private void DrawError()
        {
            if (string.IsNullOrWhiteSpace(_errorText))
            {
                return;
            }

            FXGUI.Horizontal(_errorAreaStyle, () =>
            {
                FlexibleSelectableLabel("There was an error: \n" + _errorText, _errorStyle);
            });
        }

        private void DrawChatBox()
        {
            FXGUI.Vertical(_chatBoxAreaStyle, () =>
            {
                if (!Api.Instance.IsLoggedIn)
                {
                    GUILayout.Label("Please log in to use Flexalon Copilot.", EditorStyles.boldLabel);
                    if (GUILayout.Button("Log In", GUILayout.Width(75)))
                    {
                        StartScreen.ShowStartScreen();
                    }
                }
                else if (!_rootGameObject)
                {
                    GUILayout.Label("Please set a Root GameObject.", EditorStyles.boldLabel);
                }
                else
                {
                    Event e = Event.current;
                    bool enterPressed = e.type == EventType.KeyDown &&
                        (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) &&
                        e.modifiers != EventModifiers.Shift;

                    if (enterPressed)
                    {
                        e.Use();
                    }

                    // EditorGUILayout.LabelField("Enter a prompt", _labelStyle);

                    FXGUI.Horizontal(() =>
                    {
                        _promptText = GUILayout.TextArea(_promptText, _chatBoxStyle);
                        GUILayout.Space(1);
                        if (!_processing && (GUILayout.Button("Send", _sendButtonStyle) || enterPressed) && _promptText?.Trim().Length > 3)
                        {
                            SendPromptAsync().ConfigureAwait(false);
                        }
                    });
                }
            });
        }
    }
}

#endif