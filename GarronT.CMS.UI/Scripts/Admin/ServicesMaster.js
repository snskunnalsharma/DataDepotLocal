var spinner, opts, target;
var oTable;
var count = 1;
$(document).ready(function () {
    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');

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
        $('body').on('keydown', '#txtServiceName', function (e) {
            console.log(this.value);
            if (e.which === 32 && e.target.selectionStart === 0) {
                return false;
            }
        });
    });
    //#endregion
    //$('#ddlPicture').multiselect(
    //  {
    //      includeSelectAllOption: true,
    //      enableFiltering: true
    //  })
    GetPictureList();

    //#region Save State button
    $("#btnSave").click(function () {
        $("#txtServiceName").focus();
        if ($('#form3').valid()) {
            spinner.spin(target);
            EditRecord();

        }


    })
    //#endregion



    //#region validation code
    $.validator.addMethod("AlfaNumRegex", function (value, element) {
        return this.optional(element) || /^[0-9a-zA-Z\-\ \s\b]+$/i.test(value);
    }, "Service type must contain only letters.");

    $.validator.addMethod("AmountRegex", function (value, element) {
        //return this.optional(element) || /^\d{0,4}(\d\.\d?|\.\d)?\d?$/.test(value);
        return this.optional(element) || /[^0-9\.]/.test(value);
    }, "Service Base Price in Decimal only");


    $('#txtServiceName').bind('keypress', function (event) {
        var regex = new RegExp("^[0-9a-zA-Z\-\ \s\b]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });



    var validator = $('#form3').validate({

        debug: true, onsubmit: false,
        rules: {
            Service: {
                required: true,
                maxlength: 200,
                AlfaNumRegex: true,
            },

            Description: {
                //  required: false,
                maxlength: 500,
                //AlfaNumRegex: true,
            },
            ServiceBasePrice: {
                //  required: true,
                maxlength: 50,
                // AmountRegex: true,
                // digits: true
            }

        },
        messages: {
            Service: {
                required: "Service Type Required",
                maxlength: "Maxlength 200 characters",
                AlfaNumRegex: "Service type must contain only letters, numbers or dashes."
            },
            Description: {
                maxlength: "Maxlength 500 characters",
                //AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            ServiceBasePrice: {

            }
        },

        submitHandler: function (form) {

        },
        invalidHandler: function () {


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
    var listate = $('#liServices');
    listate.addClass("active");

    //#endregion

    //#region Add State button
    $("#btnAddServiceType").click(function () {

        var urlCreateNewRecord = $('#urlCreateRecord').val();

        $.ajax({
            url: urlCreateNewRecord,
            datatype: "text",
            type: "GET",
            success: function (data) {
                //debugger;
                if (data.success === true) {

                    $("#chkShowDeactivate").prop('checked', false);
                    $("#txtServiceId").val('');
                    $("#txtServiceName").val('');
                    $("#txtServiceDescription").val('');
                    $("#txtServiceBasePrice").val('');


                    $("#btnReset").show();
                    $("h4.modal-title").text("Add New Service Type");
                    $("#myModal").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });
                    validator.resetForm();
                    $('#myModal').on('shown.bs.modal', function () {
                        $('#txtServiceName').focus();
                    })
                    $("#chkTakeAudio").attr("checked", false);
                    $("#chkTakeVideo").attr("checked", false);
                    $("#txtMaxAudio").val(0);
                    $("#txtMaxVideo").val(0);
                    GetPictureList();
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
        $("#txtServiceId").val('');
        $("#txtServiceName").val('');
        $("#txtServiceDescription").val('');
        $("#txtServiceBasePrice").val('');

        $("#myModal").modal('hide');
        $("#chkTakeAudio").attr("checked", false);
        $("#chkTakeVideo").attr("checked", false);
        $("#txtMaxAudio").val(0);
        $("#txtMaxVideo").val(0);
        GetPictureList();
    })
    //#endregion

    //#region Reset button on Create Model
    $("#btnReset").click(function () {

        validator.resetForm();
        $("#txtServiceId").val('');
        $("#txtServiceName").val('');
        $("#txtServiceDescription").val('');
        $("#txtServiceBasePrice").val('');
        $("#txtMeterSize").focus();
        $("#chkTakeAudio").attr("checked", false);
        $("#chkTakeVideo").attr("checked", false);
        $("#txtMaxAudio").val(0);
        $("#txtMaxVideo").val(0);
        RemoveErrorClass();
        GetPictureList();

    })
    //#endregion

    $("#btnclose").click(function () {
        debugger;
        validator.resetForm();
        $("#txtServiceId").val('');
        $("#txtServiceName").val('');
        $("#txtServiceDescription").val('');
        $("#txtServiceBasePrice").val('');

        $("#txtMeterSize").focus();
        $("#chkTakeAudio").attr("checked", false);
        $("#chkTakeVideo").attr("checked", false);
        $("#txtMaxAudio").val(0);
        $("#txtMaxVideo").val(0);
    })


    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddServiceType").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddServiceType").show();
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
        var ID = $("#activationID").val();
        ActivateRecords(ID);
    })
    //#endregion

    //#region Activate Record No button
    $("#btnActivatNo").click(function () {

        $("#confirmActivatedialog").modal('hide');
    })
    //#endregion


    $('#chkTakeAudio').change(function () {

        if ($("#chkTakeAudio").prop('checked') == true) {

            $('#chkTakeAudio').val(true);
            $("#txtMaxAudio").val(1);
            var a = $('#chkTakeAudio').val();
        }
        else {
            $('#chkTakeAudio').val(false);
            $("#txtMaxAudio").val(0);
        }
    });

    $('#chkTakeVideo').change(function () {

        if ($("#chkTakeVideo").prop('checked') == true) {

            $('#chkTakeVideo').val(true);
            $("#txtMaxVideo").val(1);
            var a = $('#chkTakeVideo').val();
        }
        else {
            $('#chkTakeVideo').val(false);
            $("#txtMaxVideo").val(0);
        }
    });

    //#region KeyPress Amount Validation 
    $(".two-decimals").on("keypress", function (evt) {
        var $txtBox = $(this);
        var charCode = (evt.which) ? evt.which : event.keyCode;
        if (charCode == 8 || charCode == 46)
            return true;
        if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
            return false;
        else {
            var len = $txtBox.val().length;
            var index = $txtBox.val().indexOf('.');
            var val = $txtBox.val();
            if (index > 0 && charCode == 46) {
                return false;
            }
            if (index > 0) {
                //  $txtBox.val($txtBox.val().toFixed(2));

                var arr = $txtBox.val().split('.');
                var length = arr[1].length;
                if (length >= 30) {
                    return false;
                }

            }
        }
        return $txtBox; //for chaining
    });
    //#endregion

});

function RemoveErrorClass() {

    $("#txtServiceName").removeClass("error");
    $("#txtServiceDescription").removeClass("error");
    $("#txtServiceBasePrice").removeClass("error");
    $("#txtServiceName-error").val('');
    $("#txtMeterMakeDescription-error").val('');
    $("#txtServiceBasePrice-error").val('');
}

document.getElementById('txtServiceBasePrice').onkeypress = function (e) {

    if (e.keyCode === 46 && this.value.split('.').length === 2) {
        return false;
    }
}

function EditRecord() {

    if ($("#chkTakeAudio").attr("checked")) {
        $("#chkTakeAudio").attr("checked", true);
        $("#chkTakeAudio").val(true);
        $("#txtMaxAudio").val(1);
    }
    else {
        $("#chkTakeAudio").attr("checked", false);
        $("#chkTakeAudio").val(false);
        $("#txtMaxAudio").val(0);
    }
    if ($("#chkTakeVideo").attr("checked")) {
        $("#chkTakeVideo").attr("checked", true);
        $("#chkTakeVideo").val(true);
        $("#txtMaxVideo").val(1);
    }
    else {
        $("#chkTakeVideo").attr("checked", false);
        $("#chkTakeVideo").val(false);
        $("#txtMaxVideo").val(0);
    }


    var objServicePictureList = $('#ddlPicture').val();
    var uri = $('#urlEditData').val();
    var model = $("#form3").serialize();
    $.ajax({
        url: uri,
        data: (model ? model + "&" : "") + "objServicePictureList=" + objServicePictureList,
        traditional: true,

        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable();
                $("#txtServiceId").val('');
                $("#txtServiceName").val('');
                $("#txtServiceDescription").val('');


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

    var urlActiveRecords = $("#ServicesListUrl").val();
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
                          { "mData": "Service" },
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
    $("h4.modal-title").text("Activate Service Type");
}

function ActivateRecords(ID) {
    debugger;
    var urlDeleteData = $('#urlDeleteData').val();
    var uri = urlDeleteData + '?id=' + ID + '&msg=' + "Activate";
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

                var $toast = toastr["error"](msg.returnMessage, "Notification");
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
    $("h4.modal-title").text("Deactivate Service Type");
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

                var $toast = toastr["error"](msg.returnMessage, "Notification");
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
                           { "mData": "Service" },

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
    $("#txtServiceName").removeClass("error");
    $("#txtServiceName-error").remove();
    $("#txtServiceName-error").val('');
    $("#txtServiceId").val('');
    $("#txtServiceDescription").val('');
}

//#endregion


function Edit(ID) {
    debugger;
    var urlGetData = $('#urlGetData').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { ID: ID },
        success: function (data) {

            if (data.success === undefined) {

                GetPictureList();
                $("#txtServiceId").val(data.modelData.ID);
                $("#txtServiceName").val(data.modelData.Service);
                $("#txtServiceDescription").val(data.modelData.Description);
                $("#txtServiceBasePrice").val(data.modelData.ServiceBasePrice);

                if (data.modelData.AudioRequired == true) {
                    $("#chkTakeAudio").attr("checked", true);
                    $('#chkTakeAudio').val(true);
                    $("#txtMaxAudio").val(1);
                }
                else {
                    $("#chkTakeAudio").attr("checked", false);
                    $('#chkTakeAudio').val(false);
                    $("#txtMaxAudio").val(0);
                }

                if (data.modelData.VideoRequired == true) {
                    $("#chkTakeVideo").attr("checked", true);
                    $('#chkTakeVideo').val(true);
                    $("#txtMaxVideo").val(1);
                }
                else {
                    $("#chkTakeVideo").attr("checked", false);
                    $('#chkTakeVideo').val(false);
                    $("#txtMaxVideo").val(0);
                }

                if (data.ModelCD.length > 0) {



                    $.each(data.ModelCD, function (index, item) {
                        debugger;
                        $("#ddlPicture").multiselect().find(":checkbox[value='" + item.PictureId + "']").attr("checked", "checked")
                        $("#ddlPicture option[value='" + item.PictureId + "']").attr("selected", 1);

                    });
                    $("#ddlPicture").multiselect('rebuild');
                }
                else {
                    debugger;

                    GetPictureList();
                    $("#ddlPicture").multiselect("refresh");

                }


                $("#btnReset").hide();
                $("h4.modal-title").text("Edit Service Type");
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


// Added Vishal


function setSelectedIndex(s, v) {
    debugger;

    for (var i = 0; i < s.options.length; i++) {

        if (s.options[i].text == v[i].PictureName) {

            s.options[i].selected = true;

            //return;

        }

    }

}

function GetPictureList() {

    debugger;
    var urlGetPictureList = $('#urlGetPictureList').val();
    $.ajax({
        type: 'GET',
        url: urlGetPictureList,
        async: false,
        success: function (data) {
            debugger;
            $('#ddlPicture').empty();
            $.each(data, function (index, item) {
                debugger;
                $('#ddlPicture').append(
                       $('<option></option>').val(item.ID).html(item.Picture)
                 );
            });




            $('#ddlPicture').multiselect(
       {
           includeSelectAllOption: true,
           enableFiltering: true,
           enableCaseInsensitiveFiltering: true
       })
            $("#ddlPicture").multiselect("refresh");
            spinner.stop(target);
        },
        error: function () {
            alert("something seems wrong");
        }

    });
}