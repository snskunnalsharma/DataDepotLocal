var oTable;

$(document).ready(function () {

    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');

    //#region To select Left side menu
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liRoleMaster');
    listate.addClass("active");

    //#endregion

    bindActiveTable();
    Reset();
});

//Bind Role List 
function bindActiveTable() {

    var urlActiveRecords = $("#urlGetRoleList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'GET',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (roleList) {
            oTable = $('#dynamic-table').dataTable(
                       {
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
                           "aaData": roleList.roleList,
                           "aoColumns": [
                                              { "mData": "Name" },
                                              //{ "mData": "IsAdmin" },
                                               //{ "bSearchable": false, "mData": null },
                           ],
                           //"aoColumnDefs": [
                           //                                       {
                           //                                           "mRender": function (data, type, full, row) {
                           //                                               if (roleList.success) {
                           //                                                   if (checkBStatus)
                           //                                                       return '<a href="#" id=' + full.AlertId + ' onclick="Edit(' + full.AlertId + ');" class="btn btn-primary btn-xs" role="button" alt="Add" title="Add"><i class="fa fa-pencil"></i></a>';
                           //                                               }
                           //                                           },
                           //                                           "aTargets": [2]
                           //                                       }
                           //],

                       }
                        ), $('.dataTables_filter input').attr('maxlength', 50);
            waitingDialog.hide();
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

//Reset data and set focus
function Reset() {
    $('#txtRoleName').val("");
    $("#txtRoleName").focus();
}

//Add new Role
function Add() {

    var urlAddNewRole = $('#urlAddNewRole').val();

    $.ajax({
        url: urlAddNewRole,
        data: { roleName: $('#txtRoleName').val() },
        type: 'POST',
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            if (msg.success == true) {
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
                bindActiveTable();

            } else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            Reset();
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

//Add button click function
$("#btnSave").click(function () {
    if ($('#txtRoleName').val() != "") {
        Add()
    }
    else {
        var $toast = toastr["error"]("Please Enter Role Name", "Error Notification");
    }
})
