using System.ComponentModel.DataAnnotations;

namespace Basis.Validation
{
    public static class ValidationContextExtensions
    {
        public static T GetService<T>(this ValidationContext context) where T : class
        {
            Guard.NotNull(context, nameof(context));

            var item = context.Items.ContainsKey(typeof(T)) ? context.Items[typeof(T)] : null;
            var service = context.GetService(typeof(T)) ?? item;

            if (service == null && item == null)
            {
                throw new BasisException(string.Format(Messages.service_of_type_x_is_not_registered, typeof(T)));
            }

            return (T)service;
        }
    }
}