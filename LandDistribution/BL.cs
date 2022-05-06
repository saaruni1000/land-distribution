using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandDistribution
{
    static class BL
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        static List<int> knapsackItems(int wantedValue, int[] landSize, int[] landNumbers)
        {
            int i, currentLeft;
            int n = landSize.Length;
            int[,] K = new int[n + 1, wantedValue + 1];
            List<int> chosenLands = new List<int>();

            // Build table K[][] in bottom up manner
            for (i = 0; i <= n; i++)
            {
                for (currentLeft = 0; currentLeft <= wantedValue; currentLeft++)
                {
                    if (i == 0 || currentLeft == 0)
                        K[i, currentLeft] = 0;
                    else if (landSize[i - 1] <= currentLeft)
                        K[i, currentLeft] = Math.Max(landSize[i - 1] +
                                K[i - 1, currentLeft - landSize[i - 1]], K[i - 1, currentLeft]);
                    else
                        K[i, currentLeft] = K[i - 1, currentLeft];
                }
            }

            // stores the result of Knapsack
            int res = K[n, wantedValue];
            Console.WriteLine(res);

            currentLeft = wantedValue;
            for (i = n; i > 0 && res > 0; i--)
            {

                // either the result comes from the top
                // (K[i-1][w]) or from (val[i-1] + K[i-1]
                // [w-wt[i-1]]) as in Knapsack table. If
                // it comes from the latter one/ it means
                // the item is included.
                if (res == K[i - 1, currentLeft])
                    continue;
                else
                {

                    // This item is included.
                    //Console.Write(landSize[i - 1] + " ");
                    chosenLands.Add(landNumbers[i - 1]);

                    // Since this weight is included its
                    // value is deducted
                    res = res - landSize[i - 1];
                    currentLeft = currentLeft - landSize[i - 1];
                }
            }

            return chosenLands;
        }
        public static SortedDictionary<int, List<int>> CalculateLands(Dictionary<int, int> groups, Dictionary<int, int> lands, int variance)
        {
            List<int> participants = groups.Keys.ToList();
            SortedDictionary<int, List<int>> res = new SortedDictionary<int, List<int>>();
            Dictionary<int, int> currentLands = new Dictionary<int,int>(lands);
            participants.Shuffle();
            foreach (int participant in participants)
            {
                List<int> currentChosen = knapsackItems(groups[participant]+variance, currentLands.Values.ToArray(), currentLands.Keys.ToArray());
                res[participant] = currentChosen;
                foreach (int chosenLand in currentChosen)
                {
                    currentLands.Remove(chosenLand);
                }
            }
            return res;
        }
    }
}
