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
    class Enemy_RestlessSpirit:Monster
    {
        public Enemy_RestlessSpirit(int instanceID)
            : base("enemy_restlessSpirit", instanceID)
        {
            Name = "restless spirit";

            Sprite = 'S';
            SpriteColor = new Color(libtcod.TCODColor.darkRed);

            MaxHealth = CurrentHealth = 6;

            XPValue = 50;

            Abilities.Add(new Spirit_Attack(_scene, InstanceID));
            MovementEnergyCost = 50;
        }

        protected override DamageData OnIncomingDamage(DamageData damage)
        {
            if (damage.DmgAttackType == AttackType.PHYSICAL)
            {
                _scene.WriteMessage("The physical attack passes right through the spirit!");
                damage.Magnitude = 0;
            }

            return base.OnIncomingDamage(damage);
        }
    }

    class Spirit_Attack:MonsterAbility
    {
        public Spirit_Attack(PlayScene scene, int ownerID)
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
            dmg.DmgAttackType = AttackType.ARCANE;
            tgt.TakeDamage(dmg);
            return true;
        }
    }
}
