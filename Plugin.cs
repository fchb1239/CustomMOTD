using System;
using System.IO;
using System.Reflection;

using HarmonyLib;
using BepInEx;

using UnityEngine;

namespace CustomMOTD
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    class Plugin : BaseUnityPlugin
    {
        private string message = "";
        private bool once = false;
        private bool setYellow = false;
        private Material[] motdMat;

        private void Awake()
        {
            if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MessageOfTheDay.txt"))
            {
                Console.WriteLine("Custom MOTD - Found message of the day file");
                message = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MessageOfTheDay.txt");
            }
            else
            {
                Console.WriteLine("Custom MOTD - No message of the day file found - creating a new one.");
                message = $"PLEASE GO TO {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\MessageOfTheDay.txt TO CHANGE THE MESSAGE";
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MessageOfTheDay.txt", message);
                Console.WriteLine("Custom MOTD - Generated file");
            }

            if(File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\IsNewMOTD.txt"))
            {
                try
                {
                    setYellow = bool.Parse(File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\IsNewMOTD.txt"));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Custom MOTD - Failed to convert string to bool:\n " + e);
                    setYellow = false;
                    File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\IsNewMOTD.txt", "false");
                    Console.WriteLine("Custom MOTD - Just generated a new one dw about it");
                }
            }
            else
            {
                Console.WriteLine("Custom MOTD - Couldn't see if it should be yellow or not. Creating the file!!");
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\IsNewMOTD.txt", "true");
                setYellow = true;
            }
        }

        //I can't be bothered to do this after the message has been loaded lmao - it's not like this is gonna cause lag xD
        private void Update()
        {
            //This is so it's green instead of yellow :o
            if(GorillaNetworking.GorillaComputer.instance.motdRenderer.materials != null && !once)
            {
                if(!setYellow)
                    motdMat = GorillaNetworking.GorillaComputer.instance.motdRenderer.materials;
                else
                {
                    Material[] mat = GorillaNetworking.GorillaComputer.instance.motdRenderer.materials;
                    mat[0] = GorillaNetworking.GorillaComputer.instance.updatedMOTDMaterial;
                    motdMat = mat;
                }
                once = true;
            }

            GorillaNetworking.GorillaComputer.instance.motdText.text = message;
            GorillaNetworking.GorillaComputer.instance.motdRenderer.materials = motdMat;
        }
    }
}
