using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Content.Spells;
using Archmage.Engine.Scenes;
using Archmage.Actors;

namespace Archmage.Engine.Spells
{
    class SpellFactory
    {

        public SpellFactory()
        {

        }


        public Spell CreateSpell(string name, PlayScene scene, int ownerID)
        {
            Spell s = new Spell_MagicDart(scene, ownerID);

            //Arcane
            if (name == "magicDart")
            {
                s = new Spell_MagicDart(scene, ownerID);
            }
            else if (name == "arcaneShield")
            {
                s = new Spell_ArcaneShield(scene, ownerID);
            }
            else if (name == "magicMissile")
            {
                s = new Spell_MagicMissile(scene, ownerID);
            }
            else if (name == "photonSpike")
            {
                s = new Spell_PhotonSpike(scene, ownerID);
            }
            else if (name == "astralConduit")
            {
                s = new Spell_AstralConduit(scene, ownerID);
            }
            else if (name == "energize")
            {
                s = new Spell_Energize(scene, ownerID);
            }
            else if (name == "dispel")
            {
                s = new Spell_Dispel(scene, ownerID);
            }
            else if (name == "quickshot")
            {
                s = new Spell_Quickshot(scene, ownerID);
            }
            else if (name == "arcaneBlast")
            {
                s = new Spell_ArcaneBlast(scene, ownerID);
            }
            else if (name == "boundSpirit")
            {
                s = new Spell_BoundSpirit(scene, ownerID);
            }

                //Nature
            else if (name == "gravelShot")
            {
                s = new Spell_GravelShot(scene, ownerID);
            }
            else if (name == "hurlSpines")
            {
                s = new Spell_HurlSpines(scene, ownerID);
            }
            else if (name == "scaledSkin")
            {
                s = new Spell_ScaledSkin(scene, ownerID);
            }
            else if (name == "slicingWind")
            {
                s = new Spell_SlicingWind(scene, ownerID);
            }
            else if (name == "lightningBolt")
            {
                s = new Spell_LightningBolt(scene, ownerID);
            }
            else if (name == "meteor")
            {
                s = new Spell_Meteor(scene, ownerID);
            }

                //Shadow
            else if (name == "flicker")
            {
                s = new Spell_Flicker(scene, ownerID);
            }
            else if (name == "infect")
            {
                s = new Spell_Infect(scene, ownerID);
            }
            else if (name == "blink")
            {
                s = new Spell_Blink(scene, ownerID);
            }
            else if (name == "darkArrow")
            {
                s = new Spell_DarkArrow(scene, ownerID);
            }
            else if (name == "sleep")
            {
                s = new Spell_Sleep(scene, ownerID);
            }
            else if (name == "stoneToGlass")
            {
                s = new Spell_StoneToGlass(scene, ownerID);
            }
            else if (name == "pain")
            {
                s = new Spell_Pain(scene, ownerID);
            }
            else if (name == "touchOfDeath")
            {
                s = new Spell_TouchOfDeath(scene, ownerID);
            }

                //Debug
            else if (name == "xrayVision")
            {
                s = new Spell_XrayVision(scene, ownerID);
            }
            else if (name == "invincibility")
            {
                s = new Spell_Invincibility(scene, ownerID);
            }

            else
            {
                throw new NotImplementedException();
            }

            return s;
        }

    }
}
