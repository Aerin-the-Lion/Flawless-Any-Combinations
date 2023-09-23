using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;

namespace FlawlessAnyCombinations
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin_the_Lion.Mad_Games_Tycoon_2.plugins.FlawlessAnyCombinations";
        public const string PluginName = "FlawlessAnyCombinations";
        public const string PluginVersion = "1.0.0.0";

        public static ConfigEntry<bool> CFG_IS_ENABLED { get; private set; }
        public static ConfigEntry<bool> CFG_isDisableGenreCombinationRequired { get; private set; }
        public static ConfigEntry<bool> CFG_isDisableSubGenreRequired { get; private set; }

        public static ConfigEntry<bool> CFG_isDisableMainThemeCombinationRequired { get; private set; }
        public static ConfigEntry<bool> CFG_isDisableTargetGroupCombinationRequired { get; private set; }

        //public static ConfigEntry<bool> CFG_isDisableSubThemeRequired { get; private set; }
        public void LoadConfig()
        {
            string textIsEnable = "0. MOD Settings";
            string genreCombination = "1. Genre Combination Settings";
            string themesCombination = "2. Themes Combination Settings";
            string targetGroupCombination = "3. Target Group Combination Settings";
            CFG_IS_ENABLED = Config.Bind<bool>(textIsEnable, "Activate the MOD", true, "If you need to disable the mod, Toggle it to Disabled");

            CFG_isDisableGenreCombinationRequired = Config.Bind<bool>(genreCombination, "Disable Genre Combination", true, "If you need to disable the feature of Genre Combination impacts game scores, Toggle it to Disabled");
            CFG_isDisableSubGenreRequired = Config.Bind<bool>(genreCombination, "Disable Subgenre Impact", true, "If you need to disable the feature that Subgenre impacts game scores, toggle it to Disabled.");
            CFG_isDisableMainThemeCombinationRequired = Config.Bind<bool>(themesCombination, "Disable Themes Combination", true, "If you need to disable the feature of Main(Sub) theme Combination impacts game scores, Toggle it to Disabled");
            CFG_isDisableTargetGroupCombinationRequired = Config.Bind<bool>(targetGroupCombination, "Disable Target Group Combination", true, "If you need to disable the feature of Target Group Combination impacts game scores, Toggle it to Disabled");
            //CFG_isDisableSubThemeRequired = Config.Bind<bool>(themesCombination, "Disable Sub Theme Combination", true, "If you need to disable the feature of Sub theme Combination impacts game scores, toggle it to Disabled.");

            Config.SettingChanged += delegate (object sender, SettingChangedEventArgs args){};
        }

        void Awake()
        {
            LoadConfig();
            Harmony.CreateAndPatchAll(typeof(DisableCombination));

        }

        void Update()
        {
            //UpdateCount++;
            //Debug.Log("Update Count : " + UpdateCount);
        }
    }


}
