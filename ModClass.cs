using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;


namespace AbyssPlus
{
    public class AbyssPlus : Mod, IMenuMod, IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings>
    {
        //Settings
        public static GlobalSettings gs { get; set; } = new GlobalSettings();
        public static LocalSettings ls { get; set; } = new LocalSettings();

        public void OnLoadGlobal(GlobalSettings info) => gs = info;
        public GlobalSettings OnSaveGlobal() => gs;

        public void OnLoadLocal(LocalSettings info) => ls = info;
        public LocalSettings OnSaveLocal() => ls;

        //Menu stuff

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry
                {
                    Name = "Mod enabled",
                    Description = null,
                    Values = new string[]
                    {
                        "Yes",
                        "No"
                    },

                    Saver = opt => gs.isActivated = opt switch
                    {
                        0 => true,
                        1 => false,
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => gs.isActivated switch
                    {
                        true => 0,
                        false => 1
                    },
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Cutscenes enabled",
                    Description = "If disabled, will skip to the end of cutscenes",
                    Values = new string[]
                    {
                        "Yes",
                        "No"
                    },

                    Saver = opt => gs.cutscenesActivated = opt switch
                    {
                        0 => true,
                        1 => false,
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => gs.cutscenesActivated switch
                    {
                        true => 0,
                        false => 1
                    },
                }
            };
        }

        //Mod creation
        new public string GetName() => "AbyssPlus";
        public override string GetVersion() => "v1";

        public static Dictionary<string,AssetBundle> bundle;

        public override void Initialize()
        {
            Log($"Mod activated: {gs.isActivated}, Mod cutscenes activated: {gs.cutscenesActivated}");

            if (gs.isActivated) 
            {
                Log("Initialising");
                ModHooks.HeroUpdateHook += OnUpdate;

                //Refresh();
                ModHooks.AfterSavegameLoadHook += AddLoadSceneShortcut;
                ModHooks.NewGameHook += AddLoadScene;

                Assembly asm = Assembly.GetExecutingAssembly();
                bundle = new Dictionary<string, AssetBundle>();

                foreach (string res in asm.GetManifestResourceNames())
                {
                    using (Stream s = asm.GetManifestResourceStream(res))
                    {
                        string bundleName = Path.GetExtension(res).Substring(1);
                        bundle[bundleName] = AssetBundle.LoadFromStream(s);
                    }
                }
            }
        }

        //Mod functionality
        public void OnUpdate()
        {
            if (gs.isActivated)
            {
                
            }
        }

        public void AddLoadSceneShortcut(SaveGameData data) => AddLoadScene();
        public void AddLoadScene() => GameManager.instance.gameObject.AddComponent<LoadScene>();

        public void Refresh()
        {
            ModHooks.AfterSavegameLoadHook -= AddLoadSceneShortcut;
            ModHooks.NewGameHook -= AddLoadScene;

            var x = GameManager.instance?.gameObject.GetComponent<LoadScene>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}