var oTable;
var checkBStatus;
$(document).ready(function () {
    $("#ddlReasonType").chosen({ no_results_text: "Oops, nothing found!" });
    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');
    BindFormType();
    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liReasonMaster');
    listate.addClass("active");

    //#endregion


    //#region disable first Character Space
    $(function () {
        $('body').on('keydown', '#txtReason', function (e) {
            console.log(this.value);
            if (e.which === 32 && e.target.selectionStart === 0) {
                return false;
            }
        });
    });
    //#endregion


    $.validator.addMethod(
            "chosen",
            function (value, element) {
                return (value == null ? false : (value.length == 0 ? false : true))
            },
            "Reason Type Required"//custom message
            );


    var validator = $('#form3').validate({
        debug: true, onsubmit: false,
        rules: {
            //CategoryType: { required: true },
            SkipReason: {
                required: true,
                maxlength: 500

            }
        },
        messages: {
            //CategoryType: { required: "Reason Type Required." },
            SkipReason: {
                required: "Reason Required.",
                maxlength: "Maxlength 250."
            }

        },

        submitHandler: function (form) {

        },
        invalidHandler: function () {

        }
    });
    //#endregion


    //#region Save State button
    $("#btnSave").click(function () {

        $("#ddlReasonType").focus();

        var _categoryType = $("#ddlReasonType option:selected").text();
        var _Reason = $("#txtReason").val();
        if (_categoryType == null || _categoryType == "") {
            var $toast = toastr["error"]("Please select reason type", "Notification");
        }
        else if (_Reason == null || _Reason == "") {
            var $toast = toastr["error"]("Please enter reason", "Notification");
        }
        else {
            if ($("#SkipId").val() != "") {
                UpdateRecord();
            }
            else {
                CreateRecord();
            }
        }

    });
    //#endregion

    bindActiveTable(true);




    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddReason").hide();
            bindActiveTable(false);
        }
        else {
            $("#btnAddReason").show();
            bindActiveTable(true);
        }
    });


    $("#btnAddReason").click(function () {

        var urlCreateNewRecord = $('#urlCreateData').val();

        $.ajax({
            url: urlCreateNewRecord,
            datatype: "text",
            type: "GET",
            success: function (data) {
                //debugger;
                if (data.success === true) {

                    $("#SkipId").val('');
                    $("#ddlReasonType").val('0');
                    $('#ddlReasonType').trigger('chosen:updated');
                    $("#txtReason").val('');

                    $('#chkImageRequired').prop('checked', false);
                    $('#chkAudioRequired').prop('checked', false);
                    $('#chkVideoRequired').prop('checked', false);
                    $("#btnReset").show();
                    $("h4.modal-title").text("Add New Reason");
                    RemoveErrorClass();
                    $("#myModal").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
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


    $("#btnReset").click(function () {

        $("#SkipId").val('');
        $("#ddlReasonType").val('0');
        $('#ddlReasonType').trigger('chosen:updated');
        $("#txtReason").val('');
        $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');
        $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
        RemoveErrorClass();
    })


    //#endregion

    //#region Deactivate Record Yes button
    $("#btnDeactivateYes").click(function () {

        $("#confirmDeactivatedialog").modal('hide');
        var id = $("#SkipId").val();
        DeactivateRecords(id);
    })
    //#endregion

    //#region Deactivate Record No button
    $("#btnDeactivatNo").click(function () {
        $("#confirmDeactivatedialog").modal('hide');
    })
    //#endregion

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



    $("#ulFormType").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {

            }
        },
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
            selectFormType()
        }
    });
    BindFormType();
});

//#region New code for Auditor multiselect dropdown
function selectFormType() {

    var selectedlivalue = $('#ulFormType li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
    } else {
        $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulFormType .liFormType:visible').length > 0 && $('#ulFormType .highlighted:visible').length == $('#ulFormType .liFormType:visible').length) {
        $('#ddlFormType_SelectAll').prop('checked', true);
    } else {
        $('#ddlFormType_SelectAll').prop('checked', false);
    }
    //#endregion

}

//#region open/close auditor dropdown
$("#ancFormType_chosen").click(function () {
    $("#ddlFormType_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divFormType').is(e.target)
       && $('#divFormType').has(e.target).length === 0
   ) {
        $('#ddlFormType_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});

//#endregion

function filterAuditor(element) {

    var value = $(element).val().toLowerCase();

    $("#ulFormType > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulFormType .liFormType:visible').length > 0 && $('#ulFormType .highlighted:visible').length == $('#ulFormType .liFormType:visible').length) {
        $('#ddlFormType_SelectAll').prop('checked', true);
    } else {
        $('#ddlFormType_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#ddlFormType_SelectAll').on('click', function () {
    if ($('#ulFormType .liFormType').length > 0) {

        if (this.checked) {
            $("li.liFormType:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulFormType .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
            }
            else {
                $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
            }

        }
        else {
            $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');
            $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")

        }
    }
});

//#endregion





function BindFormType() {

    // $('#ddlFormType').empty();
    //  $('#ddlFormType').trigger("chosen:updated");

    var GetProject = $('#urlGetFormType').val();

    $.ajax({
        type: 'GET',
        url: GetProject,
        async: false,
        // data: { "ProjectStatus": _proStatus, "utilityType": projectUtilityType },
        success: function (data) {

            if (data.success = true) {

                $('#ulFormType').empty();
                $.each(data.formTypeList, function (index, item) {
                    $('#ulFormType').append($('<li></li>').attr("id", item.FormTypeID).addClass("liFormType active-result").html(item.FormType));
                });

                $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")



                //  $('#ddlFormType').empty();

                //var count = data.formTypeList.length;
                //if (count > 0) {

                //    $.each(data.formTypeList, function (index, item) {
                //        if (index == 0) {
                //            $('#ddlFormType').append($('<option></option>').val('0'));
                //        }
                //        $('#ddlFormType').append($('<option></option>').val(item.FormTypeID).html(item.FormType));
                //    });
                //    $('#ddlFormType').trigger("chosen:updated");
                //}
            }
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }

    });


}


$('#ddlReasonType').change(function () {

    if ($("#ddlReasonType").val() == "3") {
        $("#divCaptureMedia1").hide();
        $("#divCaptureMedia2").hide();
        $("#divCaptureMedia3").hide();
    }
    else {
        $("#divCaptureMedia1").show();
        $("#divCaptureMedia2").show();
        $("#divCaptureMedia3").show();
    }

});

function UpdateRecord() {

    var uri = $('#urlUpdate').val();
    var _categoryType = $("#ddlReasonType option:selected").text();
    var _Reason = $("#txtReason").val();
    var _Id = $("#SkipId").val();
    var chkMediaRequired;

    var chkImageRequired = (($('#chkImageRequired').prop("checked") == true) ? true : false);
    var chkAudioRequired = (($('#chkAudioRequired').prop("checked") == true) ? true : false);
    var chkVideoRequired = (($('#chkVideoRequired').prop("checked") == true) ? true : false);

    var FormIdList = [];
    var AuditorId = $('#ulFormType li.highlighted')
    AuditorId.each(function (index, item) { FormIdList.push($(this)[0].id) });


    waitingDialog.show('Updating please wait...');

    $.ajax({
        url: uri,
        data: { "skipId": _Id, "_Reason": _Reason, "_categoryType": _categoryType, "imageRequired": chkImageRequired, "audioRequired": chkAudioRequired, "videoRequired": chkVideoRequired, "FormTypeList": FormIdList },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable(true);
                $("#txtReasoneId").val('');
                $("#ddlReasonType").val('0');
                $('#ddlReasonType').trigger('chosen:updated');
                //  $("#ddlFormType").val('0');
                //  $('#ddlFormType').trigger('chosen:updated');
                $("#txtReason").val('');
                $("#SkipId").val('');
                $('#chkImageRequired').prop('checked', false);
                $('#chkAudioRequired').prop('checked', false);
                $('#chkVideoRequired').prop('checked', false);

                $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');
                $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

function CreateRecord() {

    var uri = $('#urlCreateData').val();
    var _categoryType = $("#ddlReasonType option:selected").text();
    var _Reason = $("#txtReason").val();
    var chkImageRequired = (($('#chkImageRequired').prop("checked") == true) ? true : false);
    var chkAudioRequired = (($('#chkAudioRequired').prop("checked") == true) ? true : false);
    var chkVideoRequired = (($('#chkVideoRequired').prop("checked") == true) ? true : false);

    var FormIdList = [];
    var AuditorId = $('#ulFormType li.highlighted')
    AuditorId.each(function (index, item) { FormIdList.push($(this)[0].id) });


    waitingDialog.show('Saving record please wait...');
    //bool imageRequired, bool audioRequired, bool videoRequired
    $.ajax({
        url: uri,
        data: { "_Reason": _Reason, "_categoryType": _categoryType, "imageRequired": chkImageRequired, "audioRequired": chkAudioRequired, "videoRequired": chkVideoRequired, "FormTypeList": FormIdList },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable(true);
                $("#txtReasoneId").val('');
                $("#ddlReasonType").val('0');
                $('#ddlReasonType').trigger('chosen:updated');
                //$("#ddlFormType").val('0');
                //$('#ddlFormType').trigger('chosen:updated');
                $("#txtReason").val('');
                $("#SkipId").val('');
                // $('#chkMediaRequired').prop('checked', false);
                $('#chkImageRequired').prop('checked', false);
                $('#chkAudioRequired').prop('checked', false);
                $('#chkVideoRequired').prop('checked', false);

                $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');
                $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

$.extend($.fn.dataTable.defaults, {
    "searching": false,
    "ordering": false
});


function bindActiveTable(checkBStatus) {

    waitingDialog.show('Loading please wait...');
    var urlActiveRecords = $("#urlReasonList").val();
    $.ajax({
        url: urlActiveRecords,
        data: { "ActiveStatus": checkBStatus },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'GET',
        success: function (msg) {
            debugger;
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
                           "aaData": msg.status,
                           "aoColumns": [
                                               { "mData": "CategoryType" },
                                              { "mData": "SkipReason" },
                                               { "bSearchable": false, "mData": null },
                           ],
                           "aoColumnDefs": [
                                                                  {
                                                                      "mRender": function (data, type, full, row) {
                                                                          if (msg.success) {
                                                                              if (checkBStatus) {
                                                                                  return '<a href="#" id=' + full.SkipId + ' onclick="Edit(' + full.SkipId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.SkipId + ' onclick="Deactivate(' + full.SkipId + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                                              }
                                                                              else {
                                                                                  return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.SkipId + ' onclick="Activate(' + full.SkipId + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                                              }
                                                                          }
                                                                      },
                                                                      "aTargets": [2]
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

    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Reason");
}

//#region Cancel button on Create Model
$("#btnCancel").click(function () {
    $("#SkipId").val('');
    $("#txtReason").val('');
    // $('#chkMediaRequired').prop('checked', false);
    $('#chkImageRequired').prop('checked', false);
    $('#chkAudioRequired').prop('checked', false);
    $('#chkVideoRequired').prop('checked', false);
    $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');
    $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
    $("#myModal").modal('hide');
})

function ActivateRecords(id) {
    waitingDialog.show('Loading please wait...');

    var urlDeleteData = $('#url_activateRecord').val();
    var uri = urlDeleteData;
    $.ajax({
        url: uri,
        type: 'POST',
        data: { "id": id },
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmActivatedialog").modal('hide');
                bindActiveTable(false);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id) {

    $("#SkipId").val(id);
    $("h4.modal-title").text("Deactivate Reason");
}

function DeactivateRecords(id) {
    waitingDialog.show('Loading please wait...');

    var urlDeleteData = $('#urlDeleteData').val();
    var uri = urlDeleteData + '?id=' + id + '&msg=' + "Deactivate";
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
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}
//#endregion



//used to reset the validation messages
//#region reset the validation messages
function RemoveErrorClass() {
    $("#ddlReasonType").removeClass("error");
    $("#txtReason").removeClass("error");
    $("#txtReason-error").remove();
    $("#txtReason-error").val('');
    $("#txtSkipId").val('');
    $("#txtReason").val('');
}

//#endregion


function Edit(ID) {
    $("#SkipId").val('');
    $("#txtReason").val('');
    $("#ddlReasonType").val('0');
    // $('#chkMediaRequired').prop('checked', false);
    $('#chkImageRequired').prop('checked', false);
    $('#chkAudioRequired').prop('checked', false);
    $('#chkVideoRequired').prop('checked', false);
    waitingDialog.show('Loading record please wait...');
    
    var urlGetData = $('#urlGetEditData').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { skipId: ID },
        success: function (data) {

            if (data.success === true) {

                $("#SkipId").val(ID);
                $("#divCaptureMedia1").show();
                $("#divCaptureMedia2").show();
                $("#divCaptureMedia3").show();

                if (data.status.CategoryType == 'SKIP') {
                    $('#ddlReasonType').val('1');
                }
                else if (data.status.CategoryType == 'RTU') {
                    $('#ddlReasonType').val('2');
                }
                else if (data.status.CategoryType == 'REOPEN') {
                    $('#ddlReasonType').val('3');
                    $("#divCaptureMedia1").hide();
                    $("#divCaptureMedia2").hide();
                    $("#divCaptureMedia3").hide();
                }
                else {
                    $('#ddlReasonType').val('0');
                }


                $.each(data.status.FormTypeList, function (index, item) {
                    $("#ulFormType").find("li[id='" + item.toString() + "']").addClass("ui-selected click-selected highlighted ")
                });

                var selectedlivalue = $('#ulFormType li.highlighted').map(function (i, el) { return $(el).val(); });


                var selectedlilength = selectedlivalue.length;
                if (selectedlilength == 0) {
                    $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                } else {
                    $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
                }

                //#region SelectAll checked/unchecked
                if ($('#ulFormType .liFormType:visible').length > 0 && $('#ulFormType .highlighted:visible').length == $('#ulFormType .liFormType:visible').length) {
                    $('#ddlFormType_SelectAll').prop('checked', true);
                } else {
                    $('#ddlFormType_SelectAll').prop('checked', false);
                }
                //#endregion



                $('#ddlReasonType').trigger('chosen:updated');
                $("#txtReason").val(data.status.SkipReason);

                $('#chkImageRequired').prop('checked', (data.status.IsImageRequired == true ? true : false));
                $('#chkAudioRequired').prop('checked', (data.status.IsAudioRequired == true ? true : false));
                $('#chkVideoRequired').prop('checked', (data.status.IsVideoRequired == true ? true : false));



                $("#btnReset").hide();
                $("h4.modal-title").text("Edit Reason");
                $("#myModal").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });



            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }

            waitingDialog.hide();

        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }

    });


}