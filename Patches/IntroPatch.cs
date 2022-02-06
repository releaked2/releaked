using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using TownOfHost;
using System.Linq;

namespace TownOfHost
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.SetUpRoleText))]
    class SetUpRoleTextPatch {
        public static void Postfix(IntroCutscene __instance) {
            if (PlayerControl.LocalPlayer.isJester() || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Jester);
                __instance.RoleBlurbText.text = main.getLang(lang.JesterInfo);
                __instance.RoleText.color = main.getRoleColor(CustomRoles.Jester);
                __instance.RoleBlurbText.color = main.getRoleColor(CustomRoles.Jester);
            }
            if (PlayerControl.LocalPlayer.isMadmate())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Madmate);
                __instance.RoleBlurbText.text = main.getLang(lang.MadmateInfo);
                __instance.RoleText.color = Palette.ImpostorRed;
                __instance.RoleBlurbText.color = Palette.ImpostorRed;
            }
            if (PlayerControl.LocalPlayer.isBait())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Bait);
                __instance.RoleBlurbText.text = main.getLang(lang.BaitInfo);
                __instance.RoleText.color = Color.cyan;
                __instance.RoleBlurbText.color = Color.yellow;
            }
            if (PlayerControl.LocalPlayer.isTerrorist())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Terrorist);
                __instance.RoleBlurbText.text = main.getLang(lang.TerroristInfo);
                __instance.RoleText.color = Color.green;
                __instance.RoleBlurbText.color = Color.green;
            }
            if (PlayerControl.LocalPlayer.isMafia())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Mafia);
                __instance.RoleText.fontSize -= 0.5f;
                __instance.RoleBlurbText.text = main.getLang(lang.MafiaInfo);
                __instance.RoleText.color = Palette.ImpostorRed;
                __instance.RoleBlurbText.color = Color.red;
            }
            if (PlayerControl.LocalPlayer.isVampire())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Vampire);
                __instance.RoleText.fontSize -= 0.5f;
                __instance.RoleBlurbText.text = main.getLang(lang.VampireInfo);
                __instance.RoleText.color = Palette.ImpostorRed;
                __instance.RoleBlurbText.color = main.getRoleColor(CustomRoles.Vampire);
            }
            if (PlayerControl.LocalPlayer.isSabotageMaster())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.SabotageMaster);
                __instance.RoleBlurbText.text = main.getLang(lang.SabotageMasterInfo);
                __instance.RoleText.color = Color.blue;
                __instance.RoleBlurbText.color = Color.blue;
            }
            if (PlayerControl.LocalPlayer.isMadGuardian())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.MadGuardian);
                __instance.RoleBlurbText.text = main.getLang(lang.MadGuardianInfo);
                __instance.RoleText.color = Palette.ImpostorRed;
                __instance.RoleBlurbText.color = Palette.ImpostorRed;
            }
            if (PlayerControl.LocalPlayer.isOpportunist())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Opportunist);
                __instance.RoleBlurbText.text = main.getLang(lang.OpportunistInfo);
                __instance.RoleText.color = Color.green;
                __instance.RoleBlurbText.color = Color.green;
            }
            if (PlayerControl.LocalPlayer.isMayor())
            {
                __instance.RoleText.text = main.getRoleName(CustomRoles.Mayor);
                __instance.RoleBlurbText.text = main.getLang(lang.MayorInfo);
                __instance.RoleText.color = main.getRoleColor(CustomRoles.Mayor);
                __instance.RoleBlurbText.color = main.getRoleColor(CustomRoles.Mayor);
            }
            if(main.IsHideAndSeek) {
                if (main.AllPlayerCustomRoles[PlayerControl.LocalPlayer.PlayerId] == CustomRoles.Fox) {
                    __instance.RoleText.text = "Fox";
                    __instance.RoleBlurbText.text = "殺されずに逃げきれ";
                    __instance.RoleText.color = Color.magenta;
                    __instance.RoleBlurbText.color = Color.magenta;
                }
                if (main.AllPlayerCustomRoles[PlayerControl.LocalPlayer.PlayerId] == CustomRoles.Troll) {
                    __instance.RoleText.text = "Troll";
                    __instance.RoleBlurbText.text = "インポスターにキルされろ";
                    __instance.RoleText.color = Color.green;
                    __instance.RoleBlurbText.color = Color.green;
                }
            }
        }
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    class BeginCrewmatePatch
    {
        public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            if (PlayerControl.LocalPlayer.isJester())
            {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }
        }
        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            //チーム表示変更
            var rand = new System.Random();
            if (Input.GetKey(KeyCode.RightShift))
            {
                __instance.TeamTitle.text = "Town Of Host";
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = "https://github.com/tukasa0001/TownOfHost" +
                "\r\nOut Now on Github";
                __instance.TeamTitle.color = Color.cyan;
                __instance.BackgroundBar.material.color = Palette.CrewmateBlue;
                __instance.RoleText.text = "役職名";
                __instance.RoleBlurbText.text = "役職の説明";
            }
            if (PlayerControl.LocalPlayer.isJester())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Jester);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.JesterInfo);
                __instance.TeamTitle.color = main.getRoleColor(CustomRoles.Jester);
                __instance.BackgroundBar.material.color = main.getRoleColor(CustomRoles.Jester);
            }
            if (PlayerControl.LocalPlayer.isMadmate())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Madmate);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.MadmateInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Palette.ImpostorRed;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
            }
            if (PlayerControl.LocalPlayer.isBait())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Bait);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.BaitInfo);
                __instance.TeamTitle.color = Color.cyan;
                __instance.BackgroundBar.material.color = Color.yellow;
            }
            if (PlayerControl.LocalPlayer.isTerrorist())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Terrorist);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.TerroristInfo);
                __instance.TeamTitle.color = Color.green;
                __instance.BackgroundBar.material.color = Color.green;
                var sound = ShipStatus.Instance.CommonTasks.Where(task => task.TaskType == TaskTypes.FixWiring).FirstOrDefault()
                .MinigamePrefab.OpenSound;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = sound;
            }
            if (PlayerControl.LocalPlayer.isMafia())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Mafia);
                __instance.TeamTitle.fontSize -= 0.5f;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.MafiaInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Color.red;
            }
            if (PlayerControl.LocalPlayer.isVampire())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Vampire);
                __instance.TeamTitle.fontSize -= 0.5f;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.VampireInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = main.getRoleColor(CustomRoles.Vampire);
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }
            if (PlayerControl.LocalPlayer.isSabotageMaster())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.SabotageMaster);
                __instance.TeamTitle.fontSize -= 0.75f;
                __instance.TeamTitle.fontSizeMin = __instance.TeamTitle.fontSize;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.SabotageMasterInfo);
                __instance.TeamTitle.color = Color.blue;
                __instance.BackgroundBar.material.color = Color.blue;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = ShipStatus.Instance.SabotageSound;
            }
            if (PlayerControl.LocalPlayer.isMadGuardian())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.MadGuardian);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.MadGuardianInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Palette.ImpostorRed;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
            }
            if (PlayerControl.LocalPlayer.isOpportunist())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Opportunist);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.OpportunistInfo);
                __instance.TeamTitle.color = Color.green;
                __instance.BackgroundBar.material.color = Color.green;
            }
            if (PlayerControl.LocalPlayer.isSnitch())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Snitch);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.SnitchInfo);
                __instance.TeamTitle.color = main.getRoleColor(CustomRoles.Snitch);
                __instance.BackgroundBar.material.color = main.getRoleColor(CustomRoles.Snitch);
            }
            if (PlayerControl.LocalPlayer.isMayor())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Mayor);
                __instance.TeamTitle.fontSizeMin = __instance.TeamTitle.fontSize;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.MayorInfo);
                __instance.TeamTitle.color = main.getRoleColor(CustomRoles.Mayor);
                __instance.BackgroundBar.material.color = main.getRoleColor(CustomRoles.Mayor);
                PlayerControl.LocalPlayer.Data.Role.IntroSound = MeetingHud.Instance.VoteEndingSound;
            }
            if (PlayerControl.LocalPlayer.isSheriff())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Sheriff);
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.SheriffInfo);
                __instance.TeamTitle.color = Color.yellow;
                __instance.BackgroundBar.material.color = Color.yellow;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Scientist);
            }
            if (PlayerControl.LocalPlayer.isBountyHunter())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.BountyHunter);
                __instance.TeamTitle.fontSize -= 0.5f;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.BountyHunterInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Color.red;
            }
            if (PlayerControl.LocalPlayer.isWarlock())
            {
                __instance.TeamTitle.text = main.getRoleName(CustomRoles.Warlock);
                __instance.TeamTitle.fontSize -= 0.5f;
                __instance.ImpostorText.gameObject.SetActive(true);
                __instance.ImpostorText.text = main.getLang(lang.WarlockInfo);
                __instance.TeamTitle.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Color.red;
            }
            if(main.IsHideAndSeek) {
                if (main.AllPlayerCustomRoles[PlayerControl.LocalPlayer.PlayerId] == CustomRoles.Fox) {
                    __instance.TeamTitle.text = "Fox";
                    __instance.TeamTitle.fontSize -= 0.5f;
                    __instance.ImpostorText.gameObject.SetActive(true);
                    __instance.ImpostorText.text = "殺されずに逃げきれ";
                    __instance.TeamTitle.color = Color.magenta;
                    __instance.BackgroundBar.material.color = Color.magenta;
                    __instance.RoleText.text = "狐";
                    __instance.RoleBlurbText.text = "殺されずに逃げきれ";
                }
                if (main.AllPlayerCustomRoles[PlayerControl.LocalPlayer.PlayerId] == CustomRoles.Troll) {
                    __instance.TeamTitle.text = "Troll";
                    __instance.TeamTitle.fontSize -= 0.5f;
                    __instance.ImpostorText.gameObject.SetActive(true);
                    __instance.ImpostorText.text = "インポスターにキルされろ";
                    __instance.TeamTitle.color = Color.green;
                    __instance.BackgroundBar.material.color = Color.green;
                    __instance.RoleText.text = "トロール";
                    __instance.RoleBlurbText.text = "インポスターにキルされろ";
                }
            }
        }
        private static AudioClip GetIntroSound(RoleTypes roleType) {
            return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleType).FirstOrDefault().IntroSound;
        }
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    class BeginImpostorPatch
    {
        public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            //TODO:シェリフ時にプレイヤーの間隔とかをクルーの場合と同じにする
            BeginCrewmatePatch.Prefix(__instance, ref yourTeam);
        }
        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            BeginCrewmatePatch.Postfix(__instance, ref yourTeam);
        }
    }
}
