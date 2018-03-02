using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells.Effects;

namespace Archmage.Engine.Spells
{
    /// <summary>
    /// A generic ability.
    /// Player and monster spells inherit from this.
    /// </summary>
    class Ability
    {
        protected static PlayScene _scene;

        //These variables define the constants of the ability
        public int EnergyCost { get; set; } //The amount of turn energy this ability uses up
        public int Range { get; set; } //0 range == self use
        public int CooldownTime { get; set; }
        public int AttentionCost { get; set; }
        public int SoulCost { get; set; }
        public bool Instant { get; set; }

        //These variables are for specific instances of the ability
        public int OwnerID { get; set; }
        public bool IsActive { get; set; }
        public int CurrentCooldown { get; set; }
        public int UsesRemaining { get; set; } //For spells that can only be used X times per floor

        //These variables are for when an ability gets modified or upgraded (like with spells)
        public int CooldownModifier { get; set; }
        public int AttentionModifier { get; set; }
        public int SoulModifier { get; set; }
        public int RangeModifier { get; set; }
        public int EnergyModifier { get; set; }

        protected List<Effect> Effects { get; set; } //The actual Effects the ability creates.

        protected List<int> BehaviorIDs { get; set; } //The behaviors, if any, that this ability is currently maintaining.

        public static void InitSceneReference(PlayScene scene)
        {
            _scene = scene;
        }

        public Ability(int ownerID)
        {
            OwnerID = ownerID;
            EnergyCost = 100;
            Range = 0;
            IsActive = false;
            CooldownTime = 0;
            CurrentCooldown = 0;
            AttentionCost = 0;
            SoulCost = 0;
            UsesRemaining = -1;
            Instant = true;

            CooldownModifier = 0;
            AttentionModifier = 0;
            SoulModifier = 0;
            RangeModifier = 0;
            EnergyModifier = 0;

            Effects = new List<Effect>();
            BehaviorIDs = new List<int>();
        }

        protected virtual void InitEffects()
        {
            Effects = new List<Effect>();
        }

        public virtual bool Cast()
        {
            InitEffects();
            foreach (Effect e in Effects)
            {
                e.SetSource(_scene.GetGameObjectPool().GetActor(OwnerID).Position);
                e.Activate();
            }
            _scene.GetGameObjectPool().GetActor(OwnerID).SpendEnergy(EnergyCost + EnergyModifier);
            if (Instant)
                StartCooldown();
            return false;
        }

        public virtual bool Cast(IntVector2 target)
        {
            return CastAtTarget(target);
        }

        public virtual bool CastAtTarget(IntVector2 target)
        {
            InitEffects();
            foreach (Effect e in Effects)
            {
                e.SetSource(_scene.GetGameObjectPool().GetActor(OwnerID).Position);
                e.SetTarget(target);
            }

            //Chain effects together sequentially
            for (int i = 1; i < Effects.Count; i++)
            {
                Effects[i - 1].OnEffectEndCallback = Effects[i].Activate;
            }
            if (Effects.Count > 0 && Effects[0] != null)
                Effects[0].Activate();

            _scene.GetGameObjectPool().GetActor(OwnerID).SpendEnergy(EnergyCost + EnergyModifier);
            if (Instant)
                StartCooldown();
            return true;
        }

        public virtual void CooldownTick()
        {
            if (CurrentCooldown > 0)
                CurrentCooldown--;
        }

        public virtual void StartCooldown()
        {
            CurrentCooldown = CooldownTime;
        }

        /// <summary>
        /// If this is an active spell, this deactivates the spell and puts it on cooldown.
        /// </summary>
        public virtual void Deactivate()
        {
            IsActive = false;
            StartCooldown();
        }
    }
}
