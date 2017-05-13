using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Layouts;
using NLog.Web.LayoutRenderers;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.AspNetCore.Tests
{
    public class RegisterCustomLayoutRenderer : TestBase
    {
        [Fact]
        public void RegisterLayoutRendererTest()
        {

            //ServiceLocator.ServiceProvider;

            AspNetLayoutRendererBase.Register("test-web", (info, accessor, arg3) => accessor.HttpContext.Connection.LocalPort);
            Layout l = "${test-web}";
            var restult = l.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("something", restult);

        }
    }
}
