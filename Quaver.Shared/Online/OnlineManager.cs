/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.Server.Client;
using Quaver.Server.Client.Events;
using Quaver.Server.Client.Events.Disconnnection;
using Quaver.Server.Client.Events.Login;
using Quaver.Server.Client.Events.Scores;
using Quaver.Server.Client.Handlers;
using Quaver.Server.Client.Structures;
using Quaver.Server.Common.Enums;
using Quaver.Server.Common.Helpers;
using Quaver.Server.Common.Objects;
using Quaver.Server.Common.Objects.Listening;
using Quaver.Server.Common.Objects.Multiplayer;
using Quaver.Server.Common.Objects.Twitch;
using Quaver.Shared.Audio;
using Quaver.Shared.Config;
using Quaver.Shared.Database.Maps;
using Quaver.Shared.Database.Scores;
using Quaver.Shared.Discord;
using Quaver.Shared.Graphics;
using Quaver.Shared.Graphics.Backgrounds;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Graphics.Online.Username;
using Quaver.Shared.Graphics.Overlays.Chatting;
using Quaver.Shared.Graphics.Overlays.Hub;
using Quaver.Shared.Helpers;
using Quaver.Shared.Modifiers;
using Quaver.Shared.Online.Chat;
using Quaver.Shared.Scheduling;
using Quaver.Shared.Screens;
using Quaver.Shared.Screens.Gameplay;
using Quaver.Shared.Screens.Loading;
using Quaver.Shared.Screens.Main;
using Quaver.Shared.Screens.Menu;
using Quaver.Shared.Screens.Music;
using Quaver.Shared.Screens.Select;
using Quaver.Shared.Screens.Select.UI.Leaderboard;
using Quaver.Shared.Screens.Tournament;

using UniversalThreadManagement;
using Wobble;
using Wobble.Bindables;
using Wobble.Discord;
using Wobble.Graphics.UI.Dialogs;
using Wobble.Logging;
using static Quaver.Shared.Online.OnlineManager;

namespace Quaver.Shared.Online
{
    public static class OnlineManager
    {
        /// <summary>
        ///    The online client that connects to the Quaver servers.
        /// </summary>
        private static readonly OnlineClient _client;
        public static OnlineClient Client
        {
            get => _client;
            private set
            {
            }
        }

        /// <summary>
        ///     The current online connection status.
        /// </summary>
        public static Bindable<ConnectionStatus> Status { get; } = new Bindable<ConnectionStatus>(ConnectionStatus.Disconnected);

        /// <summary>
        ///     If we're currently connected to the server.
        /// </summary>
        public static bool Connected => Client?.Socket != null && Status.Value == ConnectionStatus.Connected;

        /// <summary>
        ///     The user client for self (the current client.)
        /// </summary>
        public static User Self { get; private set; }

        /// <summary>
        ///     Dictionary containing all of the currently online users.
        /// </summary>
        public static Dictionary<int, User> OnlineUsers { get; private set; }

        /// <summary>
        ///     Dictionary containing all the currently available multiplayer games.
        ///     game_hash:game
        /// </summary>
        public static Dictionary<string, MultiplayerGame> MultiplayerGames { get; private set; }

        /// <summary>
        ///     The current multiplayer game the player is in
        /// </summary>
        public static MultiplayerGame CurrentGame { get; private set; }

        /// <summary>
        ///     The active listening party the user is in
        /// </summary>
        public static ListeningParty ListeningParty { get; private set; }

        /// <summary>
        ///     The players who the client is currently spectating
        ///
        ///     Note:
        ///         - Only 1 player is allowed if not running a tournament client
        ///         - Otherwise multiple are allowed.
        /// </summary>
        public static Dictionary<int, SpectatorClient> SpectatorClients { get; private set; } = new Dictionary<int, SpectatorClient>();

        /// <summary>
        ///     Players who are currently spectating us
        /// </summary>
        public static Dictionary<int, User> Spectators { get; private set; } = new Dictionary<int, User>();

        /// <summary>
        ///     If we're currently being spectated by another user
        /// </summary>
        public static bool IsBeingSpectated => Client != null && Status.Value == ConnectionStatus.Connected && Spectators.Count != 0;

        /// <summary>
        ///     If the client is currently spectating someone
        /// </summary>
        public static bool IsSpectatingSomeone => Client != null & Status.Value == ConnectionStatus.Connected && SpectatorClients.Count != 0;

        ///     If the current user is a donator
        /// </summary>
        public static bool IsDonator => Connected && Self.OnlineUser.UserGroups.HasFlag(UserGroups.Donator);

        /// <summary>
        ///     Returns if the user is the host of the listening party and can perform actions
        /// </summary>
        public static bool IsListeningPartyHost => ListeningParty == null || ListeningParty.Host == Self.OnlineUser;

        /// <summary>
        ///     The user's friends list
        /// </summary>
        public static List<int> FriendsList { get; private set; }

        /// <summary>
        ///     Returns if the user is currently wanting to fetch the realtime leaderboards
        /// </summary>
        public static bool ShouldFetchRealtimeLeaderboard => ConfigManager.EnableRealtimeOnlineScoreboard.Value
                                                             && ConfigManager.DisplayUnbeatableScoresDuringGameplay.Value
                                                             && CurrentGame == null;

        /// <summary>
        ///     Event invoked when the user's friends list has changed
        ///     (user added/removed)
        /// </summary>
        public static event EventHandler<FriendsListUserChangedEventArgs> FriendsListUserChanged;

        /// <summary>
        ///     List of currently available song requests
        /// </summary>
        public static List<SongRequest> SongRequests { get; } = new List<SongRequest>();

        /// <summary>
        /// </summary>
        public static string TwitchUsername { get; private set; }

        /// <summary>
        ///     Logs into the Quaver server.
        /// </summary>
        public static void Login()
        {
        }

        /// <summary>
        ///     Subscribes to all events
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private static void SubscribeToEvents()
        {
        }

        /// <summary>
        ///     Called when the connection status of the user has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when the client receives a response after selecting a username.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnChooseAUsernameResponse(object sender, ChooseAUsernameResponseEventArgs e)
        {
        }

        /// <summary>
        ///     When the client disconnects from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDisconnection(object sender, DisconnectedEventArgs e)
        {
        }

        /// <summary>
        ///     When the client successfully logs into the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnLoginSuccess(object sender, LoginReplyEventArgs e)
        {
        }

        /// <summary>
        ///     Called when a user connects to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserConnected(object sender, UserConnectedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when a user disconnects from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when the user receives a notification from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnNotificationReceived(object sender, NotificationEventArgs e)
        {
        }

        /// <summary>
        ///     Called when retrieving online scores or updates about our map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRetrievedOnlineScores(object sender, RetrievedOnlineScoresEventArgs e)
        {
        }

        /// <summary>
        ///     Called when we've successfully submitted a score to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnScoreSubmitted(object sender, ScoreSubmissionEventArgs e)
        {
        }

        /// <summary>
        ///     Gets the large key text for discord rich presence.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetRichPresenceLargeKeyText(GameMode mode)
        {
            return "Offline";
        }

        /// <summary>
        ///     Called when we've retrieved info about a bundle of new online users.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUsersOnline(object sender, UsersOnlineEventArgs e)
        {
        }

        /// <summary>
        ///     Called when receiving user info from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnUserInfoReceived(object sender, UserInfoEventArgs e)
        {
        }

        /// <summary>
        ///     Called when receiving user statuses from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserStatusReceived(object sender, UserStatusEventArgs e)
        {
        }

        /// <summary>
        ///     Called whenever receiving information about a multiplayer game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMultiplayerGameInfoReceived(object sender, MultiplayerGameInfoEventArgs e)
        {
        }

        /// <summary>
        ///     Called when the player successfully joins a multiplayer game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnJoinedMultiplayerGame(object sender, JoinedGameEventArgs e)
        {
        }

        /// <summary>
        ///     Called when joining a game to spectate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSpectateMultiplayerGame(object sender, SpectateMultiplayerGameEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameHostChanged(object sender, GameHostChangedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when a multiplayer game has been disbanded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameDisbanded(object sender, GameDisbandedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when failing to join a multiplayer game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnJoinGameFailed(object sender, JoinGameFailedEventargs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDifficultyRangeChanged(object sender, DifficultyRangeChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMaxSongLengthChanged(object sender, MaxSongLengthChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnAllowedModesChanged(object sender, AllowedModesChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnChangedModifiers(object sender, ChangeModifiersEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnFreeModTypeChanged(object sender, FreeModTypeChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPlayerChangedModifiers(object sender, PlayerChangedModifiersEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnGameKicked(object sender, KickedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnGameNameChanged(object sender, GameNameChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameInvite(object sender, GameInviteEventArgs e)
            => NotificationManager.Show(NotificationLevel.Info, $"{e.Sender} invited you to a game. Click here to join!",
            (o, args) =>
            {
            });

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameHealthTypeChanged(object sender, HealthTypeChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameLivesChanged(object sender, LivesChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameHostRotationChanged(object sender, HostRotationChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnGamePlayerTeamChanged(object sender, PlayerTeamChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameRulesetChanged(object sender, RulesetChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameLongNotePercentageChanged(object sender, LongNotePercentageChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameMaxPlayersChanged(object sender, MaxPlayersChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameTeamWinCountChanged(object sender, TeamWinCountEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGamePlayerWinCount(object sender, PlayerWinCountEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserStats(object sender, UserStatsEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserJoinedGame(object sender, UserJoinedGameEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserLeftGame(object sender, UserLeftGameEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameEnded(object sender, GameEndedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameStarted(object sender, GameStartedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGamePlayerNoMap(object sender, PlayerGameNoMapEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGamePlayerHasMap(object sender, GamePlayerHasMapEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameHostSelectingMap(object sender, GameHostSelectingMapEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameMapChanged(object sender, GameMapChangedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameSetReferee(object sender, GameSetRefereeEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGamePlayerReady(object sender, PlayerReadyEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGamePlayerNotReady(object sender, PlayerNotReadyEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnStartedSpectatingPlayer(object sender, StartSpectatePlayerEventArgs e)
        {
        }

        private static void OnStoppedSpectatingPlayer(object sender, StopSpectatePlayerEventArgs e)
        {
        }

        private static void OnSpectatorJoined(object sender, SpectatorJoinedEventArgs e)
        {
        }

        private static void OnSpectatorLeft(object sender, SpectatorLeftEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSpectatorReplayFrames(object sender, SpectatorReplayFramesEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnJoinedListeningParty(object sender, JoinedListeningPartyEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnLeftListeningParty(object sender, ListeningPartyLeftEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnListeningPartyStateUpdate(object sender, ListeningPartyStateUpdateEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnListeningPartyFellowJoined(object sender, ListeningPartyFellowJoinedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnListeningPartyFellowLeft(object sender, ListeningPartyFellowLeftEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnListeningPartyChangeHost(object sender, ListeningPartyChangeHostEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnListeningPartyUserMissingSong(object sender, ListeningPartyUserMissingSongEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnListeningPartyUserHasSong(object sender, ListeningPartyUserHasSongEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUserFriendsListReceieved(object sender, UserFriendsListEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSongRequestReceived(object sender, SongRequestEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTwitchConnectionReceived(object sender, TwitchConnectionEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameMapsetShared(object sender, GameMapsetSharedEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameCountdownStopped(object sender, StopCountdownEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnGameCountdownStarted(object sender, StartCountdownEventArgs e)
        {
        }

        /// <summary>
        ///     Leaves the current multiplayer game if any
        /// </summary>
        public static void LeaveGame(bool dontSendPacket = false)
        {
        }

        /// <summary>
        /// </summary>
        public static void JoinLobby() => Client?.JoinLobby();

        /// <summary>
        /// </summary>
        public static void LeaveLobby()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="action"></param>
        public static void UpdateListeningPartyState(ListeningPartyAction action)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="user"></param>
        public static void AddFriend(User user)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="user"></param>
        public static void RemoveFriend(User user)
        {
        }
    }
}
