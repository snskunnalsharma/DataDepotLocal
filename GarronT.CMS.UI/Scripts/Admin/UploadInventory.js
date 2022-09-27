var oTable;

$(document).ready(function () {


    $("#InventoryManagement").addClass('active');
    $("#subInventoryManagement").addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liUploadInventory').addClass("active");


    $("#WarehouseId").chosen({ placeholder_text_single: "Select Warehouse", no_results_text: "Oops, nothing found!" });
    $('#WarehouseId').trigger('chosen:updated');

    $("#UtilityId").chosen({ placeholder_text_single: "Select Utility", no_results_text: "Oops, nothing found!" });
    $('#UtilityId').trigger('chosen:updated');

    $("#ProjectId").chosen({ placeholder_text_single: "Select Project", no_results_text: "Oops, nothing found!" });
    $('#ProjectId').trigger('chosen:updated');

    $('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" }, "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">', });
    debugger;
    $('#divSaveCancel').hide();
});



$('#UtilityId').change(function () {

    $('#ProjectId').empty();
    $('#WarehouseId').empty().trigger('chosen:updated');
    var value = $("#UtilityId option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetProjectList = $('#urlGetProjectList').val();
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


$('#ProjectId').change(function () {

    $('#WarehouseId').empty();
    var value = $("#ProjectId option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetWarehouseList = $('#urlGetWarehouseList').val();
    $.ajax({
        type: 'GET',
        data: { "ProjectId": value.val() },
        url: urlGetWarehouseList,
        success: function (data) {

            if (data != null) {
                $.each(data, function (index, item) {
                    if (index == 0) { $('#WarehouseId').append($('<option></option>').val('0').html("")); }

                    $('#WarehouseId').append($('<option></option>').val(item.WarehouseId).html(item.WarehouseName));
                });
            }
            $('#WarehouseId').trigger('chosen:updated');


            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });

});


//upload & save excel data
$("#btn_Upload").click(function () {
    debugger;

    var UtilityId = $("#UtilityId").val();
    if (UtilityId === '0' || UtilityId === '') {
        var $toast = toastr["error"]("Please, select utility first!", "Notification");
        return;
    }


    var ProjectId = $("#ProjectId").val();
    if (ProjectId === '0' || ProjectId === '') {
        var $toast = toastr["error"]("Please, select project first!", "Notification");
        return;
    }


    var warehouseId = $("#WarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select warehouse first!", "Notification");
        return;
    }



    if ($("#fileInput").val() === '') {
        var $toast = toastr["error"]("Please, select any file to upload", "Notification");
        return;
    }
    var fileExtension = ['xlsx'];
    if ($.inArray($("#fileInput").val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        var $toast = toastr["error"]("Please, select only .xlsx file to upload.", "Notification");
        return;
    }


    waitingDialog.show('Please Wait Uploading Excel...');


    var UploadURL = $("#UploadExcelFileURL").val() + "?ProjectId=" + ProjectId + "&WarehouseId=" + warehouseId;//+ "&UtilityId=" + UtilityId;

    var fileInput = document.getElementById('fileInput');
    //var params = JSON.stringify( projId);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', UploadURL, true);
    xhr.setRequestHeader('Content-type', 'multipart/form-data');
    // xhr.setRequestHeader("Content-length", params.length);
    xhr.setRequestHeader('X-File-Name', fileInput.files[0].name);
    xhr.setRequestHeader('X-File-Type', fileInput.files[0].type);
    xhr.setRequestHeader('X-File-Size', fileInput.files[0].size);

    xhr.send(fileInput.files[0]);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            debugger;
            if (xhr.status === 200) {
                var resp = JSON.parse(xhr.responseText);
                if (resp.success === true) {
                    var $toast = toastr["success"]("Excel Uploaded Successfully.", "Success Notification !");

                    $("#fileInput").val("");
                    //$("#WarehouseId").val('0').trigger("chosen:updated");
                   // $("#UtilityId").val('0').trigger("chosen:updated");

                    BindData(resp.varData)


                    var DownloadExcelExport = $('#urlDownloadExcelExport').val();
                    location.href = DownloadExcelExport;

                    $('#divSaveCancel').show();

                }
                else {
                    var $toast = toastr["error"](resp.excelData1, "Notification !");
                }
            }
            else {
                var $toast = toastr["error"]("Failed to Upload Excel.", "Notification !");
            }
            waitingDialog.hide();
        }
    }

});


// bind all data of the project
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
     // "aaSorting": [[3, "asc"]],
     "aoColumns": [

                   //{ "mData": "UploadNumber" },
                   { "mData": "UtilityType" },
                   { "mData": "CategoryName" },
                   { "mData": "ProductName" },
                   { "mData": "PartNumber" },
                   { "mData": "UPCCode" },
                   { "mData": "Make" },
                   { "mData": "Type" },
                   { "mData": "Size" },
                   { "mData": "SerialNumber" },
                   { "mData": "Quantity" },
                   { "mData": "IsDuplicate" },
                   { "bSearchable": false, "mData": null },

     ],
     "aoColumnDefs": [
                                            {
                                                "mRender": function (data, type, full, row) {
                                                    if (full.InventryStatus != 0) {
                                                        return '<a  id=' + full.UniqueId + ' onclick="EditUploadedRecord(this);" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
                                                    }
                                                    else {
                                                        return "";
                                                    }
                                                },
                                                "aTargets": [11]
                                            }
     ],

 }), $('.dataTables_filter input').attr('maxlength', 50);


}

function fnValidateDynamicContent(element) {
    // debugger;

    var currForm = $(element).find("form");
    currForm.removeData("validator");
    currForm.removeData("unobtrusiveValidation");
    var $this = $(element);

    if ($.validator.unobtrusive != undefined) {
        $.validator.unobtrusive.parse(element);
    }

    currForm.validate(); // This line is important and added for client side validation to trigger, without this it didn't fire client side errors.
}


// get selected record data from server
function EditUploadedRecord(recordId) {
    var uniqueId = $(recordId).attr("id");
    var urlEdit = $('#urlEdit').val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlEdit,
        async: true,
        data: { EditId: uniqueId },
        success: function (data) {
            debugger;
            waitingDialog.hide();
            $('#AddEditModel').empty();
            $('#AddEditModel').attr("style", "display:block")
            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")

            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            // $("#CategoryId").chosen({ no_results_text: "Oops, nothing found!" });
            // $('#CategoryId').trigger('chosen:updated');


        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }
        //error: function () {
        //    var $toast = toastr["error"]("something seems wrong", "Notification");
        //}


    })

}



$('#btnCancel').click(function () {
    $("#fileInput").val("");
    $("#WarehouseId").val('0').trigger("chosen:updated");
    // $("#UtilityId").val('0').trigger("chosen:updated");
    $('#divSaveCancel').hide();
    $('#dynamic-table').dataTable().fnClearTable();//clear the DataTable
});


$('#btnSave').click(function () {
    debugger;
    waitingDialog.show('Please Wait...');
    var urlSaveInventryRecord = $('#urlSaveInventryRecord').val();
    $.ajax({
        type: 'GET',
        url: urlSaveInventryRecord,
        async: true,
        //data: {  },
        success: function (data) {
            waitingDialog.hide();
            if (data.status == true) {
                var $toast = toastr["success"](data.resultMessage, "Success Notification !");
                $('#divSaveCancel').hide();
                $('#dynamic-table').dataTable().fnClearTable();//clear the DataTable

                var viewUploadedRecords = $('#urlViewUpload').val()
                location.href = viewUploadedRecords + "/" + data.inwardHeaderId
            }
            else {
                var $toast = toastr["error"](data.resultMessage, "Notification !");
            }

        },
        error: function () {
            waitingDialog.hide();
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }


    })




});

$(document).on("click", "#btnUpdate", function myfunction() {
    var form = $("#formAddEdit");
    debugger;
    form.validate();

    if (form.valid()) {
        UploadInventoryBO = {

            "UtilityId": $(".utilityId").val(),
            "WarehouseId": $(".warehouseId").val(),

            "UniqueId": $("#UniqueId").val(),
            "UtilityType": $("#UtilityType").val(),
            "CategoryName": $("#CategoryName").val(),
            "ProductName": $("#ProductName").val(),
            "PartNumber": $("#PartNumber").val(),
            "UPCCode": $("#UPCCode").val(),
            "Make": $("#Make").val(),
            "Type": $("#Type").val(),
            "Size": $("#Size").val(),
            "SerialNumber": $("#SerialNumber").val(),
            "Quantity": $("#Quantity").val(),
        };
        waitingDialog.show('Please Wait...');
        var urlSaveEditedRecord = $('#urlSaveEditedRecord').val();
        $.ajax({
            type: 'Post',
            url: urlSaveEditedRecord,
            async: true,
            data: { UploadInventoryBO: UploadInventoryBO },
            success: function (data) {
                debugger;
                waitingDialog.hide();
                if (data.statusMessage.Success == true) {
                    var $toast = toastr["success"](data.statusMessage.Message, "Success Notification !");
                    $("#AddEditModel").modal('hide');
                    BindData(data.recordList)

                }
                else {
                    var $toast = toastr["error"](data.statusMessage.Message, "Update Failed !");
                }

            },
            error: function () {
                waitingDialog.hide();
                var $toast = toastr["error"]("something seems wrong", "Notification");
            }


        })
    }
})

function fnValidateDynamicContent(element) {
    // debugger;

    var currForm = $(element).find("form");
    currForm.removeData("validator");
    currForm.removeData("unobtrusiveValidation");
    var $this = $(element);

    if ($.validator.unobtrusive != undefined) {
        $.validator.unobtrusive.parse(element);
    }

    currForm.validate(); // This line is important and added for client side validation to trigger, without this it didn't fire client side errors.
}




