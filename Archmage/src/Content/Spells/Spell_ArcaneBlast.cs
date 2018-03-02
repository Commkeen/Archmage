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
    class Spell_ArcaneBlast:Spell
    {
        int damage = 3;

        public Spell_ArcaneBlast(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "arcaneBlast";
            Name = "Mystic Blast";
            TargetingType = SpellTargetingType.NONE;
            Range = 6;
            CooldownTime = 2;
            AttentionCost = 1;

            Description = "Does 3 BLAST damage to all nearby enemies.";
        }

        protected override void InitEffects()
        {
            base.InitEffects();
            AttackData atk = new AttackData();
            atk.Type = AttackType.BLAST;
            atk.Element = ElementType.NONE;
            DamageData dmg = new DamageData();
            dmg.Magnitude = damage;
            dmg.DmgAttackType = AttackType.BLAST;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("You create a shockwave of magical force!"));

            Effect_Blast blastEffect = new Effect_Blast(atk, Range);
            blastEffect.BlastColor = new Color(libtcod.TCODColor.lighterBlue);
            blastEffect.ExcludedActors.Add(OwnerID);
            blastEffect.EffectsOnEachActor.Add(new Effect_BasicDamage(dmg));
            blastEffect.SetTarget(_scene.GetGameObjectPool().GetActor(OwnerID).Position);
            Effects.Add(blastEffect);

        }
    }
}
