
var Golbal_InitialID, Golbal_ServiceID;
$(document).ready(function () {

    $("#Reports").addClass('active');
    $("#subReports").addClass('block');

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liReport');
    listate.addClass("active");


    debugger;



    var user = getURLParameters('userId');
    var Proj = getURLParameters('projectId');
    var upload = getURLParameters('uploadId');
    var form = getURLParameters('reportFormId');





    $('#txtUserId').val(getURLParameters('userId'));
    $('#txtprojectId').val(getURLParameters('projectId'));
    $('#txtaccountinifiledId').val(getURLParameters('uploadId'));
    $('#txtreportFormId').val(getURLParameters('reportFormId'));

    SectionShowData();



});



function getURLParameters(paramName) {
    var sURL = window.document.URL.toString();
    if (sURL.indexOf("?") > 0) {
        var arrParams = sURL.split("?");
        var arrURLParams = arrParams[1].split("&");
        var arrParamNames = new Array(arrURLParams.length);
        var arrParamValues = new Array(arrURLParams.length);

        var i = 0;
        for (i = 0; i < arrURLParams.length; i++) {
            var sParam = arrURLParams[i].split("=");
            arrParamNames[i] = sParam[0];
            if (sParam[1] != "")
                arrParamValues[i] = unescape(sParam[1]);
            else
                arrParamValues[i] = "No Value";
        }

        for (i = 0; i < arrURLParams.length; i++) {
            if (arrParamNames[i] == paramName) {
                //alert("Parameter:" + arrParamValues[i]);
                return arrParamValues[i];
            }
        }
        return "No Parameters Found";
    }
}



function SectionShowData() {


    waitingDialog.show('Loading Please Wait...');
    var userId = $('#txtUserId').val();
    var projectId = $('#txtprojectId').val();
    var accinifiledId = $('#txtaccountinifiledId').val();
    var reportFId = $('#txtreportFormId').val();

    if (userId != null && projectId != null && accinifiledId != null && reportFId != null) {

        var urlGetData = $('#urlDisplayData').val();
        $.ajax({
            type: 'GET',
            url: urlGetData,
            data: {
                userId: userId,
                projectId: projectId,
                uploadId: accinifiledId,
                reportFormId: reportFId
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.status == true) {
                    debugger;
                    var a = msg.htmlData;
                    $("#FieldData").append(a);

                    $("#ProjectName").html(msg.Details1.ProjectName);
                    $("#InstallerName").html(msg.Details1.InstallerName);
                    //$("#FormName").html(msg.Details1.FromName);

                    var b = msg.htmlData1;
                    $("#FormName_and_CustomerAdd").append(b);

                }
                else {
                    var $toast = toastr["error"](msg.errorData, "Error Notification");
                }
                waitingDialog.hide();
            },
            error: function () {
                waitingDialog.hide();
                var $toast = toastr["error"]("Error", "Error Notification");

            }

        });
        //waitingDialog.hide();
    }

}



$('#btnpdfPrev').click(function () {
    NextPreviousData(false);
});
$('#btnpdfNext').click(function () {
    NextPreviousData(true);
});


function NextPreviousData(strRecord) {
    waitingDialog.show('Loading Please Wait...');
    var userId = $('#txtUserId').val();
    var projectId = $('#txtprojectId').val();
    var accinifiledId = $('#txtaccountinifiledId').val();
    var reportFId = $('#txtreportFormId').val();
    var urlGetData = $('#getNextPreviousRedirectDataurl').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { _valRecord: strRecord, userId: userId, projectId: projectId, uploadId: accinifiledId, reportFormId: reportFId },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            debugger;
            if (msg.Sucess == false) {
                if (msg.returnMsg == 1) {
                    $('#btnpdfNext').hide();
                    //var $toast = toastr["error"]("This is the last record", 'Notification');
                    waitingDialog.hide();
                }
                else {
                    $("#btnpdfPrev").hide();
                    // var $toast = toastr["error"]("This is the first record", 'Notification');
                    waitingDialog.hide();
                }

            }
            else {
                waitingDialog.hide();
                //$("#FieldData").empty();
                //var a = msg.htmlData;
                //$("#FieldData").append(a);
                //$("#ProjectName").html(msg.Details1.ProjectName);
                //$("#InstallerName").html(msg.Details1.InstallerName);
                //$("#FormName").html(msg.Details1.FromName);
                var createurl = $("#urlRedirectCreate").val();
                //long userId, int projectId, long uploadId, long reportFormId
                createurl = createurl + "?userId=" + msg.InstallerId + "&projectId=" + projectId + "&uploadId=" + msg.UploadId + "&reportFormId=" + msg.FormId;
                location.href = createurl;
            }
            // waitingDialog.hide();
        }


    });
    //waitingDialog.hide();

}



$('#btnCancel').click(function () {
    $("#myModal").modal('hide');
    Golbal_InitialID = "";
    Golbal_ServiceID = "";
});
$('#btnClose').click(function () {
    $("#myModal").modal('hide');
    Golbal_InitialID = "";
    Golbal_ServiceID = "";
});






$('#Submit').click(function () {
    $.getJSON("/Reports/Upload", null, function (data) {
        $("#stateList").fillSelect(data);
    });
});

$('#btnSaveData').unbind("click").click(function () {

    debugger;
    var requiredFields = $("[required]");
    var errorMessage = "";
    for (var i = 0; i < requiredFields.length; i++) {
        var field = requiredFields[i];
        var fieldType = $(field).attr("type");
        var fieldname = $(field).attr("name");

        if (fieldType == "text" || fieldType == "Number") {
            var fieldvalue = $(field).attr("value");
            if (fieldvalue == "") {
                errorMessage += fieldname + " Required\n";
            }
        }

    }

    if (errorMessage != "") {
        var $toast = toastr["error"](errorMessage);
        return false;
    }


    waitingDialog.show('Saving Data Please Wait...');

    var userId = $('#txtUserId').val();
    var projectId = $('#txtprojectId').val();
    var accinifiledId = $('#txtaccountinifiledId').val();
    var reportFId = $('#txtreportFormId').val();
    var uri = $('#urlSaveData').val();
    $.ajax({
        url: uri,
        data: $("#formsave").serialize() + "&userId=" + userId +
            "&projectId=" + projectId + "&uploadId=" + accinifiledId + "&reportFormId=" + reportFId + "",
        type: 'POST',
        success: function (msg) {
            if (msg.result == true) {
                var $toast = toastr["success"]("successfully updated.", "Success Notification");
            }
            else {
                var $toast = toastr["error"]("failed to update the record.", "Error Notification");
            }
            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });
});



var iv2 = null
var IsServiceField = false;
var CurrentImageItems = [];

function reportFieldImages(FieldDataMasterId, fieldName, reportFieldId) {


    $("#serviceImageList").css("display", "none");
    IsServiceField = false;
    CurrentImageItems = [];
    $("h4.modal-title").text('');

    $('#txtCurrentFieldName').val('');
    $('#txtCurrentImageFieldId').val('');
    $('#txtCurrentReportFieldId').val('');
    $('#txtCurrentImageId').val('');
    $('#txtCurrentServiceId').val('');
    $('#txtIsAdditionalService').val('');

    waitingDialog.show('Loading Please Wait...');

    //fieldName

    var urlGetData = $('#urlGetAllHtmlImagelink').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { FieldDataMasterId: FieldDataMasterId },
        success: function (msg) {
            debugger;
            if (msg.ImageList == null || msg.ImageList.length > 0) {

                var i = 1;

                $.each(msg.ImageList, function (index, item) {

                    var defaults = { ImagePath: item.ImagePath, ImageId: item.ImageId, ImageName: "foo", SerialNo: i, IsService: false };
                    CurrentImageItems.push(defaults);
                    i++;
                });

                //if  image found  make icon color green.
                if ($("#" + reportFieldId).hasClass("btn-danger")) {
                    $("#" + reportFieldId).removeClass("btn-danger");
                    $("#" + reportFieldId).addClass("btn-success");
                }

                //set image to control


                $('#txtCurrentImageId').val(CurrentImageItems[0].ImageId);

                if (iv2 === null) {
                    var btnPreShow = false;
                    var btnNxeShow = false;
                    // set previous next button avilable
                    if (CurrentImageItems.length > 1) {

                        btnPreShow = false;
                        btnNxeShow = true;
                    }
                    else {
                        btnPreShow = false;
                        btnNxeShow = false;
                    }

                    //initialize image control
                    iv2 = $("#viewer2").iviewer({ src: CurrentImageItems[0].ImagePath, btnPrev: btnPreShow, btnNex: btnNxeShow });
                }
                else {
                    if (CurrentImageItems.length > 1) {
                        $(".iviewer_next_image").css("display", "block");
                    }
                    else {
                        $(".iviewer_next_image").css("display", "none");
                    }

                    iv2.iviewer('loadNextImage', CurrentImageItems[0].ImagePath);
                    $(".iviewer_remove_image").css("display", "block");
                    $(".iviewer_previous_image").css("display", "none");
                }

            }
            else {
                //  $("#viewer2").empty();


                //if no image found  make icon color red.
                if ($("#" + reportFieldId).hasClass("btn-success")) {
                    $("#" + reportFieldId).removeClass("btn-success");
                    $("#" + reportFieldId).addClass("btn-danger");
                }
                if (iv2 === null) {
                    iv2 = $("#viewer2").iviewer({ src: 'https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image', btnPrev: false, btnNex: false });
                }
                else {
                    iv2.iviewer('loadNextImage', 'https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image');
                }

                $(".iviewer_previous_image").css("display", "none");
                $(".iviewer_next_image").css("display", "none");
                $(".iviewer_remove_image").css("display", "none");

            }

            $("h4.modal-title").text(fieldName);
            $("#myModal").modal({ show: true, backdrop: 'static', keyboard: false });
            $('#txtCurrentFieldName').val(fieldName);
            $('#txtCurrentImageFieldId').val(FieldDataMasterId);
            $('#txtCurrentReportFieldId').val(reportFieldId);
            //txtCurrentImageId  
            waitingDialog.hide();
        },
        error: function (msg) {
            var $toast = toastr["error"]('somthing seems wrong', 'Notification');
            waitingDialog.hide();
        }

    });
}

function reportServiceFieldImages(initialId, serviceId, serviceName, reportFieldId, IsAdditionalService) {
    debugger;
    $("#serviceImageList").css("display", "block");
    IsServiceField = true;
    CurrentImageItems = [];
    $("h4.modal-title").text('');

    $('#txtCurrentFieldName').val('');
    $('#txtCurrentImageFieldId').val('');
    $('#txtCurrentReportFieldId').val('');
    $('#txtCurrentImageId').val('');
    $('#txtCurrentServiceId').val('');
    $('#txtIsAdditionalService').val('');


    waitingDialog.show('Loading Please Wait...');

    var urlGetData = $('#urlGetHtmlImageForServices').val();
    $.ajax({
        type: 'GET',
        async: true,
        url: urlGetData,
        data: {
            reportFieldId: initialId,
            serviceId: serviceId
        },
        cache: false,
        success: function (msg) {

            var pictureList = msg.PictureList;
            $('#serviceImageList').empty();
            $.each(pictureList, function (i, item) {
                $('#serviceImageList').append($('<option>', { value: item, text: item }));
            });





            debugger;


            if (msg.ImageList == null || msg.ImageList.length > 0) {

                var i = 1;

                $.each(msg.ImageList, function (index, item) {

                    var defaults = { ImagePath: item.ImagePath, ImageId: item.ImageId, ImageName: item.ImageName, SerialNo: i, IsService: true };
                    CurrentImageItems.push(defaults);
                    i++;
                });


                //if  image found  make icon color green.
                if ($("#IMS_" + serviceId).hasClass("btn-danger")) {
                    $("#IMS_" + serviceId).removeClass("btn-danger");
                    $("#IMS_" + serviceId).addClass("btn-success");
                }

                $('#txtCurrentImageId').val(CurrentImageItems[0].ImageId);
                if (iv2 === null) {
                    var btnPreShow = false;
                    var btnNxeShow = false;
                    // set previous next button avilable
                    if (CurrentImageItems.length > 1) {

                        btnPreShow = false;
                        btnNxeShow = true;
                    }
                    else {
                        btnPreShow = false;
                        btnNxeShow = false;
                    }

                    //initialize image control
                    iv2 = $("#viewer2").iviewer({ src: CurrentImageItems[0].ImagePath, btnPrev: btnPreShow, btnNex: btnNxeShow });
                }
                else {
                    if (CurrentImageItems.length > 1) {
                        $(".iviewer_next_image").css("display", "block");
                    }
                    else {
                        $(".iviewer_next_image").css("display", "none");
                    }

                    iv2.iviewer('loadNextImage', CurrentImageItems[0].ImagePath);
                    $(".iviewer_remove_image").css("display", "block");
                    $(".iviewer_previous_image").css("display", "none");
                }

                $("h4.modal-title").text(serviceName + "-" + CurrentImageItems[0].ImageName);




            }
            else {
                //  $("#viewer2").empty();

                //if no image found  make icon color red.
                if ($("#IMS_" + serviceId).hasClass("btn-success")) {
                    $("#IMS_" + serviceId).removeClass("btn-success");
                    $("#IMS_" + serviceId).addClass("btn-danger");
                }
                $("h4.modal-title").text(serviceName + "-");

                if (iv2 === null) {
                    iv2 = $("#viewer2").iviewer({ src: 'https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image', btnPrev: false, btnNex: false });
                }
                else {
                    iv2.iviewer('loadNextImage', 'https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image');
                }

                $(".iviewer_previous_image").css("display", "none");
                $(".iviewer_next_image").css("display", "none");
                $(".iviewer_remove_image").css("display", "none");
            }




            $('#txtCurrentFieldName').val(serviceName);

            $('#txtCurrentImageFieldId').val(initialId);

            $('#txtCurrentReportFieldId').val(reportFieldId);

            $('#txtCurrentServiceId').val(serviceId);

            $('#txtIsAdditionalService').val(IsAdditionalService);



            $("#myModal").modal({ show: true, backdrop: 'static', keyboard: false });

            // $("#myModal").draggable({ handle: ".modal-header" });



            waitingDialog.hide();
        },
        error: function (msg) {
            var $toast = toastr["error"]('somthing seems wrong', 'Notification');
            waitingDialog.hide();
        }
    });

}

$("#btnRotateRight").click(function () {
    waitingDialog.show('Please Wait Updating Image...');
    debugger;
    var Deg = 90;
    var uri = $('#buttonRotateurl').val();
    var aImageId;
    $('#allImages img').each(function () {
        debugger;


        aImageId = $(this).attr('id'); // "this" is the current element in the loop


        $.ajax({
            type: "POST",
            url: uri,
            data: { direction: 'right', attachmentId: aImageId },
            cache: false,
            dataType: 'json',
            success: function (response) {
                debugger;
                var img = new Image();

                // Create var for image source
                var imageSrc = response.newPath;

                // define what happens once the image is loaded.
                img.onload = function () {
                    // Stuff to do after image load ( jQuery and all that )
                    // Within here you can make use of src=imageSrc, 
                    // knowing that it's been loaded.

                    $("#" + aImageId).attr('src', response.newPath);
                    $("#" + aImageId).attr('id', response.newId);
                    $(".imgRotateFlip").elevateZoom({
                        cursor: 'pointer',
                        imageCrossfade: true
                    });
                    waitingDialog.hide();
                };

                //// Attach the source last. 
                //// The onload function will now trigger once it's loaded.
                img.src = imageSrc;


            },
            error: function (error) {
                waitingDialog.hide();
            }
        });

    });

});

$("#btnRotateLeft").click(function () {
    waitingDialog.show('Please Wait Updating Image...');
    debugger;
    var Deg = 90;
    var uri = $('#buttonRotateurl').val();
    var aImageId;
    $('#allImages img').each(function () {
        debugger;

        aImageId = $(this).attr('id'); // "this" is the current element in the loop

    });


    $.ajax({
        type: "POST",
        url: uri,
        // async: true,

        data: { direction: 'left', attachmentId: aImageId },
        cache: false,
        dataType: 'json',
        success: function (response) {

            debugger;

            var img = new Image();

            // Create var for image source
            var imageSrc = response.newPath;

            // define what happens once the image is loaded.
            img.onload = function () {
                // Stuff to do after image load ( jQuery and all that )
                // Within here you can make use of src=imageSrc, 
                // knowing that it's been loaded.

                $("#" + aImageId).attr('src', response.newPath);
                $("#" + aImageId).attr('id', response.newId);
                $(".imgRotateFlip").elevateZoom({
                    cursor: 'pointer',
                    imageCrossfade: true
                });
                waitingDialog.hide();
            };
            //// Attach the source last. 
            //// The onload function will now trigger once it's loaded.
            img.src = imageSrc;
        },
        error: function (error) {
            //var $toast = toastr["error"]('Failed to upload image. Please try again.', 'Notification');
            waitingDialog.hide();
        }
    });

});


$("#btnImageUpload").click(function () {
    debugger;

    var formData = new FormData();

    if ($("#imageFileUpload").val() != "") {

        var pathArray = $("#imageFileUpload").val();

        pathArray = pathArray.replace(/(c:\\)*fakepath\\/i, "");

        if (pathArray != '') {

            var filepath = pathArray.split('.');

            var length = filepath.length;

            if (filepath[length - 1].toLowerCase() != 'jpeg' && filepath[length - 1].toLowerCase() != 'jpg') {

                var $toast = toastr["error"]("Please Select JPEG File format.", "Notification");

                $('#imageFileUpload').val('');

                return false;
            }
        }

        waitingDialog.show('Loading Please Wait...');

        var totalFiles = document.getElementById("imageFileUpload").files.length;

        for (var i = 0; i < totalFiles; i++) {

            var file = document.getElementById("imageFileUpload").files[i];

            formData.append("file", file);

        }




        if (IsServiceField === false) {

            //#region custom field image upload
            formData.append("userId", $("#txtUserId").val());

            formData.append("formId", $("#txtreportFormId").val());

            formData.append("accountInitialFieldId", $("#txtaccountinifiledId").val());

            formData.append("fieldDataMasterId", $("#txtCurrentImageFieldId").val());

            formData.append("fieldName", $("#txtCurrentFieldName").val());

            var webApiUploadCustomeImage = $('#urlwebApiUploadImage').val() + "api/UploadFile/UploadCustomImageFilesFromWeb";

            //  var webApiUploadCustomeImage = "https://localhost:6069/" + "api/UploadFile/UploadCustomImageFilesFromWeb";
            $.ajax({
                type: "POST",

                url: webApiUploadCustomeImage,

                data: formData,

                contentType: 'multipart/form-data',

                contentType: false,

                processData: false,

                success: function (response) {

                    debugger;

                    waitingDialog.hide();

                    $('#imageFileUpload').val('');

                    if (response.status == "Success") {

                        var $toast = toastr["success"]('File uploaded sucessfully.', 'Notification');

                        var FieldDataMasterId = $('#txtCurrentImageFieldId').val();

                        var reportFieldId = $('#txtCurrentReportFieldId').val();

                        var fieldName = $('#txtCurrentFieldName').val();

                        // $("#viewer2").empty();
                        reportFieldImages(FieldDataMasterId, fieldName, reportFieldId);

                    }
                    else if (response.status == "Error") {

                        var $toast = toastr["error"](response.errorMessage, 'Notification');
                    }
                    else {
                        var $toast = toastr["error"]('Failed to upload image. Please try again.', 'Notification');
                    }
                },
                error: function (error) {

                    var $toast = toastr["error"]('Failed to upload image. Please try again.', 'Notification');

                    waitingDialog.hide();

                }
            });

        }
        else {

            //if image upload for service
            var webApiUploadServiceImage = $('#urlwebApiUploadImage').val() + "api/UploadFile/UploadServiceImageFilesFromWeb";

            // var webApiUploadServiceImage = "https://localhost:6069/" + "api/UploadFile/UploadServiceImageFilesFromWeb";

            var ServiceName = $('#txtCurrentFieldName').val();

            var ReportFieldId = $('#txtCurrentReportFieldId').val();

            var MasterFieldId = $('#txtCurrentImageFieldId').val();

            var UserId = $("#txtUserId").val();

            var ServiceId = $('#txtCurrentServiceId').val();

            var ImageName = $('#serviceImageList option:selected').val();

            var FormId = $("#txtreportFormId").val();

            var IsAdditionalService = $('#txtIsAdditionalService').val() === 'true' ? true : false;



            formData.append("userId", UserId);

            formData.append("formId", FormId);

            formData.append("MasterFieldId", MasterFieldId);

            formData.append("ServiceId", ServiceId);

            formData.append("imageName", ImageName);

            formData.append("isAdditionalService", IsAdditionalService);

            //#region Upload Service FieldImage

            $.ajax({

                type: "POST",

                url: webApiUploadServiceImage,

                data: formData,

                contentType: 'multipart/form-data',

                contentType: false,

                processData: false,

                success: function (response) {

                    waitingDialog.hide();

                    $('#imageFileUpload').val('');

                    if (response.status == "Success") {

                        var $toast = toastr["success"]('File uploaded.', 'Notification');
                        // $("#viewer2").empty();
                        reportServiceFieldImages(MasterFieldId, ServiceId, ServiceName, ReportFieldId, IsAdditionalService);

                    }
                    else if (response.status == "Error") {

                        var $toast = toastr["error"](response.errorMessage, 'Notification');

                    }
                    else {

                        var $toast = toastr["error"]('Failed to upload image. Please try again.', 'Notification');

                    }
                },
                error: function (error) {

                    var $toast = toastr["error"]('Failed to upload image. Please try again.', 'Notification');

                    waitingDialog.hide();

                }
            });





        }


        //#endregion
    }

})


$(document).on('click', '#btnRemoveImage', function () {
    debugger;



    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Remove Image!',
        content: 'Do you really want to delete current Image?',
        buttons: {
            Yes:
            {
                text: 'Yes', // 
                btnClass: 'btn-danger',
                action: function () {

                    var uri = $('#urlRemoveImage').val();
                    var aImageId = $('#txtCurrentImageId').val();
                    waitingDialog.show('Loading Please Wait...');
                    $.ajax({
                        url: uri,
                        type: 'POST',
                        data: { imagesID: aImageId },
                        success: function (msg) {
                            var $toast = toastr["success"]("successfully Removed", "Success Notification");
                            waitingDialog.hide();
                            if (IsServiceField === false) {
                                //get image links for custom fields
                                //$("#viewer2").empty();
                                reportFieldImages($("#txtCurrentImageFieldId").val(), $("#txtCurrentFieldName").val(), $('#txtCurrentReportFieldId').val());
                            }
                            else {

                                // get image links for service.

                                var serviceName = $('#txtCurrentFieldName').val();

                                var initialId = $('#txtCurrentImageFieldId').val();

                                var reportFieldId = $('#txtCurrentReportFieldId').val();

                                var serviceId = $('#txtCurrentServiceId').val();

                                var IsAdditionalService = $('#txtIsAdditionalService').val();

                                reportServiceFieldImages(initialId, serviceId, serviceName, reportFieldId, IsAdditionalService);
                            }

                        },
                        error: function (error) {
                            var $toast = toastr["error"]('Failed to remove image. Please try again.', 'Notification');
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


//btnPreviousImage

//$('#btnPreviousImage').click(function () {
$(document).on('click', '#btnPreviousImage', function () {
    debugger;



    $.each(CurrentImageItems, function (index, item) {

        if (item.ImageId.toString() === $('#txtCurrentImageId').val()) {

            $('#txtCurrentImageId').val(CurrentImageItems[index - 1].ImageId);

            iv2.iviewer('loadNextImage', CurrentImageItems[index - 1].ImagePath);

            if (IsServiceField === true) {
                $("h4.modal-title").text($('#txtCurrentFieldName').val() + " - " + CurrentImageItems[index - 1].ImageName);
            }

            if (index - 1 === 0) {
                // hide previous button;

                // $("#btnPreviousImage").css("display", "none");

                $(".iviewer_previous_image").css("display", "none");


            }
            else {
                // show previous button;
                $(".iviewer_previous_image").css("display", "block");
            }

            $(".iviewer_next_image").css("display", "block");

            return false;
        }
        // var defaults = { ImagePath: item.ImagePath, ImageId: item.ImageId, ImageName: "foo", SerialNo: i };


    });



});


//btnNextImage
//$('#btnNextImage').click(function () {
$(document).on('click', '#btnNextImage', function () {
    debugger;


    $.each(CurrentImageItems, function (index, item) {

        if (item.ImageId.toString() === $('#txtCurrentImageId').val()) {


            $('#txtCurrentImageId').val(CurrentImageItems[index + 1].ImageId);

            iv2.iviewer('loadNextImage', CurrentImageItems[index + 1].ImagePath);

            if (IsServiceField === true) {
                $("h4.modal-title").text($('#txtCurrentFieldName').val() + " - " + CurrentImageItems[index + 1].ImageName);
            }


            if (index + 2 === CurrentImageItems.length) {
                // hide previous button;

                $(".iviewer_next_image").css("display", "none");

            }
            else {
                // show previous button;
                $(".iviewer_next_image").css("display", "block");
            }

            // $("#btnPreviousImage").css("display", "block");
            $(".iviewer_previous_image").css("display", "block");
            return false;
        }
        // var defaults = { ImagePath: item.ImagePath, ImageId: item.ImageId, ImageName: "foo", SerialNo: i };


    });

});




function showAudio(strAudioPath, strServiceName, AttachmentID) {
    debugger;
    if (AttachmentID != null) {
        $("#myModalAudio").modal({ show: true, backdrop: 'static', keyboard: false });
        $("h4.modal-title").text("Audio - " + strServiceName);
        $("#audioPlayer1").removeAttr("src");
        $("#audioPlayer1").attr('src', strAudioPath);
        $('#AudioID').val(AttachmentID);
        $("#vA").load();
    }
}


// Add Code by vishal for upload audio and Video
$("#btnUploadAudio").click(function () {
    debugger;
    if ($("#AudioFileUpload").val() != "") {
        var pathArray = $("#AudioFileUpload").val();
        pathArray = pathArray.replace(/(c:\\)*fakepath\\/i, "");
        if (pathArray != '') {
            var a1 = pathArray.split('.');
            var length = a1.length;
            if (a1[length - 1].toLowerCase() != 'm4a') {
                var $toast = toastr["error"]("Please Select M4A File format.", "Error Notification");
                $('#AudioFileUpload').val('');
                return;

            }
        }
        var uri = $('#urlUploadAudio').val();
        var formData = new FormData();
        var aAudioID = $('#AudioID').val();
        var totalFiles = document.getElementById("AudioFileUpload").files.length;
        var file = "";
        for (var i = 0; i < totalFiles; i++) {
            file = document.getElementById("AudioFileUpload").files[i];

            formData.append("FileUpload", file);
            formData.append("AudioID", aAudioID);
        }

        if (file.size > 5000000) {
            var $toast = toastr["error"]("Upload upto 5 MB audio file", "Error Notification");
            $('#AudioFileUpload').val('');
            return;
        }

        waitingDialog.show('Loading Please Wait...');

        $.ajax({
            type: "POST",
            url: uri,
            data: formData,

            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                debugger;
                if (response.success == true) {
                    $('#AudioFileUpload').val('');
                    waitingDialog.hide();
                    var $toast = toastr["success"]('File uploaded.', 'Notification');
                    $('#AudioID').val(response.ObjList.AttachmentID);
                    $("#" + aAudioID).attr('onclick', 'showAudio(' + '"' + response.ObjList.strPath + '","' + response.ObjList.strServiceName + '","' + response.ObjList.AttachmentID + '"' + ');');
                    $("#" + aAudioID).attr('id', response.ObjList.AttachmentID);

                    $("#audioPlayer1").removeAttr("src");
                    $("#audioPlayer1").attr('src', response.ObjList.strPath);

                    $("#vA").load();


                }
                else {
                    var $toast = toastr["error"](response.ObjList.returnMessage, 'Notification');
                }


            },
            error: function (error) {
                var $toast = toastr["error"]('Failed to upload Audio. Please try again.', 'Notification');
                waitingDialog.hide();
            }
        });

    }
    else {
        var $toast = toastr["error"]("Please Select M4A File format, to upload audio file.", "Error Notification");
    }
});


//shows video file
function showVideo(strVideoPath, strServiceName, AttachmentID) {
    debugger;
    if (strVideoPath != null && strVideoPath.length > 0) {
        $("#myModalVideo").modal({ show: true, backdrop: 'static', keyboard: false });
        $("h4.modal-title").text("Video - " + strServiceName);
        $("#videoPlayer1").removeAttr("src");
        $("#videoPlayer1").attr('src', strVideoPath);
        $('#VideoID').val(AttachmentID);
        $("#vT").load();
    }
}

//uploads new video file to server.
$("#btnUploadVideo").click(function () {
    debugger;


    if ($("#VideoFileUpload").val() != "") {
        var pathArray = $("#VideoFileUpload").val();
        pathArray = pathArray.replace(/(c:\\)*fakepath\\/i, "");
        if (pathArray != '') {
            var a1 = pathArray.split('.');
            var length = a1.length;
            if (a1[length - 1].toLowerCase() != 'mp4') {
                var $toast = toastr["error"]("Please Select MP4 File format.", "Error Notification");
                $('#VideoFileUpload').val('');
                return;
            }
        }

        var uri = $('#urlUploadVideo').val();
        var formData1 = new FormData();
        var aVideoID = $('#VideoID').val();
        var totalFiles = document.getElementById("VideoFileUpload").files.length;
        var file = "";
        for (var i = 0; i < totalFiles; i++) {
            file = document.getElementById("VideoFileUpload").files[i];

            formData1.append("FileUpload", file);
            formData1.append("VideoId", aVideoID);
        }

        if (file.size > 10000000) {
            var $toast = toastr["error"]("Upload upto 10 MB Video file", "Error Notification");
            $('#VideoFileUpload').val('');
            return;
        }
        waitingDialog.show('Loading Please Wait...');

        debugger;
        $.ajax({
            type: "POST",
            url: uri,
            data: formData1,

            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                debugger;
                $('#VideoFileUpload').val('');
                waitingDialog.hide();
                if (response.success == true) {
                    $('#VideoFileUpload').val('');
                    waitingDialog.hide();
                    var $toast = toastr["success"]('File uploaded.', 'Notification');
                    $('#VideoID').val(response.ObjList.AttachmentID);
                    $("#" + aVideoID).attr('onclick', 'showVideo(' + '"' + response.ObjList.strPath + '","'
                        + response.ObjList.strServiceName + '","' + response.ObjList.AttachmentID + '"' + ');');

                    $("#" + aVideoID).attr('id', response.ObjList.AttachmentID);

                    $("#videoPlayer1").removeAttr("src");
                    $("#videoPlayer1").attr('src', response.ObjList.strPath);

                    $("#vT").load();
                }
                else {
                    var $toast = toastr["error"](response.ObjList.returnMessage, 'Notification');
                }

            },
            error: function (error) {
                var $toast = toastr["error"]('Failed to upload Audio. Please try again.', 'Notification');
                waitingDialog.hide();
            }
        });

    }
    else {
        var $toast = toastr["error"]("Please Select MP4 File format, to upload video file.", "Error Notification");
    }
});





$(document).on('keypress', '.Numeric-Only', function (e) {
    debugger;

    var regex = new RegExp(/^\d*\.?\d*$/);//new RegExp("^[0-9\.b]+$");
    var fieldVal = $(this).val();
    if (event.charCode == 0) { return true; }
    else {
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        console.log('key = ' + key);
        if (key == '.') {
            if (fieldVal.indexOf('.') !== -1) {
                event.preventDefault();
                return false;
            }
        }
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
});




$(document).on('click', '.MapUpdateLocation', function () {
    debugger;

    var elem = $(this).closest('div').find('span.dynamiclatLong');

    var elem2 = $(this).attr('id');

    var urlSetGLocation = $('#urlSetGLocation').val();
    //$('#ShowDynamicMap').html(data);
    fnValidateDynamicContent("#ShowDynamicMap")

    $("#CLatLongId").text(elem2);
    var array = elem.text().split(',');
    $('#NewLatitude').text(array[0]);
    $('#NewLongitude').text(array[1]);
    initMap(array[0], array[1]);

    $("#myModalChangeLatLong").modal({ show: true, backdrop: 'static', keyboard: false });
    /*waitingDialog.show('Please Wait...');
    $.ajax({
        url: urlSetGLocation,
        datatype: "text",
        type: "GET",
        success: function (data) {

            waitingDialog.hide();
            $('#ShowDynamicMap').html(data);
            fnValidateDynamicContent("#ShowDynamicMap")

            $("#CLatLongId").text(elem2);
            var array = elem.text().split(',');
            $('#NewLatitude').text(array[0]);
            $('#NewLongitude').text(array[1]);
            initMap(array[0], array[1]);

            $("#myModalChangeLatLong").modal({ show: true, backdrop: 'static', keyboard: false });
        },
        error: function (request, status, error) {
            waitingDialog.hide();
            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });*/
})

function fnValidateDynamicContent(element) {
    debugger;

    var currForm = $(element).find("form");
    currForm.removeData("validator");
    currForm.removeData("unobtrusiveValidation");
    var $this = $(element);

    if ($.validator.unobtrusive !== undefined) {
        $.validator.unobtrusive.parse(element);
    }

    currForm.validate(); // This line is important and added for client side validation to trigger, without this it didn't fire client side errors.
}




var marker;
function initMap(strlat, strLong) {
    //initialize(strlat, strLong);

    var map = new google.maps.Map.WebGLOverlayView(document.getElementById('map'), {
        zoom: 18,
        center: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
        mapTypeId: google.maps.MapTypeId.ROADMAP.WebGLOverlayView,
        zoomControl: true,
        zoomControlOptions: { style: google.maps.ZoomControlStyle.DEFAULT },
        scaleControl: true,
        scrollwheel: true,
        panControl: true,
        streetViewControl: true,
    });

    marker = new google.maps.Marker({
        map: map,
        draggable: true,
        animation: google.maps.Animation.DROP,
        position: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
    });

    var infoWindow = new google.maps.InfoWindow({ maxWidth: 400 });

    var geocoder = new google.maps.Geocoder();

    //google.maps.event.addListener(marker, 'dragend', function (event) {
    /*var marker2 = new google.maps.StreetViewPanorama(
        document.getElementById("pano"),
        {
            map: map,
            position: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
            pov: {
                heading: 34,
                pitch: 10,
            },
            draggable: true,
            animation: google.maps.Animation.DROP,
        }
    );
    map.setStreetView(marker2);
    google.maps.event.addListener(marker2, 'dragend', function (event) {

        var a = marker.getPosition().lat().toFixed(6).toString();
        var b = marker.getPosition().lng().toFixed(6).toString();
        infoWindow.setContent(a, b);
        infoWindow.open(map, marker2);
        $('#NewLatitude').text(a);
        $('#NewLongitude').text(b);




    })*/
    google.maps.event.addListener(marker, 'dragend', function (event) {





        var a = marker.getPosition().lat().toFixed(6).toString();
        var b = marker.getPosition().lng().toFixed(6).toString();
        infoWindow.setContent(a, b);
        infoWindow.open(map, marker);
        $('#NewLatitude').text(a);
        $('#NewLongitude').text(b);
    })


    function toggleBounce() {
        if (marker.getAnimation() !== null) {
            marker.setAnimation(null);
        } else {
            marker.setAnimation(google.maps.Animation.BOUNCE);
        }
    }

}

function initialize(strlat, strLong) {
    //initMaps(strlat, strLong);
    /*const map = new google.maps.Map(
        document.getElementById("map"),
        {
            center: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
            zoom: 14,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
    );
    marker = new google.maps.StreetViewPanorama(
        document.getElementById("pano"),
        {
            map: map,
            position: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
            pov: {
                heading: 34,
                pitch: 10,
            },
            draggable: true,
            animation: google.maps.Animation.DROP,
        }
    );
    map.setStreetView(marker);
    var infoWindow = new google.maps.InfoWindow({ maxWidth: 400 });

    var geocoder = new google.maps.Geocoder();
    google.maps.event.addListener(marker, 'dragend', function (event) {



        var a = marker.getPosition().lat().toFixed(6).toString();
        var b = marker.getPosition().lng().toFixed(6).toString();
        infoWindow.setContent(a, b);
        infoWindow.open(map, marker);
        $('#NewLatitude').text(a);
        $('#NewLongitude').text(b)
    })*/

};

// Initialize and add the map
function initMaps(strlat, strLong) {
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 17,
        center: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        zoomControl: true,
        zoomControlOptions: { style: google.maps.ZoomControlStyle.DEFAULT },
        scaleControl: true,
        scrollwheel: true,
        panControl: true,
        streetViewControl: true,
    });

    marker = new google.maps.Marker({
        map: map,
        draggable: true,
        animation: google.maps.Animation.DROP,
        position: { lat: parseFloat(strlat), lng: parseFloat(strLong) }
    });
    //marker.addListener('click', toggleBounce);


    var infoWindow = new google.maps.InfoWindow({ maxWidth: 400 });

    var geocoder = new google.maps.Geocoder();

    //google.maps.event.addListener(marker, 'dragend', function (event) {
    var marker2 = new google.maps.StreetViewPanorama(
        document.getElementById("pano"),
        {
            map: map,
            position: { lat: parseFloat(strlat), lng: parseFloat(strLong) },
            pov: {
                heading: 34,
                pitch: 10,
            },
            draggable: true,
            animation: google.maps.Animation.DROP,
        }
    );
    map.setStreetView(marker2);
    google.maps.event.addListener(marker2, 'dragend', function (event) {



        var a = marker.getPosition().lat().toFixed(6).toString();
        var b = marker.getPosition().lng().toFixed(6).toString();
        infoWindow.setContent(a, b);
        infoWindow.open(map, marker2);
        $('#NewLatitude').text(a);
        $('#NewLongitude').text(b);
    })
    google.maps.event.addListener(marker, 'dragend', function (event) {



        var a = marker.getPosition().lat().toFixed(6).toString();
        var b = marker.getPosition().lng().toFixed(6).toString();
        infoWindow.setContent(a, b);
        infoWindow.open(map, marker);
        $('#NewLatitude').text(a);
        $('#NewLongitude').text(b);
    })


    function toggleBounce() {
        if (marker.getAnimation() !== null) {
            marker.setAnimation(null);
        } else {
            marker.setAnimation(google.maps.Animation.BOUNCE);
        }
    }
};
//window.initMaps = initMaps;



$(document).on('click', '#btnUpdateLatLong', function () {
    debugger;
    var elem = $("#NewLatitude").text();

    var elem1 = $("#NewLongitude").text();

    var elem2 = $("#CLatLongId").text();



    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Update Latitude and Longitude!',
        content: 'The new latitude & longitude will be directly updated in system. Do you want continue?',
        buttons: {
            Yes:
            {
                text: 'Yes',
                btnClass: 'btn-danger',
                action: function () {

                    var urlUpdateLatLong = $('#urlUpdateLatLong').val();
                    waitingDialog.show('Please Wait...');
                    $.ajax({
                        url: urlUpdateLatLong,
                        datatype: "text",
                        type: "POST",
                        data: { 'ProjectFieldDataMasterId': elem2, 'strLat': elem, 'strLong': elem1 },
                        success: function (data) {

                            waitingDialog.hide();
                            if (data.success) {
                                var $toast = toastr["success"]("Updated the lat long for the record successfully!", "Notification !");


                                $("#myModalChangeLatLong").modal('hide');
                                $('#ShowDynamicMap').html('');




                                $('.MapUpdateLocation').each(function () {

                                    if ($(this).attr('id') === elem2) {
                                        $(this).closest('div').find('span.dynamiclatLong').text(elem + "," + elem1);
                                    }

                                });
                            }
                            else {
                                var $toast = toastr["error"]("Update failed!", "Notification !");
                            }


                        },
                        error: function (request, status, error) {
                            waitingDialog.hide();
                            var $toast = toastr["error"](request.responseText, "Notification !");

                            console.log(request.responseText)

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






})


$(document).on('click', '#btnLatLongCancel', function () {

    $("#myModalChangeLatLong").modal('hide');
    $('#ShowDynamicMap').html('');
})