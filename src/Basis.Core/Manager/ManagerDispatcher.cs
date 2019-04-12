using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Basis.Logging;
using Basis.Security;
using Basis.Validation;

namespace Basis.Manager
{
    internal class ManagerDispatcher<TManager> : IManagerDispatcher<TManager>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IManagerContext _context;
        private readonly TManager _manager;

        public ManagerDispatcher(IServiceProvider serviceProvider, IManagerContext context, TManager manager)
        {
            _serviceProvider = serviceProvider;
            _logger = (ILogger)serviceProvider.GetService(typeof(ILogger));
            _context = context;
            _manager = manager;
        }

        public T Do<T>(Expression<Func<TManager, T>> expr, string caller = "")
        {
            try
            {
                Preflight(expr, caller);

                var action = expr.Compile();

                _logger.Debug($"Calling {caller} on {typeof(TManager).FullName}");

                var result = action(_manager);

                _logger.Debug($"Called {caller} on {typeof(TManager).FullName}");

                return result;
            }
            catch (ManagerException ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ManagerException("A manager exception occurred", ex);
            }
        }

        public void Do(Expression<Action<TManager>> expr, string caller = "")
        {
            try
            {
                Preflight(expr, caller);

                var action = expr.Compile();

                _logger.Debug($"Calling {caller} on {typeof(TManager).FullName}");

                action(_manager);

                _logger.Debug($"Called {caller} on {typeof(TManager).FullName}");
            }
            catch (ManagerException ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ManagerException("A manager exception occurred", ex);
            }
        }

        private void Preflight(LambdaExpression expr, string caller)
        {
            _logger.Debug($"Preflight for {caller} on {typeof(TManager)}");

            var methodCall = (MethodCallExpression)expr.Body;

            var method = methodCall.Method;

            Secure(method);

            _logger.Info($"Security checks passed for {caller} on {typeof(TManager)} for user {_context.User.Identity.Name}");

            ValidateParameters(methodCall);

            _logger.Info($"Validation checks passed for {caller} on {typeof(TManager)}");
        }

        private void Secure(MethodInfo methodInfo)
        {
            var requiredClaims = ClaimsHelper.GetRequiredClaims(methodInfo);

            if (!requiredClaims.Any())
            {
                _logger.Warn($"Method {methodInfo.Name} on {methodInfo.DeclaringType} does not have any claims specified to secure it.");
            }

            var user = _context.User;

            var userClaims = user.Claims.ToList();

            var hasRequiredClaims = Check.HasClaims(requiredClaims, userClaims);

            if (hasRequiredClaims)
            {
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("User must have all of the following claims:");
            foreach (var claim in requiredClaims)
            {
                sb.AppendLine($"\t{claim}");
            }

            if (sb.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("User has the following claims:");
                foreach (var claim in userClaims)
                {
                    sb.AppendLine($"\t{claim}");
                }

                throw new ManagerException("A manager exception occurred", new SecurityException(sb.ToString()));
            }
        }

        private void ValidateParameters(MethodCallExpression expr)
        {
            var parameters = expr.Arguments
                .Cast<MemberExpression>()
                .Select(m => new
                {
                    m.Member.Name,
                    MemberExpression = m,
                    ConstantExpression = (ConstantExpression)m.Expression
                })
                .Select(c => new
                {
                    c.Name,
                    Value = ((FieldInfo)c.MemberExpression.Member).GetValue(c.ConstantExpression.Value)
                })
                .ToList();

            var engine = BuildValidationEngine();

            foreach (var p in parameters)
            {
                var results = engine.Validate(p.Value);

                try
                {
                    results.ThrowIfInvalid();
                }
                catch (ValidationResultException ex)
                {
                    var managerType = _manager.GetType();
                    var methodName = expr.Method.Name;
                    throw new ManagerException($"Validation failed calling {methodName} on {managerType} with parameter type {p.Value.GetType()} named {p.Name}", ex);
                }
            }
        }

        private IValidationEngine BuildValidationEngine()
        {
            return new ValidationEngine(_serviceProvider, cfg =>
            {
                cfg.AddItem(typeof(IManagerContext), _context);
                cfg.ValidateAllProperties();
            });
        }
    }
}