using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static TownOfHost.Translator;

namespace TownOfHost
{
    public static class GuessManager
    {

        public static string GetFormatString() {
            string text = "玩家编号列表：";
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc.Data.IsDead) continue;
                string id = pc.PlayerId.ToString();
                string name = pc.GetRealName();
                text += $"\n{id} → {name}";
            }
            return text;
        }

        public static bool CheckCommond(ref string msg, string command, bool exact = true)
        {
            var comList = command.Split('|');
            for (int i = 0; i < comList.Count(); i++)
            {
                if (exact)
                {
                    if (msg == "/" + comList[i]) return true;
                }
                else
                {
                    if (msg.StartsWith("/" + comList[i]))
                    {
                        msg = msg.Replace("/" + comList[i], string.Empty);
                        return true;
                    }
                }
            }
            return false;
        }

        public static byte GetColorFromMsg(string msg)
        {
            if (ComfirmIncludeMsg(msg, "红|紅|red")) return 0;
            if (ComfirmIncludeMsg(msg, "蓝|藍|深蓝|blue")) return 1;
            if (ComfirmIncludeMsg(msg, "绿|綠|深绿|green")) return 2;
            if (ComfirmIncludeMsg(msg, "粉红|粉紅|pink")) return 3;
            if (ComfirmIncludeMsg(msg, "橘|橘|orange")) return 4;
            if (ComfirmIncludeMsg(msg, "黄|黃|yellow")) return 5;
            if (ComfirmIncludeMsg(msg, "黑|黑|black")) return 6;
            if (ComfirmIncludeMsg(msg, "白|白|white")) return 7;
            if (ComfirmIncludeMsg(msg, "紫|紫|perple")) return 8;
            if (ComfirmIncludeMsg(msg, "棕|棕|brown")) return 9;
            if (ComfirmIncludeMsg(msg, "青|青|cyan")) return 10;
            if (ComfirmIncludeMsg(msg, "黄绿|黃綠|浅绿|lime")) return 11;
            if (ComfirmIncludeMsg(msg, "红褐|紅褐|深红|maroon")) return 12;
            if (ComfirmIncludeMsg(msg, "玫红|玫紅|浅粉|rose")) return 13;
            if (ComfirmIncludeMsg(msg, "焦黄|焦黃|淡黄|banana")) return 14;
            if (ComfirmIncludeMsg(msg, "灰|灰|gray")) return 15;
            if (ComfirmIncludeMsg(msg, "茶|茶|tan")) return 16;
            if (ComfirmIncludeMsg(msg, "珊瑚|珊瑚|coral")) return 17;
            return byte.MaxValue;
        }

        private static bool ComfirmIncludeMsg(string msg, string key)
        {
            var keys = key.Split('|');
            for (int i = 0; i < keys.Count(); i++)
            {
                if (msg.Contains(keys[i])) return true;
            }
            return false;
        }

        public static bool GuesserMsg(PlayerControl pc, string msg)
        {
            if (!AmongUsClient.Instance.AmHost) return false;
            if (!GameStates.IsInGame || pc == null) return false;
            if (!pc.Is(CustomRoles.NiceGuesser) && !pc.Is(CustomRoles.EvilGuesser)) return false;

            int operate = 0; // 1:ID 2:下注
            msg = msg.ToLower().TrimStart().TrimEnd();
            if (CheckCommond(ref msg, "id|guesslist|gl编号|玩家编号|玩家id|id列表|玩家列表|列表|所有id|全部id")) operate = 1;
            else if (CheckCommond(ref msg, "shoot|guess|bet|st|gs|bt|猜|赌", false)) operate = 2;
            else return false;

            if (pc.Data.IsDead)
            {
                Utils.SendMessage("死亡后不能进行赌注", pc.PlayerId);
                return true;
            }

            if (operate == 1)
            {
                Utils.SendMessage(GetFormatString(), pc.PlayerId);
                return true;
            }
            else if (operate == 2)
            {
                if(!MsgToPlayerAndRole(msg, out byte targetId, out CustomRoles role, out string error))
                {
                    Utils.SendMessage(error, pc.PlayerId);
                    return true;
                }
                var target = Utils.GetPlayerById(targetId);
                if (target != null)
                {
                    bool guesserSuicide = false;
                    if (role == CustomRoles.SuperStar && target.Is(CustomRoles.SuperStar))
                    {
                        Utils.SendMessage("大明星才不会和你赌博，选个别的目标吧？", pc.PlayerId);
                        return true;
                    }
                    if (pc == target)
                    {
                        Utils.SendMessage("有一说一，你是懂赌怪的",pc.PlayerId, Utils.ColorString(Color.cyan, "咔皮呆留言"));
                        guesserSuicide = true;
                    }
                    else if (pc.Is(CustomRoles.NiceGuesser) && role.IsCrewmate() && !Options.GGCanGuessCrew.GetBool()) guesserSuicide = true;
                    else if (pc.Is(CustomRoles.EvilGuesser) && role.IsImpostor() && !Options.EGCanGuessImp.GetBool()) guesserSuicide = true;
                    else if(!target.Is(role)) guesserSuicide = true;
                    var dp = guesserSuicide ? pc : target;

                    string Name = dp.GetRealName();
                    Utils.SendMessage($"很遗憾，{Name} 在赌局中失利了");

                    new LateTask(() =>
                    {
                        dp.SetRealKiller(pc);
                        dp.RpcMurderPlayer(dp);
                        Main.PlayerStates[dp.PlayerId].deathReason = PlayerState.DeathReason.Gambled;
                        Main.PlayerStates[dp.PlayerId].SetDead();
                    }, 0.9f, "Guesser Killed");

                    foreach (var cpc in Main.AllPlayerControls)
                    {
                        cpc.RpcSetNameEx(cpc.GetRealName(isMeeting: true));
                    }
                    ChatUpdatePatch.DoBlockChat = false;
                    Utils.NotifyRoles(isMeeting: true, NoCache: true);
                }

            }
            return true;
        }

        private static bool MsgToPlayerAndRole(string msg, out byte id, out CustomRoles role, out string error)
        {
            if (msg.StartsWith("/")) msg = msg.Replace("/", string.Empty);

            Regex r = new("\\d+");
            bool ismatch = r.IsMatch(msg);
            MatchCollection mc = r.Matches(msg);
            string result = string.Empty;
            for (int i = 0; i < mc.Count; i++)
            {
                result += mc[i];//匹配结果是完整的数字，此处可以不做拼接的
            }

            if (int.TryParse(result, out int num))
            {
                id = Convert.ToByte(num);
            }
            else
            {
                //并不是玩家编号，判断是否颜色
                //byte color = GetColorFromMsg(msg);
                //好吧我不知道怎么取某位玩家的颜色，等会了的时候再来把这里补上
                id = byte.MaxValue;
                error = "指令格式：/bt [玩家编号] [职业名]\n示例：/bt 3 告密者\n玩家编号会显示在玩家名字旁边的括号内\n或者使用/id查看玩家编号列表";
                role = new();
                return false;
            }

            //判断选择的玩家是否合理
            bool targetIsNull = false;
            PlayerControl target = new();
            try {target = Utils.GetPlayerById(id);}
            catch {targetIsNull = true; }
            if (targetIsNull || target == null || target.Data.IsDead)
            {
                error = "请选择一个在场的存活角色进行赌注";
                role = new();
                return false;
            }

            if (!ChatCommands.GetRoleByName(msg, out role))
            {
                error = "指令格式：/bt [玩家编号] [职业名]\n示例：/bt 3 告密者\n玩家编号会显示在玩家名字旁边的括号内\n或者使用/id查看玩家编号列表";
                return false;
            }

            error= string.Empty;
            return true;
        }

    }
}