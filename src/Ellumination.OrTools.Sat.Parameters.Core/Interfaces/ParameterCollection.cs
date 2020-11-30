﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;
    using IEnumerableParameters = IEnumerable<IParameter>;
    using IParameterCollectionType = ICollection<IParameter>;

    /// <inheritdoc />
    public class ParameterCollection : IParameterCollection
    {
        /// <summary>
        /// Gets the Collection for Private use.
        /// </summary>
        private IParameterCollectionType Collection { get; }

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <inheritdoc />
        public ParameterCollection() : this(new List<IParameter> { })
        {
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <paramref name="parameters"/>
        /// <inheritdoc />
        public ParameterCollection(IEnumerableParameters parameters) : this(parameters.ToList())
        {
        }

        /// <summary>
        /// Public Constructor.
        /// </summary>
        /// <param name="parameters"></param>
        /// <inheritdoc />
        public ParameterCollection(IParameterCollectionType parameters)
        {
            Collection = parameters;
        }

        private void CollectionAction(Action<IParameterCollectionType> action) => action.Invoke(Collection);

        private TResult CollectionFunc<TResult>(Func<IParameterCollectionType, TResult> func) => func.Invoke(Collection);

        public IEnumerator<IParameter> GetEnumerator() => CollectionFunc(x => x.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IParameter item) => CollectionAction(x => x.Add(item));

        public void Clear() => CollectionAction(x => x.Clear());

        public bool Contains(IParameter item) => CollectionFunc(x => x.Contains(item));

        public void CopyTo(IParameter[] array, int arrayIndex) => CollectionAction(x => x.CopyTo(array, arrayIndex));

        public bool Remove(IParameter item) => CollectionFunc(x => x.Remove(item));

        public int Count => CollectionFunc(x => x.Count);

        public bool IsReadOnly => CollectionFunc(x => x.IsReadOnly);

        /// <inheritdoc />
        public override string ToString() => ToString(ParameterValueRenderingOptions.DefaultOptions);

        /// <inheritdoc />
        /// <see cref="SemiColon"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        public string ToString(IParameterValueRenderingOptions options)
            => Collection.Any()
                ? Join($"{SemiColon} ", this.Select(x => x.ToString(options)))
                : "";
    }
}
