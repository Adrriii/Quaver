using Quaver.Shared.Assets;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Online;
using Quaver.Shared.Screens;
using Quaver.Shared.Screens.Settings;
using Wobble;
using Wobble.Graphics.UI.Dialogs;
using Wobble.Managers;

namespace Quaver.Shared.Graphics.Menu.Border.Components.Buttons
{
    public class IconTextButtonDownloadMaps : IconTextButton
    {
        public IconTextButtonDownloadMaps() : base(FontAwesome.Get(FontAwesomeIcon.fa_download_to_storage_drive),
            FontManager.GetWobbleFont(Fonts.LatoBlack),"Maps", (sender, args) =>
            {
            })
        {
        }
    }
}