var oTable;

$(document).ready(function () {

    // make current page menu color as active
    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liInventroy');
    listate.addClass("active");

    //#endregion
    $("#ddlWareHouse").chosen({ no_results_text: "Oops, nothing found!" });

    //get the active project list
    warehouseData();


    $('#dynamic-table').DataTable({
        "language": {
            "emptyTable": "No Data To Show"
        }
    });
});

//get the active project list
function warehouseData() {
    debugger;

    $('#urlGetWarehouse').empty();
    $('#urlGetWarehouse').trigger("chosen:updated");
    var GetProject = $('#urlGetWarehouse').val();
    $.ajax({
        type: 'GET',
        url: GetProject,
        async: false,
        data: {},
        success: function (data) {
            $('#ddlWareHouse').empty();

            var count = data.length;
            if (count > 0) {

                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlWareHouse').append($('<option></option>').val('0'));
                    }
                    $('#ddlWareHouse').append($('<option></option>').val(item.WarehouseId).html(item.WarehouseName));
                });
                $('.chosen-select').trigger('chosen:updated');
            }

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }


    })
}


//get the active project list


//upload & save excel data
$("#btn_Upload").click(function () {
    debugger;

    var warehouseId = $("#ddlWareHouse").val();
    if ($("#ddlWareHouse").val() == '0') {
        var $toast = toastr["error"]("Please, select warehouse", "Error Notification");
        return;
    }

    if ($("#fileInput").val() == '') {
        var $toast = toastr["error"]("Please, select any file first to upload", "Error Notification");
        return;
    }
    var fileExtension = ['xlsx'];
    if ($.inArray($("#fileInput").val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        var $toast = toastr["error"]("Please, select only .xlsx file to upload.", "Error Notification");
        return;
    }


    waitingDialog.show('Please Wait Uploading Excel...');


    var UploadURL = $("#UploadExcelFileURL").val() + "?Id=" + warehouseId + "&UtilityId=" + warehouseId;

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
        if (xhr.readyState == 4) {
            debugger;
            if (xhr.status == 200) {
                var resp = JSON.parse(xhr.responseText);
                if (resp.success == true) {
                    var $toast = toastr["success"]("Excel Uploaded Successfully.", "Success Notification !");

                    $("#fileInput").val("");
                    $("#ddlWareHouse").val('0').trigger("chosen:updated");

                    BindData(resp.varData)
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
    debugger;
    oTable = $('#dynamic-table').dataTable(
 {

     "bDestroy": true,
     "bserverSide": true,
     "bDeferRender": true,
     "oLanguage": {
         "sLengthMenu": '_MENU_ Records per page'
     },
     "aaData": varData,
    // "aaSorting": [[3, "asc"]],
     "aoColumns": [
           { "mData": "UploadNumber" },
           { "mData": "MeterMake" },
           { "mData": "MeterType" },
           { "mData": "MeterSize" },
           { "mData": "SerialNo" },
           { "mData": "Description" },
           { "mData": "Quantity" },
           { "mData": "IsDuplicate" },
           { "bSearchable": false, "mData": null },

     ],
     "aoColumnDefs": [
                                            {
                                                "mRender": function (data, type, full, row) {
                                                    return '<a  id=' + full.Id + ' onclick="EditUploadedRecord(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
                                                },
                                                "aTargets": [8]
                                            }
     ],

 }), $('.dataTables_filter input').attr('maxlength', 50);


}


// get selected record data from server
function EditUploadedRecord(recordId) {

    $("#txtId").val(recordId)

    var txtId = $("#txtId").val();
    $("#txtMeterType").val('');
    $("#txtMeterSize").val('');
    $("#txtSerialNo").val('');


    var urlGetCurrentRowData = $('#urlGetCurrentRowData').val();
    $.ajax({
        type: 'GET',
        url: urlGetCurrentRowData,
        async: false,
        data: { recordId: txtId },
        success: function (data) {
            //varData
            $("#txtMeterType").val(data.varData.MeterType);
            $("#txtMeterSize").val(data.varData.MeterSize);
            $("#txtSerialNo").val(data.varData.SerialNo);
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }


    })

    $("#myModal").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
    $("#myModal").draggable({
        handle: ".modal-header"
    });
}



$('#btnCancel').click(function () {
    $("#myModal").modal('hide');
    $("#txtId").val('');
    $("#txtMeterType").val('');
    $("#txtMeterSize").val('');
    $("#txtSerialNo").val('');
});


$('#btnSave').click(function () {

    var txtId = $("#txtId").val();
    var txtMeterType = $("#txtMeterType").val();
    var txtMeterSize = $("#txtMeterSize").val();
    var txtSerialNo = $("#txtSerialNo").val();

    if (txtId == null || txtId.length == 0) {
        var $toast = toastr["error"]("Something seems wrong", "Notification !");
    }
    else if (txtMeterType == null || txtMeterType.length == 0) {
        var $toast = toastr["error"]("Please, enter the meter type", "Notification !");
    }
    else if (txtMeterSize == null || txtMeterSize.length == 0) {
        var $toast = toastr["error"]("Please, enter the meter size ", "Notification !");
    }
    else if (txtSerialNo == null || txtSerialNo.length == 0) {
        var $toast = toastr["error"]("Please, enter the serial number", "Notification !");
    }
    else {


        var urlSaveCurrentRowData = $('#urlSaveCurrentRowData').val();
        $.ajax({
            type: 'GET',
            url: urlSaveCurrentRowData,
            async: false,
            data: { recordId: txtId, meterSize: txtMeterSize, meterType: txtMeterType, serialNo: txtSerialNo },
            success: function (data) {
              
                if (data.status == true) {
                    var $toast = toastr["success"](data.resultMessage, "Success Notification !");
                    $("#myModal").modal('hide');
                    $('#ddlProject').trigger("change");
                    $("#txtId").val('');
                    $("#txtMeterType").val('');
                    $("#txtMeterSize").val('');
                    $("#txtSerialNo").val('');
                }
                else {
                    var $toast = toastr["error"](data.resultMessage, "Notification !");
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Notification");
            }


        })


    }

});



