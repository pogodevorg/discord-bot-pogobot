using Discord.Commands;
using NadekoBot.Classes;
using NadekoBot.Modules.Permissions.Classes;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NadekoBot.Modules.Administration.Commands
{
    internal class RatelimitCommand : DiscordCommand
    {

        public static ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, DateTime>> RatelimitingChannels = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, DateTime>>();
        public static int slowtime = 0;

        public RatelimitCommand(DiscordModule module) : base(module)
        {
            NadekoBot.OnReady += () => NadekoBot.Client.MessageReceived += async (s, e) =>
            {
                if (e.Channel.IsPrivate || e.User.Id == NadekoBot.Client.CurrentUser.Id)
                    return;
                if (NadekoBot.IsOwner(e.User.Id)) return;
                try {
                var role_devs = e.Server.FindRoles("Developers").FirstOrDefault();
                var role_admins = e.Server.FindRoles("Administrators").FirstOrDefault();
                var role_mods = e.Server.FindRoles("Moderators").FirstOrDefault();
                var role_custom = e.Server.FindRoles("Discord").FirstOrDefault();
                if (e.User.HasRole(role_devs) || e.User.HasRole(role_admins) || e.User.HasRole(role_mods) || e.User.HasRole(role_custom)) return;
                }
                catch { }
                ConcurrentDictionary<ulong, DateTime> userTimePair;
                if (!RatelimitingChannels.TryGetValue(e.Channel.Id, out userTimePair)) return;
                DateTime lastMessageTime;
                if (userTimePair.TryGetValue(e.User.Id, out lastMessageTime))
                {
                    TimeSpan ratelimitTime = new TimeSpan(0, 0, 0, slowtime);
                    if (DateTime.Now - lastMessageTime < ratelimitTime)
                    {
                        try
                        {
                            await e.Message.Delete().ConfigureAwait(false);
                        }
                        catch { }
                        return;
                    }
                }
                userTimePair.AddOrUpdate(e.User.Id, id => DateTime.Now, (id, dt) => DateTime.Now);
            };
        }

        internal override void Init(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand(Module.Prefix + "slowmode")
                .Description($"Toggles slow mode. When ON, users will be able to send only 1 message every " + slowtime + " seconds. **Needs Manage Messages Permissions.**| `{Prefix}slowmode`")
                .AddCheck(SimpleCheckers.ManageMessages())
                .Parameter("msg", ParameterType.Unparsed)
                .Do(async e =>
                {
                    try
                    {
                        slowtime = Int32.Parse(e.GetArg("msg"));
                    }
                    catch (FormatException x)
                    {
                        Console.WriteLine(x.Message);
                    }

                    if (slowtime == 0 || string.IsNullOrWhiteSpace(e.GetArg("msg")))
                    {
                        slowtime = 10;
                    }
                    ConcurrentDictionary<ulong, DateTime> throwaway;
                    if (RatelimitingChannels.TryRemove(e.Channel.Id, out throwaway))
                    {
                        await e.Channel.SendMessage("Slow mode disabled.").ConfigureAwait(false);
                        return;
                    }
                    if (RatelimitingChannels.TryAdd(e.Channel.Id, new ConcurrentDictionary<ulong, DateTime>()))
                    {
                        await e.Channel.SendMessage("Slow mode initiated. " +
                                                    "Users can't send more than 1 message every " + slowtime + " seconds.")
                                                    .ConfigureAwait(false);
                        return;
                    }
                });
        }
    }
}
