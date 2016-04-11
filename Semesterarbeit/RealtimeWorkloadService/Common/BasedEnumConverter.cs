using System;

namespace RealtimeWorkloadService.Common
{
	public static class BasedEnumConverter<T>
		where T : struct, IConvertible
	{
		public static T ParseEnumFromBin(string bin)
		{
			return ParseEnumFromBased(bin, 2);
		}

		public static T ParseEnumFromOct(string oct)
		{
			return ParseEnumFromBased(oct, 8);
		}

		public static T ParseEnumFromDec(string dec)
		{
			return ParseEnumFromBased(dec, 10);
		}

		public static T ParseEnumFromHex(string hex)
		{
			return ParseEnumFromBased(hex, 16);
		}

		private static T ParseEnumFromBased(string basedvalue, int fromBase)
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			var underlyingenumtype = Enum.GetUnderlyingType(typeof(T));
			if (underlyingenumtype == typeof(byte))
				return (T)Enum.ToObject(typeof(T), Convert.ToByte(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(sbyte))
				return (T)Enum.ToObject(typeof(T), Convert.ToSByte(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(Int16))
				return (T)Enum.ToObject(typeof(T), Convert.ToInt16(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(UInt16))
				return (T)Enum.ToObject(typeof(T), Convert.ToUInt16(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(Int32))
				return (T)Enum.ToObject(typeof(T), Convert.ToInt32(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(UInt32))
				return (T)Enum.ToObject(typeof(T), Convert.ToUInt32(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(Int64))
				return (T)Enum.ToObject(typeof(T), Convert.ToInt64(basedvalue, fromBase));
			else if (underlyingenumtype == typeof(UInt64))
				return (T)Enum.ToObject(typeof(T), Convert.ToUInt64(basedvalue, fromBase));

			throw new NotSupportedException(String.Format("Not supported underlying enum datatype: {0}", underlyingenumtype));
		}
	}

	/*
	void Main()
	{
		var os = Tshort.Convesionfailed.ToStringOct().Dump();         //000120
		BasedEnumConverter<Tshort>.ParseEnumFromOct(os).Dump();       //Conversionfailed

		var stsbbin = Tsbyte.BAD.ToStringBin().Dump();                //10000000
		BasedEnumConverter<Tsbyte>.ParseEnumFromBin(stsbbin).Dump();  //BAD

		var sinthex = Tint.OK.ToStringHex().Dump();                   //00000080
		BasedEnumConverter<Tint>.ParseEnumFromHex(sinthex).Dump();    //OK
	}

	enum Tenum : byte { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tbyte : byte { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tsbyte : sbyte { A = 0, B = 1, OK = 127, OK2 = 126, BAD = -128, BAD2 = -127, Convesionfailed = 80 }
	enum Tshort : short { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tushort : ushort { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tint : int { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tuint : uint { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tlong : long { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80 }
	enum Tulong : ulong { A = 0, B = 1, OK = 128, BAD = 255, Convesionfailed = 80, Min = ulong.MinValue, Max = ulong.MaxValue }
	*/

}
