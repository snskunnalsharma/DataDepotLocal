$(document).ready(function () {

    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');
    $("#liUserMaster").addClass('active');


    bindActiveTable();

    $('#chkShowDeactivate').change(function () {

        if ($(this).is(":checked")) {
            $("#btnAddUser").hide();
            bindDeactivateTable();
        }
        else {
            $("#btnAddUser").show();
            bindActiveTable();
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
})


$('#btnAddUser').click(function () {
    // debugger;
    var urlCreateNewRecord = $('#urlCreateRecord').val();

    $.ajax({
        url: urlCreateNewRecord,
        datatype: "text",
        type: "GET",
        success: function (data) {
            //debugger;
            if (data.success === undefined) {
                location.href = $("#urlCreateRecord").val();
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


function bindActiveTable() {


    var urlEditUserList = $("#EditUserListUrl").val();
    var urlActiveRecords = $("#UrlUserList").val();
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
                               { "mData": "UserName" },
                                { "mData": "FirstName" },
                                { "mData": "RoleID" },
                                 { "mData": "Email" },
                                 { "mData": "MobileNo" },
                                { "bSearchable": false, "mData": null },
            ],
            "aoColumnDefs": [
                                                   {
                                                       "mRender": function (data, type, full, row) {
                                                           return '' +
                                                               '<button class="btn btn-info btn-xs" id="' + full.UserID + '" name="" title="Edit User" onclick="EditRecord(' + full.UserID + ');"><span class="fa fa-pencil"></span></button>'
                                                               //'<a target="_self" href="' + urlEditUserList + '/' + full.UserID + '" id="' + full.UserID + '" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                           + '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.UserID + ' onclick="Deactivate(' + full.UserID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>'
                                                           + '<button class="btn btn-info btn-xs" id="' + full.UserID + '" name="" title="User Access Permissions" onclick="UserPermissions(' + full.UserID + ');"><span class="fa fa-cog"></span></button>';
                                                       },
                                                       "aTargets": [5]
                                                   }
            ],

        }
), $('.dataTables_filter input').attr('maxlength', 50);
            //  spinner.stop(target);
        }
    });


}

function Activate(id) {

    $("#activationID").val(id);
    $("h4.modal-title").text("Activate User");
}

function ActivateRecords(id) {

    var urlActivateRecord = $('#urlActivateRecord').val();
    var uri = urlActivateRecord + '?id=' + id + '&msg=' + "Activate";
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
            // spinner.stop(target);

        },
        error: function (error) {
            console.log(error)
            //alert("something seems wrong");
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id) {

    $("#deactivationID").val(id);
    $("h4.modal-title").text("Deactivate User");
}

function DeactivateRecords(id) {

    var urlDeActivateRecord = $('#urlDeActivateRecord').val();
    var uri = urlDeActivateRecord + '?id=' + id + '&msg=' + "Deactivate";
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
            // spinner.stop(target);
        },
        error: function (error) {
            console.log(error)
            //alert("something seems wrong");
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
                          { "mData": "UserName" },
                           { "mData": "FirstName" },
                           { "mData": "RoleID" },
                            { "mData": "Email" },
                            { "mData": "MobileNo" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      //return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.UserID + ' onclick="Edit(' + full.UserID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.UserID + ' onclick="Deactivate(' + full.UserID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                                      return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.UserID + ' onclick="Activate(' + full.UserID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                      //  return '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>';

                                                  },
                                                  "aTargets": [5]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }
    });


}


function Deactivate1(id) {

    $("#deactivationID1").val(id);
    $("#RemoveContactDetails").text("Remove Contact Details");
}

//#region Remove Contact Details Record Yes button Details
$("#btnDeactivateYes1").click(function () {


    $("#confirmDeactivatedialog1").modal('hide');
    var id = $("#deactivationID1").val();
    TmpListRemove(id);
})
//#endregion

//#region Remove Contact Details Record No button
$("#btnDeactivatNo1").click(function () {

    $("#confirmDeactivatedialog1").modal('hide');
})
//#endregion



function UserPermissions(userId) {

    location.href = $('#urlUserPermission').val() + "?UserId=" + userId;
}




function EditRecord(userId) {
    var urlEditUser = $('#urlEditUser').val();

    $.ajax({
        url: urlEditUser,
        datatype: "text",
        data: { 'id': userId },
        type: "GET",
        success: function (data) {

            if (data.success == undefined) {
                location.href = urlEditUser + "?id=" + userId;               

            } else {
                var $toast = toastr["error"](data.returnMessage, "Error Notification");
            }
        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });

}
