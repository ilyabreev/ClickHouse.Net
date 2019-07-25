using System;

namespace ClickHouse.Net
{
    public class DefaultPropertyBinder : IPropertyBinder
    {
        public void BindProperty(object item, string propertyName, object value)
        {
            var propertyInfo = item.GetType().GetProperty(propertyName);
            var propertyType = propertyInfo?.PropertyType;

            // Use most simple signature for Parse method with one argument.
            var parseMethod = propertyType?.GetMethod("Parse", new[] { typeof(string) });

            // If value is sting - assign it to property.
            if (propertyType == typeof(string))
            {
                propertyInfo.SetValue(item, value, null);
                return;
            }

            // If Type not declare Parse method.
            if (parseMethod == null)
            {
                throw new InvalidOperationException("This Type not contain Parse method");
            }

            // If receive null on value type property position , assign default value.
            value = propertyType.IsValueType
                ? value ?? Activator.CreateInstance(propertyType)
                : value?.ToString();

            // Casting to string guarantees correct argument for Parse method.
            var parsedValue = parseMethod.Invoke(null, new object[] { value?.ToString() });
            propertyInfo.SetValue(item, parsedValue, null);
        }
    }
}