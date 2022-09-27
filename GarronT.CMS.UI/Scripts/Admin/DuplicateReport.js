
var oTable;

$(document).ready(function () {


    // make current page menu color as active
    $("#Reports").addClass('active');
    $("#subReports").addClass('block');


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liDuplicateReport');
    listate.addClass("active");




    $("#ddlProjectStatus").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlSearchFilter").chosen({ no_results_text: "Oops, nothing found!" });

    $('#dynamic-table').DataTable({
        "language": {
            "emptyTable": "No Data To Show"
        }, "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',

    });
});


//get the project list as per the project status selected
$('#ddlProjectStatus').change(function () {
    debugger;
    if ($('#ddlProjectStatus').val() == "0") {
        resetData();
    }
    else {
        $('#ddlProject').empty();
        $('#ddlProject').trigger("chosen:updated");
        var GetProject = $('#urlGetProject').val();
        var _proStatus = $('#ddlProjectStatus').val();
        $.ajax({
            type: 'GET',
            url: GetProject,
            async: false,
            data: { "status": _proStatus },
            success: function (data) {
                $('#ddlProject').empty();

                var count = data.length;
                if (count > 0) {
                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlProject').append($('<option></option>').val('0'));
                        }
                        $('#ddlProject').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }
            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }

        });
    }

});



// reset dropdowns on the page
function resetData() {
    debugger;


    $('#ddlProject').empty();
    $('#ddlProject').trigger("chosen:updated");

    $('#ddlInstaller').empty();
    $('#ddlInstaller').trigger("chosen:updated");


}



$('#ddlProject').change(function () {


    var Project = $("#ddlProject option:selected")

    if (Project.length > 0) {

        var urlGetInstallerList = $('#urlGetInstallerList').val();

        $.ajax({
            type: 'GET',
            url: urlGetInstallerList,
            data: { projectId: Project.val() },
            async: true,
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
            success: function (data) {
                debugger;
                $('#ddlInstaller').empty();

                var count = data.length;
                if (count > 0) {

                    $('#ddlInstaller').append($('<option></option>').val('0').html("All"));

                    $.each(data, function (index, item) {

                        $('#ddlInstaller').append($('<option></option>').val(item.UserId).html(item.UserName));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                    waitingDialog.hide();
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
            },

        });

    }
});


$('#btnSubmit').click(function () {

    var ddlProjectStatus = $("#ddlProjectStatus").val();
    var ddlProject = $("#ddlProject").val();
    var ddlInstaller = $("#ddlInstaller").val();
    var ddlSearchFilter = $("#ddlSearchFilter").val();
    debugger;
    if (ddlProjectStatus == "0") {
        var $toast = toastr["error"]("Please, select the project status.", "Notification");
        return;
    }
    else if (ddlProject == "0") {
        var $toast = toastr["error"]("Please, select the project.", "Notification");
        return;
    }
    else {
        GetDuplicateRecords(ddlProject, ddlInstaller, ddlSearchFilter)
    }

});



// get the data for search
function GetDuplicateRecords(projectID, InstallerID, SearchField) {
    debugger;

    // var projectID = 1;
    var urlGetDuplicateRecords = $('#urlGetDuplicateRecords').val();

    $.ajax({
        type: 'GET',
        url: urlGetDuplicateRecords,
        async: true,
        data: { projectId: projectID, installerId: InstallerID, searchFieldId: SearchField },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;

            BindData(data.varData);

        },
        "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
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


// bind all data of the project
function BindData(varData) {
    debugger;
    oTable = $('#dynamic-table').dataTable(
 {

     "bDestroy": true,
     "bserverSide": true,
     "bDeferRender": true,
     "oLanguage": {
         "sLengthMenu": '_MENU_ Records per page'
            },
            "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
     "aaData": varData,
     //"aaSorting": [[3, "asc"]],
     "aoColumns": [
                           { "mData": "Account" },
                            { "mData": "Customer" },
                            { "mData": "Street" },
                            { "mData": "FirstName" },
                             { "mData": "visitdatetime" },
                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterRadioNo" },
                             { "mData": "NEWMETERNUMBER" },
                            { "mData": "NEWMETERRADIO" },
                        // { "bSearchable": false, "mData": null },

     ],
     //"aoColumnDefs": [
     //                                       {
     //                                           "mRender": function (data, type, full, row) {
     //                                               return '<a  id=' + full.Id + ' onclick="EditUploadedRecord(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
     //                                           },
     //                                           "aTargets": [4]
     //                                       }
     //],

 }), $('.dataTables_filter input').attr('maxlength', 50);


}


$('#btnExport').click(function () {
    var ddlProjectStatus = $("#ddlProjectStatus").val();
    var ddlProject = $("#ddlProject").val();
    var ddlInstaller = $("#ddlInstaller").val();
    var ddlSearchFilter = $("#ddlSearchFilter").val();
    debugger;
    if (ddlProjectStatus == "0") {
        var $toast = toastr["error"]("Please, select the project status.", "Notification");
        return;
    }
    else if (ddlProject == "0") {
        var $toast = toastr["error"]("Please, select the project.", "Notification");
        return;
    }
    else {

        var urlDownloadExcelInstaller = $('#urlExportDuplicateRecords').val() + "?projectId=" + ddlProject + "&installerId=" + ddlInstaller + "&searchFieldId=" + ddlSearchFilter;
        location.href = urlDownloadExcelInstaller;

    }



});


$('#btnReset').click(function () {

    $('#ddlProjectStatus').val('0').attr("selected", "selected");
    $('#ddlProjectStatus').trigger("chosen:updated");
    $('#ddlProject').empty();
    $('#ddlProject').trigger("chosen:updated");
    $('#ddlInstaller').empty();
    $('#ddlInstaller').trigger("chosen:updated");

    $("#ddlSearchFilter").val('0').attr("selected", "selected");
    $('#ddlSearchFilter').trigger("chosen:updated");

    var table = $('#dynamic-table').DataTable();

    table.fnClearTable();
    table.fnDraw();

});