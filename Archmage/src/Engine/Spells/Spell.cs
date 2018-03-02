using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells.Effects;

namespace Archmage.Engine.Spells
{
    /// <summary>
    /// Describes a Player spell.
    /// </summary>
    class Spell:Ability
    {
        public enum SpellTargetingType { NONE, ENEMY, ANYWHERE }

        #region Fixed data

        public string ID { get; protected set; } //The internal name of the spell
        public string Name { get; protected set; } //The player-facing name of the spell
        public string Description { get; protected set; }

        public int SpellbookEssenceValue { get; protected set; } //The amount of essence you get when you recycle this spellbook
        public Color SpellbookColor { get; protected set; } //What color a spellbook for this spell is (determined by discipline probably)

        public int SpellLevel { get; protected set; } //The power level of this spell, better spells are found deeper and maybe indecipherable at first

        public int UpgradeLevel { get; protected set; } //How far this spell has been upgraded
        public int MaxUpgradeLevel { get; protected set; } //How far this spell can be upgraded total



        public SpellTargetingType TargetingType { get; protected set; }

        #endregion

        #region Spell state variables
        public bool IsSelected { get; set; } //Whether the player has the spell selected right now
        

        #endregion

        public Spell(int ownerID):base(ownerID)
        {
            ID = "default";
            Name = "default";
            Description = "default";

            SpellLevel = 1;
            UpgradeLevel = 1;
            MaxUpgradeLevel = 5;

            TargetingType = SpellTargetingType.NONE;

            IsSelected = false;
            Instant = true;
        }


        

        #region Spellcasting functions

        /*
        public virtual bool Cast()
        {
            InitEffects();
            foreach (Effect e in Effects)
            {
                e.Activate();
            }
            _scene.GetGameObjectPool().GetActor(OwnerID).SpendEnergy(EnergyCost + EnergyModifier);
            if (Instant)
                StartCooldown();
            return false;
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
            if (Effects[0] != null)
                Effects[0].Activate();

            _scene.GetGameObjectPool().GetActor(OwnerID).SpendEnergy(EnergyCost + EnergyModifier);
            if (Instant)
                StartCooldown();
            return false;
        }
        */
        #endregion

        #region Upgrade functions

        public virtual string UpgradeDescription(int level)
        {
            return "default";
        }

        public virtual int UpgradeCost(int level)
        {
            return 10;
        }

        public virtual bool GainLevel()
        {
            return false;
        }

        #endregion

        public static List<string> GetAllSpellIDs()
        {
            List<string> spellIDs = new List<string>();
            spellIDs.Add("flicker");
            //spellIDs.Add("arcaneShield");
            //spellIDs.Add("magicDart");
            spellIDs.Add("scaledSkin");
            spellIDs.Add("arcaneBlast");
            spellIDs.Add("stoneToGlass");
            spellIDs.Add("sleep");

            return spellIDs;
        }
    }
}
