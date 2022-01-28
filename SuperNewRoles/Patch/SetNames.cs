﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SuperNewRoles.Patch
{
    public class SetNamesClass
    {
        private static string roleNames;
        private static Color roleColors;

        public static void SetPlayerNameColor(PlayerControl p, Color color)
        {
            p.nameText.color = color;
        }
        public static void SetPlayerRoleInfo(PlayerControl p)
        {
                    Transform playerInfoTransform = p.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo == null)
                    {
                        playerInfo = UnityEngine.Object.Instantiate(p.nameText, p.nameText.transform.parent);
                        playerInfo.fontSize *= 0.75f;
                        playerInfo.gameObject.name = "Info";
                    }

                    // Set the position every time bc it sometimes ends up in the wrong place due to camoflauge
                    playerInfo.transform.localPosition = p.nameText.transform.localPosition + Vector3.up * 0.5f;

                    PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                    Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                    TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (meetingInfo == null && playerVoteArea != null)
                    {
                        meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                        meetingInfo.transform.localPosition += Vector3.down * 0.1f;
                        meetingInfo.fontSize = 1.5f;
                        meetingInfo.gameObject.name = "Info";
                    }

            // Set player name higher to align in middle
            if (meetingInfo != null && playerVoteArea != null)
            {
                var playerName = playerVoteArea.NameText;
                playerName.transform.localPosition = new Vector3(0.3384f, (0.0311f + 0.0683f), -0.1f);
            }

                    var role = p.getRole();
            if (role == CustomRPC.RoleId.DefaultRole) {
                if (p.Data.Role.IsImpostor) 
                { 
                    roleNames = "ImpostorName"; 
                    roleColors = Roles.RoleClass.ImpostorRed; 
                }
                else 
                {
                    roleNames = "CrewMateName";
                    roleColors = Roles.RoleClass.CrewmateWhite;
                }

            } else
            {
                var introdate = Intro.IntroDate.GetIntroDate(role);
                roleNames = introdate.NameKey + "Name";
                roleColors = introdate.color;
            }
                    
                    
                    string playerInfoText = "";
                    string meetingInfoText = "";
                        playerInfoText = $"{CustomOption.CustomOptions.cs(roleColors, roleNames)}";
                        meetingInfoText = $"{CustomOption.CustomOptions.cs(roleColors, roleNames)}".Trim();



                    playerInfo.text = playerInfoText;
                    playerInfo.gameObject.SetActive(p.Visible);
            if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;  p.nameText.color = roleColors;
        }
        public static void SetPlayerNameColors(PlayerControl player)
        {
            var role = player.getRole();
            if (role == CustomRPC.RoleId.DefaultRole) return;
            SetPlayerNameColor(player, Intro.IntroDate.GetIntroDate(role).color);
        }
        public static void SetPlayerRoleNames(PlayerControl player)
        {
            SetPlayerRoleInfo(player);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerFixedUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    SetNamesClass.SetPlayerRoleNames(player);
                    SetNamesClass.SetPlayerNameColors(player);
                }
            }
            else
            {
                SetNamesClass.SetPlayerRoleNames(PlayerControl.LocalPlayer);
                SetNamesClass.SetPlayerNameColors(PlayerControl.LocalPlayer);
            }
        }
    }
}