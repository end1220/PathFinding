
using System;
using System.IO;
using System.Diagnostics;


namespace FixedPoint
{

	public class FixMatrix
	{
		private double[,] m_data;


		public FixMatrix(int row)
		{
			m_data = new double[row, row];

		}

		public FixMatrix(int row, int col)
		{
			m_data = new double[row, col];
		}

		public FixMatrix(FixMatrix m)
		{
			int row = m.Row;
			int col = m.Col;
			m_data = new double[row, col];

			for (int i = 0; i < row; i++)
				for (int j = 0; j < col; j++)
					m_data[i, j] = m[i, j];

		}

		public void SetUnit()
		{
			for (int i = 0; i < m_data.GetLength(0); i++)
				for (int j = 0; j < m_data.GetLength(1); j++)
					m_data[i, j] = ((i == j) ? 1 : 0);
		}

		public void SetValue(double d)
		{
			for (int i = 0; i < m_data.GetLength(0); i++)
				for (int j = 0; j < m_data.GetLength(1); j++)
					m_data[i, j] = d;
		}

		public int Row
		{
			get
			{
				return m_data.GetLength(0);
			}
		}

		public int Col
		{
			get
			{
				return m_data.GetLength(1);
			}
		}

		//重载索引
		//存取数据成员
		public double this[int row, int col]
		{
			get
			{
				return m_data[row, col];
			}
			set
			{
				m_data[row, col] = value;
			}
		}

		//　初等变换　对调两行：ri<-->rj
		public FixMatrix Exchange(int i, int j)
		{
			double temp;
			for (int k = 0; k < Col; k++)
			{
				temp = m_data[i, k];
				m_data[i, k] = m_data[j, k];
				m_data[j, k] = temp;
			}
			return this;
		}

		//初等变换　第index 行乘以mul
		FixMatrix Multiple(int index, double mul)
		{
			for (int j = 0; j < Col; j++)
			{
				m_data[index, j] *= mul;
			}
			return this;
		}
		//初等变换 第src行乘以mul加到第index行
		FixMatrix MultipleAdd(int index, int src, double mul)
		{
			for (int j = 0; j < Col; j++)
			{
				m_data[index, j] += m_data[src, j] * mul;
			}

			return this;
		}

		//transpose 转置
		public FixMatrix Transpose()
		{
			FixMatrix ret = new FixMatrix(Col, Row);
			for (int i = 0; i < Row; i++)
				for (int j = 0; j < Col; j++)
				{
					ret[j, i] = m_data[i, j];
				}
			return ret;
		}

		public static FixMatrix operator +(FixMatrix lhs, FixMatrix rhs)
		{
			if (lhs.Row != rhs.Row)
			{
				System.Exception e = new Exception("相加的两个矩阵的行数不等");
				throw e;
			}
			if (lhs.Col != rhs.Col)
			{
				System.Exception e = new Exception("相加的两个矩阵的列数不等");
				throw e;
			}

			int row = lhs.Row;
			int col = lhs.Col;
			FixMatrix ret = new FixMatrix(row, col);

			for (int i = 0; i < row; i++)
				for (int j = 0; j < col; j++)
				{
					double d = lhs[i, j] + rhs[i, j];
					ret[i, j] = d;
				}
			return ret;
		}

		public static FixMatrix operator -(FixMatrix lhs, FixMatrix rhs)
		{
			if (lhs.Row != rhs.Row)
			{
				System.Exception e = new Exception("相减的两个矩阵的行数不等");
				throw e;
			}
			if (lhs.Col != rhs.Col)
			{
				System.Exception e = new Exception("相减的两个矩阵的列数不等");
				throw e;
			}

			int row = lhs.Row;
			int col = lhs.Col;
			FixMatrix ret = new FixMatrix(row, col);

			for (int i = 0; i < row; i++)
				for (int j = 0; j < col; j++)
				{
					double d = lhs[i, j] - rhs[i, j];
					ret[i, j] = d;
				}
			return ret;
		}

		public static FixMatrix operator *(FixMatrix lhs, FixMatrix rhs)
		{
			if (lhs.Col != rhs.Row)
			{
				System.Exception e = new Exception("相乘的两个矩阵的行列数不匹配");
				throw e;
			}
			FixMatrix ret = new FixMatrix(lhs.Row, rhs.Col);
			double temp;
			for (int i = 0; i < lhs.Row; i++)
			{
				for (int j = 0; j < rhs.Col; j++)
				{
					temp = 0;
					for (int k = 0; k < lhs.Col; k++)
					{
						temp += lhs[i, k] * rhs[k, j];
					}
					ret[i, j] = temp;
				}
			}

			return ret;
		}

		public static FixMatrix operator /(FixMatrix lhs, FixMatrix rhs)
		{
			return lhs * rhs.Inverse();
		}

		public static FixMatrix operator +(FixMatrix m)
		{
			FixMatrix ret = new FixMatrix(m);
			return ret;
		}

		public static FixMatrix operator -(FixMatrix m)
		{
			FixMatrix ret = new FixMatrix(m);
			for (int i = 0; i < ret.Row; i++)
				for (int j = 0; j < ret.Col; j++)
				{
					ret[i, j] = -ret[i, j];
				}

			return ret;
		}

		public static FixMatrix operator *(double d, FixMatrix m)
		{
			FixMatrix ret = new FixMatrix(m);
			for (int i = 0; i < ret.Row; i++)
				for (int j = 0; j < ret.Col; j++)
					ret[i, j] *= d;

			return ret;
		}

		public static FixMatrix operator /(double d, FixMatrix m)
		{
			return d * m.Inverse();
		}

		//功能：返回列主元素的行号
		//参数：row为开始查找的行号
		//说明：在行号[row,Col)范围内查找第row列中绝对值最大的元素，返回所在行号
		int Pivot(int row)
		{
			int index = row;

			for (int i = row + 1; i < Row; i++)
			{
				if (m_data[i, row] > m_data[index, row])
					index = i;
			}

			return index;
		}

		//inversion 逆阵：使用矩阵的初等变换，列主元素消去法
		public FixMatrix Inverse()
		{
			if (Row != Col)    //异常,非方阵
			{
				System.Exception e = new Exception("求逆的矩阵不是方阵");
				throw e;
			}
			StreamWriter sw = new StreamWriter("..\\annex\\close_FixMatrix.txt");
			FixMatrix tmp = new FixMatrix(this);
			FixMatrix ret = new FixMatrix(Row);    //单位阵
			ret.SetUnit();

			int maxIndex;
			double dMul;

			for (int i = 0; i < Row; i++)
			{
				maxIndex = tmp.Pivot(i);

				if (tmp.m_data[maxIndex, i] == 0)
				{
					System.Exception e = new Exception("求逆的矩阵的行列式的值等于0,");
					throw e;
				}

				if (maxIndex != i)    //下三角阵中此列的最大值不在当前行，交换
				{
					tmp.Exchange(i, maxIndex);
					ret.Exchange(i, maxIndex);

				}

				ret.Multiple(i, 1 / tmp[i, i]);
				tmp.Multiple(i, 1 / tmp[i, i]);

				for (int j = i + 1; j < Row; j++)
				{
					dMul = -tmp[j, i] / tmp[i, i];
					tmp.MultipleAdd(j, i, dMul);

				}
				sw.WriteLine("tmp=\r\n" + tmp);
				sw.WriteLine("ret=\r\n" + ret);
			}//end for


			sw.WriteLine("**=\r\n" + this * ret);

			for (int i = Row - 1; i > 0; i--)
			{
				for (int j = i - 1; j >= 0; j--)
				{
					dMul = -tmp[j, i] / tmp[i, i];
					tmp.MultipleAdd(j, i, dMul);
					ret.MultipleAdd(j, i, dMul);
				}
			}


			sw.WriteLine("tmp=\r\n" + tmp);
			sw.WriteLine("ret=\r\n" + ret);
			sw.WriteLine("***=\r\n" + this * ret);
			sw.Close();

			return ret;

		}

		#region
		

		#endregion


		public bool IsSquare()
		{
			return Row == Col;
		}

		public bool IsSymmetric()
		{
			if (Row != Col)
				return false;
			for (int i = 0; i < Row; i++)
				for (int j = i + 1; j < Col; j++)
					if (m_data[i, j] != m_data[j, i])
						return false;
			return true;
		}

		public double ToDouble()
		{
			Trace.Assert(Row == 1 && Col == 1);

			return m_data[0, 0];
		}

		public override string ToString()
		{

			string s = "";
			for (int i = 0; i < Row; i++)
			{
				for (int j = 0; j < Col; j++)
					s += string.Format("{0} ", m_data[i, j]);

				s += "\r\n";
			}
			return s;

		}

	}

}