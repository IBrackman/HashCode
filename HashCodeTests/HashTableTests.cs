using System;
using HashCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashCodeTests
{
    [TestClass]
    public class HashTableTests
    {
        public HashTable<int> HashTable;

        public void Insert(params int[] integers)
        {
            foreach (var val in integers)
                HashTable.Insert(val);
        }

        public void Remove(params int[] integers)
        {
            foreach (var val in integers)
            {
                HashTable.Remove(val);
            }
        }

        public bool Contains(params int[] integers)
        {
            Exception exception = null;

            foreach (var val in integers)
                try
                {
                    HashTable.Search(val);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

            return exception == null;
        }

        [TestInitialize]
        public void Initialize()
        {
            const double multiplier = 2.0;
            const double loadFactor = 0.6;
            const int capacity = 10;
            HashTable = new HashTable<int>(loadFactor, capacity, multiplier);
        }

        [TestMethod]
        public void HashTableShouldContain2()
        {
            Insert(2);
            Assert.IsTrue(Contains(2));
        }

        [TestMethod]
        public void HashTableShouldContainAllElemsAfterRehashing()
        {
            Insert(1, 3, 4, 5, 6, 7, 8, 9);
            Assert.IsTrue(Contains(1, 3, 4, 5, 6, 7, 8, 9));
        }

        [TestMethod]
        public void HashTableSizeShouldBeEqual2()
        {
            Insert(1, 1);
            Assert.AreEqual(2, HashTable.Size);
        }

        [TestMethod]
        public void HashTableShouldntContain2AndHaveSize9()
        {
            Insert(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            Remove(2);

            Assert.AreEqual(9, HashTable.Size);
            Assert.IsFalse(Contains(2));
        }

        [TestMethod]
        public void HashTableShouldContainAllElemsAndHaveSize10()
        {
            Insert(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            Remove(2);
            Insert(2);

            Assert.AreEqual(10, HashTable.Size);
            Assert.IsTrue(Contains(1, 2, 3, 4, 5, 6, 7, 8, 9, 10));
        }
    }
}