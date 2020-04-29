var spinner, opts, target;
var oTable;

$(document).ready(function () {
    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');
    //#region Spin loading
    $("#loading").fadeIn();
    opts = {
        lines: 12, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        color: '#000', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false // Whether to use hardware acceleration
    };
    target = document.getElementById('loading');
    spinner = new Spinner(opts);
    //#endregion

    //#region disable first Character Space
    $(function () {
        $('body').on('keydown', '#txtMeterSize', function (e) {
            console.log(this.value);
            if (e.which === 32 && e.target.selectionStart === 0) {
                return false;
            }
        });
    });
    //#endregion



    //#region Save State button
    $("#btnSave").click(function () {
        $("#txtMeterSize").focus();
        if ($('#form3').valid()) {
            spinner.spin(target);
            EditRecord();

        }


    })
    //#endregion

    //#region validation code
    $.validator.addMethod("AlfaNumRegex", function (value, element) {
        return this.optional(element) || /^[^<>,<|>,$]+(\.\d{1,2})?$/i.test(value);
    }, "Meter Size must contain only letters, numbers or dashes.");

    //$.validator.addMethod("notEqual", function (value, element, param) {
    //    return this.optional(element) || value != "0" || value != "Select";
    //}, "Please specify a different (non-default) value");


    $('#txtMeterSize').bind('keypress', function (event) {
        var regex = new RegExp("^[^<>,<|>,$]+(\.\d{1,2})?$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || regex.test(key)) {
            return true;
        }

        // If we got this far, just return false because a disallowed key was typed.
        return false;
    });



    var validator = $('#form3').validate({
        //focusInvalid: false,
        debug: true, onsubmit: false,
        rules: {
            MeterSize: {
                required: true,
                maxlength: 200,
                AlfaNumRegex: true,
            },

            Description: {
                required: false,
                maxlength: 500,
                // AlfaNumRegex: true,
            },

        },
        messages: {
            MeterSize: {
                required: "Meter Size Required.",
                maxlength: "Maxlength 200.",
                AlfaNumRegex: "Maxlength 200."
            },
            Description: {

                maxlength: "Maxlength 500 characters.",
                // AlfaNumRegex: "Description must contain only letters, numbers, or dashes."
            },
        },

        submitHandler: function (form) {

            //EditRecord();
            //$("#element_to_pop_up").dialog("close");
            //return false;
        },
        invalidHandler: function () {

            //  $(this).find(":input.error:first").focus();
        }
    });
    //#endregion

    // $(".ddlCountry").removeClass("error");
    bindActiveTable();

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liMeterSize');
    listate.addClass("active");

    //#endregion

    //#region Add State button
    $("#btnAddMeterSize").click(function () {

        var urlCreateNewRecord = $('#urlCreateRecord').val();

        $.ajax({
            url: urlCreateNewRecord,
            datatype: "text",
            type: "GET",
            success: function (data) {
                //debugger;
                if (data.success === true) {

                    $("#chkShowDeactivate").prop('checked', false);
                    $("#txtMeterSizeId").val('');
                    $("#txtMeterSize").val('');
                    $("#txtMeterSizeDescription").val('');


                    $("#btnReset").show();
                    $("h4.modal-title").text("Add New Meter Size");
                    $("#myModal").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });
                    validator.resetForm();
                    $('#myModal').on('shown.bs.modal', function () {
                        $('#txtMeterSize').focus();
                    })
                    RemoveErrorClass();

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
        validator.resetForm();
        $("#txtMeterSizeId").val('');
        $("#txtMeterSize").val('');
        $("#txtMeterSizeDescription").val('');
        $("#myModal").modal('hide');
    })
    //#endregion

    //#region Reset button on Create Model
    $("#btnReset").click(function () {

        validator.resetForm();
        $("#txtMeterSizeId").val('');
        $("#txtMeterSize").val('');
        $("#txtMeterSizeDescription").val('');
        $("#txtMeterSize").focus();


    })
    //#endregion


    $("#btnclose").click(function () {
        debugger;
        validator.resetForm();
        $("#txtMeterSizeId").val('');
        $("#txtMeterSize").val('');
        $("#txtMeterSizeDescription").val('');
        $("#txtMeterSize").focus();
    })

    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddMeterSize").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddMeterSize").show();
            bindActiveTable();
        }
    });
    //#endregion

    //#region Deactivate Record Yes button
    $("#btnDeactivateYes").click(function () {
        spinner.spin(target);
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

    //#region Activate Record Yes button
    $("#btnActivateYes").click(function () {
        spinner.spin(target);
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



function EditRecord() {
    debugger;
    var uri = $('#urlEditData').val();

    $.ajax({
        url: uri,
        data: $("#form3").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable();
                $("#txtMeterSizeId").val('');
                $("#txtMeterSize").val('');
                $("#txtMeterSizeDescription").val('');


                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);


        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
$.extend($.fn.dataTable.defaults, {
    "searching": false,
    "ordering": false
});

function bindActiveTable() {

    var urlActiveRecords = $("#UrlGetMeterSizeList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
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
                          { "mData": "MeterSize" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.ID + ' onclick="Edit(' + full.ID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.ID + ' onclick="Deactivate(' + full.ID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      //  return '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>';

                                                  },
                                                  "aTargets": [1]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
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
    var urlDeleteData = $('#urlDeleteData').val();
    var uri = urlDeleteData + '?id=' + id + '&msg=' + "Activate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {

            if (msg.success == true) {
                $("#confirmActivatedialog").modal('hide');
                bindDeactivateTable();
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);

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
    $("h4.modal-title").text("Deactivate Meter Size");
}

function DeactivateRecords(id) {
    var urlDeleteData = $('#urlDeleteData').val();
    var uri = urlDeleteData + '?id=' + id + '&msg=' + "Deactivate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmDeactivatedialog").modal('hide');
                bindActiveTable();
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
//#endregion

function bindDeactivateTable() {
    var urlDeActiveRecords = $("#urlDeActiveRecords").val();
    $.ajax({
        url: urlDeActiveRecords,
        contentType: "application/json; charset=utf-8",
        //  dataType: "POST",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
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
                          { "mData": "MeterSize" },

                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                        {
                            "mRender": function (data, type, full, row) {
                                //  return '<a href="#" id=' + full.MobileNo + ' onclick="Edit(' + full.MobileNo + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.MobileNo + ' onclick="Deactivate(' + full.MobileNo + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.ID + ' onclick="Activate(' + full.ID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';

                            },
                            "aTargets": [1]
                        }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }
    });


}

//used to reset the validation messages
//#region reset the validation messages
function RemoveErrorClass() {
    $("#txtMeterSize").removeClass("error");
    $("#txtMeterSizeDescription").removeClass("error");
    $("#txtMeterSize-error").remove();
    $("#txtMeterSize-error").val('');
    $("#txtMeterSizeDescription").val('');
}

//#endregion


$('#txtMeterSize').keyup(function () {

    if ($('#txtMeterSize').val() === '0') {

        $('#txtMeterSize').focus();
        var $toast = toastr["error"]("Meter Size not Zero", "Error Notification");
        $('#txtMeterSize').focus();
        $('#txtMeterSize').val('');
    }
});

function Edit(ID) {

    var urlGetData = $('#urlGetData').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { editId: ID },
        success: function (data) {
            if (data.success === undefined) {
                $("#txtMeterSizeId").val(data.ID);
                $("#txtMeterSize").val(data.MeterSize);
                $("#txtMeterSizeDescription").val(data.dataDescription);

                $("#btnReset").hide();
                $("h4.modal-title").text("Edit Meter Size");
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
        error: function () {
            alert("something seems wrong");
        }

    });


}