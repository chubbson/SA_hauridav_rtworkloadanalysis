using System;

namespace RealtimeWorkloadService.Common
{
	public class BasedValueConverter
	{
		private readonly long value;
		private enum ValueType : byte { SByte, Byte, Short, UShort, Int, UInt, Long, ULong }
		private enum Basement : byte { Bin, Oct, Dec, Hex }
		private readonly byte[,] MaxLength =
					{   //[type,basement]
					{      8,     3,     3,     2}, // 0=SByte
					{      8,     3,     3,     2}, // 1=Byte
					{     16,     6,     5,     4}, // 2=Short
					{     16,     6,     5,     4}, // 3=UShort
					{     32,    11,    10,     8}, // 4=Int
					{     32,    11,    10,     8}, // 5=UInt
					{     64,    22,    19,    16}, // 6=Long
					{     64,    22,    20,    16}  // 7=ULong
					// 0=Bin, 1=Oct, 2=Dec, 3=Hex
				};
		private readonly ValueType valueType;

		private BasedValueConverter()
		{ }

		public BasedValueConverter(byte value)
		{
			this.value = value;
			valueType = ValueType.Byte;
		}

		public BasedValueConverter(sbyte value)
		{
			this.value = value;
			valueType = ValueType.SByte;
		}

		public BasedValueConverter(short value)
		{
			this.value = value;
			valueType = ValueType.Short;
		}

		public BasedValueConverter(ushort value)
		{
			this.value = value;
			valueType = ValueType.UShort;
		}

		public BasedValueConverter(int value)
		{
			this.value = value;
			valueType = ValueType.Int;
		}

		public BasedValueConverter(uint value)
		{
			this.value = value;
			valueType = ValueType.UInt;
		}

		public BasedValueConverter(long value)
		{
			this.value = value;
			valueType = ValueType.Long;
		}

		public BasedValueConverter(ulong value)
		{
			this.value = (long)value;
			valueType = ValueType.ULong;
		}

		public string ToStringBin()
		{
			return ToBasedString(Basement.Bin);
		}

		public string ToStringOct()
		{
			return ToBasedString(Basement.Oct);
		}

		public string ToStringDec()
		{
			return ToBasedString(Basement.Dec);
		}

		public string ToStringHex()
		{
			return ToBasedString(Basement.Hex);
		}

		private string ToBasedString(Basement basement)
		{
			int toBase;
			byte maxlength = GetMaxLength(this.valueType, basement);
			switch (basement)
			{
				case Basement.Bin:
					toBase = 2;
					break;
				case Basement.Oct:
					toBase = 8;
					break;
				case Basement.Dec:
					toBase = 10;
					break;
				case Basement.Hex:
					toBase = 16;
					break;
				default:
					throw new NotSupportedException(String.Format("Basement '{0}' not supported, supported values are: 2=Bin, 8=Oct, 10=Dec, 16=Hex", basement));
			}

			string result = string.Empty;
			switch (valueType)
			{
				case ValueType.Byte:
					result = Convert.ToString((byte)this.value, toBase);
					break;
				case ValueType.SByte:
					// no base sopport for sbyte
					//result = Convert.ToString((sbyte)this.value,basement);
					byte bval = (byte)this.value;
					result = Convert.ToString(bval, toBase);
					break;
				case ValueType.Short:
					result = Convert.ToString((short)this.value, toBase);
					break;
				case ValueType.UShort:
					// no base sopport for ushort
					//result = Convert.ToString((ushort)this.value,basement);
					short sval = (short)this.value;
					result = Convert.ToString(sval, toBase);
					break;
				case ValueType.Int:
					result = Convert.ToString((int)this.value, toBase);
					break;
				case ValueType.UInt:
					// no base sopport for uint
					//result = Convert.ToString((uint)this.value,basement);
					int ival = (int)this.value;
					result = Convert.ToString(ival, toBase);
					break;
				case ValueType.Long:
					result = Convert.ToString((long)this.value, toBase);
					break;
				case ValueType.ULong:
					// no base sopport for ulong
					//result = Convert.ToString((ulong)this.value,basement);
					long lval = (long)this.value;
					result = Convert.ToString(lval, toBase);
					break;
				default:
					throw new NotSupportedException(String.Format("valueType '{0}' not supported, supported values are: 0=SByte, 1=Byte, 2=Short, 3=UShort, 4=Int, 5=UInt, 6=Long, 7=ULong", valueType));
			}


			switch (basement)
			{
				case Basement.Bin:
					result = result.PadLeft(maxlength, '0');
					break;
				case Basement.Oct:
					result = result.PadLeft(maxlength, '0');
					break;
				case Basement.Dec:
					result = string.Format(string.Concat("{0:D", maxlength, "}"), this.value);
					break;
				case Basement.Hex:
					result = result.ToUpper().PadLeft(maxlength, '0');
					//result = string.Format(string.Concat("{0:X",maxlength,"}"), this.value); // cuz of type long, strange behaviour
					break;
				default:
					throw new NotSupportedException(String.Format("Basement '{0}' not supported, supported values are: 2=Bin, 8=Oct, 10=Dec, 16=Hex", basement));
			}

			return result;
		}

		private byte GetMaxLength(ValueType valType, Basement basement)
		{
			return MaxLength[(int)valueType, (int)basement];
		}
	}

	/*
	
	void Main()
{
	BasedValueConverter bvc;
	bvc = new BasedValueConverter(byte.MaxValue.Dump("BasedValueConverter byte"));
	bvc.ToStringBin().Dump();  // 11111111
	bvc.ToStringOct().Dump();  // 377
	bvc.ToStringDec().Dump();  // 255
	bvc.ToStringHex().Dump();  // FF
	
	bvc = new BasedValueConverter(byte.MinValue);
	bvc.ToStringBin().Dump();  // 00000000
	bvc.ToStringOct().Dump();  // 000
	bvc.ToStringDec().Dump();  // 000
	bvc.ToStringHex().Dump();  // 00
	
	
	
	bvc = new BasedValueConverter(sbyte.MaxValue.Dump("BasedValueConverter sbyte"));
	bvc.ToStringBin().Dump();  // 01111111
	bvc.ToStringOct().Dump();  // 177
	bvc.ToStringDec().Dump();  // 127
	bvc.ToStringHex().Dump();  // 7F
	
	bvc = new BasedValueConverter((sbyte)-1);
	bvc.ToStringBin().Dump();  // 11111111
	bvc.ToStringOct().Dump();  // 377
	bvc.ToStringDec().Dump();  // -001
	bvc.ToStringHex().Dump();  // FF
	
	
	
	bvc = new BasedValueConverter(short.MaxValue.Dump("BasedValueConverter short"));
	bvc.ToStringBin().Dump();  // 0111111111111111
	bvc.ToStringOct().Dump();  // 077777 
	bvc.ToStringDec().Dump();  // 32767
	bvc.ToStringHex().Dump();  // 7FFF
	
	bvc = new BasedValueConverter((short)-1);
	bvc.ToStringBin().Dump();  // 1111111111111111
	bvc.ToStringOct().Dump();  // 177777
	bvc.ToStringDec().Dump();  // -00001
	bvc.ToStringHex().Dump();  // FFFF
	
	
	
	bvc = new BasedValueConverter(ushort.MaxValue.Dump("BasedValueConverter ushort"));
	bvc.ToStringBin().Dump();  // 1111111111111111
	bvc.ToStringOct().Dump();  // 177777
	bvc.ToStringDec().Dump();  // 65535
	bvc.ToStringHex().Dump();  // FFFF
	
	bvc = new BasedValueConverter(ushort.MinValue);
	bvc.ToStringBin().Dump();  // 0000000000000000
	bvc.ToStringOct().Dump();  // 000000
	bvc.ToStringDec().Dump();  // 00000
	bvc.ToStringHex().Dump();  // 0000
	
	
	
	bvc = new BasedValueConverter(int.MaxValue.Dump("BasedValueConverter int"));
	bvc.ToStringBin().Dump();  // 01111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 17777777777
	bvc.ToStringDec().Dump();  // 2147483647
	bvc.ToStringHex().Dump();  // 7FFFFFFF
	
	bvc = new BasedValueConverter((int)-1);
	bvc.ToStringBin().Dump();  // 11111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 37777777777
	bvc.ToStringDec().Dump();  // -0000000001
	bvc.ToStringHex().Dump();  // FFFFFFFF
	
	
	
	bvc = new BasedValueConverter(uint.MaxValue.Dump("BasedValueConverter uint"));
	bvc.ToStringBin().Dump();  // 11111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 37777777777
	bvc.ToStringDec().Dump();  // 4294967295
	bvc.ToStringHex().Dump();  // FFFFFFFF
	
	bvc = new BasedValueConverter(uint.MinValue);
	bvc.ToStringBin().Dump();  // 00000000000000000000000000000000
	bvc.ToStringOct().Dump();  // 00000000000
	bvc.ToStringDec().Dump();  // 0000000000
	bvc.ToStringHex().Dump();  // 00000000
	
	
	
	bvc = new BasedValueConverter(long.MaxValue.Dump("BasedValueConverter long"));
	bvc.ToStringBin().Dump();  // 0111111111111111111111111111111111111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 0777777777777777777777
	bvc.ToStringDec().Dump();  // 9223372036854775807
	bvc.ToStringHex().Dump();  // 7FFFFFFFFFFFFFFF
	
	bvc = new BasedValueConverter((long)-1);
	bvc.ToStringBin().Dump();  // 1111111111111111111111111111111111111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 1777777777777777777777
	bvc.ToStringDec().Dump();  // -00000000000000000001
	bvc.ToStringHex().Dump();  // FFFFFFFFFFFFFFFF
	
	
	
	bvc = new BasedValueConverter(ulong.MaxValue.Dump("BasedValueConverter ulong"));
	bvc.ToStringBin().Dump();  // 1111111111111111111111111111111111111111111111111111111111111111
	bvc.ToStringOct().Dump();  // 1777777777777777777777
	bvc.ToStringDec().Dump();  // -00000000000000000001
	bvc.ToStringHex().Dump();  // FFFFFFFFFFFFFFFF
	
	bvc = new BasedValueConverter(ulong.MinValue);
	bvc.ToStringBin().Dump();  // 0000000000000000000000000000000000000000000000000000000000000000
	bvc.ToStringOct().Dump();  // 0000000000000000000000
	bvc.ToStringDec().Dump();  // 00000000000000000000
	bvc.ToStringHex().Dump();  // 0000000000000000
}
	
	*/
}
