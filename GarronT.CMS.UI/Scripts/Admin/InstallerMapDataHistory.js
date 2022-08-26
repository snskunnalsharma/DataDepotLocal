/*
 * -------------------------------------------------------------------------------
 * Create By     : Aniket Jadhav 
 * Created On    : 07-Sep-2016 
 * Description   : Installer Re-Assign (Skipped) Javascript code
 *--------------------------------------------------------------------------------
 */

//#region --Global Variables--

var oTable;

//#endregion

//#region --Document Ready Function
$(document).ready(function () {

    var actualDate = new Date(); // convert to actual date
    var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());

    $('#datepicker1').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,
        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onSelect: function (selectedDate) {
            //$("#datepicker1").datepicker("option", "minDate", selectedDate);
            var dt = new Date(selectedDate);
            dt.setDate(dt.getDate());
            $("#datepicker2").datepicker("option", "minDate", dt);

        }
    }).datepicker("setDate", actualDate);


    $('#datepicker2').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,

        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onSelect: function (selectedDate) {
            // $("#datepicker").datepicker("option", "maxDate", selectedDate);
            var dt = new Date(selectedDate);
            dt.setDate(dt.getDate());
            $("#datepicker1").datepicker("option", "maxDate", dt);
        }
    }).datepicker("setDate", actualDate);

    //$('#dynamic-table').DataTable().clear().draw();
    $('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" } });
    //#region --Make Menu Active--
    $("#Reports").addClass('active');
    $("#subReports").addClass('block');

    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liInstallerMapHistory');
    listate.addClass("active");
    //#endregion

    //#region --Dropdowns--
    $('#ddlProjectStatus').append($('<option></option>').val(''));
    $("#ddlProjectStatus").chosen({ no_results_text: "Oops, nothing found!" });

    $('#ddlProject').append($('<option></option>').val('0'));
    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });

    $('#ddlInstaller').append($('<option></option>').val('0'));
    $("#ddlInstaller").chosen({ no_results_text: "Oops, nothing found!" });

    $('#ddlSkipReason').append($('<option></option>').val('0'));
    $("#ddlSkipReason").chosen({ no_results_text: "Oops, nothing found!" });
    //#endregion

    // LoadActiveTable();

});
//#endregion

//#region --Load ActiveData Grid--
function LoadActiveTable() {
    //$('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" } });
    var _projectID = $('#ddlProject').val();
    var _installerID = $('#ddlInstaller').val();
    var _skipReason = $('#ddlSkipReason').val();
    var datepicker1 = $("#datepicker1").val();
    var datepicker2 = $("#datepicker2").val();
    debugger;
    var urlActiveRecords = $("#urlGetInstallerMapList").val();
    var urlViewMultiSkippedRecords = $('#urlViewMultiSkippedRecords').val();
    $.ajax({
        url: urlActiveRecords,
        data: { "projectID": _projectID, "installerID": _installerID, "SkipReason": _skipReason, "FromDate": datepicker1, "ToDate": datepicker2 },
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
            if (data.success == true) {
                oTable = $('#dynamic-table').dataTable(
                           {
                               "bFilter": true,
                               "bLengthChange": true,
                               "bDestroy": true,
                               "bserverSide": true,

                               "bDeferRender": true,
                               //"sAjaxDataProp": 'aData',
                               "oLanguage": {
                                   "sLengthMenu": '_MENU_ Records per page'
                               },
                               //"sAjaxSource": msg.result,
                               "aaData": data.installerMapHistoryList,
                               "aoColumns": [

                                                  { "mData": "VisitedDate" },
                                                  { "mData": "projectname" },
                                                  { "mData": "InstallerName" },
                                                  { "mData": "Account" },
                                                  { "mData": "Street" },
                                                  { "mData": "Cycle" },
                                                  { "mData": "Route" },
                                                  { "mData": "SkippedReason" },
                                                  { "mData": "SkipComment" },
                                                  { "bSearchable": false, "mData": null },
                               ],
                               "aoColumnDefs": [
                                                    {
                                                        "mRender": function (data, type, full, row) {
                                                            //return '<a href="' + urlViewMultiSkippedRecords + "/" + full.ProjectFieldDataId + '" id=' + full.ProjectFieldDataId + ' target="_blank" title="View" >  <i class="fa fa-file-text"></i></a>';
                                                            return '<a  id=' + full.WarehouseId + ' onclick="showDetail(' + _projectID + ',' + full.ID + ',\'' + full.Account + '\');" class="btn btn-primary btn-xs" role="button" alt="Skip Account History" title="Inventory Transfer"><i class="fa fa-history"></i></a>';
                                                            //return '<a href="#" id=' + full.ID + '  onclick="window.open("<%= Url.Action("ViewSkippedRecords",  "InstallerMapDataHistory", new { id = ' + full.ID + ' } ) %>", "_blank"); return false;"  title="View" >  <i class="fa fa-file-text"></i></a>';
                                                        },
                                                        "aTargets": [9]
                                                    }
                               ],

                           }
                            ), $('.dataTables_filter input').attr('maxlength', 50);
            }
            else {

                var $toast = toastr["error"](data.returnMessage, "Error Notification");

            }
        },
        error: function (jqXHR, textStatus, errorThrown)
        {
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

// Load data in Model Skip account detail 
function showDetail(projectID, AccountId, AccountNo) {
    var _projectID = $('#ddlProject').val();
    debugger;
    waitingDialog.show('Loading Please Wait...');
    var urlGetSkipAccountDetail = $("#urlGetSkipAccountDetail").val();
    $.ajax({
        type: 'POST',
        url: urlGetSkipAccountDetail,
        async: true,
        data: { ProjectId: _projectID, AccountId: AccountId, AccountNo: AccountNo},
        success: function (data) {
            waitingDialog.hide();
            if (data.success == "false") {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                debugger;
                if (data.objSkipAccountDetailLIst != null) {
                    $("#skipAccountHistoryDetailModal").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: true
                    });
                    debugger;
                    ShowDataInTable(data.objSkipAccountDetailLIst);
                }
                else {
                    var $toast = toastr["error"]("No Account History Available", "Notification");
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })
}

function ShowDataInTable(varData) {
    debugger;
    console.log(varData);

    debugger;
    $('#viewTable').empty();
    $.each(varData, function (index, value) {
        debugger;

        $('#viewTable').append(
            '<tr>' +
            '<td>' + value.AccountNo + '</td>' +
            '<td>' + value.CustomerAddress + '</td>' +
            '<td>' + value.Cycle + '</td>' +
            '<td>' + value.Route + '</td>' +
            '<td>' + value.InstallerName + '</td>' +
            '<td>' + value.AssignedUser + '</td>' +
            '<td>' + value.ActionDate + '</td>' +
            '<td>' + value.Status + '</td>' +
            '<td>' + value.SkippedReason + '</td>' +
            '<td>' + value.SkipComment + '</td>' +
            '</tr>'
        );
        debugger;
    });
}
$("#filter").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $("#dynamic-table-detail-data > tbody > tr").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
});
$('#btnCompleteCancel').click(function () {
  
    $("#skipAccountHistoryDetailModal").modal('hide');
});

//end hare
//#region --Project Status Select Function--
$('#ddlProjectStatus').change(function () {

    $('#ddlProject').empty().append($('<option></option>').val('0'));
    $('#ddlProject').trigger('chosen:updated')
    $('#ddlInstaller').empty().append($('<option></option>').val('0'));
    $('#ddlInstaller').trigger('chosen:updated');

    if ($('#ddlProjectStatus').val() != "0") {

        var GetProject = $('#urlGetProject').val();
        var _proStatus = $('#ddlProjectStatus').val();



        $.ajax({
            type: 'GET',
            url: GetProject,
            async: false,
            data: { "status": _proStatus },
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                if (data.success == true) {
                    $.each(data.projectList, function (index, item) {
                        $('#ddlProject').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Error Notification");
                }
            },
            error: function (jqXHR, textStatus, errorThrown)
            {
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

});
//#endregion
//#region --Project Status Select Function--
$/*('#ddlSkipReason').change(function () {

    $('#ddlProject').empty().append($('<option></option>').val('0'));
    $('#ddlProject').trigger('chosen:updated')
    $('#ddlInstaller').empty().append($('<option></option>').val('0'));
    $('#ddlInstaller').trigger('chosen:updated');

    if ($('#ddlSkipReason').val() != "0") {

        var GetProject = $('#urlGetProject').val();
        var _proStatus = $('#ddlSkipReason').val();



        $.ajax({
            type: 'GET',
            url: GetProject,
            async: false,
            data: { "status": _proStatus },
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                if (data.success == true) {
                    $.each(data.projectList, function (index, item) {
                        $('#ddlProject').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Error Notification");
                }
            },
            error: function (jqXHR, textStatus, errorThrown)
            {
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

});*/
//#endregion
//#region --Project Select Function--
$('#ddlProject').change(function () {

    var _projectID = $("#ddlProject option:selected").val();

    if (_projectID != "0") {

        var urlGetInstallerList = $('#urlGetInstallerList').val();
        $('#ddlInstaller').empty().append($('<option></option>').val('0')).trigger('chosen:updated');

        $.ajax({
            type: 'GET',
            url: urlGetInstallerList,
            data: { projectID: _projectID },
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                if (data.success == true) {
                    $('#ddlInstaller').append($('<option></option>').val("0").html("All"));
                    $.each(data.installerList, function (index, item) {
                        $('#ddlInstaller').append($('<option></option>').val(item.UserId).html(item.UserName));
                    });
                    $('#ddlInstaller').trigger('chosen:updated');
                }
                else {
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
});
//#endregion

//#region --Search Button Function--
$('#btnSearch').click(function () {

    var _projectID = $("#ddlProject option:selected").val();
    var _projectstatus = $("#ddlProjectStatus option:selected").val();
    var _SkipReason = $("#ddlSkipReason option:selected").val();
    if (_projectstatus == "0") {
        var $toast = toastr["error"]('Please select project status first', 'Notification');
    }
    else if (_projectID == "0") {
        var $toast = toastr["error"]('Please select project first', 'Notification');
    }
    else if (_SkipReason == "0") {
        var $toast = toastr["error"]('Please select Skip Reason first', 'Notification');
    }
    else {
        LoadActiveTable()
    }




});
//#endregion

//#region --Reset Button Function--
$('#btnReset').click(function () {
    $('#ddlProjectStatus option[value="0"]').attr("selected", "selected").trigger('chosen:updated').trigger("change");

    var todayDate = new Date();
    $("#datepicker1").datepicker("setDate", todayDate);
    $("#datepicker2").datepicker("setDate", todayDate);
    $('#ddlSkipReason option[value="0"]').attr("selected", "selected").trigger('chosen:updated').trigger("change");
});
//#endregion

//#region --View Skipped records function--
function VeiwRecords(data) {

    var urlStoreSkippedID = $('#urlStoreSkippedID').val();
    var urlViewSkippedRecords = $('#urlViewSkippedRecords').val();

    $.ajax({
        type: 'GET',
        data: { uploadId: data.id },
        url: urlStoreSkippedID,
        beforeSend: function () {
            waitingDialog.show('Loading please wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (result) {

            if (result.success === undefined) {
                if (result) {
                    window.open(urlViewSkippedRecords,'_blank' );
                }
                else {
                    var $toast = toastr["error"]("Unable to view records", "Error Notification");
                }
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
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