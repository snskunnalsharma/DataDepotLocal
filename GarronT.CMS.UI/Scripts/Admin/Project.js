
var spinner, opts, target;
var oTable;
var checkBStatus;
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
    };


    //debugger;
    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liProject');
    listate.addClass("active");
    listate.parent().attr("style", "display: block;")
    //#endregion


    //#region Add State button
    $("#btnAddCustomized").click(function () {



        $("#btnReset").show();
        // validator.resetForm();
        $('#myModal').on('shown.bs.modal', function () {

        })

    });

    // #region Add State button
    $("#btnAddCustomized").click(function () {



        $("#btnReset").show();
        validator.resetForm();
        $('#myModal').on('shown.bs.modal')

    });


    $("#btnAddPopup1").click(function () {
        //debugger;
        var $container = $("#divCustmizedContainer");
        $('#dynamicdiv').empty();

        $container.append('<div id="answerdiv ' + $container.children().length + 1 + '" class="alert-message"> <h4 class="modal-title">New Field ' + $container.children().length + '</h4></div>');
        var urlgetRowPartial = $('#getRowPartial').val();
        $.ajax({
            url: urlgetRowPartial,
            type: "GET",
            async: false,
            success: function (data) {
                $("#dynamicdiv").append(data);
            }
        });

    });

    $("#btn_Upload").click(function () {
        debugger;
        var urlgetRowPartial = $('#getFileUploadPartial').val();
        $.ajax({
            url: urlgetRowPartial,
            type: "GET",
            async: false,
            success: function (data) {
                $("#divPartialFileUpload").append(data);
            }
        });

    });

    bindActiveTable(true);
});


$('#chkShowDeactivate').change(function () {

    if ($(this).is(":checked")) {

        bindActiveTable(false);
    }
    else {

        bindActiveTable(true);
    }
});

function bindActiveTable(checkBStatus) {

    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    var urlActiveRecords = $("#urlActiveRecords").val();
    console.log("urlActiveRecords === " + urlActiveRecords);
    $.ajax({
        url: urlActiveRecords,
        data: { "ActiveStatus": checkBStatus },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
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
                          { "mData": "CityName" },
                          { "mData": "LatLongStatus" },
                          { "mData": "stringFromDate" },
                           { "mData": "stringToDate" },
                            { "mData": "ProjectStatus" },
                           { "bSearchable": true, "mData": null },
       ],
       "aoColumnDefs": [{
           "mRender": function (data, type, full, row) {
               if (checkBStatus) {

                   var AddCloneInfo = (full.IsCloneProject == true ? '<a href="#" data-backdrop="static" ' +
                       ' class="btn btn-primary btn-xs" role="button" alt="Cloned Project" title="Cloned Project"><i class="fa fa-copyright"></i></a> ' : '');

                   if (full.ProjectStatus == 'Completed') {

                       return '<a href="#" id=' + full.ProjectId + ' onclick="alertComplete();" class="btn btn-primary btn-xs" ' +
                       'role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                       '<a href="#" data-backdrop="static" id=' + full.ProjectId + ' ' +
                       'onclick="alertComplete();" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate">' +
                       '<i class="fa fa-ban"></i></a> <a href="#" id=' + full.ProjectId + ' data-backdrop="static" ' +
                       ' onclick="alertComplete();" class="btn btn-primary btn-xs" role="button" alt="Change Status" title="Change Status"><i class="fa fa-check"></i></a> ' +
                           '<a href="#" id=' + full.ProjectId + ' data-backdrop="static" ' +
                       ' onclick="CloneProject(' + full.ProjectId + ');" class="btn btn-primary btn-xs" role="button" alt="Clone Project" title="Clone Project"><i class="fa fa-files-o"></i></a> ' + AddCloneInfo;

                   }
                   else {

                       return '<a href="#" id=' + full.ProjectId + ' onclick="Edit(' + full.ProjectId + ');" class="btn btn-primary btn-xs" ' +
                       'role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                       '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.ProjectId + ' ' +
                       'onclick="Deactivate(' + full.ProjectId + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate">' +
                       '<i class="fa fa-ban"></i></a> <a href="#ChangeStatusdialog" id=' + full.ProjectId + ' data-backdrop="static" data-toggle="modal" id=' + full.ProjectId +
                       ' onclick="changeStatus(' + full.ProjectId + ');" class="btn btn-primary btn-xs" role="button" alt="Change Status" title="Change Status"><i class="fa fa-check"></i></a> ' +
                       '<a href="#" id=' + full.ProjectId + ' onclick="EditUploadedRecords(' + full.ProjectId + ');" class="btn btn-primary btn-xs" ' +
                       'role="button" alt="Edit Uploaded Records" title="Edit Uploaded Records"><i class="fa fa-pencil"></i></a>' +
                           '<a href="#" id=' + full.ProjectId + ' data-backdrop="static" ' +
                       ' onclick="CloneProject(' + full.ProjectId + ');" class="btn btn-primary btn-xs" role="button" alt="Clone Project" title="Clone Project"><i class="fa fa-files-o"></i></a> ' + AddCloneInfo;
                   }

               }
               else {
                   return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.ProjectId + ' onclick="Activate(' + full.ProjectId + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
               }

           },
           "aTargets": [7]
       }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
            //spinner.stop(target);

            waitingDialog.hide();
        }

    });


}




function Activate(id) {
    //debugger;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Project");
}

function ActivateRecords(id) {
    //debugger;
    var urlDeleteData = $('#url_A_Or_D_Records').val();
    $.ajax({
        url: urlDeleteData,
        type: 'POST',
        async: false,
        data: {
            "ProjectId": id,
            "status": true
        },
        success: function (msg) {

            if (msg.success != undefined && msg.success == true) {
                $("#confirmActivatedialog").modal('hide');
                bindActiveTable(false);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            // spinner.stop(target);

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
    $("h4.modal-title").text("Deactivate Project");
}

function DeactivateRecords(id) {
    var urlDeleteData = $('#url_A_Or_D_Records').val();
    //var uri = urlDeleteData + '?customFieldId=' + id + '&msg=' + "Deactivate";
    $.ajax({
        url: urlDeleteData,
        type: 'POST',
        async: false,
        data: {
            "ProjectId": id,
            "status": false
        },
        success: function (msg) {
            if (msg.success != undefined && msg.success == true) {
                $("#confirmDeactivatedialog").modal('hide');
                bindActiveTable(true);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Notification");
            }
            //spinner.stop(target);

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


    var urleditProject = $('#urleditProject').val();

    $.ajax({
        url: urleditProject,
        datatype: "text",
        type: "GET",
        data: { 'ProjectId': ProjectId },
        success: function (data) {

            if (data.success === undefined) {
                location.href = $('#urleditProject').val() + "?ProjectId=" + ProjectId;
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
}


function CloneProject(ProjectId) {
    var urlCloneProject = $('#urlCloneProject').val();

    $.ajax({
        url: urlCloneProject,
        datatype: "text",
        data: { 'CloneId': ProjectId },
        type: "GET",
        success: function (data) {
            ////debugger;
            if (data.success === undefined) {
                location.href = $('#urlCloneProject').val() + "?CloneId=" + ProjectId;
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

}




function changeStatus(id) {
    $("#changeStatusID").val(id);
    $("h4.modal-title").text("Change Project Status.");
}



$("#btnChangeStatus").click(function () {
    waitingDialog.show('Please Wait...');

    $("#ChangeStatusdialog").modal('hide');
    var ProjectId = $("#changeStatusID").val();

    var urlGetData = $('#urlChangeStatus').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { "ProjectId": ProjectId },
        success: function (data) {

            if (data.success == true) {
                bindActiveTable(true);
                var $toast = toastr["success"]("Status Changed Successfully.", "Success Notification");
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification");
            }

            $("#ChangeStatusdialog").modal('hide');
            waitingDialog.hide();
        },
        error: function () {
            $("#ChangeStatusdialog").modal('hide');
            waitingDialog.hide();
            alert("something seems wrong");
        }
    });
})



$("#btnCancelChange").click(function () {

    $("#ChangeStatusdialog").modal('hide');
})



function alertComplete() {
    //debugger;
    var $toast = toastr["success"]("You can not perform any operations on completed Project.", "");

}

$("#btnNewProject").click(function () {

    var urlCreateNewRecord = $('#urlCreateProject').val();

    $.ajax({
        url: urlCreateNewRecord,
        datatype: "text",
        type: "GET",
        success: function (data) {
            //debugger;
            if (data.success === undefined) {
                location.href = $("#urlCreateProject").val();
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


function EditUploadedRecords(projectID) {
    var ProjectID = projectID;

    var urlGetUploadedDatabyProjectID = $('#urlGetUploadedDatabyProjectID').val();

    $.ajax({
        type: 'GET',
        url: urlGetUploadedDatabyProjectID,
        async: true,
        data: { "ProjectId": ProjectID },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            if (data.success === undefined) {
                location.href = $('#urlRedirectToEditUploadedData').val();
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
