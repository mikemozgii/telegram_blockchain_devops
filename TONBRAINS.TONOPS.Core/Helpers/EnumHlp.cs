using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public static class EnumHlp
    {
		public static string GetDescription(this Enum en, int attrNum = default)
		{
			var type = en.GetType();
			var memInfo = type.GetMember(en.ToString());
			if (memInfo.Length <= 0) return en.ToString();
			var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attrs.Length > 0 ? ((DescriptionAttribute)attrs[attrNum]).Description : en.ToString();
		}

		public static int GetValue(this Enum en)
			=> Convert.ToInt32(Enum.Parse(en.GetType(), en.ToString()));
	}


}
