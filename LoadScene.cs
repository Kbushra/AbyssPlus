using System.Collections;
using System.Data.SqlClient;
using GlobalEnums;
using Modding;
using Satchel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AbyssPlus
{
    public class LoadScene : MonoBehaviour
    {
        string scene;
        string prevscene;
        Satchel.Core satchel;

        private void Start()
        {
            Modding.Logger.Log("[AbyssPlus] - Scene manager created");
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += PrepareObjects;
            On.GameManager.OnNextLevelReady += PositionKnight;
        }

        private void PrepareObjects(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
        {
            prevscene = from.name;
            scene = to.name;

            if (scene == "Town")
            {
                //Gateway
                CreateGateway("left1", new Vector2(30, 5),
                new Vector2(99, 99), "AbyssCorpseRoom", "right1", false, true, GameManager.SceneLoadVisualizations.Default);
            }
            else if (scene == "AbyssCorpseRoom")
            {
                //Scene Manager
                GameObject scenemanager = GameObject.Find("SceneManager");
                scenemanager.AddComponent<SceneManager>();
                SetUpLevel(scenemanager.GetComponent<SceneManager>());

                var dreamnailmanager = satchel.GetCustomDreamNailManager();
                var convo = new Convo("DreamStatue");
                dreamnailmanager.SetText(convo, "Father...");
                dreamnailmanager.SetText(convo, "Mother...");
                dreamnailmanager.SetText(convo, "Anyone...");
            }
        }

        private void SetUpLevel(SceneManager self)
        {
            if (scene == "AbyssCorpseRoom")
            {
                self.sceneType = SceneType.GAMEPLAY;
                self.mapZone = MapZone.ABYSS;
                self.darknessLevel = 999;
                self.saturation = 0.2f;
                self.ignorePlatformSaturationModifiers = false;
                self.isWindy = false;
                self.isTremorZone = false;
                self.environmentType = 2;
                self.noParticles = false;
                self.overrideParticlesWith = MapZone.ABYSS;
                self.defaultColor = new Color(33f, 34f, 36f, 1f);
                self.defaultIntensity = 0.2f;
                self.heroLightColor = new Color(117f, 121f, 128f);

                Modding.Logger.Log("[AbyssPlus] - Custom room loaded");
            }
        }

        private void PositionKnight(On.GameManager.orig_OnNextLevelReady orig, GameManager self)
        {
            orig(self);
            if (scene == "AbyssCorpseRoom") { HeroController.instance.transform.position = new Vector2(29, 10); }
        }

        //private void Update() => Modding.Logger.Log("");

        private void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate,
        bool right, bool left, GameManager.SceneLoadVisualizations vis)
        {
            GameObject gate = new GameObject(gateName);
            gate.transform.SetPosition2D(pos);
            var tp = gate.AddComponent<TransitionPoint>();

            var gbc = gate.AddComponent<BoxCollider2D>();
            gbc.size = size;
            gbc.isTrigger = true;
            tp.targetScene = toScene;
            tp.entryPoint = entryGate;

            tp.alwaysEnterLeft = left;
            tp.alwaysEnterRight = right;
            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            int side;
            if (right) { side = -1; } else { side = 1; }
            rm.transform.position = new Vector2(tp.transform.position.x + (side * 3f), tp.transform.position.y);
            rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
            tp.sceneLoadVisualization = vis;
        }
    }
}
