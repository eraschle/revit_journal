using Autodesk.Revit.DB;
using Rvt = Autodesk.Revit.DB;
using DataSource.Model.Catalog;
using Cat = DataSource.Model.Catalog;
using DataSource.Model.ProductData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RevitCommand
{
    public class RevitEnumCreator
    {
        public Cat.Category CreateCategory(Rvt.Category category)
        {
            if(category is null){ return null; }

            var bic = GetBuiltInCategory(category);
            var metaCategory = new Cat.Category
            {
                Id = category.Id.ToString(),
                BuiltIn = GetBuiltInEnum(bic, BuiltInCategory.INVALID),
                Name = category.Name
            };
            return metaCategory;
        }

        private static BuiltInCategory GetBuiltInCategory(Rvt.Category category)
        {
            var categoryId = category.Id.IntegerValue;
            foreach (BuiltInCategory bic in Enum.GetValues(typeof(BuiltInCategory)))
            {
                if ((int)bic != categoryId) { continue; }

                return bic;
            }
            return BuiltInCategory.INVALID;
        }

        public RevitProductData GetBuiltInParameters(RevitProductData productData)
        {
            foreach (BuiltInParameter parameter in Enum.GetValues(typeof(BuiltInParameter)))
            {
                var model = CreateBuiltInParameter(parameter);
                if (model is null) { continue; }

                productData.BuiltInParameters.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateBuiltInParameter(BuiltInParameter parameter)
        {
            var model = Create< BuiltInParameter, RevitParameter>(
                parameter,
                BuiltInParameter.INVALID,
                () => LabelUtils.GetLabelFor(parameter),
                () => (int)parameter);
            return model;
        }

        public RevitProductData GetBuiltInParameterGroup(RevitProductData productData)
        {
            foreach (BuiltInParameterGroup group in Enum.GetValues(typeof(BuiltInParameterGroup)))
            {

                var model = CreateBuiltInParameterGroup(group);
                if (model is null) { continue; }

                productData.ParameterGroups.Add(model);
            }
            return productData;
        }

        public RevitParameterGroup CreateBuiltInParameterGroup(BuiltInParameterGroup group)
        {
            var model = Create<BuiltInParameterGroup, RevitParameterGroup>(
                group,
                BuiltInParameterGroup.INVALID,
                () => LabelUtils.GetLabelFor(group),
                () => (int)group);
            return model;
        }

        public RevitProductData GetParameterType(RevitProductData productData)
        {
            foreach (ParameterType parameterType in Enum.GetValues(typeof(ParameterType)))
            {
                var model = CreateParameterType(parameterType);
                if (model is null) { continue; }

                productData.ParameterTypes.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateParameterType(ParameterType parameterType)
        {
            var model = Create< ParameterType, RevitParameter>(
                parameterType,
                ParameterType.Invalid,
                () => LabelUtils.GetLabelFor(parameterType),
                () => (int)parameterType);
            return model;
        }

        public RevitProductData GetDisplayUnitTypes(RevitProductData productData)
        {
            foreach (DisplayUnitType displayUnit in Enum.GetValues(typeof(DisplayUnitType)))
            {
                var model = CreateDisplayUnitType(displayUnit);
                if (model is null) { continue; }

                productData.DisplayUnitTypes.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateDisplayUnitType(DisplayUnitType displayUnit)
        {
            var model = Create<DisplayUnitType, RevitDisplayUnit>(
                displayUnit,
                 DisplayUnitType.DUT_UNDEFINED,
                 () => LabelUtils.GetLabelFor(displayUnit),
                 () => (int)displayUnit);
            return model;
        }

        public RevitProductData GetUnitTypes(RevitProductData productData)
        {
            foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
            {
                var model = CreateUnitType(unitType);
                if (model is null) { continue; }

                productData.UnitTypes.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateUnitType(UnitType unitType)
        {
            var model = Create<UnitType, RevitUnitType>(
                unitType,
                UnitType.UT_Custom,
                () => LabelUtils.GetLabelFor(unitType),
                () => (int)unitType);
            return model;
        }

        public RevitProductData GetUnitSymbolTypes(RevitProductData productData)
        {
            foreach (UnitSymbolType unitSymbol in Enum.GetValues(typeof(UnitSymbolType)))
            {
                var model = CreateUnitSymbolType(unitSymbol);
                if (model is null) { continue; }

                productData.UnitSymbolTypes.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateUnitSymbolType(UnitSymbolType unitSymbol)
        {
            var model = Create<UnitSymbolType, RevitUnitSymbol>(
                unitSymbol,
                () => LabelUtils.GetLabelFor(unitSymbol),
                () => (int)unitSymbol);
            return model;
        }

        public RevitProductData GetFamilyPlacementTypes(RevitProductData productData)
        {
            foreach (FamilyPlacementType placementType in Enum.GetValues(typeof(FamilyPlacementType)))
            {
                var model = CreateFamilyPlacementType(placementType);
                if (model is null) { continue; }

                productData.PlacementTypes.Add(model);
            }
            return productData;
        }

        public RevitEnum CreateFamilyPlacementType(FamilyPlacementType placementType)
        {
            var model = Create<FamilyPlacementType, RevitPlacement>(
                placementType,
                FamilyPlacementType.Invalid,
                () => { return placementType.ToString(); },
                () => ((int)placementType));
            return model;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private TModel Create<TEnum, TModel>(TEnum builtIn, Func<string> getName, Func<int> enumValue) where TModel : RevitEnum, new()
        {
            TModel model = null;
            try
            {
                model = Create<TModel>(getName, enumValue);
                model.Id = GetBuiltInEnum(builtIn);
            }
            catch { }
            return model;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private TModel Create<TEnum, TModel>(TEnum builtIn, TEnum notValid, Func<string> getName, Func<int> enumValue) where TModel : RevitEnum, new()
        {
            TModel model = null;
            try
            {
                model = Create<TModel>(getName, enumValue);
                model.BuiltIn = GetBuiltInEnum(builtIn, notValid);
            }
            catch { }
            return model;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private TModel Create<TModel>(Func<string> getName, Func<int> enumValue) where TModel : RevitEnum, new()
        {
            TModel model = null;
            try
            {
                model = new TModel
                {
                    Name = getName.Invoke(),
                    Id = enumValue.Invoke().ToString()
                };
            }
            catch { }
            return model;
        }

        public static string GetBuiltInEnum<TEnum>(TEnum builtInEnum)
        {
            return $"{typeof(TEnum).Name}.{builtInEnum}";
        }

        public static string GetBuiltInEnum<TEnum>(TEnum builtInEnum, TEnum notValid)
        {
            return builtInEnum.Equals(notValid) ? null : GetBuiltInEnum(builtInEnum);
        }
    }
}
