var oTable;
var serialNumberRequired = false;

//window.onbeforeunload = function () {
//    return "Data will be lost if you leave the page, are you sure?";
//};
$(document).ready(function () {
    debugger;


    $("#InventoryManagement").addClass('active');
    $("#subInventoryManagement").addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liWebInvenotryRecieve').addClass("active");

    $("#UtilityId").chosen({ placeholder_text_single: "Select Utility", no_results_text: "Oops, nothing found!" });
    $('#UtilityId').trigger('chosen:updated');

    $("#ProjectId").chosen({ placeholder_text_single: "Select Project", no_results_text: "Oops, nothing found!" });
    $('#ProjectId').trigger('chosen:updated');

    $("#WarehouseId").chosen({ placeholder_text_single: "Select Warehouse", no_results_text: "Oops, nothing found!" });
    $('#WarehouseId').trigger('chosen:updated');

    $("#ProductId").chosen({ placeholder_text_single: "Select Product", no_results_text: "Oops, nothing found!" });
    $('#ProductId').trigger('chosen:updated');

    $('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" } });

    $("#btnupdate").hide();

    $('#divFinalSaveCancel').hide();

});


$('#UtilityId').change(function () {

    $('#ProjectId').empty();
    $('#ProjectId').trigger('chosen:updated');
    $('#WarehouseId').empty();
    $('#WarehouseId').trigger('chosen:updated');
    $('#ProductId').empty();
    $('#ProductId').trigger('chosen:updated');


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


$('#ProjectId').change(function () {

    $('#WarehouseId').empty();
    $('#ProductId').empty();
    $('#ProductId').trigger('chosen:updated');
    var value = $("#ProjectId option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetWarehouseAndProduct = $('#urlGetWarehouseAndProduct').val();
    $.ajax({
        type: 'GET',
        data: { "ProjectId": value.val() },
        url: urlGetWarehouseAndProduct,
        success: function (data) {

            if (data.WarehouseList != null) {
                $.each(data.WarehouseList, function (index, item) {
                    if (index == 0) { $('#WarehouseId').append($('<option></option>').val('0').html("")); }

                    $('#WarehouseId').append($('<option></option>').val(item.WarehouseId).html(item.WarehouseName));
                });
            }
            $('#WarehouseId').trigger('chosen:updated');

            if (data.ProductList != null) {
                $.each(data.ProductList, function (index, item) {
                    if (index == 0) { $('#ProductId').append($('<option></option>').val('0').html("")); }

                    $('#ProductId').append($('<option></option>').val(item.Id).html(item.FieldValue));
                });
            }
            $('#ProductId').trigger('chosen:updated');


            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });

});

//numeric only validation code
$('body').on('keypress', '.numeric-only', function (e) {
    var regex = new RegExp("^[0-9\b]+$");

    if (event.charCode == 0) {
        return true;
    }
    else {
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
});

//character only validation code
$(function () {
    $('body').on('keypress', '.Character-Only', function (e) {
        var regex = new RegExp("^[A-Za-z\ \s]*$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
});

//alfanumeric validation code
$(function () {
    $('body').on('keypress', '.alfanumeric', function (e) {
        var regex = new RegExp("^[A-Za-z0-9]*$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
});


$("#ProductId").change(function () {

    $('#chkGenerateBarcode').attr('checked', false); // uncheck it
    var ProductId = $("#ProductId").val();
    if (ProductId != "0") {

        waitingDialog.show('Please Wait...');
        var urlGetProductData = $("#urlGetProductData").val();

        $.ajax({
            type: 'GET',
            url: urlGetProductData,
            async: true,
            data: { productId: ProductId },
            success: function (data) {
                waitingDialog.hide();
                if (data.success == "false") {
                    var $toast = toastr["error"](data.ErrorMessage, "Notification");
                }
                else {
                    serialNumberRequired = data.productModel.SerialNumberRequired;
                    if (data.productModel.SerialNumberRequired) {
                        $("#divGenerateBarcode").show();
                    }
                    else {
                        //$("#divGenerateBarcode").hide();
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
    else {
        var $toast = toastr["error"]("Please, Select product", "Notification");
    }
});


var RecordType = 0; // 1 = quantity , 2 = serial number, 3 = barcode;


$("#btnAdd").click(function () {

    var UtilityId = $("#UtilityId").val();
    if (UtilityId === '0' || UtilityId === '') {
        var $toast = toastr["error"]("Please, select utility", "Notification");
        return;
    }

    var ProjectId = $("#ProjectId").val();
    if (ProjectId === '0' || ProjectId === '') {
        var $toast = toastr["error"]("Please, select project", "Notification");
        return;
    }


    var warehouseId = $("#WarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select warehouse", "Notification");
        return;
    }

    var ProductId = $("#ProductId").val();
    if (ProductId === '0' || ProductId === '') {
        var $toast = toastr["error"]("Please, select product", "Notification");
        return;
    }

    var txtQuantity = parseInt($("#txtQuantity").val()) || 0;
    if (txtQuantity === '' || txtQuantity === 0) {
        var $toast = toastr["error"]("Please, enter quantity", "Notification");
        $("#txtQuantity").focus();
        return;
    }

    if (serialNumberRequired != undefined && serialNumberRequired != null) {
        if (serialNumberRequired == true) {
            var chkGenerateBarcode = $('#chkGenerateBarcode').is(":checked")
            if (chkGenerateBarcode == true) {
                RecordType = 3 //barcode
                AddProduct(RecordType)
            }
            else {
                InsertSerialNumber()
            }
        }
        else {
            RecordType = 1 //quantity
            AddProduct(RecordType)
        }
    }
})

$("#btnAddSerialNumber").click(function () {
    RecordType = 2 //serial number
    AddProduct(RecordType)
})

function AddProduct(RecordType) {
    
    //hideerrormessage = false;

    var UtilityId = $("#UtilityId").val();
    if (UtilityId === '0' || UtilityId === '') {
        var $toast = toastr["error"]("Please, select utility", "Notification");
        return;
    }

    var ProjectId = $("#ProjectId").val();
    if (ProjectId === '0' || ProjectId === '') {
        var $toast = toastr["error"]("Please, select project", "Notification");
        return;
    }
    
    var warehouseId = $("#WarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select warehouse", "Notification");
        return;
    }

    var ProductId = $("#ProductId").val();
    if (ProductId === '0' || ProductId === '') {
        var $toast = toastr["error"]("Please, select Product", "Notification");
        return;
    }

    var quantity = $("#txtQuantity").val();
    if (quantity === '0' || quantity === '') {
        var $toast = toastr["error"]("Please, enter Quantity", "Notification");
        return;
    }

    if (serialNumberRequired == true && RecordType == 2) { // 2 serial number record

        var serialnumbers = [];
        var serialNumbertextbox = $(".serialNumber");
        for (var i = 0; i < serialNumbertextbox.length; i++) {
            var serialNumber = $(serialNumbertextbox[i]).val();
            if (serialNumber == "") {
                if (hideerrormessage == false) {
                    hideerrormessage = true;
                    var $toast = toastr["error"]("Please,enter serial number", "Notification");
                }
                $(serialNumbertextbox[i]).focus();

                return false;
            }
            else {
                serialnumbers.push(serialNumber);
            }
        }

    }

    var isGenerateBarcode = $('#chkGenerateBarcode').is(":checked")


    var urlAddInventry = $("#urlAddInventry").val();
    waitingDialog.show('Please Wait...');

    $.ajax({
        url: urlAddInventry,
        type: "POST",
        traditional: true,
        data: { warehouseId: warehouseId, projectId:ProjectId, productId: ProductId, quantity: quantity, RecordType: RecordType, serialNos: serialnumbers, isGenerateBarcode: isGenerateBarcode },
        success: function (data) {
            waitingDialog.hide();
            
            if (data.success != undefined && data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                //$("#inventryReceiveDiv").html(data);
                //$('#dynamic-table').dataTable();
                $("#inventrySerialDialog").modal("hide");
                serialNumbers = [];

                BindDataTable(data.productModelList);
                CheckDatatableData();
                if (data.duplicateFound) {
                    $.confirm({
                        icon: 'fa fa-warning',
                        columnClass: 'col-md-6 col-md-offset-3',
                        closeIcon: true,
                        animationBounce: 2,
                        title: 'Duplicate Serial Numbers found!',
                        content: data.duplicateSerialNumberMsg,
                        buttons: {
                            Yes:
                                {
                                    text: 'Ok', // 
                                    btnClass: 'btn-danger',
                                    action: function () {

                                    }
                                },
                            //No: {
                            //    text: 'No',
                            //    btnClass: 'btn-blue',
                            //    action: function () {

                            //    }
                            //}
                        }
                    });
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

function CheckDatatableData() {
    
    var tableRow = $("#inventryReceiveDiv").children().find('table').children('tbody').children('tr')
    if (tableRow.length == 1 && tableRow.children().attr("class") == "dataTables_empty") {
        //$('#WarehouseId').val("0").trigger('chosen:updated');
        $('#WarehouseId').prop("disabled", false).trigger('chosen:updated');       
        $('#UtilityId').prop("disabled", false).trigger('chosen:updated');       
        $('#ProjectId').prop("disabled", false).trigger('chosen:updated');

        $('#divFinalSaveCancel').hide();
    }
    else {
        $('#WarehouseId').prop("disabled", true).trigger('chosen:updated');
        $('#UtilityId').prop("disabled", true).trigger('chosen:updated');       
        $('#ProjectId').prop("disabled", true).trigger('chosen:updated');

        $('#divFinalSaveCancel').show();
    }
    resetData()
}

//open popup to enter serial numbers
function InsertSerialNumber() {
    
    var txtQuantity = $("#txtQuantity").val();
    if (txtQuantity === '') {
        var $toast = toastr["error"]("Please, enter quantity", "Notification");
        $("#txtQuantity").focus();
        return;
    }


    var urlInsertSerialNumber = $("#urlInsertSerialNumber").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlInsertSerialNumber,

        data: { Quantity: txtQuantity },
        success: function (data) {
            waitingDialog.hide();
            
            if (data.success != undefined && data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                $("#divSerialNumber").html(data);

                $("#inventrySerialDialog").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });
                
                setTimeout(function () {
                    $($('.serialNumber')[0]).focus()
                }, 1000);
               
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

var serialNumbers = [];
var hideerrormessage = false;
$(document).on("blur", ".serialNumber", function (e) {
    debugger;
    hideerrormessage = false;
    var serialNumberId = $(this).attr('id');
    var serialNumber = $(this).val();

    if (serialNumber != "") {
        var serialNumberObject = {
            Id: serialNumberId,
            SerialNumber: serialNumber
        };

        var resultObject = search(serialNumberObject, serialNumbers);
        if (resultObject == undefined) {
            objIndex = serialNumbers.findIndex((obj => obj.Id == serialNumberObject.Id));
            if (objIndex == -1) {
                serialNumbers.push(serialNumberObject)
            }
            else {
                serialNumbers[objIndex].SerialNumber = serialNumberObject.SerialNumber
            }
        }
        else {
            $(this).val("");
            $(this).focus();
            tempField = $(this);
            setTimeout("tempField.focus();", 1);

            if (hideerrormessage == false) {
                hideerrormessage = true;
                var $toast = toastr["error"](serialNumberObject.SerialNumber + " </br> Serial number already exist", "Notification");
            }
           
            return false;
        }
    }
})

function search(nameKey, myArray) {

    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].SerialNumber === nameKey.SerialNumber && nameKey.Id != myArray[i].Id) {

            return myArray[i];
        }
    }
}

function editInvetryRecord(element) {
    
    var uniqueid = $(element).parent().parent().find("#hndUniqueId").val();
    var warehouseid = $(element).parent().parent().find("#hndWarehouseId").val();
    var productid = $(element).parent().parent().find("#hndProductId").val();
    var serialnumber = $(element).parent().parent().find("#hndSerialNumber").val();
    var quantity = $(element).parent().parent().find("#hndQuantity").val();
    var serialnumberRequired = $(element).parent().parent().find("#hndSerialNumberRequired").val();
    var isBarcode = $(element).parent().parent().find('[type="checkbox"]').is(":checked");
    if (serialnumberRequired == "true") {

        if (isBarcode) {
            $("#txtEditQuantity").show();
            $("#txtEditSerialNumber").hide();
            $("#editRecordLabel").text("Quantity");
            $("#txtEditQuantity").val(quantity);

        }
        else {
            $("#txtEditQuantity").hide();
            $("#txtEditSerialNumber").show();
            $("#txtEditSerialNumber").val(serialnumber);
            $("#editRecordLabel").text("Serial Number");
        }

        $("#hndUniqueInventryId").val(uniqueid);

        $("#hndIsSerialNumber").val(serialnumberRequired);
        $("#hndIsBarcode").val(isBarcode);

        $("#editInventrySerialDialog").modal({
            show: true,
            backdrop: 'static',
            keyboard: false
        });
    }
    else {
        $("#txtEditQuantity").show();
        $("#txtEditSerialNumber").hide();

        $("#txtEditQuantity").val(quantity);
        $("#hndUniqueInventryId").val(uniqueid);
        $("#editRecordLabel").text("Quantity");
        $("#hndIsSerialNumber").val(serialnumberRequired);

        $("#editInventrySerialDialog").modal({
            show: true,
            backdrop: 'static',
            keyboard: false
        });
    }
}

$("#btnUpdateSerialNumber").click(function () {
    

    var serialNo = $("#txtEditSerialNumber").val();
    var quantity = $("#txtEditQuantity").val() || 0;
    var InventryId = $("#hndUniqueInventryId").val();
    var serialNumberRequired = $("#hndIsSerialNumber").val();
    var IsBarcode = $("#hndIsBarcode").val();
    if (serialNumberRequired == "true") {
        if (IsBarcode == "true") {
            var txtEditQuantity = $("#txtEditQuantity").val();
            if (txtEditQuantity === '') {
                var $toast = toastr["error"]("Please, enter quantity", "Notification");
                $("#txtEditQuantity").focus();
                return;
            }
        }
        else {
            var txtEditSerialNumber = $("#txtEditSerialNumber").val();
            if (txtEditSerialNumber === '') {
                var $toast = toastr["error"]("Please, enter serial number", "Notification");
                $("#txtEditSerialNumber").focus();
                return;
            }
        }

    }
    else {
        var txtEditQuantity = $("#txtEditQuantity").val();
        if (txtEditQuantity === '') {
            var $toast = toastr["error"]("Please, enter quantity", "Notification");
            $("#txtEditQuantity").focus();
            return;
        }
    }

    var urlEditInventry = $("#urlEditInventry").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlEditInventry,

        data: { InventryId: InventryId, serialNo: serialNo, quantity: quantity, serialNumberRequired: serialNumberRequired },
        success: function (data) {
            waitingDialog.hide();
            
            if (data.success != undefined && data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                BindDataTable(data.productModelList);
                //$("#inventryReceiveDiv").html(data);
                //$('#dynamic-table').dataTable();
                $("#editInventrySerialDialog").modal("hide");

                var tableRow = $("#inventryReceiveDiv").children().find('table').children('tbody').children('tr')
                if (tableRow.length == 1 && tableRow.children().attr("class") == "dataTables_empty") {
                    $('#WarehouseId').val("0").trigger('chosen:updated');
                    $('#WarehouseId').prop("disabled", false).trigger('chosen:updated');
                }
                else {
                    $('#WarehouseId').prop("disabled", true).trigger('chosen:updated');
                }

                resetData()

                $("#btnAdd").show();

                $("#btnupdate").hide();
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })
})

$("#btnupdate").click(function () {

    var warehouseId = $("#WarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select warehouse", "Notification");
        return;
    }

    var ProductId = $("#ProductId").val();
    if (ProductId === '0' || ProductId === '') {
        var $toast = toastr["error"]("Please, select Product", "Notification");
        return;
    }

    var quantity = $("#txtQuantity").val();
    var serialNo = $("#txtSerialNumber").val();
    var InventryId = $("#hndUniqueInventryId").val();


    var urlEditInventry = $("#urlEditInventry").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlEditInventry,

        data: { InventryId: InventryId, warehouseId: warehouseId, productId: ProductId, quantity: quantity, serialNo: serialNo },
        success: function (data) {
            waitingDialog.hide();
            
            if (data.success != undefined && data.success == true) {
                BindDataTable(data.productModelList)
                CheckDatatableData()
                //$("#inventryReceiveDiv").html(data);
                //$('#dynamic-table').dataTable();

            }
            else {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })
})

$("#btnReset").click(function () {
    
    resetData()
    var tableRow = $("#inventryReceiveDiv").children().find('table').children('tbody').children('tr')
    if (tableRow.length == 1 && tableRow.children().attr("class") == "dataTables_empty") {
        $('#UtilityId').val("0").trigger('chosen:updated');
        $('#UtilityId').prop("disabled", false).trigger('chosen:updated');

        //$('#ProjectId').val("0").trigger('chosen:updated');
        //$('#ProjectId').prop("disabled", false).trigger('chosen:updated');

        //$('#WarehouseId').val("0").trigger('chosen:updated');
        //$('#WarehouseId').prop("disabled", false).trigger('chosen:updated');

        $('#ProjectId').empty();
        $('#ProjectId').trigger('chosen:updated');
        $('#WarehouseId').empty();
        $('#WarehouseId').trigger('chosen:updated');
        $('#ProductId').empty();
        $('#ProductId').trigger('chosen:updated');


    }
    else {
        $('#WarehouseId').prop("disabled", true).trigger('chosen:updated');
    }
});

function resetData() {

    //$('#WarehouseId').val("0").trigger('chosen:updated');
    $('#chkGenerateBarcode').attr('checked', false); // uncheck it
    $('#ProductId').val("0").trigger('chosen:updated');
    $("#txtQuantity").val("").prop("readonly", false);
    $("#txtSerialNumber").val("").prop("readonly", false);
    $("#btnAdd").show();
    $("#btnupdate").hide();
    $("#txtBarcode").val("");

}

function deleteInvetryRecord(element) {
    var InventryId = $(element).parent().parent().find("#hndUniqueId").val();
    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Delete Product!',
        content: 'Do you want to delete the current product?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        DeleteProductFromList(InventryId)
                    }
                },
            No: {
                text: 'No',
                btnClass: 'btn-blue',
                action: function () {

                }
            }
        }
    });
}

function DeleteProductFromList(InventryId) {
    waitingDialog.show('Please Wait...');

    var urlDeleteInventry = $("#urlDeleteInventry").val();

    $.ajax({
        type: 'GET',
        url: urlDeleteInventry,

        data: { InventryId: InventryId },
        success: function (data) {
            waitingDialog.hide();
            
            if (data.success != undefined && data.success == false) {
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                // $("#inventryReceiveDiv").html(data);
                //$('#dynamic-table').dataTable();
                BindDataTable(data.productModelList)
                CheckDatatableData();
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

function fnValidateDynamicContent(element) {
    // 

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




    var urlEdit = $('#urlEdit').val();
    $.ajax({
        type: 'GET',
        url: urlEdit,
        async: false,
        data: { EditId: recordId },
        success: function (data) {
            
            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")

            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            // $("#CategoryId").chosen({ no_results_text: "Oops, nothing found!" });
            // $('#CategoryId').trigger('chosen:updated');


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }
        //error: function () {
        //    var $toast = toastr["error"]("something seems wrong", "Notification");
        //}


    })

    //$("#myModal").modal({ show: true, backdrop: 'static', keyboard: false });

    //$("#myModal").draggable({ handle: ".modal-header" });
}

$('#btnSave').click(function () {
    
    var warehouseId = $('#WarehouseId').val();
    //var utilityId = $('#WarehouseId').val();


    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select warehouse", "Notification");
        return;
    }

    var tableRow = $("#inventryReceiveDiv").children().find('table').children('tbody').children('tr')
    if (tableRow.length == 1 && tableRow.children().attr("class") == "dataTables_empty") {
        var $toast = toastr["error"]("Please, Add products", "Notification");
        return;
    }
    waitingDialog.show('Please Wait...');
    var urlSaveInventryList = $('#urlSaveInventryList').val();
    $.ajax({
        type: 'GET',
        url: urlSaveInventryList,
        async: true,
        data: { warehouseId: warehouseId },
        success: function (data) {
            waitingDialog.hide();
            if (data.status == true) {
                var $toast = toastr["success"](data.resultMessage, "Success Notification !");
                $('#WarehouseId').val("").prop("disabled", false).trigger('chosen:updated');
                resetData()
                $('#dynamic-table').dataTable().fnClearTable();
                ResetAllTempData();
                
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

$('#btnCancelSave').click(function () {
    //$("#WarehouseId").val('0').prop("disabled", false).trigger("chosen:updated");
    //$('#ProductId').val("0").trigger('chosen:updated');
    $('#UtilityId').val("0").trigger('chosen:updated');
    $('#ProjectId').empty();
    $('#ProjectId').trigger('chosen:updated');
    $('#WarehouseId').empty();
    $('#WarehouseId').trigger('chosen:updated');
    $('#ProductId').empty();
    $('#ProductId').trigger('chosen:updated');
    $("#txtQuantity").val('')
    $('#divSaveCancel').hide();
    $('#dynamic-table').dataTable().fnClearTable();//clear the DataTable
    CheckDatatableData();
    ResetAllTempData();

});

function ResetAllTempData() {
    var urlResetAllData = $("#urlResetAllData").val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        type: 'GET',
        url: urlResetAllData,
        sync: false,
        // data: { InventryId: InventryId, serialNo: serialNo, quantity: quantity, serialNumberRequired: serialNumberRequired },
        success: function (data) {
            waitingDialog.hide();

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    })
}

// bind all data of the project
function BindDataTable(varData) {
    
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

                   { "bSearchable": false, "mData": null },
                   { "mData": "WarehouseName" },
                   { "mData": "ProductName" },
                   { "mData": "ProductDescription" },
                   { "mData": "ProductPartNumber" },
                   { "mData": "ProductUPCCode" },
                   { "mData": "UtilityName" },
                   { "mData": "CategoryName" },
                   { "mData": "Make" },
                   { "mData": "Type" },
                   { "mData": "Size" },
                   { "mData": "SerialNumber" },
                   { "mData": "Quantity" },
                    { "mData": "RecordStatus" },
                   { "bSearchable": false, "mData": null },

     ],
     "aoColumnDefs": [
                   {
                       "mRender": function (data, type, full, row) {
                           if (full.GenerateBarcode) {
                               return '<input type="checkbox" disabled="disabled" name="GenerateBarcode" checked="checked" />' +
                                   '<input type="hidden" id="hndSerialNumberRequired" name="name" value="' + full.SerialNumberRequired + '" />' +
                                   '<input type="hidden" id="hndUniqueId" name="name" value="' + full.UniqueId + '" />' +
                                   '<input type="hidden" id="hndWarehouseId" name="name" value="' + full.WarehouseId + '" />' +
                                   '<input type="hidden" id="hndProductId" name="name" value="' + full.Id + '" />' +
                                   '<input type="hidden" id="hndSerialNumber" name="name" value="' + full.SerialNumber + '" />' +
                                   '<input type="hidden" id="hndQuantity" name="name" value="' + full.Quantity + '" />'

                           }
                           else {
                               return '<input type="checkbox" disabled="disabled" name="GenerateBarcode" />' +
                                   '<input type="hidden" id="hndSerialNumberRequired" name="name" value="' + full.SerialNumberRequired + '" />' +
                                   '<input type="hidden" id="hndUniqueId" name="name" value="' + full.UniqueId + '" />' +
                                   '<input type="hidden" id="hndWarehouseId" name="name" value="' + full.WarehouseId + '" />' +
                                   '<input type="hidden" id="hndProductId" name="name" value="' + full.Id + '" />' +
                                   '<input type="hidden" id="hndSerialNumber" name="name" value="' + full.SerialNumber + '" />' +
                                   '<input type="hidden" id="hndQuantity" name="name" value="' + full.Quantity + '" />'
                           }

                       },
                       "aTargets": [0]
                   },
                   {
                       "mRender": function (data, type, full, row) {
                           return '<a id="" onclick="editInvetryRecord(this)" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                               '<a id="" onclick="deleteInvetryRecord(this)" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>'
                       },
                       "aTargets": [14]
                   }


     ],

 }), $('.dataTables_filter input').attr('maxlength', 50);


}



