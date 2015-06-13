﻿using System.Collections.Generic;
using Aura.Shared.Util;

namespace Aura.Channel.World.Dungeons.Puzzles
{
	public enum DungeonPropPositionType
	{
		Corner4,
		Corner8,
		Center9,
		Center,
		Random,
	}

	public class DungeonPropPositionProvider
	{
		private static readonly int[,] _corner4Offsets =
			{
				{ -1,1,315 }, { 1,1,225 },
				{ -1,-1,45 }, { 1,-1,135 },
			};
		private static readonly int[,] _corner8Offsets =
			{
				{ -1,1,315 }, { 0,1,270 }, { 1,1,225 },
				{ -1,0,0 },                { 1,0,180 },
				{ -1,-1,45 }, { 0,-1,90 }, { 1,-1,135 },
			};
		private static readonly int[,] _center9Offsets =
			{
				{ -1,1,0 }, { 0,1,0 }, { 1,1,0 },
				{ -1,0,0 }, { 0,0,0 }, { 1,0,0 },
				{ -1,-1,0 },{ 0,-1,0 },{ 1,-1,0 },
			};
		private static readonly int[,] _centerOffset =
			{
				{ 0,0,0 },
			};

		private Queue<int[]> _positionQueue;
		private DungeonPropPositionType _positionType;
		private int _radius;

		public DungeonPropPositionProvider(DungeonPropPositionType positionType, int radius=600)
		{
			_positionQueue = new Queue<int[]>();
			_positionType = positionType;
			_radius = radius;
			int[,] offsets = null;
			switch (positionType)
			{
				case DungeonPropPositionType.Corner4:
					offsets = _corner4Offsets;
					break;
				case DungeonPropPositionType.Corner8:
					offsets = _corner8Offsets;
					break;
				case DungeonPropPositionType.Center9:
					offsets = _center9Offsets;
					break;
				case DungeonPropPositionType.Center:
					offsets = _centerOffset;
					break;
				case DungeonPropPositionType.Random:
					return;
			}
			if (offsets == null) return;
			// should we shuffle ?
			for (int i = 0; i < offsets.GetLength(0); i++)
				_positionQueue.Enqueue(new int[] { offsets[i, 0] * radius, offsets[i, 1] * radius, offsets[i, 2] });
		}

		public int[] GetPosition()
		{
			if (_positionType == DungeonPropPositionType.Random)
			{
				var rnd = RandomProvider.Get();
				return new int[] { rnd.Next(-_radius, _radius), rnd.Next(-_radius, _radius), rnd.Next(360) };
			}
			return _positionQueue.Dequeue();
		}

	}
}