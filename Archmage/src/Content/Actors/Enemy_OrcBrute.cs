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
    class Enemy_OrcBrute:Monster
    {

        public Enemy_OrcBrute(int instanceID)
            : base("enemy_orcBrute", instanceID)
        {
            Name = "gopherkin brute";

            Sprite = 'G';
            SpriteColor = new Color(libtcod.TCODColor.darkGreen);

            MaxHealth = CurrentHealth = 8;

            XPValue = 25;

            ArmorPoints = 1;

            Abilities.Add(new Brute_Melee(InstanceID));
            MovementEnergyCost = 150;
        }
    }

    class Brute_Melee : MonsterAbility
    {
        public Brute_Melee(int ownerID)
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
            atk.Element = ElementType.NONE;
            DamageData dmg = new DamageData();
            dmg.Magnitude = 2;
            dmg.DmgAttackType = AttackType.MELEE;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("The gopherkin brute smacks you!"));
            for (int i = 0; i < 1; i++)
            {
                Effect_GenericMelee melee = new Effect_GenericMelee(atk);
                melee.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(melee);
            }

        }
    }
}
