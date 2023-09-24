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
using System.Reflection.Emit;

namespace FlawlessAnyCombinations
{
    internal class DisableCombination : MonoBehaviour
    {
        public static int gameAP_Gameplay;
        public static int gameAP_Grafik;
        public static int gameAP_Sound;
        public static int gameAP_Technik;

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
            if (!Main.CFG_isDisableMainThemeCombinationRequired.Value) { return true; }
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

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(gameScript))]
        [HarmonyPatch("CalcReview")]
        static IEnumerable<CodeInstruction> DisableDesignFocusTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);

            //インデックスを読み込んで、修正する手法と取っています。ですので、CalcReviewにアップデートが適用された場合は、動かなくなる可能性があります。
            int[] currentIndex = { 1456, 1462, 1469, 1475, 1482, 1488, 1495, 1501, 1508, 1514, 1521, 1527 };
            for (int i = 0; i < codes.Count; i++)
            {
                //Debug.Log(i + "回目 : " + codes[i].opcode);
                if (!Main.CFG_IS_ENABLED.Value){return codes.AsEnumerable();}
                if (!Main.CFG_isDisableDesignFocusRequired.Value) { return codes.AsEnumerable(); }
                for (int j = 0; j < currentIndex.Length; j++)
                {
                    if (i == currentIndex[j] && codes[i].opcode == OpCodes.Ldc_R4)
                    {
                        //マイナスされるfloatを0にして、実質計算しないようにする
                        codes[i].operand = 0f;
                        Debug.Log("-------------------------------------");
                        Debug.Log("Design Focus : Index[" + currentIndex[j] + "] is DISABLED!");
                        Debug.Log("-------------------------------------");
                        found = true;
                    }
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(gameScript))]
        [HarmonyPatch("CalcReview")]
        static IEnumerable<CodeInstruction> DisableDesignDirectionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);

            //インデックスを読み込んで、修正する手法と取っています。ですので、CalcReviewにアップデートが適用された場合は、動かなくなる可能性があります。
            int[] currentIndex = { 1571, 1577, 1584, 1590, 1597, 1603, 1610, 1616, 1623, 1629, 1636, 1642 };
            for (int i = 0; i < codes.Count; i++)
            {
                //Debug.Log(i + "回目 : " + codes[i].opcode);
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }
                if (!Main.CFG_isDisableDesignDirectionRequired.Value) { return codes.AsEnumerable(); }
                for (int j = 0; j < currentIndex.Length; j++)
                {
                    if (i == currentIndex[j] && codes[i].opcode == OpCodes.Ldc_R4)
                    {
                        //マイナスされるfloatを0にして、実質計算しないようにする
                        codes[i].operand = 0f;
                        Debug.Log("-------------------------------------");
                        Debug.Log("Design Direction : Index[" + currentIndex[j] + "] is DISABLED!");
                        Debug.Log("-------------------------------------");
                        found = true;
                    }
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(gameScript), "CalcReview")]
        static bool GetGameAP_Points(gameScript __instance)
        {
            if(!Main.CFG_IS_ENABLED.Value) { return true; }
            if (!Main.CFG_isDisableWorkPriorityRequired.Value) { return true; }
            /*
             * 		num6 += num2 * 0.01f * this.genres_.genres_GAMEPLAY[this.maingenre];
		            num6 += num3 * 0.01f * this.genres_.genres_GRAPHIC[this.maingenre];
		            num6 += num4 * 0.01f * this.genres_.genres_SOUND[this.maingenre];
		            num6 += num5 * 0.01f * this.genres_.genres_CONTROL[this.maingenre];
                    CalcReviewのWorkPriorityについて、解説する。
                    おそらくだが、num2, 3, 4, 5は、それぞれ、GameDesign、Graphic、Sound、Controlが該当する。
                    そして、この上記の式が示すことは、明らかにWorkPriorityが最終的な評価に累乗することを示している。
                    つまり、genres_GAMEPLAY[]が、50であった場合、実際のGameplayのポイントが非常に高い場合は、スコアがかなり高くなる。
                    ……そして、WorkPriorityは、ゲームファイルの..Mad Games Tycoon 2_Data\Extern\Text\DATA\Genres.txtに格納されており、ここから参照されている？
                    →C:\Users\admin\AppData\LocalLow\Eggcode\Mad Games Tycoon 2、savegame.savに格納されている模様。おそらくだが、ランダムで設定を変更するなどもあるので、ここに格納しているのだろう。
                    ここを弄って変更でも良いが、これだと永久的に変更となってしまう。そこで、自身で設定した数値に書き換えようというのが今回の目的。
            */
            gameAP_Gameplay = __instance.gameAP_Gameplay * 5;
            gameAP_Grafik = __instance.gameAP_Grafik * 5;
            gameAP_Sound = __instance.gameAP_Sound * 5;
            gameAP_Technik = __instance.gameAP_Technik * 5;

            __instance.genres_.genres_GAMEPLAY.SetValue(gameAP_Gameplay, __instance.maingenre);
            __instance.genres_.genres_GRAPHIC.SetValue(gameAP_Grafik, __instance.maingenre);
            __instance.genres_.genres_SOUND.SetValue(gameAP_Sound, __instance.maingenre);
            __instance.genres_.genres_CONTROL.SetValue(gameAP_Technik, __instance.maingenre);
            return true;
        }
        /*
        //ゲームの機能の評価点を増やす関数、テストで作っただけ
        [HarmonyPrefix, HarmonyPatch(typeof(DesignplayFeatures), "GetBonus")]
        static bool AnythingGood(DesignplayFeatures __instance, int i, int maingenre_, int subgenre_, ref float __result)
        {
            __instance.DesignplayFeatures_GOOD[0,1] = __instance.DesignplayFeatures_GOOD[i, maingenre_];
            if (__instance.DesignplayFeatures_GOOD[i, maingenre_])
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

            /*
            [HarmonyPrefix, HarmonyPatch(typeof(DesignScript), "CalcReview")]
            static bool DisableDesignFocusCombination(DesignScript __instance)
            {
                if (!Main.CFG_IS_ENABLED.Value) { return true; }
                if (!Main.CFG_isDisableDesignFocusRequired.Value) { return true; }
                //今回は、DesignFocusの正解の数値を、直接レビュー時に入れる方式にする。
                // 結論として、これをしてしまうと、自分で作った点数がどっかに消えて、記録としては、GetFocusの数値で塗り替えられる結果となる。
                //……まあ、結果としてPerfectになるわけだけど、これだと主旨とズレてしまうので、次からやめることとする。
                for (int i = 0; i < __instance.Designschwerpunkt.Length; i++) {
                    Debug.Log("Designschwerpunkt[" + i + "] is :" + __instance.Designschwerpunkt[i]);
                    Debug.Log("genres_.GetFocus[" + i + "] is :" + __instance.genres_.GetFocus(i, __instance.maingenre, __instance.subgenre));
                    __instance.Designschwerpunkt[i] = __instance.genres_.GetFocus(i, __instance.maingenre, __instance.subgenre);
                }
                return true;
            }
            */
        }
    }
