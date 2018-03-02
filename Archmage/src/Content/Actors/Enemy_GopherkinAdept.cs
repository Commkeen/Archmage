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
    class Enemy_GopherkinAdept:Monster
    {
        public Enemy_GopherkinAdept(int instanceID)
            : base("enemy_gopherkinAdept", instanceID)
        {
            Name = "gopherkin adept";

            Sprite = 'g';
            SpriteColor = new Color(libtcod.TCODColor.darkYellow);

            MaxHealth = CurrentHealth = 6;


            XPValue = 40;

            Abilities.Add(new Adept_Shot(InstanceID));
            Abilities.Add(new Adept_Shield(InstanceID));
            MovementEnergyCost = 100;
        }
    }

    class Adept_Shot:MonsterAbility
    {
        public Adept_Shot(int ownerID)
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

            Effects.Add(new Effect_WriteText("The gopherkin adept shoots magic missiles!"));
            for (int i = 0; i < 2; i++)
            {
                Effect_GenericProjectile projectile = new Effect_GenericProjectile(atk);
                projectile.EffectsOnDamage.Add(new Effect_BasicDamage(dmg));
                Effects.Add(projectile);
            }

        }
    }

    class Adept_Shield : MonsterAbility
    {

        int _behaviorID;

        public Adept_Shield(int ownerID)
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
            ((ActorBehavior_GenericShield)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).BreakShieldMessage = "The gopherkin adept's magic shield shatters!";
            ((ActorBehavior_GenericShield)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).AbsorbDamageMessage = "The gopherkin adept's shield absorbs the attack!";
            owner.AddBehavior(_behaviorID);

            _scene.WriteMessage("The gopherkin adept raises a magical shield!");

            IsActive = true;

            return base.Cast(enemy);
        }
    }

    class Monster_Stun : MonsterAbility
    {

        int _behaviorID;

        public Monster_Stun(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            Priority = true;

            EnergyCost = 100;
        }

        public override bool Cast(IntVector2 enemy)
        {
            Actor tgt = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(enemy)[0]);
            Actor owner = _scene.GetGameObjectPool().GetActor(OwnerID);
            _behaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_stun", tgt.InstanceID);
            _scene.GetGameObjectPool().GetActorBehavior(_behaviorID).ParentAbility = this;
            ((ActorBehavior_Stun)_scene.GetGameObjectPool().GetActorBehavior(_behaviorID)).TurnsRemaining = 5;


            
            tgt.AddBehavior(_behaviorID);

            _scene.WriteMessage("The monster stuns you!");

            IsActive = true;

            return base.Cast(enemy);
        }
    }
}
