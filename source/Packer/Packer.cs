using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packer
{
    public class Packer
    {
        public static string Pack(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            StringBuilder sb = new StringBuilder();

            foreach (var line in lines)
            { 
                string[] p = line.Split(" : ");
                decimal maxWeight = Decimal.Parse(p[0]);
                if (maxWeight > 100)
                    throw new APIException($"Package weight should not be greater than 100. Given package weight: {maxWeight}");

                List<PackageItem> items = ExtractEligiblePackageItems(p[1], maxWeight);
                
                sb.AppendLine(FindItems(items, maxWeight));
            }

            return sb.ToString();
        }

        //Splits the input string into package items
        //Excludes items over the weight limit, to avoid unecessary processing later
        private static List<PackageItem> ExtractEligiblePackageItems(string line, decimal maxWeight)
        {
            List<PackageItem> result = new List<PackageItem>();
            string[] items = line.Trim().TrimStart('(').TrimEnd(')').Split(") (");
            if (items.Length > 15)
                throw new APIException($"Number of items to sort should not exceed 15. Given items: {items.Length}");

            foreach (var item in items)
            {
                string[] i = item.Replace("\u20AC", "").Split(',');
                PackageItem pi = new PackageItem();
                pi.index = Int32.Parse(i[0]);
                pi.weight = Decimal.Parse(i[1]);
                pi.cost = Decimal.Parse(i[2]);

                if (pi.cost > 100 || pi.weight > 100)
                    throw new APIException($"Items should not weigh or cost more than 100");

                if (pi.weight <= maxWeight)
                {
                    result.Add(pi);
                }
            }

            return result;
        }

        //Finds combinations of items that are within weight limit
        //And returns the most expensive combination
        private static string FindItems(List<PackageItem> items, decimal maxWeight)
        {
            string result = "-";

            //Used to store combination details. Cost, weight, indices
            List<Tuple<decimal, decimal, string>> combos = new List<Tuple<decimal, decimal, string>>();

            //Sort the items by descending cost, to process higher priced items first.
            //To reduce the number of processing iterations
            items.Sort((x, y) => y.cost.CompareTo(x.cost));

            int step = 0;
            while (step < items.Count)
            {
                List<PackageItem> combo = new List<PackageItem>();
                combo.Add(items[step]);

                decimal currentWeight = items[step].weight;
                decimal cost = items[step].cost;

                int i = step + 1;
                while (i < items.Count)
                {
                    bool add = CheckNextItem(ref currentWeight, maxWeight, items[i]);
                    if (add)
                    {
                        cost += items[i].cost;
                        combo.Add(items[i]);
                    }

                    i++;
                }

                step++;

                string indices = String.Join(",", combo.Select(x => x.index).OrderBy(x => x).ToArray());

                combos.Add(new Tuple<decimal, decimal, string>(cost, currentWeight, indices));
            }

            if (combos.Count > 0)
            {
                result = combos.OrderByDescending(x => x.Item1).ThenBy(x => x.Item2).FirstOrDefault().Item3;
            }

            return result;
        }

        private static bool CheckNextItem(ref decimal currentWeight, decimal maxWeight, PackageItem item)
        {
            if ((currentWeight + item.weight) <= maxWeight)
            {
                currentWeight += item.weight;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class PackageItem
    {
        public int index { get; set; }
        public decimal weight { get; set; }
        public decimal cost { get; set; }
    }
}
