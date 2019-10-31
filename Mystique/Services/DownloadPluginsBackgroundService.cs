using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mystique.Services
{
    public class DownloadPluginsBackgroundService : BackgroundService
    {
        private readonly IServiceScope serviceScope;
        private readonly PluginDbContext pluginDbContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly FtpClient.FtpClientOption ftpClientOption;
        private readonly ILogger<DownloadPluginsBackgroundService> logger;

        public DownloadPluginsBackgroundService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
        {
            serviceScope = serviceScopeFactory.CreateScope();
            pluginDbContext = serviceScope.ServiceProvider.GetRequiredService<PluginDbContext>();
            unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            ftpClientOption = configuration.GetSection("FtpClientOption").Get<FtpClient.FtpClientOption>();
            logger = loggerFactory.CreateLogger<DownloadPluginsBackgroundService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (ftpClientOption.Host?.Any() != true)
            {
                logger.LogInformation(new EventId(204), "未配置 ftp 地址，此服务不执行");
                return;
            }

            var ftpClient = new FtpClient(ftpClientOption);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DetectAndUpdatePlugins(ftpClient);
                }
                catch (Exception ex)
                {
                    logger.LogError(new EventId(500, "检查插件更新"), ex, ex.Message);
                }
#if DEBUG
                await Task.Delay(12 * 1000);
#else
                await Task.Delay(5 * 60 * 1000);    // 五分钟检查一次插件是否有更新
#endif
            }
        }

        private async Task DetectAndUpdatePlugins(FtpClient ftpClient)
        {
            var dbFiles = await pluginDbContext.FtpFileDetails.ToArrayAsync();
            var ftpFiles = await ftpClient.GetFileDetailsAsync();

            foreach (var o in dbFiles.Select(o => o.Name).Concat(ftpFiles.Select(o => o.name)).Distinct())
            {
                var cache = dbFiles.FirstOrDefault(x => x.Name == o); // cache
                var (name, size, modified) = ftpFiles.FirstOrDefault(x => x.name == o); // current
                if (size != cache?.Size || modified != cache?.Modified)
                {
                    try
                    {
                        // 下载并更新插件
                        using (var stream = await ftpClient.DownloadAsync(name))
                        {
                            var pluginPackage = serviceScope.ServiceProvider.GetRequiredService<PluginPackage>();
                            var pluginManager = serviceScope.ServiceProvider.GetRequiredService<IPluginManager>();

                            await pluginPackage.InitializeAsync(stream);
                            await pluginManager.AddPluginsAsync(pluginPackage, true);
                        }

                        // 记录状态
                        if (cache == null)
                        {
                            pluginDbContext.Entry(new Core.ViewModels.FtpFileDetail { Name = name, Size = size, Modified = modified }).State = EntityState.Added;
                        }
                        else
                        {
                            cache.Size = size;
                            cache.Modified = modified;
                        }
                        logger.LogInformation(new EventId(200, "下载并更新插件"), $"name={name},size={size},modified={modified}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(new EventId(400, "下载并更新插件"), ex, ex.Message);
                    }
                }
            }

            await unitOfWork.SaveAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            using (serviceScope) { }
            return base.StopAsync(cancellationToken);
        }
    }

    public class FtpClient
    {
        private readonly FtpClientOption option;

        public FtpClient(FtpClientOption option)
        {
            var host = option.Host;
            option.Host = host.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ? host : $"ftp://{host}";
            this.option = option;
        }

        /// <summary>
        /// 创建一个FTP请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方法</param>
        /// <param name="requestAction">修改 request 参数</param>
        /// <returns>FTP请求</returns>
        private FtpWebRequest FastCreateRequest(string file, string method)
        {
            var url = option.BuildAbsoluteUri(file);
            var request = (FtpWebRequest)WebRequest.Create(new Uri(url));
            request.Credentials = new NetworkCredential(option.UserId, option.Password);
            request.Proxy = option.Proxy;
            request.KeepAlive = false; // 命令执行完毕之后关闭连接
            request.UseBinary = option.UseBinary;
            request.UsePassive = option.UsePassive;
            request.EnableSsl = option.EnableSsl;
            request.Method = method;
            return request;
        }

        /// <summary>
        /// 获取详细列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<(string name, long size, DateTime modified)>> GetFileDetailsAsync()
        {
            var request = FastCreateRequest(null, WebRequestMethods.Ftp.ListDirectoryDetails);
            using (var response = (FtpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                var files = new List<(string name, long size, DateTime modified)>();
                var regex = new Regex(@"^(\d+-\d+-\d+\s+\d+:\d+(?:AM|PM))\s+(<DIR>|\d+)\s+(.+)$");
                IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");
                string line;
                while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
                {
                    var match = regex.Match(line);
                    var modified = DateTime.TryParseExact(match.Groups[1].Value, "MM-dd-yy  hh:mmtt", culture, DateTimeStyles.None, out DateTime dt) ? dt : DateTime.MaxValue;
                    var size = long.TryParse(match.Groups[2].Value, out long lg) ? lg : 0;
                    var name = match.Groups[3].Value;
                    files.Add((name, size, modified));
                }
                return files;
            }
        }

        /// <summary>
        /// 从当前目录下下载文件
        /// <para>
        /// 如果本地文件存在,则从本地文件结束的位置开始下载.
        /// </para>
        /// </summary>
        /// <param name="serverName">服务器上的文件名称</param>
        /// <param name="localName">本地文件名称</param>
        /// <returns>返回一个值,指示是否下载成功</returns>

        public async Task<Stream> DownloadAsync(string serverName)
        {
            var request = FastCreateRequest(serverName, WebRequestMethods.Ftp.DownloadFile);
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                var stream = response.GetResponseStream();
                var memoryStream = new MemoryStream();
                await CopyStreamAsync(stream, memoryStream);
                return memoryStream;
            }

            async Task CopyStreamAsync(Stream input, Stream output)
            {
                var buffer = new byte[16 * 1024];
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await output.WriteAsync(buffer, 0, read);
                }
            }
        }


        /// <summary>
        /// 默认不使用SSL,使用二进制传输方式,使用被动模式。FTP有两种使用模式：主动和被动。
        /// 主动模式要求客户端和服务器端同时打开并且监听一个端口以建立连接。
        /// 在这种情况下，客户端由于安装了防火墙会产生一些问题。
        /// 所以，创立了被动模式。
        /// 被动模式只要求服务器端产生一个监听相应端口的进程，这样就可以绕过客户端安装了防火墙的问题。
        /// </summary>
        public class FtpClientOption
        {
            /// <summary>
            /// 主机
            /// </summary>
            public string Host { get; set; } = "ftp://localhost";
            /// <summary>
            /// 登录用户名
            /// </summary>
            public string UserId { get; set; } = string.Empty;
            /// <summary>
            /// 登录密码
            /// </summary>
            public string Password { get; set; } = string.Empty;
            /// <summary>
            /// 端口（默认 21）
            /// </summary>
            public int? Port { get; set; } = 21;
            /// <summary>
            /// 代理
            /// </summary>
            public IWebProxy Proxy { get; set; }
            /// <summary>
            /// SSL 加密
            /// </summary>
            public bool EnableSsl { get; set; } = false;
            /// <summary>
            /// 二进制方式
            /// </summary>
            public bool UseBinary { get; set; } = true;
            /// <summary>
            /// 被动模式
            /// </summary>
            public bool UsePassive { get; set; } = true;
            /// <summary>
            /// 远端路径
            /// <para>
            /// 返回FTP服务器上的当前路径(可以是 / 或 /a/../ 的形式)
            /// </para>
            /// </summary>
            public string RemotePath { get; set; } = "/";

            public string BuildAbsoluteUri(string remoteFileName = "")
            {
                var url = new Uri(new[] { RemotePath ?? "/", remoteFileName ?? "" }.Aggregate(Host, (current, path) =>
                {
                    return $"{current.TrimEnd('/')}/{path.TrimStart('/')}";
                }));
                return url.AbsoluteUri;
            }
        }
    }

}
