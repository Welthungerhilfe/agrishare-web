/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Agrishare.Core
{
    public static class EnumInfo
    {
        public static List<EnumDescriptor> ToList<TEnum>() where TEnum : struct
        {
            var list = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            return list.Select(e => new EnumDescriptor
            {
                Id = Convert.ToInt32(e),
                Title = $"{e}".ExplodeCamelCase()
            }).ToList();
        }
    }

    public class EnumDescriptor
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
