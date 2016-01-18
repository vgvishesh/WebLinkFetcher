using System.Collections.Generic;

namespace CreateHtmlFakePages
{
    public class UniqueList<T> : List<T>
    {
        private Dictionary<T, bool> _hashTable = new Dictionary<T, bool>();

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                if(!_hashTable.ContainsKey(element))
                {
                    _hashTable.Add(element, true);
                    Add(element);
                }
            }
        }
    }
}