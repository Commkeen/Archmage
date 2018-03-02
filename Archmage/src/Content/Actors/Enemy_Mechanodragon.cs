using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.SpecialEffects;
using Archmage.Content.SpecialEffects;
using Archmage.Content.Behaviors.ActorBehaviors;
using Archmage.Engine.Spells.Effects;
using Archmage.Engine.System;

namespace Archmage.Content.Actors
{
    class Enemy_Mechanodragon:Monster
    {
        public Enemy_Mechanodragon(int instanceID)
            : base("enemy_mechanodragon", instanceID)
        {
            Name = "mechanodragon";

            Sprite = 'D';
            SpriteColor = new Color(libtcod.TCODColor.lightGreen);

            MaxHealth = CurrentHealth = 40;


            XPValue = 400;

            Abilities.Add(new Mechanodragon_RocketPods(InstanceID));
            MovementEnergyCost = 100;

            //CanMove = false;
        }
    }

    class Mechanodragon_RocketPods:MonsterAbility
    {
        public Mechanodragon_RocketPods(int ownerID)
            : base(ownerID)
        {
            Range = 12;
            Priority = false;

            UseChance = 40;

            EnergyCost = 300;
        }

        protected override void InitEffects()
        {
            int rocketScatterRadius = 5;
            int rocketBlastRadius = 3;

            base.InitEffects();
            AttackData rocketProjectileAttack = new AttackData();
            rocketProjectileAttack.Type = AttackType.PHYSICAL;
            rocketProjectileAttack.Element = ElementType.NONE;
            AttackData rocketBlastAttack = new AttackData();
            rocketProjectileAttack.Type = AttackType.BLAST;
            rocketProjectileAttack.Element = ElementType.NONE;

            DamageData dmg = new DamageData();
            dmg.Magnitude = 4;
            dmg.DmgAttackType = AttackType.BLAST;
            dmg.DmgElement = ElementType.NONE;
            dmg.DamagesWalls = true;

            Effects.Add(new Effect_WriteText("The mechanodragon fires a volley of rockets!"));

            Effect_PickRandomTargetsInRadius randomTargets = new Effect_PickRandomTargetsInRadius(rocketScatterRadius, 4, 4);

            Effect_GenericProjectile rocketProjectile = new Effect_GenericProjectile(rocketProjectileAttack);
            Effect_Blast rocketBlast = new Effect_Blast(rocketBlastAttack, rocketBlastRadius);
            rocketProjectile.SetSource(_scene.GetGameObjectPool().GetActor(OwnerID).Position);
            rocketBlast.EffectsOnEachActor.Add(new Effect_BasicDamage(dmg));
            rocketProjectile.EffectsOnStrike.Add(rocketBlast);
            randomTargets.EffectsOnEachTarget.Add(rocketProjectile);

            Effects.Add(randomTargets);
        }
    }
}
