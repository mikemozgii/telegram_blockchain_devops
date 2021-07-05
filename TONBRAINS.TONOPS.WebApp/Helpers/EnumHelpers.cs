using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Common;

namespace TONBRAINS.TONOPS.WebApp.Helpers
{
	public static class EnumHelpers<TEnum> where TEnum : Enum
	{
		public static IEnumerable<TEnum> GetEnumValues()
		{
			var types = Enum.GetValues(typeof(TEnum));
			foreach (var type in types)
				yield return (TEnum)type;
		}

		public static IEnumerable<SelectListItem<int>> GetEnumSelectList()
			=> GetEnumValues().Select(x => new SelectListItem<int>(x.GetValue(), x.GetDescription()));
	}
}
