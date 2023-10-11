#if UNITY_TMPRO && UNITY_UI

using System;
using UnityEditor;
using UnityEngine;

namespace FlexalonCopilot.Editor
{
    internal class LoginWindow
    {
        public event Action StateChanged;

        private enum State
        {
            Email,
            Code,
            Verifying,
            Done,
            Error
        }

        private string _email;
        private string _code;
        private string _errorMessage = "Something went wrong. Please try again later.";
        private State _state = State.Email;

        private GUIStyle _boldStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _warningStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _buttonStyle;
        private bool _enterPressed;
        private string _emailForCode;

        public LoginWindow(GUIStyle boldStyle, GUIStyle bodyStyle, GUIStyle warningStyle)
        {
            _boldStyle = boldStyle;
            _bodyStyle = bodyStyle;
            _warningStyle = warningStyle;

            _inputStyle = new GUIStyle(EditorStyles.textField);
            _inputStyle.fontSize = 20;
            _inputStyle.margin = new RectOffset(10, 10, 10, 10);
            _inputStyle.padding = new RectOffset(10, 10, 0, 0);
            _inputStyle.alignment = TextAnchor.MiddleLeft;

            _buttonStyle = new GUIStyle(_bodyStyle);
            _buttonStyle.fontSize = 14;
            _buttonStyle.margin.bottom = 5;
            _buttonStyle.padding.top = 5;
            _buttonStyle.padding.left = 10;
            _buttonStyle.padding.right = 10;
            _buttonStyle.padding.bottom = 5;
            _buttonStyle.hover.background = Texture2D.grayTexture;
            _buttonStyle.hover.textColor = Color.white;
            _buttonStyle.active.background = Texture2D.grayTexture;
            _buttonStyle.active.textColor = Color.white;
            _buttonStyle.focused.background = Texture2D.grayTexture;
            _buttonStyle.focused.textColor = Color.white;
            _buttonStyle.normal.background = Texture2D.grayTexture;
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.wordWrap = false;
            _buttonStyle.stretchWidth = false;
        }

        public void Draw()
        {
            _enterPressed = Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            if (_enterPressed)
            {
                Event.current.Use();
            }

            switch (_state)
            {
                case State.Email:
                    EmailState();
                    break;
                case State.Code:
                    CodeState();
                    break;
                case State.Verifying:
                    VeryfyingState();
                    break;
                case State.Done:
                    DoneState();
                    break;
                case State.Error:
                    ErrorState();
                    break;
            }
        }

        private void SetState(State state)
        {
            _state = state;
            _email = "";
            _code = "";
            StateChanged?.Invoke();
        }

        private void EmailState()
        {
            GUILayout.Label("Email", _bodyStyle);
            _email = EditorGUILayout.TextField(_email, _inputStyle, GUILayout.Height(40));
            GUILayout.Label("We will send a one-time code to this email.", _bodyStyle);
            EditorGUILayout.Space();
            GUI.SetNextControlName("LoginNextButton");
            if (_enterPressed || GUILayout.Button("Next", _buttonStyle))
            {
                GUI.FocusControl("LoginNextButton");
                _emailForCode = _email;
                SendEmail();
                SetState(State.Code);
            }
        }

        private async void SendEmail()
        {
            try
            {
                await Api.Instance.SignupAsync(_email);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                _errorMessage = e.Message;
                SetState(State.Error);
            }
        }

        private void CodeState()
        {
            GUILayout.Label("Please enter the code you received by email", _bodyStyle);
            _code = EditorGUILayout.TextField(_code, _inputStyle, GUILayout.Height(40));
            FXGUI.Horizontal(() =>
            {
                if (GUILayout.Button("Back", _buttonStyle))
                {
                    SetState(State.Email);
                }

                GUI.SetNextControlName("LoginCodeButton");
                if (_enterPressed || GUILayout.Button("Login", _buttonStyle))
                {
                    GUI.FocusControl("LoginCodeButton");
                    SendLogin();
                    SetState(State.Verifying);
                }
            });
        }

        private async void SendLogin()
        {
            try
            {
                var sendCode = _code;
                SetState(State.Verifying);
                await Api.Instance.LoginAsync(_emailForCode, sendCode);
                SetState(State.Done);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                _errorMessage = e.Message;
                SetState(State.Error);
            }
        }

        private void VeryfyingState()
        {
            GUILayout.Label("Verifying your code...", _bodyStyle);
        }

        private void DoneState()
        {
            GUILayout.Label("You are now logged in", _bodyStyle);
        }

        private void ErrorState()
        {
            GUILayout.Label(_errorMessage, _warningStyle);
            EditorGUILayout.Space();
            if (GUILayout.Button("Try again", _buttonStyle))
            {
                SetState(State.Email);
            }
        }
    }
}

#endif