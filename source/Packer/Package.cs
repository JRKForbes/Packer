using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packer
{
    public class Package
    {
        #region Properties

        private string _rawInput;

        private SortedList<decimal, PackageItem> _items;
        private decimal _maxWeight;

        private Comparer<decimal> _descComparer;

        #endregion

        public Package(string rawInput)
        {
            _descComparer = Comparer<decimal>.Create((x, y) => y.CompareTo(x));

            this._rawInput = rawInput;
            _items = new SortedList<decimal, PackageItem>(_descComparer);

            ExtractDetails();
        }

        /// <summary>
        /// Finds combination of items that are within weight limit
        /// </summary>
        /// <returns>indices of most expensive combo</returns>
        public string FindItems()
        {
            string result = "-";

            List<Tuple<decimal, decimal, string>> combos = new List<Tuple<decimal, decimal, string>>();

            int step = 0;
            while (step < _items.Count)
            {
                List<PackageItem> combo = new List<PackageItem>();
                combo.Add(_items[step]);

                decimal comboWeight = _items[step].weight;
                decimal cost = _items[step].cost;

                int i = step + 1;
                while (i < _items.Count)
                {
                    bool add = CheckNextItem(ref comboWeight, _items[i]);
                    if (add)
                    {
                        cost += _items[i].cost;
                        combo.Add(_items[i]);
                    }

                    i++;
                }

                step++;

                string indices = String.Join(",", combo.Select(x => x.index).OrderBy(x => x).ToArray());

                combos.Add(new Tuple<decimal, decimal, string>(cost, comboWeight, indices));
            }

            if (combos.Count > 0)
            {
                result = combos.OrderByDescending(x => x.Item1).ThenBy(x => x.Item2).FirstOrDefault().Item3;
            }            

            return result;
        }


        #region Private Helpers


        /// <summary>
        /// Splits the input string into package items
        /// Excludes items over the weight limit, to avoid unnecessary processing later
        /// </summary>
        private void ExtractDetails()
        {
            string[] split = _rawInput.Split(" : ");
            _maxWeight = Decimal.Parse(split[0]);

            if (_maxWeight > 100)
                throw new APIException($"Package weight should not be greater than 100. Given package weight: {_maxWeight}");


            string[] items = split[1].Trim().TrimStart('(').TrimEnd(')').Split(") (");

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
                    throw new APIException("Items should not weigh or cost more than 100");

                if (pi.weight <= _maxWeight)
                    _items.Add(pi.cost, pi);
            }
        }

        private bool CheckNextItem(ref decimal currentWeight, PackageItem item)
        {
            if ((currentWeight + item.weight) <= _maxWeight)
            {
                currentWeight += item.weight;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
