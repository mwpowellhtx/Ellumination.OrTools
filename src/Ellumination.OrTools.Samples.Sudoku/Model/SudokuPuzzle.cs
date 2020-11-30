﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Ellumination.OrTools.Samples.Sudoku
{
    using static Domain;

    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="IDictionary{Address,Int32}" />
    public partial class SudokuPuzzle : IDictionary<Address, int>
    {
        private static InvalidOperationException ThrowNotSupported(string callerName)
            => new InvalidOperationException($"{callerName} is not supported.");

        /// <summary>
        /// Grid backing field.
        /// </summary>
        private readonly IDictionary<Address, int> _grid = new Dictionary<Address, int>();

        /// <summary>
        /// Returns a Default Grid.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<Address, int>> GetDefaultGrid()
        {
            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var column = MinimumValue; column < MaximumValue; column++)
                {
                    yield return new KeyValuePair<Address, int>(new Address(row, column), MinimumValue);
                }
            }
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SudokuPuzzle()
        {
            foreach (var item in GetDefaultGrid())
            {
                _grid.Add(new Address(item.Key), item.Value);
            }
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other"></param>
        public SudokuPuzzle(SudokuPuzzle other)
        {
            _grid = new Dictionary<Address, int>();
            foreach (var item in other._grid)
            {
                _grid.Add(new Address(item.Key), item.Value);
            }
        }

        /// <inheritdoc />
        public void Add(Address key, int value) => throw ThrowNotSupported(nameof(Add));

        /// <inheritdoc />
        public bool ContainsKey(Address key) => _grid.ContainsKey(key);

        /// <inheritdoc />
        public ICollection<Address> Keys => _grid.Keys;

        /// <inheritdoc />
        public bool Remove(Address key) => throw ThrowNotSupported(nameof(Remove));

        /// <inheritdoc />
        public bool TryGetValue(Address key, out int value)
        {
            value = default(int);
            if (!ContainsKey(key))
            {
                return false;
            }

            value = this[key];
            return true;
        }

        /// <inheritdoc />
        public ICollection<int> Values => _grid.Values;

        /// <inheritdoc />
        public int this[Address key]
        {
            get => _grid[key];
            set
            {
                value.VerifyValue();
                _grid[key] = value;
            }
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<Address, int> item) => throw ThrowNotSupported(nameof(Add));

        /// <inheritdoc />
        public void Clear() => throw ThrowNotSupported(nameof(Clear));

        /// <inheritdoc />
        public bool Contains(KeyValuePair<Address, int> item) => _grid.Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<Address, int>[] array, int arrayIndex)
            => _grid.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public int Count => _grid.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _grid.IsReadOnly;

        /// <inheritdoc />
        public bool Remove(KeyValuePair<Address, int> item) => throw ThrowNotSupported(nameof(Remove));

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Address, int>> GetEnumerator() => _grid.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
