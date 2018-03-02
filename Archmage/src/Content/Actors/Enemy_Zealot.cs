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

namespace Archmage.Content.Actors
{
    class Enemy_Zealot:Monster
    {
        public Enemy_Zealot(int instanceID)
            : base("enemy_zealot", instanceID)
        {
            Name = "Phlegian zealot";

            Sprite = 'Z';
            SpriteColor = new Color(libtcod.TCODColor.orange);

            MaxHealth = CurrentHealth = 13;

            XPValue = 15;

            Abilities.Add(new Zealot_Shot(_scene, InstanceID));
            MovementEnergyCost = 100;
        }

        
    }

    class Zealot_Shot:MonsterAbility
    {

        public Zealot_Shot(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            Range = 6;
            Priority = false;

            UseChance = 80;

            EnergyCost = 100;
        }

        public override bool Cast(IntVector2 enemy)
        {

            SpecialEffect.OnSpecialEffectEndCallback callback = () =>
            {
                Actor tgt = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(enemy)[0]);
                DamageData dmg = new DamageData();
                dmg.TargetID = tgt.InstanceID;
                dmg.SourceID = OwnerID;
                dmg.Magnitude = 5;
                dmg.DmgAttackType = AttackType.ARCANE;
                tgt.TakeDamage(dmg);
            };

            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicProjectile");
            SpecialEffect_BasicProjectile fx = (SpecialEffect_BasicProjectile)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
            fx.Init(currentCaster.Position, enemy, new Color(libtcod.TCODColor.lightOrange), callback);
            _scene.AddSpecialEffect(fxId);


            return true;
        }

    }

}
