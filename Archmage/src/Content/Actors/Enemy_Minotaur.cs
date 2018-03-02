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
    class Enemy_Minotaur:Monster
    {
        public Enemy_Minotaur(int instanceID)
            : base("enemy_minotaur", instanceID)
        {
            Name = "minotaur";

            Sprite = 'M';
            SpriteColor = new Color(libtcod.TCODColor.darkRed);

            MaxHealth = CurrentHealth = 9;

            XPValue = 60;

            Abilities.Add(new Minotaur_Attack(_scene, InstanceID));
            MovementEnergyCost = 50;
        }

        protected override DamageData OnIncomingDamage(DamageData damage)
        {
            if (damage.DmgAttackType == AttackType.ARCANE)
            {
                _scene.WriteMessage("The minotaur laughs at your magical attack...");
                damage.Magnitude = 0;
            }

            return base.OnIncomingDamage(damage);
        }
    }

    class Minotaur_Attack:MonsterAbility
    {
        public Minotaur_Attack(PlayScene scene, int ownerID)
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
            dmg.Magnitude = 5;
            dmg.DmgAttackType = AttackType.PHYSICAL;
            tgt.TakeDamage(dmg);
            return true;
        }
    }
}
