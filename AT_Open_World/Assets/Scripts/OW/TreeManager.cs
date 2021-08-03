using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager
{
	public static List<Vector2> Create(float r, Vector2 bound, int s = 15)
	{
		float gridS = r / Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(bound.x / gridS), Mathf.CeilToInt(bound.y / gridS)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> startP = new List<Vector2>();

		startP.Add(bound / 2);
		while (startP.Count > 0)
		{
			int spawnIndex = Random.Range(0, startP.Count);
			Vector2 origin = startP[spawnIndex];
			bool checkCoordTrue = false;

			for (int i = 0; i <s; i++)
			{
				float angle = Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 checkCoord = origin + dir * Random.Range(r, 2 * r);
				if (ValidCheck(checkCoord, bound, gridS, r, points, grid))
				{
					points.Add(checkCoord);
					startP.Add(checkCoord);
					grid[(int)(checkCoord.x / gridS), (int)(checkCoord.y / gridS)] = points.Count;
					checkCoordTrue = true;
					break;
				}
			}
			if (!checkCoordTrue)
			{
				startP.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	//Checks if the new point overlaps with another point. if so discard
	static bool ValidCheck(Vector2 point, Vector2 bound, float cell, float r, List<Vector2> pL, int[,] grid)
	{
		if (point.x >= 0 && point.x < bound.x && point.y >= 0 && point.y < bound.y)
		{
			int cX = (int)(point.x / cell);
			int cY = (int)(point.y / cell);
			int checkSX = Mathf.Max(0, cX - 2);
			int checkEX = Mathf.Min(cX + 2, grid.GetLength(0) - 1);
			int checkSY = Mathf.Max(0, cY - 2);
			int checkEY = Mathf.Min(cY + 2, grid.GetLength(1) - 1);

			for (int x = checkSX; x <= checkEX; x++)
			{
				for (int y = checkSY; y <= checkEY; y++)
				{
					int pI = grid[x, y] - 1;
					if (pI != -1)
					{
						float sqrDst = (point - pL[pI]).sqrMagnitude;
						if (sqrDst < r * r)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}
