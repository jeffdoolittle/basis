using System;
using FluentAssertions;
using Xunit;

namespace Basis.Inversion.Tests
{
    public class RegistryTests : IDisposable
    {
        private readonly IServiceFactory _defaultProfile;
        private readonly IServiceFactory _overrideProfile;
        private readonly IServiceFactory _reverseProfile;

        public RegistryTests()
        {
            _defaultProfile = new ServiceFactory(_ => _.Apply<DefaultRegistry>());
            _overrideProfile = new ServiceFactory(_ => _.Apply<RootRegistry>());
            _reverseProfile = new ServiceFactory(_ => _.Apply<ReverseRegistry>());
        }

        [Fact]
        public void can_resolve_default_services()
        {
            var str = _defaultProfile.GetService<string>();

            str.Should().Be("default");
        }

        [Fact]
        public void can_resolve_profile_specific_services()
        {
            var value = _overrideProfile.GetService<string>();

            value.Should().Be("override");
        }

        [Fact]
        public void last_registration_wins()
        {
            var value = _reverseProfile.GetService<string>();

            value.Should().Be("default");
        }

        [Fact]
        public void can_resolve_scoped_object()
        {
            var scope = _defaultProfile.CreateScope();
            scope.GetService<object>().Should().NotBeNull();
            scope.Dispose();
        }

        public class RootRegistry : Registry
        {
            public RootRegistry()
            {
                IncludeRegistry<DefaultRegistry>();
                IncludeRegistry<OverrideRegistry>();
            }
        }

        public class ReverseRegistry : Registry
        {
            public ReverseRegistry()
            {
                IncludeRegistry<OverrideRegistry>();
                IncludeRegistry<DefaultRegistry>();
            }
        }

        public class DefaultRegistry : Registry
        {
            public DefaultRegistry()
            {
                Register(services =>
                {
                    services.AddSingleton(sp => "default");
                    services.AddScoped<object>(_ => new { });
                });
            }
        }

        public class OverrideRegistry : Registry
        {
            public OverrideRegistry()
            {
                Register(services =>
                {
                    services.AddTransient(_ => "override");
                });
            }
        }

        public void Dispose()
        {
            _defaultProfile?.Dispose();
            _overrideProfile?.Dispose();
            _reverseProfile?.Dispose();
        }
    }
}