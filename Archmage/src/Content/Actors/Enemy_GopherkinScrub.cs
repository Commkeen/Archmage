using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells.Effects;

namespace Archmage.Content.Actors
{
    class Enemy_GopherkinScrub:Monster
    {
        public Enemy_GopherkinScrub(int instanceID)
            : base("enemy_gopherkinScrub", instanceID)
        {
            Name = "gopherkin scrub";

            Sprite = 'g';
            SpriteColor = new Color(libtcod.TCODColor.darkGreen);

            MaxHealth = CurrentHealth = 6;

            XPValue = 15;

            Abilities.Add(new ScrubAttack(_scene, InstanceID));
            MovementEnergyCost = 100;
        }
    }

    class ScrubAttack:MonsterAbility
    {
        public ScrubAttack(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            Range = 1;
            Priority = false;

            UseChance = 100;
        }

        protected override void InitEffects()
        {
            base.InitEffects();
            AttackData atk = new AttackData();
            atk.Type = AttackType.MELEE;
            atk.Element = ElementType.PHYSICAL;
            DamageData dmg = new DamageData();
            dmg.Magnitude = 2;
            dmg.DmgAttackType = AttackType.MELEE;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("The gopherkin scrub stabs you!"));
            for (int i = 0; i < 1; i++)
            {
                Effect_GenericMelee melee = new Effect_GenericMelee(atk);
                melee.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(melee);
            }

        }

        /*
        public override bool Cast(IntVector2 enemy)
        {
            Actor tgt = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(enemy)[0]);
            DamageData dmg = new DamageData();
            dmg.TargetID = tgt.InstanceID;
            dmg.SourceID = OwnerID;
            dmg.Magnitude = 1;
            dmg.DmgAttackType = AttackType.PHYSICAL;
            tgt.TakeDamage(dmg);
            return true;
        }
         **/
    }
}
