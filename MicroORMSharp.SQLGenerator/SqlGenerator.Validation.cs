using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public void ValidateAttributes(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var prop in DataProperties)
            {
                ValidateProp(prop, obj);
            }
        }

        public void ValidateAttributes(T obj, IEnumerable<PropertyInfo> properties)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            foreach (var prop in properties.Distinct())
            {
                ValidateProp(prop, obj);
            }
        }

        public void ValidateAttributes(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                ValidateAttributes(entity);
            }
        }

        private void ValidateProp(PropertyInfo prop, T obj)
        {
            var metadata = GetPropertyMetadata(prop);

            //Validate MaxLength for string properties
            if (!metadata.MaxLength.HasValue)
            {
                return;
            }

            if (!(prop.GetValue(obj) is string value))
            {
                return;
            }

            if (value.Length > metadata.MaxLength.Value)
            {
                throw new InvalidOperationException(
                    $"Value for {prop.DeclaringType?.FullName}.{prop.Name} exceeds DbMaxLength({metadata.MaxLength.Value}). Actual length: {value.Length}."
                );
            }
        }
    }
}
