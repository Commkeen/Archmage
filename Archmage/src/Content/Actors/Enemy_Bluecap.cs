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
    class Enemy_Bluecap:Monster
    {
        public Enemy_Bluecap(int instanceID)
            : base("enemy_bluecap", instanceID)
        {
            Name = "bluecap";

            Sprite = 's';
            SpriteColor = new Color(libtcod.TCODColor.blue);

            MaxHealth = CurrentHealth = 4;
            EvasionPoints = 1;

            XPValue = 20;

            Abilities.Add(new Bluecap_Shot(InstanceID));
            MovementEnergyCost = 100;
        }

        public override TurnResult TakeTurn()
        {
            return base.TakeTurn();
        }
    }

    class Bluecap_Shot:MonsterAbility
    {

        public Bluecap_Shot(int ownerID)
            : base(ownerID)
        {
            Range = 4;
            Priority = false;

            UseChance = 80;

            EnergyCost = 100;
        }

        protected override void InitEffects()
        {
            base.InitEffects();
            AttackData atk = new AttackData();
            atk.Type = AttackType.ARCANE;
            atk.Element = ElementType.NONE;
            DamageData dmg = new DamageData();
            dmg.Magnitude = 1;
            dmg.DmgAttackType = AttackType.ARCANE;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("The bluecap shoots fairy darts!"));
            for (int i = 0; i < 2; i++)
            {
                Effect_GenericProjectile projectile = new Effect_GenericProjectile(atk);
                projectile.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(projectile);
            }

        }

    }

}
