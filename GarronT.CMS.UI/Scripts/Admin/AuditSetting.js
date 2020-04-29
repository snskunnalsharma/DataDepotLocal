var oTable;
var checkBStatus;
$(document).ready(function () {
    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');


    debugger;
    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liAudit');
    listate.addClass("active");
    listate.parent().attr("style", "display: block;")
    //#endregion


    bindActiveTable(1);
});


$('#chkShowDeactivate').change(function () {

    if ($(this).is(":checked")) {

        bindActiveTable(0);
    }
    else {

        bindActiveTable(1);
    }
});

function bindActiveTable(checkBStatus) {

    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    var urlActiveRecords = $("#urlGetAuditProjectList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: { "activeStatus": checkBStatus },
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
                                              { "mData": "ProjectName" },
                                              { "mData": "ClientName" },
                                               { "mData": "Utilitytype" },
                                              { "mData": "AuditStartDate" },
                                               { "mData": "AuditEndDate" },
                                               { "bSearchable": true, "mData": null },
                           ],
                           "aoColumnDefs": [{
                               "mRender": function (data, type, full, row) {
                                   if (checkBStatus) {

                                       return '<a id=' + full.Id + ' onclick="Edit(' + full.Id + ');" class="btn btn-primary btn-xs" ' +
                                       'role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                                       '<a  data-backdrop="static" id=' + full.Id + ' ' +
                                       'onclick="Deactivate(' + full.Id + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate">' +
                                       '<i class="fa fa-ban"></i></a>' +
                           '<a href="#" id=' + full.Id + ' data-backdrop="static" ' +
                           ' onclick="CloneProject(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Clone Project" title="Clone Project"><i class="fa fa-files-o"></i></a> ';

                                   }
                                   else {
                                       return '<a data-toggle="modal" data-backdrop="static" id='
                                           + full.ProjectId + ' onclick="Activate(' + full.Id +
                                           ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                   }

                               },
                               "aTargets": [5]
                           }
                           ],

                       }), $('.dataTables_filter input').attr('maxlength', 50);
            waitingDialog.hide();
        }

    });


}


function CloneProject(auditProjectId) {


    var urlCloneProject = $("#urlCloneProject").val();


    $.ajax({
        type: 'GET',
        url: urlCloneProject,
        data: { CloneAuditId: auditProjectId },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.success === undefined) {

                location.href = $('#urlCloneProject').val() + "?CloneAuditId=" + auditProjectId;
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");
            }

        },
        error: function () {

        }

    });



}

function Activate(id) {
    debugger;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Audit Project");
    $("#confirmActivatedialog").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

function ActivateRecords(id) {
    debugger;
    var urlDeleteData = $('#urlActiveDeactivateRecord').val();
    $.ajax({
        url: urlDeleteData,
        type: 'POST',
        data: {
            "auditId": id,
            "activate": true
        },
        success: function (msg) {

            if (msg.success == undefined) {
                if (msg.Status == true) {
                    $("#confirmActivatedialog").modal('hide');
                    bindActiveTable(0);
                    var $toast = toastr["success"]("Activated the record successfully", "Notification");
                }
                else {

                    var $toast = toastr["error"]("Failed to activate the record", "Notification");
                }
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");

            }
            waitingDialog.hide();
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
    $("h4.modal-title").text("Deactivate Audit Project");

    $("#confirmDeactivatedialog").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

function DeactivateRecords(id) {
    var urlDeleteData = $('#urlActiveDeactivateRecord').val();
    $.ajax({
        url: urlDeleteData,
        type: 'POST',
        data: {
            "auditId": id,
            "activate": false
        },
        success: function (msg) {

            if (msg.success == undefined) {
                if (msg.Status == true) {
                    $("#confirmDeactivatedialog").modal('hide');
                    bindActiveTable(1);
                    var $toast = toastr["success"]("Deactivated the record successfully.", "Notification");
                }
                else {

                    var $toast = toastr["error"]("Failed to deactivate the record.", "Notification");
                }
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");

            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}


//#region Activate Record Yes button
$("#btnActivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    var id = $("#activationID").val();
    ActivateRecords(id);
})
//#endregion

//#region Activate Record No button
$("#btnActivatNo").click(function () {

    $("#confirmActivatedialog").modal('hide');
})
//#endregion


//#region Deactivate Record No button
$("#btnDeactivatNo").click(function () {

    $("#confirmDeactivatedialog").modal('hide');
})
//#endregion

//#region Deactivate Record Yes button
$("#btnDeactivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    $("#confirmDeactivatedialog").modal('hide');
    var id = $("#deactivationID").val();
    DeactivateRecords(id);

})
//#endregion

function Edit(ProjectId) {

    var urlGetData = $('#urlEdit').val() + '?auditId=' + ProjectId.toString();

    var urlEdit = $('#urlEdit').val();


    $.ajax({
        type: 'GET',
        url: urlEdit,
        data: { auditId: ProjectId },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.success === undefined) {

                location.href = urlGetData;
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");
            }

        },
        error: function () {

        }

    });
}




$("#btnAdd").click(function () {


    var urlCreate = $("#urlCreate").val();


    $.ajax({
        type: 'GET',
        url: urlCreate,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.success === undefined) {

                location.href = $("#urlCreate").val();
            }
            else {
                var $toast = toastr["error"](msg.returnMessage, "Notification !");
            }

        },
        error: function () {

        }

    });

})