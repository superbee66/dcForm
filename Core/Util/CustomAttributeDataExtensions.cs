using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace dCForm.Core.Util
{
    public static class CustomAttributeDataExtensions
    {
        public static CustomAttributeBuilder ToAttributeBuilder(this CustomAttributeData _CustomAttributeData)
        {
            if (_CustomAttributeData == null)
                throw new ArgumentNullException("data");

            List<object> constructorArguments = new List<object>();

            foreach (CustomAttributeTypedArgument _CustomAttributeTypedArgument in _CustomAttributeData.ConstructorArguments)
                constructorArguments.Add(_CustomAttributeTypedArgument.Value);

            List<PropertyInfo> propertyArguments = new List<PropertyInfo>();
            List<object> propertyArgumentValues = new List<object>();
            List<FieldInfo> fieldArguments = new List<FieldInfo>();
            List<object> fieldArgumentValues = new List<object>();

            foreach (CustomAttributeNamedArgument _CustomAttributeNamedArgument in _CustomAttributeData.NamedArguments)
            {
                FieldInfo _FieldInfo = _CustomAttributeNamedArgument.MemberInfo as FieldInfo;
                PropertyInfo _PropertyInfo = _CustomAttributeNamedArgument.MemberInfo as PropertyInfo;

                if (_FieldInfo != null)
                {
                    fieldArguments.Add(_FieldInfo);
                    fieldArgumentValues.Add(_CustomAttributeNamedArgument.TypedValue.Value);
                } else if (_PropertyInfo != null)
                {
                    propertyArguments.Add(_PropertyInfo);
                    propertyArgumentValues.Add(_CustomAttributeNamedArgument.TypedValue.Value);
                }
            }

            return new CustomAttributeBuilder(
                _CustomAttributeData.Constructor,
                constructorArguments.ToArray(),
                propertyArguments.ToArray(),
                propertyArgumentValues.ToArray(),
                fieldArguments.ToArray(),
                fieldArgumentValues.ToArray());
        }
    }
}