using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using static NLog.LayoutRenderers.LayoutRenderer;

#if NETSTANDARD_1plus
using NLog.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET layout renderers.
    /// </summary>
    public abstract class AspNetLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Initializes the <see cref="AspNetLayoutRendererBase"/>.
        /// </summary>
        protected AspNetLayoutRendererBase()
        {

        }

        /// <summary>
        /// Context for DI
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        public IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = GetHttpContextAccessor());
            set => _httpContextAccessor = value;
        }

        /// <summary>
        ///  Initialize
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            if (HttpContextAccessor == null)
            {
                Common.InternalLogger.Warn("Missing IHttpContextAccessor. Has it been registered before loading NLog Configuration? Consider reloading NLog Configuration after having registered the IHttpContextAccessor.");
            }
        }

        /// <summary>
        /// Validates that the HttpContext is available and delegates append to subclasses.<see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor?.HttpContext == null)
                return;

            DoAppend(builder, logEvent);
        }

        /// <summary>
        /// Implemented by subclasses to render request information and append it to the specified <see cref="StringBuilder" />.
        /// 
        /// Won't be called if <see cref="HttpContextAccessor"/> of <see cref="IHttpContextAccessor.HttpContext"/> is <c>null</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void DoAppend(StringBuilder builder, LogEventInfo logEvent);

#if NETSTANDARD_1plus

/// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _httpContextAccessor = null;
            base.CloseLayoutRenderer();
        }
#endif

#if !NETSTANDARD_1plus
        private static IHttpContextAccessor GetHttpContextAccessor() => new DefaultHttpContextAccessor();
#else
        private static IHttpContextAccessor GetHttpContextAccessor() => ServiceLocator.ServiceProvider?.GetService<IHttpContextAccessor>();
#endif

        /// <summary>
        /// Register a custom layout renderer with a callback function <paramref name="func" />. The callback recieves the logEvent and the current configuration.
        /// </summary>
        /// <param name="name">Name of the layout renderer - without ${}.</param>
        /// <param name="func">Callback that returns the value for the layout renderer.</param>
        public static void Register(string name, Func<LogEventInfo, IHttpContextAccessor, LoggingConfiguration, object> func)
        {
            object Func2(LogEventInfo logEventInfo, LoggingConfiguration configuration) => func(logEventInfo, GetHttpContextAccessor(), configuration);

            Register(name, Func2);
        }
    }
}

