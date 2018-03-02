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

namespace Archmage.Content.Spells
{
    class Spell_Blink:Spell
    {

        IntVector2 currentTarget;

        public Spell_Blink(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "blink";
            Name = "Blink";
            TargetingType = SpellTargetingType.ANYWHERE;
            Range = 5;
            CooldownTime = 6;
            AttentionCost = 2;
            SoulCost = 1;

            Description = "Allows you to teleport a short distance.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            currentTarget = target;

            if (!_scene.IsPositionFree(target))
            {
                _scene.WriteMessage("There's something in the way there!");
                return false;
            }

            else
            {
                currentCaster.WarpToPosition(target);
            }

            StartCooldown();

            return true;
        }
    }
}
