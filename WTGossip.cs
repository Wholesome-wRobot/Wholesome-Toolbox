﻿using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Helpers;

namespace WholesomeToolbox
{
    /// <summary>
    /// Gossip (vendors, questgivers etc...) related methods
    /// </summary>
    public class WTGossip
    {
        /// <summary>
        /// Returns whether a NPC gossip or quest frame is currently open
        /// </summary>
        public static bool IsQuestGiverFrameActive => Lua.LuaDoString<bool>("return GetClickFrame('GossipFrame'):IsVisible() == 1 or GetClickFrame('QuestFrame'):IsVisible() == 1;");
        
        /// <summary>
        /// Returns whether the complete quest button is visible in current gossip
        /// </summary>
        public static bool QuestFrameCompleteQuestButtonIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('QuestFrameCompleteQuestButton'):IsVisible() == 1 or QuestFrameCompleteQuestButton:IsVisible() == 1");

        /// <summary>
        /// Returns whether the accept quest button is visible in current gossip
        /// </summary>
        public static bool QuestFrameAcceptButtonIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('QuestFrameAcceptButton'):IsVisible() == 1");

        /// <summary>
        /// Returns whether the complete button is visible in current gossip
        /// </summary>
        public static bool QuestFrameCompleteButtonIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('QuestFrameCompleteButton'):IsVisible() == 1");

        /// <summary>
        /// Returns whether the quest frame is visible
        /// </summary>
        public static bool QuestFrameIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('QuestFrame'):IsVisible() == 1");

        /// <summary>
        /// Returns whether the gossip frame is visible
        /// </summary>
        public static bool GossipFrameIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('GossipFrame'):IsVisible() == 1");

        /// <summary>
        /// Returns whether the complete quest close button is visible in current gossip
        /// </summary>
        public static bool QuestFrameCloseButtonIsVisible => Lua.LuaDoString<bool>("return GetClickFrame('QuestFrameCloseButton'):IsVisible() == 1");

        /// <summary>
        /// Returns whether the quest items frame is active (usually when ready to hand in)
        /// </summary>
        public static bool HasQuestItems => Lua.LuaDoString<bool>("return GetNumQuestItems() > 0;");

        /// <summary>
        /// Returns whether the reward selection is visible in current gossip
        /// </summary>
        public static bool HasQuestChoices => Lua.LuaDoString<bool>("return GetNumQuestChoices() > 0;");

        /// <summary>
        /// Returns the number of quest choices
        /// </summary>
        public static int NbQuestChoices => Lua.LuaDoString<int>("return GetNumQuestChoices();");

        /// <summary>
        /// Returns whether a vendor gossip is open
        /// </summary>
        public static bool IsVendorGossipOpen => Lua.LuaDoString<int>("return GetMerchantNumItems()") > 0;

        /// <summary>
        /// Returns whether a trainer gossip is open
        /// </summary>
        public static bool IsTrainerGossipOpen => Lua.LuaDoString<int>("return GetNumTrainerServices()") > 0;

        /// <summary>
        /// Learns a spell by name. Trainer gossip must be open.
        /// </summary>
        /// <param name="spellName"></param>
        public static void LearnSpellByName(string spellName)
        {
            WTLogger.Log($"Learning spell {spellName}");
            Lua.LuaDoString($@"
                for i=1,GetNumTrainerServices() do
                    local name = GetTrainerServiceInfo(i)
                    if (name == ""{spellName}"") then
                        BuyTrainerService(i)
                     end
                end
            ");
        }

        /// <summary>
        /// Enable the available trainer filter and expands all. Trainer service must be open
        /// </summary>
        public static void ShowAndExpandAvailableTrainerSpells()
        {
            Lua.LuaDoString($"SetTrainerServiceTypeFilter('available', 1)");
            Lua.LuaDoString($"ExpandTrainerSkillLine(0)");
        }

        /// <summary>
        /// Repairs all. Repair icon must be present in vendor gossip
        /// </summary>
        public static void RepairAll()
        {
            Lua.LuaDoString("MerchantRepairAllButton:Click();", false);
            Lua.LuaDoString("RepairAllItems();", false);
        }

        /// <summary>
        /// Buys the specified amount of items by name. Will buy the higher ceiling if stacks don't exactly match. 
        /// ex: if you buy 17 drinks, it will buy 4x5 drinks
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="amount"></param>
        /// <param name="stackValue"></param>
        public static void BuyItem(string itemName, int amount, int stackValue)
        {
            double numberOfStacksToBuy = (int)(System.Math.Ceiling(amount / (double)stackValue));
            WTLogger.Log($"Buying {amount} x {itemName} ({numberOfStacksToBuy} stack(s) of {stackValue})");
            for (int  i = 0; i < numberOfStacksToBuy; i++)
            {
                bool stackBought = Lua.LuaDoString<bool>($@"
                        for i=1, GetMerchantNumItems() do
                            local name = GetMerchantItemInfo(i);
                            if name and name == ""{itemName}"" then 
                                BuyMerchantItem(i, 1);
                                return true;
                            end
                        end
                        return false;
                    ");

                if (stackBought)
                    Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Clicks on a frame button
        /// ex: ClickOnFrameButton("StaticPopup1Button1")
        /// </summary>
        /// <param name="button"></param>
        public static void ClickOnFrameButton(string button)
        {
            Lua.LuaDoString($@"
                    if GetClickFrame('{button}'):IsVisible() then
                        {button}:Click();
                    end
                ");
        }
    }
}
