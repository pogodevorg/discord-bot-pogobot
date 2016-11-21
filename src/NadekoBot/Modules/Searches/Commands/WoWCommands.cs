using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class WoWCommands
        {
            private Logger _log;

            public WoWCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
            }

            private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
            {
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime time = startDate.AddMilliseconds(unixTimeStamp);
                return time;
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task WoWStatus(IUserMessage umsg, string region, [Remainder] string realmNum = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(realmNum))
                {
                    await channel.SendMessageAsync("Please enter a region (e.g., us, eu, kr, tw, cn, sea), followed by a a realm number (0-x).").ConfigureAwait(false);
                    return;
                }
                var realm = GetWoWRealmStatus(region, int.Parse(realmNum));
                var status = await realm.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(status))
                    await channel.SendMessageAsync("Something went wrong ;(.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(status).ConfigureAwait(false);
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task WoWChar(IUserMessage umsg, string region, string realmName, [Remainder] string characterName = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(realmName) || string.IsNullOrWhiteSpace(characterName))
                {
                    await channel.SendMessageAsync("Please enter a region (e.g., us, eu, kr, tw, cn, sea), realm name (e.g., medivh), followed by a character name (e.g., Lisiano)").ConfigureAwait(false);
                    return;
                }

                var realm = GetCharacter(region, realmName, characterName);
                var status = await realm.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(status))
                    await channel.SendMessageAsync("Something went wrong ;(.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(status).ConfigureAwait(false);
            }

            public static async Task<string> GetWoWRealmStatus(string region, int realmNum)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var reqString = $"https://{region.ToLower()}.api.battle.net/wow/realm/status?apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["realms"] as JArray;

                        string statusCheck = (bool)items[realmNum]["status"] ? "(✔)GOOD" : "(X)BAD";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"`---Connected Realms---`");
                        foreach (var item in items[realmNum]["connected_realms"].ToArray())
                        {
                            sb.AppendLine($"〔:rosette: {item.ToString().ToUpper()}〕");
                        }
                        sb.Append($"`----------------------`");

                        var response = $@"```css
[☕ {region.ToUpper()}-WoW (Realm {realmNum}): {items[realmNum]["name"].ToString()} 〘Status: {statusCheck}〙]
Server Type: [{items[realmNum]["type"].ToString().ToUpper()}]
Battle Group: [{items[realmNum]["battlegroup"].ToString()}]
Population: [{items[realmNum]["population"].ToString().ToUpper()}]
Timezone: [{items[realmNum]["timezone"].ToString()}]
```
{sb.ToString()}";
                        return response;
                    }
                }
                catch
                {
                    return "Something went wrong ;(";
                }
            }

            public static async Task<string> GetCharacter(string region, string realm, string characterName)
            {
                var characterString = "";
                var classesString = "";
                var racesString = "";
                long lastModified = 0;
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        //Character Data
                        characterString = $"https://{region.ToLower()}.api.battle.net/wow/character/{realm.ToLower()}/{characterName.ToLower()}?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var characterObject = JObject.Parse(await http.GetStringAsync(characterString).ConfigureAwait(false));

                        //Classes Data
                        classesString = $"https://{region.ToLower()}.api.battle.net/wow/data/character/classes?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var classesObject = JObject.Parse(await http.GetStringAsync(classesString).ConfigureAwait(false));

                        //Races Data
                        racesString = $"https://{region.ToLower()}.api.battle.net/wow/data/character/races?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var racesObject = JObject.Parse(await http.GetStringAsync(racesString).ConfigureAwait(false));

                        var classesData = classesObject["classes"] as JArray;
                        var racesData = racesObject["races"] as JArray;

                        float charClassNum = (float)characterObject["class"];
                        float charRaceNum = (float)characterObject["race"];
                        lastModified = (long)characterObject["lastModified"];
                        DateTime time = UnixTimeStampToDateTime(lastModified);
                        string battleGroup = characterObject["battlegroup"].ToString();
                        string charClass = classesData[(int)charClassNum]["name"].ToString();
                        string charClass_powerType = classesData[(int)charClassNum]["powerType"].ToString().ToUpper();
                        string charRace = racesData[(int)charRaceNum]["name"].ToString();
                        string charRace_side = racesData[(int)charRaceNum]["side"].ToString().ToUpper();
                        string charGender = (bool)characterObject["gender"] ? "MALE" : "FEMALE";
                        float charLvl = (float)characterObject["level"];
                        float achievementPoints = (float)characterObject["achievementPoints"];
                        float totalHonorableKills = (float)characterObject["totalHonorableKills"];
                        //int charFaction = int.Parse(data["faction"].ToString());


                        var response = $@"```css
[☕ {region.ToUpper()}-WoW (Realm: {realm})〘Name: {characterName}〙]
Battle Group: [{battleGroup}]
Class: [{charClass}] / [Power Type: {charClass_powerType}]
Race: [{charRace}] / [{charRace_side}]
Level: [{(int)charLvl}]
Gender: [{charGender}]
Achievement Points: [{achievementPoints}]
Honorable Kills: [{totalHonorableKills}]
Last Modified (24hr): [{time}]
```";
                        return response;
                    }
                }
                catch
                {
                    return "Something went wrong ;(.";
                }
            }
        }
    }
}
