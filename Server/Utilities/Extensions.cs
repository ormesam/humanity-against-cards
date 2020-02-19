using System;
using System.Collections.Generic;

namespace Server.Utilities {
    public static class Extensions {
        public static void Shuffle<T>(this IList<T> list) {
            int count = list.Count;
            Random rnd = new Random();

            while (count > 1) {
                count--;
                int randomIdx = rnd.Next(count + 1);

                T value = list[randomIdx];
                list[randomIdx] = list[count];
                list[count] = value;
            }
        }
    }
}
