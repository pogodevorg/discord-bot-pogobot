using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class RatelimitCommand
        {
            public static ConcurrentDictionary<ulong, Ratelimiter> RatelimitingChannels = new ConcurrentDictionary<ulong, Ratelimiter>();
            private Logger _log { get; }

            private ShardedDiscordClient _client { get; }

            public class Ratelimiter
            {
                public class RatelimitedUser
                {
                    public ulong UserId { get; set; }
                    public int MessageCount { get; set; } = 0;
                }

                public ulong ChannelId { get; set; }

                public int MaxMessages { get; set; }
                public int PerSeconds { get; set; }

                public CancellationTokenSource cancelSource { get; set; } = new CancellationTokenSource();

                public ConcurrentDictionary<ulong, RatelimitedUser> Users { get; set; } = new ConcurrentDictionary<ulong, RatelimitedUser>();

                public bool CheckUserRatelimit(ulong id)
                {
                    RatelimitedUser usr = Users.GetOrAdd(id, (key) => new RatelimitedUser() { UserId = id });
                    if (usr.MessageCount == MaxMessages)
                    {
                        return true;
                    }
                    else
                    {
                        usr.MessageCount++;
                        var t = Task.Run(async () => {
                            try
                            {
                                await Task.Delay(PerSeconds * 1000, cancelSource.Token);
                            }
                            catch (OperationCanceledException) { }
                            usr.MessageCount--;
                        });
                        return false;
                    }

                }
            }

            public RatelimitCommand()
            {
                this._client = NadekoBot.Client;
                this._log = LogManager.GetCurrentClassLogger();

               _client.MessageReceived += (umsg) =>
                {
                    var t = Task.Run(async () =>
                    {
                        var usrMsg = umsg as IUserMessage;
                        var usrGuild = umsg as IGuildUser;
                        var channel = usrMsg.Channel as ITextChannel;
                        var usr = channel.GetUser(umsg.Author.Id) as IUser;
                        ulong[] white = new ulong[] { 
                            208441726464032769, // administrators
                            240045137810554880, // moderators
                            240049119035523073, // devs
                            240049515770413057, // moderators I18n
                            240049523370622976 // helpers
                        };
                        var roles = usrGuild.Roles;
                        if (channel == null || usrMsg.IsAuthor() || usrMsg.Author.Id == channel.Guild.OwnerId)
                            return;
                        for (int i = 0; i <= roles.Count; i++)
                        {
                            for (int x = 0; x <= white.Length; x++) {
                               var usrGuild_Roles = channel.Guild.GetRole(white[x]);
                               var usrGuild_Roles_Members = usrGuild_Roles.Members();
                                foreach (IUser user_roles in usrGuild_Roles_Members)
                                {
                                    if (usr == user_roles)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        Ratelimiter limiter;
                        if (!RatelimitingChannels.TryGetValue(channel.Id, out limiter))
                            return;

                        if (limiter.CheckUserRatelimit(usrMsg.Author.Id))
                            try { await usrMsg.DeleteAsync(); } catch (Exception ex) { _log.Warn(ex); }
                    });
                    return Task.CompletedTask;
                };
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageMessages)]
            public async Task Slowmode(IUserMessage umsg)
            {
                var channel = (ITextChannel)umsg.Channel;

                Ratelimiter throwaway;
                if (RatelimitingChannels.TryRemove(channel.Id, out throwaway))
                {
                    throwaway.cancelSource.Cancel();
                    await channel.SendMessageAsync("`Slow mode disabled.`").ConfigureAwait(false);
                    return;
                }
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageMessages)]
            public async Task Slowmode(IUserMessage umsg, int msg, int perSec)
            {
                await Slowmode(umsg).ConfigureAwait(false); // disable if exists
                var channel = (ITextChannel)umsg.Channel;

                if (msg < 1 || perSec < 1 || msg > 100 || perSec > 3600)
                {
                    await channel.SendMessageAsync("`Invalid parameters.`");
                    return;
                }
                var toAdd = new Ratelimiter()
                {
                    ChannelId = channel.Id,
                    MaxMessages = msg,
                    PerSeconds = perSec,
                };
                if(RatelimitingChannels.TryAdd(channel.Id, toAdd))
                {
                    await channel.SendMessageAsync("`Slow mode initiated.` " +
                                                $"Users can't send more than {toAdd.MaxMessages} message(s) every {toAdd.PerSeconds} second(s).")
                                                .ConfigureAwait(false);
                }
            }
        }
    }
}