using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Replays;
using Quaver.Server.Client.Handlers;
using Quaver.Server.Client.Structures;
using Quaver.Server.Common.Enums;
using Quaver.Shared.Config;
using Quaver.Shared.Database.Maps;
using Quaver.Shared.Database.Scores;
using Quaver.Shared.Graphics.Backgrounds;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Graphics.Overlays.Hub;
using Quaver.Shared.Online.API.Maps;
using Quaver.Shared.Scheduling;
using Quaver.Shared.Screens;
using Quaver.Shared.Screens.Importing;
using Quaver.Shared.Screens.Loading;
using Quaver.Shared.Screens.Main;
using Wobble;
using Wobble.Audio.Tracks;
using Wobble.Logging;

namespace Quaver.Shared.Online
{
    public class SpectatorClient
    {
        /// <summary>
        ///     The player that is currently being spectated
        /// </summary>
        public User Player { get; }

        /// <summary>
        ///     The user's current replay that is being received.
        /// </summary>
        public Replay Replay { get; set; }

        /// <summary>
        ///     The map that the user is currently playing
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        ///     The list of frames for the current map/play session
        /// </summary>
        private List<SpectatorReplayFramesEventArgs> Frames { get; } = new List<SpectatorReplayFramesEventArgs>();

        /// <summary>
        ///     Returns if the client has notified the user if they don't have the map
        /// </summary>
        private bool HasNotifiedForThisMap { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        public SpectatorClient(User player) => Player = player;

        /// <summary>
        ///     Goes to spectate the user immediately
        /// </summary>
        public void WatchUserImmediately() => PlayNewMap(new List<ReplayFrame>(), false, true);

        /// <summary>
        ///     Handles when the client is beginning to play a new map
        /// </summary>
        private void PlayNewMap(List<ReplayFrame> frames, bool createNewReplay = true, bool forceIfImporting = false)
        {
        }

        /// <summary>
        ///     Adds a single replay frame to the spectating replay
        /// </summary>
        /// <param name="f"></param>
        public void AddFrame(ReplayFrame f) => Replay.Frames.Add(f);

        /// <summary>
        ///     Adds a bundle of replay frames to the spectating replay
        /// </summary>
        public void AddFrames(SpectatorReplayFramesEventArgs e)
        {
        }

        /// <summary>
        ///     Starts the download for the map if it is available for downloading
        /// </summary>
        private void DownloadMap()
        {
        }
    }
}