var spinner, opts, target;
var curStep, curStepBtn, nextStepWizard, curInputs, isValid;
var $sourceFields, $destinationFields, $chooser, navListItems, NewData;
var editMode = false;

var selectDropDown;
var manageEventvalue = "";
$(document).ready(function () {

    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');

    waitingDialog.show('Loading Please Wait...');

    navListItems = $('div.setup-panel div a'),
        allWells = $('.setup-content'),
        allNextBtn = $('.nextBtn'),
        allPreviousBtn = $('.previous'),
        selectDropDown = $('.dropDownFoo');

    allWells.hide();

    navListItems.click(function (e) {
        e.preventDefault();
        var $target = $($(this).attr('href')),
            $item = $(this);

        if (!$item.hasClass('disabled')) {
            navListItems.removeClass('btn-primary').addClass('btn-default');
            $item.addClass('btn-primary');
            allWells.hide();
            $target.show();
            $target.find('input:eq(0)').focus();
        }
    });

    allNextBtn.click(function () {
        //debugger;;
        curStep = $(this).closest(".setup-content"),
            curStepBtn = curStep.attr("id"),
            nextStepWizard = $('div.setup-panel div a[href="#' + curStepBtn + '"]').parent().next().children("a"),
            curInputs = curStep.find("input[type='text'],input[type='url'],textarea[textarea]"),
            isValid = true;




        if (curStepBtn == 'step-1') {
            var returnValue = checkTab1Status();
        }
        else if (curStepBtn == 'step-2') {
            var returnValue1 = checkTab2Status();
        }
        else if (curStepBtn == 'step-3') {
            checkTab3Status();
        }
        else if (curStepBtn == 'step-4') {
            checkTab4Status();
        }
            //else if (curStepBtn == 'step-5') {
            //    GetTab_Step6();
            //}
        else if (curStepBtn == 'step-5') {
            nextStepWizard.removeAttr('disabled').trigger('click');
            GetTab_Step6();
        }


    });




    allPreviousBtn.click(function () {
        //debugger;;
        var curStep = $(this).closest(".setup-content"),
            curStepBtn = curStep.attr("id"),
            previousStepWizard = $('div.setup-panel div a[href="#' + curStepBtn + '"]').parent().prev().children("a"),
            curInputs = curStep.find("input[type='text'],input[type='url'],textarea[textarea]"),
            isValid = true;

        if (isValid)
            previousStepWizard.removeAttr('disabled').trigger('click');
    });

    $('div.setup-panel div a.btn-primary').trigger('click');







    var actualDate = new Date(); // convert to actual date
    var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());
    NewData = "New";
    //debugger;
    $('#txtFromDate').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,
        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onClose: function (selectedDate) {
            $("#txtToDate").datepicker("option", "minDate", selectedDate);
        }
    }).datepicker("setDate", actualDate);


    $('#txtToDate').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,
        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onClose: function (selectedDate) {
            $("#txtFromDate").datepicker("option", "maxDate", selectedDate);
        }
    }).datepicker("setDate", actualDate);





    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liProject');
    listate.addClass("active");

    //#endregion




    $("#btnAddCustomized").click(function () {

        $("#btnReset").show();
        $('#myModal').on('shown.bs.modal', function () {
        })
    })




    $("#btnAddPopup1").click(function () {
        //debugger;;
        var $container = $("#divCustmizedContainer");
        $('#dynamicdiv').empty();

        $container.append('<div id="answerdiv' + $container.children().length + 1 + '" class="alert-message"> <h4 class="modal-title">New Field '
            + $container.children().length + '</h4></div>');
        var urlgetRowPartial = $('#getRowPartial').val();
        $.ajax({
            url: urlgetRowPartial,
            type: "GET",
            // async: false,
            success: function (data) {
                $("#dynamicdiv").append(data);
            }
        });

    })



    $("#btn_Upload").click(function () {
        debugger;;

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

        var projId = $("#txtProjectId").val();
        var fileUploadAction = $("#getFileUploadPartialURL").val();
        var fileInput = document.getElementById('fileInput');
        var xhr = new XMLHttpRequest();
        xhr.open('POST', fileUploadAction);
        xhr.setRequestHeader('Content-type', 'multipart/form-data');
        xhr.setRequestHeader('X-File-Name', fileInput.files[0].name);
        xhr.setRequestHeader('X-File-Type', fileInput.files[0].type);
        xhr.setRequestHeader('X-File-Size', fileInput.files[0].size);
        //Sending file in XMLHttpRequest
        xhr.send(fileInput.files[0]);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //debugger;;
                var resp = JSON.parse(xhr.responseText);
                if (resp.success == true) {
                    var urlgetRowPartial = $('#GetPatialViewDataUrl').val();
                    $.ajax({
                        url: urlgetRowPartial,
                        data: { ProjectID: projId },
                        type: "POST",
                        // async: false,
                        success: function (data) {
                            if (data.success == true) {
                                $("#divPartialFileUpload").empty();
                                $("#divPartialFileUpload").append(data.excelData);
                                //spinner.stop(target);
                                waitingDialog.hide();
                                var $toast = toastr["success"]("Excel Uploaded Successfully.", "Success Notification !");
                            }
                            else {
                                waitingDialog.hide();
                                var $toast = toastr["error"](data.excelData, "Error Notification !");
                            }
                        }
                    });
                }
                else {
                    waitingDialog.hide();
                    var $toast = toastr["error"](resp.excelData1, "Error Notification !");
                }
            }
        }

    });


    GetExistingProjectDetails();


    $("#ddlClient").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlCPerson").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlState").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlCity").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlUtilitytype").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlConnectionType").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlSyncType").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ManageInventory").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlWarehouse").chosen({ no_results_text: "Oops, nothing found!" });

    ShowHideAddProductButton()
});

$(".nextBtn")
$(document).on("click", ".nextBtn", function () {
    ShowHideAddProductButton()
})


$("#ManageInventory").change(function () {
    ShowHideAddProductButton()
})

function ShowHideAddProductButton() {
    $("#ManageInventory").val() === "Yes" ? $(".btnAddProduct").show() : $(".btnAddProduct").hide();
}

$(document).on('click', '.getPicture', function (e) {

    //debugger;;
    waitingDialog.show('Please Wait...');

    //spinner.spin(target);
    $("#divPictureList").empty();

    var baseId = $($(this)).attr("id");

    var serviceId = baseId.replace('btnservice', '');
    var urlClientData = $('#GetPictureListurl').val();
    $.ajax({
        type: 'GET',
        url: urlClientData,
        data: { "ServiceId": serviceId },
        success: function (data) {
            //debugger;;
            //spinner.stop(target);

            waitingDialog.hide();
            if (data.success == true) {
                if (data.pictureData != null && data.pictureData != '') {
                    $("#divPictureList").append(data.pictureData);
                    $("#myModal1").modal('show');
                }
                else {
                    var $toast = toastr["error"]('No picture set to the current service.');
                }
            }
            else {

                var $toast = toastr["Error"](data.pictureData);
            }

        },
        error: function () {
            //spinner.stop(target);
            waitingDialog.hide();
            var $toast = toastr["error"]("something seems wrong", "Notification");

        }

    });

})



$("#ancddlWarehouse_chosen").click(function () {

    $("#ddlWarehouse_chosen").toggleClass("chosen-with-drop chosen-container-active")
});


function filterddlWarehouse(element) {

    var value = $(element).val().toLowerCase();

    $("#ulddlWarehouse > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulddlWarehouse .liddlWarehouse:visible').length > 0 && $('#ulddlWarehouse .highlighted:visible').length == $('#ulddlWarehouse .liddlWarehouse:visible').length) {
        $('#ddlWarehouse_SelectAll').prop('checked', true);
    } else {
        $('#ddlWarehouse_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('body').on('click', function (e) {
    if (!$('#divddlWarehouse').is(e.target)
        && $('#divddlWarehouse').has(e.target).length === 0
    ) {
        $('#ddlWarehouse_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});
$('#ddlWarehouse_SelectAll').on('click', function () {

    if ($('#ulddlWarehouse .liddlWarehouse').length > 0) {
        if (this.checked) {
            $("li.liddlWarehouse:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulddlWarehouse .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse")
            } else {
                $("#ddlWarehouseHeader").addClass("ddlHeader").text(selectedlilength + " Warehouse selected")
            }

        }
        else {
            $("li.liddlWarehouse:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulddlWarehouse .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse")
            } else {
                $("#ddlWarehouseHeader").addClass("ddlHeader").text(selectedlilength + " Warehouse selected")
            }
        }


    }

});

function selectStatus() {

    var selectedlivalue = $('#ulddlWarehouse li.highlighted').map(function (i, el) {
        return $(el).val();
    });

    var selectedlivalue1 = $('#ulddlWarehouse li.highlighted').map(function (i, el) {
        return $(el);
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse")
    } else if (selectedlilength == 1) {
        $("#ddlWarehouseHeader").addClass("ddlHeader").text(selectedlivalue1[0].text())
    }
    else {
        $("#ddlWarehouseHeader").addClass("ddlHeader").text(selectedlilength + " Warehouse selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulddlWarehouse .liStatus:visible').length > 0 && $('#ulddlWarehouse .highlighted:visible').length == $('#ulddlWarehouse .liStatus:visible').length) {
        $('#ddlWarehouse_SelectAll').prop('checked', true);
    } else {
        $('#ddlWarehouse_SelectAll').prop('checked', false);
    }
    //#endregion


}

function GetExistingProjectDetails() {


    $('#txtProjectId').val('');
    $('#txtProjectName').val('');
    $('#txtProDes').val('');
    $('#txtFromDate').val('');
    $('#txtToDate').val('');
    $('#lblProjectName').text('');
    $('#txtdataSynch').val('');


    var urlClientData = $('#getTab1Data').val();
    $.ajax({
        type: 'GET',
        url: urlClientData,

        success: function (data) {

            if (data.ProjectId > 0) {

                $('#txtCloneId').val(data.CloneId);
                $('#txtProjectId').val(data.ProjectId);
                $('#txtProjectName').val(data.ProjectName);
                $('#txtProDes').val(data.ProjectDescription);
                $('#txtFromDate').val(data.stringFromDate);
                $('#txtToDate').val(data.stringToDate);
                $('#txtdataSynch').val(data.dataSynch);
                $('#txtProjUrl').val(data.ProjectAppointmentUrl);


                $('#ddlUtilitytype').val(
                    (data.Utilitytype == 'Electric') ? '1' : (data.Utilitytype == 'Gas' ? '2' :
                (data.Utilitytype == 'Water' ? '3' : '0'))).attr("selected", "selected").trigger('chosen:updated');

                $('#ddlConnectionType').val(data.ConnectionType).attr("selected", "selected").trigger('chosen:updated');
                $('#ddlSyncType').val(data.SyncType).attr("selected", "selected").trigger('chosen:updated');
                $('#ManageInventory').val(data.ManageInventory).attr("selected", "selected").trigger('chosen:updated');

            }
            else {

                $('#ddlUtilitytype').val('0').attr("selected", "selected");
                $('#ddlConnectionType').val('0').attr("selected", "selected");
            }

            /*State dropdown*/

            $('#ddlState').empty();

            $.each(data.objStateList, function (index, item) {
                if (index == 0) { $('#ddlState').append($('<option></option>').val('0').html("")); }
                $('#ddlState').append($('<option></option>').val(item.StateID).html(item.StateName));
            });

            if (data.StateId != '' && data.StateId != null) {
                $('#ddlState').val(data.StateId).attr("selected", "selected");
            }
            $('#ddlState').trigger('chosen:updated');


            /*End State dropdown*/

            /*City dropdown*/
            $('#ddlCity').empty();

            if (data.CityId == 0) {
                $('#ddlCity').append($('<option></option>').val('0').html("")).trigger('chosen:updated');
            }
            else {
                $.each(data.objCityList, function (index, item) {
                    if (index == 0) { $('#ddlCity').append($('<option></option>').val('0').html("")); }
                    $('#ddlCity').append($('<option></option>').val(item.CityID).html(item.CityName));
                });

                $('#ddlCity').val(data.CityId).attr("selected", "selected").trigger('chosen:updated');

                if ($('#txtProjectName').val().trim().length > 0) {
                    $('#lblProjectName').text(' - ' + $('#txtProjectName').val().trim() + ', ' + $('#ddlCity option:selected').text() +
                        ', ' + $('#ddlState option:selected').text());
                }
            }
            /*End City dropdown*/


            /*Client dropdown*/
            $('#ddlClient').empty();

            $.each(data.objClientModelList, function (index, item) {
                if (index == 0) { $('#ddlClient').append($('<option></option>').val('0').html("")); }
                $('#ddlClient').append($('<option></option>').val(item.ClientId).html(item.ClientName));
            });

            $('#ddlClient').val((data.ClientId != '' && data.ClientId != null) ? data.ClientId : 0).attr("selected", "selected");
            $('#ddlClient').trigger('chosen:updated');

            /*End Client dropdown*/

            /*Contact dropdown*/

            $('#ddlCPerson').empty();

            if (data.ContactId == 0) {
                $('#ddlCPerson').append($('<option></option>').val('0').html("")).trigger('chosen:updated');
            }
            else {

                $.each(data.objContactModelDDLList, function (index, item) {
                    if (index == 0) { $('#ddlCPerson').append($('<option></option>').val('0').html("")); }
                    $('#ddlCPerson').append($('<option></option>').val(item.ContactId).html(item.ContactName));
                });

                $('#ddlCPerson').val(data.ContactId).attr("selected", "selected").trigger('chosen:updated');

            }

            /*End Contact dropdown*/


            //#region warehouse dropdown

            $("#ulddlWarehouse").selectable({

                start: function (event, ui) {
                },
                selected: function (event, ui) {

                    if ($(ui.selected).hasClass('highlighted')) {
                        $(ui.selected).removeClass('ui-selected click-selected highlighted');

                    } else {
                        $(ui.selected).addClass('click-selected highlighted');

                    }
                },
                unselected: function (event, ui) {

                    $(ui.unselected).removeClass('click-selected');
                },
                stop: function (event, ui) {

                    selectStatus()

                    $('.txtfilterall').val("");
                    $('.chbSelectAll').prop('checked', false);
                }
            });

            $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse")

            $('#ulddlWarehouse').empty();
            var count = data.objMobileWarehouseBOList.length;
            for (i = 0; i < count; i++) {

                $('#ulddlWarehouse').append($('<li></li>').val(data.objMobileWarehouseBOList[i].Id).addClass("liddlWarehouse active-result").html(data.objMobileWarehouseBOList[i].WarehouseName));
            }


            if (data.objWarehouseArray != null && data.objWarehouseArray.length > 0) {
                $.each(data.objWarehouseArray, function (index, item) {
                    $("#ulddlWarehouse li").filter(function () { return $(this).val() === item; }).addClass("ui-selected click-selected highlighted");
                });
            }

            var selectedlivalue = $('#ulddlWarehouse li.highlighted').map(function (i, el) { return $(el).val(); });
            var selectedlilength = selectedlivalue.length;
            if (selectedlilength == 0) {
                $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse")
            } else {
                $("#ddlWarehouseHeader").addClass("ddlHeader").text(selectedlilength + " Warehouse selected")
            }


            //#endregion






            /*Tab 2 dropdown*/

            $('#ddlMetersize').empty();
            $.each(data.objMeterSizeList, function (index, item) { $('#ddlMetersize').append($('<option></option>').val(item.ID).html(item.MeterSize)); });
            $('#ddlMetersize').multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true })
            $("#ddlMetersize").multiselect("refresh");


            $('#ddlMeterMake').empty();
            $.each(data.objMeterMakeModelList, function (index, item) { $('#ddlMeterMake').append($('<option></option>').val(item.ID).html(item.Make)); });
            $('#ddlMeterMake').multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true });
            $("#ddlMeterMake").multiselect("refresh");

            $('#ddlMetertype').empty();
            $.each(data.objMeterTypeList, function (index, item) { $('#ddlMetertype').append($('<option></option>').val(item.ID).html(item.MeterType)); });
            $('#ddlMetertype').multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true });
            $("#ddlMetertype").multiselect("refresh");


            $('#ddlService').empty();
            $.each(data.objServiceList, function (index, item) { $('#ddlService').append($('<option></option>').val(item.ID).html(item.Service)); });
            $('#ddlService').multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true });
            $("#ddlService").multiselect("refresh");


            $('#ddlInstaller').empty();
            $.each(data.objUsersSmallModelList, function (index, item) { $('#ddlInstaller').append($('<option></option>').val(item.UserId).html(item.UserName)); });
            $('#ddlInstaller').multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true })
            $("#ddlInstaller").multiselect("rebuild");





            if (data.objTypeArray != null && data.objTypeArray.length > 0) {
                $("#ddlMetertype").val(data.objTypeArray);
                $("#ddlMetertype").multiselect("refresh");
            }

            if (data.objSizeArray != null && data.objSizeArray.length > 0) {
                $("#ddlMetersize").val(data.objSizeArray);
                $("#ddlMetersize").multiselect("refresh");
            }


            if (data.objMakeArray != null && data.objMakeArray.length > 0) {
                $("#ddlMeterMake").val(data.objMakeArray);
                $("#ddlMeterMake").multiselect("refresh");
            }


            if (data.objServiceArray != null && data.objServiceArray.length > 0) {
                $("#ddlService").val(data.objServiceArray);
                $("#ddlService").multiselect("refresh");
            }


            if (data.objUsersArray != null && data.objUsersArray.length > 0) {
                $("#ddlInstaller").val(data.objUsersArray);
                $("#ddlInstaller").multiselect("refresh");
            }



            waitingDialog.hide();

            /*End Tab 2 dropdown*/
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }

    });


}



$('#ddlClient').change(function () {


    $('#ddlCPerson').prop('disabled', false);
    $('#ddlCPerson').empty();
    var value = $("#ddlClient option:selected")

    waitingDialog.show('Please Wait Loading Contacts...');
    var urlContactData = $('#ContactByClientListUrl').val();
    $.ajax({
        type: 'POST',
        data: {
            "ClientId": value.val()
        },
        url: urlContactData,
        success: function (data) {

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCPerson').append($('<option></option>').val('0').html(""));
                }

                $('#ddlCPerson').append($('<option></option>').val(item.ContactId).html(item.ContactName));
            });

            $('#ddlCPerson').trigger('chosen:updated');


            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }
    });

});




$('#ManageInventory').change(function () {



    var value = $("#ManageInventory option:selected")

    if (value.val() == "Yes") {
        waitingDialog.show('Please Wait Loading...');
        var UrlGetWarehouseList = $('#UrlGetWarehouseList').val();
        $.ajax({
            type: 'GET',
            url: UrlGetWarehouseList,
            success: function (data) {

                var count = data.length;
                for (i = 0; i < count; i++) {

                    $('#ulddlWarehouse').append($('<li></li>').val(data[i].Id).addClass("liddlWarehouse active-result").html(data[i].WarehouseName));
                }

                $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse");
                waitingDialog.hide();
            },

        });
    }
    else {
        $('#ulddlWarehouse').empty();
        $("#ddlWarehouseHeader").removeClass("ddlHeader").text("Select Warehouse");
    }


});


var BindMetertype, BindMetersize, BindServices;





$('#ddlState').change(function () {

    $('#ddlCity').prop('disabled', false);
    $('#ddlCity').empty();
    var value = $("#ddlState option:selected")
    $('#txtStateID').val(value.val());

    waitingDialog.show('Please Wait Loading Cities...');
    var urlCityData = $('#CityByStateIDListUrl').val();
    $.ajax({
        type: 'GET',
        data: {
            "stateId": value.val(),
            "cityName": ''
        },

        url: urlCityData,
        success: function (data) {

            $.each(data, function (index, item) {
                if (index == 0) {
                    $('#ddlCity').append($('<option></option>').val('0').html(""));
                }

                $('#ddlCity').append($('<option></option>').val(item.CityID).html(item.CityName));
            });

            $('#ddlCity').trigger('chosen:updated');
            waitingDialog.hide();
        },

    });

});



function checkTab1Status() {

    var objWarehouseList = [];
    $('#ulddlWarehouse li.highlighted').each(function (index, item) { objWarehouseList.push($(this)[0].id) });


    if ($('#txtProjectName').val() == '') {
        var $toast = toastr["error"]('Please enter project name', 'Notification');
        return false;
    }
    else if ($("#ddlClient").val() == 0) {
        var $toast = toastr["error"]('Please select client name', 'Notification');
        return false;
    }
    else if ($('#txtFromDate').val() == '') {
        var $toast = toastr["error"]('Please select from date', 'Notification');
        return false;
    }
    else if ($('#txtToDate').val() == '') {
        var $toast = toastr["error"]('Please select to date', 'Notification');
        return false;
    }
    else if ($('#ddlState').val() < 1) {
        var $toast = toastr["error"]('Please select state name', 'Notification');
        return false;
    }
    else if ($('#ddlCity').val() < 1) {
        var $toast = toastr["error"]('Please select city name', 'Notification');
        return false;
    }
    else if ($('#ddlUtilitytype').val() == 0) {
        var $toast = toastr["error"]('Please select Utility type', 'Notification');
        return false;
    }
    else if ($('#ddlConnectionType').val() == 0) {
        var $toast = toastr["error"]('Please select connection type', 'Notification');
        return false;
    }
    else if ($('#ddlSyncType').val() == 0) {
        var $toast = toastr["error"]('Please select sync type', 'Notification');
        return false;
    }
    else if ($('#txtdataSynch').val().length == 0) {
        var $toast = toastr["error"]('Please enter data synch interval in miniutes', 'Notification');
        return false;
    }
    else if ($('#ManageInventory').val() === undefined || $('#ManageInventory').val() === null || $('#ManageInventory').val().length === 0) {
        var $toast = toastr["error"]('Please select manage inventory option', 'Notification');
        return false;
    }
    else if ($('#ManageInventory').val() === "Yes" && (objWarehouseList == null || objWarehouseList.length == 0)) {
        var $toast = toastr["error"]('Please select warehouse for  project', 'Notification');
        return false;
    }

    else {
        var query = $('#txtdataSynch').val();
        if (isNaN(parseInt(query))) {
            toastr["error"]('Please enter data synch interval in miniutes', 'Notification');
            return false;
        }
        else if (parseInt(query) <= 0) {
            toastr["error"]('Data synch interval can not have minus value', 'Notification');
            return false;
        }
        else {
            waitingDialog.show('Please Wait...');

            saveData();

            nextStepWizard.removeAttr('disabled').trigger('click');
        }


    }


}


//Save Project Details
function saveData() {


    var urlSaveData = $('#SaveTab1Url').val();


    // ProjectId     CloneId       ProjectName   ProjectDescription ClientId StateId  CityId ContactId  FromDate  ToDate  Utilitytype ConnectionType
    //SyncType    ProjectAppointmentUrl   ManageInventory  objWarehouseArray
    var FormData = {};

    FormData.ProjectId = $("#txtProjectId").val();
    FormData.CloneId = $("#txtCloneId").val();//1;
    FormData.ProjectName = $("#txtProjectName").val();//1;
    FormData.ProjectDescription = $("#txtProDes").val();//1;
    FormData.ClientId = $("#ddlClient").val();//1;
    FormData.ContactId = $("#ddlCPerson").val();//1; 
    FormData.StateId = $("#ddlState").val();
    FormData.CityId = $('#ddlCity').val();
    FormData.FromDate = $('#txtFromDate').val();
    FormData.ToDate = $('#txtToDate').val();
    FormData.Utilitytype = $('#ddlUtilitytype').val();
    FormData.ConnectionType = $('#ddlConnectionType').val();
    FormData.SyncType = $('#ddlSyncType').val();
    FormData.dataSynch = $('#txtdataSynch').val();
    FormData.ProjectAppointmentUrl = $('#ProjectAppointmentUrl').val();
    FormData.ManageInventory = $('#ManageInventory').val();



    var objWarehouseList = [];

    $('#ulddlWarehouse li.highlighted').each(function (index, item) {
        objWarehouseList.push($(this).val());
    });
    FormData.objWarehouseArray = objWarehouseList;


    $.ajax({
        url: urlSaveData,
        data: { "objProject": FormData },
        type: 'POST',

        success: function (data1) {
            if (data1.success == true) {

                $("#txtProjectId").val(data1.proId);

                $("#txtCloneId").val('0');

                var $toast = toastr["success"]("Project information saved successfully.");
                $('#lblProjectName').text(' - ' + $('#txtProjectName').val().trim() + ', '
                    + $('#ddlCity option:selected').text() + ', ' + $('#ddlState option:selected').text());

            } else {
                if (data1.returnMessage != "") {
                    var $toast = toastr["error"](data1.returnMessage, "Notification");
                }
                else {
                    var $toast = toastr["error"]("Save failed. Please try again !", " Error Notification");
                }
            }
            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Notification");
        }
    });
}



function checkTab2Status() {

    var meterMakeList = $('#ddlMeterMake').val();
    var meterTypeList = $('#ddlMetertype').val();
    var meterSizeList = $('#ddlMetersize').val();
    var serviceList = $('#ddlService').val();
    var installerList = $('#ddlInstaller').val();
    var projId = $("#txtProjectId").val();
    //debugger;;
    var urlSaveData = $('#SaveTab2Url').val();

    if (meterMakeList != null && meterTypeList != null && meterSizeList != null && serviceList != null && installerList != null
        && meterTypeList.length > 0 && meterSizeList.length > 0 && serviceList.length > 0 && installerList.length > 0) {
        waitingDialog.show('Please Wait...');
        // spinner.spin(target);
        $.ajax({
            url: urlSaveData,
            //async: false,
            data: {
                "projectId": projId,
                "objMeterSizeList": meterSizeList,
                "objMeterTypeList": meterTypeList,
                "objServiceList": serviceList,
                "objInstallerList": installerList,
                "objMakeList": meterMakeList
            },
            type: 'POST',
            async: true,
            success: function (data) {
                if (data.success == true) {
                    nextStepWizard.removeAttr('disabled').trigger('click');
                    getTab3Data();

                } else {
                    waitingDialog.hide();
                    return false;
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Notification");
                return false;
            }


        });

    }
    else if (meterSizeList == null || meterSizeList.length == 0) {
        var $toast = toastr["error"]('Please select meter size', 'Notification');
    }
    else if (meterMakeList == null || meterMakeList.length == 0) {
        var $toast = toastr["error"]('Please select meter make', 'Notification');
    }
    else if (meterTypeList == null || meterTypeList.length == 0) {
        var $toast = toastr["error"]('Please select meter type', 'Notification');
    }
    else if (serviceList == null || serviceList.length == 0) {
        var $toast = toastr["error"]('Please select services', 'Notification');
    }
    else if (installerList == null || installerList.length == 0) {
        var $toast = toastr["error"]('Please select installers', 'Notification');
    }
}

function getTab3Data() {
    //debugger;;
    var projId = $("#txtProjectId").val();
    var urlGetTab3Data = $('#GetTab3Url').val();
    var mastDiv = $('#ulServicePrice');
    mastDiv.empty();
    var mastAdditionalDiv = $('#ulAdditionalServicePrice');
    mastAdditionalDiv.empty();
    var mastMeterPayRateDiv = $('#ulMeterSizesPayRates');
    mastMeterPayRateDiv.empty();
    var result = '<li class="row" style="margin-top: -12px; margin-bottom: -10px;" > <div class="task-title"> <span class="task-title-sp col-lg-2 col-sm-3 col-xs-3 show_deactive" style="font-weight:700">Service Name</span>' +
        '<span class="task-title-sp col-lg-2 col-sm-3 col-xs-2" style="font-weight:700; ">Rate For Service ($)</span> ' +
        '<span class="task-title-sp1 col-lg-2 col-sm-3 col-xs-3" style="font-weight:700;padding-left: 34px;">Service Images</span>' +
        '<span class="task-title-sp2 col-lg-2 col-sm-3 col-xs-3" style="font-weight:700;padding-left:34px;">Images Required</span> ' +
        '<span class="task-title-sp2 col-lg-3 col-sm-3 col-xs-3 btnAddProduct" style="font-weight:700; ">Products Required</span></div></li><br>';

    var result1 = '<li class="row" style="margin-top: -12px; margin-bottom: -10px;" > <div class="task-title"> <span class="task-title-sp col-lg-2 col-sm-3 col-xs-3 show_deactive" style="font-weight:700">Service Name</span>' +
        '<span class="task-title-sp col-lg-2 col-sm-3 col-xs-2" style="font-weight:700">Rate For Service ($)</span> ' +
        '<span class="task-title-sp1 col-lg-2 col-sm-3 col-xs-3" style="font-weight:700;padding-left:34px;">Service Images</span>' +
        '<span class="task-title-sp2 col-lg-2 col-sm-3 col-xs-3" style="font-weight:700;padding-left:34px;">Images Required</span>' +
        '<span class="task-title-sp2 col-lg-3 col-sm-3 col-xs-3 btnAddProduct" style="font-weight:700;">Products Required</span></div></li><br>';

    var result2 = '<li class="row" style="margin-top: -12px; margin-bottom: -10px;" > <div class="task-title"> <span class="task-title-sp col-lg-2 col-sm-3 col-xs-3 show_deactive" style="font-weight:700">Meter Size</span>' +
        '<span class="task-title-sp col-lg-2 col-sm-3 col-xs-2" style="font-weight:700">Meter Pay Rate ($)</span></div></li><br>';
    $.ajax({
        cache: false,
        type: "GET",
        url: urlGetTab3Data,
        data: { "projectId": projId },
        //async: false,
        success: function (data) {


            var Servicelist = [];

            //style="width: 100%; margin-top: 0%; margin-bottom: 1%;"
            $.each(data.objList1, function (id, ServiceSmallModel) {
                result = result + ' <li class="row" style="margin-bottom: 5px;"> <div class="task-content">';

                result = result + ' <div class="col-lg-2 col-sm-3 col-xs-3 show_deactive"><span>'
                    + ServiceSmallModel.ServiceName + '</span></div>';

                result = result + '<div class="col-lg-2 col-sm-3 col-xs-3">' +
                    '<input maxlength="20" id="' + ServiceSmallModel.ServiceId + '" type="text" placeholder="Enter rate for service" value="' +
                    ServiceSmallModel.ServiceBasePrice + '"' +
                    'style="width:100%; text-align:right; padding-right:10px;" class="tab3txt allownumericwithdecimal"  />' +
                    '</div>';

                result = result + '<div class="col-lg-2 col-sm-3 col-xs-3" style="text-align:center">';
                if (ServiceSmallModel.isImageMapped == true) {
                    result = result + '<a id="btnservice' + ServiceSmallModel.ServiceId + '" class="btn btn-primary getPicture" data-backdrop="static" href="#"><i class="fa fa-eye"></i></a>';
                }
                else {
                    result = result + '<div class="row" style=""> <div class="col-sm-3 text-center"></div></div>'
                }

                result = result + '</div>';


                var _imageFlag = ServiceSmallModel.ImageFlag == false ? false : true;
                result = result + '<div class="col-lg-2 col-sm-3 col-xs-3 sel-serv" style="">';
                if (ServiceSmallModel.isImageMapped == true) {
                    result = result + '<div class="row m-bot15" style=""> <div class="col-sm-3 text-center">' +
                        '<input class="serviceImage" id="chk' + ServiceSmallModel.ServiceId + '" type="checkbox" ' +
                        (_imageFlag == true ? ' checked ' : '') + ' data-toggle="switch"></div></div>';
                }
                else {
                    result = result + '<div class="row " style="margin-bottom:29%;"> <div class="col-sm-3 text-center"></div></div>'
                }
                result = result + ' </div>';



                result = result + ' <div class="col-lg-3 col-sm-3 col-xs-3 regProduct" style="">';

                // if (data.objList3 != undefined && data.objList3 != null) {
                //modifiedby AniketJ on 05-Aug-2017
                //Added button to add product

                //result = result + '<select id="ddlService' + ServiceSmallModel.ServiceId + '" name="Service" style="display:none;" multiple="multiple">';
                //$.each(data.objList3, function (index, item) {
                //    result = result + '<option value=' + item.productId + '>' + item.productName + '</option>';

                //})

                //result = result + '</select>';
                //Servicelist.push('ddlService' + ServiceSmallModel.ServiceId);


                result = result + '<input type="button" class="btn btn-success btnAddProduct" name="' + ServiceSmallModel.ServiceId + '" id="btnAddServiceProduct" value="Add Product" >';

                // }

                result = result + '</div></div>  </li>';


            });

            //style="width: 100%; margin-top: 0%; margin-bottom: 1%;"
            $.each(data.objList2, function (id, ServiceNotSelectedModel) {
                //debugger;
                result1 = result1 + ' <li class="row"style="margin-bottom: 5px;" > <div class="task-content">';

                result1 = result1 + '<div class="col-lg-2 col-sm-3 col-xs-3 show_deactive"><span>'
                    + ServiceNotSelectedModel.ServiceName + '</span></div>';

                result1 = result1 + '<div class="col-lg-2 col-sm-3 col-xs-3">' +
                    '<input maxlength="20" id="' + ServiceNotSelectedModel.ServiceId + '" type="text" placeholder="Enter rate for service" value="' +
                    ServiceNotSelectedModel.ServiceBasePrice + '"' +
                    'style="width:100%; text-align:right; padding-right:10px;" class="tab3txt allownumericwithdecimal"  />' +
                    '</div>';

                result1 = result1 + '<div class="col-lg-2 col-sm-3 col-xs-3" style="text-align:center">';
                if (ServiceNotSelectedModel.isImageMapped == true) {
                    result1 = result1 + '<a id="btnservice' + ServiceNotSelectedModel.ServiceId
                        + '" class="btn btn-primary getPicture" data-backdrop="static" href="#"><i class="fa fa-eye"></i></a>';
                }
                else {
                    result1 = result1 + '<div class="row" style=""> <div class="col-sm-3 text-center"></div></div> '
                }
                result1 = result1 + '</div>';

                var _imageFlag = ServiceNotSelectedModel.ImageFlag == false ? false : true;
                result1 = result1 + '<div class="col-lg-2 col-sm-3 col-xs-3 sel-serv" style="text-align:center">';
                if (ServiceNotSelectedModel.isImageMapped == true) {
                    result1 = result1 + '<div class="row m-bot15" style=""> <div class="col-sm-3 text-center">' +
                        '<input class="serviceImage" id="chk' + ServiceNotSelectedModel.ServiceId + '" type="checkbox" ' +
                        (_imageFlag == true ? ' checked ' : '') + ' data-toggle="switch"></div></div>';
                }
                else {
                    result1 = result1 + '<div class="row " style="margin-bottom:29%;"> <div class="col-sm-3 text-center"></div></div>'
                }
                result1 = result1 + ' </div>';



                result1 = result1 + ' <div class="col-lg-3 col-sm-3 col-xs-3 regProduct" style="">';

                //if (data.objList3 != undefined && data.objList3 != null) {
                //result1 = result1 + '<select id="ddlService' + ServiceNotSelectedModel.ServiceId + '" name="Service" style="display:none;" multiple="multiple">';
                //$.each(data.objList3, function (index, item) {
                //    result1 = result1 + '<option value=' + item.productId + '>' + item.productName + '</option>';

                //})

                //result1 = result1 + '</select>';

                //Servicelist.push('ddlService' + ServiceNotSelectedModel.ServiceId);

                result1 = result1 + '<input type="button" class="btn btn-success btnAddProduct" name="' + ServiceNotSelectedModel.ServiceId + '" id="btnAddServiceProduct" value="Add Product" >';
                // }

                result1 = result1 + '</div> </div> </li>';

            });

            $.each(data.objList4, function (id, ServiceSmallModel) {
                result2 = result2 + ' <li class="row" style="margin-bottom: 5px;"> <div class="task-content">';
                result2 = result2 + ' <div class="col-lg-2 col-sm-3 col-xs-3 show_deactive"><span>' + ServiceSmallModel.MeterSize + '</span></div>';
                result2 = result2 + '<div class="col-lg-2 col-sm-3 col-xs-3">' +
                    '<input maxlength="20" id="' + ServiceSmallModel.Id + '" type="text" placeholder="Enter Meter Pay Rate" value="' +
                    ServiceSmallModel.Pay + '"' +
                    'style="width:100%; text-align:right; padding-right:10px;" class="tab4txt allownumericwithdecimal"  />' +
                    '</div>';
                
                result2 = result2 + ' </div>';
                result2 = result2 + ' <div class="col-lg-3 col-sm-3 col-xs-3 regProduct" style="">';
                //result2 = result2 + '<input type="button" class="btn btn-success btnAddMeterPayRate" name="' + ServiceSmallModel.Id + '" id="btnAddMeterPayRates" value="Save" >';
                result2 = result2 + '</div></div>  </li>';
            });

            mastDiv.append(result);
            mastAdditionalDiv.append(result1);
            mastMeterPayRateDiv.append(result2);
            ShowHideAddProductButton()
            //debugger;;
            $("[data-toggle='switch']").wrap('<div class="switch" />').parent().bootstrapSwitch()


            if (Servicelist != null && Servicelist.length > 0) {
                $.each(Servicelist, function (key, value) {

                    $('#' + value).multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true });
                    $('#' + value).multiselect("refresh");
                });

            }


            $.each(data.objList1, function (id, ServiceSmallModel) {
                if (ServiceSmallModel.productArray != undefined && ServiceSmallModel.productArray.length > 0) {
                    $("#ddlService" + ServiceSmallModel.ServiceId).val(ServiceSmallModel.productArray);
                    $("#ddlService" + ServiceSmallModel.ServiceId).multiselect("refresh");
                }
            });

            $.each(data.objList2, function (id, ServiceNotSelectedModel) {
                if (ServiceNotSelectedModel.productArray != undefined && ServiceNotSelectedModel.productArray.length > 0) {
                    $("#ddlService" + ServiceNotSelectedModel.ServiceId).val(ServiceNotSelectedModel.productArray);
                    $("#ddlService" + ServiceNotSelectedModel.ServiceId).multiselect("refresh");
                }
            });

            $.each(data.objList4, function (id, ServiceSmallModel) {
                if (ServiceSmallModel.MeterPayRateArray != undefined && ServiceSmallModel.MeterPayRateArray.length > 0) {
                    $("#ddlService" + ServiceSmallModel.Id).val(ServiceSmallModel.MeterPayRateArray);
                    $("#ddlService" + ServiceSmallModel.Id).multiselect("refresh");
                }
            });


            waitingDialog.hide();
        },

        error: function (xhr, ajaxOptions, thrownError) {
            //spinner.stop(target);
            waitingDialog.hide();
            var $toast = toastr["error"]("something seems wrong", "Error Notification");

        }

    });



}

$(document).on('keypress keyup blu', '.allownumericwithdecimal', function (e) {
    //debugger;;
    //$(this).val($(this).val().replace(/[^0-9\.]/g, ''));
    //if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
    //    event.preventDefault();
    //}
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) &&
        ((event.which < 48 || event.which > 57) &&
            (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(this).val();

    if ((text.indexOf('.') != -1) &&
        (text.substring(text.indexOf('.')).length > 2) &&
        (event.which != 0 && event.which != 8) &&
        ($(this)[0].selectionStart >= text.length - 2)) {
        event.preventDefault();
    }

});

$(document).on('keypress keyup blu', '.allowalphanumeric', function (e) {
    var regex = new RegExp("^[a-zA-Z0-9\\-\\s]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
        return true;
    }

    e.preventDefault();
    return false;
});

$(document).on('change', '.checkalphanumeric', function (e) {
    //debugger;;
    var otxt = document.getElementById('txt1');

    var val = $(this).val();
    val = $.trim(val);
    {
        if (val.length == 0) {
            $(this).val(""); return;
        }
    }
    for (i = 0; i < val.length; i++) {
        //debugger;;
        var code = val.charCodeAt(i);
        if (!(code >= 65 && code <= 91) && !(code >= 97 && code <= 122) && !(code >= 48 && code <= 57) && !(code == 32)) { $(this).val(""); return; }

    }
});



function checkTab3Status() {
    debugger;

    var data1 = [];

    var checkVal = false;
    var serviceName = ''

    $(".tab3txt").each(function () {

        if ($.isNumeric($(this).val())) {

            var imageRequired = 'false';

            var a = $('#chk' + $(this)[0].id);

            if (a != null) {
                var b = a.parent().attr('class');
                if (b != null && (b == 'switch-animate switch-on' || b == 'switch-on switch-animate')) {
                    imageRequired = 'true';
                }
            }

            //ddlService767
            var selectedProductList = $('#ddlService' + $(this)[0].id).val();

            debugger;
            var serviceObject = { serviceId: $(this)[0].id, servicePrice: $(this).val(), imageCaptureRequired: imageRequired, AssignedProductList: selectedProductList };
            data1.push(serviceObject);
            debugger;
        }
        else {
            checkVal = true;
            serviceName = $(this)[0].id.text;
        }

    });

    if (checkVal) {
        var $toast = toastr["error"]('Please enter valid rate for all services.', 'Notification !');
        $(this).focus();
        return;
    }



    waitingDialog.show('Please Wait...');

    debugger;

    var data0 = { projectId: $("#txtProjectId").val(), Servicedata: data1};
    debugger;
    var json = JSON.stringify(data0);
    console.log(json);

    debugger;
    jQuery.ajaxSettings.traditional = true;
    var urlSaveTab3Data = $('#SaveTab3Url').val();
    $.ajax({
        url: urlSaveTab3Data,
        type: "POST",
        // async: false,
        data: {
            "Objdata": json
        },
        success: function (data) {
            if (data.success == true) {
                //debugger;;
                nextStepWizard.removeAttr('disabled').trigger('click');
                saveMetePayRates(); // Add new line
                getTab4Data();

            } else {
                return false;
            }
        },
        error: function () {
            console.log("get error");
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            return false;
        }
    });

}

function saveMetePayRates() {
    debugger;

    var data2 = [];

    var checkVal = false;
    var serviceName = ''

    $(".tab4txt").each(function () {

        if ($.isNumeric($(this).val())) {
            var Object = { Id: $(this)[0].id, Pay: $(this).val() };
            data2.push(Object);
        }
        else {
            checkVal = true;
            serviceName = $(this)[0].id.text;
        }

    });

    if (checkVal) {
        var $toast = toastr["error"]('Please enter valid rate for all pay rates.', 'Notification !');
        $(this).focus();
        return;
    }



    waitingDialog.show('Please Wait...');

    debugger;;

    var data0 = { projectId: $("#txtProjectId").val(), newMeterPayRate: data2 };

    var json = JSON.stringify(data0);


    jQuery.ajaxSettings.traditional = true;
    var urlSaveTab3Data = $('#SaveTab3UrlMeterPayRate').val();
    $.ajax({
        url: urlSaveTab3Data,
        type: "GET",
        // async: false,
        data: {
            "Objdata": json
        },
        success: function (data) {
            if (data.success == true) {
                //debugger;;
                nextStepWizard.removeAttr('disabled').trigger('click');
                getTab4Data();

            } else {
                return false;
            }

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            return false;
        }
    });
}

function getTab4Data() {
    //debugger;;
    // waitingDialog.hide();
    $sourceFields = null;
    $destinationFields = null;
    if ($('#sourceFields1').children().size() == 0 &&
        $('#destinationFields1').children().size() == 0) {
        $('#sourceFields1').empty();
        $('#destinationFields1').empty();
        var projId = $("#txtProjectId").val();
        var urlGetTab4Data = $('#GetTab4Url').val();
        $.ajax({
            url: urlGetTab4Data,
            type: "GET",
            data: { "ProjectId": projId },
            //async: false,
            success: function (data) {
                $("#sourceFields1").append(data.SourcceDiv);
                $("#destinationFields1").append(data.DestinationDiv);
                $sourceFields = $("#sourceFields1");
                $destinationFields = $("#destinationFields1");
                //$chooser = $("#fieldChooser").fieldChooser(sourceFields, destinationFields);
                //  spinner.stop(target);
                waitingDialog.hide();
            },
            error: function (data) {
                waitingDialog.hide();
            }
        });
    }
    else {
        //spinner.stop(target);1195
        waitingDialog.hide();
    }

    $(".source, .target").sortable({
        connectWith: ".connected",
        containment: ".sideBySide"
    });
}

function checkTab4Status() {
    //debugger;;
    var myArray = [];
    $('#destinationFields1').find('li').each(function () {
        myArray.push($(this).text());
    });
    var projId = $("#txtProjectId").val();

    if (myArray.length == 0) {
        //nextStepWizard.removeAttr('disabled').trigger('click');
        //checkTab5Status();
        var $toast = toastr["error"]("Please select atleast one standard field.", "Error Notification");
    }
    else {
        waitingDialog.show('Please Wait...');

        //spinner.spin(target);
        //debugger;;
        var urlSaveData = $('#SaveTab4Url').val();

        $.ajax({
            url: urlSaveData,
            //async: false,
            data: {
                "projectId": projId,
                "objStandardFieldList": myArray
            },
            type: 'POST',

            success: function (data) {
                if (data.success == true) {
                    nextStepWizard.removeAttr('disabled').trigger('click');
                    checkTab5Status();
                } else {

                    var $toast = toastr["error"](data.returnMessage, "Notification");
                    waitingDialog.hide();
                    return false;
                }
            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Notification");
                return false;
            }

        });
    }



}

function checkTab5Status() {
    //custom fields
    bindActiveTable(true);

}


$('#chkShowDeactivate').change(function () {

    if ($(this).is(":checked")) {

        bindActiveTable(false);
    }
    else {

        bindActiveTable(true);
    }
});


function bindActiveTable(checkBStatus) {

    //debugger;;
    var projId = $("#txtProjectId").val();
    var urlActiveRecords = $("#GetTab5Url").val();

    $.ajax({
        url: urlActiveRecords,
        //async: false,
        data: {
            "ActiveStatus": checkBStatus,
            "projectId": projId
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //  data: JSON.stringify(WidgetViewModel),
        type: 'GET',
        success: function (msg) {

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
                    "aaData": msg,
                    "aoColumns": [
                        { "mData": "FieldName" },
                        { "bSearchable": true, "mData": null },
                    ],
                    "aoColumnDefs": [
                        {
                            "mRender": function (data, type, full, row) {
                                if (checkBStatus) { return '<a href="#" id=' + full.ID + ' onclick="Edit(' + full.ID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.ID + ' onclick="Deactivate(' + full.ID + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>'; }
                                else {
                                    return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.ID + ' onclick="Activate(' + full.ID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                }

                            },
                            "aTargets": [1]
                        }
                    ],

                }
            ), $('.dataTables_filter input').attr('maxlength', 50);
            // spinner.stop(target);

            waitingDialog.hide();
        }

    });


}



function Activate(id) {
    //debugger;;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Custom Field");
}

function ActivateRecords(id) {
    //debugger;;
    var urlDeleteData = $('#ActivateCustomFieldRecordUrl').val();
    var uri = urlDeleteData + '?customFieldId=' + id + '&msg=' + "Activate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmActivatedialog").modal('hide');
                bindActiveTable(false);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            //spinner.stop(target);

            waitingDialog.hide();
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id) {

    $("#deactivationID").val(id);
    $("h4.modal-title").text("Deactivate Custom Field");
}

function DeactivateRecords(id) {
    var urlDeleteData = $('#DeactivateCustomFieldRecordUrl').val();
    var uri = urlDeleteData + '?customFieldId=' + id + '&msg=' + "Deactivate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {
            waitingDialog.hide();
            if (msg.success == true) {
                $("#confirmDeactivatedialog").modal('hide');
                bindActiveTable(true);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            //spinner.stop(target);


        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }
    });
}


//#region Activate Record Yes button
$("#btnActivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    var id = $("#activationID").val();
    ActivateRecords(id);
})
//#endregion

//#region Activate Record No button
$("#btnActivatNo").click(function () {

    $("#confirmActivatedialog").modal('hide');
})
//#endregion


//#region Deactivate Record No button
$("#btnDeactivatNo").click(function () {

    $("#confirmDeactivatedialog").modal('hide');
})
//#endregion

//#region Deactivate Record Yes button
$("#btnDeactivateYes").click(function () {
    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    $("#confirmDeactivatedialog").modal('hide');
    var id = $("#deactivationID").val();
    DeactivateRecords(id);
})
//#endregion

//#region Save State button
$("#btnAddNewCustomField").click(function () {
    $("#txtnewCustomFieldName").focus();
    if ($('#txtnewCustomFieldName').val() == '') {
        var $toast = toastr["error"]("Please enter the custom field name.", "Error Notification");
    }
    else {
        waitingDialog.show('Please Wait...');

        //spinner.spin(target);
        EditRecord();
    }

})
//#endregion


function EditRecord() {
    //debugger;;

    var uri = $('#SaveTab5Url').val();
    var projId = $("#txtProjectId").val().trim();

    var id = $("#txtnewCustomFieldId").val().trim() == '' ? 0 : $("#txtnewCustomFieldId").val();
    var customField = $("#txtnewCustomFieldName").val().trim();
    var status = id == 0 ? 1 : 2;


    $.ajax({
        url: uri,
        async: false,
        data: {
            "projectId": projId,
            "CustomFieldId": id,
            "CustomFieldName": customField,
            "status": status,
        },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {

                $("#myModalCustomField").modal('hide');
                $("#txtnewCustomFieldId").val('');
                $("#txtnewCustomFieldName").val('');
                //      spinner.stop(target);

                waitingDialog.hide();
                bindActiveTable(true);

                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            //spinner.stop(target);

            waitingDialog.hide();

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }
    });
}
//#region Add State button
$("#btnAddCustField").click(function () {
    $("#chkShowDeactivate").prop('checked', false);
    $("#txtnewCustomFieldId").val('');
    $("#txtnewCustomFieldName").val('');


    $("#btnReset").show();
    $("h4.modal-title").text("Add New Custom Field");

    $('#myModalCustomField').on('shown.bs.modal', function () {
        $('#txtnewCustomFieldName').focus();
    })

})
//#endregion

$("#btnAddNewCustomFieldCancel").click(function () {

    $("#txtnewCustomFieldName").val('');
    $("#txtnewCustomFieldId").val('');
    $("#myModalCustomField").modal('hide');
})
//#endregion

//#region Reset button on Create Model
$("#btnAddNewCustomFieldReset").click(function () {


    $("#txtnewCustomFieldName").val('');
    $("#txtnewCustomFieldId").val('');
    $("#txtnewCustomFieldName").focus();


})
//#endregion


$("#btnMoveAll").click(function () {
    //debugger;;
    $("#sourceFields1").children().appendTo("#destinationFields1");

})



function Edit(ID) {
    //debugger;;
    NewData = "old";
    var urlGetData = $('#GetCustomFieldRecordUrl').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { "customFieldId": ID },
        success: function (msg) {
            //debugger;;
            $("#txtnewCustomFieldId").val(msg.ID);
            $("#txtnewCustomFieldName").val(msg.FieldName);
            $("#txtnewCustomFieldName").focus();
            $("#btnReset").hide();
            $("h4.modal-title").text("Edit Custom Field");
            $("#myModalCustomField").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }

    });


}




function GetTab_Step6() {
    //debugger;;

    //spinner.spin(target);
    waitingDialog.show('Please Wait...');

    var Projectid = $("#txtProjectId").val().trim();
    var UrlSaveExcelData = $('#GetOldExcelData').val();
    var OLD;
    $.ajax({
        url: UrlSaveExcelData,
        data: { ProjectID: Projectid },
        type: 'GET',
        //async: false,
        success: function (data) {
            if (data.success == true) {
                $("#divPartialFileUpload").empty();
                $("#divPartialFileUpload").append(data.excelData);
            }
            else {
                var $toast = toastr["error"](data.excelData, "Error Notification");
            }
            //  spinner.stop(target);

            waitingDialog.hide();
        }

    });
}

$(document).on('change', $(".dropDownFoo"), function (e) {
    //$("select[id$='DFoo']").change(function () {

    // alert(e.target.options[e.target.selectedIndex].text);
    // var value = e.target.options[e.target.selectedIndex].text;
    var value = $(e.target).val();
    $(".dropDownFoo").each(function (index, item) {

        if ($($(item)).attr("id") != $(e.target).attr("id")) {
            if ($(item).val() == value && $(item).val() != '0') {
                var $toast = toastr["error"]("Selected value not available as it was already mapped to another excel field.", "Error Notification");
                e.target.selectedIndex = 0;
            }
        }
    });
});


$("#btn_Submit").click(function () {
    //debugger;;
    var checkTab7html = $($("#divPartialFileUpload")).children().size();

    if (checkTab7html == null || checkTab7html == 1) {
        var $toast = toastr["error"]("Submit failed. Please upload excel file first to map the excel columns for current project.", "Error Notification");
        return;
    }
    else {
        //spinner.spin(target);
        waitingDialog.show('Please Wait...');


        //debugger;;
        valFields = [];
        FiledType = [];
        ExcelFields = [];
        FieldID = [];
        var text = $("#divPartialFileUpload")//document.getElementById('FrmSaveExcelmappingdata');
        var Projectid = $("#txtProjectId").val().trim();
        for (var i = 1; i <= (text.children().length - 1) ; i++) {

            var lablename = $($(text).children()[i]).children()[1].innerHTML;
            var select = $("#" + ($($($(text).children()[i]).children()[2]).children().attr('id'))).val();
            var select1 = $("#" + ($($($(text).children()[i]).children()[2]).children().attr('id')) + " option:selected").text();

            ExcelFields.push(lablename);
            FieldID.push(select);
            valFields.push(select1);
            //alert(select1);
        }

        // Todays changes dropDownFoo  08/11/2016  
        //DropDownLists validations  
        var arr = ['ROUTE', 'ACCOUNT', 'STREET', 'OLD METER NUMBER', 'CYCLE'];
        for (var i = 0; i < arr.length; i++) {
            if (jQuery.inArray(arr[i], valFields) > -1) {

            }
            else {
                var $toast = toastr["error"]("Please select " + arr[i] + "", "Error Notification");
                waitingDialog.hide();
                return;
            }

        }




        var GetFromID = document.getElementById('FrmSaveExcelmappingdata');
        var abc = $(GetFromID).children().children('div');

        // var hv = $('#ProjectID').val();
        var UrlSaveExcelData = $('#SaveTab6Url').val();





        $.ajax({
            url: UrlSaveExcelData,

            data: { ProjectID: Projectid, ExcelField: ExcelFields, FieldID: FieldID },
            type: 'POST',
            // async: false,
            success: function (data) {
                //debugger;;
                if (data.success == true) {
                    waitingDialog.hide();

                    var $toast = toastr["success"](data.returnMessage, "Success Notification");

                    window.location.href = $('#GetprojectIndex').val();;
                }
                else {
                    waitingDialog.hide();
                    var $toast = toastr["success"](data.returnMessage, "Notification");
                }
                //spinner.stop(target);


            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
                waitingDialog.hide();
            }

        });



        //END

    }



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


var serviceid = "";
$(document).on("click", "#btnAddServiceProduct", function () {
    $('#txtQuantity').val("");
    $("#btnSaveServiceProduct").text("Add")
    serviceid = $(this).attr("name")
    productAddEdit = "Add";
    $("#ddlProductList").attr('disabled', false).trigger("chosen:updated")
    $("#serviceAddProject").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });

    $("#ddlProductList").chosen({ placeholder_text_single: "Select Product", no_results_text: "Oops, nothing found!" });
    $('#ddlProductList').trigger('chosen:updated');

    getProductList(serviceid)
    GetServiceProduct(serviceid)
})

$(document).on("change", "#ddlProductList", function () {
    //debugger;;
    var projectId = $(this).val();

    var selectedproduct = search(projectId, productModel)
    if (selectedproduct.SerialNumberRequired) {
        $('#txtQuantity').val("1").prop("disabled", true);
    }
    else {
        $('#txtQuantity').val("").prop("disabled", false);
    }

});


function search(nameKey, myArray) {
    //debugger;;
    var productid = parseInt(nameKey);
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].productId === productid) {

            return myArray[i];
        }
    }
}

var productModel;
function getProductList() {
    var projectId = $('#txtProjectId').val();

    var UrlGetProjectList = $('#UrlGetProjectList').val();
    waitingDialog.show('Please Wait...');
    $.ajax({
        url: UrlGetProjectList,
        data: { ProjectID: projectId },
        type: 'POST',
        // async: false,
        success: function (data) {
            waitingDialog.hide();
            //debugger;;
            if (data.success == true) {
                $('#ddlProductList').empty();
                productModel = data.poductList;

                $.each(data.poductList, function (index, item) {
                    if (index == 0) {
                        $('#ddlProductList').append(
                            $('<option></option>').val('0').html(""));
                    }

                    $('#ddlProductList').append(
                        $('<option></option>').val(item.productId).html(item.productName)
                    );
                    $('#ddlProductList').trigger('chosen:updated');
                });
            }
            else {

                var $toast = toastr["success"](data.returnMessage, "Notification");
            }

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }

    });


}



function GetServiceProduct(serviceid) {
    var projectId = $('#txtProjectId').val();
    var serviceId = serviceid;
    var UrlGetServiceProduct = $('#UrlGetServiceProduct').val();

    waitingDialog.show('Please Wait...');
    $.ajax({
        url: UrlGetServiceProduct,
        data: { ProjectID: projectId, serviceId: serviceId },
        type: 'GET',
        // async: false,
        success: function (data) {
            waitingDialog.hide();

            $("#tblServiceProduct").html(data)

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }

    });
}


$('#btnSaveServiceProduct').click(function () {
    debugger;
    var productid = $('#ddlProductList').val();
    var quantity = $('#txtQuantity').val();

    if (productid === '0' || productid === '') {
        var $toast = toastr["error"]("Please, select product", "Notification");
        return;
    }

    if (quantity === '0' || quantity === '') {
        var $toast = toastr["error"]("Please, enter quantity", "Notification");
        return;
    }
    
    saveServiceProduct()
})
var productAddEdit = "";
function saveServiceProduct() {
    debugger;
    var projectServiceId = parseInt($('#hndProjectServiceId').val()) || 0;
    var projectId = $('#txtProjectId').val();
    var productid = $('#ddlProductList').val();
    var quantity = $('#txtQuantity').val();
    var serviceId = serviceid;


    var productIds = $(".hndProductId");
    for (var i = 0; i < productIds.length; i++) {
        var hndProductId = $(productIds[i]).val();
        if (isSerialProduct || productAddEdit == "Add") {
            if (hndProductId == productid) {
                var productname = $('#ddlProductList :selected').text();
                var $toast = toastr["error"](productname + " already added", "Error Notification");
                return false;
            }
        }
    }


    waitingDialog.show('Please Wait...');
    var UrlSaveServiceProduct = $('#UrlSaveServiceProduct').val();

    $.ajax({
        url: UrlSaveServiceProduct,
        data: { ProjectID: projectId, serviceId: serviceId, productid: productid, quantity: quantity, projectServiceId: projectServiceId },
        type: 'POST',
        // async: false,
        success: function (data) {
            waitingDialog.hide();
            //debugger;;
            ResetServiceProduct();
            checkTab2Status();      // Add product time relation table get data meter pay rate Recall this method
            $("#tblServiceProduct").html(data)
            

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }

    });
}

var isSerialProduct = false;
function EditProductService(element, pServiceId) {
    debugger;
    var isSerialRequired = $(element).parent().parent().children().find(".SerialNumberRequired").val();

    //debugger;;
    var projectId = $(element).attr("id");
    var quantity = $(element).attr("name");
    var serviceid = pServiceId;

    productAddEdit = "Edit";


    $("#btnSaveServiceProduct").text("Update")

    $('#ddlProductList').val(projectId).attr("selected", "selected").trigger('chosen:updated');
    $('#txtQuantity').val(quantity);

    $('#hndProjectServiceId').val(serviceid);
    if (isSerialRequired == "True") {
        isSerialProduct = true;
        $('#txtQuantity').prop("disabled", true);
    }
    else {
        isSerialProduct = false;
        $('#txtQuantity').prop("disabled", false);
        $("#ddlProductList").attr('disabled', true).trigger("chosen:updated")
    }
}

function DeleteProductService(pServiceId) {
    debugger;
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
                    DeleteProductFromList(pServiceId)
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


function DeleteProductFromList(pServiceId) {
    debugger;
    waitingDialog.show('Please Wait...');
    var UrlDeleteServiceProduct = $('#UrlDeleteServiceProduct').val();
    var projectId = $('#txtProjectId').val();
    $.ajax({
        url: UrlDeleteServiceProduct,
        data: { ProjectID: projectId, projectServiceId: pServiceId, ServiceId: serviceid },
        type: 'GET',
        // async: false,
        success: function (data) {
            waitingDialog.hide();

            ResetServiceProduct();

            checkTab2Status();      // Add product time relation table get data meter pay rate Recall this method

            $("#tblServiceProduct").html("");
            $("#tblServiceProduct").html(data)

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
            waitingDialog.hide();
        }

    });
}

$('#btnResetServiceProduct').click(function () {

    ResetServiceProduct();
})
function ResetServiceProduct() {
    $("#btnSaveServiceProduct").text("Add")
    $('#txtQuantity').val("").prop("disabled", false);

    $('#ddlProductList').val("0").attr("selected", "selected").trigger('chosen:updated');
    $('#hndProjectServiceId').val("");

    $("#ddlProductList").attr('disabled', false).trigger("chosen:updated")

    productAddEdit = "Add";

}