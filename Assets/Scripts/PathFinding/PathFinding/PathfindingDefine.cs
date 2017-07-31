

namespace PathFinding
{

	// 地表类型
	public enum TerrainType
	{
		Default = 0,
		Walkable,
		Unwalkable,
		ShortWall,
		TallWall,
	}


	// 移动方式
	public enum MoveType
	{
		Normal,     // 普通行走
		Charge,     // 冲刺
		Blink       // 闪现
	}

}