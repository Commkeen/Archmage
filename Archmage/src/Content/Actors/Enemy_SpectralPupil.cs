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

namespace Archmage.Content.Actors
{
    class Enemy_SpectralPupil:Monster
    {
        public Enemy_SpectralPupil(int instanceID)
            : base("enemy_spectralPupil", instanceID)
        {
            Name = "spectral pupil";

            Sprite = 'u';
            SpriteColor = new Color(libtcod.TCODColor.grey);

            MaxHealth = CurrentHealth = 5;


            XPValue = 20;

            Abilities.Add(new Ghost_ColdAtk(InstanceID));
            Abilities.Add(new Ghost_Shield(InstanceID));
            MovementEnergyCost = 100;
        }
    }

    class Ghost_ColdAtk:MonsterAbility
    {
        public Ghost_ColdAtk(int ownerID)
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
            atk.Element = ElementType.COLD;
            DamageData dmg = new DamageData();
            dmg.Magnitude = 2;
            dmg.DmgAttackType = AttackType.ARCANE;
            dmg.DmgElement = ElementType.COLD;

            Effects.Add(new Effect_WriteText("The spectral pupil summons a chilling blast!"));
            for (int i = 0; i < 2; i++)
            {
                Effect_GenericProjectile projectile = new Effect_GenericProjectile(atk);
                projectile.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(projectile);
            }

        }
    }

    class Ghost_Shield : MonsterAbility
    {

        int _behaviorID;

        public Ghost_Shield(int ownerID)
            : base(ownerID)
        {
            Priority = true;
            CooldownTime = 15;
            EnergyCost = 150;
        }

        public override bool Cast(IntVector2 enemy)
        {
            Actor owner = _scene.GetGameObjectPool().GetActor(OwnerID);
            _behaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_genericShield", OwnerID);
            _scene.GetGameObjectPool().GetActorBehavior(_behaviorID).ParentAbility = this;
            ((ActorBehavior_GenericShield)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).ShieldLayers = 1;
            ((ActorBehavior_GenericShield)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).BreakShieldMessage = "The spectral pupil's magic shield shatters!";
            ((ActorBehavior_GenericShield)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).AbsorbDamageMessage = "The spectral pupil's shield absorbs the attack!";
            owner.AddBehavior(_behaviorID);

            _scene.WriteMessage("The spectral pupil raises a magical shield!");

            IsActive = true;

            return base.Cast(enemy);
        }
    }
}
