using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace BindInContainer;

internal static class ContainerBinding
{
    private static readonly string[] Slots = ["SecuredContainer", "TacticalVest", "Pockets"];

    public static bool CanBind(InventoryController controller, Item item)
    {
        if (!Plugin.Enabled.Value)
        {
            return false;
        }

        if (item is not (Meds or FoodDrink or ThrowWeap or Weapon or Compass or PortableRangeFinder or RadioTransmitter)
            && item.GetItemComponent<KnifeComponent>() == null)
        {
            return false;
        }

        if (item is CompoundItem compound && compound.MissingVitalParts.Any())
        {
            return false;
        }

        var address = item.CurrentAddress;
        while (address != null && address is not OwnerItself)
        {
            var parent = address.Container.ParentItem;
            if (parent == controller.Inventory.Equipment)
            {
                return address.Container is Slot slot
                    && slot.ContainedItem != item
                    && Slots.Contains(slot.ID)
                    && controller.Examined(item);
            }

            address = parent.CurrentAddress;
        }

        return false;
    }
}

internal class BindablePlacePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.Method(typeof(InventoryController), nameof(InventoryController.IsAtBindablePlace));

    [PatchPostfix, HarmonyPriority(Priority.Last)]
    private static void Postfix(InventoryController __instance, Item item, ref bool __result)
    {
        __result = __result || ContainerBinding.CanBind(__instance, item);
    }
}

internal class ReachablePlacePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.Method(typeof(InventoryController), nameof(InventoryController.IsAtReachablePlace));

    [PatchPostfix, HarmonyPriority(Priority.Last)]
    private static void Postfix(InventoryController __instance, Item item, ref bool __result)
    {
        __result = __result || ContainerBinding.CanBind(__instance, item);
    }
}
