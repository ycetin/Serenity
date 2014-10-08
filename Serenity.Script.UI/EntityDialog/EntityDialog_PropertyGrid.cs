﻿using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected PropertyGrid propertyGrid;

        [Obsolete("Prefer async version")]
        private void InitPropertyGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            #pragma warning disable 618
            var pgOptions = GetPropertyGridOptions();
            #pragma warning restore 618

            propertyGrid = new PropertyGrid(pgDiv, pgOptions);
        }

        private Promise InitPropertyGridAsync()
        {
            return Promise.Void.ThenAwait(() =>
            {
                var pgDiv = this.ById("PropertyGrid");
                if (pgDiv.Length <= 0)
                    return Promise.Void;

                return GetPropertyGridOptionsAsync()
                    .ThenAwait(pgOptions =>
                    {
                        propertyGrid = new PropertyGrid(pgDiv, pgOptions);
                        return propertyGrid.Initialize();
                    });
            });
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var formKey = GetFormKey();
            #pragma warning disable 618
            return Q.GetForm(formKey);
            #pragma warning restore 618
        }

        protected virtual PropertyGridOptions GetPropertyGridOptions()
        {
            #pragma warning disable 618
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = GetPropertyItems(),
                Mode = PropertyGridMode.Insert,
                LocalTextPrefix = "Forms." + GetFormKey() + "."
            };
            #pragma warning restore 618
        }

        protected virtual Promise<PropertyGridOptions> GetPropertyGridOptionsAsync()
        {
            return new Promise<PropertyGridOptions>((done, fail) =>
            {
                GetPropertyItemsAsync().Then(propertyItems =>
                {
                    done(new PropertyGridOptions
                    {
                        IdPrefix = this.idPrefix,
                        Items = propertyItems,
                        Mode = PropertyGridMode.Insert
                    });
                });
            });
        }

        protected virtual Promise<List<PropertyItem>> GetPropertyItemsAsync()
        {
            return new Promise<List<PropertyItem>>((done, fail) => 
            {
                var formKey = GetFormKey();
                Q.GetFormAsync(formKey).Then(done, fail);
            });
        }
    }
}