var oTable;

$(document).ready(function () {
    $("#Administration").addClass('active').css('color', '#EF6C00');;
    $("#subAdministration").addClass('block');

    //#region Save State button
    $("#btnSave").click(function () {


        if ($.trim($("#txtFormType").val()) != "") {
            waitingDialog.show('Saving record please wait...');
            var id = 0;
            if ($("#txtFormTypeId").val() != "") {
                id = $("#txtFormTypeId").val();
            }
            $("#txtFormTypeId").val();
            var formType = $("#txtFormType").val();
            var FormTypeDes = $("#txtFormTypeDescription").val();
            var idArray = [];
            var idArrayVal = [];
            $("#FormMapping input:checkbox").each(function () {
                idArray.push($(this).attr("id"));
                if ($("#" + ($(this).attr("id"))).prop("checked") == true) {
                    idArrayVal.push("true");
                }
                else {
                    idArrayVal.push("false");
                }
            });


            var uri = $('#urlEditData').val();

            $.ajax({
                url: uri,
                data: {
                    "Id": id,
                    "FormType": formType,
                    "FormTypeDescription": FormTypeDes,
                    "FType": idArray,
                    "FtypeBool": idArrayVal
                },
                type: 'POST',
                success: function (msg) {
                    if (msg.success == true) {

                        $("#myModal").modal('hide');
                        $("#txtFormTypeId").val('');
                        $("#txtFormType").val('');
                        $("#txtFormTypeDescription").val('');


                        var $toast = toastr["success"](msg.returnMessage, "Success Notification");
                        bindActiveTable(true);

                    } else {

                        var $toast = toastr["error"](msg.returnMessage, "Notification");
                    }
                    waitingDialog.hide();
                },
                error: function () {
                    alert("something seems wrong");
                    waitingDialog.hide();
                }
            });

        }
        else {
            var $toast = toastr["error"]("Please enter the form type", "Notification");
        }

    })
    //#endregion


    bindActiveTable(true);


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liFormType');
    listate.addClass("active");

    //#endregion




    //#region Add State button
    $("#btnAddFormType").click(function () {


        var urlCreateNewRecord = $('#urlEditData').val();

        $.ajax({
            url: urlCreateNewRecord,
            datatype: "text",
            type: "GET",
            success: function (data) {
                //debugger;
                if (data.success === true) {

                    $("#chkShowDeactivate").prop('checked', false);
                    $("#txtFormTypeId").val('');
                    $("#txtFormType").val('');
                    $("#txtFormTypeDescription").val('');


                    $("#btnReset").show();
                    $("h4.modal-title").text("Add New Form Type");
                    $("#myModal").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });

                    var urlGetData = $('#urlGetAddNew').val();
                    var mastDiv = $('#FormMapping');
                    mastDiv.empty();

                    $.ajax({
                        type: 'GET',
                        url: urlGetData,
                        success: function (msg) {

                            var result = '';
                            mastDiv.empty();

                            //new code
                            var $div = $("<div>", {
                                class: "form-group",
                            });

                            var formgroup = '<div class="form-group">';
                            var formgroupend = '</div>';

                            $.each(msg, function (id, BaseFormTypeModel) {
                                result = result + '<label class="col-lg-2 col-md-2 col-sm-3 col-xs-4 col-xxs-6 control-label ">' + BaseFormTypeModel.FormType + ' </label><div class="col-lg-2 col-sm-2 col-xs-2"><input type="checkbox" id="chk' + BaseFormTypeModel.FormTypeID + '"' +
                                    '' + (BaseFormTypeModel.ImpactStatus ? 'checked' : '') + '></div>';
                            })

                            $div.append(result)
                            mastDiv.append($div)

                            // End new code


                            $('#myModal').on('shown.bs.modal', function () {
                                $('#txtFormTypeId').focus();
                            })
                            RemoveErrorClass();

                        },
                        error: function () {
                            alert("something seems wrong");

                        }

                    });

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




    })
    //#endregion

    //#region Cancel button on Create Model
    $("#btnCancel").click(function () {
        $("#txtFormTypeId").val('');
        $("#txtFormType").val('');
        $("#txtFormTypeDescription").val('');
        $("#myModal").modal('hide');
    })
    //#endregion

    //#region Reset button on Create Model
    $("#btnReset").click(function () {

        //validator.resetForm();
        $("#txtFormTypeId").val('');
        $("#txtFormType").val('');
        $("#txtFormTypeDescription").val('');
        $("#txtFormType").focus();


    })
    //#endregion


    $("#btnclose").click(function () {

        //validator.resetForm();
        $("#txtFormTypeId").val('');
        $("#txtFormType").val('');
        $("#txtFormTypeDescription").val('');
        $("#txtFormType").focus();
    })

    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddFormType").hide();
            bindActiveTable(false);
        }
        else {
            $("#btnAddFormType").show();
            bindActiveTable(true);
        }
    });
    //#endregion

    //#region Deactivate Record Yes button
    $("#btnDeactivateYes").click(function () {

        $("#confirmDeactivatedialog").modal('hide');
        var id = $("#deactivationID").val();
        DeactivateRecords(id);
    })
    //#endregion

    //#region Deactivate Record No button
    $("#btnDeactivatNo").click(function () {

        $("#confirmDeactivatedialog").modal('hide');
    })
    //#endregion
    //abc
    //#region Activate Record Yes button
    $("#btnActivateYes").click(function () {

        var id = $("#activationID").val();
        ActivateRecords(id);
    })
    //#endregion


    //#region Activate Record No button
    $("#btnActivatNo").click(function () {

        $("#confirmActivatedialog").modal('hide');
    })
    //#endregion

});




$.extend($.fn.dataTable.defaults, {
    "searching": false,
    "ordering": false
});
function bindActiveTable(isActive) {
    debugger;
    var urlActiveRecords = $("#UrlGetFormTypeList").val();
    waitingDialog.show('Loading please wait...');
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {
            "isActive": isActive
        },
        type: 'GET',
        success: function (msg) {

            oTable = $('#dynamic-table').dataTable(
   {

       "bDestroy": true,
       "bserverSide": true,
       "bDeferRender": true,
       //"sAjaxDataProp": 'aData',
       "oLanguage": {
           "sLengthMenu": '_MENU_ Records per page'
       },
       //"sAjaxSource": msg.result,
       "aaData": msg,
       "aoColumns": [
                          { "mData": "strFormType" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      if (isActive) {
                                                          return '<a href="#" id=' + full.ID + ' onclick="Edit(' + full.ID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                                                              '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.ID + ' onclick="Deactivate(' + full.ID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      }
                                                      else {
                                                          return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.ID + ' onclick="Activate(' + full.ID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                      }
                                                  },
                                                  "aTargets": [1]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);


            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });


}

//#region Record Activation functions

function Activate(id) {
    debugger;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Meter Size");
}

function ActivateRecords(id) {
    debugger;
    var urlDeleteData = $('#urlActiveDeactiveData').val();
    var uri = urlDeleteData + '?id=' + id + '&activate=true';
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {


            if (msg.success == true) {
                $("#confirmActivatedialog").modal('hide');
                bindActiveTable(false);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Notification");
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id) {

    $("#deactivationID").val(id);
    $("h4.modal-title").text("Deactivate Form Type");

}

function DeactivateRecords(id) {
    var urlDeleteData = $('#urlActiveDeactiveData').val();
    var uri = urlDeleteData + '?id=' + id + '&activate=false';
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmDeactivatedialog").modal('hide');
                bindActiveTable(true);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Notification");
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
//#endregion



//used to reset the validation messages
//#region reset the validation messages
function RemoveErrorClass() {
    $("#txtFormType").removeClass("error");
    $("#txtFormTypeDescription").removeClass("error");
    $("#txtFormType-error").remove();
    $("#txtFormType-error").val('');
    $("#txtFormTypeDescription").val('');
}

//#endregion




function Edit(ID) {

    var urlGetData = $('#urlGetData').val();
    var mastDiv = $('#FormMapping');
    mastDiv.empty();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        // contentType: "application/json; charset=utf-8",
        // dataType: "json",
        data: { editId: ID },
        success: function (msg) {

            if (msg.success == undefined) {
                var result = '';
                mastDiv.empty();
                $("#txtFormTypeId").val(msg.ID);
                $("#txtFormType").val(msg.strFormType);
                $("#txtFormTypeDescription").val(msg.FormTypeDescription);
                result = result + '<div class="form-group"> ';
                $.each(msg.objBaseFormTypeModelList, function (id, BaseFormTypeModel) {
                    result = result + ' <label class="col-lg-2 col-md-2 col-sm-3 col-xs-4 col-xxs-6 control-label dep-from-lab xxs-clear">' + BaseFormTypeModel.FormType + ' </label><div class="col-lg-2 col-md-2 col-sm-1 col-xs-1 col-xxs-6"><input type="checkbox" id="chk' + BaseFormTypeModel.FormTypeID + '"' +
                             '' + (BaseFormTypeModel.ImpactStatus ? 'checked' : '') + '></div>';
                })
                result = result + '</div>';
                mastDiv.append(result);

                $("#btnReset").hide();
                $("h4.modal-title").text("Edit Form Type");
                $("#myModal").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Notification");
            }

        },
        error: function () {
            alert("something seems wrong");
        }

    });


}