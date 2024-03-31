using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class AlterFriendAvatars
    {

        private static Texture2D[] s_profileImages;

        private static string s_profilePicsPath => Path.Combine(UT2Paths.TextureFolder, "ProfilePictures");

        public static void Load()
        {
            if(!Directory.Exists(s_profilePicsPath))
            {
                Directory.CreateDirectory(s_profilePicsPath);

                //unpack the built-in profile pictures
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "boi.jpg"), Properties.Resources.boi);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "awdjao.png"), Properties.Resources.awdjao);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "padrick.jpg"), Properties.Resources.padrick);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "spongecry.png"), Properties.Resources.spongecry);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "scientistshavediscovered.png"), Properties.Resources.scientistshavediscovered);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "roachcarving.png"), Properties.Resources.roachcarving);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "pronounssoy.png"), Properties.Resources.pronounssoy);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "plink.png"), Properties.Resources.plink);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "steamhappy.png"), Properties.Resources.steamhappy);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "twodabloons.png"), Properties.Resources.twodabloons);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "monkscream.jpg"), Properties.Resources.monkscream);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "ermcat.png"), Properties.Resources.ermcat);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "dogstare1.jpeg"), Properties.Resources.dogstare1);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "dogstare0.png"), Properties.Resources.dogstare0);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "bigassgloopy.png"), Properties.Resources.bigassgloopy);
                ValidateBuiltInImage(Path.Combine(s_profilePicsPath, "hakitalightning.png"), Properties.Resources.hakitalightning);
            }

            s_profileImages = TextureHelper.FindTextures(s_profilePicsPath);
        }

        private static void ValidateBuiltInImage(string filePath, byte[] data)
        {
            if (File.Exists(filePath))
                return;

            File.WriteAllBytes(filePath, data);
        }

        [Configgable("Patches", "Alter Leaderboard Avatars")]
        private static ConfigToggle s_enabled = new(true);

        [HarmonyPatch(typeof(SteamController), nameof(SteamController.FetchAvatar)), HarmonyPrefix]
        public static bool OnAvatarFetch(SteamController __instance, RawImage target, Friend user)
        {
            if (!s_enabled.Value || s_profileImages == null || s_profileImages.Length <= 0)
                return true;

            string name = user.Name;

            UniRandom rand = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSeed(name).GetSeed());
            target.texture = rand.SelectRandom(s_profileImages);
            target.rectTransform.localScale = new Vector3(1, 1, 1); //The images are negative y scale in the game so we need to flip it back
            return false;
        }
    }
}
