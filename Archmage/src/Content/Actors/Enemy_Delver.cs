using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Actors
{
    class Enemy_Delver:Monster
    {
        public Enemy_Delver(int instanceID)
            : base("enemy_delver", instanceID)
        {
            Name = "delver";

            Sprite = 'D';
            SpriteColor = new Color(libtcod.TCODColor.darkRed);

            MaxHealth = CurrentHealth = 5;

            XPValue = 30;

            Abilities.Add(new Delver_Bite(_scene, InstanceID));
            MovementEnergyCost = 150;
        }

        protected override DamageData OnIncomingDamage(DamageData damage)
        {
            if (damage.DmgAttackType == AttackType.ARCANE)
            {
                _scene.WriteMessage("The delver shrugs off the magical attack...");
                damage.Magnitude = 0;
            }

            return base.OnIncomingDamage(damage);
        }
    }

    class Delver_Bite:MonsterAbility
    {
        public Delver_Bite(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            Range = 1;
            Priority = false;

            UseChance = 100;
        }

        public override bool Cast(IntVector2 enemy)
        {
            Actor tgt = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(enemy)[0]);
            DamageData dmg = new DamageData();
            dmg.TargetID = tgt.InstanceID;
            dmg.SourceID = OwnerID;
            dmg.Magnitude = 3;
            dmg.DmgAttackType = AttackType.PHYSICAL;
            tgt.TakeDamage(dmg);
            return true;
        }
    }
}
