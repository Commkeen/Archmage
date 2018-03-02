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
using Archmage.Engine.Spells.Effects;

namespace Archmage.Content.Actors
{
    class Enemy_QuazaxRocketeer:Monster
    {
        public Enemy_QuazaxRocketeer(int instanceID)
            : base("enemy_goblinTinker", instanceID)
        {
            Name = "quazax rocketeer";

            Sprite = 'q';
            SpriteColor = new Color(libtcod.TCODColor.darkRed);

            MaxHealth = CurrentHealth = 11;

            XPValue = 30;

            Abilities.Add(new Quazax_Rocket(InstanceID));
            MovementEnergyCost = 150;
        }

        
    }

    class Quazax_Rocket : MonsterAbility
    {
        public Quazax_Rocket(int ownerID)
            : base(ownerID)
        {
            Range = 6;
            Priority = false;

            UseChance = 40;

            EnergyCost = 250;
        }

        protected override void InitEffects()
        {
            int rocketBlastRadius = 3;

            base.InitEffects();
            AttackData rocketProjectileAttack = new AttackData();
            rocketProjectileAttack.Type = AttackType.PHYSICAL;
            rocketProjectileAttack.Element = ElementType.NONE;
            AttackData rocketBlastAttack = new AttackData();
            rocketProjectileAttack.Type = AttackType.BLAST;
            rocketProjectileAttack.Element = ElementType.NONE;

            DamageData dmg = new DamageData();
            dmg.Magnitude = 6;
            dmg.DmgAttackType = AttackType.BLAST;
            dmg.DmgElement = ElementType.NONE;
            dmg.DamagesWalls = true;

            Effects.Add(new Effect_WriteText("The quazax rocketeer fires a rocket!"));

            Effect_GenericProjectile rocketProjectile = new Effect_GenericProjectile(rocketProjectileAttack);
            Effect_Blast rocketBlast = new Effect_Blast(rocketBlastAttack, rocketBlastRadius);
            rocketProjectile.SetSource(_scene.GetGameObjectPool().GetActor(OwnerID).Position);
            rocketBlast.EffectsOnEachActor.Add(new Effect_BasicDamage(dmg));
            rocketProjectile.EffectsOnStrike.Add(rocketBlast);

            Effects.Add(rocketProjectile);
        }
    }

}
