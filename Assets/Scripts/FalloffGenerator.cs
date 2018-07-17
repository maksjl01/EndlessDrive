using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int size)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i,j] = Evaluate(value);
			}
		}
		return map;
	}

	public static float[,] GenerateTopRightFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateTopLeftFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Max(Mathf.Abs(1-x), Mathf.Abs(y));
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateBottomLeftFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Max(Mathf.Abs(1 - x), Mathf.Abs(1 - y));
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateBottomRightFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(1 - y));
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateBottomFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Abs(1-y);
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateLeftFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Abs(1 - x);
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateTopFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Abs(y);
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	public static float[,] GenerateRightFalloffMap(int size, float a, float b)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size;
				float y = j / (float)size;

				float value = Mathf.Abs(x);
				map[i, j] = EvaluateC(value, a, b);
			}
		}
		return map;
	}

	static float Evaluate(float value)
	{
		float a = 3f;
		float b = 2.2f;

		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}

	static float EvaluateC(float value, float a, float b)
	{
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}
