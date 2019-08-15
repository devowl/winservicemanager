using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace WS.Manager.Presentation.Behaviors
{
    /// <summary>
    /// Makes <see cref="ListView"/> column sortable.
    /// </summary>
    public class ListViewSortableColumnsBehavior : Behavior<ListView>
    {
        private GridView _gridView;

        public static void ApplySort(ICollectionView view, string propertyName)
        {
            var direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                var currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    direction = currentSort.Direction == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }

                view.SortDescriptions.Clear();
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            _gridView = AssociatedObject.View as GridView;
            if (_gridView == null)
            {
                throw new NotSupportedException("None supporting view type");
            }

            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnColumnClick));
            AssociatedObject.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItem != null)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnColumnClick));
        }

        private void OnColumnClick(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            if (header != null)
            {
                var columnHeader = _gridView.Columns.First(column => Equals(column.Header, header.Content));
                string propertyName = ((Binding)columnHeader.DisplayMemberBinding).Path.Path;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ApplySort(AssociatedObject.Items, propertyName);
                }
            }
        }
    }
}