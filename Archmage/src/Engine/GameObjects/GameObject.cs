using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;

namespace Archmage
{
    class GameObject
    {
        protected static PlayScene _scene;

        public string ObjectID { get; protected set; } //The type of object this is
        public int InstanceID { get; protected set; } //Unique instance identifier
        public bool Alive { get; set; } //Whether the object should be able to do anything

        public GameObject(string objID, int instanceID)
        {
            ObjectID = objID;
            InstanceID = instanceID;
            Alive = true;
        }

        /// <summary>
        /// Initialize a reference to the current scene.
        /// This is placed here so that it doesn't have to be in the constructor of every game object.
        /// </summary>
        /// <param name="scene"></param>
        public static void InitSceneReference(PlayScene scene)
        {
            _scene = scene;
        }
    }
}
