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
    class Enemy_Dog:Monster
    {
        public Enemy_Dog(int instanceID)
            : base("enemy_dog", instanceID)
        {
            Name = "wolf";

            Sprite = 'w';
            SpriteColor = new Color(libtcod.TCODColor.sepia);

            MaxHealth = CurrentHealth = 2;

            XPValue = 15;

            Abilities.Add(new Dog_Bite(_scene, InstanceID));
            MovementEnergyCost = 50;
        }
    }

    class Dog_Bite:MonsterAbility
    {
        public Dog_Bite(PlayScene scene, int ownerID)
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
            dmg.Magnitude = 1;
            dmg.DmgAttackType = AttackType.MELEE;
            dmg.DmgElement = ElementType.NONE;

            Effects.Add(new Effect_WriteText("The wolf bites you!"));
            for (int i = 0; i < 1; i++)
            {
                Effect_GenericMelee melee = new Effect_GenericMelee(atk);
                melee.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(melee);
            }

        }
    }
}
