using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Behaviors;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Behaviors.TileBehaviors
{
    class TileBehavior_Door:TileBehavior
    {
        public bool IsOpen { get; protected set; }

        public TileBehavior_Door(int instanceID, IntVector2 affectedTile)
            : base("door", instanceID, affectedTile)
        {
            IsOpen = false;
        }

        public void OpenDoor()
        {
            IsOpen = true;

        }

        public void CloseDoor()
        {
            IsOpen = false;
        }

        public override char GetSprite()
        {
            if (IsOpen)
                return '/';
            return '+';
        }

        public override Color GetSpriteColor()
        {
            return new Color(libtcod.TCODColor.darkerSepia);
        }

        public override bool GetObstructsActors()
        {
            if (IsOpen)
                return false;
            return true;
        }

        public override bool GetObstructsVision(bool prevResult)
        {
            if (IsOpen)
                return false;
            return true;
        }

        public override bool GetObstructsProjectiles()
        {
            if (IsOpen)
                return false;
            return true;
        }
    }
}
