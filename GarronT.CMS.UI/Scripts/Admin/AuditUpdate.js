
var ClassInstallerCycleList = [];
var ClassProjectFailPercentageList = [];


$(document).ready(function () {

    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');

    var actualDate = new Date(); // convert to actual date
    var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());

    debugger
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



    $("#ulAuditor").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {

            }
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
            selectAuditor()
        }
    });

    GetData();
})

//#region New code for Auditor multiselect dropdown
function selectAuditor() {

    var selectedlivalue = $('#ulAuditor li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#AuditorHeader").removeClass("ddlHeader").text("Select Auditor")
    } else {
        $("#AuditorHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulAuditor .liInstaller:visible').length > 0 && $('#ulAuditor .highlighted:visible').length == $('#ulAuditor .liInstaller:visible').length) {
        $('#Auditor_SelectAll').prop('checked', true);
    } else {
        $('#Auditor_SelectAll').prop('checked', false);
    }
    //#endregion

}

//#region open/close auditor dropdown
$("#ancAuditor_chosen").click(function () {
    $("#ddlAuditor_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divAuditor').is(e.target)
       && $('#divAuditor').has(e.target).length === 0
   ) {
        $('#ddlAuditor_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});

//#endregion

function filterAuditor(element) {

    var value = $(element).val().toLowerCase();

    $("#ulAuditor > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulAuditor .liAuditor:visible').length > 0 && $('#ulAuditor .highlighted:visible').length == $('#ulAuditor .liAuditor:visible').length) {
        $('#Auditor_SelectAll').prop('checked', true);
    } else {
        $('#Auditor_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Auditor_SelectAll').on('click', function () {
    if ($('#ulAuditor .liAuditor').length > 0) {

        if (this.checked) {
            $("li.liAuditor:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulAuditor .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#AuditorHeader").removeClass("ddlHeader").text("Select Auditor")
            }
            else {
                $("#AuditorHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
            }

        }
        else {
            $("li.liAuditor:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulAuditor .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#AuditorHeader").removeClass("ddlHeader").text("Select Auditor")
            }
            else {
                $("#AuditorHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
            }
        }
    }
});

//#endregion





//#region save record
$("#btnSave").click(function () {

    var AuditorIdList = [];
    var AuditorId = $('#ulAuditor li.highlighted')
    AuditorId.each(function (index, item) {
        AuditorIdList.push($(this)[0].id)
    });



    var AuditObj = {
        projectId: $("#FKProjectId").val(),
        auditProjectId: $("#txtProjectId").val(),
        startDate: $('#txtFromDate').val(),
        endDate: $('#txtToDate').val(),
        auditorList: AuditorIdList,
        objList: ClassInstallerCycleList,
        objAuditProjectFailBOList: ClassProjectFailPercentageList
    }

    //location.href = $('#urlGoToIndex').val();

    if (AuditorIdList.length == 0) {
        var $toast = toastr["error"]('Please select auditor first', 'Notification');
        return false;
    }
    else {

        var urlGetInstallerList = $('#urlSaveAuditDetails').val();

        $.ajax({
            type: 'POST',
            url: urlGetInstallerList,
            //data: { auditId: Project, startDate: AuditStartDate, endDate: AuditEndDate, auditorList: AuditorIdList },
            data: { objAuditCreateBO: AuditObj },
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (data) {

                if (data.Status == true) {
                    var $toast = toastr["success"]('Record saved successfully...!!!', 'Notification');
                    waitingDialog.hide();
                    //location.href = $('#urlGoToIndex').val();
                }
                else {
                    var $toast = toastr["error"](data.ResultMessage, 'Notification');
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

})
//#endregion





//#region cancel action
$("#btnCancelAudit").click(function () {
    location.href = $('#urlGoToIndex').val();
})
//#endregion


function GetData() {
    var AuditId = $("#txtProjectId").val();

    var urlGetInstallerList = $('#urlGetProjectDataForUpdate').val();

    $.ajax({
        type: 'GET',
        url: urlGetInstallerList,
        data: { auditId: AuditId },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            //waitingDialog.hide();
        },
        success: function (data) {
            debugger;

            $('#lblProjectUtility').html(data.objProjectSmallModel.Utility);
            $('#lblProjectName').html(data.objProjectSmallModel.ProjectName);
            $('#lblProjectName1').html(data.objProjectSmallModel.ProjectName);
            $('#lblCustomerName').html(data.objProjectSmallModel.CustomerName);
            $('#lblProjectStartDate').html(data.objProjectSmallModel.stringFromDate);
            $('#lblProjectEndDate').html(data.objProjectSmallModel.stringToDate);

            $('#txtFromDate').val(data.objProjectSmallModel.stringFromDate);
            $('#txtToDate').val(data.objProjectSmallModel.stringToDate);

            var actualDate1 = stringToDate(data.objProjectSmallModel.stringFromDate, "mmm-dd-yyyy", "-");

            // data.objProjectSmallModel.stringFromDate
            $("#txtFromDate").datepicker("option", "minDate", actualDate1);
            //$("#txtToDate").datepicker("option", "minDate", selectedDate);


            $('#ulAuditor').empty();
            $.each(data.objProjectSmallModel.objPresentList, function (index, item) {
                $('#ulAuditor').append($('<li></li>').attr("id", item.UserId).addClass("liAuditor active-result").html(item.UserName));
            });



            //var count = data.objProjectSmallModel.objPresentList.length;
            //for (i = 0; i < count; i++) {
            //    $('#ulAuditor').append($('<li></li>').val(ata.objPresentList[i].UserId).addClass("liAuditor active-result").html(data.OrignalCycleList[i].UserName));
            //}

            $.each(data.objProjectSmallModel.objLongList, function (index, item) {
                //$("#ulAuditor li").filter(function () { return $(this).text() === item.toString(); }).addClass("ui-selected click-selected highlighted");
                $("#ulAuditor").find("li[id='" + item.toString() + "']").addClass("ui-selected click-selected highlighted ")
            });

            var selectedlivalue = $('#ulAuditor li.highlighted').map(function (i, el) { return $(el).val(); });


            var selectedlilength = selectedlivalue.length;
            if (selectedlilength == 0) {
                $("#AuditorHeader").removeClass("ddlHeader").text("Select Auditor")
            } else {
                $("#AuditorHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
            }

            //#region SelectAll checked/unchecked
            if ($('#ulAuditor .liAuditor:visible').length > 0 && $('#ulAuditor .highlighted:visible').length == $('#ulAuditor .liAuditor:visible').length) {
                $('#Auditor_SelectAll').prop('checked', true);
            } else {
                $('#Auditor_SelectAll').prop('checked', false);
            }
            //#endregion



            if (data.objProjectSmallModel.objList != undefined && data.objProjectSmallModel.objList.length > 0) {
                $.each(data.objProjectSmallModel.objList, function (index, item) {
                    var uuid = guid();
                    var currentObject = {
                        hdnCycleId: uuid,
                        Active: 1,
                        Id: item.Id,
                        AuditStartDate: item.AuditStartDate,
                        AuditEndDate: item.AuditEndDate,
                        InstallerId: item.InstallerId,
                        InstallerName: item.InstallerName,
                        AuditPercentage: item.AuditPercentage,
                        AuditFailed: item.AuditFailed
                    };

                    ClassInstallerCycleList.push(currentObject);

                })
            }

            if (data.objProjectSmallModel.objAuditProjectFailBOList != undefined && data.objProjectSmallModel.objAuditProjectFailBOList.length > 0) {
                $.each(data.objProjectSmallModel.objAuditProjectFailBOList, function (index, item) {
                    var uuid = guid();
                    var currentObject = {
                        hdnFailCycleId: uuid,
                        Active: 1,
                        Id: item.Id,
                        FailPercentageFrom: item.FailPercentageFrom,
                        FailPercentageTo: item.FailPercentageTo,
                        IncreaseByDays: item.IncreaseByDays
                    };

                    ClassProjectFailPercentageList.push(currentObject);

                })
            }


            BindDatatable(ClassInstallerCycleList);

            BindDatatable1();


            waitingDialog.hide();

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







//#region add new Record Audit Percentage
$("#btnAddCustomerPercent").click(function () {
    //debugger;


    var urlAddNewAuditCycle = $('#urlAddNewAuditCycle').val();

    $.ajax({
        url: urlAddNewAuditCycle,
        datatype: "text",
        data: { projectId: $('#FKProjectId').val() },
        type: "GET",
        success: function (data) {
            //debugger;

            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")
            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            $("#InstallerId").chosen({ no_results_text: "Oops, nothing found!" });
            $('#InstallerId').focus();
            //$.validator.unobtrusive.parse("#AddEditModel");
            $('#AddEditModel').draggable({ handle: ".modal-header" });
        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });


})
//#endregion



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

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}


$(document).on("click", '#btnAuditCycleCancel', function () {

    $("#AddEditModel").modal('hide');
    return false;
})





//#region Add Audit Cycle
$(document).on("click", '#btnAuditCycleSave', function () {





    var a = true;

    if ($('#InstallerId').val() == null || $('#InstallerId').val() == '' || $('#InstallerId').val() == "0") {
        var $toast = toastr["error"]("Installer required.", "Error");
        $('#InstallerId').focus();
        a = false;
    }
    else if ($('#AuditPercentage').val() == null || $('#AuditPercentage').val() == "0") {
        var $toast = toastr["error"]("Audit percentage required.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (isNaN($('#AuditPercentage').val())) {
        var $toast = toastr["error"]("Audit percentage must be number.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if ($('#AuditStartDate').val() == null || $('#AuditStartDate').val() == "0") {
        var $toast = toastr["error"]("Start day required.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if (isNaN($('#AuditStartDate').val())) {
        var $toast = toastr["error"]("Start day must be number.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if ($('#AuditEndDate').val() == null || $('#AuditEndDate').val() == "0") {
        var $toast = toastr["error"]("End day required.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (isNaN($('#AuditEndDate').val())) {
        var $toast = toastr["error"]("End day must be number.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (parseFloat($('#AuditPercentage').val()) <= 0) {
        var $toast = toastr["error"]("Audit percentage must be greater than 0.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (parseFloat($('#AuditPercentage').val()) > 100) {
        var $toast = toastr["error"]("Audit percentage must be less or equal to than 100.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (parseInt($('#AuditStartDate').val()) <= 0) {
        var $toast = toastr["error"]("Start day must be greater than 0.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if (parseInt($('#AuditEndDate').val()) <= 0) {
        var $toast = toastr["error"]("End day must be greater than 0.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (parseInt($('#AuditEndDate').val()) < parseInt($('#AuditStartDate').val())) {
        var $toast = toastr["error"]("End day must be greater than start day.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }



    if (a == true) {

        var InstallerId = parseInt($("#InstallerId").val());
        var AuditStartDate = parseInt($("#AuditStartDate").val());
        var AuditEndDate = parseInt($("#AuditEndDate").val());

        if (ClassInstallerCycleList != undefined && ClassInstallerCycleList.length > 0) {
            $.each(ClassInstallerCycleList, function (index, item) {

                if (a === true && item.InstallerId === InstallerId && item.Active === 1 &&
                    ((item.AuditStartDate <= AuditStartDate && item.AuditEndDate >= AuditStartDate)
                || (item.AuditStartDate <= AuditEndDate && item.AuditEndDate >= AuditEndDate)
                || (AuditStartDate <= item.AuditStartDate && AuditEndDate >=item.AuditStartDate)
                || (AuditStartDate <= item.AuditEndDate && AuditEndDate >= item.AuditEndDate))) {

                    var $toast = toastr["error"]("Insert failed, Day range already in use.", "Error");
                    a = false;
                    return
                }

            })
        }


        if (a === true) {

            var uuid = guid();
            var currentObject = {
                hdnCycleId: uuid,
                Active: 1,
                Id: 0,
                AuditStartDate: AuditStartDate,
                AuditEndDate: AuditEndDate,
                InstallerId: InstallerId,
                InstallerName: $("#InstallerId option:selected").text(),
                AuditPercentage: $.trim($("#AuditPercentage").val()),
                AuditFailed: 'No'
            };

            ClassInstallerCycleList.push(currentObject);

           // sortObjects(ClassInstallerCycleList, "InstallerName, AuditStartDate");

            BindDatatable(ClassInstallerCycleList);

            $("#AddEditModel").modal('hide');
        }


    }
    return false;
})

//#endregion


//#region update Audit Cycle

$(document).on("click", '#btnAuditCycleUpdate', function () {



    if ($('#AuditPercentage').val() == null || $('#AuditPercentage').val() == "0") {
        var $toast = toastr["error"]("Audit percentage required.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (isNaN($('#AuditPercentage').val())) {
        var $toast = toastr["error"]("Audit percentage must be number.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if ($('#AuditStartDate').val() == null || $('#AuditStartDate').val() == "0") {
        var $toast = toastr["error"]("Start day required.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if (isNaN($('#AuditStartDate').val())) {
        var $toast = toastr["error"]("Start day must be number.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if ($('#AuditEndDate').val() == null || $('#AuditEndDate').val() == "0") {
        var $toast = toastr["error"]("End day required.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (isNaN($('#AuditEndDate').val())) {
        var $toast = toastr["error"]("End day must be number.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (parseFloat($('#AuditPercentage').val()) <= 0) {
        var $toast = toastr["error"]("Audit percentage must be greater than 0.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (parseFloat($('#AuditPercentage').val()) > 100) {
        var $toast = toastr["error"]("Audit percentage must be less or equal to than 100.", "Error");
        $('#AuditPercentage').focus();
        a = false;
    }
    else if (parseFloat($('#AuditStartDate').val()) <= 0) {
        var $toast = toastr["error"]("Start day must be greater than 0.", "Error");
        $('#AuditStartDate').focus();
        a = false;
    }
    else if (parseFloat($('#AuditEndDate').val()) <= 0) {
        var $toast = toastr["error"]("End day must be greater than 0.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else if (parseFloat($('#AuditEndDate').val()) < parseFloat($('#AuditStartDate').val())) {
        var $toast = toastr["error"]("End day must be greater than start day.", "Error");
        $('#AuditEndDate').focus();
        a = false;
    }
    else {


        var a = true;

        var element = $("#hdnCycleId").val();

        var InstallerId = parseInt($("#InstallerId").val())
        var AuditStartDate = parseInt($("#AuditStartDate").val());
        var AuditEndDate = parseInt($("#AuditEndDate").val());

        $.each(ClassInstallerCycleList, function (index, item) {

            if (a === true && item.InstallerId === InstallerId && item.hdnCycleId != element && item.Active === 1 &&
                ((item.AuditStartDate <= AuditStartDate && item.AuditEndDate >= AuditStartDate)
                || (item.AuditStartDate <= AuditEndDate && item.AuditEndDate >= AuditEndDate)
                || (AuditStartDate <= item.AuditStartDate && AuditEndDate >= item.AuditStartDate)
                || (AuditStartDate <= item.AuditEndDate && AuditEndDate >= item.AuditEndDate)
                )

                ) {

                var $toast = toastr["error"]("Update failed, Day range already in use.", "Error");
                a = false;
                return
            }
        })

        if (a === true) {
            $.each(ClassInstallerCycleList, function (index, item) {

                if (element === item.hdnCycleId) {
                    item.AuditStartDate = AuditStartDate;
                    item.AuditEndDate = AuditEndDate;
                    item.AuditPercentage = $.trim($("#AuditPercentage").val());

                    return;
                }


            })

            // sortObjects(ClassInstallerCycleList, "InstallerName, AuditStartDate");
            BindDatatable(ClassInstallerCycleList);


            $("#AddEditModel").modal('hide');

        }


    }



})
//#endregion






//#region Bind datatable grid'
function BindDatatable(data) {




    oTable = $('#dynamic-table').DataTable(
         {
             "bFilter": false, "bPaginate": false, "bAutoWidth": false, "bDestroy": true,
             "bDeferRender": true, "bLengthChange": true, "bInfo": false, "aaData": ClassInstallerCycleList, "aaSorting": [], "order": [],
             "aoColumns": [
                            { "mData": "InstallerName" },
                            { "mData": "AuditPercentage" },
                            { "mData": "AuditStartDate" },
                            { "mData": "AuditEndDate" },
                             { "mData": "AuditFailed" },
                             { "bSearchable": false, "mData": null },
             ],
             "aoColumnDefs": [
              {
                  "mRender": function (ClassInstallerCycleList, type, full, row) {
                      if (full.Active == true) {
                          return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.hdnCycleId + "' name='' title='Edit'  onclick='Edit(&#39;" + full.hdnCycleId + "&#39;);'><span class='fa fa-pencil-square-o'></span></button>" +

                                 "<button class='btn btn-danger btn-xs'  type='button' id='" + full.hdnCycleId + "' name='' title='Deactivate'  onclick='ActiveDeactivateCycle(&#39;" + full.hdnCycleId + "&#39;,0);'><span class='fa fa-ban'></span></i></button>";
                      }
                      else {
                          return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.hdnCycleId + "' name='' title='Edit' disabled='disabled'><span class='fa fa-pencil-square-o'></span></button>" +

                                 "<button class='btn btn-success btn-xs' type='button' lang='" + full.hdnCycleId + "' name='' title='Activate'  onclick='ActiveDeactivateCycle(&#39;" + full.hdnCycleId + "&#39;,1);'><span class='fa fa-check'></span></button>";
                      }
                  },
                  "aTargets": [5]
              }
             ]
         }
          )
}
//#endregion



//#region Edit record

function Edit(element) {


    var urlEditAuditCycle = $('#urlEditAuditCycle').val();

    $.ajax({
        url: urlEditAuditCycle,
        datatype: "text",
        data: { projectId: $('#FKProjectId').val() },
        type: "GET",
        success: function (data) {
            //debugger;

            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")



            $("#InstallerId").chosen({ no_results_text: "Oops, nothing found!" });
            $('#InstallerId').focus();


            if (ClassInstallerCycleList != undefined && ClassInstallerCycleList.length > 0) {
                $.each(ClassInstallerCycleList, function (index, item) {

                    if (element === item.hdnCycleId) {

                        $("#InstallerId").val(item.InstallerId);
                        $('#InstallerId').trigger('chosen:updated');

                        $("#AuditStartDate").val(item.AuditStartDate);
                        $("#AuditEndDate").val(item.AuditEndDate);
                        $("#AuditPercentage").val(item.AuditPercentage);
                        $("#hdnCycleId").val(item.hdnCycleId);

                        $("#InstallerId").prop("disabled", true).trigger('chosen:updated');
                        return
                    }

                })
            }

            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            $('#AddEditModel').draggable({ handle: ".modal-header" });


        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });



}

//#endregion


//#region Activate record
function ActiveDeactivateCycle(id, action) {

    if (action === 0) { //deactivate record
        $.confirm({
            title: 'Confirmation!',
            content: 'Are you sure you want deactivate this record?',
            type: 'primary',
            buttons: {
                confirm: {
                    text: 'Yes',
                    btnClass: 'btn-primary',
                    action: function () {

                        $.each(ClassInstallerCycleList, function (index, item) {
                            if (id === item.hdnCycleId) {
                                item.Active = 0;
                                BindDatatable(ClassInstallerCycleList);
                                return;
                            }
                        })

                    }
                },
                cancel: function () {
                    text: 'No'
                }
            }
        });
    }
    else {   //Activate record
        $.confirm({
            title: 'Confirmation!',
            content: 'Are you sure you want activate this record? ',
            type: 'primary',
            buttons: {
                confirm: {
                    text: 'Yes',
                    btnClass: 'btn-primary',
                    action: function () {

                        var InstallerId;
                        var AuditStartDate;
                        var AuditEndDate;


                        $.each(ClassInstallerCycleList, function (index, item) {
                            if (id === item.hdnCycleId) {
                                InstallerId = parseInt(item.InstallerId);
                                AuditStartDate = parseInt(item.AuditStartDate);
                                AuditEndDate = parseInt(item.AuditEndDate);
                            }
                        })


                        var a = true;
                        $.each(ClassInstallerCycleList, function (index, item) {

                            if (a === true && item.InstallerId === InstallerId && item.hdnCycleId != id && item.Active === 1 &&
                                ((parseInt(item.AuditStartDate) <= AuditStartDate && parseInt(item.AuditEndDate) >= AuditStartDate)
                                || (parseInt(item.AuditStartDate) <= AuditEndDate && parseInt(item.AuditEndDate) >= AuditEndDate)
                                || (AuditStartDate <= parseInt(item.AuditStartDate) && AuditEndDate >= parseInt(item.AuditStartDate))
                                || (AuditStartDate <= parseInt(item.AuditEndDate) && AuditEndDate >= parseInt(item.AuditEndDate))
                                )) {

                                var $toast = toastr["error"]("Operation failed, day range already in use.", "Error");
                                a = false;
                                return
                            }
                        })

                        if (a === true) {
                            $.each(ClassInstallerCycleList, function (index, item) {
                                if (id === item.hdnCycleId) {
                                    item.Active = 1;
                                    BindDatatable(ClassInstallerCycleList);
                                    return;
                                }
                            })
                        }


                    }
                },
                cancel: function () {
                    text: 'No'
                }
            }
        });
    }



}



function stringToDate(_date, _format, _delimiter) {
    var month = new Array(12);
    month[0] = "Jan";
    month[1] = "Feb";
    month[2] = "Mar";
    month[3] = "Apr";
    month[4] = "May";
    month[5] = "Jun";
    month[6] = "Jul";
    month[7] = "Aug";
    month[8] = "Sep";
    month[9] = "Oct";
    month[10] = "Nov";
    month[11] = "Dec";

    var formatLowerCase = _format.toLowerCase();
    var formatItems = formatLowerCase.split(_delimiter);
    var dateItems = _date.split(_delimiter);
    var monthIndex = formatItems.indexOf("mmm");
    var dayIndex = formatItems.indexOf("dd");
    var yearIndex = formatItems.indexOf("yyyy");
    var month1 = month.indexOf(dateItems[monthIndex]);
    var formatedDate = new Date(dateItems[yearIndex], month1, dateItems[dayIndex]);
    return formatedDate;
}




function getInstallerAuditCycles(projectId) {

    var urlGetInstallerList = $('#urlGetInstallerAuditCycleData').val();


    var ActiveDeactive = true;


    $.ajax({
        type: 'POST',
        url: urlGetInstallerList,
        data: { projectId: projectId },

        success: function (data) {

            if (data.Status == true) {



                oTable = $('#dynamic-table').dataTable(
                         {
                             "bFilter": true,
                             "bLengthChange": true,
                             "bDestroy": true,
                             "bserverSide": true,
                             "bDeferRender": true,
                             "oLanguage": {
                                 "sLengthMenu": '_MENU_ Records per page'
                             },

                             "aaData": data.objList,
                             "aoColumns": [
                                                { "mData": "InstallerName" },
                                                { "mData": "AuditPercentage" },
                                                { "mData": "AuditStartDate" },
                                                { "mData": "AuditEndDate" },
                                                { "mData": "AuditFailReached" },
                                                { "bSearchable": false, "mData": null },
                             ],
                             "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {


                                                      // return '<a id=' + full.Id + ' onclick="Edit(' + full.Id + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>';
                                                      //if (full.Active == true) {
                                                      return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.Id + "' name='' title='Edit'  onclick='Edit(&#39;" + full.Id + "&#39;);'><span class='fa fa-pencil-square-o'></span></button>" +

                                                             "<button class='btn btn-danger btn-xs'  type='button' id='" + full.Id + "' name='' title='Deactivate'  onclick='ActiveDeactivateCycle(&#39;" + full.Id + "&#39;,0);'><span class='fa fa-ban'></span></i></button>";
                                                      //}
                                                      //else {
                                                      //    return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.Id + "' name='' title='Edit' disabled='disabled'><span class='fa fa-pencil-square-o'></span></button>" +

                                                      //           "<button class='btn btn-success btn-xs' type='button' lang='" + full.Id + "' name='' title='Activate'  onclick='ActiveDeactivateCycle(&#39;" + full.Id + "&#39;,1);'><span class='fa fa-check'></span></button>";
                                                      //}

                                                  },
                                                  "aTargets": [5]
                                              }
                             ],

                         }
                          ), $('.dataTables_filter input').attr('maxlength', 50);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        },

    });

    $("#divInstallerAuditCycleInfo").attr('style', 'display: normal');

}




//#region add new Record Fail Percentage
$(document).on("click", "#btnAddFailurePercent", function () {
    debugger;

    var urlAddNewFailCycle = $('#urlAddNewFailCycle').val();

    $.ajax({
        url: urlAddNewFailCycle,
        datatype: "text",
        data: { IsCreate: true },
        type: "GET",
        success: function (data) {
            debugger;

            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")
            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            $('#AddEditModel').draggable({ handle: ".modal-header" });
        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });

})
//#endregion





//#region cancel Fail Percentage popup
$(document).on("click", "#btnFailCancel", function () {
    $("#AddEditModel").modal('hide');
    return false;
})
//endregion



//#region Bind datatable grid'
function BindDatatable1() {




    oTable = $('#dynamic-table1').DataTable(
         {
             "bFilter": false, "bPaginate": false, "bAutoWidth": false, "bDestroy": true,
             "bDeferRender": true, "bLengthChange": true, "bInfo": false, "aaData": ClassProjectFailPercentageList, "aaSorting": [], "order": [],
             "aoColumns": [
                            { "mData": "FailPercentageFrom" },
                            { "mData": "FailPercentageTo" },
                             { "mData": "IncreaseByDays" },
                             { "bSearchable": false, "mData": null },
             ],
             "aoColumnDefs": [
              {
                  "mRender": function (ClassProjectFailPercentageList, type, full, row) {
                      if (full.Active == 1) {
                          return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.hdnFailCycleId + "' name='' title='Edit'  onclick='EditFail(&#39;" + full.hdnFailCycleId + "&#39;);'><span class='fa fa-pencil-square-o'></span></button>" +

                                 "<button class='btn btn-danger btn-xs'  type='button' id='" + full.hdnFailCycleId + "' name='' title='Deactivate'  onclick='ActiveDeactivateFailCycle(&#39;" + full.hdnFailCycleId + "&#39;,0);'><span class='fa fa-ban'></span></i></button>";
                      }
                      else {
                          return "<button class='btn btn-primary btn-xs' type='button' lang='" + full.hdnFailCycleId + "' name='' title='Edit' disabled='disabled'><span class='fa fa-pencil-square-o'></span></button>" +

                                 "<button class='btn btn-success btn-xs' type='button' lang='" + full.hdnFailCycleId + "' name='' title='Activate'  onclick='ActiveDeactivateFailCycle(&#39;" + full.hdnFailCycleId + "&#39;,1);'><span class='fa fa-check'></span></button>";
                      }
                  },
                  "aTargets": [3]
              }
             ]
         }
          )
}
//#endregion



$(document).on("click", "#btnFailSave", function () {

    var a = true;

    if ($('#FailPercentageFrom').val() == null || $('#FailPercentageFrom').val() == '' || $('#FailPercentageFrom').val() == "0") {
        var $toast = toastr["error"]("Please enter percentage from.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if ($('#FailPercentageTo').val() == null || $('#FailPercentageTo').val() == '' || $('#FailPercentageTo').val() == "0") {
        var $toast = toastr["error"]("Please enter percentage to.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (isNaN($('#FailPercentageFrom').val())) {
        var $toast = toastr["error"]("Fail percentage from must be number.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (isNaN($('#FailPercentageTo').val())) {
        var $toast = toastr["error"]("Fail percentage to must be number.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if ($('#IncreaseByDays').val() == null || $('#IncreaseByDays').val() == '' || $('#IncreaseByDays').val() == "0") {
        var $toast = toastr["error"]("Increase by day required.", "Error");
        $('#IncreaseByDays').focus();
        a = false;
    }
    else if (isNaN($('#IncreaseByDays').val())) {
        var $toast = toastr["error"]("Increase by day must be number.", "Error");
        $('#IncreaseByDays').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageFrom').val()) <= 0) {
        var $toast = toastr["error"]("Fail percentage from must be greater than 0.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageFrom').val()) > 100) {
        var $toast = toastr["error"]("Fail percentage from must be less than or equal to 100.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (parseInt($('#FailPercentageTo').val()) <= 0) {
        var $toast = toastr["error"]("fail percentage to must be greater than 0.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageTo').val()) > 100) {
        var $toast = toastr["error"]("Fail percentage to must be less than or equal to 100.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageTo').val()) <= parseFloat($('#FailPercentageFrom').val())) {
        var $toast = toastr["error"]("Fail percentage to must be greater than fail percentage from.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }

    if (a == true) {


        var FailPercentageFrom = parseFloat($('#FailPercentageFrom').val());
        var FailPercentageTo = parseFloat($('#FailPercentageTo').val());

        if (ClassProjectFailPercentageList != undefined && ClassProjectFailPercentageList.length > 0) {
            $.each(ClassProjectFailPercentageList, function (index, item) {

                if (a === true && item.Active === 1 &&
                    ((item.FailPercentageFrom <= FailPercentageFrom && item.FailPercentageTo >= FailPercentageFrom)
                || (item.FailPercentageFrom <= FailPercentageTo && item.FailPercentageTo >= FailPercentageTo)
            || (FailPercentageFrom <= item.FailPercentageFrom && FailPercentageTo >= item.FailPercentageFrom)
            || (FailPercentageFrom <= item.FailPercentageTo && FailPercentageTo >= item.FailPercentageTo))) {

                    var $toast = toastr["error"]("Fail percentage range already in use.", "Error");
                    a = false;
                    return
                }

            })
        }


        if (a === true) {

            var uuid = guid();
            var currentObject = {
                hdnFailCycleId: uuid,
                Active: 1,
                Id: 0,
                FailPercentageFrom: FailPercentageFrom,
                FailPercentageTo: FailPercentageTo,
                IncreaseByDays: $("#IncreaseByDays").val()
            };

            ClassProjectFailPercentageList.push(currentObject);

            //sortObjects(ClassProjectFailPercentageList, "FailPercentageFrom");

            BindDatatable1();

            $("#AddEditModel").modal('hide');
        }


    }
    return false;
})



//#region Edit record

function EditFail(element) {


    var urlAddNewFailCycle = $('#urlAddNewFailCycle').val();

    $.ajax({
        url: urlAddNewFailCycle,
        datatype: "text",
        data: { IsCreate: false },
        type: "GET",
        success: function (data) {
            debugger;

            $('#AddEditModel').html(data);
            fnValidateDynamicContent("#AddEditModel")

            if (ClassProjectFailPercentageList != undefined && ClassProjectFailPercentageList.length > 0) {
                $.each(ClassProjectFailPercentageList, function (index, item) {

                    if (element === item.hdnFailCycleId) {

                        $("#FailPercentageFrom").val(item.FailPercentageFrom);
                        $("#FailPercentageTo").val(item.FailPercentageTo);
                        $("#IncreaseByDays").val(item.IncreaseByDays);
                        $('#hdnFailCycleId').val(item.hdnFailCycleId);
                        return
                    }

                })
            }
            $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            $('#AddEditModel').draggable({ handle: ".modal-header" });
        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });




}

//#endregion

$(document).on("click", "#btnFailUpdate", function () {

    var a = true;

    if ($('#FailPercentageFrom').val() == null || $('#FailPercentageFrom').val() == '' || $('#FailPercentageFrom').val() == "0") {
        var $toast = toastr["error"]("Please enter percentage from.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if ($('#FailPercentageTo').val() == null || $('#FailPercentageTo').val() == '' || $('#FailPercentageTo').val() == "0") {
        var $toast = toastr["error"]("Please enter percentage to.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (isNaN($('#FailPercentageFrom').val())) {
        var $toast = toastr["error"]("Fail percentage from must be number.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (isNaN($('#FailPercentageTo').val())) {
        var $toast = toastr["error"]("Fail percentage to must be number.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if ($('#IncreaseByDays').val() == null || $('#IncreaseByDays').val() == '' || $('#IncreaseByDays').val() == "0") {
        var $toast = toastr["error"]("Increase by day required.", "Error");
        $('#IncreaseByDays').focus();
        a = false;
    }
    else if (isNaN($('#IncreaseByDays').val())) {
        var $toast = toastr["error"]("Increase by day must be number.", "Error");
        $('#IncreaseByDays').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageFrom').val()) <= 0) {
        var $toast = toastr["error"]("Fail percentage from must be greater than 0.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageFrom').val()) > 100) {
        var $toast = toastr["error"]("Fail percentage from must be less than or equal to 100.", "Error");
        $('#FailPercentageFrom').focus();
        a = false;
    }
    else if (parseInt($('#FailPercentageTo').val()) <= 0) {
        var $toast = toastr["error"]("fail percentage to must be greater than 0.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageTo').val()) > 100) {
        var $toast = toastr["error"]("Fail percentage to must be less than or equal to 100.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }
    else if (parseFloat($('#FailPercentageTo').val()) <= parseFloat($('#FailPercentageFrom').val())) {
        var $toast = toastr["error"]("Fail percentage to must be greater than fail percentage from.", "Error");
        $('#FailPercentageTo').focus();
        a = false;
    }

    if (a == true) {

        var Id = $('#hdnFailCycleId').val();
        var FailPercentageFrom = parseFloat($('#FailPercentageFrom').val());
        var FailPercentageTo = parseFloat($('#FailPercentageTo').val());

        if (ClassProjectFailPercentageList != undefined && ClassProjectFailPercentageList.length > 0) {
            $.each(ClassProjectFailPercentageList, function (index, item) {

                if (a === true && item.hdnFailCycleId != Id && item.Active === 1 &&
                    ((item.FailPercentageFrom <= FailPercentageFrom && item.FailPercentageTo >= FailPercentageFrom)
                || (item.FailPercentageFrom <= FailPercentageTo && item.FailPercentageTo >= FailPercentageTo)
                || (FailPercentageFrom <= item.FailPercentageFrom && FailPercentageTo >= item.FailPercentageFrom)
                || (FailPercentageFrom <= item.FailPercentageTo && FailPercentageTo >= item.FailPercentageTo))) {

                    var $toast = toastr["error"]("Fail percentage range already in use.", "Error");
                    a = false;
                    return
                }

            })
        }


        if (a === true) {



            $.each(ClassProjectFailPercentageList, function (index, item) {

                if (Id === item.hdnFailCycleId) {
                    item.FailPercentageFrom = FailPercentageFrom;
                    item.FailPercentageTo = FailPercentageTo;
                    item.IncreaseByDays = $("#IncreaseByDays").val();

                    return;
                }


            })

           // sortObjects(ClassProjectFailPercentageList, "FailPercentageFrom");

            BindDatatable1();

            $("#AddEditModel").modal('hide');
        }


    }
    return false;
})


//#region Activate record
function ActiveDeactivateFailCycle(id, action) {

    if (action === 0) { //deactivate record
        $.confirm({
            title: 'Confirmation!',
            content: 'Are you sure you want deactivate this record? ',
            type: 'primary',
            buttons: {
                confirm: {
                    text: 'Yes',
                    btnClass: 'btn-primary',
                    action: function () {

                        $.each(ClassProjectFailPercentageList, function (index, item) {
                            if (id === item.hdnFailCycleId) {

                                item.Active = 0;
                                BindDatatable1();
                                return;
                            }

                        })
                    }
                },
                cancel: function () {
                    text: 'No'
                }
            }
        });
    }
    else {   //Activate record
        $.confirm({
            title: 'Confirmation!',
            content: 'Are you sure you want activate this record? ',
            type: 'primary',
            buttons: {
                confirm: {
                    text: 'Yes',
                    btnClass: 'btn-primary',
                    action: function () {

                        var FailPercentageFrom;
                        var FailPercentageTo;


                        $.each(ClassProjectFailPercentageList, function (index, item) {
                            if (id === item.hdnFailCycleId) {
                                FailPercentageFrom = item.FailPercentageFrom;
                                FailPercentageTo = item.FailPercentageTo;
                            }
                        })


                        var a = true;
                        $.each(ClassProjectFailPercentageList, function (index, item) {

                            if (a === true && item.hdnFailCycleId != id && item.Active === 1 &&
                                ((item.FailPercentageFrom <= FailPercentageFrom && item.FailPercentageTo >= FailPercentageFrom)
                                || (item.FailPercentageFrom <= FailPercentageTo && item.FailPercentageTo >= FailPercentageTo)
                                || (FailPercentageFrom <= item.FailPercentageFrom && FailPercentageTo >= item.FailPercentageFrom)
                                || (FailPercentageFrom <= item.FailPercentageTo && FailPercentageTo >= item.FailPercentageTo))) {

                                var $toast = toastr["error"]("Operation failed, day range already in use.", "Error");
                                a = false;
                                return
                            }
                        })

                        if (a === true) {
                            $.each(ClassProjectFailPercentageList, function (index, item) {
                                if (id === item.hdnFailCycleId) {
                                    item.Active = 1;
                                    BindDatatable1();
                                    return;
                                }
                            })
                        }



                    }
                },
                cancel: function () {
                    text: 'No'
                }
            }
        });
    }



}
