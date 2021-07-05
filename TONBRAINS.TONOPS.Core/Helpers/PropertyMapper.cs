using System.Reflection;

namespace TONBRAINS.TONOPS.Core.Helpers
{
    public static class PropertyMapper
    {
        public static T2 FillProperties<T1, T2>(T1 original, T2 duplicate)
        {
            var originalType = typeof(T1).GetTypeInfo();
            var properties = originalType.GetProperties();

            var duplicateType = typeof(T2).GetTypeInfo();

            foreach (var property in properties)
            {
                var duplicateProperty = duplicateType.GetProperty(property.Name);
                if (duplicateProperty == null) continue;
                if (duplicateProperty.PropertyType.FullName != property.PropertyType.FullName) continue;

                duplicateProperty.SetMethod.Invoke(duplicate, new object[] { property.GetMethod.Invoke(original, null) });
            }

            return duplicate;
        }

    }
}
