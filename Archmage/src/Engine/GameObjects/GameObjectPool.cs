using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Behaviors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Items;
using Archmage.Content.Actors;
using Archmage.Content.Behaviors.ActorBehaviors;
using Archmage.Content.Behaviors.TileBehaviors;
using Archmage.SpecialEffects;
using Archmage.Content.SpecialEffects;
using Archmage.Content.Items;

namespace Archmage
{
    class GameObjectPool
    {

        PlayScene _scene;

        Dictionary<int, GameObject> _gameObjectTable;

        int _nextID;

        public GameObjectPool(PlayScene scene)
        {
            _scene = scene;
            ResetGameObjectPool();
        }

        public void ResetGameObjectPool()
        {
            _gameObjectTable = new Dictionary<int, GameObject>();
            _nextID = 1;
        }

        /// <summary>
        /// Shouldn't ever really be used, I need to find another way to do this
        /// Currently only used to move the Student between levels
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void AddObjectToPool(GameObject obj, int key)
        {
            if (_gameObjectTable.ContainsKey(key))
                throw new Exception();
            _gameObjectTable.Add(key, obj);
        }


        #region Get methods
        

        public Actor GetActor(int key)
        {
            Actor a = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is Actor)
                a = (Actor)obj;

            return a;
        }

        public List<Actor> GetAllAliveActors()
        {
            List<Actor> actors = new List<Actor>();

            foreach (GameObject obj in _gameObjectTable.Values)
            {
                if (obj is Actor && obj.Alive)
                    actors.Add((Actor)obj);
            }

            return actors;
        }

        public Item GetItem(int key)
        {
            Item i = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is Item)
                i = (Item)obj;

            return i;
        }

        public ItemToken GetItemToken(int key)
        {
            ItemToken i = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is ItemToken)
                i = (ItemToken)obj;

            return i;
        }

        public List<ItemToken> GetAllAliveItemTokens()
        {
            List<ItemToken> tokens = new List<ItemToken>();

            foreach (GameObject obj in _gameObjectTable.Values)
            {
                if (obj is ItemToken && obj.Alive)
                    tokens.Add((ItemToken)obj);
            }

            return tokens;
        }

        public ActorBehavior GetActorBehavior(int key)
        {
            ActorBehavior b = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is ActorBehavior)
                b = (ActorBehavior)obj;

            return b;
        }

        public TileBehavior GetTileBehavior(int key)
        {
            TileBehavior b = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is TileBehavior)
                b = (TileBehavior)obj;

            return b;
        }

        public SpecialEffect GetSpecialEffect(int key)
        {
            SpecialEffect e = null;

            GameObject obj = GetGameObject(key);
            if (obj != null && obj is SpecialEffect)
                e = (SpecialEffect)obj;

            return e;
        }

        public List<SpecialEffect> GetAllAliveSpecialEffects()
        {
            List<SpecialEffect> fx = new List<SpecialEffect>();

            foreach (GameObject obj in _gameObjectTable.Values)
            {
                if (obj is SpecialEffect && obj.Alive)
                    fx.Add((SpecialEffect)obj);
            }

            return fx;
        }

        public ITakesTurns GetITakesTurns(int key)
        {
            ITakesTurns turnObj = null;

            GameObject gobj = GetGameObject(key);
            if (gobj != null && gobj is ITakesTurns)
                turnObj = (ITakesTurns)gobj;

            return turnObj;
        }

        private GameObject GetGameObject(int key)
        {
            GameObject obj = null;

            if (_gameObjectTable.ContainsKey(key))
                obj = _gameObjectTable[key];

            //Remove dead GameObjects
            if (obj == null || !obj.Alive)
            {
                _gameObjectTable.Remove(key);
                obj = null;
            }

            return obj;
        }

        #endregion

        #region Factory Methods

        public int CreateActor(string actorType)
        {
            GameObject obj = null;
            if (actorType == "playerStudent")
            {
                obj = new Student(GetNextID());
            }
            else if (actorType == "enemy_dog")
            {
                obj = new Enemy_Dog(GetNextID());
            }
            else if (actorType == "enemy_wisp")
            {
                obj = new Enemy_Wisp(GetNextID());
            }
            else if (actorType == "enemy_bluecap")
            {
                obj = new Enemy_Bluecap(GetNextID());
            }
            else if (actorType == "enemy_gopherkinScrub")
            {
                obj = new Enemy_GopherkinScrub(GetNextID());
            }
            else if (actorType == "enemy_gopherkinAdept")
            {
                obj = new Enemy_GopherkinAdept(GetNextID());
            }
            else if (actorType == "enemy_spectralPupil")
            {
                obj = new Enemy_SpectralPupil(GetNextID());
            }
            else if (actorType == "enemy_armoredWorg")
            {
                obj = new Enemy_ArmoredWorg(GetNextID());
            }
            else if (actorType == "enemy_delver")
            {
                obj = new Enemy_Delver(GetNextID());
            }
            else if (actorType == "enemy_minotaur")
            {
                obj = new Enemy_Minotaur(GetNextID());
            }
            else if (actorType == "enemy_orcBrute")
            {
                obj = new Enemy_OrcBrute(GetNextID());
            }
            else if (actorType == "enemy_restlessSpirit")
            {
                obj = new Enemy_RestlessSpirit(GetNextID());
            }
            else if (actorType == "enemy_zealot")
            {
                obj = new Enemy_Zealot(GetNextID());
            }
            else if (actorType == "enemy_mechanodragon")
            {
                obj = new Enemy_Mechanodragon(GetNextID());
            }
            else if (actorType == "enemy_quazaxRocketeer")
            {
                obj = new Enemy_QuazaxRocketeer(GetNextID());
            }
            else
            {
                throw new NotImplementedException();
            }

            return RegisterGameObject(obj);
        }

        public int CreateItem(string itemType)
        {
            GameObject obj = null;

            if (itemType == "tonic_healthSmall")
            {
                obj = new Potion_Health(GetNextID());
            }
            else if (itemType == "tonic_essenceSmall")
            {
                obj = new Tonic_EssenceSmall(GetNextID());
            }
            else if (itemType == "essenceCrystal" || itemType == "soul")
            {
                obj = new EssenceCrystal(10, GetNextID());
            }
            else if (itemType == "amuletSealing")
            {
                obj = new Amulet_Sealing(GetNextID());
            }
            else if (itemType == "itemSpellbook")
            {
                obj = new Item_Spellbook(GetNextID());
            }
            else
            {
                throw new NotImplementedException();
            }

            return RegisterGameObject(obj);
        }

        public int CreateItemToken(int affectedItemID)
        {
            GameObject obj = null;

            obj = new ItemToken(GetNextID(), affectedItemID);

            return RegisterGameObject(obj);
        }

        public int CreateActorBehavior(string actorBehaviorType, int casterID)
        {
            GameObject obj = null;

            if (actorBehaviorType == "b_genericShield")
            {
                obj = new ActorBehavior_GenericShield(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_genericBuff")
            {
                obj = new ActorBehavior_GenericBuff(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_astralConduit")
            {
                obj = new ActorBehavior_AstralConduit(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_enemyMagicShield")
            {
                obj = new ActorBehavior_EnemyMagicShield(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_damageOverTime")
            {
                obj = new ActorBehavior_DamageOverTime(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_stun")
            {
                obj = new ActorBehavior_Stun(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_invincibility")
            {
                obj = new ActorBehavior_Invincibility(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_xrayVision")
            {
                obj = new ActorBehavior_XrayVision(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_energize")
            {
                obj = new ActorBehavior_Energize(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_boundSpirit")
            {
                obj = new ActorBehavior_BoundSpirit(GetNextID(), casterID);
            }
            else if (actorBehaviorType == "b_scaledSkin")
            {
                obj = new ActorBehavior_ScaledSkin(GetNextID(), casterID);
            }
            else
            {
                throw new NotImplementedException();
            }

            return RegisterGameObject(obj);
        }

        public int CreateTileBehavior(string tileBehaviorType, IntVector2 affectedTile)
        {
            GameObject obj = null;
            if (tileBehaviorType == "door")
            {
                obj = new TileBehavior_Door(GetNextID(), affectedTile);
            }
            else if (tileBehaviorType == "stoneToGlass")
            {
                obj = new TileBehavior_StoneToGlass(GetNextID(), affectedTile);
            }
            else
            {
                throw new NotImplementedException();
            }

            return RegisterGameObject(obj);
        }

        public int CreateSpecialEffect(string specialEffectType)
        {
            GameObject obj = null;
            if (specialEffectType == "specialEffect_basicProjectile")
            {
                obj = new SpecialEffect_BasicProjectile(GetNextID());
            }
            else if (specialEffectType == "specialEffect_basicMelee")
            {
                obj = new SpecialEffect_BasicMelee(GetNextID());
            }
            else if (specialEffectType == "specialEffect_basicBlast")
            {
                obj = new SpecialEffect_BasicBlast(GetNextID());
            }
            else
            {
                throw new NotImplementedException();
            }

            return RegisterGameObject(obj);
        }

        private int RegisterGameObject(GameObject obj)
        {
            int newKey = GetNextID();

            _gameObjectTable.Add(newKey, obj);

            return newKey;
        }

        private int GetNextID()
        {
            int newKey = _nextID;
            while (_gameObjectTable.ContainsKey(newKey))
            {
                _nextID++;
                newKey = _nextID;
            }

            return newKey;
        }

        #endregion


    }
}
