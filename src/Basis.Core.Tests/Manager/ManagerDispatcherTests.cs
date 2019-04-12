using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Basis.Logging;
using Basis.Manager;
using Basis.Security;
using Basis.Validation;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Manager
{
    public class ManagerDispatcherTests
    {
        private readonly TestContext _context;

        public ManagerDispatcherTests()
        {
            _context = new TestContext();
        }

        [Fact]
        public void security_exception_when_claims_for_user_do_not_match_required_claims()
        {
            _context.ManagerContext.AddClaims(new Claim("other", "mismatch"));

            Action action = () =>
                _context.Manager.When(new Model());

            Action action2 = () =>
                _context.Manager.When2(new Model());

            action.Should().Throw<ManagerException>()
                .WithInnerException<SecurityException>();

            action2.Should().Throw<ManagerException>()
                .WithInnerException<SecurityException>();
        }

        [Fact]
        public void validation_exception_when_parameter_fails_validation()
        {
            _context.ManagerContext.AddClaims(new Claim("type", "value"));

            Action action = () =>
                _context.Manager.When(new Model());

            Action action2 = () =>
                _context.Manager.When2(new Model());

            action.Should().Throw<ManagerException>()
                .WithInnerException<ValidationResultException>();

            action2.Should().Throw<ManagerException>()
                .WithInnerException<ValidationResultException>();
        }

        [Fact]
        public void no_exception_when_preflight_checks_pass()
        {
            _context.ManagerContext.AddClaims(new Claim("type", "value"));

            Action action = () =>
                _context.Manager.When(new Model { Id = 1 });

            Action action2 = () =>
                _context.Manager.When2(new Model { Id = 1 });

            action.Should().NotThrow<ManagerException>();
            action2.Should().NotThrow<ManagerException>();
        }

        [Fact]
        public void exceptions_in_manager_implementation_are_wrapped_in_a_manager_exception()
        {
            Action action = () => _context.Manager.Error("foobar");

            action.Should().Throw<ManagerException>()
                .WithInnerException<Exception>()
                .WithMessage("foobar");

            Action action2 = () => _context.Manager.Error2("froboz");

            action2.Should().Throw<ManagerException>()
                .WithInnerException<Exception>()
                .WithMessage("froboz");
        }

        [Fact]
        public void can_return_value_when_using_manager_dispatcher()
        {
            var result = _context.Manager.Add(1, 1);
            result.Should().Be(2);
        }

        [Fact]
        public void can_pass_null_parameter_values_to_manager()
        {
            _context.Manager.Error(null);
            _context.Manager.Error2(null);
        }

        public class TestContext
        {
            public TestContext()
            {
                ServiceProvider = A.Fake<IServiceProvider>();
                ManagerContext = new TestManagerContext();
                Logger = new LoggerStub();

                A.CallTo(() => ServiceProvider.GetService(typeof(ILogger)))
                    .Returns(Logger);

                var manager = new Manager();
                var factory = new ManagerDispatcherFactory(ServiceProvider, ManagerContext);
                Manager = new ManagerDecorator(factory.CreateFor<IManager>(manager));
            }

            public IServiceProvider ServiceProvider { get; set; }
            public TestManagerContext ManagerContext { get; set; }
            public IManager Manager { get; set; }
            public LoggerStub Logger { get; set; }
        }

        public interface IManager
        {
            [RequireClaims("type", "value")]
            void When(Model model);

            [RequireClaims("type", "value")]
            bool When2(Model model);

            int Add(int x, int y);

            void Error(string message);
            bool Error2(string message);
        }

        public class ManagerDecorator : IManager
        {
            private readonly IManagerDispatcher<IManager> _dispatcher;

            public ManagerDecorator(IManagerDispatcher<IManager> dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public void When(Model model)
            {
                _dispatcher.Do(_ => _.When(model));
            }

            public bool When2(Model model)
            {
                return _dispatcher.Do(_ => _.When2(model));
            }

            public int Add(int x, int y)
            {
                return _dispatcher.Do(_ => _.Add(x, y));
            }

            public void Error(string message)
            {
                _dispatcher.Do(_ => _.Error(message));
            }

            public bool Error2(string message)
            {
                return _dispatcher.Do(_ => _.Error2(message));
            }
        }

        public class Manager : IManager
        {
            public void When(Model model)
            {
            }

            public bool When2(Model model)
            {
                return false;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public void Error(string message)
            {
                if (message != null) throw new Exception(message);
            }

            public bool Error2(string message)
            {
                if (message != null) throw new Exception(message);
                return false;
            }
        }

        public class Model
        {
            [Range(1, int.MaxValue)]
            public int Id { get; set; }
        }
    }
}
