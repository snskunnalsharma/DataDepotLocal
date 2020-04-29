


$(document).ready(function () {

    $("#Administration").addClass('active');
    $("#subAdministration").addClass('block');
    $("#liUploadCityLogoMaster").addClass('active');

    $("#ddlState").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlCity").chosen({ no_results_text: "Oops, nothing found!" });

    $("#ddlCustomer").chosen({ no_results_text: "Oops, nothing found!" });


    debugger;
    $('#UploadDiv').hide();
    BindDataTable();

    BindStateList();

    BindCustomerList();

    $("#btn_Upload").click(function () {
        debugger;

        var stateid = $('#ddlState option:selected').val();

        var cityid = $('#ddlCity option:selected').val();

        var customerId = $('#ddlCustomer option:selected').val();

        var imageName = $('#txtImageName').val();

        imageName = imageName.split(' ').join('');

        var editCityLogoId = $("#hdnCiltyLogoId").val();

        if (editCityLogoId.length == 0) {

            editCityLogoId = 0;

        }

        if (stateid == "0" || stateid === undefined) {
            var $toast = toastr["error"]('Please select state first', 'Notification');
            return;
        }
        else if (cityid == "0" || cityid === undefined) {
            var $toast = toastr["error"]('Please select city first', 'Notification');
            return;
        }
        else if (customerId == "0" || customerId === undefined) {
            var $toast = toastr["error"]('Please select customer first', 'Notification');
            return;
        }
        else if (imageName.length == 0 || imageName === undefined) {
            var $toast = toastr["error"]("Please, enter image name first", "Error Notification");
            return;
        }
        else if ($("#fileInput").val() == '') {
            var $toast = toastr["error"]("Please, select any file first to upload", "Error Notification");
            return;
        }


        debugger;
        waitingDialog.show('Please Wait Uploading...');
        var fileUploadAction = $("#urlUploadFile").val();
        var fileInput = document.getElementById('fileInput');
        var xhr = new XMLHttpRequest();
        xhr.open('POST', fileUploadAction);
        xhr.setRequestHeader('Content-type', 'multipart/form-data');

        xhr.setRequestHeader('X-File-Name', fileInput.files[0].name);
        xhr.setRequestHeader('X-File-Type', fileInput.files[0].type);
        xhr.setRequestHeader('X-File-Size', fileInput.files[0].size);
        xhr.setRequestHeader('stateid', stateid);
        xhr.setRequestHeader('cityid', cityid);
        xhr.setRequestHeader('imageName', imageName);
        xhr.setRequestHeader('editCityLogoId', editCityLogoId);
        xhr.setRequestHeader('customerId', customerId);

        //Sending file in XMLHttpRequest
        xhr.send(fileInput.files[0], stateid, cityid, customerId, imageName, editCityLogoId);

        waitingDialog.hide();
        debugger;
        xhr.onreadystatechange = function () {
            debugger;
            if (xhr.readyState == 4 && xhr.status == 200) {

                var resp = JSON.parse(xhr.responseText);

                if (resp.success == true) {
                    $("#EditCityLogo").attr('src', '');
                    // $("#EditCityLogo").attr('src', resp.ImagePath);
                    $("#fileInput").val('');


                    $('.chosen-select').val("0").trigger('chosen:updated');
                    $("#txtImageName").val('');
                    $("#hdnCiltyLogoId").val('');
                    $('#UploadCityLogo').modal('hide');
                    $("#ImagPriview").hide();

                    BindDataTable();

                }
                else {
                    var $toast = toastr["error"](resp.Data1, " Notification !");
                }
            }
            waitingDialog.hide();
        }


    });

    $("#btn_UploadNew").click(function () {


        var urlCreateRecord = $("#urlCreateRecord").val();
        $.ajax({
            type: 'GET',
            url: urlCreateRecord,
            success: function (data) {

                if (data.success) {
                    $('#UploadCityLogo').modal('show');
                    $('.chosen-select').val("0").trigger('chosen:updated');
                    $("#txtImageName").val('');
                    $("#hdnCiltyLogoId").val('');
                    $('#UploadDiv').show();
                    $("#EditCityLogo").attr('src', '');
                }
                else {

                    var $toast = toastr["error"](data.returnMessage, "Notification !");
                }

            },
            error: function (error) {
                console.log(error)
                var $toast = toastr["error"]("Please try again, we are unable to process request.", "Error Notification !");
            }

        });


    });

    $("#btnUploadCancel").click(function () {
       
        $('.chosen-select').val("0").trigger('chosen:updated');
        $("#txtImageName").val('');
        $("#hdnCiltyLogoId").val('');
        $('#UploadCityLogo').modal('hide');
        $("#ImagPriview").hide();
        $("#fileInput").val('');
    });

    $('#ddlState').change(function () {

        var stateid = $('#ddlState option:selected').val();
        if (stateid != undefined && stateid != "0") {

            BindCity(stateid);

        }

    })




});

function BindDataTable() {
    debugger;
    var urlBindTblData = $("#urlGetUploadedImageData").val();
    $.ajax({
        url: urlBindTblData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'POST',
        success: function (dataList) {
            debugger;
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
       "aaData": dataList,
       "aoColumns": [
                           { "mData": "StateName" },
                           { "mData": "CityName" },
                           { "mData": "CustomerName" },
                            { "mData": "ImageName" },
                           { "bSearchable": false, "mData": null },
       ],
       "aoColumnDefs": [
                            {
                                "mRender": function (data, type, full, row) {

                                    return '<a id="' + full.CityLogoID + '" onclick="EditRecord(' + full.CityLogoID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.CityLogoID + ' onclick="DeactivateRecord(' + full.CityLogoID + ',' + full.StateID + ',' + full.CityID + ' );" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';

                                },
                                "aTargets": [4]
                            }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
            //spinner.stop(target);
        }
    });



}

function EditRecord(CityLogoID) {

    waitingDialog.show('Loading Please Wait...');

    $("#EditCityLogo").attr('src', '');

    $('.chosen-select').val("0").trigger('chosen:updated');

    $("#txtImageName").val('');

    $("#hdnCiltyLogoId").val('');

    var urlUpdateLogoRecord = $("#urlUpdateLogoImage").val();

    $.ajax({
        type: 'POST',
        data: { cityLogoId: CityLogoID },
        async: false,
        url: urlUpdateLogoRecord,
        success: function (data) {
           
            if (data.status == true) {

                $('#ddlCustomer').find('option[value=' + data.Obj.FK_CustomerId + ']').attr('selected', 'selected');
                $('#ddlCustomer').trigger('chosen:updated');


                $('#ddlState').find('option[value=' + data.Obj.FK_StateId + ']').attr('selected', 'selected');
                $('#ddlState').trigger('chosen:updated');

                BindCity(data.Obj.FK_StateId);

                $('#ddlCity').find('option[value=' + data.Obj.FK_CityId + ']').attr('selected', 'selected');
                $('#ddlCity').trigger('chosen:updated');

                $("#txtImageName").val(data.Obj.ImageName);

                $("#EditCityLogo").attr('src', data.Obj.strImagePath);

                $('#hdnCiltyLogoId').val(data.Obj.CityLogoID);


                $('#UploadCityLogo').modal('show');
                $("#ImagPriview").show();

                waitingDialog.hide();

            }
            else {
                waitingDialog.hide();
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }


        },
        error: function (error) {
            console.log(error)
            var $toast = toastr["error"]("Please try again, we are unable to process request.", " Notification !");
        }

    });


}


function DeactivateRecord(CityLogoID) {
    debugger;

    var urlDeactivateLogoRecord = $("#urlDeactivateCityLogo").val();
    $.ajax({
        type: 'POST',
        data: { "cityLogoId": CityLogoID },
        async: false,
        url: urlDeactivateLogoRecord,
        success: function (data) {
            debugger;
            if (data.success == true) {
                BindDataTable();
                waitingDialog.hide();
                var $toast = toastr["success"]("Record deactiveted successfully", "Success Notification !");
            }
            else {
                waitingDialog.hide();
                var $toast = toastr["error"]("Record is not deactiveted", "Notification !");
            }


        },
        error: function (error) {
            console.log(error)
            var $toast = toastr["error"]("Please try again, we are unable to process request.", "Error Notification !");
        }

    });

}


function BindStateList() {
    debugger;
    var urlStateData = $('#urlGetStateList').val();

    $.ajax({
        type: 'POST',
        url: urlStateData,
        async: false,
        success: function (data) {
            debugger;
            $('#ddlState').empty();

            //StateData = data;
            $.each(data, function (key, value) {
                if (key == 0) {
                    $('#ddlState').append(
                      $('<option></option>').val("").html(""));
                }
                $('#ddlState').append(
                      $('<option></option>').val(value.StateID).html(value.StateName)
                );


            });

            $('#ddlState').trigger('chosen:updated');
        },
        error: function (error) {
            console.log(error)
            //alert("something seems wrong");
        }

    });

}


function BindCustomerList() {
    
    var urlCustomerData = $('#urlGetCustomerList').val();

    $.ajax({
        type: 'POST',
        url: urlCustomerData,
        async: false,
        success: function (data) {
           
            $('#ddlCustomer').empty();

            $.each(data, function (key, value) {
                if (key == 0) {
                    $('#ddlCustomer').append(
                      $('<option></option>').val("").html(""));
                }
                $('#ddlCustomer').append(
                      $('<option></option>').val(value.CustomerID).html(value.CustomerName)
                );

            });

            $('#ddlCustomer').trigger('chosen:updated');
        },
        error: function (error) {
            console.log(error)
            //alert("something seems wrong");
        }

    });

}


function BindCity(stateid) {
    debugger;
    var urlCityData = $('#urlGetCityLisyByStateId').val();
    $.ajax({
        type: 'GET',
        data: { "stateId": stateid },
        async: false,
        url: urlCityData,
        success: function (data) {

            $('#ddlCity').empty();
            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCity').append(
                      $('<option></option>').val('').html(""));
                }

                $('#ddlCity').append(
                       $('<option></option>').val(item.CityID).html(item.CityName)
                 );
            });

            $('#ddlCity').trigger('chosen:updated');
        },

    });
}