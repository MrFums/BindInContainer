using BepInEx;
using BepInEx.Configuration;

namespace BindInContainer;

[BepInPlugin("com.fums.bindincontainer", "Bind in Container", "1.0.0")]
[BepInDependency("com.SPT.core")]
public class Plugin : BaseUnityPlugin
{
    internal static ConfigEntry<bool> Enabled;

    private void Awake()
    {
        Enabled = Config.Bind("General", "Enabled", true, "Allow binding items kept in the secure container, or in a pouch inside the secure container, rig or pockets.");

        new BindablePlacePatch().Enable();
        new ReachablePlacePatch().Enable();
    }
}
