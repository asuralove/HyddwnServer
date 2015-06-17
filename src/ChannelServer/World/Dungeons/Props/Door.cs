﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Mabi;
using Aura.Mabi.Const;
using Aura.Channel.World.Entities;
using Aura.Shared.Util;

namespace Aura.Channel.World.Dungeons.Props
{
	/// <summary>
	/// A door, as found in dungeons.
	/// </summary>
	public class Door : Prop
	{
		/// <summary>
		/// Name of the door, used from puzzles.
		/// </summary>
		public string InternalName { get; protected set; }

		/// <summary>
		/// Specifies whether the door is locked or not.
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// Creates new door prop.
		/// </summary>
		/// <param name="propId"></param>
		/// <param name="regionId"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="direction">Direction the door faces in, in degree.</param>
		/// <param name="doorType"></param>
		/// <param name="name"></param>
		/// <param name="state"></param>
		public Door(int propId, int regionId, int x, int y, int direction, DungeonBlockType doorType, string name, string state = "open")
			: base(propId, regionId, x, y, direction, 1, 0, state, "", "")
		{
			this.InternalName = name;

			switch (doorType)
			{
				default:
				case DungeonBlockType.Door:
					this.Behavior = this.UnlockedBehavior;
					break;

				case DungeonBlockType.BossDoor:
				case DungeonBlockType.DoorWithLock:
					this.Behavior = LockedDoorBehavior;
					break;
			}

			if (doorType == DungeonBlockType.BossDoor)
			{
				this.Info.Direction = MabiMath.DirectionToRadian(0, 1);
				this.Info.Y += Dungeon.TileSize + Dungeon.TileSize / 2;
			}
			else
			{
				this.Info.Direction = MabiMath.DegreeToRadian(direction);
			}
		}

		/// <summary>
		/// Door's behavior when it's not locked.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="prop"></param>
		private void UnlockedBehavior(Creature creature, Prop prop)
		{
			// TODO: Allow teleporting into the room.

			// Confirmation extension added when the door closes,
			// condition might be directions, so the message only
			// appears when going in?
			//
			//Op: 0000908D, Id: 00A1273000070001
			//001 [........000000CA] Int    : 202
			//002 [........0000044C] Int    : 1100
			//003 [................] String : directed_ask(1,4)
			//004 [..............02] Byte   : 2
			//005 [................] String : message:s:Do you wish to go inside the room?;condition:s:notfrom(1,4);
		}

		/// <summary>
		/// Door's behavior when it's locked.
		/// </summary>
		/// <remarks>
		/// TODO: Couldn't this be done in one behavior?
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="prop"></param>
		private void LockedDoorBehavior(Creature creature, Prop prop)
		{
			// todo: sometimes we don't want to open a door, only unlock it and teleport in
			if (this.IsLocked)
			{
				if (this.OpenWithKey(creature))
				{
					this.Open();
					this.IsLocked = false;
					Send.Notice(creature, NoticeType.MiddleSystem, Localization.Get("You have opened the door with the key."));
				}
				else
					Send.Notice(creature, NoticeType.MiddleSystem, Localization.Get("There is no matching key."));
			}
			else
			{
				// Why would a locked door ever be opened on click without a key? [exec]
				this.Open();
			}
		}

		/// <summary>
		/// Returns true if character has the key to unlock this door,
		/// and removes it from his inventory.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		private bool OpenWithKey(Creature character)
		{
			foreach (var item in character.Inventory.Items)
			{
				if (item.Info.Id == 70029 || item.Info.Id == 70030)
				{
					if (item.MetaData1.GetString("prop_to_unlock") == this.InternalName)
					{
						character.Inventory.Remove(item);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Closes door.
		/// </summary>
		public void Close()
		{
			this.SetState("closed");
		}

		/// <summary>
		/// Opens door.
		/// </summary>
		public void Open()
		{
			this.SetState("open");
		}
	}
}
