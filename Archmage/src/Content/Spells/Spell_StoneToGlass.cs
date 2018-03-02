using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Behaviors;
using Archmage.Engine.Spells;
using Archmage.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Content.SpecialEffects;
using Archmage.Content.Behaviors.ActorBehaviors;
using Archmage.Content.Behaviors.TileBehaviors;

namespace Archmage.Content.Spells
{
    class Spell_StoneToGlass:Spell
    {

        int _activeBehaviorID;

        public Spell_StoneToGlass(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "stoneToGlass";
            Name = "Stone to Glass";
            TargetingType = SpellTargetingType.ANYWHERE;
            Range = 2;
            CooldownTime = 8;
            AttentionCost = 2;
            SoulCost = 1;

            _activeBehaviorID = 0;

            Description = "Turns the targeted wall to glass, allowing you to see through it.  The enchantment will spread to nearby walls as well.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            //We can't cast if there's nobody there
            if (!_scene.GetMap().DoesTileHaveFeature(target, Tiles.Tile_SimpleFeatureType.STONE_WALL))
            {
                _scene.WriteMessage("That spell must be cast on a wall!");
                return false;
            }

            _activeBehaviorID = _scene.GetGameObjectPool().CreateTileBehavior("stoneToGlass", target);
            TileBehavior_StoneToGlass b = (TileBehavior_StoneToGlass)_scene.GetGameObjectPool().GetTileBehavior(_activeBehaviorID);
            _scene.GetMap().AddTileBehavior(_activeBehaviorID);
            _scene.GetTurnCounter().AddObjectToCounter(_activeBehaviorID);

            b.SpreadStrength = 10;

            StartCooldown();
            return true;
        }

        public override void Deactivate()
        {
            IsActive = false;
            StartCooldown();

            //Remove behavior from target and kill behavior
            ActorBehavior behavior = _scene.GetGameObjectPool().GetActorBehavior(_activeBehaviorID);
            if (behavior != null)
            {
                Actor targetActor = _scene.GetGameObjectPool().GetActor(behavior.AffectedActor);
                if (targetActor != null)
                {
                    targetActor.RemoveBehavior(behavior.InstanceID);
                }

                behavior.Alive = false;
            }

            _activeBehaviorID = 0;

            base.Deactivate();
        }
    }
}
