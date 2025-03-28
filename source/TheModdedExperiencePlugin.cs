using BepInEx;
using Random = UnityEngine.Random;
using Tp = CreatureTemplate.Type;
using Tp1 = LBMergedMods.Enums.CreatureTemplateType;
using Tp2 = ShroudedAssembly.Creatures.CreatureTemplateType;
using static LBMergedMods.Hooks.AbstractPhysicalObjectHooks;
using TpM = MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType;
using System;
using LBMergedMods.Creatures;
using System.Security.Permissions;
using System.Security;

#pragma warning disable CS0618 // ignore false message
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace TheModdedExperience;

[BepInPlugin("lb-fgf-m4r-ik.the-modded-experience", "The Modded Experience", "1.1.0"),
	BepInDependency("lb-fgf-m4r-ik.modpack"),
	BepInDependency("com.rainworldgame.shroudedassembly.plugin")]
public sealed class TheModdedExperiencePlugin : BaseUnityPlugin
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed class NoStateCopyIsIntendedAttribute : Attribute { }

    public void OnEnable()
	{
        On.SaveState.AbstractCreatureFromString += On_SaveState_AbstractCreatureFromString;
        On.WorldLoader.CreatureTypeFromString += On_WorldLoader_CreatureTypeFromString;
        On.Fly.ctor += On_Fly_ctor;
        On.Hazer.Update += On_Hazer_Update;
        On.DaddyLongLegs.ctor += On_DaddyLongLegs_ctor;
    }

    static void On_DaddyLongLegs_ctor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world)
    {
        var state = Random.state;
        Random.InitState(abstractCreature.ID.RandomSeed);
        if (Random.value < .5f && world.game?.session is not SandboxGameSession)
        {
            if (!Jelly.TryGetValue(abstractCreature, out var props))
                Jelly.Add(abstractCreature, props = new());
            props.Born = true;
            props.IsJelly = true;
        }
        Random.state = state;
        orig(self, abstractCreature, world);
    }

    static void On_Hazer_Update(On.Hazer.orig_Update orig, Hazer self, bool eu)
    {
        orig(self, eu);
        var state = Random.state;
        Random.InitState(self.abstractPhysicalObject.ID.RandomSeed);
        var rand = Random.value;
        if (rand < .5f && self.room is Room rm && !self.slatedForDeletetion && self.abstractPhysicalObject.world.game?.session is not SandboxGameSession)
        {
            AbstractCreature abstractMom;
            rm.abstractRoom.AddEntity(abstractMom = new(rm.world, StaticWorld.GetCreatureTemplate(Tp1.HazerMom), null, rm.GetWorldCoordinate(self.firstChunk.pos), self.abstractPhysicalObject.ID));
            abstractMom.RealizeInRoom();
            var rlMom = (abstractMom.realizedObject as HazerMom)!;
            rlMom.dead = self.dead;
            if (self.dead)
                rlMom.InkLeft = 0f;
            abstractMom.superSizeMe = Random.value < .5f;
            rlMom.firstChunk.HardSetPosition(rm.MiddleOfTile(self.firstChunk.pos));
            self.Destroy();
        }
        else if (rand < 2f / 3f && Albino.TryGetValue(self.abstractCreature, out var props))
            props.Value = true;
        Random.state = state;
    }

    static void On_Fly_ctor(On.Fly.orig_ctor orig, Fly self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        var state = Random.state;
        Random.InitState(abstractCreature.ID.RandomSeed);
        if (Random.value < .5f && world.game?.session is not SandboxGameSession && Seed.TryGetValue(self.abstractCreature, out var props))
        {
            props.IsSeed = true;
            props.Born = true;
        }
        Random.state = state;
    }

    static Tp? On_WorldLoader_CreatureTypeFromString(On.WorldLoader.orig_CreatureTypeFromString orig, string s)
    {
        var type = orig(s);
        float rand;
        if (type is null)
            return type;
        if (type == Tp.MirosBird)
        {
            if (Random.value < .5f)
                type = Tp1.Blizzor;
        }
        else if (type == Tp.Snail)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.BouncingBall;
            else if (rand < 2f / 3f)
                type = Tp1.WaterBlob;
        }
        else if (type == Tp.Spider)
        {
            if (Random.value < 1f / 3f)
                type = Tp1.ChipChop;
        }
        else if (type == TpM.Yeek)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.ChipChop;
            else if (rand < 2f / 3f)
                type = Tp1.WaterBlob;
        }
        else if (type == Tp.JetFish || type == Tp.BigEel || type == TpM.AquaCenti || type == TpM.BigJelly)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.CommonEel;
            else if (rand < 2f / 3f)
                type = Tp1.MiniLeviathan;
        }
        else if (type == Tp.PoleMimic)
        {
            if (Random.value < .5f)
                type = Tp1.Denture;
        }
        else if (type == Tp.TentaclePlant || type == TpM.StowawayBug)
        {
            if (Random.value < .5f)
                type = Tp1.Denture;
        }
        else if (type == Tp.SeaLeech || type == TpM.JungleLeech)
        {
            rand = Random.value;
            if (rand < .1f)
                type = Tp1.DivingBeetle;
            else if (rand < 1f / 3f)
                type = Tp1.MiniBlackLeech;
        }
        else if (type == TpM.EelLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.DivingBeetle;
            else if (rand < 2f / 3f)
                type = Tp1.CommonEel;
        }
        else if (type == Tp.KingVulture || type == Tp.Vulture || type == TpM.MirosVulture)
        {
            rand = Random.value;
            if (rand < .25f)
                type = Tp1.FlyingBigEel;
            else if (rand < .75f)
                type = Tp1.FatFireFly;
        }
        else if (type == Tp.BigSpider || type == TpM.MotherSpider)
        {
            rand = Random.value;
            if (rand < .5f)
                type = Tp1.Glowpillar;
            else if (rand < .6f)
                type = Tp2.MaracaSpider;
        }
        else if (type == Tp.Hazer)
        {
            if (Random.value < .5f)
                type = Tp1.HazerMom;
        }
        else if (type == Tp.CicadaA || type == Tp.CicadaB)
        {
            if (Random.value < .5f)
                type = Tp1.HazerMom;
        }
        else if (type == Tp.CyanLizard || type == Tp.WhiteLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.HunterSeeker;
            else if (rand < 2f / 3f)
                type = Tp2.Gecko;
        }
        else if (type == Tp.Centipede)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.Killerpillar;
            else if (rand < .7f)
                type = Tp1.Scutigera;
        }
        else if (type == Tp.Leech)
        {
            if (Random.value < .6f)
                type = Tp1.MiniBlackLeech;
        }
        else if (type == Tp.BigNeedleWorm)
        {
            if (Random.value < 1f / 3f)
                type = Tp1.MiniFlyingBigEel;
        }
        else if (type == Tp.SmallCentipede)
        {
            if (Random.value < .4f)
                type = Tp1.MiniScutigera;
        }
        else if (type == Tp.BlackLizard)
        {
            if (Random.value < .4f)
                type = Tp1.MoleSalamander;
        }
        else if (type == Tp.BlueLizard || type == TpM.ZoopLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.NoodleEater;
            else if (rand < 2f / 3f)
                type = Tp2.BabyCroaker;
        }
        else if (type == Tp.Salamander)
        {
            rand = Random.value;
            if (rand < .25f)
                type = Tp1.Polliwog;
            else if (rand < .5f)
                type = Tp1.WaterSpitter;
            else if (rand < .75f)
                type = Tp1.MoleSalamander;
        }
        else if (type == Tp.YellowLizard)
        {
            if (Random.value < .4f)
                type = Tp1.Polliwog;
        }
        else if (type == Tp.RedCentipede)
        {
            if (Random.value < .5f)
                type = Tp1.RedHorrorCenti;
        }
        else if (type == Tp.Centiwing)
        {
            rand = Random.value;
            if (rand < .1f)
                type = Tp1.RedHorrorCenti;
            else if (rand < .4f)
                type = Tp1.MiniFlyingBigEel;
        }
        else if (type == Tp.SpitterSpider)
        {
            rand = Random.value;
            if (rand < 1 / 3f)
                type = Tp2.MaracaSpider;
            else if (rand < 2 / 3f) 
                type = Tp1.Sporantula;
        }
        else if (type == Tp.GreenLizard || type == TpM.SpitLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.SilverLizard;
            else if (rand < 2f / 3f)
                type = Tp1.WaterSpitter;
        }
        else if (type == Tp.PinkLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = Tp1.SilverLizard;
            else if (rand < .385f)
                type = Tp1.NoodleEater;
        }
        else if (type == Tp.EggBug || type == TpM.FireBug)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
                type = type == TpM.FireBug ? Tp1.ThornBug : Tp1.SurfaceSwimmer;
            else if (rand < 2f / 3f)
                type = Tp1.TintedBeetle;
        }
        else if (type == Tp.DropBug)
        {
            if (Random.value < .5f)
                type = Tp1.ThornBug;
        }
        else if (type == Tp.RedLizard)
        {
            if (Random.value < .4f)
                type = Tp1.SilverLizard;
        }
        return type;
    }

    [NoStateCopyIsIntended]
    static AbstractCreature? On_SaveState_AbstractCreatureFromString(On.SaveState.orig_AbstractCreatureFromString orig, World world, string creatureString, bool onlyInCurrentRegion)
    {
        var res = orig(world, creatureString, onlyInCurrentRegion);
        if (res is null)
            return res;
        var type = res.creatureTemplate.type;
        var iD = res.ID;
        var state = Random.state;
        float rand;
        Random.InitState(iD.RandomSeed);
        if (type == Tp.TubeWorm)
        {
            if (Random.value < .5f)
            {
                if (Big.TryGetValue(res, out var props))
                {
                    props.IsBig = true;
                    props.Born = true;
                    var rnd = Random.value;
                    if (rnd < 1f / 3f)
                    {
                        res.superSizeMe = true;
                        props.NormalLook = false;
                    }
                    else if (rnd < 2f / 3f)
                    {
                        res.superSizeMe = false;
                        props.NormalLook = true;
                    }
                    else
                    {
                        res.superSizeMe = false;
                        props.NormalLook = false;
                    }
                }
            }
        }
        else if (type == Tp.MirosBird)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Blizzor), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.Snail)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.BouncingBall), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.WaterBlob), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.Spider)
        {
            if (Random.value < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.ChipChop), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == TpM.Yeek)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.ChipChop), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.WaterBlob), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.JetFish || type == Tp.BigEel || type == TpM.AquaCenti || type == TpM.BigJelly)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.CommonEel), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniLeviathan), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.PoleMimic)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Denture), null, res.pos, res.ID);
                res.setCustomFlags();
                if (Albino.TryGetValue(res, out var props))
                {
                    var rnd = Random.value;
                    if (rnd < .25f)
                    {
                        res.superSizeMe = true;
                        props.Value = true;
                    }
                    else if (rnd < .5f)
                    {
                        res.superSizeMe = true;
                        props.Value = false;
                    }
                    else if (rnd < .75f)
                    {
                        res.superSizeMe = false;
                        props.Value = true;
                    }
                    else
                    {
                        res.superSizeMe = false;
                        props.Value = false;
                    }
                }
            }
        }
        else if (type == Tp.TentaclePlant || type == TpM.StowawayBug)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Denture), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = true;
                if (Albino.TryGetValue(res, out var props))
                    props.Value = Random.value < .3f;
            }
        }
        else if (type == Tp.SeaLeech || type == TpM.JungleLeech)
        {
            rand = Random.value;
            if (rand < .1f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.DivingBeetle), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniBlackLeech), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == TpM.EelLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.DivingBeetle), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.CommonEel), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.KingVulture || type == Tp.Vulture || type == TpM.MirosVulture)
        {
            rand = Random.value;
            if (rand < .25f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.FlyingBigEel), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < .75f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.FatFireFly), null, res.pos, res.ID);
                res.setCustomFlags();
                if (Albino.TryGetValue(res, out var props))
                {
                    var rnd = Random.value;
                    if (rnd < .25f)
                    {
                        res.superSizeMe = true;
                        props.Value = true;
                    }
                    else if (rnd < .5f)
                    {
                        res.superSizeMe = true;
                        props.Value = false;
                    }
                    else if (rnd < .75f)
                    {
                        res.superSizeMe = false;
                        props.Value = true;
                    }
                    else
                    {
                        res.superSizeMe = false;
                        props.Value = false;
                    }
                }
            }
        }
        else if (type == Tp.BigSpider || type == TpM.MotherSpider)
        {
            rand = Random.value;
            if (rand < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Glowpillar), null, res.pos, res.ID);
                res.setCustomFlags();
                if (Albino.TryGetValue(res, out var props))
                {
                    var rnd = Random.value;
                    if (rnd < 1 / 3f)
                    {
                        res.superSizeMe = false;
                        props.Value = true;
                    }
                    else if (rnd < 2 / 3f)
                    {
                        res.superSizeMe = true;
                        props.Value = false;
                    }
                    else
                    {
                        res.superSizeMe = false;
                        props.Value = false;
                    }
                }
            }
            else if (rand < .6f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp2.MaracaSpider), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.Hazer)
        {
            rand = Random.value;
            if (rand < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.HazerMom), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
            else if (rand < 2f / 3f && Albino.TryGetValue(res, out var props))
                props.Value = true;
        }
        else if (type == Tp.CicadaA || type == Tp.CicadaB)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Hoverfly), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.CyanLizard || type == Tp.WhiteLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.HunterSeeker), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp2.Gecko), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.BrotherLongLegs || type == Tp.DaddyLongLegs || type == TpM.TerrorLongLegs)
        {
            if (Random.value < .5f)
            {
                if (!Jelly.TryGetValue(res, out var props))
                    Jelly.Add(res, props = new());
                props.Born = true;
                props.IsJelly = true;
            }
        }
        else if (type == Tp.Centipede)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Killerpillar), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
            else if (rand < .7f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Scutigera), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.Leech)
        {
            if (Random.value < .6f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniBlackLeech), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.BigNeedleWorm)
        {
            if (Random.value < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniFlyingBigEel), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.SmallCentipede)
        {
            if (Random.value < .4f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniScutigera), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.BlackLizard)
        {
            if (Random.value < .4f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MoleSalamander), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.BlueLizard || type == TpM.ZoopLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.NoodleEater), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp2.BabyCroaker), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.Salamander)
        {
            rand = Random.value;
            if (rand < .25f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Polliwog), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.WaterSpitter), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < .75f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MoleSalamander), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.YellowLizard)
        {
            if (Random.value < .4f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Polliwog), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.RedCentipede)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.RedHorrorCenti), null, res.pos, res.ID);
                res.setCustomFlags();
                res.voidCreature = Random.value < .1f;
            }
        }
        else if (type == Tp.Centiwing)
        {
            rand = Random.value;
            if (rand < .1f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.RedHorrorCenti), null, res.pos, res.ID);
                res.setCustomFlags();
                res.voidCreature = Random.value < .1f;
            }
            else if (rand < .4f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.MiniFlyingBigEel), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.Fly)
        {
            if (Random.value < .5f && Seed.TryGetValue(res, out var props))
            {
                props.IsSeed = true;
                props.Born = true;
            }
        }
        else if (type == Tp.SpitterSpider)
        {
            rand = Random.value;
            if (rand < 1 / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp2.MaracaSpider), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
            else if (rand < 2 / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.Sporantula), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.GreenLizard || type == TpM.SpitLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.SilverLizard), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.WaterSpitter), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        else if (type == Tp.PinkLizard)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.SilverLizard), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < .385f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.NoodleEater), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = false;
            }
        }
        else if (type == Tp.EggBug || type == TpM.FireBug)
        {
            rand = Random.value;
            if (rand < 1f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(type == TpM.FireBug ? Tp1.ThornBug : Tp1.SurfaceSwimmer), null, res.pos, res.ID);
                res.setCustomFlags();
            }
            else if (rand < 2f / 3f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.TintedBeetle), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.DropBug)
        {
            if (Random.value < .5f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.ThornBug), null, res.pos, res.ID);
                res.setCustomFlags();
                res.superSizeMe = Random.value < .5f;
            }
        }
        else if (type == Tp.RedLizard)
        {
            if (Random.value < .4f)
            {
                res = new(res.world, StaticWorld.GetCreatureTemplate(Tp1.SilverLizard), null, res.pos, res.ID);
                res.setCustomFlags();
            }
        }
        Random.state = state;
        return res;
    }
}