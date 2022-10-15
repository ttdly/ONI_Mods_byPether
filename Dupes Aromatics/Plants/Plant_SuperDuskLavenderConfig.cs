﻿using System;
using TUNING;
using UnityEngine;
using STRINGS;
using System.Collections.Generic;
using Klei.AI;

namespace Dupes_Aromatics.Plants
{
    public class Plant_SuperDuskLavenderConfig : IEntityConfig
    {
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_EXPANSION1_ONLY;
        }

        //===> BASE INFORMATION <=========================================
        public const string Id = "DuskberryLavender";
        public const string Name = "Duskberry Lavender";
        public static string Description = string.Concat(new string[] { "A shrub-like plant sprouts with an edible " + UI.FormatAsLink("Duskberry", "Duskberry") + "." });
        public static string DomesticatedDescription = string.Concat(new string[] { "/n/n In domesticated environment this crop requires the use of " + UI.FormatAsLink("Phosphorite", "PHOSPHORITE") + " as fertilization." });
        public const string SeedId = "LavenderSeed";
        public const string SeedName = "Dusk Seed";
        public static string SeedDescription = "The tiny seed of a " + UI.FormatAsLink("Duskbloom Lavender", "DuskbloomLavender") + ".";

        //===> DEFINE THE ANIMATION SETTINGS FOR A STANDARD CROP PLANT <=
        private static StandardCropPlant.AnimSet animSet = new StandardCropPlant.AnimSet
        {
            grow = "super_grow",
            grow_pst = "super_grow_pst",
            idle_full = "super_idle_full",
            wilt_base = "super_wilt",
            harvest = "super_harvest"
        };

        //===> TEMPERATURE SETTINGS <=====================================
        public const float DefaultTemperature = 299.15f;       //  26°C: Normal Temperature
        public const float TemperatureLethalLow = 258.15f;     // -15ºC: Plant will die (Lowest Temp)
        public const float TemperatureWarningLow = 288.15f;    //  15°C: Plant will stop growing (Lowest Temp)
        public const float TemperatureWarningHigh = 313.15f;   //  40°C: Plant will stop growing (Highest Temp)
        public const float TemperatureLethalHigh = 333.15f;    //  60°C: Plant will die (Highest Temp)

        public const float Fertilization = 0.014f;         // Phosphorite Fertilization Needed

        //public static AromaticsPlantsTuning.CropsTuning tuning;
        public ComplexRecipe Recipe;

        //===> DEFINE THE BASE TEMPLATE <=====================================================================
        public GameObject CreatePrefab()
        {
            GameObject gameObject = Plant_SuperDuskLavenderConfig.BaseWormPlant(
                Id,
                Name,
                Description,
                "plant_lavender_kanim",  // Crop KAnim file.
                TUNING.DECOR.BONUS.TIER1,  // Decor tier the crop produces around it.
                "Duskberry");  // The produce ID of this crop. 

            gameObject.AddOrGet<SeedProducer>().Configure(
                "LavenderSeed",  // It takes the seed definitions from its standard counterpart.
                 SeedProducer.ProductionType.Harvest, // Implies that this Crop will yeild its seed upon harvest.
                 1); // Number of seeds it will produce each time.

            return gameObject;
        }

        //===> ENTITY SETTINGS <===========================================================================
        public static GameObject BaseWormPlant(string id, string name, string desc, string animFile, EffectorValues decor, string cropID)
        {
            GameObject gameObject = EntityTemplates.CreatePlacedEntity(
                Id,
                Name,
                Description,
                1f, // Specify the entity mass in kg.
                Assets.GetAnim("plant_lavender_kanim"),
                "idle_empty",
                Grid.SceneLayer.BuildingBack,  // The layer which this crop will be placed in game.
                1, //Crop width.
                3, //Crop height.
                TUNING.DECOR.BONUS.TIER2,
                default(EffectorValues),
                SimHashes.Creature,
                null,
                DefaultTemperature
                );

            EntityTemplates.ExtendEntityToBasicPlant(
                gameObject,
                TemperatureLethalLow,
                TemperatureWarningLow,
                TemperatureWarningHigh,
                TemperatureLethalHigh,

                //===> SAFE ATMOSPHERE ELEMENTS <===================================================================================
                // Plant will not grow in any element other than the ones here.
                new SimHashes[]{
                SimHashes.Oxygen,
                SimHashes.ContaminatedOxygen,
                SimHashes.CarbonDioxide
                },

                //===> BASE SETTINGS <==============================================================================================
                true, // Implies that this Crop is sensible to Atmospheric Pressure
                0f, // Pressure which this Crop will die
                0.15f, // Pressure which this Crop will stop growing.
                Crop_DuskberryConfig.Id,
                true, // Implies this Crop can be drowned by liquids.
                true, // Implies this Crop can receive Micro Fertilizer buff in the agricultural room.
                true, // Implies this Crop requires a solid ground to grow.
                true, // Implies this Crop will grow old and eventualy yeilds a produce.
                2400f, // Max age this Crop can grow, or the time it require for it to complete its growth.
                0f, // Minium Radiation required by this Crop.
                9800f, // Maxium value of Radiation this Crop can get before stop growing and dying.
                "SuperLavenderOriginal", // Crop trait id.
                "Fruiting Lavender Original"); // Crop trait name.

            //===> SOLID FERTILIZER THIS CROP REQUIRES <============================================================================
            EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
            {
            new PlantElementAbsorber.ConsumeInfo
            {
                tag = SimHashes.Phosphorite.CreateTag(),
                massConsumptionRate = Fertilization
            }
            });

            gameObject.AddOrGet<StandardCropPlant>();
            gameObject.AddOrGet<LoopingSounds>();
            gameObject.AddOrGet<BlightVulnerable>();

            //===> DISEASE OR GERMS THIS CROP RELEASES <===========================================================================
            DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
            def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
            def.singleEmitQuantity = 1000000;

            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
            TransformingPlant transformingPlant = prefab.AddOrGet<TransformingPlant>();
            transformingPlant.SubscribeToTransformEvent(GameHashes.HarvestComplete);
            transformingPlant.transformPlantId = Plant_DuskLavenderConfig.Id;
            prefab.GetComponent<KAnimControllerBase>().SetSymbolVisiblity("flower", false);
            prefab.AddOrGet<StandardCropPlant>().anims = Plant_SuperDuskLavenderConfig.animSet;
        }
        public void OnSpawn(GameObject inst)
        {
        }
    }
}