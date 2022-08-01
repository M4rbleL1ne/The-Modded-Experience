using BepInEx;
using RWCustom;
using UnityEngine;
using System.Text.RegularExpressions;
using Random = UnityEngine.Random;
using Tp = CreatureTemplate.Type;

namespace TheModdedExperience;

[BepInPlugin("lb-fgf-m4r-ik.the-modded-experience", nameof(TheModdedExperience), "0.1.0")]
sealed class TheModdedExperiencePlugin : BaseUnityPlugin
{
    public void OnEnable()
	{
		static Tp? TryParse(string value)
        {
            try
            {
				return Custom.ParseEnum<Tp>(value);
            }
            catch
            {
				return null;
            }
        }
		On.RainWorld.Start += (orig, self) =>
		{
			orig(self);
			Debug.Log("TheModdedExperience will use the following modded creatures: ");
			Debug.Log($"Basilisk: {TryParse("Basilisk") is not null}");
			Debug.Log($"Blizzard: {TryParse("Blizzard") is not null}");
			Debug.Log($"ProtoLizard: {TryParse("ProtoLizard") is not null}");
			Debug.Log($"ReaperLizard: {TryParse("ReaperLizard") is not null}");
			Debug.Log($"SilverLizard: {TryParse("SilverLizard") is not null}");
			Debug.Log($"WaterSpitter: {TryParse("WaterSpitter") is not null}");
			Debug.Log($"Polliwog: {TryParse("Polliwog") is not null}");
			Debug.Log($"HunterSeeker: {TryParse("HunterSeeker") is not null}");
			Debug.Log($"Scutigera: {TryParse("Scutigera") is not null}");
			Debug.Log($"BouncingBall: {TryParse("BouncingBall") is not null}");
			Debug.Log($"WaterBlob: {TryParse("WaterBlob") is not null}");
			Debug.Log($"Mosquito: {TryParse("Mosquito") is not null}");
		};
		/*On.WorldLoader.CreatureTypeFromString += (orig, s) =>
		{
			var r = orig(s);
			switch (TryParse(sAr[0]))
			{
				case Tp.PinkLizard:
					if (seed <= .3f)
						type = TryParse("Basilisk");
					else if (seed <= .5f)
						type = TryParse("Blizzard");
					break;
				case Tp.BlueLizard:
					if (seed <= .2f)
						type = TryParse("ProtoLizard");
					break;
				case Tp.RedLizard:
					if (seed <= .3f)
						type = TryParse("ReaperLizard");
					break;
				case Tp.GreenLizard:
					if (seed <= .3f)
						type = TryParse("SilverLizard");
					else if (seed <= .5f)
						type = TryParse("WaterSpitter");
					break;
				case Tp.YellowLizard:
					if (seed <= .3f)
						type = TryParse("Polliwog");
					break;
				case Tp.Salamander:
					if (seed <= .3f)
						type = TryParse("Polliwog");
					else if (seed <= .5f)
						type = TryParse("WaterSpitter");
					break;
				case Tp.WhiteLizard:
					if (seed <= .3f)
						type = TryParse("HunterSeeker");
					break;
				case Tp.CyanLizard:
					if (seed <= .3f)
						type = TryParse("HunterSeeker");
					else if (seed <= .4f)
						type = TryParse("ReaperLizard");
					break;
				case Tp.Centipede:
					if (seed <= .3f)
						type = TryParse("Scutigera");
					break;
				case Tp.Snail:
					if (seed <= .3f)
						type = TryParse("BouncingBall");
					break;
				case Tp.Hazer:
					if (seed <= .3f)
						r = TryParse("WaterBlob");
					break;
				case Tp.SmallNeedleWorm:
					if (seed <= .3f)
						r = TryParse("Mosquito");
					break;
			}
			return r;
		};*/
        On.SaveState.AbstractCreatureFromString += (orig, world, creatureString, onlyInCurrentRegion) =>
        {
			var sAr = Regex.Split(creatureString, "<cA>");
			var seed = Random.seed;
			Tp? type = null;
			Random.seed = EntityID.FromString(sAr[1]).number;
			switch (TryParse(sAr[0]))
            {
				case Tp.PinkLizard:
					if (Random.value <= .4f) 
						type = TryParse("Basilisk");
					else if (Random.value <= .8f)
						type = TryParse("Blizzard");
					break;
				case Tp.BlueLizard:
					if (Random.value <= .5f)
						type = TryParse("ProtoLizard");
					break;
				case Tp.RedLizard:
					if (Random.value <= .5f)
						type = TryParse("ReaperLizard");
					break;
				case Tp.GreenLizard:
					if (Random.value <= .4f)
						type = TryParse("SilverLizard");
					else if (Random.value <= .8f)
						type = TryParse("WaterSpitter");
					break;
				case Tp.YellowLizard:
					if (Random.value <= .5f)
						type = TryParse("Polliwog");
					break;
				case Tp.Salamander:
					if (Random.value <= .4f)
						type = TryParse("Polliwog");
					else if (Random.value <= .8f)
						type = TryParse("WaterSpitter");
					break;
				case Tp.WhiteLizard:
					if (Random.value <= .5f)
						type = TryParse("HunterSeeker");
					break;
				case Tp.CyanLizard:
					if (Random.value <= .4f)
						type = TryParse("HunterSeeker");
					else if (Random.value <= .8f)
						type = TryParse("ReaperLizard");
					break;
				case Tp.Centipede:
					if (Random.value <= .5f)
						type = TryParse("Scutigera");
					break;
				case Tp.Snail:
					if (Random.value <= .5f)
						type = TryParse("BouncingBall");
					break;
				case Tp.Hazer:
					if (Random.value <= .5f)
						type = TryParse("WaterBlob");
					break;
				case Tp.SmallNeedleWorm:
					if (Random.value <= .5f)
						type = TryParse("Mosquito");
					break;
			}
			Random.seed = seed;
			if (type is not null)
			{
				WorldCoordinate worldCoordinate = new(int.Parse(sAr[2].Split('.')[0]), -1, -1, int.Parse(sAr[2].Split('.')[1]));
				var iD = EntityID.FromString(sAr[1]);
				if (onlyInCurrentRegion && (worldCoordinate.room < world.firstRoomIndex || worldCoordinate.room >= world.firstRoomIndex + world.NumberOfRooms))
				{
					Debug.Log("Creature trying to spawn out of region: " + creatureString + " r:" + worldCoordinate.room + " fr:" + world.firstRoomIndex + " lr:" + (world.firstRoomIndex + world.NumberOfRooms));
					if (world.GetSpawner(iD) is null)
					{
						Debug.Log("Spawner out of region " + ((world.region is null) ? string.Empty : world.region.name) + " ~ " + iD.spawner + " creature not spawning");
						return null;
					}
					worldCoordinate = world.GetSpawner(iD).den;
					Debug.Log("Spawner is in region. Moving to original spawn den : " + worldCoordinate.ToString());
				}
				AbstractCreature abstractCreature = new(world, StaticWorld.GetCreatureTemplate(type.Value), null, worldCoordinate, iD);
				abstractCreature.state.LoadFromString(Regex.Split(sAr[3], "<cB>"));
				return abstractCreature;
			}
			return orig(world, creatureString, onlyInCurrentRegion);
		};
    }
}