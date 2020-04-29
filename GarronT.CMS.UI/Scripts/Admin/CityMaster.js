var spinner, opts, target;
var oTable;
var StateData;
var BindCon;
$(document).ready(function () {
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
        $('body').on('keydown', '#txtCityName', function (e) {
            console.log(this.value);
            if (e.which === 32 && e.target.selectionStart === 0) {
                return false;
            }
        });
    });
    //#endregion

    //#region Save State button
    $("#btnSave").click(function () {
        $("#txtCityName").focus();
        if ($('#form3').valid()) {
            spinner.spin(target);
            EditRecord();

        }


    })
    //#endregion

    //#region validation code
    $.validator.addMethod("AlfaNumRegex", function (value, element) {
        return this.optional(element) || /^[a-z\s]+$/i.test(value);
    }, "City Name must contain only letters.");

    //$.validator.addMethod("notEqual", function (value, element, param) {
    //    return this.optional(element) || value != "0" || value != "Select";
    //}, "Please specify a different (non-default) value");


    $('#txtCityName').bind('keypress', function (event) {
        var regex = new RegExp("^[a-zA-Z\ \s\b]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });



    var validator = $('#form3').validate({

        debug: true, onsubmit: false,
        rules: {
            CityName: {
                required: true,
                maxlength: 50,
                AlfaNumRegex: true,
            },

            State: {
                required: true,

            },

            Country: {
                required: true,

            },

        },
        messages: {
            CityName: {
                required: "City Name Required",
                maxlength: "Maxlength 50 characters",
                AlfaNumRegex: "City Name must contain only letters, numbers, or dashes."
            },
            State: {
                required: "State Name Required",
                AlfaNumRegex: "State Name Required"
            },
            Country: {
                required: "Country Name Required",
                AlfaNumRegex: "Country Name Required"

            },

            //Country: function (Country) {
            //    $(Country).removeClass("error");
            //}
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
    var listate = $('#licity');
    listate.addClass("active");

    //#endregion

    //#region Add State button
    $("#btnAddCity").click(function () {

        GetCountry();
        GetState();
        $("#chkShowDeactivate").prop('checked', false);
        $("#txtCityName").val('');
        $("#txtCountryID").val('');
        $("#txtStateID").val('');
        $("#txtID").val('');


        $("#btnReset").show();
        $("h4.modal-title").text("Add New City");
        validator.resetForm();
        $('#ddlState').prop('disabled', true);
        $('#myModal').on('shown.bs.modal', function () {
            $('#ddlCountry').focus();
        })

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


        GetCountry();
        GetState();
        validator.resetForm();

        $("#txtCityName").val('');
        $("#ddlCountry").focus();
        $('#ddlState').prop('disabled', true);

    })
    //#endregion


    $('#ddlCountry').change(function () {
        debugger;
        //GetState();
        $('#ddlState').prop('disabled', false);
        $('#ddlState').empty();
        var value = $("#ddlCountry option:selected")
        $('#txtCountryID').val(value.val());
        $('#ddlState').append(
                     $('<option></option>').val('').html("--Select--"));
        GetStateByCountryID(value.val());
        //for (var i = 0; i < StateData.length; i++) {
        //    if (StateData[i].CountryID == value.val()) {
        //        $('#ddlState').append(
        //                  $('<option></option>').val(StateData[i].StateID).html(StateData[i].StateName)
        //            );

        //    }

        //}
    });
    $('#ddlState').change(function () {
        debugger;
        var value = $("#ddlState option:selected")
        $('#txtStateID').val(value.val());
        $("#txtCityName").val('');
    });

    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddCity").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddCity").show();
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



function GetCountry() {
    var urlCountryData = $('#CountryListUrl').val();
    $.ajax({
        type: 'POST',
        url: urlCountryData,
        //   contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (data) {

            $('#ddlCountry').empty();


            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCountry').append(
                      $('<option></option>').val('').html("--Select--"));

                }


                $('#ddlCountry').append(
                       $('<option></option>').val(item.CountryID).html(item.CountryName)
                 );
            });


            if (BindCon != '' && BindCon != null) {
                $('#ddlCountry').val(BindCon).attr("selected", "selected");
                BindCon = '';
            }

        },
        //error: function () {
        //    alert("something seems wrong");
        //}
    });
}
function GetStateByCountryID(id) {
    debugger;
    var urlStateData = $('#StateByCoutryIDListUrl').val();
    $.ajax({
        type: 'POST',
        data: { Id: id },
        url: urlStateData,
        //   contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (data) {
            debugger;
            $('#ddlState').empty();

            StateData = data;

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlState').append(
                      $('<option></option>').val('').html("--Select--"));

                }


                $('#ddlState').append(
                       $('<option></option>').val(item.StateID).html(item.StateName)
                 );
            });


            if (BindState != '' && BindState != null) {
                $('#ddlState').val(BindState).attr("selected", "selected");
                BindState = '';
            }

        },
        //error: function () {
        //    alert("something seems wrong");
        //}
    });
}

function GetState() {
    var urlStateData = $('#StateListUrl').val();
    $.ajax({
        type: 'POST',
        url: urlStateData,
        //   contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (data) {

            $('#ddlState').empty();

            StateData = data;

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlState').append(
                      $('<option></option>').val('').html("--Select--"));

                }


                $('#ddlState').append(
                       $('<option></option>').val(item.StateID).html(item.StateName)
                 );
            });


            if (BindState != '' && BindState != null) {
                $('#ddlState').val(BindState).attr("selected", "selected");
                BindState = '';
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

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
                GetCountry();
                GetState();
                $("#txtCityName").val('');
                $("#txtCountryID").val('');
                $("#ddlState").val('');

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
    debugger;
    var urlActiveRecords = $("#CityListUrl").val();
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
                              { "mData": "StateName" },
                               { "mData": "CityName" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.CityID + ' onclick="Edit(' + full.CityID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CityID + ' onclick="Deactivate(' + full.CityID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      //  return '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>';

                                                  },
                                                  "aTargets": [3]
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
    $("h4.modal-title").text("Activate City");
}

function ActivateRecords(id) {
    debugger;
    var urlDeleteData = $('#urlDeleteData').val();
    var uri = urlDeleteData + '?id=' + id + '&msg=' + "Activate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {

            //$("#errorMessage").html('');
            //$("#errorMessage").css('display', 'block');
            //$("#errorMessage").text(msg.returnMessage);
            //$('#errorMessage').delay(5000).fadeOut('slow');
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
    $("h4.modal-title").text("Deactivate City");
}

function DeactivateRecords(id) {
    debugger;
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
                          { "mData": "CountryName" },
                              { "mData": "StateName" },
                               { "mData": "CityName" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      //  return '<a href="#" id=' + full.MobileNo + ' onclick="Edit(' + full.MobileNo + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.MobileNo + ' onclick="Deactivate(' + full.MobileNo + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.CityID + ' onclick="Activate(' + full.CityID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';

                                                  },
                                                  "aTargets": [3]
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
    $("#txtCityName").removeClass("error");
    $("#txtCityName-error").remove();
    $("#txtCityName-error").val('');
}

//#endregion

var BindCon, BindState;
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
            debugger;
            GetCountry();
            $('#ddlState').prop('disabled', false);
            GetStateByCountryID(msg.CountryID);

            BindCon = msg.CountryID;
            BindState = msg.StateID;
            // $("#ddlCountry").val(msg.CountryID);
            $("#txtID").val(msg.CityID);
            $("#txtCountryID").val(msg.CountryID);
            $("#txtStateID").val(msg.StateID);

            $("#txtCityName").val(msg.CityName);
            $("#btnReset").hide();
            $("h4.modal-title").text("Edit City");
            $("#myModal").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });



            $('#ddlCountry').val(msg.CountryID).attr("selected", "selected");
            $('#ddlState').val(msg.StateID).attr("selected", "selected");
        },
        error: function () {
            alert("something seems wrong");
        }

    });


    //$('#ddlCountry').val(BindCon).attr("selected", "selected");
}