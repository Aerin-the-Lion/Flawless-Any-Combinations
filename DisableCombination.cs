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
using System.Runtime.CompilerServices;
using System.Reflection;

namespace FlawlessAnyCombinations
{
    internal class DisableCombination : MonoBehaviour
    {
        //どちらのジャンルorテーマも在り、そしてそのジャンルorテーマのコンボが正しい場合は、Trueを返す。
        //そして、その後の処理において、IsGenreCombinationがFalseの場合は、評価値を下げるということをしている。
        //この、評価値を下げることをDisableする。要は、どのコンボでも、たとえ一つのジャンルでも、評価値は一定になるということ。

        [HarmonyPrefix, HarmonyPatch(typeof(genres), "IsGenreCombination")]
        static bool DisableGenreCombination(genres __instance, int genre_, int subgenre_, ref bool __result)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return true; }

            if (!Main.CFG_isDisableSubGenreRequired.Value && Main.CFG_isDisableGenreCombinationRequired.Value)
            {
                __result = genre_ >= 0 && subgenre_ >= 0;
                return false;
            }

            if (!Main.CFG_isDisableSubGenreRequired.Value && !Main.CFG_isDisableGenreCombinationRequired.Value)
            {
                __result = genre_ >= 0 && subgenre_ >= 0 && __instance.genres_COMBINATION[genre_, subgenre_];
                return false;
            }

            __result = true;
            return false;

            //original
            //return genre_ >= 0 && subgenre_ >= 0 && __instance.genres_COMBINATION[genre_, subgenre_];
        }

        [HarmonyPrefix, HarmonyPatch(typeof(themes), "IsThemesFitWithGenre")]
        static bool DisableThemeCombination(genres __instance, int theme_, int genre_, ref bool __result)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return true; }


            if (!Main.CFG_isDisableMainThemeCombinationRequired.Value){ return true; }

            __result = true;
            return false;

            //original
            //return theme_ >= 0 && genre_ >= 0 && theme_ <= this.themes_LEVEL.Length && this.themes_FITGENRE[theme_, genre_];
        }

        [HarmonyPrefix, HarmonyPatch(typeof(genres), "IsTargetGroup")]
        static bool DisableTargetGroupCombination(genres __instance, int genre_, int group_, ref bool __result)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return true; }
            if (!Main.CFG_isDisableTargetGroupCombinationRequired.Value) { return true; }

            __result = true;
            return false;

            //original
            //return genre_ >= 0 && group_ >= 0 && this.genres_TARGETGROUP[genre_, group_];
        }
        /*
        //ゲームの機能の評価点を増やす関数、テストで作っただけ
        [HarmonyPrefix, HarmonyPatch(typeof(gameplayFeatures), "GetBonus")]
        static bool AnythingGood(gameplayFeatures __instance, int i, int maingenre_, int subgenre_, ref float __result)
        {
            __instance.gameplayFeatures_GOOD[0,1] = __instance.gameplayFeatures_GOOD[i, maingenre_];
            if (__instance.gameplayFeatures_GOOD[i, maingenre_])
            {
                Debug.Log("Success to GetBonus!");
                return true;
            }
            else
            {
                Debug.Log("Failed to GetBonus");
                __result = 999f;
                return false;
            }
        }
        */
    }
}
