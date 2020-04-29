var spinner, opts, target;
var oTable;
var count = 1;
var EditMobile;
$(document).ready(function () {
    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');

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

    //GetCity();


    //#region Save State button
    $("#btnSave").click(function () {
        debugger;
        $("#txtClientName").focus();
        if ($('#form3').valid()) {
            // spinner.spin(target);
            EditRecord();

        }


    })
    //#endregion

    //#region validation code
    $.validator.addMethod("AlfaNumRegex", function (value, element) {
        return this.optional(element) || /^[0-9a-zA-Z\-\ \s\b]+$/i.test(value);
    }, "must contain only letters, numbers or dashes.");

    $.validator.addMethod("CharacterRegex", function (value, element) {
        return this.optional(element) || /^[a-zA-Z\ \s\b]+$/i.test(value);
    }, "must contain only letters dashes.");

    $.validator.addMethod("PinCodeRegex", function (value, element) {
        return this.optional(element) || /^[0-9]+$/i.test(value);
    }, "must contain only digits");

    $.validator.addMethod("MobileNoRegex", function (value, element) {
        return this.optional(element) || /^\+?\d+(-\d+)*$/i.test(value);
    }, " must contain only numbers");

    $.validator.addMethod("PhoneNoRegex", function (value, element) {
        return this.optional(element) || /^[0-9\-]+$/i.test(value);
    }, "must contain only  numbers.");




    $('#txtClientName').keyup(function () {
        var max = 500;
        var len = $(this).val().length;
        if (len >= max) {
            $('#charNum').text(' you have reached the limit');
        } else {
            var char = max - len;
            $('#charNum').text(char + ' characters left');
        }
    });


    $('#txtClientName').bind('keypress', function (event) {
        $(this).val($(this).val().replace(/^\s+/, ""));
        var regex = new RegExp("^[0-9a-zA-Z\-\ \s\b]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 43 || regex.test(key)) {
            return true;
        }
        return false;
    });

    $.validator.addMethod("EmailRegex", function (value, element) {
        return this.optional(element) || /^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$/.test(value);
    }, "Please enter a valid Email ID");


    $('.numericOnly').bind('keypress', function (event) {
        $(this).val($(this).val().replace(/^\s+/, ""));
        var regex = new RegExp("^[0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 43 || regex.test(key)) {
            return true;
        }
        return false;
    });



    $('#txtClientName').keyup(function () {
        var maxLength = 200;
        var text = $(this).val();
        var textLength = text.length;
        if (textLength > maxLength) {
            $(this).val(text.substring(0, (maxLength)));
            var $toast = toastr["error"]('Sorry, Only 200 characters are allowed', 'Error Notification');
            return false;
        }
        else {
            //alert("Required Min. 500 characters");
        }
    });


    var validator = $('#form3').validate({
        debug: true, onsubmit: false,
        rules: {
            ClientName: {
                required: true,
                maxlength: 200,
                AlfaNumRegex: true,
            },

            Address1: {
                required: true,
                maxlength: 200,
                //   AlfaNumRegex: true,
            },
            Address2: {
                //   required: true,
                maxlength: 200,
                //   AlfaNumRegex: true,
            },
            Address3: {
                //   required: true,
                maxlength: 200,
                //   AlfaNumRegex: true,
            },
            City: {
                //    required: true,
                //    maxlength: 250,
                //  AlfaNumRegex: true,
            },
            State: {
                //   required: true,
                //  maxlength: 250,
                //   AlfaNumRegex: true,
            },
            Pincode: {
                //  required: true,
                maxlength: 5,
                minlength: 5,
                PinCodeRegex: true,
            },
            MobileNumber: {
                // required: true,
                maxlength: 15,
                minlength: 10,
                MobileNoRegex: true,
            },
            PhoneNumber: {
                //  required: true,
                maxlength: 15,
                minlength: 10,
                PhoneNoRegex: true,
            },
            ClientEmailId: {
                //   required: true,
                maxlength: 100,
                EmailRegex: true
            },

        },
        messages: {
            ClientName: {
                required: "Customer Name Required.",
                //maxlength: "Maxlength 200 characters.",
                AlfaNumRegex: "Meter Make must contain only letters, numbers or dashes."
            },
            Address1: {
                required: "Address1 Required.",
                maxlength: "Maxlength 200 characters.",
                //  AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            Address2: {
                //  required: "Address2 Required.",
                maxlength: "Maxlength 200 characters.",
                //   AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            Address3: {
                //  required: "Address2 Required.",
                maxlength: "Maxlength 200 characters.",
                //   AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            City: {
                //  required: "City Required.",
                //  maxlength: "Maxlength 250 characters.",
                //  AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            State: {
                //  required: "State Required.",
                //  maxlength: "Maxlength 250 characters.",
                //  AlfaNumRegex: "Description must contain only letters, numbers or dashes."
            },
            Pincode: {
                //  required: "Pincode Required.",
                maxlength: "Pincode 5 digits.",
                minlength: "Pincode 5 digits.",
                PinCodeRegex: "Pincode must contain only digit."
            },
            MobileNumber: {
                // required: "Mobile Number Required.",
                maxlength: "Maxlength 15 characters.",
                minlength: "Minlength 10 characters.",
                MobileNoRegex: "Mobile Number must contain only  numbers."
            },
            PhoneNumber: {
                // required: "Phone Number Required.",
                maxlength: "Maxlength 15 characters.",
                minlength: "Minlength 10 characters.",

                PhoneNoRegex: "Phone Number must contain numbers and dash."
            },
            ClientEmailId: {
                // required: "Email-ID Required.",
                maxlength: "Maxlength 100 characters.",
                EmailRegex: "Please enter a valid Email ID"
            },
        },

        submitHandler: function (form) {

            //EditRecord();
            //$("#element_to_pop_up").dialog("close");
            //return false;
        },
        invalidHandler: function () {


        }
    });



    var validator1 = $('#form4').validate({
        debug: true, onsubmit: false,
        rules: {

            FirstName:
                {
                    required: true,
                    maxlength: 200,
                    AlfaNumRegex: true
                },
            LastName: {
                maxlength: 200,
                AlfaNumRegex: true
            },
            MobileNumber: {
                // required: true,
                maxlength: 15,
                minlength: 10,
                MobileNoRegex: true,
                //   AlfaNumRegex: false,
            },

            EmailId: {
                //   required: true,
                maxlength: 100,
                EmailRegex: true
            },

        },
        messages: {
            FirstName: {
                required: "First Name Required.",
                maxlength: "Maxlength 200 characters.",
                AlfaNumRegex: "First Name must contain only letters, numbers or dashes."
            },
            LastName: {
                maxlength: "Maxlength 200 characters.",
                AlfaNumRegex: "Last Name must contain only letters, numbers or dashes."


            },
            MobileNumber: {
                //required: "Mobile Number Required.",
                maxlength: "Maxlength 15 characters.",
                minlength: "Minlength 10 characters.",
                MobileNoRegex: "Mobile Number must contain only  numbers."
            },
            ClientEmailId: {
                // required: "Email-ID Required.",
                maxlength: "Maxlength 100 characters.",
                EmailRegex: "Please enter a valid Email ID"
            },
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

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liClient');
    listate.addClass("active");

    //#endregion

    //#region Add Client
    $("#btnAddCLient").click(function () {

        waitingDialog.show('Loading Please Wait...');

        var UrlCreateCustomer = $('#UrlCreateCustomer').val();

        $.ajax({
            url: UrlCreateCustomer,
            datatype: "text",
            type: "GET",
            success: function (data) {

                if (data.success === true) {
                    GetState('', '');
                    $('#ddlState').find('option:first').attr('selected', 'selected');
                    $('#ddlCity').find('option:first').attr('selected', 'selected');

                    $('#ddlState').trigger('chosen:updated');
                    $('#ddlCity').trigger('chosen:updated');
                    $("#chkShowDeactivate").prop('checked', false);
                    $("#btnReset").show();
                    $("h4.modal-title").text("Add New Customer");
                    validator.resetForm();
                    validator1.resetForm();

                    $('#myModal').on('shown.bs.modal', function () {
                        $('#txtClientName').focus();
                    })
                    $("#myModal").modal({ show: true, backdrop: 'static', keyboard: false });
                    TmpListRemove(0);
                    $("#btnReset").show();
                    RemoveErrorClass();
                    ClearClient();
                    resetForm();
                    ClearContactClient();
                    count = 1;
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Notification !");
                }
                waitingDialog.hide();
            },
            error: function (request, status, error) {

                var $toast = toastr["error"](request.responseText, "Notification !");
                console.log(request.responseText)
                waitingDialog.hide();
            }
        });



    })
    //#endregion

    //#region Cancel button on Create Model
    $("#btnCancel").click(function () {
        debugger;
        waitingDialog.show('Loading Please Wait...');
        validator.resetForm();
        validator1.resetForm();
        $('#ddlState').find('option:first').attr('selected', 'selected');
        $('#ddlCity').find('option:first').attr('selected', 'selected');
        $('#txtClientName').focus();
        $("#myModal").modal('hide');
        //$("#ddlState").multiselect("rebuild");
        //$("#ddlCity").multiselect("rebuild");

        $('#ddlState').trigger('chosen:updated');
        $('#ddlCity').trigger('chosen:updated');
        ClearContactClient();

        RemoveErrorClass();
        TmplistClear();
        // GetState('', '');
        //GetCity();
        waitingDialog.hide();
    })
    //#endregion

    //#region Reset button on Create Model
    $("#btnReset").click(function () {
        debugger;
        validator.resetForm();
        validator1.resetForm();
        $('#txtClientName').focus();
        $('#ddlState').find('option:first').attr('selected', 'selected');
        $('#ddlCity').find('option:first').attr('selected', 'selected');
        $('#ddlState').prop('disabled', false);
        $('#ddlCity').empty().prop('disabled', false);
        //$("#ddlState").multiselect("rebuild");
        //$("#ddlCity").multiselect("rebuild");
        $('#ddlState').trigger('chosen:updated');
        $('#ddlCity').trigger('chosen:updated');
        ClearClient();
        resetForm();
        // GetState('', '');
        ClearContactClient();
        RemoveErrorClass();
        TmplistClear();
        TmpListRemove(0);
        count = 1;

    })
    $("#btnClose").click(function () {
        debugger;
        // $('#ddlCity').val(0).attr("selected", "selected");
        $('#ddlState').find('option:first').attr('selected', 'selected');
        $('#ddlCity').find('option:first').attr('selected', 'selected');
        validator.resetForm();
        validator1.resetForm();
        $('#txtClientName').focus();
        //$("#ddlState").multiselect("rebuild");
        //$("#ddlCity").multiselect("rebuild");
        $('#ddlState').trigger('chosen:updated');
        $('#ddlCity').trigger('chosen:updated');
        //$('#ddlState').empty();
        ClearClient();
        resetForm();
        ClearContactClient();
        RemoveErrorClass();
        TmplistClear();
        TmpListRemove(0);
    })
    //#endregionEditContactDetails btnAddCLient

    $("#btnAddCPerson").click(function () {
        debugger;
        if ($('#form4').valid()) {
            //   spinner.spin(target);
            //  EditRecord();


            if (EditMobile != "" && EditMobile != undefined) {
                var id = EditMobile
                TmpListRemove(id);
                bindCategoryTable();
                ClearContactClient();
                EditMobile = "";
            }
            else {
                bindCategoryTable();
                ClearContactClient();
                spinner.stop(target);
            }


        }
    });
    $("#btnclose").click(function () {
        debugger;
        validator.resetForm();
        validator1.resetForm();
        $('#txtClientName').focus();
    })

    //#region show Deactivated State Checkbox
    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddCLient").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddCLient").show();
            bindActiveTable();
        }
    });
    //#endregion

    //#region Deactivate Record Yes button
    $("#btnDeactivateYes").click(function () {
        //  spinner.spin(target);
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
        //  spinner.spin(target);
        var id = $("#activationID").val();
        ActivateRecords(id);
    })
    //#endregion

    //#region Activate Record No button
    $("#btnActivatNo").click(function () {

        $("#confirmActivatedialog").modal('hide');
    })
    //#endregion

    $("#ddlState").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlCity").chosen({ no_results_text: "Oops, nothing found!" });

});






function EditRecord() {

    debugger;
    // waitingDialog.show('Loading Please Wait...');
    var value = $("#ddlState option:selected")
    $('#txtStateID').val(value.val());
    var va = $("#ddlCity option:selected")
    $('#txtCityID').val(va.val());
    var uri = $('#UrlCreateCustomer').val();

    $.ajax({
        url: uri,
        data: $("#form3").serialize(),
        type: 'POST',
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            if (msg.success == true) {
                $("#myModal").modal('hide');
                bindActiveTable();
                ClearClient();
                resetForm();
                ClearContactClient();


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
    //waitingDialog.hide();
}
$.extend($.fn.dataTable.defaults, {
    "searching": false,
    "ordering": false
});
function bindActiveTable() {
    //spinner.spin(target);
    //waitingDialog.show('Loading Please Wait...');
    debugger;


    var urlActiveRecords = $("#UrlClientList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
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
                          { "mData": "ClientName" },
                           { "mData": "CityName" },
                            { "mData": "PhoneNumber" },
                             { "mData": "MobileNumber" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.ClientId + ' onclick="Edit(' + full.ClientId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit">' +
                                                          '<i class="fa fa-pencil"></i></a> ' +
                                                          '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.ClientId +
                                                          ' onclick="Deactivate(' + full.ClientId + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a> &nbsp;' +
                                                          '<a href="#" id=' + full.ClientId + ' title="Audit Settings" onclick="EditAuditSetting(' + full.ClientId + ')" >  <i class="fa fa-pencil"></i></a>';
                                                      //Url.Action("CreatePerson", "Person", new { id = id });
                                                      //'<a href="' + urlAuditSettings + "/" + full.ClientId + '" id=' + full.ClientId + ' target="_self" title="Audit Settings" >  <i class="fa fa-pencil"></i></a>';
                                                      //  return '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>';

                                                  },
                                                  "aTargets": [4]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);

            //  waitingDialog.hide();
        }
    });

    //  waitingDialog.hide();
}

//#region Record Activation functions

function Activate(id) {
    debugger;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Customer");
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
    $("h4.modal-title").text("Deactivate Customer");
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
    debugger;
    var urlDeActiveRecords = $("#urlDeActiveRecords").val();
    $.ajax({
        url: urlDeActiveRecords,
        contentType: "application/json; charset=utf-8",
        //  dataType: "POST",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
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
       "aaData": msg,
       "aoColumns": [
                             { "mData": "ClientName" },
                           { "mData": "CityName" },
                            { "mData": "PhoneNumber" },
                             { "mData": "MobileNumber" },

                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      //  return '<a href="#" id=' + full.MobileNo + ' onclick="Edit(' + full.MobileNo + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.MobileNo + ' onclick="Deactivate(' + full.MobileNo + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.ClientId + ' onclick="Activate(' + full.ClientId + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';

                                                  },
                                                  "aTargets": [4]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }
    });


}



function Deactivate1(id) {
    debugger;
    $("#deactivationID1").val(id);
    $("#RemoveContactDetails").text("Remove Contact Details");
}

//#region Remove Contact Details Record Yes button Details
$("#btnDeactivateYes1").click(function () {
    debugger;
    // spinner.spin(target);
    $("#confirmDeactivatedialog1").modal('hide');
    var id = $("#deactivationID1").val();
    TmpListRemove(id);
})
//#endregion

//#region Remove Contact Details Record No button
$("#btnDeactivatNo1").click(function () {
    debugger;
    $("#confirmDeactivatedialog1").modal('hide');
})
//#endregion


// Clear and Remove Error Class 
function ClearClient() {
    $("#txtClientId").val('');
    $("#txtClientName").val('');
    $("#txtAddress1").val('');
    $("#txtAddress2").val('');
    $("#txtAddress3").val('');
    $("#txtCityID").val('');
    $("#txtStateID").val('');
    $("#txtPinCode").val('');
    $("#txtMobile").val('');
    $("#txtPhone").val('');



}

function resetForm() {
    $("#form4[type='reset']").click();
}

function ClearContactClient() {
    $("#txtFirstName").val('');
    $("#txtLastName").val('');
    $("#txtMobileNumber").val('');
    $("#txtEmailId").val('');
    $("#txtClientEmailId1").val('');
}

function RemoveErrorClass() {
    debugger;
    $("#txtClientName").removeClass("error");
    $("#txtClientName-error").remove();
    $("#txtClientName-error").val('');

    $("#txtAddress1").removeClass("error");
    $("#txtAddress1-error").remove();
    $("#txtAddress1-error").val('');

    $("#txtAddress2").removeClass("error");
    $("#txtAddress2-error").remove();
    $("#txtAddress2-error").val('');

    $("#txtClientName").removeClass("error");
    $("#txtClientName-error").remove();
    $("#txtClientName-error").val('');

    $("#txtCityID").removeClass("error");
    $("#txtCityID-error").remove();
    $("#txtCityID-error").val('');

    $("#txtStateID").removeClass("error");
    $("#txtStateID-error").remove();
    $("#txtStateID-error").val('');

    $("#txtPinCode").removeClass("error");
    $("#txtPinCode-error").remove();
    $("#txtPinCode-error").val('');

    $("#txtMobile").removeClass("error");
    $("#txtMobile-error").remove();
    $("#txtMobile-error").val('');

    $("#txtClientEmailId1").removeClass("error");
    $("#txtClientEmailId1-error").remove();
    $("#txtClientEmailId1-error").val('');

    $("#txtFirstName").removeClass("error");
    $("#txtFirstName-error").remove();
    $("#txtFirstName-error").val('');

    $("#txtMobileNumber").removeClass("error");
    $("#txtMobileNumber-error").remove();
    $("#txtMobileNumber-error").val('');

    $("#txtEmailId").removeClass("error");
    $("#txtEmailId-error").remove();
    $("#txtEmailId-error").val('');
}

// end  Clear and Remove Error Class 

function TmplistClear() {
    var uri = $('#TmpListClearUrl').val();

    $.ajax({
        url: uri,
        //data: $("#form3").serialize(),
        type: 'POST',
        //dataType: 'json',
        //data: JSON.stringify(AttributeModel),
        //contentType: "application/json; charset=utf-8",
        success: function (msg) {
            spinner.stop(target);
        }
    })
    spinner.stop(target);
}

function bindCategoryTable() {
    debugger;

    var ClientContactDetailsModel = new Object();

    ClientContactDetailsModel.TempID = count;
    ClientContactDetailsModel.FirstName = $('#txtFirstName').val();
    ClientContactDetailsModel.LastName = $('#txtLastName').val();
    ClientContactDetailsModel.MobileNumber = $('#txtMobileNumber').val();
    ClientContactDetailsModel.EmailId = $('#txtEmailId').val();


    var urlActiveRecords = $("#TmpCategoryListUrl").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(ClientContactDetailsModel),
        type: 'POST',
        success: function (msg) {

            oTable = $('#dynamic-tableCategory').dataTable(
   {
       "bInfo": false,
       "bPaginate": false,
       "bFilter": false,
       "bLengthChange": false,
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
                           { "mData": "TempID" },
                          { "mData": "FirstName" },
                         { "mData": "LastName" },
                         { "mData": "MobileNumber" },
                         { "mData": "EmailId" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [


                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.TempID + ' onclick="EditContactDetails(' + full.TempID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a><a href="#confirmDeactivatedialog1" data-backdrop="static" data-toggle="modal" id=' + full.TempID + ' onclick="Deactivate1(' + full.TempID + ');" class="btn btn-danger btn-xs" role="button" alt="Remove" title="Remove"><i class="fa fa-trash"></i></a>';


                                                  },
                                                  "aTargets": [5]

                                              },

       { "bVisible": false, "aTargets": [0] },
       ],

   }

)
            spinner.stop(target);
            count++;
        }
    });

    spinner.stop(target);
}

function TmpListRemove(id) {
    debugger;
    var urlGetData = $('#TmpListRemoveUrl').val();
    $.ajax({
        type: 'POST',
        url: urlGetData,
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        data: { id: id },
        success: function (msg) {

            oTable = $('#dynamic-tableCategory').dataTable(
   {
       "bInfo": false,
       "bPaginate": false,
       "bFilter": false,
       "bLengthChange": false,
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
                          { "mData": "TempID" },
                         { "mData": "FirstName" },
                         { "mData": "LastName" },
                         { "mData": "MobileNumber" },
                         { "mData": "EmailId" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [

                                              {
                                                  "mRender": function (data, type, full, row) {

                                                      // return '<a href="#confirmDeactivatedialog1" data-backdrop="static" data-toggle="modal" id=' + full.MobileNumber + ' onclick="Deactivate1(' + full.MobileNumber + ');" class="btn btn-danger btn-xs" role="button" alt="Remove" title="Remove"><i class="fa fa-ban"></i></a>';
                                                      return '<a href="#" id=' + full.TempID + ' onclick="EditContactDetails(' + full.TempID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a><a href="#confirmDeactivatedialog1" data-backdrop="static" data-toggle="modal" id=' + full.TempID + ' onclick="Deactivate1(' + full.TempID + ');" class="btn btn-danger btn-xs" role="button" alt="Remove" title="Remove"><i class="fa fa-ban"></i></a>';

                                                  },

                                                  "aTargets": [5]
                                              },
      { "bVisible": false, "aTargets": [0] },
       ],

   }
)
            spinner.stop(target);


        }

    });
    // spinner.stop(target);
}


function EditContactDetails(id) {
    debugger;
    // waitingDialog.show('Loading Please Wait...');
    var urlGetData = $('#EditContactDetalisUrl').val();

    $.ajax({
        type: 'GET',
        url: urlGetData,
        // contentType: "application/json; charset=utf-8",
        // dataType: "json",
        data: { id: id },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            debugger;
            // $("#txtID").val(msg.modelData.TerritoryID);

            $("#txtFirstName").val(msg.FirstName);
            $("#txtLastName").val(msg.LastName);
            $("#txtMobileNumber").val(msg.MobileNumber);
            $("#txtEmailId").val(msg.EmailId);
            EditMobile = msg.TempID;
            // spinner.stop(target);

        },
        error: function () {
            alert("something seems wrong");
        }

    });
    // waitingDialog.hide();
}


var BindState, BindCity
function Edit(ClientId) {
    GetState('', '');

    var urlGetData = $('#urlGetData').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        // contentType: "application/json; charset=utf-8",
        // dataType: "json",
        data: { id: ClientId },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.success === undefined) {

                $("#txtClientId").val(msg.modelData.ClientId);
                $("#txtClientName").val(msg.modelData.ClientName);
                $("#txtAddress1").val(msg.modelData.Address1);
                $("#txtAddress2").val(msg.modelData.Address2);
                $("#txtAddress3").val(msg.modelData.Address3);
                $("#txtCityID").val(msg.modelData.City);
                $("#txtStateID").val(msg.modelData.State);
                $("#txtPinCode").val(msg.modelData.Pincode);
                $("#txtMobile").val(msg.modelData.MobileNumber);
                $("#txtPhone").val(msg.modelData.PhoneNumber);
                $("#txtClientEmailId1").val(msg.modelData.ClientEmailId)




                $('#ddlState').val(msg.modelData.State).attr("selected", "selected");
                $('#ddlState').trigger('chosen:updated');

                GetCityByStateID(msg.modelData.State, msg.modelData.City)



                $('#ddlState').prop('disabled', false);
                $('#ddlCity').prop('disabled', false);
                // GetStateListByCityID(msg.modelData.CityName);


                $("#txtStateID").val(msg.modelData.State);
                $("#txtCityID").val(msg.modelData.City);






                $("#btnReset").hide();
                $("h4.modal-title").text("Edit Customer");
                $("#myModal").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

                count = msg.TCount + 1;
                TmpListRemove(0);
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");
            }

        },


        error: function () {
            //spinner.stop(target);
            //alert("something seems wrong");
        }

    });


}


var BindCity, BindState;

function GetState(ID, Id1) {
    debugger;
    var urlStateData = $('#StateListUrl').val();

    //$("#ddlState").multiselect().find(":radio[value='" + ID + "']").attr("checked", "checked")
    //$("#ddlState option[value='" + ID + "']").attr("selected", 1);
    //$("#ddlState").multiselect("rebuild")

    $.ajax({
        type: 'POST',
        url: urlStateData,
        async: false,
        success: function (data) {

            $('#ddlState').empty();

            StateData = data;

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlState').append(
                      $('<option></option>').val('0').html(""));

                }


                $('#ddlState').append(
                       $('<option></option>').val(item.StateID).html(item.StateName)
                 );
            });


            if (ID != '' && ID != null) {
                $('#ddlState').val(ID).attr("selected", "selected");
                BindState = '';
            }
            dropDown2 = true;
            //$('#ddlState').multiselect({
            //    includeSelectAllOption: false,
            //    enableFiltering: true,
            //    enableCaseInsensitiveFiltering: true
            //});
            $('#ddlState').trigger('chosen:updated');
            // tab1Loader();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }

    });
    if (ID != '' && ID != null) {
        GetCityByStateID(ID, Id1);

    }
    else {
        $('#ddlCity').empty();
        $('#ddlCity').append(
                 $('<option></option>').val('0').html(""));
        //$('#ddlCity').multiselect({
        //    includeSelectAllOption: false,
        //    enableFiltering: true,
        //    enableCaseInsensitiveFiltering: true
        //});
        $('#ddlCity').trigger('chosen:updated');
        dropDown3 = true;
        //tab1Loader();
    }
}


function GetCityByStateID(StateID, Id1) {
    debugger;
    //waitingDialog.show('Loading Please Wait...');
    var urlCityData = $('#CityByStateIDListUrl').val();
    $.ajax({
        type: 'GET',
        data: {
            "stateId": StateID,
            "cityName": ''
        },
        async: false,
        url: urlCityData,
        success: function (data) {
            debugger;
            $('#ddlCity').empty();

            StateData = data;

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCity').append(
                      $('<option></option>').val('0').html(""));
                }

                $('#ddlCity').append(
                       $('<option></option>').val(item.CityID).html(item.CityName)
                 );
            });


            if (Id1 != '' && Id1 != null) {
                $('#ddlCity').val(Id1).attr("selected", "selected");
                BindCity = '';
            }
            dropDown3 = true;
            //$('#ddlCity').multiselect({
            //    includeSelectAllOption: false,
            //    enableFiltering: true,
            //    enableCaseInsensitiveFiltering: true
            //});
            //$("#ddlCity").multiselect('rebuild');
            $('#ddlCity').trigger('chosen:updated');
            // tab1Loader();
        },

    });
    //waitingDialog.hide();
}



$('#ddlState').change(function () {
    debugger;
    $('#ddlCity').prop('disabled', false);
    $('#ddlCity').empty();
    var value = $("#ddlState option:selected")
    $('#txtStateID').val(value.val());
    //GetCityByStateID(value.val());
    waitingDialog.show('Please Wait Loading Cities...');
    var urlCityData = $('#CityByStateIDListUrl').val();
    $.ajax({
        type: 'GET',
        data: {
            "stateId": value.val(),
            "cityName": ''
        },
        //async: false,
        url: urlCityData,
        success: function (data) {
            debugger;
            $('#ddlCity').empty();

            StateData = data;

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCity').append(
                      $('<option></option>').val('0').html(""));
                }

                $('#ddlCity').append(
                       $('<option></option>').val(item.CityID).html(item.CityName)
                 );
            });



            $('#ddlCity').trigger('chosen:updated');
            waitingDialog.hide();
        },

    });

});


function EditAuditSetting(customerId) {


    if (customerId != null) {


        var urlAuditSettings = $("#AuditSettingsUrl").val();

       
        $.ajax({
            type: 'GET',
            url: urlAuditSettings,
            // contentType: "application/json; charset=utf-8",
            // dataType: "json",
            data: { CustomerId: customerId },
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (msg) {

                if (msg.success === undefined) {

                    window.location.href = urlAuditSettings + "?CustomerId=" + customerId;
                }
                else {
                    var $toast = toastr["error"](msg.returnMessage, "Notification !");
                }

            },
            error: function () {
                //spinner.stop(target);
                //alert("something seems wrong");
            }

        });



    }




}



