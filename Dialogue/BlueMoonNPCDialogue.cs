/* doesn't work
using BlueMoon.Events;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Dialogue
{
    public class BlueMoonNPCDialogue : GlobalNPC
    {
        public override void OnChatButtonClicked(NPC npc, bool shop)
        {
            if (BlueMoonEvent.blueMoon)
            {
                if (npc.type == NPCID.Nurse)
                {
                    Main.npcChatText = "The Blue Moon is so calming, isn't it? It's a nice change of pace.";
                }
                else if (npc.type == NPCID.Dryad)
                {
                    Main.npcChatText = "The energies of the Blue Moon harmonize with nature in a unique way.";
                }
                else if (npc.type == NPCID.Guide)
                {
                    Main.npcChatText = "Did you know? The Blue Moon event is a rare celestial occurrence. We're lucky to witness it.";
                }
                else if (npc.type == NPCID.Merchant)
                {
                    Main.npcChatText = "A peaceful night like this can be quite profitable, don't you think?";
                }
                else if (npc.type == NPCID.Demolitionist)
                {
                    Main.npcChatText = "I hope nobody tries to blow up the Blue Moon! It's too beautiful to destroy.";
                }
                else if (npc.type == NPCID.GoblinTinkerer)
                {
                    Main.npcChatText = "My machines work even better under the light of the Blue Moon!";
                }
                else if (npc.type == NPCID.Wizard)
                {
                    Main.npcChatText = "The arcane energies of the Blue Moon enhance my magical abilities.";
                }
                else if (npc.type == NPCID.Stylist)
                {
                    Main.npcChatText = "The Blue Moonlight inspires me to create amazing hairstyles!";
                }
                else if (npc.type == NPCID.Painter)
                {
                    Main.npcChatText = "I should paint a masterpiece capturing the essence of the Blue Moon!";
                }
                else if (npc.type == NPCID.TravellingMerchant)
                {
                    Main.npcChatText = "A night like this is perfect for rare merchandise.";
                }
                else if (npc.type == NPCID.TaxCollector)
                {
                    Main.npcChatText = "Even the tax collector can enjoy a peaceful night under the Blue Moon.";
                }
                else if (npc.type == NPCID.Angler)
                {
                    Main.npcChatText = "Fishing during the Blue Moon? What a catch!";
                }
                else if (npc.type == NPCID.Cyborg)
                {
                    Main.npcChatText = "The Blue Moon reminds me of the neon lights in my circuits.";
                }
                else if (npc.type == NPCID.ArmsDealer)
                {
                    Main.npcChatText = "A night like this makes me appreciate a well-maintained firearm.";
                }
                else
                {
                    Main.npcChatText = "Such a peaceful night with the Blue Moon, right?";
                }
            }
        }
    }
}
*/
