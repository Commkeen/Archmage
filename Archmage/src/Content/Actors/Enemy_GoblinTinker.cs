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
    class Enemy_GoblinTinker:Monster
    {
        public Enemy_GoblinTinker(int instanceID)
            : base("enemy_goblinTinker", instanceID)
        {
            Name = "quazax rigger";

            Sprite = 'q';
            SpriteColor = new Color(libtcod.TCODColor.darkPurple);

            MaxHealth = CurrentHealth = 7;

            XPValue = 15;

            Abilities.Add(new GoblinTinker_Shot(_scene, InstanceID));
            MovementEnergyCost = 100;
        }

        
    }

    class GoblinTinker_Shot:MonsterAbility
    {

        public GoblinTinker_Shot(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            Range = 4;
            Priority = false;

            UseChance = 60;

            EnergyCost = 50;
        }

        public override bool Cast(IntVector2 enemy)
        {
            SpecialEffect.OnSpecialEffectEndCallback callback = () =>
            {
                Actor tgt = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(enemy)[0]);
                DamageData dmg = new DamageData();
                dmg.TargetID = tgt.InstanceID;
                dmg.SourceID = OwnerID;
                dmg.Magnitude = 2;
                dmg.DmgAttackType = AttackType.ENERGY;
                tgt.TakeDamage(dmg);
            };

            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicProjectile");
            SpecialEffect_BasicProjectile fx = (SpecialEffect_BasicProjectile)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
            fx.Init(currentCaster.Position, enemy, new Color(libtcod.TCODColor.purple), callback);
            _scene.AddSpecialEffect(fxId);

            return true;
        }

    }

}
