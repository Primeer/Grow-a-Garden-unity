using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem.Plants
{
    [Serializable]
    public struct PlantsSaveData
    {
        public List<PlantSaveData> Plants;
        public long SaveTimeTicks;
    }

    [Serializable]
    public struct PlantSaveData
    {
        public int PlantId;
        public TransformData TransformData;
        [Obsolete]
        public int Step;
        public float Timer;
        public PlantChildSaveData[] Children;
    }

    [Serializable]
    public struct PlantChildSaveData
    {
        public bool HasChild;
        public float TimeToSpawn;
        public TransformData TransformData;
        [Obsolete]
        public int Step;
        public float Timer;
    }

    [Serializable]
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static TransformData Create(Transform transform)
        {
            return new TransformData
            {
                Position = transform.localPosition,
                Rotation = transform.localRotation,
                Scale = transform.lossyScale
            };
        }
    }
}