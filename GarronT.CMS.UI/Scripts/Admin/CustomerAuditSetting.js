$(document).ready(function () {
    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');

    LoadAuditPercentageTable(1);
    LoadFailPercentageTable(1);
});




//#region --Load AuditPercentage Grid--
function LoadAuditPercentageTable(ActiveDeactive) {


    var customerId = $('#txtCustomerId').val();


    var urlViewRecords = $('#urlGetAuditPercentage').val();
    $.ajax({
        url: urlViewRecords,
        data: { "customerId": customerId, "activeDeactive": ActiveDeactive },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'GET',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;
            if (data.status == true) {

                oTable = $('#dynamic-table').dataTable(
                           {
                               "bFilter": true,
                               "bLengthChange": true,
                               "bDestroy": true,
                               "bserverSide": true,
                               "bDeferRender": true,
                               "oLanguage": {
                                   "sLengthMenu": '_MENU_ Records per page'
                               },

                               "aaData": data.list,
                               "aoColumns": [
                                                  { "mData": "AuditPercentage" },
                                                  { "mData": "StartDay" },
                                                  { "mData": "EndDay" },
                                                  { "bSearchable": false, "mData": null },
                               ],
                               "aoColumnDefs": [
                                                    {
                                                        "mRender": function (data, type, full, row) {
                                                            //  return '<a href="#" id=' + full.AuditId + ' onclick="Edit(' + full.AuditId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> ';
                                                            if (ActiveDeactive) {
                                                                return '<a href="#" id=' + full.AuditId + ' onclick="Edit(' + full.AuditId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.AuditId + ' onclick="Deactivate(' + full.AuditId + ',' + true + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                            }
                                                            else {
                                                                return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.AuditId + ' onclick="Activate(' + full.AuditId + ',' + true + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                            }
                                                        },
                                                        "aTargets": [3]
                                                    }
                               ],

                           }
                            ), $('.dataTables_filter input').attr('maxlength', 50);
            }
            else if (data.status == false) {
                var $toast = toastr["error"](data.returnMessage, "Error Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });

}
//#endregion



//#region --Load FailPercentage Grid--
function LoadFailPercentageTable(ActiveDeactive) {


    var customerId = $('#txtCustomerId').val();


    var urlViewRecords = $('#urlGetFailPercentage').val();
    $.ajax({
        url: urlViewRecords,
        data: { "customerId": customerId, "activeDeactive": ActiveDeactive },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'GET',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;
            if (data.status == true) {
                oTable = $('#dynamic-table1').dataTable(
                           {
                               "bFilter": true,
                               "bLengthChange": true,
                               "bDestroy": true,
                               "bserverSide": true,
                               "bDeferRender": true,
                               "oLanguage": {
                                   "sLengthMenu": '_MENU_ Records per page'
                               },

                               "aaData": data.list,
                               "aoColumns": [
                                                  { "mData": "FailPercentageStart" },
                                                    { "mData": "FailPercentageEnd" },
                                                  { "mData": "IncreaseByDays" },
                                                  { "bSearchable": false, "mData": null },
                               ],
                               "aoColumnDefs": [
                                                  {
                                                      "mRender": function (data, type, full, row) {
                                                          //return '<a href="#" id=' + full.Id + ' onclick="EditFailPercent(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> ';
                                                          if (ActiveDeactive) {
                                                              return '<a href="#" id=' + full.Id + ' onclick="EditFailPercent(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.Id + ' onclick="Deactivate(' + full.Id + ',' + false + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                          }
                                                          else {
                                                              return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.Id + ' onclick="Activate(' + full.Id + ',' + false + ' );"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                          }
                                                      },
                                                      "aTargets": [3]
                                                  }
                               ],

                           }
                            ), $('.dataTables_filter input').attr('maxlength', 50);
            }
            else if (data.status == false) {
                var $toast = toastr["error"](data.returnMessage, "Error Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });

}
//#endregion



//#region add new Record Audit Percentage
$("#btnAddCustomerPercent").click(function () {
    debugger;
    $("#txtAuditId").val('');
    $("#txtAuditPercentage").val('');
    $("#txtStartDay").val('');
    $("#txtEndDay").val('');
    $("#txtAuditCustomerId").val($('#txtCustomerId').val());
    $("#btnAuditReset").show();
    $("h4.modal-title").text("Add New Audit Percentage");


    $('#txtAuditPercentage').focus();
    $("#myAuditModal").modal({
        show: true,
        backdrop: 'static'
    });

})
//#endregion

//#region add new Record Fail Percentage
$("#btnAddFailurePercent").click(function () {
    debugger;
    $("#txtFailId").val('');
    $("#txtFailPercentageStart").val('');
    $("#txtFailPercentageEnd").val('');
    $("#txtIncreaseByDays").val('');
    $("#txtFailAuditCustomerId").val($('#txtCustomerId').val());
    $("#btnFailReset").show();
    $("h4.modal-title").text("Add New Fail Percentage");

    $("#myFailModal").modal({ show: true });
    $('#txtFailPercentage').focus();
})
//#endregion



//#region cancel Audit Percentage popup
$("#btnAuditCancel").click(function () {
    $("#txtAuditId").val('');
    $("#txtAuditPercentage").val('');
    $("#txtStartDay").val('');
    $("#txtEndDay").val('');
    $("#myAuditModal").modal('hide');
})
//endregion


//#region cancel Fail Percentage popup
$("#btnFailCancel").click(function () {
    $("#txtFailId").val('');
    $("#txtFailPercentageStart").val('');
    $("#txtFailPercentageEnd").val('');
    $("#txtIncreaseByDays").val('');
    $("#myFailModal").modal('hide');
})
//endregion


//#region clear Audit Percentage
$("#btnAuditReset").click(function () {
    $("#txtAuditId").val('');
    $("#txtAuditPercentage").val('');
    $("#txtStartDay").val('');
    $("#txtEndDay").val('');
    $("#txtAuditPercentage").focus();
})
//endregion


//#region clear Fail Percentage
$("#btnFailReset").click(function () {
    $("#txtFailId").val('');
    $("#txtFailPercentageStart").val('');
    $("#txtFailPercentageEnd").val('');
    $("#txtIncreaseByDays").val('');
    $("#txtFailPercentage").focus();
})
//endregion





//#region save Record Audit Percentage
$("#btnAuditSave").click(function () {
    debugger;

    var txt1 = $("#txtAuditPercentage").val();
    var txt2 = $("#txtStartDay").val();
    var txt3 = $("#txtEndDay").val();

    if (txt1 == "") {
        var $toast = toastr["error"]("Please enter audit percentage...!", "Notification");
    }
    else if (!$.isNumeric(txt1)) {
        var $toast = toastr["error"]("Audit Percentage must be number...!", "Notification");
    }
    else if (parseFloat(txt1) > 100) {
        var $toast = toastr["error"]("Audit Percentage must be less than 100...!", "Notification");
    }
    else if (txt2 == "") {
        var $toast = toastr["error"]("Please enter start day...!", "Notification");
    }
    else if (!$.isNumeric(txt2, 10)) {
        var $toast = toastr["error"]("Start day must be integer value...!", "Notification");
    }
    else if (parseInt(txt2, 10) < 1) {
        var $toast = toastr["error"]("Start day must be greater than 0", "Notification");
    }
    else if (txt3 == "") {
        var $toast = toastr["error"]("Please enter end day...!", "Notification");
    }
    else if (!$.isNumeric(txt3)) {
        var $toast = toastr["error"]("End day must be integer value...!", "Notification");
    }
    else if (parseInt(txt2, 10) > parseInt(txt3, 10)) {
        var $toast = toastr["error"]("Start day must be less than End day...!", "Notification");
    }
    else {
        var customerId = $("#txtAuditCustomerId").val();
        var auditId = $("#txtAuditId").val() == "" ? "0" : $("#txtAuditId").val();
        var auditPercent = $("#txtAuditPercentage").val();
        var auditStartDay = $("#txtStartDay").val();
        var auditEndDay = $("#txtEndDay").val();



        var urlViewRecords = $('#urlSaveAuditPercentage').val();
        $.ajax({
            url: urlViewRecords,
            data: JSON.stringify({ "customerId": customerId, "auditId": auditId, "auditPercent": auditPercent, "startDay": auditStartDay, "endDay": auditEndDay }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            type: 'POST',
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                debugger;
                if (data.status == true) {
                    var $toast = toastr["success"](data.resultMessage, "Notification");
                    $("#txtAuditId").val('');
                    $("#txtAuditPercentage").val('');
                    $("#txtStartDay").val('');
                    $("#txtEndDay").val('');
                    $("#myAuditModal").modal('hide');
                    LoadAuditPercentageTable(1);
                }
                else if (data.status == false) {
                    var $toast = toastr["error"](data.resultMessage, "Notification");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }
})
//#endregion

//#region save Record Fail Percentage
$("#btnFailSave").click(function () {
    debugger;


    if ($("#txtFailPercentage").val() == "") {
        var $toast = toastr["error"]("Please enter Fail Percentage...!", "Notification");
    }
    else if (!$.isNumeric($("#txtFailPercentageStart").val())) {
        var $toast = toastr["error"]("Fail % From Percentage must be number...!", "Notification");
    }
    else if (parseFloat($("#txtFailPercentageStart").val(), 10) > 100) {
        var $toast = toastr["error"]("Fail % From Percentage must be less than 100 %...!", "Notification");
    }
    else if (!$.isNumeric($("#txtFailPercentageEnd").val())) {
        var $toast = toastr["error"]("Fail % To Percentage must be number...!", "Notification");
    }
    else if (parseFloat($("#txtFailPercentageEnd").val(), 10) > 100) {
        var $toast = toastr["error"]("Fail % To Percentage must be less than 100 %...!", "Notification");
    }
    else if (parseFloat($("#txtFailPercentageEnd").val(), 10) <= parseFloat($("#txtFailPercentageStart").val(), 10)) {
        var $toast = toastr["error"]("Fail % From Percentage must be less than Fail % To ...!", "Notification");
    }
    else if ($("#txtIncreaseByDays").val() == "") {
        var $toast = toastr["error"]("Please enter Increase Audit Days...!", "Notification");
    }
    else if (!$.isNumeric($("#txtIncreaseByDays").val())) {
        var $toast = toastr["error"]("Increase Audit Days must be number...!", "Notification");
    }
    else if (parseInt($("#txtIncreaseByDays").val(), 10) < 1) {
        var $toast = toastr["error"]("Increase Audit Days must be greater than 0", "Notification");
    }
    else {
        var customerId = $('#txtFailAuditCustomerId').val();
        var FailId = $("#txtFailId").val() == "" ? "0" : $("#txtFailId").val();
        var failPercentStart = $("#txtFailPercentageStart").val();
        var failPercentEnd = $("#txtFailPercentageEnd").val();
        var IncreaseByDays = $("#txtIncreaseByDays").val();


        var urlViewRecords = $('#urlSaveFailPercentage').val();
        $.ajax({
            url: urlViewRecords,
            data: JSON.stringify({ "customerId": customerId, "Id": FailId, "failPercentStart": failPercentStart, "failPercentEnd": failPercentEnd, "increasedDays": IncreaseByDays }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            type: 'POST',
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                debugger;
                if (data.status == true) {
                    var $toast = toastr["success"](data.resultMessage, "Notification");
                    $("#txtFailId").val('');
                    $("#txtFailPercentageStart").val('');
                    $("#txtFailPercentageEnd").val('');
                    $("#txtIncreaseByDays").val('');
                    $("#myFailModal").modal('hide');

                    LoadFailPercentageTable(1);
                }
                else if (data.status == false) {
                    var $toast = toastr["error"](data.resultMessage, "Notification");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }

})
//#endregion

//#region save Record Fail Percentage
//txtAuditCustomerId
function Edit(AuditId) {

    debugger;
    var urlViewRecords = $('#urlGetAuditPercentageRecord').val();
    $.ajax({
        url: urlViewRecords,
        data: { "auditId": AuditId },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'GET',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;
            if (data.status == true) {
                $("#txtAuditId").val(data.result.AuditId);
                $("#txtAuditPercentage").val(data.result.AuditPercentage);
                $("#txtStartDay").val(data.result.StartDay);
                $("#txtEndDay").val(data.result.EndDay);
                $("#txtAuditCustomerId").val($('#txtCustomerId').val());

                $("#myAuditModal").modal({
                    show: true,
                    backdrop: 'static'
                });

                $('#txtAuditPercentage').focus();
            }
            else {
                var $toast = toastr["error"](data.resultMessage, "Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });
}

//#endregion



//#region save Record Fail Percentage
//txtFailAuditCustomerId
function EditFailPercent(FailId) {
    debugger;

    var urlViewRecords = $('#urlGetFailPercentageRecord').val();
    $.ajax({
        url: urlViewRecords,
        data: { "Id": FailId },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'GET',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;
            if (data.status == true) {
                $("#txtFailId").val(data.result.Id);
              
                $("#txtIncreaseByDays").val(data.result.IncreaseByDays);
                $("#txtFailAuditCustomerId").val($('#txtCustomerId').val());

                $("#txtFailPercentageStart").val(data.result.FailPercentageStart);
                $("#txtFailPercentageEnd").val(data.result.FailPercentageEnd);
                $("#myFailModal").modal({ show: true });

                $('#txtFailPercentage').focus();
            }
            else {
                var $toast = toastr["error"](data.resultMessage, "Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });
}

//#endregion




//#region Activate Record Yes button
$("#btnActivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');
    var isAudit = $("#deactivationType").val();
    var id = $("#activationID").val();
    ActivateDeactivateRecords(id, isAudit, true);
})
//#endregion



//#region Deactivate Record Yes button
$("#btnDeactivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    $("#confirmDeactivatedialog").modal('hide');
    var isAudit = $("#deactivationType").val();
    var id = $("#deactivationID").val();
    ActivateDeactivateRecords(id, isAudit, false);
})
//#endregion



function Activate(id, isAudit) {
    debugger;
    $("#activationID").val(id);
    $("#deactivationType").val(isAudit);
    $("h4.modal-title").text("Activate Record");
}

function ActivateDeactivateRecords(id, isAuditType, isActivateOrDeactivate) {
    debugger;
    var urlData = $('#urlActivateDeactivateRecord').val();

    $.ajax({
        url: urlData,
        data: { "Id": id, "isAuditId": isAuditType, "isActive": isActivateOrDeactivate },
        type: 'POST',
        success: function (msg) {
            if (msg.status == true) {
                $("#confirmActivatedialog").modal('hide');
                //bindActiveTable(false);
                var $toast = toastr["success"](msg.resultMessage, "Notification");
                waitingDialog.hide();

                if (isActivateOrDeactivate == true) {
                    if (isAuditType == true) {
                        LoadAuditPercentageTable(0);
                    }
                    else {
                        LoadFailPercentageTable(0);
                    }
                }
                else {
                    if (isAuditType == true) {
                        LoadAuditPercentageTable(1);
                    }
                    else {
                        LoadFailPercentageTable(1);
                    }
                }
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Notification");
            }



        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id, isAudit) {

    $("#deactivationType").val(isAudit);
    $("#deactivationID").val(id);
    $("h4.modal-title").text("Deactivate Record");
}


$('#chkShowDeactivateAudit').change(function () {

    if ($('#chkShowDeactivateAudit').prop('checked') == true) {

        LoadAuditPercentageTable(0);
    }
    else {

        LoadAuditPercentageTable(1);
    }
});


$('#chkShowDeactivateFail').change(function () {

    if ($('#chkShowDeactivateFail').prop('checked') == true) {

        LoadFailPercentageTable(0);
    }
    else {

        LoadFailPercentageTable(1);
    }
});