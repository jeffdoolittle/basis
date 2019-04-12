namespace Basis.Resource
{
    public class CommandConfigurer : ICommandConfigurer
    {
        private readonly Configuration _configuration;

        public CommandConfigurer()
        {
            _configuration = new Configuration();
        }

        public ICommandConfigurer TimeoutSeconds(int value)
        {
            Guard.InRangeInclusive(value, 0, int.MaxValue, nameof(value));

            _configuration.CommandTimeoutSeconds = value;

            return this;
        }

        public ICommandConfiguration GetConfiguration()
        {
            return _configuration;
        }

        private class Configuration : ICommandConfiguration
        {
            public int CommandTimeoutSeconds { get; set; } = 10;
        }
    }
}