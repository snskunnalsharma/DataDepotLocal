$(document).ready(function () {

    BindDataTable(true);

})

function BindDataTable(checkBStatus) {

    var urlBindTblData = $("#urlGetAllData").val();

    $.ajax({
        url: urlBindTblData,
        contentType: "application/json; charset=utf-8",
        data: { "Status": checkBStatus },
        dataType: "json",
        type: 'GET',
        success: function (data) {

            oTable = $('#dynamic-table').dataTable(
            {
                "bDestroy": true,
                "bserverSide": true,
                "bDeferRender": true,
                "oLanguage": {
                    "sLengthMenu": '_MENU_ Records per page'
                },

                "aaData": data.objList,
                "aoColumns": [
                               { "mData": "InventoryName" },
                               { "mData": "BarcodeRequired" },
                               { "bSearchable": false, "mData": null },
                ],
                "aoColumnDefs": [
                                {
                                    "mRender": function (data, type, full, row) {
                                        if (checkBStatus) {
                                            return '<a id="' + full.Id + '" onclick="EditRecord(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                                                '<a data-backdrop="static" data-toggle="modal" id=' + full.Id + ' onclick="Deactivate(' + full.Id + ' );" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                        }
                                        else {
                                            return '<a data-toggle="modal" data-backdrop="static" id=' + full.Id + ' onclick="Activate(' + full.Id + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                        }
                                    },
                                    "aTargets": [2]
                                },
                                {
                                    "mRender": function (data, type, full, row) {
                                        if (full.BarcodeRequired === true) {
                                            return 'Yes';
                                        } else {
                                            return 'No';
                                        }
                                    },
                                    "aTargets": [1]
                                },
                ],

            }
        ), $('.dataTables_filter input').attr('maxlength', 50);

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



$('#btnAddInventory').click(function () {
    // debugger;
    var urlCreateNewRecord = $('#urlCreateRecord').val();

    $.ajax({
        url: urlCreateNewRecord,
        datatype: "text",
        type: "GET",
        success: function (data) {
            //debugger;

            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")

            SetMultiSelect('MeterSize');
            SetMultiSelect('MeterType');
            SetMultiSelect('MeterMake');
            //data

            var urlGetDropDownData = $('#urlGetDropDownData').val();

            $.ajax({
                url: urlGetDropDownData,
                contentType: "application/json; charset=utf-8",
                data: { "RecordId": "0" },
                dataType: "json",
                type: "GET",
                success: function (data) {

                    //
                    SetDropdown('MeterSize', data.objInventoryMasterBO.MeterSizeList);
                    SetDropdown('MeterType', data.objInventoryMasterBO.MeterTypeList);
                    SetDropdown('MeterMake', data.objInventoryMasterBO.MeterMakeList);

                    $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });


                },
                error: function (request, status, error) {

                    var $toast = toastr["error"](request.responseText, "Notification !");

                    console.log(request.responseText)

                }
            });


        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });




})







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

    debugger;
    var form = $("#formAddEdit");

    form.validate();

    if (form.valid()) {

        var formData = $('#formAddEdit').serializeArray();

        var FormObject = {};

        $(formData).each(function (index, obj) { FormObject[obj.name] = obj.value; });




        var SelectedMeterSizeList = [];
        var MeterSizeId = $('#ulMeterSize li.highlighted')
        MeterSizeId.each(function (index, item) { SelectedMeterSizeList.push($(this)[0].id) });

        var SelectedMeterTypeList = [];
        var MeterTypeId = $('#ulMeterType li.highlighted')
        MeterTypeId.each(function (index, item) { SelectedMeterTypeList.push($(this)[0].id) });

        var SelectedMeterMakeList = [];
        var MeterMakeId = $('#ulMeterMake li.highlighted')
        MeterMakeId.each(function (index, item) { SelectedMeterMakeList.push($(this)[0].id) });



        FormObject['SelectedMeterSizeList'] = SelectedMeterSizeList;
        FormObject['SelectedMeterTypeList'] = SelectedMeterTypeList;
        FormObject['SelectedMeterMakeList'] = SelectedMeterMakeList;


        if ($('#Id').val() == "0") {

            var urlCreateNewRecord = $('#urlCreateRecord').val();
            $.ajax({
                url: urlCreateNewRecord,
                datatype: "text",
                type: "POST",
                data: { 'objInventoryMasterBO': FormObject },
                success: function (data) {
                    //debugger;
                    if (data.statusMessage.Success) {

                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
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
            $.ajax({
                url: urlEditRecord,
                datatype: "text",
                type: "POST",
                data: { 'objInventoryMasterBO': FormObject },
                success: function (data) {
                    //debugger;
                    if (data.statusMessage.Success) {
                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
                        $('#RoleId').prop('disabled', true);
                        var $toast = toastr["error"](data.statusMessage.Message, "Notification !");
                    }


                },
                error: function (request, status, error) {
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
            // debugger;


            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")

            SetMultiSelect('MeterSize');
            SetMultiSelect('MeterType');
            SetMultiSelect('MeterMake');
            //data

            var urlGetDropDownData = $('#urlGetDropDownData').val();

            $.ajax({
                url: urlGetDropDownData,
                contentType: "application/json; charset=utf-8",
                data: { "RecordId": id },
                dataType: "json",
                type: "GET",
                success: function (data) {


                    SetDropdown('MeterSize', data.objInventoryMasterBO.MeterSizeList);
                    SetDropdown('MeterType', data.objInventoryMasterBO.MeterTypeList);
                    SetDropdown('MeterMake', data.objInventoryMasterBO.MeterMakeList);


                    SetDropdownItemsOnEditOperation('MeterSize', data.objInventoryMasterBO.SelectedMeterSizeList);
                    SetDropdownItemsOnEditOperation('MeterType', data.objInventoryMasterBO.SelectedMeterTypeList);
                    SetDropdownItemsOnEditOperation('MeterMake', data.objInventoryMasterBO.SelectedMeterMakeList);



                    $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });


                },
                error: function (request, status, error) {

                    var $toast = toastr["error"](request.responseText, "Notification !");

                    console.log(request.responseText)

                }
            });




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
        content: 'Do you want to activate the current warehouse?',
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
        content: 'Do you want to deactivate the current warehouse?',
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

    $.ajax({
        type: "GET",
        url: urlActivateRecord,
        contentType: "application/json; charset=utf-8",
        data: { 'Id': id, 'Status': makeActive },
        dataType: "json",
        async: true,

        success: function (data) {
            //debugger;
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
});

//#endregion

function filterMultiSelectDropDown(element) {

    var value = $(element).val().toLowerCase();
    var FieldName = (element == "txtfilterallMeterSize") ? "MeterSize" : (element == "txtfilterallMeterType") ? "MeterType" : "MeterMake";

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
    var CurrentItem = (e.target.id === "MeterSize_SelectAll" ? "MeterSize" : (e.target.id === "MeterType_SelectAll" ? "MeterType" : "MeterMake"));
    //debugger;
    if ($('#ul' + CurrentItem + ' .li' + CurrentItem + '').length > 0) {

        if (this.checked) {

            $("li.li" + CurrentItem + ":visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ul' + CurrentItem + ' .highlighted').map(function (i, el) {
                return $(el)[0].value;
            });

            var selectedlilength = selectedlitext.length;

            if (selectedlilength == 0) {
                $("#" + CurrentItem + "Header").removeClass("ddlHeader").text("Select Any Item")
            } else {
                $("#" + CurrentItem + "Header").addClass("ddlHeader").text(selectedlilength + " Item selected")
            }

        }
        else {

            $("li.li" + CurrentItem + ":visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ul' + CurrentItem + ' .highlighted').map(function (i, el) {
                return $(el)[0].value;
            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#" + CurrentItem + "Header").removeClass("ddlHeader").text("Select Any Item")
            } else {
                $("#" + CurrentItem + "Header").addClass("ddlHeader").text(selectedlilength + " Item selected")
            }
        }
    }


});





function SetDropdownItemsOnEditOperation(dropdownName, ObjList) {
    //debugger;
    $.each(ObjList, function (index, item) {
        $("#ul" + dropdownName).find("li[value='" + item + "']").addClass("ui-selected click-selected highlighted")
    })

    var selectedlivalue = $('#ul' + dropdownName + ' li.highlighted').map(function (i, el) { return $(el).val(); });

    if (selectedlivalue.length == 0) {
        $("#" + dropdownName + "Header").removeClass("ddlHeader").text("Select Any Item")
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