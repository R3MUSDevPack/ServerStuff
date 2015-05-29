using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using eZet.EveLib.Core;
using eZet.EveLib.Core.RequestHandlers;
using eZet.EveLib.Core.Serializers;

using eZet.EveLib.ZKillboardModule.Models;
using eZet.EveLib.ZKillboardModule.RequestHandlers;

namespace Hipchat_Plugin_TestHarness
{
    class zKillboard_Check
    {        /// <summary>
        ///     Default base URI.
        /// </summary>
        public const string DefaultHost = "https://zkillboard.com";

        /// <summary>
        ///     Default constructor
        /// </summary>
        public zKillboard_Check()
        {
            RequestHandler = new ZkbRequestHandler(new JsonSerializer(), Config.CacheFactory());
            Host = new Uri(DefaultHost);
        }

        /// <summary>
        ///     Returns true if the current request handler supports caching.
        /// </summary>
        public bool IsCacheHandler {
            get { return cachedRequestHandler() != null; }
        }

        /// <summary>
        ///     Gets or sets the default base URI
        /// </summary>
        public Uri Host { get; set; }

        /// <summary>
        ///     Gets or sets the request handler
        /// </summary>
        public IRequestHandler RequestHandler { get; set; }

        private ICachedRequestHandler cachedRequestHandler() {
            return RequestHandler as ICachedRequestHandler;
        }

        /// <summary>
        ///     Returns kill mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Kill mails</returns>
        public ZkbResponse GetKills(zKillboardOptions_Check options) {
            Contract.Requires(options != null, "Options cannot be null");
            return GetKillsAsync(options).Result;
        }

        /// <summary>
        ///     Returns kill mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Kill mails</returns>
        public Task<ZkbResponse> GetKillsAsync(zKillboardOptions_Check options)
        {
            Contract.Requires(options != null, "Options cannot be null");
            string relPath = "/api/kills";
            relPath = options.GetQueryString(relPath);
            return requestAsync(new Uri(Host, relPath));
        }

        /// <summary>
        ///     Returns loss mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Loss mails</returns>
        public ZkbResponse GetLosses(zKillboardOptions_Check options)
        {
            Contract.Requires(options != null, "Options cannot be null");
            return GetLossesAsync(options).Result;
        }

        /// <summary>
        ///     Returns loss mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Loss mails</returns>
        public Task<ZkbResponse> GetLossesAsync(zKillboardOptions_Check options)
        {
            Contract.Requires(options != null, "Options cannot be null");
            string relPath = "/api/losses";
            relPath = options.GetQueryString(relPath);
            return requestAsync(new Uri(Host, relPath));
        }

        /// <summary>
        ///     Returns both Kill and loss mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Kill and loss mails</returns>
        public ZkbResponse GetAll(zKillboardOptions_Check options)
        {
            Contract.Requires(options != null, "Options cannot be null");
            return GetAllAsync(options).Result;
        }

        /// <summary>
        ///     Returns both Kill and loss mails
        /// </summary>
        /// <param name="options">ZKillboard options</param>
        /// <returns>Kill and loss mails</returns>
        public Task<ZkbResponse> GetAllAsync(zKillboardOptions_Check options)
        {
            Contract.Requires(options != null, "Options cannot be null");
            string relPath = "/api";
            relPath = options.GetQueryString(relPath);
            return requestAsync(new Uri(Host, relPath));
        }

        private Task<ZkbResponse> requestAsync(Uri uri) {
            return RequestHandler.RequestAsync<ZkbResponse>(uri);
        }
    }
}
