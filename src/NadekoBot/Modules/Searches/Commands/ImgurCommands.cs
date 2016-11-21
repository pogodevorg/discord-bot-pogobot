using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Services;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class ImgurCommands
        {
            private Logger _log;

            public ImgurCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task RI(IUserMessage umsg, [Remainder] string query = null)
            {
                var channel = (ITextChannel)umsg.Channel;
                var provider = GetRandomImgur();
                var link = await provider.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(link))
                    await channel.SendMessageAsync("Search yielded no results ;(").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(link).ConfigureAwait(false);
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task RGI(IUserMessage umsg, string page, [Remainder] string window = null)
            {
                var channel = (ITextChannel)umsg.Channel;
                var provider = GetRandomGalleryImageID(page, window);
                var link = await provider.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(link))
                    await channel.SendMessageAsync("Search yielded no results ;(").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(link).ConfigureAwait(false);
            }

            public static async Task<string> GetRandomGalleryImageID(string page, string window)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        http.DefaultRequestHeaders.Add("Authorization", $"Client-ID {NadekoBot.Credentials.ImgurApiKey}");
                        var rng = new NadekoRandom();
                        var reqString = $"	https://api.imgur.com/3/gallery/hot/viral/{Uri.EscapeUriString(window)}/{Uri.EscapeUriString(page)}?showViral=bool";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["data"] as JArray;
                        var randomInt = rng.Next(1, items.Count);
                        if (obj["success"].ToString().Equals("false"))
                        {
                            return "API call to Imgur was unsuccessful ;(";
                        } else
                        {
                            return items[randomInt]["link"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "";
                }
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Imgur(IUserMessage umsg, [Remainder] string query = null)
            {
                var channel = (ITextChannel)umsg.Channel;
                if (string.IsNullOrWhiteSpace(query))
                {
                    await channel.SendMessageAsync("Please enter a query ;(").ConfigureAwait(false);
                    return;
                }
                var provider = GetImgurImage(query);
                var link = await provider.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(link))
                    await channel.SendMessageAsync("Search yielded no results ;(").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(link).ConfigureAwait(false);
            }

            public static async Task<string> GetImgurImage(string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                    return "Enter a query!";
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        http.DefaultRequestHeaders.Add("Authorization", $"Client-ID {NadekoBot.Credentials.ImgurApiKey}");
                        var rng = new NadekoRandom();
                        var reqString = $"https://api.imgur.com/3/gallery/search/top/1?q_any={Uri.EscapeUriString(query)}";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["data"] as JArray;
                        if (obj["success"].ToString().Equals("false"))
                        {
                            return "API call to Imgur was unsuccessful ;(";
                        }
                        else
                        {
                            return items[rng.Next(1, items.Count)]["link"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "";
                }
            }

            public static async Task<string> GetGalleryImg(string id)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        http.DefaultRequestHeaders.Add("Authorization", $"Client-ID {NadekoBot.Credentials.ImgurApiKey}");
                        var rng = new NadekoRandom();
                        var reqString = $"https://api.imgur.com/3/gallery/image/{id}";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["data"] as JArray;
                        var itemsImages = items["images"] as JArray;
                        var randomInt = rng.Next(1, itemsImages.Count);
                        if (obj["success"].ToString().Equals("false"))
                        {
                            return "API call to Imgur was unsuccessful ;(";
                        }
                        else
                        {
                           return items[randomInt]["link"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "";
                }
            }

            public static async Task<string> GetRandomImgur()
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        http.DefaultRequestHeaders.Add("Authorization", $"Client-ID {NadekoBot.Credentials.ImgurApiKey}");
                        var rng = new NadekoRandom();
                        var reqString = $"https://api.imgur.com/3/gallery/random/random/{rng.Next(1, 50)}";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["data"] as JArray;
                        if (obj["success"].ToString().Equals("false"))
                        {
                            return "API call to Imgur was unsuccessful ;(";
                        }
                        else
                        {
                            return items[rng.Next(1, items.Count)]["link"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "";
                }
            }


        }
    }
}
