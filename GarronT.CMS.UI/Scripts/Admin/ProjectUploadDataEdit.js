var spinner, opts, target;
var oTable, validator;
var streetName;

$(document).ready(function () {

    //#region validation code
    validator = $('#UploadForm').validate({
        debug: true, onsubmit: false,
        rules: {
            Latitude: {
                required: false,

            },

            Longitude: {
                required: false,

            },

            Street: {
                required: true,
            },

        },
        messages: {
            Latitude: {
                required: "Latitude Required",


            },
            Longitude: {
                required: "Longitude Required",

            },
            Street: {
                required: "Street Required",
            },

            //Country: function (Country) {
            //    $(Country).removeClass("error");
            //}
        },

        submitHandler: function (form) {

            //EditRecord();
            //$("#element_to_pop_up").dialog("close");
            //return false;
        },
        invalidHandler: function () {


        }
    });
    //#endregion

    bindActiveTable();

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});




$('body').on('keypress', '.Decimal-Only', function (event) {
    return isNumber(event, this)
});

function isNumber(evt, element) {

    var charCode = (evt.which) ? evt.which : event.keyCode

    if (
        (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;

    return true;
}
//#region BindActiveTable

function bindActiveTable() {
    debugger;
    var GetActiveUploadedData = $("#GetActiveUploadedData").val();

    $.ajax({
        url: GetActiveUploadedData,
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        dataType: "json",
        type: 'GET',
        success: function (msg) {
            debugger
            if (msg.LatLongButtonShow === false) {
                $("#btnCalculate").css("display", "none");
            }


            $("#Pname").text('Project - ' + msg.projectName);

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
       "aaData": msg.uploadedDataList,
       "aaSorting": [[4, "asc"]],
       "aoColumns": [
                             { "mData": "Cycle" },
                              { "mData": "Route" },
                              { "mData": "Account" },
                              { "mData": "Street" },
                               { "mData": "Latitude" },
                                { "mData": "Longitude" },
                                { "mData": "CreatedOn" },

                           { "bSearchable": false, "mData": null },

       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      return '<a href="#" id=' + full.ID + ' onclick="EditUploadedRecord(' + full.ID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
                                                      //  return '<a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CountryID + ' onclick="Deactivate(' + full.CountryID + ');" class="btn btn-danger btn-xs" role="button" alt="Delete" title="Delete"><i class="fa fa-ban"></i></a>';

                                                  },
                                                  "aTargets": [7]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }
    });

}
//#endregion

//#region Edit (Get Records for edit)

//#This method don't use
function EditUploadedRecord_old(uploadedID) {
    debugger;
    $('#UploadForm').validate().resetForm();

    var GetRecordsbyUploadedID = $("#urlGetRecordsbyUploadedID").val();

    $.ajax({
        type: 'GET',
        url: GetRecordsbyUploadedID,
        async: true,
        data: { "uploadedID": uploadedID },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            $("#txtUploadID").val(msg.ID);
            $("#txtAccount").val(msg.Account);
            $("#txtLatitude").val(msg.Latitude);
            $("#txtLongitude").val(msg.Longitude);
            $("#txtStreet").val(msg.Street);
            streetName = msg.Street;

            $("#txtCycle").val(msg.Cycle);
            $("#txtRoute").val(msg.Route);
            $("#txtOldMeterType").val(msg.OldMeterType);
            $("#txtOldMeterMake").val(msg.OldMeterMake);
            $("#txtOldMeterNumber").val(msg.OldMeterNumber);
            $("#txtOldMeterSize").val(msg.OldMeterSize);
            $("#txtOldMeterRadioNumber").val(msg.OldMeterRadioNumber);
            $("#txtCustomer").val(msg.Customer);

            $("#myModal").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            $("#myModal").draggable({
                handle: ".modal-header"
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

function EditUploadedRecord(uploadedID) {
    debugger;
    $('#UploadForm').validate().resetForm();
    document.getElementById("ModelData").innerHTML = '';
    $('#ModelData').html('');
    var GetRecordsbyUploadedID = $("#urlGetRecordsbyUploadedID").val();

    $.ajax({
        type: 'GET',
        url: GetRecordsbyUploadedID,
        async: true,
        data: { "uploadedID": uploadedID },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            if (msg.status == true) {
                debugger;
                var a = msg.htmlData;
                $("#ModelData").append(a);
            }
            else {
                var $toast = toastr["error"](msg.errorData, "Error Notification");
            }
            $("#myModal").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            $("#myModal").draggable({
                handle: ".modal-header"
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

$("#btnSave").click(function () {
    debugger;
    if ($("#UploadForm").valid()) {
        UpdateUploadedData()
    }

})
var StreetChanged = false;
//#This method don't use
function UpdateUploadedData_old() {
    StreetChanged = false;
    var previousValue = streetName;
    var currentValue = $("#txtStreet").val();

    if (currentValue != previousValue) {
        StreetChanged = true;
    }

    var UploadformData = $("#UploadForm").serializeArray();
    debugger

    var UploadData = {};
    $.each(UploadformData, function ()
    {
        if (UploadData[this.name] !== undefined)
        {
            if (!UploadData[this.name].push)
            {
                UploadData[this.name] = [UploadData[this.name]];
            }
            UploadData[this.name].push(this.value || '');
        }
        else
        {
            UploadData[this.name] = this.value || '';
        }
    });

    var urlUpdateUploadData = $("#urlUpdateUploadData").val();
    debugger
    $.ajax({
        url: urlUpdateUploadData,
        data: { uploadedData: UploadData, StreetChanged: StreetChanged },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModal").modal('hide');
                bindActiveTable();

                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

StreetChanged = false;
function UpdateUploadedData() {
 
    var urlUpdateUploadData = $("#urlUpdateUploadData").val();
    debugger
    $.ajax({
        url: urlUpdateUploadData,
        data: $("#UploadForm").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg.success == true)
            {
                $("#myModal").modal('hide');
                bindActiveTable();
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else
            {
                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}


$("#btnCancel").click(function () {

    //$("#txtAccount").val('');
    //$("#txtLatitude").val('');
    //$("#txtLongitude").val('');
    //$("#txtStreet").val('');

    document.getElementById("ModelData").innerHTML = '';
    $('#ModelData').html('');

    $("#myModal").modal('hide');
})


//#endregion


function wait(ms) {
    var start = new Date().getTime();
    var end = start;
    while (end < start + ms) {
        end = new Date().getTime();
    }
}

$("#btnCalculate").click(function () {
    debugger;
    

    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Calculate Pending Latitude & Longitude!',
        content: 'Do you want to start the latitude & longitude calculation for the pending records?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        // var hv = $('#ProjectID').val();
                        var UrlCalculatePendingLatLon = $('#UrlCalculatePendingLatLon').val();

                        $.ajax({
                            url: UrlCalculatePendingLatLon,
                            type: 'POST',                            
                            success: function (data) {
                                debugger;
                                if (data.success == true) {
                                    waitingDialog.hide();

                                    //  var $toast = toastr["success"](data.resultMessage, "Success Notification");             
                                    window.location.href = $('#GetprojectIndex').val();

                                }
                                else {
                                    waitingDialog.hide();
                                    var $toast = toastr["success"](data.resultMessage, "Notification");
                                }
                                


                            },
                            error: function () {
                                var $toast = toastr["error"]("something seems wrong", "Error Notification");
                                waitingDialog.hide();
                            }

                        });
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



});