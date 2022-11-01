using Shadowsocks.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shadowsocks.CLI
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            var clientCommand = new Command("client", "Shadowsocks client.");
            clientCommand.AddAlias("c");
            clientCommand.AddOption(new Option<Backend>("--backend", "Shadowsocks backend to use. Available backends: shadowsocks-rust, v2ray, legacy, pipelines."));
            clientCommand.AddOption(new Option<string?>("--listen", "Address and port to listen on for both SOCKS5 and HTTP proxy."));
            clientCommand.AddOption(new Option<string?>("--listen-socks", "Address and port to listen on for SOCKS5 proxy."));
            clientCommand.AddOption(new Option<string?>("--listen-http", "Address and port to listen on for HTTP proxy."));
            clientCommand.AddOption(new Option<string>("--server-address", "Address of the remote Shadowsocks server to connect to."));
            clientCommand.AddOption(new Option<int>("--server-port", "Port of the remote Shadowsocks server to connect to."));
            clientCommand.AddOption(new Option<string>("--method", "Encryption method to use for remote Shadowsocks server."));
            clientCommand.AddOption(new Option<string?>("--password", "Password to use for remote Shadowsocks server."));
            clientCommand.AddOption(new Option<string?>("--key", "Encryption key (NOT password!) to use for remote Shadowsocks server."));
            clientCommand.AddOption(new Option<string?>("--plugin", "Plugin binary path."));
            clientCommand.AddOption(new Option<string?>("--plugin-opts", "Plugin options."));
            clientCommand.AddOption(new Option<string?>("--plugin-args", "Plugin startup arguments."));
            clientCommand.Handler = CommandHandler.Create(
                async (Backend backend, string? listen, string? listenSocks, string? listenHttp, string serverAddress, int serverPort, string method, string? password, string? key, string? plugin, string? pluginOpts, string? pluginArgs, CancellationToken cancellationToken) =>
                {
                    Locator.CurrentMutable.RegisterConstant<ConsoleLogger>(new());
                    if (string.IsNullOrEmpty(listenSocks))
                    {
                        LogHost.Default.Error("You must specify SOCKS5 listen address and port.");
                        return;
                    }

                    Client.Legacy? legacyClient = null;
                    Client.Pipelines? pipelinesClient = null;

                    switch (backend)
                    {
                        case Backend.SsRust:
                            LogHost.Default.Error("Not implemented.");
                            break;
                        case Backend.V2Ray:
                            LogHost.Default.Error("Not implemented.");
                            break;
                        case Backend.Legacy:
                            if (!string.IsNullOrEmpty(password))
                            {
                                legacyClient = new();
                                legacyClient.Start(listenSocks, serverAddress, serverPort, method, password, plugin, pluginOpts, pluginArgs);
                            }
                            else
                                LogHost.Default.Error("The legacy backend requires password.");
                            break;
                        case Backend.Pipelines:
                            pipelinesClient = new();
                            await pipelinesClient.Start(listenSocks, serverAddress, serverPort, method, password, key, plugin, pluginOpts, pluginArgs);
                            break;
                        default:
                            LogHost.Default.Error("Not implemented.");
                            break;
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromHours(1.00), cancellationToken);
                        Console.WriteLine("An hour has passed.");
                    }

                    switch (backend)
                    {
                        case Backend.SsRust:
                            LogHost.Default.Error("Not implemented.");
                            break;
                        case Backend.V2Ray:
                            LogHost.Default.Error("Not implemented.");
                            break;
                        case Backend.Legacy:
                            legacyClient?.Stop();
                            break;
                        case Backend.Pipelines:
                            pipelinesClient?.Stop();
                            break;
                        default:
                            LogHost.Default.Error("Not implemented.");
                            break;
                    }
                });

}
