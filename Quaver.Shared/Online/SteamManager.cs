/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Graphics;
using Quaver.Shared.Assets;
using Quaver.Shared.Scheduling;

namespace Quaver.Shared.Online
{
    public static class SteamManager
    {
        /// <summary>
        ///     The application id for steam.
        /// </summary>
        public static uint ApplicationId => 980610;

        /// <summary>
        ///     The avatars for steam users.
        /// </summary>
        public static ConcurrentDictionary<ulong, Texture2D> UserAvatars { get; } = new ConcurrentDictionary<ulong, Texture2D>();

        /// <summary>
        ///     Large Steam user avatars
        /// </summary>
        public static ConcurrentDictionary<ulong, Texture2D> UserAvatarsLarge { get; } = new ConcurrentDictionary<ulong, Texture2D>();

        /// <summary>
        ///     A user's steam avatar has loaded.
        /// </summary>
        public static EventHandler<SteamAvatarLoadedEventArgs> SteamUserAvatarLoaded;

        /// <summary>
        /// </summary>
        public static void SendAvatarRetrievalRequest(ulong steamId) => ThreadScheduler.Run(() =>
        {
            UserAvatars[steamId] = UserInterface.UnknownAvatar;
            UserAvatarsLarge[steamId] = UserInterface.UnknownAvatar;

            SteamUserAvatarLoaded?.Invoke(typeof(SteamManager), new SteamAvatarLoadedEventArgs(steamId, UserInterface.UnknownAvatar));
        });

        /// <summary>
        /// </summary>
        public static Texture2D GetAvatarOrUnknown(ulong steamId)
        {
            if (UserAvatars.ContainsKey(steamId))
                return UserAvatars[steamId];

            return UserInterface.UnknownAvatar;
        }
    }
}
