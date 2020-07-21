using System;
using System.IO;
using Quaver.Shared.Config;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Helpers;
using Quaver.Shared.Skinning;
using Wobble.Logging;

namespace Quaver.Shared.Online
{
    public class SteamWorkshopSkin
    {
        /// <summary>
        ///     The current workshop skin upload
        /// </summary>
        public static SteamWorkshopSkin Current { get; private set; }

        /// <summary>
        ///     The title of the skin
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     The path to the file
        /// </summary>
        public string PreviewFilePath { get; }

        /// <summary>
        ///     The path to the skin folder
        /// </summary>
        public string SkinFolderPath { get; }

        /// <summary>
        ///     The file path of the text file that contains the id of the skin
        /// </summary>
        public string WorkshopIdFilePath => $"{SkinFolderPath}/steam_workshop_id.txt";

        /// <summary>
        ///     The id of the existing workshop file (if updating)
        /// </summary>
        public ulong ExistingWorkshopFileId { get; private set; }

        /// <summary>
        ///     If the skin has uploaded
        /// </summary>
        public bool HasUploaded { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="skin"></param>
        public SteamWorkshopSkin(string skin)
        {
            if (Current != null && !Current.HasUploaded)
            {
                NotificationManager.Show(NotificationLevel.Warning, "You must wait until the previous upload has completed!");
                return;
            }

            Current = this;
            Title = skin;
            SkinFolderPath = SkinStore.Dir.Replace("\\", "/");
            PreviewFilePath = $"{SkinFolderPath}/steam_workshop_preview.png";

            if (!File.Exists(PreviewFilePath))
            {
                NotificationManager.Show(NotificationLevel.Error,
                    "You must place a steam_workshop_preview.png in the skin folder in order to upload it.");

                HasUploaded = true;
                return;
            }
        }
    }
}