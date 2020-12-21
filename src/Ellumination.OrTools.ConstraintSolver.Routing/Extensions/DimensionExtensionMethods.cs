using System;
using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using TranslateRoutingModelIndexCallback = Func<RoutingModel, int, long>;

    /// <summary>
    /// Provides a handful of helpful extension methods which operate via
    /// <see cref="RoutingModel"/> instead of <see cref="RoutingContext"/>.
    /// Many of the extension methods assume that the Dimension has been
    /// added to the Model.
    /// </summary>
    public static class DimensionExtensionMethods
    {
        /// <summary>
        /// Sets the Accumulator Variable <em>Lower-</em> and <em>Upper-Bounds</em>
        /// corresponding to the Index <paramref name="i"/> translated according to
        /// <paramref name="onTranslateIndex"/> terms. Assumes that the Dimension has
        /// been added to the Model.
        /// </summary>
        /// <typeparam name="TDimension">A Dimension type.</typeparam>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="lowerBound">The Lower Bounds.</param>
        /// <param name="upperBound">The Upper Bounds.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        public static void SetCumulVarRange<TDimension>(this TDimension dim, int i, long lowerBound, long upperBound
            , TranslateRoutingModelIndexCallback onTranslateIndex)
            where TDimension : Dimension =>
            dim.SetCumulVarRange(i, lowerBound, upperBound
                , (RoutingContext context, int _) => onTranslateIndex(context.Model, _));

        /// <summary>
        /// Sets the Accumulator Variable <em>Lower-</em> and <em>Upper-Bounds</em>
        /// corresponding to the Index <paramref name="i"/> translated according to
        /// <paramref name="onTranslateIndex"/> terms. Assumes that the Dimension has
        /// been added to the Model.
        /// </summary>
        /// <typeparam name="TDimension">A Dimension type.</typeparam>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="bounds">The Lower and Upper Bounds.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        public static void SetCumulVarRange<TDimension>(this TDimension dim, int i, in (long lower, long upper) bounds
            , TranslateRoutingModelIndexCallback onTranslateIndex)
            where TDimension : Dimension =>
            dim.SetCumulVarRange(i, bounds
                , (RoutingContext context, int _) => onTranslateIndex(context.Model, _));

        // TODO: TBD: could perhaps add other variations of the finalizer concerns...
        /// <summary>
        /// Adds a Variable Minimized by Finalizer instruction to the Model. Assumes that
        /// the Dimension has been added to the Model.
        /// </summary>
        /// <typeparam name="TDimension">A Dimension type.</typeparam>
        /// <param name="dim">A <typeparamref name="TDimension"/> instance.</param>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>As with the other helper methods, the purpose of the finalizer methods is to
        /// abstract away the mechanics of arranging such, in order to allow the Dimension author
        /// to focus on the important aspects pertaining to the dimension in particular.</remarks>
        /// <see cref="AddVariableMaximizedByFinalizer{TDimension}(TDimension, int, TranslateRoutingModelIndexCallback)"/>
        public static void AddVariableMinimizedByFinalizer<TDimension>(this TDimension dim, int i, TranslateRoutingModelIndexCallback onTranslateIndex)
            where TDimension : Dimension =>
            dim.AddVariableMaximizedByFinalizer(i
                , (RoutingContext context, int _) => onTranslateIndex(context.Model, _));

        /// <summary>
        /// Adds a Variable Minimized by Finalizer instruction to the Model. Assumes that
        /// the Dimension has been added to the Model.
        /// </summary>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>As with the other helper methods, the purpose of the finalizer methods is to
        /// abstract away the mechanics of arranging such, in order to allow the Dimension author
        /// to focus on the important aspects pertaining to the dimension in particular.</remarks>
        /// <see cref="AddVariableMinimizedByFinalizer{TDimension}(TDimension, int, TranslateRoutingModelIndexCallback)"/>
        public static void AddVariableMaximizedByFinalizer<TDimension>(this TDimension dim, int i, TranslateRoutingModelIndexCallback onTranslateIndex)
            where TDimension : Dimension =>
            dim.AddVariableMaximizedByFinalizer(i
                , (RoutingContext context, int _) => onTranslateIndex(context.Model, _));
    }
}
