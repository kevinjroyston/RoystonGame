﻿using Backend.GameInfrastructure.DataModels.Users;
using Backend.GameInfrastructure.DataModels.Enums;
using Backend.GameInfrastructure.DataModels.States.GameStates;
using Backend.GameInfrastructure.DataModels.States.StateGroups;
using Backend.GameInfrastructure.Extensions;
using Backend.Games.BriansGames.BattleReady;
using Backend.Games.BriansGames.BodyBuilder;
using Backend.Games.BriansGames.TwoToneDrawing;
using Backend.APIs.DataModels;
using Backend.APIs.DataModels.Exceptions;
using Common.DataModels.Requests;
using Common.DataModels.Requests.LobbyManagement;
using Common.DataModels.Responses;
using Backend.APIs.DataModels.UnityObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static System.FormattableString;
using Backend.GameInfrastructure.DataModels;
using Backend.Games.KevinsGames.Mimic;
using Backend.Games.TimsGames.FriendQuiz;
using Backend.Games.BriansGames.ImposterDrawing;
using Backend.APIs.DataModels.Enums;
using Common.Code.Extensions;
namespace Backend.GameInfrastructure
{
    public class Lobby : IInlet
    {
        /// <summary>
        /// An email address denoting what authenticated user created this lobby.
        /// </summary>
        public AuthenticatedUser Owner { get; }
        private ConcurrentBag<User> UsersInLobby { get; } = new ConcurrentBag<User>();
        public string LobbyId { get; }

        /// <summary>
        /// Used for monitoring lobby age.
        /// </summary>
        public DateTime CreationTime { get; } = DateTime.Now;
        public List<ConfigureLobbyRequest.GameModeOptionRequest> GameModeOptions { get; private set; }
        public GameModeMetadataHolder SelectedGameMode { get; private set; }

        #region GameStates
        private GameState CurrentGameState { get; set; }
        private GameState EndOfGameRestart { get; set; }
        private WaitForLobbyCloseGameState WaitForLobbyStart { get; set; }
        #endregion

        private IGameMode Game { get; set; }
        private GameManager GameManager { get; set; }


        #region GameModes
        public static IReadOnlyList<GameModeMetadataHolder> GameModes { get; } = new List<GameModeMetadataHolder>
        {
            #region Imposter Syndrome (OLD Removed)
            /*
            new GameModeMetadata
            {
                Title = "Imposter Syndrome",
                Description = "Come up with a difference only you'll be able to spot!",
                MinPlayers = 4,
                MaxPlayers = null,
                GameModeInstantiator = (lobby, options) => new OneOfTheseThingsIsNotLikeTheOtherOneGameMode(lobby, options),
                Options = new List<GameModeOptionResponse>
                {
                    new GameModeOptionResponse
                    {
                        Description = "Max total drawings per player.",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 6,
                        MinValue = 3
                    },
                }
            },*/
            #endregion
            #region Imposter Syndrome Text (Removed)
            /*new GameModeMetadata
            {
                Title = "Imposter Syndrome (Text)",
                Description = "Come up with a difference only you'll be able to spot!",
                MinPlayers = 4,
                MaxPlayers = null,
                GameModeInstantiator = (lobby, options) => new ImposterTextGameMode(lobby, options),
                Options = new List<GameModeOptionResponse>
                {
                    new GameModeOptionResponse
                    {
                        Description = "Length of the game (10 for longest 1 for shortest 0 for no timer)",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 5,
                        MinValue = 0,
                        MaxValue = 10,
                    }
                }
            },*/
            #endregion
            new GameModeMetadataHolder()
            {
                GameModeMetadata = ImposterDrawingGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new ImposterDrawingGameMode(lobby, options)
            },
            new GameModeMetadataHolder()
            {
                GameModeMetadata = TwoToneDrawingGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new TwoToneDrawingGameMode(lobby, options)
            },
            new GameModeMetadataHolder()
            {
                GameModeMetadata = BodyBuilderGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new BodyBuilderGameMode(lobby, options)
            },
            new GameModeMetadataHolder()
            {
                GameModeMetadata = BattleReadyGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new BattleReadyGameMode(lobby, options)
            },
            new GameModeMetadataHolder()
            {
                GameModeMetadata = MimicGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new MimicGameMode(lobby, options)
            },
            new GameModeMetadataHolder()
            {
                GameModeMetadata = FriendQuizGameMode.GameModeMetadata,
                GameModeInstantiator = (lobby, options) => new FriendQuizGameMode(lobby, options)
            },
            
            #region StoryTime (Removed)
            /*new GameModeMetadata
            {
                Title = "StoryTime",
                Description = "Work together to make the best story that fits set of rapidly changing genres",
                MinPlayers = 3,
                MaxPlayers = null,
                GameModeInstantiator = (lobby, options) => new StoryTimeGameMode(lobby, options),
                Options = new List<GameModeOptionResponse>
                {
                    new GameModeOptionResponse
                    {
                        Description = "Number of players asked to write each round",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 2,
                        MinValue = 2,
                        MaxValue = 30,
                    },
                    new GameModeOptionResponse
                    {
                        Description = "Number of rounds",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 10,
                        MinValue = 2,
                        MaxValue = 60,
                    },
                    new GameModeOptionResponse
                    {
                        Description = "length of timer for writing",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 45,
                        MinValue = 10,
                        MaxValue = 120,
                    },
                    new GameModeOptionResponse
                    {
                        Description = "length of timer for voting",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 30,
                        MinValue = 5,
                        MaxValue = 60,
                    },
                    new GameModeOptionResponse
                    {
                        Description = "character limit for sentences",
                        ResponseType = ResponseType.Integer,
                        DefaultValue = 100,
                        MinValue = 50,
                        MaxValue = 200,
                    },
                }
            },*/
            #endregion
        }.AsReadOnly();
        #endregion

        public Lobby(string friendlyName, AuthenticatedUser owner, GameManager gameManager)
        {
            this.LobbyId = friendlyName;
            this.Owner = owner;
            this.GameManager = gameManager;
            InitializeAllGameStates();
        }

        /// <summary>
        /// Lobby is open if there is not a game instantiated already and there is still space based on currently selected game mode.
        /// </summary>
        /// <returns>True if the lobby is accepting new users.</returns>
        public bool IsLobbyOpen()
        {
            // Either a game mode hasn't been selected, or the selected gamemode is not at its' capacity.
            return !IsGameInProgress() && (this.SelectedGameMode == null || this.SelectedGameMode.GameModeMetadata.IsSupportedPlayerCount(this.UsersInLobby.Count, ignoreMinimum: true));
        }
        public bool IsGameInProgress()
        {
            return this.Game != null;
        }
        public GameState GetCurrentGameState()
        {
            return this.CurrentGameState;
        }

        public void CloseLobbyWithError(Exception error = null)
        {
            GameManager.ReportGameError(type: ErrorType.GetContent, lobbyId: LobbyId, error: error);
        }
        public bool ConfigureLobby(ConfigureLobbyRequest request, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (IsGameInProgress())
            {
                // TODO: this might need updating for replay logic.
                errorMsg = "Cannot change configuration lobby while game is in progress!";
                return false;
            }

            if (request?.GameMode == null || request.GameMode.Value < 0 || request.GameMode.Value >= Lobby.GameModes.Count)
            {
                errorMsg = "Unsupported Game Mode";
                return false;
            }

            // Don't check player minimum count when configuring, but do check on start.
            if (!GameModes[request.GameMode.Value].GameModeMetadata.IsSupportedPlayerCount(GetAllUsers().Count, ignoreMinimum: true))
            {
                errorMsg = Invariant($"Selected game mode has following restrictions: {GameModes[request.GameMode.Value].GameModeMetadata.RestrictionsToString()}");
                return false;
            }

            IReadOnlyList<GameModeOptionResponse> requiredOptions = GameModes[request.GameMode.Value].GameModeMetadata.Options;
            if (request?.Options == null || request.Options.Count != requiredOptions.Count)
            {
                errorMsg = "Wrong number of options provided for selected game mode.";
                return false;
            }

            for (int i = 0; i < requiredOptions.Count; i++)
            {
                if(!request.Options[i].ParseValue(requiredOptions[i], out errorMsg))
                {
                    return false;
                }
            }

            this.SelectedGameMode = GameModes[request.GameMode.Value];
            this.GameModeOptions = request.Options;

            return true;
        }

        /// <summary>
        /// UserStates will need to be reinitialized on startup and replay. These are used to stage players in and out of IGameModes as well as show relevant information
        /// to the TV client.
        /// </summary>
        private void InitializeAllGameStates()
        {
            this.WaitForLobbyStart = new WaitForLobbyCloseGameState(this);
            this.EndOfGameRestart = new EndOfGameState(this, PrepareToRestartGame);
        }

        /// <summary>
        /// Attempts to add a specified user to the lobby.
        /// </summary>
        /// <param name="user">User object to add.</param>
        /// <param name="errorMsg">Error message only populated on failure.</param>
        /// <returns>True if successfully added.</returns>
        public bool TryAddUser(User user, out string errorMsg)
        {
            if (user == null)
            {
                errorMsg = "Something went wrong.";
                return false;
            }

            errorMsg = string.Empty;
            if (this.UsersInLobby.Contains(user))
            {
                user.LobbyId = LobbyId;
                return true;
            }

            if (!IsLobbyOpen())
            {
                errorMsg = "Lobby is closed.";
                return false;
            }

            // Should be a quick check in most scenarios
            if (!this.UsersInLobby.Any((user)=>user.IsPartyLeader))
            {
                user.IsPartyLeader = true;
            }
            this.UsersInLobby.Add(user);
            user.SetLobbyJoinTime();
            user.LobbyId = LobbyId;

            return true;
        }
        public void Inlet(User user, UserStateResult result, UserFormSubmission formSubmission)
        {
            if (!this.UsersInLobby.Contains(user))
            {
                throw new Exception("User not registered for this lobby");
            }
            this.WaitForLobbyStart.Inlet(user, result, formSubmission);
        }

        /// <summary>
        /// Returns the unity view that needs to be potentially sent to the clients.
        /// </summary>
        /// <returns>The active unity view</returns>
        public UnityView GetActiveUnityView()
        {
            return this.CurrentGameState?.GetActiveUnityView();
        }

        /// <summary>
        /// Transition to a new game state. A transition happens when the first user exits the game state. The other users presumably will be
        /// configured to follow suit (but wont call this function).
        /// </summary>
        /// <param name="transitionTo">The GameState to treat as the current state.</param>
        /// <remarks>This function is not responsible for moving users, users are individually responsible for traversing their FSMs. And the constructor of the FSMs
        /// is responsible for adding proper States to synchronize leaving game states.</remarks>
        public void TransitionCurrentGameState(GameState transitionTo)
        {
            this.CurrentGameState = transitionTo;
        }

        /// <summary>
        /// Returns the list of users which are currently registered in the lobby.
        /// </summary>
        public IReadOnlyList<User> GetUsers(UserActivity acitivity)
        {
            return this.UsersInLobby.Where(user => user.Activity == acitivity).ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns the list of users which are currently registered in the lobby.
        /// </summary>
        public IReadOnlyList<User> GetAllUsers()
        {
            return this.UsersInLobby.ToList().AsReadOnly();
        }

        public LobbyMetadataResponse GenerateLobbyMetadataResponseObject()
        {
            return new LobbyMetadataResponse()
            {
                LobbyId = this.LobbyId,
                PlayerCount = this.GetAllUsers().Count(),
                GameInProgress = this.IsGameInProgress(),
                /*this.GameModeSettings = lobby.SelectedGameMode;
                for (int i = 0; i < (this.GameModeSettings?.Options?.Count ?? 0); i++)
                {
                    if (lobby?.GameModeOptions?[i]?.ValueParsed != null)
                    {
                        this.GameModeSettings.Options[i].DefaultValue = lobby.GameModeOptions[i].Value;
                    }
                }*/
                SelectedGameMode = GameModes.FirstIndex((gameMode) => gameMode.GameModeMetadata.Title.Equals(SelectedGameMode?.GameModeMetadata?.Title, StringComparison.InvariantCultureIgnoreCase))
            };
        }

        /// <summary>
        /// Starts the game, throws if something is wrong with the configuration values.
        /// </summary>
        /// <param name="specialTransitionFrom">Where the current users are sitting (if somewhere other than WaitForLobbyStart)</param>
        public bool StartGame(out string errorMsg, GameState specialTransitionFrom = null)
        {
            errorMsg = string.Empty;
            if (this.SelectedGameMode == null)
            {
                errorMsg = "No game mode selected!";
                return false;
            }

            if (!this.SelectedGameMode.GameModeMetadata.IsSupportedPlayerCount(this.GetAllUsers().Count))
            {
                errorMsg = Invariant($"Selected game mode has following restrictions: {this.SelectedGameMode.GameModeMetadata.RestrictionsToString()}");
                return false;
            }

            // Slightly hacky default because it can't be passed in.
            GameState transitionFrom = specialTransitionFrom ?? this.WaitForLobbyStart;

            GameModeMetadataHolder gameModeMetadata = this.SelectedGameMode;
            IGameMode game;
            try
            {
                game = gameModeMetadata.GameModeInstantiator(this, this.GameModeOptions);
            }
            catch (GameModeInstantiationException err)
            {
                errorMsg = err.Message;
                return false;
            }

            this.Game = game;

            transitionFrom.Transition(game);
            game.Transition(this.EndOfGameRestart);
            this.WaitForLobbyStart.LobbyHasClosed();



            return true;
        }

        /// <summary>
        /// Updates the FSM based on the type of restart.
        /// </summary>
        public void PrepareToRestartGame(EndOfGameRestartType restartType)
        {
            GameState previousEndOfGameRestart = this.EndOfGameRestart;
            this.Game = null;
            switch (restartType)
            {
                case EndOfGameRestartType.Disband:
                    UnregisterAllUsers();
                    InitializeAllGameStates();
                    break;
                case EndOfGameRestartType.ResetScore:
                    InitializeAllGameStates();
                    previousEndOfGameRestart.Transition(this.WaitForLobbyStart);

                    foreach (User user in this.UsersInLobby)
                    {
                        user.Score = 0;
                    }
                    break;
                case EndOfGameRestartType.KeepScore:
                    InitializeAllGameStates();
                    previousEndOfGameRestart.Transition(this.WaitForLobbyStart);
                    break;
                default:
                    throw new Exception("Unknown restart game type");
            }
        }

        /// <summary>
        /// Kicks all the users out of the lobby and puts them back in the unregistered state.
        /// </summary>
        public void UnregisterAllUsers()
        {
            foreach (User user in GetAllUsers())
            {
                this.GameManager.UnregisterUser(user);
            }
            UsersInLobby.Clear();
        }

        public void AddEntranceListener(Action listener)
        {
            throw new NotImplementedException();
        }

        public void AddPerUserEntranceListener(Action<User> listener)
        {
            throw new NotImplementedException();
        }

        // TODO: unregister individual users manually as well as automatically. Handle it gracefully in the gamemode (don't wait for them on timeouts, also don't index OOB anywhere).
    }
}
