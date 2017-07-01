

public static class ByteTool
{

	public static void IntToBytes(byte[] bt, int index, int value)
	{
		bt[index++] = (byte)(value >> 24);
		bt[index++] = (byte)(value >> 16);
		bt[index++] = (byte)(value >> 8);
		bt[index] = (byte)(value >> 0);
	}

	public static int IntFromBytes(byte[] bt, int index)
	{
		int value = 0;
		value |= (int)bt[index++] << 24;
		value |= (int)bt[index++] << 16;
		value |= (int)bt[index++] << 8;
		value |= (int)bt[index] << 0;
		return value;
	}

	public static void ShortToBytes(byte[] bt, int index, short value)
	{
		bt[index++] = (byte)(value >> 8);
		bt[index] = (byte)(value >> 0);
	}

	public static short ShortFromBytes(byte[] bt, int index)
	{
		short value = 0;
		value |= (short)((int)bt[index++] << 8);
		value |= (short)((int)bt[index] << 0);
		return value;
	}

	public static void UshortToBytes(byte[] bt, int index, ushort value)
	{
		bt[index++] = (byte)(value >> 8);
		bt[index] = (byte)(value >> 0);
	}

	public static ushort UshortFromBytes(byte[] bt, int index)
	{
		ushort value = 0;
		value |= (ushort)((int)bt[index++] << 8);
		value |= (ushort)((int)bt[index] << 0);
		return value;
	}


}