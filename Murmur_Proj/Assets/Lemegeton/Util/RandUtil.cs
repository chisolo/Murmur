using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Lemegeton
{
    public static class RandUtil
    {
        public static void InitSeed()
        {
            Random.InitState(unchecked((int)DateTime.Now.Ticks));
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int max)
        {
            return Random.Range(0, max);
        }

        /// <summary>
        /// 生成min <= x < max之间的乱数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// 生成min <= x <= max之间的乱数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// 0<= p <= 1f, 百分比概率
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Percent(float p)
        {
            if (p == 0f) {
                return false;
            }
            var rand = Random.Range(0f, 100f);

            p = p * 100f;

            //AppLogger.Log("rand " + rand);
            //AppLogger.Log("p " + p);
            return p >= rand;
        }
        public static int Pick(List<float> rates)
        {
            float total = 0;
            for (var i = 0; i < rates.Count; i++) total += rates[i];
            if(total <= 0) return -1;
            float r = Range(0, total);
            float t = 0;
            for (var i = 0; i < rates.Count; i++) {
                t += rates[i];
                if (r <= t) return i;
            }
            return 0;
        }

        public static float RandomOne(List<float> list)
        {
            var i = Range(0, list.Count);
            return list[i];
        }
    }
}