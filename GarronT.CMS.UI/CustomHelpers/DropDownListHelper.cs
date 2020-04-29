using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GarronT.CMS.UI.CustomHelpers
{
    public static partial class DropDownListHelper
    {

        #region StateDropDownList

        public static MvcHtmlString StateDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue, string optionallable, object htmlattribute)
        {
            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL();

            var values = objProjectMasterDAL.GetActiveStateRecords();

            objProjectMasterDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.StateName,
                                                     Value = value.StateID.ToString(),
                                                     Selected = (value.StateID.Equals(selectedValue.ToString()))
                                                 }).OrderBy(o => o.Text).ToList();
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion





        #region CityDropDownList


        public static MvcHtmlString CityDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, long StateId, Nullable<long> selectedValue,
            string optionallable,
            object htmlattribute
            )
        {
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            var values = objWareHouseDAL.GetActiveCityRecordsForState(StateId);
            objWareHouseDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.CityName,
                                                     Value = value.CityID.ToString(),
                                                     Selected = (value.CityID.Equals(selectedValue.ToString()))
                                                 }).OrderBy(o => o.Text).ToList(); ;
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion


        #region CategoryDropDownList

        public static MvcHtmlString CategoryDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue, string optionallable, object htmlattribute)
        {
            CategoryDAL objCategoryDAL = new CategoryDAL();

            var values = objCategoryDAL.GetAllRecord(true);

            objCategoryDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.CategoryName,
                                                     Value = value.CategoryId.ToString(),
                                                     Selected = (value.CategoryId.Equals(selectedValue.ToString()))
                                                 }).OrderBy(o => o.Text).ToList();
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion


        public static MvcHtmlString MeterSizeDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue,
          string optionallable,
          object htmlattribute
          )
        {

            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            var values = projectMasterDAL.getMetersizeList();
            projectMasterDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.MeterSize,
                                                     Value = value.ID.ToString()
                                                 }).OrderBy(o => o.Text).ToList(); ;
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }



        public static MvcHtmlString MeterMakeDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue,
      string optionallable,
      object htmlattribute
      )
        {

            // using (ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL())
            //{
            //    masterListModel.objMeterMakeModelList = projectMasterDAL.getMeterMakeList();
            //    masterListModel.objMeterSizeList = projectMasterDAL.getMetersizeList();
            //    masterListModel.objMeterTypeList = projectMasterDAL.getMeterTypeList();
            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            var values = projectMasterDAL.getMeterMakeList();
            projectMasterDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.Make,
                                                     Value = value.ID.ToString()
                                                 }).OrderBy(o => o.Text).ToList(); ;
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }




        public static MvcHtmlString MeterTypeDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue,
   string optionallable,
   object htmlattribute
   )
        {


            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            var values = projectMasterDAL.getMeterTypeList();
            projectMasterDAL.Dispose();


            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.MeterType,
                                                     Value = value.ID.ToString()
                                                 }).OrderBy(o => o.Text).ToList(); ;
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }



        #region WarehouseDropDownList

        public static MvcHtmlString WarehouseDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue, string optionallable, object htmlattribute)
        {

            InventoryDAL objDAL = new InventoryDAL();
            var values = objDAL.GetWareHouseList();

            objDAL.Dispose();



            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.WarehouseName,
                                                     Value = value.WarehouseId.ToString()
                                                 }).OrderBy(o => o.Text).ToList();
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion


        #region UtilityDropDownList


        public static MvcHtmlString UtilityDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue, string optionallable, object htmlattribute)
        {
            InventoryMasterDAL objDAL = new InventoryMasterDAL();
            var values = objDAL.GetUtilityTypeList();

            objDAL.Dispose();



            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.FieldValue,
                                                     Value = value.Id.ToString(),
                                                     Selected = (value.Id.Equals(selectedValue.ToString()))
                                                 }).OrderBy(o => o.Text).ToList();
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion




        #region ProductDropDownList


        public static MvcHtmlString ProductDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Nullable<long> selectedValue, string optionallable, object htmlattribute)
        {
            InventoryMasterDAL objDAL = new InventoryMasterDAL();
            var values = objDAL.GetProductList();

            objDAL.Dispose();

            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.FieldValue,
                                                     Value = value.Id.ToString(),
                                                     Selected = (value.Id.Equals(selectedValue.ToString()))
                                                 }).OrderBy(o => o.Text).ToList();

            return SelectExtensions.DropDownListFor(htmlHelper, expression, items, optionallable, htmlattribute);
        }

        #endregion



        
    }
}