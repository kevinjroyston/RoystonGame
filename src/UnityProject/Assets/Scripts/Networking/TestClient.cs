﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Assets.Scripts.Networking.DataModels;
using Newtonsoft.Json;
using Assets.Scripts.Networking.DataModels.Enums;
using Newtonsoft.Json.Linq;
using static UnityEngine.UI.GridLayoutGroup;
using Assets.Scripts.Networking.DataModels.UnityObjects;
using System.Collections.Specialized;

/// <summary>
/// This class opens a connection to the server and listens for updates. From the main thread the secondary connection thread is
/// monitored and restarted as needed. Any updates from the secondary thread will get merged back to the main thread in the update
/// loop before leaving this class.
/// </summary>
public class TestClient : MonoBehaviour
{
    private SignalRLib srLib;

    private const string ClientVersion = "2.1.0";
    private Task hubTask;

    private List<string> handlers = new List<string>() { "ConfigureMetadata", "UpdateState", "LobbyClose" };
    private string signalRHubURL = "";

    private bool Connected { get; set; } = false;

    // Hacky fix to send the update from the main thread.
    private bool Dirty { get; set; } = false;
    private bool LobbyClosed { get; set; } = false;
    private UnityView CurrentView { get; set; }

    private bool ConfigDirty { get; set; } = false;
    private ConfigurationMetadata ConfigurationMeta { get; set; }

    private bool Restarting { get; set; } = false;
    public EnterLobbyId EnterLobbyId;

    /// <summary>
    /// Set up the connection and callbacks.
    /// </summary>
    void Awake()
        {
#if DEBUG
        signalRHubURL = "http://localhost:50403/signalr";

        //signalRHubURL="https://api.test.scrawlbrawl.tv/signalr";

#else
        signalRHubURL="https://api.scrawlbrawl.tv/signalr";
#endif
#if UNITY_WEBGL
        
        if (Application.absoluteURL.Contains("localhost") || Application.absoluteURL=="")
        {
            signalRHubURL = "http://localhost:50403/signalr";
        }
        else if (Application.absoluteURL.Contains("test.")) {
            signalRHubURL="https://api.test.scrawlbrawl.tv/signalr";
        }
        else{
            signalRHubURL = "https://api.scrawlbrawl.tv/signalr";
        }
#endif

        Debug.Log("URL:"+Application.absoluteURL);
        srLib = new SignalRLib(signalRHubURL, handlers, true);

        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;  // VSync must be disabled

#if UNITY_WEBGL
        Application.targetFrameRate = -1;  // https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
#else
        Application.targetFrameRate = 60;
#endif

        srLib.ConnectionStarted += (object sender, ConnectionEventArgs e) =>
        {
            Debug.Log(e.ConnectionId);
            Connected = true;  // just a flag we are using to know we connected, does not ensure we have not been disconnected

            Uri uri = new Uri(Application.absoluteURL);
            string[] lobby = uri.Query.Split(new string[]{"lobby="},StringSplitOptions.None);
            if (lobby.Length==2 && lobby[1].Length > 0)
            {
                ConnectToLobby(lobby[1].Split('&')[0].Truncate(10));
            }

        };

        srLib.HandlerInvoked += (object sender, HandlerEventArgs e) =>
        {
            Debug.Log("handler invoked");

            switch (e.HandlerName)
            {
                case "ConfigureMetadata":
                    ConfigurationMeta = JsonConvert.DeserializeObject<ConfigurationMetadata>(e.Payload);
                    ConfigDirty = true;
                    break;
                case "UpdateState":
                    try {
                        CurrentView = ParseJObjects(JsonConvert.DeserializeObject<UnityView>(e.Payload));
                    }
                    catch (Exception err)
                    {
                        Debug.Log(err.Message);
                    }
                    Dirty = true;
                    break;
                case "LobbyClose":
                    LobbyClosed = true;
                    Dirty = true;
                    break;
                default:
                    Debug.Log($"Handler: '{e.HandlerName}' not defined");
                    break;
            }
        };

        // plr ConnectToHub();
    }

        /// <summary>
        /// Iterates through all "object" dictionaries and parses objects.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public UnityView ParseJObjects(UnityView view)
        {
            foreach (UnityViewOptions key in view?.Options?.Keys?.ToList() ?? new List<UnityViewOptions>())
            {
                switch (key)
                {
                    case UnityViewOptions.BlurAnimate:
                        view.Options[key] = ((JObject)view.Options[key]).ToObject<UnityField<float?>>();
                        break;
                    default:
                        break;
                }
            }

            if (view.UnityObjects?.Value != null)
            {
                List<UnityObject> unityObjects = new List<UnityObject>();
                foreach (object obj in view.UnityObjects.Value)
                {
                    JObject jObject = (JObject)obj;
                    switch (jObject["Type"].ToObject<UnityObjectType>())
                    {
                        case UnityObjectType.Image:
                            unityObjects.Add(jObject.ToObject<UnityImage>());
                            break;
                        case UnityObjectType.Slider:
                            unityObjects.Add(jObject.ToObject<UnitySlider>());
                            break;
                        case UnityObjectType.Text:
                            unityObjects.Add(jObject.ToObject<UnityText>());
                            break;
                        default:
                            throw new NotImplementedException("Not implemented");
                    }
                }
            view.UnityObjects.Value = unityObjects.Cast<object>().ToList();
            }
            if (view.UnityObjects?.StartValue != null || view.UnityObjects?.EndValue !=null)
            {
                throw new NotImplementedException("not implemented");
            }

            return view;
        }

        string LobbyId = null;
        public void ConnectToLobby(string lobby)
        {
            LobbyId = lobby;
            srLib.SendToHub("ConnectWebLobby", LobbyId+"-"+ClientVersion);
        }

        

    public void Start()
    {
        #region ugly
        string noneUnityImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAWgAAAFoCAYAAAB65WHVAAAACXBIWXMAAC4jAAAuIwF4pT92AAAF8WlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0MzYwLCAyMDIwLzAyLzEzLTAxOjA3OjIyICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdEV2dD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlRXZlbnQjIiB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iIHhtbG5zOnBob3Rvc2hvcD0iaHR0cDovL25zLmFkb2JlLmNvbS9waG90b3Nob3AvMS4wLyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMSAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIwLTA3LTIzVDE2OjU4OjQ4LTA3OjAwIiB4bXA6TWV0YWRhdGFEYXRlPSIyMDIwLTA3LTIzVDE2OjU4OjQ4LTA3OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMC0wNy0yM1QxNjo1ODo0OC0wNzowMCIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo5NDNkZTk2Zi00NGFlLTJkNDctYTJiYy0yOTRlNGExNGVmYmUiIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDphNzhlOTRlZC03YzdiLWI5NGItYmQzZC01NmYzNWE3YjJjODYiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDpkZjVjODM1OC00YzRiLWM5NGItYjdjMS05NjM0MTkwNjVhNmEiIGRjOmZvcm1hdD0iaW1hZ2UvcG5nIiBwaG90b3Nob3A6Q29sb3JNb2RlPSIzIiBwaG90b3Nob3A6SUNDUHJvZmlsZT0ic1JHQiBJRUM2MTk2Ni0yLjEiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOmRmNWM4MzU4LTRjNGItYzk0Yi1iN2MxLTk2MzQxOTA2NWE2YSIgc3RFdnQ6d2hlbj0iMjAyMC0wNy0yM1QxNjo1ODo0OC0wNzowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIxLjEgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDo5NDNkZTk2Zi00NGFlLTJkNDctYTJiYy0yOTRlNGExNGVmYmUiIHN0RXZ0OndoZW49IjIwMjAtMDctMjNUMTY6NTg6NDgtMDc6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4xIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz53sUbsAAAoDklEQVR4nO3de7hWZZ3/8feGvREVUFBRBxVQUTwf8HgZnipPlI4aOs10tqayqaYym2l+pZ3m95sxrS4rLbM0J8u0nE6eStPwbIpKKogliIYcAhQPIBv274/v88gG9t7sw/e77nut9Xld17pA1O+64Vnrw3rudR9aOjo6EBGR/AxK3QAREemaAlpEJFMKaBGRTCmgRUQypYAWEcmUAlpEJFMKaBGRTCmgRUQypYAWEcmUAlpEJFMKaBGRTCmgRUQypYAWEcmUAlpEJFMKaBGRTCmg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";
        #endregion
        #region Debug Unity View 
        /// This is paired with ViewManager.SetDebugCustomView() to allow testing of views without having to do anything on backend
        /// To use uncomment out this code, modify the UnityView being passed in and COMMENT OUT THE UPDATE LOOP
        /// When you are done please revert back the comments
        /// ====================================================================================
        /// THIS CODE IS ONLY FOR DEBUGGING PURPOSES AND SHOULD NOT BE CALLED EVER ON PRODUCTION
        /// ====================================================================================
        
        /*
        List<UnityUser> fakeUsers = new List<UnityUser>()
        {
            new UnityUser()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User 1",
                SelfPortrait = noneUnityImage,
                Status = UserStatus.AnsweringPrompts,
            },
            new UnityUser()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User 2",
                Activity = UserActivity.Active,
                SelfPortrait = noneUnityImage,
                Status = UserStatus.Waiting,
            },
            new UnityUser()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User 3",
                Activity = UserActivity.Active,
                SelfPortrait = noneUnityImage,
                Status = UserStatus.Waiting,
            },
            new UnityUser()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User 4",
                Activity = UserActivity.Active,
                SelfPortrait = noneUnityImage,
                Status = UserStatus.Waiting,
            },
        };
            ViewManager.Singleton.SetDebugCustomView(
                TVScreenId.HorizontalObjectView,
                new UnityView()
                {
                    UnityObjects = new UnityField<IReadOnlyList<object>>
                    {
                        Value = new List<UnityTextStack>()
                        {
                            new UnityTextStack()
                            {
                                OwnerUserId = Guid.NewGuid(),
                                Type = UnityObjectType.TextStack,
                                Title = new UnityField<string>()
                                {
                                    Value = "Hints"
                                },
                                FixedNumItems = 10,
                                TextStackList = new UnityField<IReadOnlyList<StackItemHolder>>()
                                {
                                    Value = new List<StackItemHolder>()
                                    {
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 1",
                                            Owner = fakeUsers[0],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 2",
                                            Owner = fakeUsers[1],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 3",
                                            Owner = fakeUsers[2],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 4",
                                            Owner = fakeUsers[3],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 5",
                                            Owner = fakeUsers[2],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 6",
                                            Owner = fakeUsers[1],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 7",
                                            Owner = fakeUsers[0],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 8",
                                            Owner = fakeUsers[0],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 9",
                                            Owner = fakeUsers[0],
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Hint 10",
                                            Owner = fakeUsers[0],
                                        },
                                    },
                                    Options = new Dictionary<UnityFieldOptions, object>()
                                    {
                                        {UnityFieldOptions.ScrollingTextStack, true }
                                    }
                                },
                                
                                
                                UnityObjectId = Guid.NewGuid(),
                            },
                            new UnityTextStack()
                            {
                                OwnerUserId = Guid.NewGuid(),
                                Type = UnityObjectType.TextStack,
                                Title = new UnityField<string>()
                                {
                                    Value = "Guesses"
                                },
                                //FixedNumItems = 10,
                                TextStackList = new UnityField<IReadOnlyList<StackItemHolder>>()
                                {
                                    Value = new List<StackItemHolder>()
                                    {
                                        new StackItemHolder()
                                        {
                                            Text = "Oops! <color=red>User</color> was tricked!",
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Real: <color=green>Real</color>",
                                        },
                                        new StackItemHolder()
                                        {
                                            Text = "Trap: <color=red>Fake</color>",
                                        },
                                    },
                                    Options = new Dictionary<UnityFieldOptions, object>()
                                    {
                                        {UnityFieldOptions.ScrollingTextStack, false }
                                    }
                                },
                                UnityObjectId = Guid.NewGuid(),
                            },
                        }
                    },
                    Users = fakeUsers,
                    Title = new UnityField<string>()
                    {
                        Value = "Test Title"
                    },
                    Instructions = new UnityField<string>()
                    {
                        Value = "Test Instructions"
                    },
                    ServerTime = DateTime.UtcNow,
                    StateEndTime = null,
                    IsRevealing = true,
                    Options = new Dictionary<UnityViewOptions, object>()
                });
            */
             
        #endregion
    }
    
    
    public void Update()
    {
        // If the Dirty bit is set that means the networking thread got a response from the server. Since it is not possible
        // to make certain types of calls outside of the main thread we listen for it here and make the call here.
        if (Dirty)
        {
            // If the Dirty bit is set that means the networking thread got a response from the server. Since it is not possible
            // to make certain types of calls outside of the main thread we listen for it here and make the call here.
            if (Dirty)
            {
                Debug.Log($"Server update");
                Dirty = false;
                if (LobbyClosed)
                {
                    LobbyClosed = false;
                    ViewManager.Singleton.OnLobbyClose();

                }
                else
                {
                    ViewManager.Singleton.SwitchToView(CurrentView?.ScreenId ?? TVScreenId.Unknown, CurrentView);
                }
            }

            if (ConfigDirty)
            {
                Debug.Log($"Config Update");
                ConfigDirty = false;
                ViewManager.Singleton.UpdateConfigMetaData(ConfigurationMeta);
            }

            // If we aren't in the process of a delayed restart and the connection task failed. Begin a delayed restart.
            // plr-old if (!Restarting && (hubConnection?.State != HubConnectionState.Connected || hubTask==null || hubTask.IsFaulted || hubTask.IsCanceled))
            // NOTE: the Connected in the line below only says we EVER connected, not that we are still connected
            if (!Restarting && (Connected || hubTask == null || hubTask.IsFaulted || hubTask.IsCanceled))
            {
                StartCoroutine(DelayedConnectToHub());
            }
        } 

        /// <summary>
        /// Restarts the connection after a 5 second delay.
        /// </summary>
        /// <returns>A coroutine representing the task.</returns>
        IEnumerator DelayedConnectToHub()
        {
            Restarting = true;
            yield return new WaitForSeconds(5);
            ConnectToHub();
            Restarting = false;
        }
    } 
    

        public void OnApplicationQuit()
        {
        }

        private void ConnectToHub()
        {
            if(srLib == null)
            {
                return;
            }

            if(hubTask!=null && !(Connected || hubTask.IsFaulted || hubTask.IsCanceled))
            {
                Debug.Log("Hub restart requested but connection is active");
                return;
            }
        }
 
}
