$(document).ready(function () {
    $("#InventoryManagement").addClass('active');
    $("#subInventoryManagement").addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liProductMaster').addClass("active");
    BindDataTable(true);

})

function BindDataTable(checkBStatus) {

    var urlBindTblData = $("#urlGetAllData").val();
    waitingDialog.show('Please Wait...');

    $.ajax({
        url: urlBindTblData,
        contentType: "application/json; charset=utf-8",
        data: { "Status": checkBStatus },
        dataType: "json",
        type: 'GET',
        success: function (data) {
            waitingDialog.hide();

            oTable = $('#dynamic-table').dataTable({
                "bDestroy": true,
                "bserverSide": true,
                "bDeferRender": true,
                "oLanguage": {
                    "sLengthMenu": '_MENU_ Records per page'
                },
                "aaData": data.objList,
                "aoColumns": [
                    { "mData": "UtilityName" },
                    { "mData": "CategoryName" },
                    { "mData": "ProductName" },
                    { "mData": "ProductPartNumber" },
                    { "mData": "ProductUPCCode" },
                    { "mData": "Make" },
                    { "mData": "Type" },
                    { "mData": "Size" },
                    { "mData": "strSerialNumberTracking" },
                    { "mData": "strConsumableProduct" },
                    { "bSearchable": false, "mData": null }
                ],
                "aoColumnDefs": [
                    {
                        "mRender": function (data, type, full, row) {
                            if (checkBStatus) {
                                return '<a id="' + full.Id + '" onclick="EditRecord(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                                    '<a data-backdrop="static" data-toggle="modal" id=' + full.Id + ' onclick="Deactivate(' + full.Id + ' );" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                            }
                            else {
                                return '<a data-toggle="modal" data-backdrop="static" id=' + full.Id + ' onclick="Activate(' + full.Id + ');" class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                            }
                        },
                        "aTargets": [10]
                    }
                ]
            });

            $('.dataTables_filter input').attr('maxlength', 50);
        },
        error: function (request, status, error) {
            waitingDialog.hide();

            console.log("GetAllData Error:", request.responseText);
            toastr["error"]("Product data loading failed. Please check server error.", "Notification !");
        }
    });
}

$('#chkShowDeactivate').change(function () {

    if ($(this).is(":checked")) {

        BindDataTable(false);
    }
    else {

        BindDataTable(true);
    }
});

$('#btnAddProduct').click(function () {
    // debugger;
    var urlCreateNewRecord = $('#urlCreateRecord').val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        url: urlCreateNewRecord,
        datatype: "text",
        type: "GET",
        success: function (data) {


            if (data.success === undefined) {
            
             
                $('#AddEditModel').html(data);
                fnValidateDynamicContent("#AddEditModel")

                $("#CategoryId").chosen({ no_results_text: "Oops, nothing found!" });
                $('#CategoryId').trigger('chosen:updated');

                $("#UtilityId").chosen({ no_results_text: "Oops, nothing found!" });
                $('#UtilityId').trigger('chosen:updated');

                $("#MeterTypeId").chosen({ no_results_text: "Oops, nothing found!" });
                $('#MeterTypeId').trigger('chosen:updated');

                $("#MeterSizeId").chosen({ no_results_text: "Oops, nothing found!" });
                $('#MeterSizeId').trigger('chosen:updated');

                $("#MeterMakeId").chosen({ no_results_text: "Oops, nothing found!" });
                $('#MeterMakeId').trigger('chosen:updated');


                $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });


            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }
            waitingDialog.hide();

        },
        error: function (request, status, error) {
            waitingDialog.hide();
            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });




})

//numeric only validation code
$('body').on('keypress', '.numeric-only', function (e) {
    var regex = new RegExp("^[0-9\b]+$");

    if (event.charCode == 0) {
        return true;
    }
    else {
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
});

function fnValidateDynamicContent(element) {
    // debugger;

    var currForm = $(element).find("form");
    currForm.removeData("validator");
    currForm.removeData("unobtrusiveValidation");
    var $this = $(element);

    if ($.validator.unobtrusive != undefined) {
        $.validator.unobtrusive.parse(element);
    }

    currForm.validate(); // This line is important and added for client side validation to trigger, without this it didn't fire client side errors.
}

$(document).on("click", '#btnSave', function () {


    
    var form = $("#formAddEdit");

    form.validate();

    if (form.valid()) {

        if ($('#SerialNumberTracking').is(":checked") && $('#ConsumableProduct').is(":checked")) {
            var $toast = toastr["error"]("Save failed. Product can not have Serial number & Consumables at a time.", "Notification !");
            return;
        }
        else if ($("#CategoryId").val() === "") {
            var $toast = toastr["error"]("Save failed. Category required.", "Notification !");
            return;
        }
        else if ($("#UtilityId").val() === "") {
            var $toast = toastr["error"]("Save failed. Utility required.", "Notification !");
            return;
        }
        else if ($("#MeterMakeId").val() === "") {
            var $toast = toastr["error"]("Save failed. Make required.", "Notification !");
            return;
        }
        var formData = $('#formAddEdit').serializeArray();

        var FormObject = {};

        //$(formData).each(function (index, obj) {
        //    debugger;
        //    FormObject[obj.name] = obj.value;
        //});
        debugger;
        FormObject['Id'] = parseInt($("#Id").val());
        FormObject['CategoryId'] = $("#CategoryId").val();
        FormObject['UtilityId'] = $("#UtilityId").val();
        FormObject['MeterMakeId'] = $("#MeterMakeId").val();
        FormObject['MeterSizeId'] = $("#MeterSizeId").val();
        FormObject['MeterTypeId'] = $("#MeterTypeId").val();
        FormObject['Base'] = $("#Base").val();
        FormObject['ProductName'] = $("#ProductName").val();
        FormObject['ProductDescription'] = $("#ProductDescription").val();

        FormObject['ProductPartNumber'] = $("#ProductPartNumber").val();
        FormObject['ProductUPCCode'] = $("#ProductUPCCode").val();

        FormObject['SerialNumberRequired'] = $('#rbSerialNumber').is(":checked") ? true : false;
        //FormObject['ConsumableProduct'] = $('#ConsumableProduct').is(":checked") ? true : false;
        FormObject['GenerateBarcode'] = $('#GenerateBarcode').is(":checked") ? true : false;


        FormObject['WarehouseMinimumStock'] = $("#WarehouseMinimumStock").val();
        FormObject['UserMinimumStock'] = $("#UserMinimumStock").val();
        FormObject['AlertTypeEmail'] = $('#AlertTypeEmail').is(":checked") ? true : false;
        FormObject['AlertTypeSMS'] = $('#AlertTypeSMS').is(":checked") ? true : false;

        var ManagerId = [];

        $('#ulUser li.highlighted').map(function (i, el) {
            ManagerId.push(el.value);
        });
        FormObject["ManagerList"] = ManagerId;

        //var SelectedUtilityTypeList = [];
        //var UtilityTypeId = $('#ulUtilityType li.highlighted')
        //UtilityTypeId.each(function (index, item) { SelectedUtilityTypeList.push($(this)[0].id) });

        //if (SelectedUtilityTypeList.length === 0) {
        //    var $toast = toastr["error"]("Please select utlity type first.", "Notification !");
        //}
        //if ($("#CategoryId").val() === undefined || $("#CategoryId").val() === "" || $("#CategoryId").val() === null) {
        //    var $toast = toastr["error"]("Please select category first.", "Notification !");
        //}

        //var SelectedMeterMakeList = [];
        //var MeterMakeId = $('#ulMeterMake li.highlighted')
        //MeterMakeId.each(function (index, item) { SelectedMeterMakeList.push($(this)[0].id) });


        //var SelectedMeterSizeList = [];
        //var MeterSizeId = $('#ulMeterSize li.highlighted')
        //MeterSizeId.each(function (index, item) { SelectedMeterSizeList.push($(this)[0].id) });

        //var SelectedMeterTypeList = [];
        //var MeterTypeId = $('#ulMeterType li.highlighted')
        //MeterTypeId.each(function (index, item) { SelectedMeterTypeList.push($(this)[0].id) });

        //var SelectedMeterMakeList = [];
        //var MeterMakeId = $('#ulMeterMake li.highlighted')
        //MeterMakeId.each(function (index, item) { SelectedMeterMakeList.push($(this)[0].id) });


        //FormObject['SelectedUtilityTypeList'] = SelectedUtilityTypeList;
        //FormObject['SelectedMeterMakeList'] = SelectedMeterMakeList;
        //FormObject['SelectedMeterSizeList'] = SelectedMeterSizeList;
        //FormObject['SelectedMeterTypeList'] = SelectedMeterTypeList;





        if ($('#Id').val() == "0") {

            var urlCreateNewRecord = $('#urlCreateRecord').val();
            $.ajax({
                url: urlCreateNewRecord,
                datatype: "text",
                //contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                type: "POST",
                data: { 'objProductMasterBO': FormObject },
                success: function (data) {
                    debugger;
                    if (data.statusMessage.Success) {
                        //$("#btnPrintPartBarcode").attr("disabled", false)

                        //var upccodevalue = $("#ProductUPCCode").val()
                        //if (upccodevalue != "") {
                        //    $("#btnPrintUpcBarcode").attr("disabled", false)
                        //}
                        //else {
                        //    $("#btnPrintUpcBarcode").attr("disabled", true)
                        //}

                        $('#Id').val(data.productId)
                        $('#ProductUPCCode').val(data.ProductUPCCode)
                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        // $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
                        //$("#btnPrintPartBarcode").attr("disabled", true)
                        // $("#btnPrintUpcBarcode").attr("disabled", true)

                        var $toast = toastr["error"](data.statusMessage.Message, "Notification !");
                    }

                },
                error: function (request, status, error) {
                    var $toast = toastr["error"](request.responseText, "Notification !");
                    console.log(request.responseText)
                }
            });
        }
        else {

            var urlEditRecord = $('#urlEdit').val();
            waitingDialog.show('Please Wait...');
            $.ajax({
                url: urlEditRecord,
                datatype: "text",
                type: "POST",
                //contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: { 'objProductMasterBO': FormObject },
                success: function (data) {
                    //debugger;
                    waitingDialog.hide();
                    if (data.statusMessage.Success) {
                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        //$("#btnPrintPartBarcode").attr("disabled", false)

                        //var upccodevalue = $("#ProductUPCCode").val()
                        //if (upccodevalue != "") {
                        //    $("#btnPrintUpcBarcode").attr("disabled", false)
                        //}
                        //else {
                        //    $("#btnPrintUpcBarcode").attr("disabled", true)
                        //}

                        // $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
                        $('#RoleId').prop('disabled', true);
                        var $toast = toastr["error"](data.statusMessage.Message, "Notification !");
                    }


                },
                error: function (request, status, error) {
                    waitingDialog.hide();
                    var $toast = toastr["error"](request.responseText, "Notification !");
                    console.log(request.responseText)
                }
            });
        }
    }

})

$(document).on("click", '#btnCancel', function () {

    $("#AddEditModel").modal('hide');

})

function EditRecord(id) {
    //debugger;

    var urlEditRecord = $("#urlEdit").val();

    $.ajax({
        type: "GET",
        url: urlEditRecord,
        data: { 'Id': id },
        datatype: "text",
        success: function (data) {

            if (data.success === undefined) {

                $('#AddEditModel').html(data);
                fnValidateDynamicContent("#AddEditModel")

                $("#CategoryId").chosen({ no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');

                $("#UtilityId").chosen({ no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');

                $("#MeterTypeId").chosen({ no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');

                $("#MeterSizeId").chosen({ no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');

                $("#MeterMakeId").chosen({ no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');

                $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });

            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }



        },
        error: function (request, status, error) {
            var $toast = toastr["error"](request.responseText, "Notification !");
            console.log(request.responseText)
        }
    });

}

//#region Activate record
function Activate(id) {
    //debugger;

    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Activate Warehouse!',
        content: 'Do you want to activate the current product?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        ActiveDeactivateFunction(id, true)
                    }
                },
            No: {
                text: 'No',
                btnClass: 'btn-blue',
                action: function () {

                }
            }
        }
    });


}

function Deactivate(id) {
    //debugger;


    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Deactivate Warehouse!',
        content: 'Do you want to deactivate the current product?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        ActiveDeactivateFunction(id, false)
                    }
                },
            No: {
                text: 'No',
                btnClass: 'btn-blue',
                action: function () {

                }
            }
        }
    });
}

// Active deactive the individual customer
function ActiveDeactivateFunction(id, makeActive) {

    var urlActivateRecord = $("#urlActiveDeactive").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: "GET",
        url: urlActivateRecord,
        contentType: "application/json; charset=utf-8",
        data: { 'Id': id, 'Status': makeActive },
        dataType: "json",
        async: true,

        success: function (data) {
            waitingDialog.hide();

            if (data.statusMessage.Success) {
                var $toast = toastr["success"](data.statusMessage.Message, "Notification !");

            }
            else {
                var $toast = toastr["error"](data.statusMessage.Message, "Notification !");

            }
            if ($("#chkShowDeactivate").is(":checked")) {

                BindDataTable(false);
            }
            else {

                BindDataTable(true);
            }
        },
        error: function (request, status, error) {
            waitingDialog.hide();
            var $toast = toastr["error"](request.responseText, "Notification !");
            console.log(request.responseText)
        }
    });
}

//#

function SetMultiSelect(dropDownName) {

    $("#ul" + dropDownName).selectable({

        selected: function (event, ui) {

            if ($(ui.selected).hasClass('highlighted')) {
                $(ui.selected).removeClass('ui-selected click-selected highlighted');

            } else {
                $(ui.selected).addClass('click-selected highlighted');
            }
        },
        unselected: function (event, ui) {
            $(ui.unselected).removeClass('click-selected');
        },
        stop: function (event, ui) {
            selectDropdownItems(dropDownName);
        }
    });



}

function SetDropdown(dropdownName, DropDownListItems) {

    $('#ul' + dropdownName).empty();

    $.each(DropDownListItems, function (index, item) {
        $('#ul' + dropdownName).append($('<li></li>').val(item.Id).attr("ID", item.Id).addClass("li" + dropdownName + " active-result").html(item.FieldValue));
    });

}

function selectDropdownItems(dropdownName) {
    //debugger;
    var selectedlivalue = $('#ul' + dropdownName + ' li.highlighted').map(function (i, el) { return $(el).val(); });


    if (selectedlivalue.length == 0) {
        $("#" + dropdownName + "Header").removeClass("ddlHeader").text("Select")
    } else {
        $("#" + dropdownName + "Header").addClass("ddlHeader").text(selectedlivalue.length + " Items Selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ul' + dropdownName + ' .li' + dropdownName + ':visible').length > 0 && $('#ul' + dropdownName + ' .highlighted:visible').length == $('#ul' + dropdownName + ' .li' + dropdownName + ':visible').length) {
        $('#' + dropdownName + '_SelectAll').prop('checked', true);
    } else {
        $('#' + dropdownName + '_SelectAll').prop('checked', false);
    }
    //#endregion

}
//#region open/close Cycle dropdown

$(document).on("click", '.CurrentSelectedDropDown', function (e) {
    if (e.target.id === "MeterSizeHeader" || e.target.id === "ancMeterSize_chosen") {
        $("#MeterSize_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

    if (e.target.id === "MeterTypeHeader" || e.target.id === "ancMeterType_chosen") {
        $("#MeterType_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

    if (e.target.id === "MeterMakeHeader" || e.target.id === "ancMeterMake_chosen") {
        $("#MeterMake_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

    if (e.target.id === "UtilityTypeHeader" || e.target.id === "ancUtilityType_chosen") {
        $("#UtilityType_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }
});

$('body').on('click', function (e) {

    if (!$('#divMeterSize').is(e.target) && $('#divMeterSize').has(e.target).length === 0) {
        $('#ancMeterSize_chosen').removeClass('chosen-with-drop chosen-container-active');
    }

    if (!$('#divMeterType').is(e.target) && $('#divMeterType').has(e.target).length === 0) {
        $('#ancMeterType_chosen').removeClass('chosen-with-drop chosen-container-active');
    }

    if (!$('#divMeterMake').is(e.target) && $('#divMeterMake').has(e.target).length === 0) {
        $('#ancMeterMake_chosen').removeClass('chosen-with-drop chosen-container-active');
    }

    if (!$('#divUtilityType').is(e.target) && $('#divUtilityType').has(e.target).length === 0) {
        $('#ancUtilityType_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});

//#endregion

function filterMultiSelectDropDown(element) {

    var value = $(element).val().toLowerCase();
    var FieldName = (element == "txtfilterallMeterSize") ? "MeterSize" : (element == "txtfilterallMeterType") ? "MeterType" : (element == "txtfilterallMeterMake") ? "MeterMake" : "UtilityType";

    $("#txtfilterall" + FieldName + " > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ul' + FieldName + ' .li' + FieldName + ':visible').length > 0 && $('#ul' + FieldName + ' .highlighted:visible').length == $('#ul' + FieldName + ' .li' + FieldName + ':visible').length) {
        $('#' + FieldName + '_SelectAll').prop('checked', true);
    } else {
        $('#' + FieldName + '_SelectAll').prop('checked', false);
    }
    //#endregion
}


$(document).on("click", '.chbSelectAll', function (e) {
    //$('#chbSelectAll').on('click', function (e) {
    var CurrentItem = (e.target.id === "MeterSize_SelectAll" ? "MeterSize" : (e.target.id === "MeterType_SelectAll" ? "MeterType" : (e.target.id === "MeterMake_SelectAll" ? "MeterMake" : "UtilityType")));
    //debugger;
    if ($('#ul' + CurrentItem + ' .li' + CurrentItem + '').length > 0) {

        if (this.checked) {

            $("li.li" + CurrentItem + ":visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ul' + CurrentItem + ' .highlighted').map(function (i, el) {
                return $(el)[0].value;
            });

            var selectedlilength = selectedlitext.length;

            if (selectedlilength == 0) {
                $("#" + CurrentItem + "Header").removeClass("ddlHeader").text("-- Select --")
            } else {
                $("#" + CurrentItem + "Header").addClass("ddlHeader").text(selectedlilength + " Item Selected")
            }

        }
        else {

            $("li.li" + CurrentItem + ":visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ul' + CurrentItem + ' .highlighted').map(function (i, el) {
                return $(el)[0].value;
            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#" + CurrentItem + "Header").removeClass("ddlHeader").text("-- Select --")
            } else {
                $("#" + CurrentItem + "Header").addClass("ddlHeader").text(selectedlilength + " Item Selected")
            }
        }
    }


});

function SetDropdownItemsOnEditOperation(dropdownName, ObjList) {
    debugger;
    $.each(ObjList, function (index, item) {
        $("#ul" + dropdownName).find("li[value='" + item + "']").addClass("ui-selected click-selected highlighted")
    })

    var selectedlivalue = $('#ul' + dropdownName + ' li.highlighted').map(function (i, el) { return $(el).val(); });

    if (selectedlivalue.length == 0) {
        $("#" + dropdownName + "Header").removeClass("ddlHeader").text("-- Select --")
    } else {
        $("#" + dropdownName + "Header").addClass("ddlHeader").text(selectedlivalue.length + " Items Selected")
    }

    //#region SelectAll checked/unchecked
    //if ($('#ul' + dropdownName + ' .li' + dropdownName + ':visible').length > 0 && $('#ul' + dropdownName + ' .highlighted:visible').length == $('#ul' + dropdownName + ' .li' + dropdownName + ':visible').length) {
    //    $('#' + dropdownName + '_SelectAll').prop('checked', true);
    //} else {
    //    $('#' + dropdownName + '_SelectAll').prop('checked', false);
    //}
    //#endregion
    //$('#ul' + dropdownName  + ' .li' + dropdownName  + '').length
    //$('#ul' + dropdownName + ' .highlighted').length
    if ($('#ul' + dropdownName + ' .li' + dropdownName + '').length === $('#ul' + dropdownName + ' .highlighted').length) {
        $('#' + dropdownName + '_SelectAll').prop('checked', true);
    } else {
        $('#' + dropdownName + '_SelectAll').prop('checked', false);
    }
}

//$(document).on("click", "#btnPrintPartBarcode", function () {
//    debugger;
//    GeneratePartBarcode();
//})

function GeneratePartBarcode() {

    var ProductName = $("#ProductName").val();
    var PartNumber = $("#ProductPartNumber").val();
    if (ProductName == "") {
        var $toast = toastr["error"]("Please, enter product name", "Notification");
        $("#ProductName").focus();

        return false;
    }

    if (PartNumber == "") {
        var $toast = toastr["error"]("Please, enter part number", "Notification");
        $("#ProductPartNumber").focus();

        return false;
    }
    //var urlGenerateBarcode = $("#urlGenerateBarcode").val();
    //location.href = urlGenerateBarcode + "?productName=" + ProductName + "&partNumber=" + PartNumber

    var urlGenerateBarcode = $("#urlGenerateBarcode").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlGenerateBarcode,
        data: { productName: ProductName, partNumber: PartNumber },
        success: function (data) {
            waitingDialog.hide();

            if (data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                //location.href = data.filepath;

                window.open(data.filepath, '_blank');
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })

}


//$(document).on("click", "#btnPrintUpcBarcode", function () {
//    debugger;
//    GenerateUPCBarcode();
//})



function GenerateUPCBarcode() {

    var ProductName = $("#ProductName").val();
    var ProductUPCCode = $("#ProductUPCCode").val();
    if (ProductName == "") {
        var $toast = toastr["error"]("Please, enter product name", "Notification");
        $("#ProductName").focus();

        return false;
    }

    if (ProductUPCCode == "") {
        var $toast = toastr["error"]("Please, enter upc code", "Notification");
        $("#ProductUPCCode").focus();

        return false;
    }
    //var urlGenerateBarcode = $("#urlGenerateBarcode").val();
    //location.href = urlGenerateBarcode + "?productName=" + ProductName + "&partNumber=" + PartNumber

    var urlGenerateBarcodeUPC = $("#urlGenerateBarcodeUPC").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlGenerateBarcodeUPC,
        data: { productName: ProductName, ProductUPCCode: ProductUPCCode },
        success: function (data) {
            waitingDialog.hide();

            if (data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                //location.href = data.filepath;

                window.open(data.filepath, '_blank');
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })

}


$(document).on("click", "#btnPrintBarcode", function () {
    var ProductId = $("#Id").val();

    if (ProductId != undefined && ProductId != "" && ProductId != "0") {

        var urlPrintBarcode = $("#urlPrintBarcode").val();
        waitingDialog.show('Please Wait...');
        $.ajax({
            type: 'GET',
            url: urlPrintBarcode,
            data: { productId: ProductId },
            success: function (data) {
                waitingDialog.hide();

                if (data.success == false) {
                    var $toast = toastr["error"](data.ErrorMessage, "Notification");
                }
                else {
                    //location.href = data.filepath;

                    window.open(data.filepath, '_blank');
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                console.log("error " + textStatus);
                console.log("incoming Text " + jqXHR.responseText);
                var $toast = toastr["error"]("something seems wrong", "Notification");
            }

        })
    }

})


