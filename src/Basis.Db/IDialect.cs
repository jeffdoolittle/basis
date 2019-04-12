using Basis.Resource;

namespace Basis.Db
{
    public interface IDialect
    {
        DbProviderTypes ProviderType { get; }
        string ParameterPrefix { get; }
    }
}