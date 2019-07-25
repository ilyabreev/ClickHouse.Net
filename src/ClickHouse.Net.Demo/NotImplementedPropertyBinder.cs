using System;

namespace ClickHouse.Net.Demo
{
    public class NotImplementedPropertyBinder : IPropertyBinder
    {
        public void BindProperty(object item, string propertyName, object value)
        {
            throw new NotImplementedException("Custom realization of property binder");
        }
    }
}