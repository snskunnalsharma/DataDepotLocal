var oTable;
var checkBStatus;
var UserIDlist = new Array();
var OldUserIDlist = new Array();
var GlobalUserid;

$(document).ready(function () {

    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');

    bindActiveTable(true);


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liAlertMaster');
    listate.addClass("active");

    //#region Save State button
    $("#btnSave").click(function () {
        debugger;


        if ($('#form3').valid()) {
            if (UserIDlist.length > 0) {
                CreateRecord();
            } else {
                var $toast = toastr["error"]("At least select one user.", "Error Notification");
            }
        }

    });
    //#endregion


    var validator = $('#form3').validate({
        debug: true, onsubmit: false,
        rules: {
            Subject: {
                required: true,
                maxlength: 200,

            },


        },
        messages: {
            Subject: {
                required: "Subject  Required.",
                maxlength: "Maxlength 250 characters.",
                //   AlfaNumRegex: "Meter Make must contain only letters, numbers or dashes."
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

    //#region Save State button
    $("#btnAdd").click(function () {

        if (UserIDlist.length > 0) {

            BindUserDetails(UserIDlist);
        }
    });
    //#endregion


    //#region Cancel button on Create Model
    $("#btnCancel").click(function () {
        debugger;
        $("#txtSubject").val('');
        CKEDITOR.instances['txtMessage'].setData('');
        $("#AlertId").val('');

        $("#myModal").modal('hide');
    })

    //#region Role dropdown

    $("#uluser").selectable(
        {

            start: function (event, ui) {

                var target = event.target || event.srcElement;

                if (target.tagName == "LI") {

                }


            },
            selected: function (event, ui) {

                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            },
            unselected: function (event, ui) {

                $(ui.unselected).removeClass('click-selected');

            },
            stop: function (event, ui) {

                selectUser()
                //checkinstallerroleselect()
            }
        });

    //#endregion
});

function BindUserDetails(UserIDlist) {

    waitingDialog.show('Loading please wait...');
    var urlurlGetuserDetails = $("#urlGetuserDetails").val();
    $.ajax({
        url: urlurlGetuserDetails,
        data: JSON.stringify({ "Userid": UserIDlist }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
        success: function (msg) {

            oTable = $('#dynamic-tableCategory').dataTable(
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
                           "aaData": msg.status,
                           "aoColumns": [
                                               //{ "mData": "ProjectName" },


                                              { "mData": "FirstName" },

                                              { "mData": "MobileNumber" },
                                             { "mData": "EmailId" },

                           ],
                           "aoColumnDefs": [
                                                                  {

                                                                      "aTargets": [2]
                                                                  }
                           ],

                       }
                        ), $('.dataTables_filter input').attr('maxlength', 50);
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });


}

function filteruserlist(element) {

    var value = $(element).val().toLowerCase();

    $("#uluser > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

function Edit(ID) {


    UserIDlist = [];
    waitingDialog.show('Loading record please wait...');

    var urlGetData = $('#urlGetEditData').val();

    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { AlertID: ID },
        success: function (data) {

            if (data.success === true) {

                $("#AlertId").val(data.status.AlertId);
                $("#txtalertname").val(data.status.AlertName);

                $("#btnReset").hide();

                $("#myModal").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });


                getUserList();


                CKEDITOR.instances['txtMessage'].setData('');
                getOldData();
                waitingDialog.hide();
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
                waitingDialog.hide();
            }
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }

    });


}

function getOldData() {
    debugger;
    var GeturlurlOldData = $('#urlOldData').val();
    var Alerttableid = $('#AlertId').val();
    $.ajax({
        type: 'GET',
        url: GeturlurlOldData,
        data: { AlertID: Alerttableid },
        success: function (data) {

            debugger;
            $("#txtSubject").val('');


            if (data != null && data.status != null) {
                debugger;
                if (data.status.Subject != null) {
                    $("#txtSubject").val(data.status.Subject);
                }
                if (data.status.Message != null) {
                    // $("#txtMessage").val(data.status.Message);
                    CKEDITOR.instances['txtMessage'].setData(data.status.Message);
                }

                $("#AlertId").val(data.status.AlertTableID);


                $.each(data.status.Userid, function (index, item) {
                    debugger;
                    $("#uluser").find("li[value='" + item + "']").addClass("ui-selected click-selected highlighted")

                });
                // selectFormType();
                debugger;
                UserIDlist = data.status.Userid;


            }
            var selectedlilength = UserIDlist.length;
            if (selectedlilength == 0) {
                $("#userlistHeader").removeClass("ddlHeader").text("Select User")
            } else {
                $("#userlistHeader").addClass("ddlHeader").text(selectedlilength + " User selected")
            }
            BindUserDetails(UserIDlist);

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
        },

    });

}

function getUserList() {
    debugger;
    var GeturlUserList = $('#urlUserList').val();

    $.ajax({
        type: 'GET',
        url: GeturlUserList,
        async: false,

        success: function (data) {

            debugger;
            _ProjectList = data;
            $('#uluser').empty();

            var count = data.status.length;
            if (count > 0) {

                $.each(data.status, function (index, item) {
                    $('#uluser').append($('<li></li>').attr("Id", item.UserID).val(item.UserID).addClass("liuser active-result").html(item.FullName));

                    //if (index == 0) {
                    //    $('#ddluserlist').append(
                    //      $('<option></option>').val('0'));
                    //}
                    //$('#ddluserlist').append(
                    //                      $('<option></option>').val(item.UserID).html(item.FullName)
                    //                );

                });

            }
            // $('.chosen-select').trigger('chosen:updated');


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
        },

    });

}


function selectUser() {
    debugger;
    var selectedlivalue = $('#uluser li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#userlistHeader").removeClass("ddlHeader").text("Select User")
    } else {
        $("#userlistHeader").addClass("ddlHeader").text(selectedlilength + " User selected")
    }


    UserIDlist = new Array();
    $.each(selectedlivalue, function (index, value) {
        //if ($.inArray(value, UserIDlist) == -1) {
        //UserIDlist.push(value)
        UserIDlist.push(value)

        // }
    });
    BindUserDetails(UserIDlist);

    //var selectedlivalue = $('#uluser li.highlighted').map(function (i, el) {
    //    debugger;
    //    userid = "";
    //    var userid = $(el).val();
    //    if (UserIDlist.)
    //    UserIDlist.push(userid)
    //    BindUserDetails(UserIDlist);
    //    return $(el).val();
    //});




}

$("#ancUser_chosen").click(function () {
    $("#ddluserlist").toggleClass("chosen-with-drop chosen-container-active")
});
$('body').on('click', function (e) {
    if (!$('#divuserlist').is(e.target)
       && $('#divuserlist').has(e.target).length === 0
   ) {
        $('#ddluserlist').removeClass('chosen-with-drop chosen-container-active');
    }
});

function bindActiveTable(checkBStatus) {
    debugger;
    waitingDialog.show('Loading please wait...');
    var urlActiveRecords = $("#urlAlertList").val();
    $.ajax({
        url: urlActiveRecords,
        data: { "ActiveStatus": checkBStatus },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'GET',
        success: function (msg) {
            debugger;
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
                           "aaData": msg.status,
                           "aoColumns": [
                                               //{ "mData": "ProjectName" },

                                              { "mData": "AlertName" },

                                               { "bSearchable": false, "mData": null },
                           ],
                           "aoColumnDefs": [
                                                                  {
                                                                      "mRender": function (data, type, full, row) {
                                                                          if (msg.success) {
                                                                              if (checkBStatus)
                                                                                  return '<a href="#" id=' + full.AlertId + ' onclick="Edit(' + full.AlertId + ');" class="btn btn-primary btn-xs" role="button" alt="Add" title="Add"><i class="fa fa-pencil"></i></a>';


                                                                          }
                                                                      },
                                                                      "aTargets": [1]
                                                                  }
                           ],

                       }
                        ), $('.dataTables_filter input').attr('maxlength', 50);
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });


}


function UpdateRecord() {

    var uri = $('#urlUpdate').val();
    var _categoryType = $("#ddlReasonType option:selected").text();
    var _Reason = $("#txtReason").val();
    var _Id = $("#SkipId").val();
    waitingDialog.show('Updating please wait...');

    $.ajax({
        url: uri,
        data: { "skipId": _Id, "_Reason": _Reason, "_categoryType": _categoryType },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable(true);
                $("#txtReasoneId").val('');
                $("#ddlReasoneType").text('');
                $("#txtReason").val('');
                $("#SkipId").val('');
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}


function CreateRecord() {
    debugger;
    var uri = $('#urlCreateData').val();
    var subject = $("#txtSubject").val();

    // var Message = $("#txtMessage").val();
    var AlertTableID = $("#AlertId").val();
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances['txtMessage'].updateElement();

    var Message = $("#txtMessage").val();

    var _EmailConfigureBO = {
        "subject": subject,
        "Message": Message,
        "AlertTableID": AlertTableID,
        "Userid": UserIDlist,
    }



    waitingDialog.show('Saving record please wait...');

    $.ajax({
        url: uri,
        data: JSON.stringify(_EmailConfigureBO),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        type: 'POST',

        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');

                $("#txtSubject").text('');
                //$("#txtMessage").text('');
                CKEDITOR.instances['txtMessage'].setData('');

                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

function selectFormType() {

    var selectedlivalue = $('#uluser li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#userlistHeader").removeClass("ddluserlist").text("Select User")
    } else {
        $("#userlistHeader").addClass("ddluserlist").text(selectedlilength + " User selected")
    }



}