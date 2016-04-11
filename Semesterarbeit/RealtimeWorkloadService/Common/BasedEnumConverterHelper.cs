using System;

namespace RealtimeWorkloadService.Common
{
	public static class BasedEnumConverterHelper
	{
		public static string ToStringBin<T>(this T enumval)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return ToStringBased(enumval, 2);
		}

		public static string ToStringOct<T>(this T enumval)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return ToStringBased(enumval, 8);
		}

		public static string ToStringDec<T>(this T enumval)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return ToStringBased(enumval, 10);
		}

		public static string ToStringHex<T>(this T enumval)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return ToStringBased(enumval, 16);
		}

		private static string ToStringBased<T>(this T enumval, int toBase)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			BasedValueConverter bvc;
			var underlyingenumtype = Enum.GetUnderlyingType(typeof(T));
			if (underlyingenumtype == typeof(byte))
				bvc = new BasedValueConverter((byte)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(sbyte))
				bvc = new BasedValueConverter((sbyte)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(short))
				bvc = new BasedValueConverter((short)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(ushort))
				bvc = new BasedValueConverter((ushort)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(int))
				bvc = new BasedValueConverter((int)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(uint))
				bvc = new BasedValueConverter((uint)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(long))
				bvc = new BasedValueConverter((long)Enum.ToObject(typeof(T), enumval));
			else if (underlyingenumtype == typeof(ulong))
				bvc = new BasedValueConverter((ulong)Enum.ToObject(typeof(T), enumval));
			else
				throw new NotSupportedException(String.Format("Not supported underlying enum datatype: {0}", underlyingenumtype));

			string res = string.Empty;
			switch (toBase)
			{
				case 2:
					res = bvc.ToStringBin();
					break;
				case 8:
					res = bvc.ToStringOct();
					break;
				case 10:
					res = bvc.ToStringDec();
					break;
				case 16:
					res = bvc.ToStringHex();
					break;
				default:
					throw new NotSupportedException(String.Format("toBase '{0}' not supported, supported values are: 2=Bin, 8=Oct, 10=Dec, 16=Hex", toBase));
			}

			return res.ToUpper();
		}
	}
}
