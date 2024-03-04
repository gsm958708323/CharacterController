#if UNITY_TMPRO && UNITY_UI

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FlexalonCopilot.Editor
{
    internal class Prompt
    {
        private string _errorMessage;
        public string ErrorMessage => _errorMessage;

        public event Action OnUpdate;

        public string Id;
        public string PreviousId;
        public PromptContext PromptContext;
        public SceneUpdater SceneUpdater;
        public CancellationToken CancellationToken;

        public async Task SendAsync(string prompt)
        {
            await SendAndCatchErrors(async () =>
            {
                await Api.Instance.SendPromptAsync(
                    Id, PreviousId, prompt, PromptContext, CancellationToken, OnData);
            });
        }

        public async Task SendTestAsync(string gptResponse)
        {
            await SendAndCatchErrors(async () =>
            {
                await Api.Instance.SendPromptTestAsync(gptResponse, PromptContext, OnData);
            });
        }

        public async Task SendRepeatAsync(string promptId)
        {
            await SendAndCatchErrors(async () =>
            {
                await Api.Instance.SendPromptRepeatAsync(promptId, PreviousId, PromptContext, CancellationToken, OnData);
            });
        }

        public async Task SendAndCatchErrors(Func<Task> sendAction)
        {
            _errorMessage = "";

            try
            {
                await sendAction();
            }
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                _errorMessage = ex.Message;
            }
            finally
            {
                await SceneUpdater.PostUpdate();
            }
        }

        private void OnData(PromptResponseData data)
        {
            try
            {
                ProcessResponse(SceneUpdater, data);
                OnUpdate?.Invoke();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private void ProcessResponse(SceneUpdater sceneUpdater, PromptResponseData data)
        {
            switch (data.cmd)
            {
                case "create":
                    sceneUpdater.CreateGameObject(data.id, data.a, data.b);
                    break;
                case "move":
                    sceneUpdater.MoveGameObject(data.id, data.a, data.b);
                    break;
                case "destroy":
                    sceneUpdater.DestroyGameObject(data.id);
                    break;
                case "addComponent":
                    sceneUpdater.AddComponent(data.id, data.a, data.b);
                    break;
                case "removeComponent":
                    sceneUpdater.RemoveComponent(data.id);
                    break;
                case "setProperty":
                    sceneUpdater.SetComponentProperty(data.id, data.a, data.b);
                    break;
                case "clearProperty":
                    sceneUpdater.ClearComponentProperty(data.id, data.a);
                    break;
                case "setStyleProperty":
                    sceneUpdater.SetStyleProperty(data.id, data.a, data.b, data.c);
                    break;
                case "clearStyleProperty":
                    sceneUpdater.ClearStyleProperty(data.id, data.a, data.b);
                    break;
            }

            sceneUpdater.PostCommand();
        }
    }
}

#endif