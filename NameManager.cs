using Oxide.Core;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("NameManager", "Ankawi", "1.1.0")]
    [Description("Manage names on your server")]
    class NameManager : CovalencePlugin
    {

        #region Config
        List<object> RestrictedNames;
        List<object> RestrictedCharacters;
        List<object> RestrictedWords;

        new void LoadConfig()
        {
            SetConfig("Characters Required", 2);
            SetConfig("Enable Restricted Characters", false);
            SetConfig("Enable Restricted Names", false);
            SetConfig("Enable Restricted Words", true);
            SetConfig("Enable Character Requirement", false);

            SetConfig("Restricted Characters", new List<object> { '$', '!', '+' });
            SetConfig("Restricted Names", new List<object> { "Oxide", "Admin", "Owner", "Moderator" });
            SetConfig("Restricted Words", new List<object> { "chink", "fag", "fagging", "faggitt", "faggot", "faggots", "faggs", "fagot", "fagots", "fags", "gaylord", "gaysex", "n1gga", "n1gger", "nazi", "nigg3r", "nigg4h", "nigga", "niggah", "niggas", "niggaz", "nigger", "niggers", "queer", "whore" });

            SaveConfig();
        }
        protected override void LoadDefaultConfig() => PrintWarning("Creating a new configuration file...");
        #endregion

        #region Hooks
        void Loaded()
        {
            permission.RegisterPermission("namemanager.admin", this);
            LoadDefaultMessages();
            LoadConfig();

            RestrictedNames = (List<object>)Config["Restricted Names"];
            RestrictedCharacters = (List<object>)Config["Restricted Characters"];
            RestrictedWords = (List<object>)Config["Restricted Words"];
        }

        object CanUserLogin(string name, string id, string ip)
        {
            // PrintWarning($"{name} ({id}) tries to connect from {ip} ({name.Length} of minimal {Config["Characters Required"]})");

            if (permission.UserHasPermission(id, "namemanager.admin")) return null;

            if ((bool)Config["Enable Character Requirement"])
            {
                if (name.Length < (int)(Config["Characters Required"]))
                {
                    return (GetMsg("Not Enough Characters", id));
                }
            }
            if ((bool)Config["Enable Restricted Names"])
            {
                if (RestrictedNames.Contains(name))
                {
                    return (GetMsg("Restricted Name", id));
                }
            }
            if ((bool)Config["Enable Restricted Characters"])
            {
                foreach(var bannedChar in RestrictedCharacters)
                {
                    if (name.Contains(bannedChar.ToString()))
                    {
                        return (GetMsg("Restricted Character", id));
                    }
                }
            }
            if ((bool)Config["Enable Restricted Words"])
            {
                string playername = name.ToLower();

                foreach (var badword in RestrictedWords)
                {
                    if (playername.Contains(badword.ToString()))
                    {
                        //PrintWarning($"{name} ({id}) kicked for bad word: {badword}");
                        return (GetMsg("Restricted Word", id) + " (" + badword.ToString() + ")");
                    }
                }
            }

            return null;
        }
        #endregion

        #region Localization

        void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"Restricted Name", "You were kicked for using a restricted name"},
                {"Restricted Character", "You were kicked for using a restricted character"},
                {"Not Enough Characters", "You were kicked because your name was not long enough"},
                {"Restricted Word", "Kicked for having a banned word in your name"},

            }, this);
        }
        #endregion

        #region Helpers

        void SetConfig(params object[] args)
        {
            List<string> stringArgs = (from arg in args select arg.ToString()).ToList<string>();
            stringArgs.RemoveAt(args.Length - 1);

            if (Config.Get(stringArgs.ToArray()) == null) Config.Set(args);
        }

        string GetMsg(string key, object id = null) => lang.GetMessage(key, this, id == null ? null : id.ToString());
        #endregion
    }
}
