using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Engine.Spells;
using Archmage.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Content.SpecialEffects;
using Archmage.Engine.Spells.Effects;

namespace Archmage.Content.Spells
{
    class Spell_MagicDart:Spell
    {

        IntVector2 currentTarget;

        int damage = 2;

        public Spell_MagicDart(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "magicDart";
            Name = "Magic Dart";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 6;
            CooldownTime = 3;
            AttentionCost = 1;
            Description = "Does 2 ARCANE damage.";
        }

        protected override void InitEffects()
        {
            base.InitEffects();
            AttackData atk = new AttackData();
            atk.Type = AttackType.ARCANE;
            atk.Element = ElementType.NONE;
            DamageData dmg = new DamageData();
            dmg.Magnitude = 2;
            dmg.DmgAttackType = AttackType.ARCANE;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("Some magic darts zip from your finger!"));
            for (int i = 0; i < 3; i++)
            {
                Effect_GenericProjectile projectile = new Effect_GenericProjectile(atk);
                projectile.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(projectile);
            }

        }
    }
}
