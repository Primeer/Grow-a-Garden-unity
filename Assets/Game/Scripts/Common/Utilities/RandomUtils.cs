using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Game.Scripts.Common.Utilities
{
    public static class RandomUtils
    {
        public static readonly Random Random = new Random();
        
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Next(0, array.Length)];
        }
        
        public static T[] GetRandomUniqList<T>(this T[] array, int count)
        {
            if (array.Length < count)
                throw new Exception();
            
            HashSet<T> selectedItems = new();
            var result = new T[count];

            for (var i = 0; i < count; i++)
            {
                var item = array.GetRandom();

                if (selectedItems.Add(item) == false)
                {
                    i--;
                    continue;
                }

                result[i] = item;
            }
            
            return result;
        }

        public static Vector3 NormalScale()
        {
            return Vector3.one * (float)Normal(1.1, 0.3);
        }
        
        public static float RandomAngle360()
        {
            return (float)(Random.NextDouble() * 360f);
        }

        public static Quaternion RandomYRotation()
        {
            return Quaternion.AngleAxis(RandomAngle360(), Vector3.up);
        }

        public static Quaternion RandomYRotation(Quaternion baseRotation)
        {
            return Quaternion.AngleAxis(RandomAngle360(), Vector3.up) * baseRotation;
        }
        
        public static double Normal(double mean, double stdDev = 1)
        {
            double u1 = 1.0 - Random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - Random.NextDouble();
            
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}
