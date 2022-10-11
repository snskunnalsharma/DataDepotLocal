$(document).ready(function () {
    $("#InventoryReports").addClass('active');
    $("#subReports").addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liInventoryMovementReport').addClass("active");

    $('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" }, "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">', });

    $("#UtilityId").chosen({ placeholder_text_single: "Select Utility", no_results_text: "Oops, nothing found!" });
    $('#UtilityId').trigger('chosen:updated');

    $("#ProjectId").chosen({ placeholder_text_single: "Select Project", no_results_text: "Oops, nothing found!" });
    $('#ProjectId').trigger('chosen:updated');
})

$('#UtilityId').change(function () {

    $('#ProjectId').empty().trigger('chosen:updated');

    var value = $("#UtilityId option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetProjectList = $('#urlGetProject').val();
    $.ajax({
        type: 'GET',
        data: { "UtilityId": value.val() },
        url: urlGetProjectList,
        success: function (data) {

            if (data != null) {
                $.each(data, function (index, item) {
                    if (index == 0) { $('#ProjectId').append($('<option></option>').val('0').html("")); }

                    $('#ProjectId').append($('<option></option>').val(item.ProjectId).html(item.ProjectName));
                });
            }
            $('#ProjectId').trigger('chosen:updated');


            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });

});

$("#btnSearch").click(function () {
    debugger;
    var serialNumber = $("#txtSerialNumber").val();
    var value1 = $("#UtilityId option:selected").val();

    var value2 = $("#ProjectId option:selected").val();


    if (value1 === null || value1 === undefined || value1 === "0") {
        var $toast = toastr["error"]("Please select utility", "Notification");
    }
    else if (value2 == null || value2 === undefined || value2 === "0") {
        var $toast = toastr["error"]("Please select project", "Notification");
    }
    else if (serialNumber == "") {
        var $toast = toastr["error"]("Please enter serial number", "Notification");
    }
    else {


        waitingDialog.show('Please Wait...');
        var urlgetReportData = $('#urlgetReportData').val();
        $.ajax({
            type: 'GET',
            url: urlgetReportData,
            async: true,
            data: { projectId: value2, serialnumber: serialNumber },
            success: function (data) {
                debugger;
                waitingDialog.hide();
                if (data.StatusMessage.Success == true) {
                    BindData(data.reportdata)
                }
                else {
                    var $toast = toastr["error"](data.StatusMessage.Message, "Notification !");
                }

            },
            error: function () {
                waitingDialog.hide();
                var $toast = toastr["error"]("something seems wrong", "Notification");
            }


        })
    }

})


$("#btnReset").click(function () {
    $("#txtSerialNumber").val("");
    $('#dynamic-table').dataTable().fnClearTable();//clear the DataTable


    $('#UtilityId').val("0").trigger('chosen:updated');

    $('#ProjectId').empty().trigger('chosen:updated');
})

function BindData(varData) {

    // $('#dynamic-table').dataTable().clear();//clear the DataTable
    // $('#dynamic-table').dataTable().clear();
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
     "iDisplayLength": -1,
     "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
     // "aaSorting": [[3, "asc"]],
     "aoColumns": [

                   { "mData": "SerialNumber" },
                   { "mData": "StockStatus" },
                   { "mData": "Location" },
                   { "mData": "StockMovementDateString" },
                   { "mData": "LatLong" },
                   { "mData": "Account" },
                   { "mData": "Street" },
                   { "mData": "ProjectName" },


                   //{ "bSearchable": false, "mData": null },

     ],
     //"aoColumnDefs": [
     //                                       {
     //"mRender": function (data, type, full, row) {
     //    if (full.InventryStatus != 0) {
     //        return '<a  id=' + full.UniqueId + ' onclick="EditUploadedRecord(this);" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
     //    }
     //    else {
     //        return "";
     //    }
     //},
     //"aTargets": [11]
     // }
     //],

 }), $('.dataTables_filter input').attr('maxlength', 50);


}

