using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HazardPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using SPT.Reflection.Patching;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tetris.DeHazardifier
{
    [BepInPlugin("com.Tetris.DeHazardifier", "Tetris.DeHazardifier", "1.0.0")]

    public class DeHazardifier : BaseUnityPlugin
    {
        private ConfigEntry<bool> _deHazardifierMasterConfig;
        private ConfigEntry<bool> _minefieldsConfig;
        private ConfigEntry<bool> _directionalMinesConfig;
        private ConfigEntry<bool> _directionalMinesVisualsConfig;
        private ConfigEntry<bool> _barbedWireConfig;
        private ConfigEntry<bool> _barbedWireVisualsConfig;
        private ConfigEntry<bool> _sniperBorderZoneConfig;
        private ConfigEntry<bool> _fireDamageConfig;
        
        private static GameWorld gameWorld;
        public static bool MapLoaded() => Singleton<GameWorld>.Instantiated;
        public static List<GameObject> savedDirectionalMinesObjects = new List<GameObject>();
        public static List<GameObject> savedBarbedWireObjects = new List<GameObject>();
        public static bool deHazardifiered = false;

        private readonly ModulePatch[] _minefieldPatches =
        {
            new MinefieldCoroutinePatch(),
            new MinefieldCoroutinePatch(),
            new MinefieldDamagePatch(),
            new MinefieldViewTriggerPatch()
        };
        
        private readonly ModulePatch[] _directionalMinesPatches =
        {
            new MineDirectionalAwakePatch(),
            new MineDirectionalTriggerPatch(),
            new MineDirectionalTriggerColliderPatch(),
            new MineDirectionalDamagePatch()
        };

        private readonly ModulePatch[] _barbedWirePatches =
        {
            new BarbedWireDamagePatch(),
            new BarbedWireSpeedPenaltyPatch(),
            new BarbedWireSpeedPenalty2Patch()
        };

        private readonly ModulePatch[] _sniperBorderZonePatches =
        {
            new SniperImitatorAwakePatch(),
            new SniperImitatorDamagePatch(),
            new SniperImitatorShootPatch(),
            new SniperFiringZoneShootPatch(),
            new SniperFiringZoneCoroutinePatch(),
            new SniperFiringZoneTargetPatch(),
            new SniperFiringZoneTarget2Patch()
        };

        private readonly ModulePatch[] _fireDamagePatches =
        {
            new FlameDamageTriggerPatch()
        };

        private (ConfigEntry<bool> config, ModulePatch[] patches)[] _groups;
        private void Awake()
        {
            _deHazardifierMasterConfig = Config.Bind("A - De-Hazardifier Enabler", "A - De-Hazardifier Enabled", true, "Enables the De-Hazardifier.");
            _minefieldsConfig = Config.Bind("A - De-Hazardifier Settings", "A - Minefield Disabler", true, "Disables minefields.");
            _directionalMinesConfig = Config.Bind("A - De-Hazardifier Settings", "B - Claymore Mines Disabler", true, "Disables claymore mines.");
            _directionalMinesVisualsConfig = Config.Bind("A - De-Hazardifier Settings", "C - Claymore Mines Visuals Disabler", true, "Disables visual model of claymore mines.");
            _barbedWireConfig = Config.Bind("A - De-Hazardifier Settings", "D - Barbed Wire Disabler", true, "Disables barbed wire.");
            _barbedWireVisualsConfig = Config.Bind("A - De-Hazardifier Settings", "E - Barbed Wire Visuals Disabler", true, "Disables visual model of barbed wire.");
            _sniperBorderZoneConfig = Config.Bind("A - De-Hazardifier Settings", "F - Sniper Border Zones Disabler", true, "Disables sniper border zones.");
            _fireDamageConfig = Config.Bind("A - De-Hazardifier Settings", "G - Fire Damage Disabler", true, "Disables damage taken from standing in fire.");

            _groups = new[]
            {
                (_minefieldsConfig, _minefieldPatches),
                (_directionalMinesConfig, _directionalMinesPatches),
                (_barbedWireConfig, _barbedWirePatches),
                (_sniperBorderZoneConfig,_sniperBorderZonePatches),
                (_fireDamageConfig, _fireDamagePatches)
            };
            
            foreach (var (cfg,patches) in _groups)
                cfg.SettingChanged += (sender, args) => ApplyGroup(cfg.Value, patches);

            _deHazardifierMasterConfig.SettingChanged += (sender, args) => ApplyMasterConfig();
            _directionalMinesVisualsConfig.SettingChanged += OnApplyDirectionalMinesVisualsSettingChanged;
            _barbedWireVisualsConfig.SettingChanged += OnApplyBarbedWireVisualsSettingChanged;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            ApplyMasterConfig();
        }

        private void ApplyGroup(bool enable, ModulePatch[] patches)
        {
            foreach (var patch in patches)
            {
                patch.SetEnabled(enable);
            }
        }
        
        private void ApplyMasterConfig()
        {
            // if master is OFF, disable everything
            if (!_deHazardifierMasterConfig.Value)
            {
                foreach (var (cfg,patches) in _groups)
                    ApplyGroup(false, patches);
                return;
            }

            // otherwise re‐apply each subgroup based on its own config
            foreach (var (cfg,patches) in _groups)
                ApplyGroup(cfg.Value, patches);
            return;
        }

        private void OnApplyDirectionalMinesVisualsSettingChanged(object sender, EventArgs e)
        {
            DirectionalMinesVisualsScene();
        }
        
        private void OnApplyBarbedWireVisualsSettingChanged(object sender, EventArgs e)
        {
            BarbedWireVisualsScene();
        }
        
        private void BarbedWireVisualsScene()
        {
            foreach (GameObject barbedWireObject in savedBarbedWireObjects)
            {
                barbedWireObject?.SetActive(_barbedWireVisualsConfig.Value);
            }
        }
        
        private void DirectionalMinesVisualsScene()
        {
            foreach (GameObject directionalMinesObject in savedDirectionalMinesObjects)
            {
                directionalMinesObject?.SetActive(_directionalMinesVisualsConfig.Value);
            }
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            savedDirectionalMinesObjects.Clear();
            savedBarbedWireObjects.Clear();
            deHazardifiered = false;
        }
        
        private void Update()
        {
            if (!MapLoaded() || deHazardifiered || !_deHazardifierMasterConfig.Value)
                return;

            gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.MainPlayer == null)
                return;

            StaticManager.BeginCoroutine(DeHazardifyVisuals());
            DirectionalMinesVisualsScene();
            BarbedWireVisualsScene();
            deHazardifiered = true;
        }
        
        private IEnumerator DeHazardifyVisuals()
        {
            List<GameObject> allGameObjects = new List<GameObject>();
            GameObject[] rootObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject root in rootObjects)
            {
                bool isMine = root.GetComponent<MineDirectional>() != null;
                string isMineGrouped = root.name.ToLower();
                bool isBarbedWire = root.GetComponent<BarbedWire>() != null;
                if (_directionalMinesVisualsConfig.Value && (isMine || isMineGrouped == "mines"))
                {
                    allGameObjects.Add(root);
                    savedDirectionalMinesObjects.Add(root);
                }
                if (_barbedWireVisualsConfig.Value && isBarbedWire)
                {
                    allGameObjects.Add(root);
                    savedBarbedWireObjects.Add(root);
                }
            }
            yield break;
        }
    }

    static class PatchExtensions
    {
        public static void SetEnabled(this ModulePatch patch, bool enabled)
        {
            if (enabled)
                patch.Enable();
            else
                patch.Disable();
        }
    }
}
