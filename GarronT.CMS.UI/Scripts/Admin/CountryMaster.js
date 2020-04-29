var spinner, opts, target;

var oTable;
$(document).ready(function () {

    //spinner = new Spinner();
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
    //#endregionedit

    //#region disable first Character Space
    //$(function () {
    //    $('body').on('keydown', '#txtCountryName', function (e) {
    //        console.log(this.value);
    //        if (e.which === 32 && e.target.selectionStart === 0) {
    //            return false;
    //        }
    //    });
    //});
    //#endregion

    //#region Add Country button
    $("#btnAddCountry").click(function () {
        debugger;
        $("#txtCountryName").val('');
       
        $("#chkShowDeactivate").prop('checked', false);
        $("#txtID").val('');
        $("#btnReset").show();
        validator.resetForm();
        $("h4.modal-title").text("Add New Country");
        // $("#txtCountryName").focus();
        //$("#txtCountryName").attr("autofocus", "autofocus")
        $('#txtCountryName').trigger('blur');
        $("#txtCountryName").focus();
        $('#myModal').on('shown.bs.modal', function () {
            $('#txtCountryName').focus();
        })

    })
    //#endregion

    //#region Save Country button
    $("#btnSave").click(function () {
        debugger;
        $("#txtCountryName").focus();
        if ($('#form3').valid()) {
            spinner.spin(target);
            EditRecord();
        }
    })
    //#endregion

    //#region Cancel button on Create Model
    $("#btnCancel").click(function () {
        validator.resetForm();
        $("#myModal").modal('hide');
    })
    //#endregion

    //#region Reset button on Create Model
    $("#btnReset").click(function () {
        validator.resetForm();
        $("#txtCountryName").val('');
        $("#txtID").val('');
        $("#txtCountryName").focus();
    })
    //#endregion

    //#region show Deactivated Country Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddCountry").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddCountry").show();
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
        debugger;
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

    //#region validation code
    $.validator.addMethod("AlfaNumRegex", function (value, element) {
        return this.optional(element) || /^[a-z\s]+$/i.test(value);
    }, "Country Name must contain only letters.");

    $('#txtCountryName').bind('keypress', function (event) {

        var regex = new RegExp("^[A-Za-z\ \s\b]*$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
    var validator = $('#form3').validate({
        debug: true, onsubmit: false,
        rules: {
            CountryName: {
                required: true,
                maxlength: 50,

            }
        },
        messages: {
            CountryName: {
                required: "Country Name Required",
                maxlength: "Maxlength 50 characters",
                AlfaNumRegex: "Country Name must contain only letters ."
            }
        },

        submitHandler: function (form) {

            //EditRecord();
            //$("#element_to_pop_up").dialog("close");
            //return false;
        },
        invalidHandler: function () {

        }
    });
    //#endregion

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

    var leftmenuui = $('.menu');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liCountry');
    listate.addClass("active");
});

//used to reset the validation messages
//#region reset the validation messages
function RemoveErrorClass() {
    $("#txtCountryName").removeClass("error");
    $("#txtCountryName-error").remove();
    $("#txtCountryName-error").val('');
}

//#endregion

//#region Record Deactivate functions
function Deactivate(id) {
    $("#deactivationID").val(id);
    //$("#confirmActivatedialog").modal('show');

    $("h4.modal-title").text("Deactivate Country");
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
//#endregion

//#region Record Activation functions

function Activate(id) {

    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Country");
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
//#endregion

//#region Record Add / Edit functions

function Edit(id) {
    debugger;
    var urlGetData = $('#urlGetData').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        // contentType: "application/json; charset=utf-8",
        // dataType: "json",
        data: { id: id },
        success: function (msg) {
            $("#txtID").val(msg.CountryID);
            $("#txtCountryName").val(msg.CountryName);
            $("#btnReset").hide();
            $("h4.modal-title").text("Edit Country");
            $("#myModal").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            //$("#myModal").modal('show');
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

function EditRecord() {
    debugger;
    var uri = $('#urlEditData').val();
    debugger;
    $.ajax({
        url: uri,
        data: $("#form3").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable();
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
                $("#txtCountryName").val('');
                $("#txtID").val('');

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



//#endregion

$.extend($.fn.dataTable.defaults, {
    "searching": false,
    "ordering": false
});
//#region Bind Datatable functions
function bindActiveTable() {
    debugger;
    var urlActiveRecords = $("#CountryListUrl").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
        success: function (msg) {
            debugger
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
                          { "mData": "CountryName" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.CountryID + ' onclick="Edit(' + full.CountryID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
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

function bindDeactivateTable() {
    debugger
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
                          { "mData": "CountryName" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      //  return '<a href="#" id=' + full.MobileNo + ' onclick="Edit(' + full.MobileNo + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.MobileNo + ' onclick="Deactivate(' + full.MobileNo + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.CountryID + ' onclick="Activate(' + full.CountryID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';

                                                  },
                                                  "aTargets": [1]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }
    });

   
}


//#endregion

