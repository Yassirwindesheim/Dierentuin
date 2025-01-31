using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumHelper
{
    public static IEnumerable<SelectListItem> GetSelectList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => new SelectListItem
                   {
                       Value = e.ToString(),
                       Text = e.ToString()
                   });
    }
}