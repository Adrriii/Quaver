using System;
using Quaver.Shared.Config;
using Quaver.Shared.Online;

namespace Quaver.Shared.Graphics.Dialogs.Online
{
    public class CreatingAccountDialog : LoadingDialog
    {
        public CreatingAccountDialog(string username) : base("CREATING ACCOUNT", "Your account is being created. Please wait!",
            () =>
            {
                OnlineManager.Client.ChooseUsername(username, 0UL, ConfigManager.Username.Value);
            })
        {
        }
    }
}