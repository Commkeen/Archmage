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
    class Enemy_ArmoredWorg:Monster
    {

        public Enemy_ArmoredWorg(int instanceID)
            : base("enemy_armoredWorg", instanceID)
        {
            Name = "armored worg";

            Sprite = 'W';
            SpriteColor = new Color(libtcod.TCODColor.darkRed);

            MaxHealth = CurrentHealth = 5;

            XPValue = 30;

            Abilities.Add(new Worg_Bite(_scene, InstanceID));
            MovementEnergyCost = 50;
        }
    }

    class Worg_Bite:MonsterAbility
    {
        public Worg_Bite(PlayScene scene, int ownerID)
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
            dmg.Magnitude = 4;
            dmg.DmgAttackType = AttackType.PHYSICAL;
            tgt.TakeDamage(dmg);
            return true;
        }
    }
}
