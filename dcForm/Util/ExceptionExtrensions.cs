﻿using System;
using System.Linq;

namespace dCForm.Util
{
    public static class ExceptionExtrensions
    {
        public static string AsString(this Exception exception)
        {
            return string.Join("\n", exception
                .GetType()
                .GetProperties()
                .Select(property => new
                {
                    property.Name,
                    Value = property.GetValue(exception, null)
                })
                .Select(x => string.Format(
                    "{0} = {1}",
                    x.Name,
                    x.Value?.ToString() ?? string.Empty
                                 )));
        }
    }
}