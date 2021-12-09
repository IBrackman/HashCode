using System;

namespace HashCode
{
    // Возможные состояния ячейки
    internal enum CellState
    {
        Empty,
        Deleted,
        Busy
    }

    /// <summary>
    /// This class implements hash table using methods: table creating, element inserting, removing and searching.
    /// </summary>
    /// <typeparam name="T">Should be comparable.</typeparam>
    public class HashTable<T> where T : IComparable<T>
    {
        // Коэффициент загруженности
        private readonly double loadFactor;

        // Таблица значений
        private T[] table;

        // Таблица состояний
        private CellState[] cellStatesList;

        // Коэффициент расширения таблицы при рехеширование
        private readonly double spreadingFactorTable;

        // Предельное количество элементов в таблице, при котором происходит рехеширование
        private int rehashLimit;

        // Количество элементов в таблице
        public int Size { get; private set; }

        /// <summary>
        /// this method creating hash table.
        /// </summary>
        /// <param name="loadFactor">Table load factor. Should belong to the range from 0.5 to 1.</param>
        /// <param name="capacity">Table capacity.</param>
        /// <param name="spreadingFactorTable">Table expansion ratio for rehashing. Should be more than 1.</param>
        public HashTable(double loadFactor, int capacity, double spreadingFactorTable)
        {
            this.spreadingFactorTable = spreadingFactorTable;
            this.loadFactor = loadFactor;

            if (loadFactor <= 0.5 || loadFactor >= 1.0)
                throw new Exception("Invalid loadFactor");

            if (spreadingFactorTable <= 1)
                throw new Exception("Invalid spreadingFactorTable");

            table = new T[capacity];
            cellStatesList = new CellState[capacity];

            Size = 0;
            rehashLimit = (int) (this.loadFactor * table.Length);
        }

        /// <summary>
        /// This method inserts <param name="data"/> into table.
        /// </summary>
        public void Insert(T data)
        {
            Insert(data, ref table, ref cellStatesList);
        }

        private void Insert(T data, ref T[] list, ref CellState[] cellStateList)
        {
            var hash = data.GetHashCode();
            var index = hash % list.Length;

            if (Size <= rehashLimit)
            {
                //Если ячейка пустая - вставляем в нее элемент
                if (cellStateList[index] == CellState.Empty)
                {
                    cellStateList[index] = CellState.Busy;
                    list[index] = data;
                }
                // Если ячейка занята, то по методу открытой адерсации вставляем в первую свободную ячейку после нее
                else
                {
                    while (cellStateList[index] == CellState.Busy)
                    {
                        index++;

                        // Переходим в начало списка
                        if (index >= list.Length)
                        {
                            index = 0;
                        }
                    }

                    cellStateList[index] = CellState.Busy;
                    list[index] = data;
                }
            }
            else
            {
                Rehash(data);
            }

            Size++;
        }

        private void Rehash(T data)
        {
            // Создаем новые таблицы
            var newTable = new T[(int) (spreadingFactorTable * table.Length)];
            var newStates = new CellState[(int) (spreadingFactorTable * table.Length)];
            Size = 0;
            rehashLimit = (int) (loadFactor * newTable.Length);

            // Последовательно вставляем в нее элементы тем, же алгоритмом
            for (var i = 0; i < table.Length; ++i)
            {
                if (cellStatesList[i] == CellState.Busy)
                    Insert(table[i], ref newTable, ref newStates);
            }

            Insert(data, ref newTable, ref newStates);
            Size--;

            table = newTable;
            cellStatesList = newStates;
        }

        /// <summary>
        /// This method removes <param name="data"/> from table.
        /// </summary>
        public void Remove(T data)
        {
            var hash = data.GetHashCode();
            var index = (hash % table.Length);


            if (cellStatesList[index] == CellState.Busy)
            {
                // Если эта ячейка есть этот же элемент - удаляем
                if (data.CompareTo(table[index]) == 0)
                {
                    cellStatesList[index] = CellState.Deleted;
                    table[index] = default(T);
                }
                else
                {
                    // Находим этот элемент сразу после занятой ячейки
                    while (data.CompareTo(table[index]) != 0)
                    {
                        index++;

                        if (index >= table.Length)
                        {
                            index = 0;
                        }
                    }

                    // Помечаем как удаленное
                    cellStatesList[index] = CellState.Deleted;
                }
            }

            Size--;
        }

        /// <summary>
        /// This method searching element with index <param name="key"/> in table.
        /// </summary>
        /// <returns>Element with index <param name="key"/>.</returns>
        public T Search(T key)
        {
            var hash = key.GetHashCode();
            var index = (hash % table.Length);

            if (cellStatesList[index] != CellState.Empty)
            {
                if (key.CompareTo(table[index]) == 0)
                {
                    if (cellStatesList[index] == CellState.Busy)
                        return table[index];
                }
                else
                {
                    // Ищем среди удаленных и занятых
                    while (key.CompareTo(table[index]) != 0 && cellStatesList[index] != CellState.Empty)
                    {
                        index++;

                        if (index >= table.Length)
                        {
                            index = 0;
                        }
                    }

                    if (cellStatesList[index] == CellState.Busy)
                        return table[index];
                }
            }

            // Не нашел элемент
            throw new Exception("Nothing");
        }
    }
}