var oTable;
var serialNumberRequired = false;
var isMobileBrowser = /Android|iPhone|iPad|iPod/i.test(navigator.userAgent);

// Mobile native: option[hidden] keeps closed-box label but hides the row from the opened list (avoid disabled — jQuery .val() skips disabled selected options).
function applyMobileNativePlaceholder($select, sentinelValue, label) {
    if (!isMobileBrowser || !$select || !$select.length) return;
    var v = String(sentinelValue);
    var $opt = $select.find("option").filter(function () { return String($(this).val()) === v; });
    if ($opt.length === 0) {
        $opt = $('<option></option>').val(v).text(label);
        $select.prepend($opt);
    } else {
        $opt = $opt.first();
        $opt.text(label);
        $opt.prependTo($select);
    }
    $select.find("option").prop("selected", false);
    $opt.prop({ hidden: true, selected: true });
}

function appendSentinelOptionVal0($select, mobileLabel) {
    var $o = $('<option></option>').val('0');
    if (isMobileBrowser) {
        $o.text(mobileLabel).prop({ hidden: true, selected: true });
    } else {
        $o.text('');
    }
    $select.append($o);
}

//window.onbeforeunload = function () {
//    return "Data will be lost if you leave the page, are you sure?";
//};
$(document).ready(function () {
    $("#InventoryReports").addClass('active');
    $("#InventoryReports").next('ul.sub').addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liWarehouseTransfer').addClass("active");

    if (!isMobileBrowser) {
        $("#UtilityId").chosen({ placeholder_text_single: "Select Utility", no_results_text: "Oops, nothing found!" });
        $('#UtilityId').trigger('chosen:updated');

        $("#ProjectId").chosen({ placeholder_text_single: "Select Project", no_results_text: "Oops, nothing found!" });
        $('#ProjectId').trigger('chosen:updated');

        $("#FromWarehouseId").chosen({ placeholder_text_single: "From Warehouse", no_results_text: "Oops, nothing found!" });
        $('#FromWarehouseId').trigger('chosen:updated');

        $("#ToWarehouseId").chosen({ placeholder_text_single: "To Warehouse", no_results_text: "Oops, nothing found!" });
        $('#ToWarehouseId').trigger('chosen:updated');

        $("#ProductId").chosen({ placeholder_text_single: "Select Product", no_results_text: "Oops, nothing found!" });
        $('#ProductId').trigger('chosen:updated');
    } else {
        $("#UtilityId option").filter(function () { return $.trim($(this).text()) === ""; }).remove();
        applyMobileNativePlaceholder($("#UtilityId"), "0", "Select Utility");
        applyMobileNativePlaceholder($("#ProjectId"), "0", "Select Project");
        applyMobileNativePlaceholder($("#FromWarehouseId"), "0", "From Warehouse");
        applyMobileNativePlaceholder($("#ToWarehouseId"), "0", "To Warehouse");
        applyMobileNativePlaceholder($("#ProductId"), "0", "Select Product");
    }

    $('#dynamic-table').DataTable({ "language": { "emptyTable": "No Data To Show" }, "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',  });

    $("#btnupdate").hide();

    $('#divFinalSaveCancel').hide();

});


$('#UtilityId').change(function () {

    $('#ProjectId').empty().trigger('chosen:updated');
    $('#FromWarehouseId').empty().trigger('chosen:updated');
    $('#ToWarehouseId').empty().trigger('chosen:updated');
    $('#ProductId').empty().trigger('chosen:updated');


    var value = $("#UtilityId option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetProjectList = $('#urlGetProject').val();
    $.ajax({
        type: 'GET',
        data: { "UtilityId": value.val() },
        url: urlGetProjectList,
        success: function (data) {
            $('#ProjectId').empty();
            if (data != null) {
                appendSentinelOptionVal0($('#ProjectId'), "Select Project");
                $.each(data, function (index, item) {
                    $('#ProjectId').append($('<option></option>').val(item.ProjectId).html(item.ProjectName));
                });
            } else if (isMobileBrowser) {
                appendSentinelOptionVal0($('#ProjectId'), "Select Project");
            }
            $('#ProjectId').trigger('chosen:updated');

            if (isMobileBrowser) {
                applyMobileNativePlaceholder($("#FromWarehouseId"), "0", "From Warehouse");
                applyMobileNativePlaceholder($("#ToWarehouseId"), "0", "To Warehouse");
                applyMobileNativePlaceholder($("#ProductId"), "0", "Select Product");
            }

            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });

});


$('#ProjectId').change(function () {

    $('#FromWarehouseId').empty().trigger('chosen:updated');
    $('#ToWarehouseId').empty().trigger('chosen:updated');
    $('#ProductId').empty().trigger('chosen:updated');
    var value = $("#ProjectId option:selected");

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlGetWarehouseAndProduct = $('#urlGetWarehouseAndProduct').val();
    $.ajax({
        type: 'GET',
        data: { "ProjectId": value.val() },
        url: urlGetWarehouseAndProduct,
        success: function (data) {

            $('#FromWarehouseId').empty();
            $('#ToWarehouseId').empty();

            if (data.WarehouseList != null && data.WarehouseList.length > 0) {
                // Add placeholder option
                appendSentinelOptionVal0($('#FromWarehouseId'), "From Warehouse");
                appendSentinelOptionVal0($('#ToWarehouseId'), "To Warehouse");

                // Append warehouses
                $.each(data.WarehouseList, function (index, item) {
                    $('#FromWarehouseId').append($('<option></option>').val(item.WarehouseId).html(item.WarehouseName));
                    $('#ToWarehouseId').append($('<option></option>').val(item.WarehouseId).html(item.WarehouseName));
                });

                // ✅ Default selection for desktop
                $('#FromWarehouseId').val(data.WarehouseList[0].WarehouseId).trigger('chosen:updated');
                $('#ToWarehouseId').val(data.WarehouseList[1] ? data.WarehouseList[1].WarehouseId : data.WarehouseList[0].WarehouseId).trigger('chosen:updated');

                // ✅ Default selection for mobile
                if (isMobileBrowser) {
                    $('#FromWarehouseId').val(data.WarehouseList[0].WarehouseId);
                    $('#ToWarehouseId').val(data.WarehouseList[1] ? data.WarehouseList[1].WarehouseId : data.WarehouseList[0].WarehouseId);
                }

            } else if (isMobileBrowser) {
                appendSentinelOptionVal0($('#FromWarehouseId'), "From Warehouse");
                appendSentinelOptionVal0($('#ToWarehouseId'), "To Warehouse");
            }

            $('#FromWarehouseId').trigger('chosen:updated');
            $('#ToWarehouseId').trigger('chosen:updated');

            // Product List
            $('#ProductId').empty();
            if (data.ProductList != null && data.ProductList.length > 0) {
                appendSentinelOptionVal0($('#ProductId'), "Select Product");
                $.each(data.ProductList, function (index, item) {
                    $('#ProductId').append($('<option></option>').val(item.Id).html(item.FieldValue));
                });
            } else if (isMobileBrowser) {
                appendSentinelOptionVal0($('#ProductId'), "Select Product");
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


    var warehouseId = $("#FromWarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select from warehouse", "Notification");
        return;
    }
    var toWarehouseId = $("#ToWarehouseId").val();
    if (toWarehouseId === '0' || toWarehouseId === '') {
        var $toast = toastr["error"]("Please, select to warehouse", "Notification");
        return;
    }
    if (warehouseId === toWarehouseId) {
        var $toast = toastr["error"]("From warehouse and To warehouse must be different.", "Notification");
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
    var serialnumbers = [];

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
    
    var warehouseId = $("#FromWarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select from warehouse", "Notification");
        return;
    }
    var toWarehouseId = $("#ToWarehouseId").val();
    if (toWarehouseId === '0' || toWarehouseId === '') {
        var $toast = toastr["error"]("Please, select to warehouse", "Notification");
        return;
    }
    if (warehouseId === toWarehouseId) {
        var $toast = toastr["error"]("From warehouse and To warehouse must be different.", "Notification");
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
        data: { warehouseId: warehouseId, toWarehouseId: toWarehouseId, projectId: ProjectId, productId: ProductId, quantity: quantity, RecordType: RecordType, serialNos: serialnumbers, isGenerateBarcode: isGenerateBarcode },
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
        $('#FromWarehouseId').prop("disabled", false).trigger('chosen:updated');
        $('#ToWarehouseId').prop("disabled", false).trigger('chosen:updated');
        $('#UtilityId').prop("disabled", false).trigger('chosen:updated');       
        $('#ProjectId').prop("disabled", false).trigger('chosen:updated');

        $('#divFinalSaveCancel').hide();
        resetData();
    }
    else {
        // Lock utility / project / warehouses so this batch stays consistent with server temp data
        $('#FromWarehouseId').prop("disabled", true).trigger('chosen:updated');
        $('#ToWarehouseId').prop("disabled", true).trigger('chosen:updated');
        $('#UtilityId').prop("disabled", true).trigger('chosen:updated');       
        $('#ProjectId').prop("disabled", true).trigger('chosen:updated');

        $('#divFinalSaveCancel').show();
        // Keep product, quantity, and barcode fields active so more lines can be added
        $('#ProductId').prop("disabled", false).trigger('chosen:updated');
        $('#txtQuantity').prop("readonly", false);
        $('#txtBarcode').prop("readonly", false);
        $('#chkGenerateBarcode').prop("disabled", false);
    }
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
                    $('#FromWarehouseId').val("0").trigger('chosen:updated');
                    $('#FromWarehouseId').prop("disabled", false).trigger('chosen:updated');
                    $('#ToWarehouseId').val("0").trigger('chosen:updated');
                    $('#ToWarehouseId').prop("disabled", false).trigger('chosen:updated');
                }
                else {
                    $('#FromWarehouseId').prop("disabled", true).trigger('chosen:updated');
                    $('#ToWarehouseId').prop("disabled", true).trigger('chosen:updated');
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

    var warehouseId = $("#FromWarehouseId").val();
    if (warehouseId === '0' || warehouseId === '') {
        var $toast = toastr["error"]("Please, select from warehouse", "Notification");
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
        $('#FromWarehouseId').empty();
        $('#FromWarehouseId').trigger('chosen:updated');
        $('#ToWarehouseId').empty();
        $('#ToWarehouseId').trigger('chosen:updated');
        $('#ProductId').empty();
        $('#ProductId').trigger('chosen:updated');

        if (isMobileBrowser) {
            applyMobileNativePlaceholder($("#UtilityId"), "0", "Select Utility");
            applyMobileNativePlaceholder($("#ProjectId"), "0", "Select Project");
            applyMobileNativePlaceholder($("#FromWarehouseId"), "0", "From Warehouse");
            applyMobileNativePlaceholder($("#ToWarehouseId"), "0", "To Warehouse");
            applyMobileNativePlaceholder($("#ProductId"), "0", "Select Product");
        }

    }
    else {
        $('#FromWarehouseId').prop("disabled", true).trigger('chosen:updated');
        $('#ToWarehouseId').prop("disabled", true).trigger('chosen:updated');
    }
});

function resetData() {

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
    var fromWarehouseId = $('#FromWarehouseId').val();
    console.log("DEBUG FromWarehouseId:", fromWarehouseId); // check karo
    var toWarehouseId = $('#ToWarehouseId').val();
    var projectId = $('#ProjectId').val();

    // Validation
    if (!fromWarehouseId || fromWarehouseId === '0') { toastr.error("Select From Warehouse"); return; }
    if (!toWarehouseId || toWarehouseId === '0') { toastr.error("Select To Warehouse"); return; }
    if (fromWarehouseId === toWarehouseId) { toastr.error("From and To Warehouse must be different"); return; }

    var transferList = [];
    $('#dynamic-table tbody tr').each(function () {
        var serialNumber = $(this).find('input.serialNumber').val();
        var productId = $(this).find('input[id^="hndProductId"]').val();

        if (serialNumber) {
            transferList.push({
                SerialNumber: serialNumber,
                ProductId: parseInt(productId),
                FromWarehouseId: parseInt(fromWarehouseId),
                ToWarehouseId: parseInt(toWarehouseId),
                ProjectId: parseInt(projectId)
            });
        }
    });

    // Call controller
    $.ajax({
        type: 'POST',
        url: $('#urlWarehouseTransferSave').val(),
        contentType: 'application/json',
        data: JSON.stringify({ transferList: transferList, projectId: parseInt(projectId) }),
        success: function (resp) {
            if (resp.success) {
                toastr.success("WarehouseTransfer saved successfully!");
            } else {
                toastr.error(resp.message || "Error saving WarehouseTransfer");
            }
        },
        error: function () {
            toastr.error("Error saving WarehouseTransfer");
        }
    });
});

$('#btnCancelSave').click(function () {
    //$("#WarehouseId").val('0').prop("disabled", false).trigger("chosen:updated");
    //$('#ProductId').val("0").trigger('chosen:updated');
    $('#UtilityId').val("0").trigger('chosen:updated');
    $('#ProjectId').empty();
    $('#ProjectId').trigger('chosen:updated');
    $('#FromWarehouseId').empty();
    $('#FromWarehouseId').trigger('chosen:updated');
    $('#ToWarehouseId').empty();
    $('#ToWarehouseId').trigger('chosen:updated');
    $('#ProductId').empty();
    $('#ProductId').trigger('chosen:updated');
    if (isMobileBrowser) {
        applyMobileNativePlaceholder($("#UtilityId"), "0", "Select Utility");
        applyMobileNativePlaceholder($("#ProjectId"), "0", "Select Project");
        applyMobileNativePlaceholder($("#FromWarehouseId"), "0", "From Warehouse");
        applyMobileNativePlaceholder($("#ToWarehouseId"), "0", "To Warehouse");
        applyMobileNativePlaceholder($("#ProductId"), "0", "Select Product");
    }
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
            "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
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
function loadWarehouses(projectId) {
    $.ajax({
        url: $('#urlGetWarehouseAndProduct').val(),
        type: 'GET',
        data: { ProjectId: projectId },
        success: function (data) {
            $('#FromWarehouseId').empty();
            $('#ToWarehouseId').empty();

            $.each(data.WarehouseList, function (i, w) {
                $('#FromWarehouseId').append('<option value="' + w.WarehouseId + '">' + w.WarehouseName + '</option>');
                $('#ToWarehouseId').append('<option value="' + w.WarehouseId + '">' + w.WarehouseName + '</option>');
            });

            // **Default select first warehouse**
            if (data.WarehouseList.length > 0) {
                $('#FromWarehouseId').val(data.WarehouseList[0].FKWarehouseId || data.WarehouseList[0].Id).trigger('chosen:updated');
                $('#ToWarehouseId').val(data.WarehouseList[1] ? (data.WarehouseList[1].FKWarehouseId || data.WarehouseList[1].Id) : data.WarehouseList[0].Id).trigger('chosen:updated');
            }
        },
        error: function () {
            toastr.error("Error loading warehouses");
        }
    });
}


