using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.Helpers;

namespace Nop.Plugin.Api.Common.Validators
{
    public class TypeValidator<T>
    {
        public TypeValidator() => InvalidProperties = new List<string>();

        public List<string> InvalidProperties { get; set; }

        public bool IsValid(Dictionary<string, object> propertyValuePaires)
        {
            var isValid = true;

            var jsonPropertyNameTypePair = new Dictionary<string, Type>();

            PropertyInfo[] objectProperties = typeof(T).GetProperties();

            foreach (PropertyInfo property in objectProperties)
                if (property.GetCustomAttribute(typeof(JsonPropertyAttribute)) is JsonPropertyAttribute jsonPropertyAttribute)
                    if (!jsonPropertyNameTypePair.ContainsKey(jsonPropertyAttribute.PropertyName))
                        jsonPropertyNameTypePair.Add(jsonPropertyAttribute.PropertyName, property.PropertyType);

            foreach (KeyValuePair<string, object> pair in propertyValuePaires)
            {
                var isCurrentPropertyValid = true;

                if (jsonPropertyNameTypePair.ContainsKey(pair.Key))
                {
                    Type propertyType = jsonPropertyNameTypePair[pair.Key];

                    // handle nested properties
                    if (pair.Value is Dictionary<string, object> objects)
                    {
                        isCurrentPropertyValid = ValidateNestedProperty(propertyType, objects);
                    }
                    // This case hadles collections.
                    else if (pair.Value is ICollection<object> propertyValueAsCollection && propertyType.GetInterface("IEnumerable") != null)
                    {
                        Type elementsType = ReflectionHelper.GetGenericElementType(propertyType);

                        // Validate the collection items.
                        foreach (object item in propertyValueAsCollection)
                        {
                            isCurrentPropertyValid = IsCurrentPropertyValid(elementsType, item);

                            if (!isCurrentPropertyValid) break;
                        }
                    }
                    else
                    {
                        isCurrentPropertyValid = IsCurrentPropertyValid(jsonPropertyNameTypePair[pair.Key], pair.Value);
                    }

                    if (!isCurrentPropertyValid)
                    {
                        isValid = false;
                        InvalidProperties.Add(pair.Key);
                    }
                }
            }

            return isValid;
        }

        private static bool IsCurrentPropertyValid(Type type, object value)
        {
            var isCurrentPropertyValid = true;

            if (type.Namespace == "System")
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);

                object valueToValidate = value;

                // This is needed because the isValid method does not work well if the value it is trying to validate is object.
                if (value != null) valueToValidate = string.Format(CultureInfo.InvariantCulture, "{0}", value);

                if (!converter.IsValid(valueToValidate)) isCurrentPropertyValid = false;
            }
            else
            {
                if (value != null) isCurrentPropertyValid = ValidateNestedProperty(type, (Dictionary<string, object>) value);
            }

            return isCurrentPropertyValid;
        }

        private static bool ValidateNestedProperty(Type propertyType, Dictionary<string, object> value)
        {
            Type constructedType = typeof(TypeValidator<>).MakeGenericType(propertyType);
            object typeValidatorForNestedProperty = Activator.CreateInstance(constructedType);

            MethodInfo isValidMethod = constructedType.GetMethod("IsValid");

            var isCurrentPropertyValid = (bool) isValidMethod.Invoke(typeValidatorForNestedProperty, new object[] {value});

            return isCurrentPropertyValid;
        }
    }
}