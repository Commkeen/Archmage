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
    class Spell_SlicingWind:Spell
    {
        int damage = 2;

        public Spell_SlicingWind(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "slicingWind";
            Name = "Slicing Wind";
            TargetingType = SpellTargetingType.NONE;
            Range = 4;
            CooldownTime = 8;
            AttentionCost = 2;
            SoulCost = 1;

            Description = "Does 2 energy damage to all nearby enemies.";
        }

        public override bool Cast()
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);

            List<int> enemies = _scene.GetActorsWithinDistance(currentCaster.Position, Range);

            Actor tgtEnemy = null;
            for (int i = 0; i < enemies.Count;)
            {
                tgtEnemy = _scene.GetGameObjectPool().GetActor(enemies[i]);
                if (!_scene.IsTileShootableFromLocation(currentCaster.Position, tgtEnemy.Position, Range))
                {
                    enemies.Remove(enemies[i]);
                }
                else if (enemies[i] == OwnerID)
                {
                    enemies.Remove(enemies[i]);
                }
                else
                {
                    i++;
                }
            }

            _scene.WriteMessage("You summon a howling wind around yourself!");

            foreach (int id in enemies)
            {
                Actor tgt = _scene.GetGameObjectPool().GetActor(id);
                DamageData dmg = new DamageData();
                dmg.DmgAttackType = AttackType.ENERGY;
                dmg.SourceID = OwnerID;
                dmg.TargetID = tgt.InstanceID;
                dmg.Magnitude = damage;

                dmg = _scene.GetGameObjectPool().GetActor(OwnerID).OnSendDamage(dmg);

                tgt.TakeDamage(dmg);
            }

            StartCooldown();

            return true;
        }
    }
}
