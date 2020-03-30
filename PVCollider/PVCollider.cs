using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Harmony;
using HarmonyLib;
using BepInEx.Configuration;



namespace PVCollider
{

    [BepInPlugin("roy12.pvcollider", "PV Collider", "1.0.0.0")]
    public class PVCollider : BaseUnityPlugin
    {
        private static ConfigEntry<int> _dbdirection;
        private static ConfigEntry<float> _dbcenter_x;
        private static ConfigEntry<float> _dbcenter_y;
        private static ConfigEntry<float> _dbcenter_z;
        private static ConfigEntry<int> _dbbound;
        private static ConfigEntry<float> _dbRadius;
        private static ConfigEntry<float> _dbHeight;
        private static DynamicBoneColliderBase.Direction _realDBDirection;
        private static DynamicBoneColliderBase.Bound _realDBBound;

        private static ConfigEntry<int> _dbshaftdirection;
        private static ConfigEntry<float> _dbshaftcenter_x;
        private static ConfigEntry<float> _dbshaftcenter_y;
        private static ConfigEntry<float> _dbshaftcenter_z;
        private static ConfigEntry<int> _dbshaftbound;
        private static ConfigEntry<float> _dbshaftRadius;
        private static ConfigEntry<float> _dbshaftHeight;
        private static DynamicBoneColliderBase.Direction _realDBShaftDirection;
        private static DynamicBoneColliderBase.Bound _realDBShaftBound;
        private static ConfigEntry<bool> _usePenisOffset;

        public static ConfigEntry<float> _lookAtTargetOffset_x;
        public static ConfigEntry<float> _lookAtTargetOffset_y;
        public static ConfigEntry<float> _lookAtTargetOffset_z;
        

        public static DynamicBoneCollider dbc;
        public static DynamicBoneCollider dbcshaft;
        public static bool inHScene;
        public static string poseName;
        public static List<String> excludedPoses;
        public static AIChara.ChaControl[] fem_list;
        public static AIChara.ChaControl[] male_list;
        public static List<DynamicBone> vagBones = new List<DynamicBone>();
        public static Transform newLookAtTarget;
        public static AIChara.ChaControl lookAtFemale;
        private static Vector3 PenisOffset;
 

        private void Awake()
        {
            _dbdirection = Config.Bind<int>("Head", "Direction of the penis tip collider", 2, "Set the penis collider's direction, don't change if not sure");
            _dbcenter_x = Config.Bind<float>("Head", "X center of the penis tip collider", 0, "Set the penis collider's X center, don't change if not sure");
            _dbcenter_y = Config.Bind<float>("Head", "Y center of the penis tip collider", 0, "Set the penis collider's Y center, don't change if not sure");
            _dbcenter_z = Config.Bind<float>("Head", "Z center of the penis tip collider", -0.91f, "Set the penis collider's Z center, change depending on uncensor");
            _dbbound = Config.Bind<int>("Head", "Bounds of the penis tip collider", 0, "Set the penis collider's bounds, don't change if not sure");
            _dbRadius = Config.Bind<float>("Head", "Radius of the penis tip collider", 0.20f, "Set the penis collider's radius, change depending on uncensor");
            _dbHeight = Config.Bind<float>("Head", "Height of the penis tip collider", 0.66f, "Set the penis collider's height, change depending on uncensor");

            _dbshaftdirection = Config.Bind<int>("Shaft", "Direction of the penis shaft collider", 2, "Set the penis collider's direction, don't change if not sure");
            _dbshaftcenter_x = Config.Bind<float>("Shaft", "X center of the penis shaft collider", 0, "Set the penis collider's X center, don't change if not sure");
            _dbshaftcenter_y = Config.Bind<float>("Shaft", "Y center of the penis shaft collider", 0, "Set the penis collider's Y center, don't change if not sure");
            _dbshaftcenter_z = Config.Bind<float>("Shaft", "Z center of the penis shaft collider", 0.91f, "Set the penis collider's Z center, change depending on uncensor");
            _dbshaftbound = Config.Bind<int>("Shaft", "Bounds of the penis shaft collider", 0, "Set the penis collider's bounds, don't change if not sure");
            _dbshaftRadius = Config.Bind<float>("Shaft", "Radius of the penis shaft collider", 0.20f, "Set the penis collider's radius, change depending on uncensor");
            _dbshaftHeight = Config.Bind<float>("Shaft", "Height of the penis shaft collider", 2.2f, "Set the penis collider's height, change depending on uncensor");

            _lookAtTargetOffset_x = Config.Bind("Retarget", "Offset in X for penis look at", 0.0f, "Set x offset for the penis look at bone");
            _lookAtTargetOffset_y = Config.Bind("Retarget", "Offset in Y for penis look at", 0.0f, "Set y offset for the penis look at bone");
            _lookAtTargetOffset_z = Config.Bind("Retarget", "Offset in Z for penis look at", 0.0f, "Set z offset for the penis look at bone");

            _usePenisOffset = Config.Bind("Retarget", "Toggle for penis offset", false, "Broken right now");

            _realDBDirection = (DynamicBoneColliderBase.Direction)_dbdirection.Value;
            _realDBBound = (DynamicBoneColliderBase.Bound)_dbbound.Value;

            _realDBShaftDirection = (DynamicBoneColliderBase.Direction)_dbshaftdirection.Value;
            _realDBShaftBound = (DynamicBoneColliderBase.Bound)_dbshaftbound.Value;

            _dbdirection.SettingChanged += delegate
            {
                if (inHScene)
                {
                    _realDBDirection = (DynamicBoneColliderBase.Direction)_dbdirection.Value;
                    dbc.m_Direction = _realDBDirection;
                }
            };

            _dbcenter_x.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbc.m_Center.x = _dbcenter_x.Value;
                }
            };

            _dbcenter_y.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbc.m_Center.y = _dbcenter_y.Value;
                }
            };

            _dbcenter_z.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbc.m_Center.z = _dbcenter_z.Value;
                }
            };

            _dbbound.SettingChanged += delegate
            {
                if (inHScene)
                {
                    _realDBBound = (DynamicBoneColliderBase.Bound)_dbbound.Value;
                    dbc.m_Bound = _realDBBound;
                }
            };

            _dbRadius.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbc.m_Radius = _dbRadius.Value;
                }
            };

            _dbHeight.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbc.m_Height = _dbHeight.Value;
                }
            };

            //////////////////////////////

            _dbshaftdirection.SettingChanged += delegate
            {
                if (inHScene)
                {
                    _realDBShaftDirection = (DynamicBoneColliderBase.Direction)_dbshaftdirection.Value;
                    dbcshaft.m_Direction = _realDBShaftDirection;
                }
            };

            _dbshaftcenter_x.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbcshaft.m_Center.x = _dbshaftcenter_x.Value;
                }
            };

            _dbshaftcenter_y.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbcshaft.m_Center.y = _dbshaftcenter_y.Value;
                }
            };

            _dbshaftcenter_z.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbcshaft.m_Center.z = _dbshaftcenter_z.Value;
                }
            };

            _dbshaftbound.SettingChanged += delegate
            {
                if (inHScene)
                {
                    _realDBShaftBound = (DynamicBoneColliderBase.Bound)_dbshaftbound.Value;
                    dbcshaft.m_Bound = _realDBShaftBound;
                }
            };

            _dbshaftRadius.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbcshaft.m_Radius = _dbshaftRadius.Value;
                }
            };

            _dbshaftHeight.SettingChanged += delegate
            {
                if (inHScene)
                {
                    dbcshaft.m_Height = _dbshaftHeight.Value;
                }
            };




            var harmony = new Harmony("AI_PVCollider");
            //var type_1 = typeof(Manager.Resources.HSceneTables).GetNestedTypes(BindingFlags.NonPublic).Single(x => x.Name.StartsWith("<LoadDankonList>"));
            //var method_1 = type_1.GetMethod("MoveNext");
            //var postfix_1 = new HarmonyMethod(typeof(PVCollider), nameof(ReplaceKokan));
            //harmony.Patch(method_1, null, postfix_1);

            //var type_2 = typeof(HScene).GetNestedTypes(BindingFlags.NonPublic).Single(x => x.Name.StartsWith("<ChangeAnimation>"));
            //var method_2 = type_2.GetMethod("MoveNext");
            //var postfix_2 = new HarmonyMethod(typeof(PVCollider), nameof(LateReplaceKokan));
            //harmony.Patch(method_2, null, postfix_2);

            HarmonyWrapper.PatchAll(typeof(PVCollider), harmony);
        }


    

        [HarmonyPostfix, HarmonyPatch(typeof(H_Lookat_dan), "setInfo")]
        private static void LateReplaceKokan(H_Lookat_dan __instance, System.Text.StringBuilder ___assetName)
        {
            if (__instance.transLookAtNull == null)
                return;

            if (___assetName.Length == 0)
                return;


            lookAtFemale = __instance.transLookAtNull.transform.GetComponentInParent<AIChara.ChaControl>();
            if (lookAtFemale == null)
            {
                Console.WriteLine("Female is null");
            }
            else
            {
                newLookAtTarget = lookAtFemale.GetComponentsInChildren<Transform>().Where(x => x.name.Contains("cervix")).FirstOrDefault();
                if(newLookAtTarget != null)
                    Console.WriteLine("newLookAtTarget is " + newLookAtTarget.name);
            }
            if (newLookAtTarget == null)
            {
                Console.WriteLine("Previous is null");
            }

            var fOpen = BepInEx.Paths.PluginPath + "/PVCollider.Exclusions.txt";
            excludedPoses = System.IO.File.ReadAllLines(fOpen).ToList();
            Console.WriteLine("List of excluded poses to retarget penis at:");
            if (excludedPoses != null)
            {
                foreach (var item in excludedPoses)
                    Console.WriteLine(item);
            }
            else
            {
                Console.Write(fOpen + " not found");
            }

            poseName = ___assetName.ToString();
            Transform lookatTarget = __instance.transLookAtNull;
            Console.WriteLine(lookatTarget.name);
            if (excludedPoses.Contains(poseName))
            {
                Console.WriteLine("Pose " + poseName + " found in exclusion list, aborting");
            }
            else
            {
                if (lookatTarget.name.Contains("k_f_kokan_00"))
                {
                    Console.WriteLine("Target set to " + lookatTarget.name + ". Changing...");
                    if (lookAtFemale != null)
                    {

                        if (newLookAtTarget != null)
                        {
                            __instance.transLookAtNull = newLookAtTarget;
                            __instance.dan_Info.SetTargetTransform(newLookAtTarget);
                            Console.WriteLine("New target is " + __instance.transLookAtNull.name);
                        }
                        else
                        {
                            Console.WriteLine("Could not find any additional vagina target, leaving as is");
                        }
                    }
                }
                if (lookatTarget != null)
                {
                    PenisOffset = new Vector3(lookatTarget.position.x + _lookAtTargetOffset_x.Value, lookatTarget.position.y + _lookAtTargetOffset_y.Value, lookatTarget.position.z + _lookAtTargetOffset_z.Value);
                }

            }
        }


        [HarmonyPostfix, HarmonyPatch(typeof(H_Lookat_dan), "LateUpdate")]
        public static void OffsetPenisTarget(H_Lookat_dan __instance)
        {
            if(_usePenisOffset.Value)
            { 
                if (__instance.transLookAtNull != null)
                {                 
                __instance.transLookAtNull.position = PenisOffset;
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void AddPColliders(HScene __instance)
        {
            inHScene = true;

            male_list = __instance.GetMales().Where(male => male != null).ToArray();
            fem_list = __instance.GetFemales().Where(female => female != null).ToArray();
            

            Console.WriteLine("ItJustWerks(tm)");
            
            foreach (var male in male_list.Where(male => male != null))
            {
                foreach (var penisshaft in male.GetComponentsInChildren<Transform>().Where(shaft=>shaft.name.Contains("cm_J_dan101_00")))
                {
                     dbcshaft = penisshaft.GetComponent<DynamicBoneCollider>();                    

                     if (dbcshaft == null)
                     {
                         Console.WriteLine("No collider found in penis shaft, adding");
                         dbcshaft = penisshaft.gameObject.AddComponent(typeof(DynamicBoneCollider)) as DynamicBoneCollider;
                         Console.WriteLine("Collider added to penis shaft");
                     }
                     else
                     {
                         Console.WriteLine("Collider exists at penis shaft");
                     }
                     //dbc.m_Direction = DynamicBoneColliderBase.Direction.Z;
                     dbcshaft.m_Direction = _realDBShaftDirection;
                     //dbc.m_Center = new Vector3(0, 0, -0.91f);
                     dbcshaft.m_Center = new Vector3(_dbshaftcenter_x.Value, _dbshaftcenter_y.Value, _dbshaftcenter_z.Value);
                     //dbc.m_Bound = DynamicBoneColliderBase.Bound.Outside;
                     dbcshaft.m_Bound = _realDBShaftBound;
                     //dbc.m_Radius = 0.20f;
                     dbcshaft.m_Radius = _dbshaftRadius.Value;
                     //dbc.m_Height = 2.2f;
                     dbcshaft.m_Height = _dbshaftHeight.Value;
                     foreach (var penistip in penisshaft.GetComponentsInChildren<Transform>().Where(x => x.name.Contains("cm_J_dan109_00")))
                     {                        
                         dbc = penistip.GetComponent<DynamicBoneCollider>();
                         Console.WriteLine("Checking to see if the penis tip has a collider");
                         if (dbc == null)
                         {
                             Console.WriteLine("No collider found in penis head, adding");
                             dbc = penistip.gameObject.AddComponent(typeof(DynamicBoneCollider)) as DynamicBoneCollider;
                             Console.WriteLine("Collider added to penis head");
                         }
                         else
                         {
                             Console.WriteLine("Collider exists at penis head");
                         }
                         //dbc.m_Direction = DynamicBoneColliderBase.Direction.Z;
                         dbc.m_Direction = _realDBDirection;
                         //dbc.m_Center = new Vector3(0, 0, -0.91f);
                         dbc.m_Center = new Vector3(_dbcenter_x.Value, _dbcenter_y.Value, _dbcenter_z.Value);
                         //dbc.m_Bound = DynamicBoneColliderBase.Bound.Outside;
                         dbc.m_Bound = _realDBBound;
                         //dbc.m_Radius = 0.20f;
                         dbc.m_Radius = _dbRadius.Value;
                         //dbc.m_Height = 2.2f;
                         dbc.m_Height = _dbHeight.Value;
                     }
                 }
                
            }                
            foreach(var female in fem_list.Where(female => female != null))
            {
                 Console.WriteLine("Checking for additional vagina dynamic bones");
                 foreach (DynamicBone db in female.GetComponentsInChildren<DynamicBone>().Where(x => x.m_Root.name.Contains("cf_J_Vagina")))
                 {
                      if (db != null)
                      {
                         Console.WriteLine("Vagina bones found, adding penis colliders to collider lists.");
                        //foreach (var male in __instance.GetMales().Where(male => male != null))
                        //{
                        //foreach (var dbc in male.GetComponentsInChildren<DynamicBoneCollider>().Where(x => x.name.Contains("cm_J_dan109_00")))
                        //{
                        vagBones.Add(db);
                        if (db.m_Colliders.Contains(dbcshaft))
                        {
                            Console.WriteLine("Instance of " + dbcshaft.name + " already exists in list for DB " + db.m_Root.name);                            
                        }
                        else
                        { 
                            db.m_Colliders.Add(dbcshaft);
                            Console.WriteLine(dbcshaft.name + " added to " + female.name + " for bone " + db.m_Root.name);
                        }
                        if (db.m_Colliders.Contains(dbc))
                        {
                            Console.WriteLine("Instance of " + dbc.name + " already exists in list for DB " + db.m_Root.name);                                
                        }
                        else
                        { 
                            db.m_Colliders.Add(dbc);
                            Console.WriteLine(dbc.name + " added to " + female.name + " for bone " + db.m_Root.name);
                        }
                             //}
                         //}
                      }
                      else
                      {
                          Console.WriteLine("No vagina dynamic bones found.");
                      }
                 }
            }
            
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HScene), "EndProc")]
        public static void HScene_EndProc_Patch()
        {
            inHScene = false;
            if(!inHScene)
            { 
                if(vagBones.Any())
                { 
                    foreach (DynamicBone vagBone in vagBones)
                    {
                        if (vagBone != null)
                        { 
                            Console.WriteLine("Clearing colliders from " + vagBone.m_Root.name);
                            vagBone.m_Colliders.Clear();
                        }
                    }
                    Console.WriteLine("Destroying collider " + dbc.name);
                    Destroy(dbc);
                    Console.WriteLine("Destroying collider " + dbcshaft.name);
                    Destroy(dbcshaft);
                    Console.WriteLine("Clearing females list");
                    Array.Clear(fem_list,0,fem_list.Length);
                    Console.WriteLine("Clearing males list");
                    Array.Clear(male_list, 0, male_list.Length);
                    Console.WriteLine("Clearing excluded poses list");
                    excludedPoses.Clear();
                    poseName = "";
                }
            }
        }

        private void OnGUI()
        {
            if (inHScene & _usePenisOffset.Value)
                UI.DrawOffsetsUI();
        }
    }
}