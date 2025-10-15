using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Panik;

[BepInPlugin("com.pharmacomaniac.startlook", "Start Look Mod", "1.0.0")]
public class StartLookPlugin : BaseUnityPlugin
{
    private static bool _appliedThisRun;

    private void Awake()
    {
        new Harmony("com.pharmacomaniac.startlook").PatchAll();
        SceneManager.sceneLoaded += (_, __) => _appliedThisRun = false;
    }

    // After the intro step snaps the camera to Free and copies its rotation,
    // replace that cached rotation with our desired one.
    [HarmonyPatch(typeof(GameplayMaster), "IntroPhaseBehaviour")]
    static class Patch_IntroPhaseBehaviour
    {
        static void Postfix()
        {
            if (_appliedThisRun) return;

            Vector3 desiredEuler = new Vector3(0.3f, 180f, 0f); // pitch,yaw,roll in degrees
            var m = AccessTools.Method(typeof(CameraController), "SetFreeCameraRotation", new[] { typeof(Vector3) });
            if (m != null)
                m.Invoke(null, new object[] { desiredEuler });
            else
                CameraController.SetFreeCameraRotation(desiredEuler);

            _appliedThisRun = true;
        }
    }
}
