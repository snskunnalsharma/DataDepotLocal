var oTable;
var navListItems;
var count = 1;
var CycleID;
var Lat = [];
var Lag = [];
var Address = [];
var AddressID = [];
var Installer;
var Bindproject, BindCycle, BindRoot, BindAddress;
var GlobalProjectID;
var BindInstaller;
var _ProjectList;
$(document).ready(function () {
    //$("#Reports").addClass('active').css('background-color', '#EF6C00').css('border-radius', '6px');
    //$("#Reports").children().css('color', '#fff');
    $('#dynamic-table').DataTable({
        "sDom": '<"dt_head"fp>t<"F"l>'
    });

    $("#Reports").addClass('active');
    $("#subReports").addClass('block');

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liAuditorReport');
    listate.addClass("active");

    //#endregion

    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlReopenInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlReasonMaster").chosen({ no_results_text: "Oops, nothing found!" });

    setControllOnBack();


    var actualDate = new Date(); // convert to actual date
    var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());




    $("#datepicker1").datepicker({
        dateFormat: 'M-dd-yy',
        //numberOfMonths: 2,
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate());
            $("#datepicker2").datepicker("option", "minDate", dt);
        }
    });
    $("#datepicker2").datepicker({
        dateFormat: 'M-dd-yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate());
            $("#datepicker1").datepicker("option", "maxDate", dt);
        }
    });


    //Aniket code

    //#region ProjectStatus dropdown

    $("#ulProjectStatus").selectable({

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

            selectProjectStatus()

            $('.txtfilterall').val("");
            $('.chbSelectAll').prop('checked', false);

        }
    });

    //#endregion


    //#region Status dropdown

    $("#ulStatus").selectable({

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

            selectStatus()

            $('.txtfilterall').val("");
            $('.chbSelectAll').prop('checked', false);

        }

    });

    //#endregion

    //#region Installer dropdown

    $("#ulInstaller").selectable({

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
            selectInstaller()
        }
    });

    //#endregion

    //#region Cycle dropdown

    $("#ulCycle").selectable({

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

            selectCycle()

        }
    });

    //#endregion

    //#region Route dropdown

    $("#ulRoute").selectable({

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
            selectRoute()
        }
    });

    //#endregion

    //#region Address dropdown

    $("#ulAddress").selectable({

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

            selectAddress()

        }
    });

    //#endregion

    GetProjectStatusList()
    GetStatusList()
});

function setControllOnBack() {

    var urlRoleData = $('#UrlgetListDetailsOnBack').val();
    $.ajax({
        type: 'POST',
        url: urlRoleData,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if ((msg != null && msg.status == true && msg.demo1 != null && msg.demo1.length != 0)) {
                bindTable()
            }
            else {
                // var $toast = toastr["error"]('No data found for the current search.', '');

                bindTable();
            }

        }
    });
};




//Aniket code
function GetProjectStatusList() {
    var ProjectStatusList = [];
    ProjectStatusList = [
        { ID: 0, ProjectStatus: "Active" },
        { ID: 1, ProjectStatus: "Complete" },
    ]


    $('#ulProjectStatus').empty();
    $("#ProjectStatusHeader").removeClass("ddlHeader").text("Select Project Status")
    $("#ProjectStatus_SelectAll").prop("checked", false);
    $.each(ProjectStatusList, function (index, item) {
        $('#ulProjectStatus').append($('<li></li>').val(item.ID).attr("ID", item.ProjectStatus).addClass("liProjectStatus active-result").html(item.ProjectStatus));
    });


}

//Vishal code
function GetStatusList() {
    var ProjectStatusList = [];
    ProjectStatusList = [
        { ID: "SKIP", ProjectStatus: "SKIP" },
        { ID: "COMPLETED", ProjectStatus: "COMPLETED" },
        //{ ID: "RTU", ProjectStatus: "RTU" },
        { ID: "PENDING", ProjectStatus: "PENDING" },
        { ID: "NOT ALLOCATED", ProjectStatus: "NOT ALLOCATED" },
    ]


    $('#ulStatus').empty();
    $("#StatusHeader").removeClass("ddlHeader").text("Select Visit Status")
    $("#Status_SelectAll").prop("checked", false);
    $.each(ProjectStatusList, function (index, item) {
        $('#ulStatus').append($('<li></li>').val(item.ID).attr("ID", item.ProjectStatus).addClass("liStatus active-result").html(item.ProjectStatus));
    });


}

function GetProjectListbyProjectStatus() {

    var objProjectStatusList = [];
    var ProjectStatusvalue = $('#ulProjectStatus li.highlighted')
    ProjectStatusvalue.each(function (index, item) {
        objProjectStatusList.push($(this)[0].id)
    });

    var GetProject = $('#urlGetProjectbyProjectStatus').val();

    $.ajax({
        type: 'POST',
        url: GetProject,
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        data: { ProjectStatusList: objProjectStatusList },
        success: function (ProjectList) {
            _ProjectList = ProjectList;
            $('#ddlProject').empty();
            $('#ddlProject').append($('<option></option>').val(''));
            $.each(ProjectList, function (index, item) {
                $('#ddlProject').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));

            });

            $('.chosen-select').trigger('chosen:updated');

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

$('#ddlProject').change(function () {

    var projectID = $("#ddlProject option:selected").val();
    ResetInstaller()
    if (projectID != "0" && projectID != undefined) {

        $("#datepicker").datepicker("destroy");
        $("#datepicker1").datepicker("destroy");

        if (_ProjectList != undefined && _ProjectList != null) {
            $.each(_ProjectList, function (index, item) {
                if (item.ProjectId == projectID) {
                    debugger;
                    prjFromDate = new Date(item.stringFromDate);
                    prjToDate = new Date(item.stringToDate);
                    var todaysDate = new Date();
                    //alert(item.stringFromDate + " " + item.stringToDate)

                    $("#datepicker1").datepicker({
                        changeMonth: true,
                        changeYear: true,
                        minDate: prjFromDate,
                        maxDate: prjToDate,
                        onSelect: function () {

                            var date = $("#datepicker1").val();
                            if ($.trim(date) != "") {
                                var newDate1 = new Date(date);
                                var strfromdate1 = $("#datepicker2").val();
                                var fromdate = new Date($("#datepicker2").val());

                                $("#datepicker2").datepicker('option', 'minDate', newDate1);
                                if (newDate1 > fromdate) {
                                    $("#datepicker2").val(date)
                                }
                                else {
                                    $("#datepicker2").val(strfromdate1)
                                }


                            }


                            //validator.resetForm();

                            //$('#ddlProject').val(0).attr("selected", "selected");
                            //$("#ddlProject").multiselect("rebuild");
                            //$('#ddlProject').find('option:first').attr('selected', 'selected');
                            // $('#ddlInstaller').empty();
                            // $('#ddlCycle').empty();
                            $('#ddlRoot').empty();
                            $('#ddlAddress').empty();
                            $("#ddlRoot").multiselect("rebuild");
                            $("#ddlAddress").multiselect("rebuild");
                            // $("#ddlCycle").multiselect("rebuild");
                            //$("#ddlInstaller").multiselect("rebuild");
                            $("#AddressDropDownDiv").slideUp("fast");
                            $('#MultipleSelectBox_Address').empty();
                            var $spandiv = $("#btnAddressbtn");
                            $spandiv.children("span").text("Select Street")
                            $spandiv.attr("title", "Select Street")


                            $("#CycleDropDownDiv").slideUp("fast");
                            $("#MultipleSelectBox_Cycle").children("li").removeClass("selected").removeClass("selecting")
                            var $spandiv1 = $("#btnCyclebtn");
                            $spandiv1.children("span").text("Select Cycle")
                            $spandiv1.attr("title", "Select Cycle")


                            $("#RouteDropDownDiv").slideUp("fast");
                            $('#MultipleSelectBox_Route').empty();
                            var $spandiv2 = $("#btnRoutebtn");
                            $spandiv2.children("span").text("Select Route")
                            $spandiv2.attr("title", "Select Route")
                            $('#ddlInstaller').trigger("change");
                            // initialize()
                        },
                        //dateFormat: 'dd MM yy',
                        dateFormat: 'M-dd-yy',
                    }).datepicker("setDate", todaysDate);


                    $("#datepicker2").datepicker({
                        changeMonth: true,
                        changeYear: true,
                        minDate: prjFromDate,
                        maxDate: prjToDate,
                        onSelect: function () {
                            //validator.resetForm();

                            $('#ddlRoot').empty();
                            $('#ddlAddress').empty();
                            $("#ddlRoot").multiselect("rebuild");
                            $("#ddlAddress").multiselect("rebuild");
                            // $("#ddlCycle").multiselect("rebuild");
                            //$("#ddlInstaller").multiselect("rebuild");
                            $("#AddressDropDownDiv").slideUp("fast");
                            $('#MultipleSelectBox_Address').empty();
                            var $spandiv = $("#btnAddressbtn");
                            $spandiv.children("span").text("Select Street")
                            $spandiv.attr("title", "Select Street")

                            $("#CycleDropDownDiv").slideUp("fast");
                            $("#MultipleSelectBox_Cycle").children("li").removeClass("selected").removeClass("selecting")
                            var $spandiv1 = $("#btnCyclebtn");
                            $spandiv1.children("span").text("Select Cycle")
                            $spandiv1.attr("title", "Select Cycle")

                            $("#RouteDropDownDiv").slideUp("fast");
                            $('#MultipleSelectBox_Route').empty();
                            var $spandiv2 = $("#btnRoutebtn");
                            $spandiv2.children("span").text("Select Route")
                            $spandiv2.attr("title", "Select Route")
                            $('#ddlInstaller').trigger("change");
                            // initialize()
                        },
                        dateFormat: 'M-dd-yy',
                    }).datepicker("setDate", todaysDate);
                }

            })
        }

        GetAuditorByProjectID(projectID)
        GetCycleListbyProjectID(projectID)

    }

});

function GetAuditorByProjectID(projectID) {
    var urlGetAuditorList = $('#urlGetAuditorList').val();

    var i = 0;
    $.ajax({
        url: urlGetAuditorList,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        data: { ProjectID: projectID },
        success: function (installerList) {

            $('#ulInstaller').empty();

            $.each(installerList, function (index, item) {

                $('#ulInstaller').append($('<li></li>').val(item.UserId).attr("ID", item.UserId).addClass("liInstaller active-result").html(item.UserName));
            });

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

function GetCycleListbyProjectID(projectID) {

    var urlCycleData = $('#GetCycle').val();

    var i = 0;
    $.ajax({
        url: urlCycleData,
        // async: false,
        data: { ProjectID: projectID },
        success: function (CycleList) {

            $('#ulCycle').empty();

            $.each(CycleList, function (index, item) {

                $('#ulCycle').append($('<li></li>').val(item).addClass("liCycle active-result").html(item));
            });

        },
        error: function () {
        }
    });
}

$("#btnReset").click(function () {
    ResetAllData()

})


function ResetProject() {
    $('#ddlProject').empty();
    $('.chosen-select').trigger('chosen:updated');

    ResetInstaller()
}

function ResetInstaller() {
    $('#ulInstaller').empty();
    $("#InstallerHeader").removeClass("ddlHeader").text("Select Auditor")
    $('#txtfilterallInstaller').val("");
    $('#Installer_SelectAll').prop('checked', false);
    ResetCycle()
}

function ResetCycle() {
    $('#ulCycle').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $('#txtfilterallCycle').val("");
    $('#Cycle_SelectAll').prop('checked', false);
    ResetRoute()
}

function ResetRoute() {
    $('#ulRoute').empty();
    $("#RouteHeader").removeClass("ddlHeader").text("Select Route")
    $('#txtfilterallRoute').val("");
    $('#Route_SelectAll').prop('checked', false);


    ResetAddress()
}

function ResetAddress() {
    $('#txtfilterallAddress').val("");
    $('#Address_SelectAll').prop('checked', false);
    $('#ulAddress').empty();
    $("#AddressHeader").removeClass("ddlHeader").text("Select Address")
}

function ResetAllData() {
    GetProjectStatusList()
    GetStatusList()
    ResetProject()
    $('.txtfilterall').val("");
    $('.chbSelectAll').prop('checked', false);

}

//#region Cancel button on Create Model
$("#btnCancel1").click(function () {

    //location.href = $('#UrlDashboard').val()
    $("#myModal").modal('hide');
})
//#endregion




$('#btnSearch').unbind("click").click(function () {

    $("#example-select-all").attr('checked', false);
    _searchClick();
});


function SelectAll_Recods() {
    waitingDialog.show('Loading Please Wait...');
    var objProjectStatusList = [];
    var ProjectStatusvalue = $('#ulProjectStatus li.highlighted')
    ProjectStatusvalue.each(function (index, item) {
        objProjectStatusList.push($(this)[0].id)
    });



    var objStatusList = [];
    var Statusvalue = $('#ulStatus li.highlighted')
    Statusvalue.each(function (index, item) {
        objStatusList.push($(this)[0].id)
        //  objStatusList.push($(this).text())
    });



    var ProjectId = $("#ddlProject").val();


    var InstallerIdList = [];
    var InstallerId = $('#ulInstaller li.highlighted')
    InstallerId.each(function (index, item) {
        InstallerIdList.push($(this)[0].id)
    });

    var cycleIdList = [];
    var cycleId = $('#ulCycle li.highlighted')
    cycleId.each(function (index, item) {

        cycleIdList.push($(this).val())

    });

    var RouteIdList = [];
    var RouteId = $('#ulRoute li.highlighted')
    RouteId.each(function (index, item) {
        RouteIdList.push($(this).val())
    });

    var AdressIdList = [];
    var AdressId = $('#ulAddress li.highlighted')
    AdressId.each(function (index, item) {
        AdressIdList.push($(this).text())
    });



    var fromDate = $('#datepicker1').val();
    var Todate = $('#datepicker2').val();
    var urlRoleData = $('#urlSelectAll').val();
    $.ajax({
        type: 'POST',
        url: urlRoleData,
        data: { ProjectId: ProjectId, InstallerId: InstallerIdList, fromDate: fromDate, Todate: Todate, cycleId: cycleIdList, RouteId: RouteIdList, AdressId: AdressIdList, Status: objStatusList },
        //beforeSend: function () {
        //    waitingDialog.show('Loading Please Wait...');
        //},
        success: function (msg) {

            if (msg.status == true) {
                //bindTable()
                oTable = $('#dynamic-table').DataTable(
                    {
                        "bFilter": true,
                        "bLengthChange": true,
                        "bDestroy": true,
                        "bserverSide": true,
                        'responsive': true,
                        "bDeferRender": true,
                        //"sAjaxDataProp": 'aData',
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },

                        "aaSorting": [],
                        "iDisplayLength": -1,
                        "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                        // "sDom": 'ltipr',


                        "aaData": msg.demo1,
                        "aoColumns": [
                            { "bSearchable": false, "mData": null },
                            { "mData": "Dates" },
                            { "mData": "Account" },
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "Street" },
                            //{ "mData": "FormName" },
                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterSize" },
                            //{ "mData": "OLDMETERFINALREADING" },
                            { "mData": "NewMeterNo" },
                            { "mData": "NewMeterSize" },
                            { "mData": "NEWRADIONUMBER" },
                            { "mData": "FirstName" },
                            { "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            { "bSearchable": false, "mData": null },
                        ],
                        "aoColumnDefs": [
                            {
                                "mRender": function (data, type, full, row) {
                                    var timeInMillis = Date.parse(full.Dates);

                                    if (full.ProjectStatus == "Active") {
                                        if (full.CategoryType == "COMPLETED") {
                                            return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                                '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                        } else {
                                            if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {
                                                return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                                    //Export_SKipRTU Added by AniketJ on 1-sep-2016
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'


                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                            } else {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'

                                            }
                                        }
                                    }
                                    else {
                                        if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {

                                            return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'

                                        } else {
                                            //return
                                            //'<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                            if (full.CategoryType == "COMPLETED") {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'

                                            } else {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'
                                            }


                                        }

                                    }
                                },
                                "aTargets": [13]
                            },
                            {
                                "mRender": function (data, type, full, row) {
                                    if ($("#example-select-all").prop("checked") == true) {
                                        return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]" checked >';

                                    } else {
                                        return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]"  >';
                                    }

                                },
                                "aTargets": [0]
                            },
                            //{
                            //    "mRender": function (data, type, full, row) {
                            //        if (full.ShortComment != "") {
                            //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + ' checked " >';
                            //        } else {
                            //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + '" >';
                            //        }
                            //    },
                            //    "aTargets": [13]
                            //},



                        ],
                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
            }
            waitingDialog.hide();

            if ((msg.length == 0)) {
                var $toast = toastr["error"]('No data found for the current search.', '');
            }
            //bindTable();

        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

function _searchClick() {

    var objProjectStatusList = [];
    var ProjectStatusvalue = $('#ulProjectStatus li.highlighted')
    ProjectStatusvalue.each(function (index, item) { objProjectStatusList.push($(this)[0].id) });

    if (objProjectStatusList.length < 1) {
        var $toast = toastr["error"]('Please select Project Status first', 'Error Notification');
        return false;
    }

    var objStatusList = [];
    var Statusvalue = $('#ulStatus li.highlighted')
    Statusvalue.each(function (index, item) { objStatusList.push($(this)[0].id) });

    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select project name first', 'Error Notification');
        return false;
    }

    var ProjectId = $("#ddlProject").val();

    var InstallerIdList = [];
    var InstallerId = $('#ulInstaller li.highlighted')
    InstallerId.each(function (index, item) { InstallerIdList.push($(this)[0].id) });

    var cycleIdList = [];
    var cycleId = $('#ulCycle li.highlighted')
    cycleId.each(function (index, item) { cycleIdList.push($(this).text()) });

    var RouteIdList = [];
    var RouteId = $('#ulRoute li.highlighted')
    RouteId.each(function (index, item) { RouteIdList.push($(this).text()) });

    var AdressIdList = [];
    var AdressId = $('#ulAddress li.highlighted')
    AdressId.each(function (index, item) { AdressIdList.push($(this).text()) });


    var fromDate = $('#datepicker1').val();
    var Todate = $('#datepicker2').val();
    var urlRoleData = $('#UrlgetListDetails').val();

    $.ajax({
        type: 'POST',
        url: urlRoleData,
        data: { ProjectId: ProjectId, AuditorId: InstallerIdList, fromDate: fromDate, Todate: Todate, cycleId: cycleIdList, RouteId: RouteIdList, AdressId: AdressIdList, Status: objStatusList },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        success: function (msg) {

            if (msg.status == true) {



                oTable = $('#dynamic-table').DataTable(
                    {
                        "bFilter": true,
                        "bLengthChange": true,
                        "bDestroy": true,
                        "bserverSide": true,
                        'responsive': true,
                        "bDeferRender": true,
                        //"sAjaxDataProp": 'aData',
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },

                        "aaSorting": [],
                        "iDisplayLength": -1,
                        "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                        // "sDom": 'ltipr',


                        "aaData": msg.demo1,
                        "aoColumns": [
                            //{ "bSearchable": false, "mData": null },
                            //{ "mData": "Dates" },
                            //{ "mData": "Account" },
                            //{ "mData": "FormName" },
                            //{ "mData": "Cycle" },
                            //{ "mData": "Route" },
                            //{ "mData": "Street" },
                            //{ "mData": "FirstName" },
                            //{ "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            //{ "bSearchable": false, "mData": null },


                            { "bSearchable": false, "mData": null },
                            { "mData": "Dates" },
                            { "mData": "Account" },
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "Street" },
                            //{ "mData": "FormName" },
                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterSize" },
                            //{ "mData": "OLDMETERFINALREADING" },
                            { "mData": "NewMeterNo" },
                            { "mData": "NewMeterSize" },
                            { "mData": "NEWRADIONUMBER" },
                            { "mData": "FirstName" },
                            { "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            { "bSearchable": false, "mData": null },

                        ],
                        "aoColumnDefs": [
                            {
                                "mRender": function (data, type, full, row) {
                                    var timeInMillis = Date.parse(full.Dates);

                                    if (full.ProjectStatus == "Active") {
                                        if (full.CategoryType == "COMPLETED") {
                                            return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.AuditorId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                            // +'<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.AuditorId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                        } else {
                                            if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {
                                                return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.AuditorId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                                    //Export_SKipRTU Added by AniketJ on 1-sep-2016
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'


                                                //+ '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.AuditorId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                            } else {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'

                                            }
                                        }
                                    }
                                    else {
                                        if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {

                                            return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.AuditorId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'

                                        } else {
                                            //return
                                            //'<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                            if (full.CategoryType == "COMPLETED") {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'

                                            } else {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'
                                            }


                                        }

                                    }
                                },
                                "aTargets": [13]
                            },
                            {
                                "mRender": function (data, type, full, row) {
                                    if ($("#example-select-all").prop("checked") == true) {
                                        return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]" checked >';

                                    } else {
                                        return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]"  >';
                                    }

                                },
                                "aTargets": [0]
                            },
                            //{
                            //    "mRender": function (data, type, full, row) {
                            //        if (full.ShortComment != "") {
                            //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + ' checked " >';
                            //        } else {
                            //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + '" >';
                            //        }
                            //    },
                            //    "aTargets": [13]
                            //},
                        ],
                        //"fnRowCallback": function (nRow, data, iDisplayIndex, iDisplayIndexFull) {

                        //    if (data.IsDuplicate == true) {
                        //        $('td', nRow).css('background-color', 'yellow');
                        //    }

                        //}
                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
                waitingDialog.hide();

                if ((msg.length == 0)) {
                    var $toast = toastr["error"]('No data found for the current search.', '');
                }
            }
            else {
                var $toast = toastr["error"](ErrorMessage, '');
            }






        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
    // waitingDialog.hide();
}

$('#btnReopen').click(function () {


    var uploadIdV = $("#upLoadId").val();
    var FormIDV = $("#formId").val();
    var reopenid = $("#ddlReasonMaster").val();
    var reopenComm = $("#txtReopenReason").val();

    var UrlReopen = $("#urlReopenForm").val();
    var InstallerId = $("#ddlReopenInstaller option:selected").val();
    //var strstartDate = $('#txtFromDate').val();
    //var strendDate = $('#txtToDate').val();

    $.ajax({
        type: 'GET',
        url: UrlReopen,
        data: {
            "uploadId": uploadIdV,
            "installerId": InstallerId,
            "formId": FormIDV,
            "reOpenId": reopenid,
            "reOpenComment": reopenComm
            //"strfromDate": strstartDate,
            //"strtoDate": strendDate
        },
        async: false,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {

            if (data.success == true) {
                $("#myModal1").modal('hide');
                var $toast = toastr["success"]('Record reopened successfully.', '');
            }
            else {
                //var $toast = toastr["error"]('No data found for the current search.', '');
            }
            _searchClick();

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
});

$('#btnReopenCancel').click(function () {
    $("#myModal1").modal('hide');
    $("#upLoadId").val('');
    $("#formId").val('');
    $("#txtReopenReason").val('');

});
$('#btnClose').click(function () {
    $("#ModelSKIP_RTU").modal('hide');
    // $("#upLoadId").val('');
    //   $("#formId").val('');
    // $("#txtReopenReason").val('');

});

function Reopen(UpId, instId, formId) {

    $("#upLoadId").val('');
    $("#formId").val('');
    //$("#txtFromDate").val('');
    //$("#txtToDate").val('');
    $("#txtReopenReason").val('');



    //Reopen(' + full.ID + ', ' + full.InstallerId + ', ' + full.FormId + ');
    var Project = $("#ddlProject option:selected")
    var urlGetReopenData = $('#urlGetReopenData').val();
    $.ajax({
        type: 'GET',
        url: urlGetReopenData,
        data: { projectId: Project.val() },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {

            if (data.success === undefined) {

                $("#ddlReasonMaster").empty();
                $('#ddlReopenInstaller').empty();
                var count = data.InstallerList.length;
                if (count > 0) {

                    $.each(data.InstallerList, function (index, item) {
                        if (index == 0) {
                            $('#ddlReopenInstaller').append($('<option></option>').val('0').html(""));
                        }
                        $('#ddlReopenInstaller').append($('<option></option>').val(item.UserId).html(item.UserName));
                    });
                    $('#ddlReopenInstaller').val(instId).attr("selected", "selected");
                    $('#ddlReopenInstaller').trigger('chosen:updated');
                }

                var count1 = data.ReopenList.length;
                if (count1 > 0) {

                    $.each(data.ReopenList, function (index, item) {
                        if (index == 0) {
                            $('#ddlReasonMaster').append($('<option></option>').val('0').html(""));
                        }
                        $('#ddlReasonMaster').append($('<option></option>').val(item.SkipId).html(item.SkipReason));
                    });
                    $('#ddlReasonMaster').trigger('chosen:updated');
                }


                $("#upLoadId").val(UpId);
                $("#formId").val(formId);

            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
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


    $("#myModal1").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

function PDFRecords(data) {



    var urlstoredPDFIDs = $('#urlstoredPDFIDs').val();

    var urlGeneratePDFbyID = $('#urlGeneratePDFbyID').val();
    $.ajax({
        type: 'GET',
        data: { id: data.id, frmID: data.name, buttonclicked: null },
        url: urlstoredPDFIDs,
        success: function (data1) {
            if (data1.success === undefined) {
                window.open(
                    urlGeneratePDFbyID,
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });



}

var formId;
function bindTable() {
    debugger;

    // waitingDialog.show('Loading Please Wait...');
    var urlActiveRecords = $("#UrlReportsList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'POST',
        success: function (msg) {

            oTable = $('#dynamic-table').dataTable(
                {
                    "bFilter": true,
                    "bLengthChange": true,
                    "bDestroy": true,
                    "bserverSide": true,
                    'responsive': true,
                    "bDeferRender": true,
                    //"sAjaxDataProp": 'aData',
                    "oLanguage": {
                        "sLengthMenu": '_MENU_ Records per page'
                    },

                    //"sAjaxSource": msg.result,

                    "aaSorting": [],
                    "iDisplayLength": -1,
                    "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    //  "sDom": 'ltipr',

                    "aaData": msg,
                    "aoColumns": [
                        //{ "bSearchable": false, "mData": null },
                        //{ "mData": "Dates" },
                        //{ "mData": "Account" },
                        //{ "mData": "FormName" },
                        //{ "mData": "Cycle" },
                        //{ "mData": "Route" },
                        //{ "mData": "Street" },
                        //{ "mData": "FirstName" },
                        //{ "mData": "CategoryType" },
                        //{ "mData": "SkipReason" },
                        //{ "mData": "ShortComment" },
                        //{ "bSearchable": false, "mData": null },

                        { "bSearchable": false, "mData": null },
                        { "mData": "Dates" },
                        { "mData": "Account" },
                        { "mData": "Cycle" },
                        { "mData": "Route" },
                        { "mData": "Street" },
                        //{ "mData": "FormName" },
                        { "mData": "OldMeterNo" },
                        { "mData": "OldMeterSize" },
                        //{ "mData": "OLDMETERFINALREADING" },
                        { "mData": "NewMeterNo" },
                        { "mData": "NewMeterSize" },
                        { "mData": "NEWRADIONUMBER" },
                        { "mData": "FirstName" },
                        { "mData": "CategoryType" },
                        //{ "mData": "SkipReason" },
                        //{ "mData": "ShortComment" },
                        { "bSearchable": false, "mData": null },
                    ],

                    "aoColumnDefs": [
                        {
                            "mRender": function (data, type, full, row) {
                                var timeInMillis = Date.parse(full.Dates);

                                if (full.ProjectStatus == "Active") {
                                    if (full.CategoryType == "COMPLETED") {
                                        return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                            // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                            + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                            '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                    } else {
                                        if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {
                                            return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                                //Export_SKipRTU Added by AniketJ on 1-sep-2016
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'


                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',' + full.CategoryType + ');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>';
                                        } else {
                                            return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'

                                        }
                                    }
                                }
                                else {
                                    if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {

                                        return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'

                                    } else {
                                        //return
                                        //'<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                        if (full.CategoryType == "COMPLETED") {
                                            return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'

                                        } else {
                                            return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'
                                        }


                                    }

                                }
                            },
                            "aTargets": [13]
                        },
                        {
                            "mRender": function (data, type, full, row) {
                                if ($("#example-select-all").prop("checked") == true) {
                                    return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]" checked >';

                                } else {
                                    return '<input type="checkbox" id=' + full.ID + ' value=' + full.FormId + ' name="id[]"  >';
                                }

                            },
                            "aTargets": [0]
                        },
                        //{
                        //    "mRender": function (data, type, full, row) {
                        //        if (full.ShortComment != "") {
                        //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + ' checked " >';
                        //        } else {
                        //            return '<input type="checkbox" id=' + full.ID + ' disabled="disabled"  value=' + full.FormId + '" >';
                        //        }
                        //    },
                        //    "aTargets": [13]
                        //},
                    ],
                    "fnRowCallback": function (nRow, data, iDisplayIndex, iDisplayIndexFull) {

                        if (data.IsDuplicate == true) {
                            $('td', nRow).css('background-color', 'yellow');
                        }
                        //else {
                        //    $('td', nRow).css('background-color', 'Green');
                        //}
                    }
                }
            ), $('.dataTables_filter input').attr('maxlength', 50);

            waitingDialog.hide();

        }
        ,
        error: function () {
            waitingDialog.hide();
            alert("something seems wrong");
        }
    });

}

jQuery(function ($) {
    $('a[href$=".pdf"]').attr('target', '_blank');
});

function GetSKipRTU_Details(AuditorId, ProjectId, ID, FormId) {

    //  $("#activationID").val(ID);
    waitingDialog.show('Loading record please wait...');

    var urlGetData = $('#urlGetSkipEditData').val();

    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { _ID: ID, _InstallerId: AuditorId, _FormID: FormId },
        success: function (msg) {

            waitingDialog.hide();
            if (data.success === undefined) {

                $("#divImage").hide();
                $("#divAudio").hide();
                $("#divVideo").hide();
                $(".btnNextPreviousSkipRTU").hide();
                if (msg.SkipRtuAttachmentModel != undefined && msg.SkipRtuAttachmentModel != null && msg.SkipRtuAttachmentModel.length > 0) {
                    $("#divImage").children("img").remove();
                    $.each(msg.SkipRtuAttachmentModel, function (index, item) {
                        if (item.IsAudioVideoImage == "3") {
                            $("#imagenotfound").text("");
                            $("#divImage").show();

                            var imagetag = $("#divImage")

                            var imageElement = $('<img/>', {
                                src: item.strFilePath,
                                class: "mySlides",
                                style: "width: 29em;height: 25em;"
                            }).appendTo('#divImage');
                        }
                        else if (item.IsAudioVideoImage == "1") {
                            $("#audionotfound").text("");
                            $("#divAudio").show();
                            var audiotag = $("#srcAudio")
                            audiotag.attr("src", item.strFilePath)
                            $("#aT").load();
                        }
                        else if (item.IsAudioVideoImage == "2") {
                            $("#videonotfound").text("");
                            $("#divVideo").show();
                            var videotag = $("#srcVideo");
                            videotag.attr("src", item.strFilePath)
                            $("#vT").load();
                        }
                    })
                    var imagecount = $(".mySlides");
                    if (imagecount.length > 1) {
                        $(".btnNextPreviousSkipRTU").show();
                    }
                }

                showDivs(1);
                $("#ModelSKIP_RTU").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });
                if (msg.CategoryType == "SKIP") {
                    $("#Status").text("SKIP Details");
                } else {
                    $("#Status").text("RTU Details");
                }

                $("#txtAccount").text(msg.Account);
                $("#txtAddress").text(msg.Street);

                if (msg.FormName != null) {
                    $("#txtFromName").text(msg.FormName);
                }
                else {
                    $("#txtFromName").text('-');
                }

                $("#txtCycle").text(msg.Cycle);
                $("#txtRoute").text(msg.Route);
                if (msg.FirstName != null) {
                    $("#txtInstaller").text(msg.FirstName);
                }
                else {
                    $("#txtInstaller").text('-');
                }
                if (msg.CategoryType != null) {
                    $("#txtStatus").text(msg.CategoryType);
                }
                else {
                    $("#txtStatus").text('-');
                }

                if (msg.Dates != null) {
                    $("#txtVisitDate").text(msg.Dates);
                }
                else {
                    $("#txtVisitDate").text('-');
                }

                if (msg.SkipReason != null) {
                    $("#txtReason").text(msg.SkipReason);
                }
                else {
                    $("#txtReason").text('-');
                }

                if (msg.SkipComment != null) {
                    $("#txtComment").text(msg.SkipComment);
                }
                else {
                    $("#txtComment").text('-');
                }
                if (msg.skipGPSLocation != null && msg.skipGPSLocation != "") {
                    var center1 = msg.skipGPSLocation;
                    var url = "https://maps.google.com/maps?q=" + center1
                    $("#txtlocationGPS").attr("href", url);

                    $("#spnLocationGPS").text(center1)
                    $("#divLocationGPS").show();
                }
                else {
                    $("#divLocationGPS").hide();

                }
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }


        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }

    });


}
function EditRecords(userId, projectId, accinifiledId, reportFId) {

    waitingDialog.show('Please Wait...');
    var urlGetData = $('#urlEditRecords').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        //async: false,
        data: { userId1: userId, projectId1: projectId, accountinifiledId: accinifiledId, reportFormId: reportFId },
        success: function (data) {
            waitingDialog.hide();

            if (data.success != undefined && data.success === true) {

                var createurl = $("#urlRedirectCreate").val();
                //location.href = createurl + "?userId1=" + userId + "&projectId1=" + projectId + "&accountinifiledId=" + accinifiledId + "&reportFormId=" + reportFId;
                window.open(createurl, '_blank');
            }
            else {

                var $toast = toastr["error"](data.returnMessage, "Error Notification");
            }

        }
    });
}

var reportFormId
$(document).on("click", '#ReportName input', function () {
    reportFormId = this.id;
});

function Display(Id) {

    //spinner.spin(target);
    //  waitingDialog.show('Please Wait...');
    $("#FieldImage").empty();
    $('#FieldData').html('');
    var urlGetData = $('#urlDisplay').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { id: Id },
        success: function (msg) {
            if (msg.Field.length > 0) {

                $("#FieldData").append('<div style="width:100%"><table style="width:100%"><br/>');
                $.each(msg.Field, function (index, item) {

                    //var mm = $('#'+item.FieldId+'').val();
                    if (item.FielddatatypeNames == "Text") {
                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><input id="' + item.FieldId + '" name = "' + item.FieldName + '" type="text" style="width: 120px" value = "' + item.FieldText + '" onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '" /><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        // $("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('minlength', item.MinLengthValue);
                            $("#" + item.FieldId + "").attr('maxlength', item.MaxLengthValue);
                        }
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }
                    else if (item.FielddatatypeNames == "Numeric") {

                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><input id="' + item.FieldId + '" name = "' + item.FieldName + '" type="Number" style="width: 120px"s value = "' + item.FieldText + '" placeholder="' + item.FieldLabel + '"  /><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        // $("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('max', item.MaxLengthValue);
                            $("#" + item.FieldId + "").attr('min', item.MinLengthValue);

                        }
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }
                    else if (item.FielddatatypeNames == "Date/Time") {
                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><input id="' + item.FieldId + '" name = "' + item.FieldName + '" type="Date" style="width: 150px; height:20px" value = " " onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '" /><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        //$("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('minlength', item.MinLengthValue);
                            $("#" + item.FieldId + "").attr('maxlength', item.MaxLengthValue);
                        }
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }
                    else if (item.FielddatatypeNames == "Long Text") {
                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><textarea style="padding-left:100px" id="' + item.FieldId + '" name = "' + item.FieldName + '" type="text"   value ="' + item.FieldText + '" onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '" /><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        //$("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('minlength', item.MinLengthValue);
                            $("#" + item.FieldId + "").attr('maxlength', item.MaxLengthValue);
                        }
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }
                    else if (item.FielddatatypeNames == "Static text") {
                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><input id="' + item.FieldId + '" name = "' + item.FieldName + '" type="text" style="width: 120px" value ="' + item.FieldText + '" onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '" /><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        //$("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('minlength', item.MinLengthValue);
                            $("#" + item.FieldId + "").attr('maxlength', item.MaxLengthValue);
                        }
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }
                    else if (item.FielddatatypeNames == "List") {
                        $(item).attr("href", item);
                        $("#FieldData").append('<tr><td style="text-align:right">' + item.FieldName + '</td><td style="text-align:left"><select id="' + item.FieldId + '" name = "' + item.FieldName + '" type="Date" style="width: 120px" value = " " onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '"/><br/><label id="' + item.FieldId + 'l" class="col-lg-1" style="color: red"></label></td></tr>');
                        // $("#" + item.FieldId + "").prop("disabled", item.IsEnabled);
                        $("#" + item.FieldId + "").prop("required", true);
                        $("#" + item.FieldId + "").attr('title', '' + item.HintText + '');
                        var i = item.EnforceMinMax;
                        if (i == "True") {
                            $("#" + item.FieldId + "").attr('minlength', item.MinLengthValue);
                            $("#" + item.FieldId + "").attr('maxlength', item.MaxLengthValue);
                        }

                        ListData(item.FieldId);
                        $("#FieldImage").empty();
                        DisplayImages(item.initialId, item.FieldName)
                    }

                });
                $("#FieldData").append('</table></div>');
            }

            $("h4.modal-title").text("Edit Report");
            $("#myModal").modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

    });
}

function ListData(FieldId) {

    var urlGetData = $('#urlgetlistitem').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { id: FieldId },
        success: function (msg) {
            $.each(msg.FieldDllMaster, function (index, item) {

                $("#" + FieldId + "").append($("<option></option>").val(item.Fieldvalue).html(item.FieldText));
            });
        }
    });
}

function DisplayImages(initialId, FieldName) {

    var urlGetData = $('#urlGetFieldImages').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { initialId: initialId, FieldName: FieldName },
        success: function (msg) {
            $.each(msg.FieldDataAttach, function (index, item) {

                var imagepath = item.strFilePath.replace('~', '..');
                $("#FieldImage").append('<img src="' + imagepath + '" height="120" width="120" style="margin-left: 2%;">');
            });
        }
    });
}


function CheckIt(FieldId, event) {

    var urlGetData = $('#urlDisplay1').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { id: FieldId },
        success: function (msg) {
            if (msg.Fieldproperty.length > 0) {

                $.each(msg.Fieldproperty, function (index, item) {

                    if (item.FieldDatatypeName == "Text") {
                        var i = item.IsRequired;
                        if (i == true) {
                            $('#nameLabel').text('<br/><span class="error"> Requried</span>');
                            $('#nameLabel').removeClass('error');
                        }
                    }
                    else if (item.FieldDatatypeName == "Numeric") {
                        var i = item.MinLengthValue;
                        var msg = $("#" + FieldId + "").val().length;
                        var specialKeys = new Array();
                        specialKeys.push(8);
                        var keyCode = event.which ? event.which : event.keyCode
                        var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
                        document.getElementById("#" + FieldId + "l").text = ret ? "none" : "inline";
                        return ret;


                        //var charCode = (event.which) ? event.which : event.keyCode;

                        //if (charCode == 8 || charCode == 13 || charCode == 99 || charCode == 118 || charCode == 46) {
                        //    $("#" + FieldId + "l").text('error');
                        //    return true;
                        //}
                        //if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                        //    //return false;
                        //    $("#" + FieldId + "l").text('error');
                        //    $("#" + FieldId + "l").text('Only Numbers');
                        //}
                        //else {
                        //    if (msg <= i) {

                        //        $("#" + FieldId + "l").text('error');
                        //        $("#" + FieldId + "l").text('MinLenght' + i);
                        //    }
                        //    else {
                        //        $("#" + FieldId + "l").text('');
                        //    }
                        //}
                    }
                    else {
                        alert("select formate in not correct")

                    }
                });
            }
        }

    });
}


function validateForm(IsRequired) {

    var nameReg = /^[A-Za-z]+$/;
    var numberReg = /^[0-9]+$/;
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var msg = $('#t1').val();
    if (msg == null) {
        if (IsRequired == true) {
            $('#nameLabel').after('<br/><span class="error"> Requried</span>');
            return false
        }
        return true;
    }
}

//$("#Uncheckall2").click(function () {
//   // a button with Check All as its value
//    $(':checkbox').prop('checked', false); // all checkboxes, you can narrow with a better selector
//});

//#region New code for ProjectStatus multiselect dropdown
function selectProjectStatus() {

    var selectedlivalue = $('#ulProjectStatus li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#ProjectStatusHeader").removeClass("ddlHeader").text("Select Project Status")
    } else {
        $("#ProjectStatusHeader").addClass("ddlHeader").text(selectedlilength + " Project Status selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulProjectStatus .liProjectStatus:visible').length > 0 && $('#ulProjectStatus .highlighted:visible').length == $('#ulProjectStatus .liProjectStatus:visible').length) {
        $('#ProjectStatus_SelectAll').prop('checked', true);
    } else {
        $('#ProjectStatus_SelectAll').prop('checked', false);
    }
    //#endregion

    ResetProject()
    GetProjectListbyProjectStatus()
}

function selectStatus() {

    var selectedlivalue = $('#ulStatus li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#StatusHeader").removeClass("ddlHeader").text("Select visit Status")
    } else {
        $("#StatusHeader").addClass("ddlHeader").text(selectedlilength + " visit Status selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulStatus .liStatus:visible').length > 0 && $('#ulStatus .highlighted:visible').length == $('#ulStatus .liStatus:visible').length) {
        $('#Status_SelectAll').prop('checked', true);
    } else {
        $('#Status_SelectAll').prop('checked', false);
    }
    //#endregion

    //  ResetProject()
    // GetProjectListbyProjectStatus()
}

//#region open/close Cycle dropdown
$("#ancProjectStatus_chosen").click(function () {
    $("#ddlProjectStatus_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divProjectStatus').is(e.target)
        && $('#divProjectStatus').has(e.target).length === 0
    ) {
        $('#ddlProjectStatus_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});
//#endregion

//#region open/close visit status dropdown
$("#ancStatus_chosen").click(function () {
    $("#ddlStatus_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divStatus').is(e.target)
        && $('#divStatus').has(e.target).length === 0
    ) {
        $('#ddlStatus_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});

//#endregion

function filterProjectStatus(element) {

    var value = $(element).val().toLowerCase();

    $("#ulProjectStatus > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulProjectStatus .liProjectStatus:visible').length > 0 && $('#ulProjectStatus .highlighted:visible').length == $('#ulProjectStatus .liProjectStatus:visible').length) {
        $('#ProjectStatus_SelectAll').prop('checked', true);
    } else {
        $('#ProjectStatus_SelectAll').prop('checked', false);
    }
    //#endregion
}

function filtervisitStatus(element) {

    var value = $(element).val().toLowerCase();

    $("#ulStatus > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulStatus .liStatus:visible').length > 0 && $('#ulStatus .highlighted:visible').length == $('#ulStatus .liProjectStatus:visible').length) {
        $('#Status_SelectAll').prop('checked', true);
    } else {
        $('#Status_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#ProjectStatus_SelectAll').on('click', function () {
    if ($('#ulProjectStatus .liProjectStatus').length > 0) {
        if (this.checked) {
            $("li.liProjectStatus:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulProjectStatus .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#ProjectStatusHeader").removeClass("ddlHeader").text("Select Project Status")
            } else {
                $("#ProjectStatusHeader").addClass("ddlHeader").text(selectedlilength + " Project Status selected")
            }

        }
        else {
            $("li.liProjectStatus:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulProjectStatus .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#ProjectStatusHeader").removeClass("ddlHeader").text("Select Project Status")
            } else {
                $("#ProjectStatusHeader").addClass("ddlHeader").text(selectedlilength + " Project Status selected")
            }
        }

        // checkinstallerroleselect()
    }

    selectProjectStatus()
});


$('#Status_SelectAll').on('click', function () {
    if ($('#ulStatus .liStatus').length > 0) {
        if (this.checked) {
            $("li.liStatus:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulStatus .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#StatusHeader").removeClass("ddlHeader").text("Select visit Status")
            } else {
                $("#StatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
            }

        }
        else {
            $("li.liStatus:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulStatus .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#StatusHeader").removeClass("ddlHeader").text("Select Visit Status")
            } else {
                $("#StatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
            }
        }

        // checkinstallerroleselect()
    }

    //  selectProjectStatus()

    $("#btnExport").visible = true;
});
//#endregion

//#region New code for Installer multiselect dropdown
function selectInstaller() {

    var selectedlivalue = $('#ulInstaller li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#InstallerHeader").removeClass("ddlHeader").text("Select Auditor")
    } else {
        $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulInstaller .liInstaller:visible').length > 0 && $('#ulInstaller .highlighted:visible').length == $('#ulInstaller .liInstaller:visible').length) {
        $('#Installer_SelectAll').prop('checked', true);
    } else {
        $('#Installer_SelectAll').prop('checked', false);
    }
    //#endregion

}


$("#ancInstaller_chosen").click(function () {
    $("#ddlInstaller_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divInstaller').is(e.target)
        && $('#divInstaller').has(e.target).length === 0
    ) {
        $('#ddlInstaller_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});



function filterInstaller(element) {

    var value = $(element).val().toLowerCase();

    $("#ulInstaller > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulInstaller .liInstaller:visible').length > 0 && $('#ulInstaller .highlighted:visible').length == $('#ulInstaller .liInstaller:visible').length) {
        $('#Installer_SelectAll').prop('checked', true);
    } else {
        $('#Installer_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Installer_SelectAll').on('click', function () {
    if ($('#ulInstaller .liInstaller').length > 0) {

        if (this.checked) {
            $("li.liInstaller:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulInstaller .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#InstallerHeader").removeClass("ddlHeader").text("Select Auditor")
            } else {
                $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
            }

        }
        else {
            $("li.liInstaller:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulInstaller .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#InstallerHeader").removeClass("ddlHeader").text("Select Auditor")
            } else {
                $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Auditor selected")
            }
        }


    }
});




function selectCycle() {

    var selectedlivalue = $('#ulCycle li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    } else {
        $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#endregion

    ResetRoute()
    debugger;
    getRouteListbyCycle()
}


$("#ancCycle_chosen").click(function () {
    $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divCycle').is(e.target)
        && $('#divCycle').has(e.target).length === 0
    ) {
        $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});


function filterCycle(element) {

    var value = $(element).val().toLowerCase();

    $("#ulCycle > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Cycle_SelectAll').on('click', function () {
    if ($('#ulCycle .liCycle').length > 0) {

        if (this.checked) {
            $("li.liCycle:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }

        }
        else {
            $("li.liCycle:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }
        }


    }

    selectCycle()
});


function getRouteListbyCycle() {

    debugger;
    var ProjectID = $('#ddlProject').val();

    var cyclevalue = [];
    //var value = $('#ulCycle li.highlighted')
    //value.each(function (index, item) {
    //    cyclevalue.push($(this).val())
    //});
    var cyclevalue = [];
    var value = $('#ulCycle li.highlighted')
    value.each(function (index, item) {
        cyclevalue.push($(this).text())
    });

    if (cyclevalue != null && cyclevalue.length > 0) {

        //GetRootByCycle(value[i].text);

        var urlGetRouteListByCycle = $('#urlGetRouteListByCycle').val();

        $.ajax({
            url: urlGetRouteListByCycle,
            type: 'POST',
            async: true,
            data: { ProjectId: ProjectID, Cycle: cyclevalue },
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {

                $('#ulRoute').empty();

                $.each(data.Route, function (index, item) {
                    $('#ulRoute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item));
                });

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
            }
        });



    }

};

function selectRoute() {

    var selectedlivalue = $('#ulRoute li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#RouteHeader").removeClass("ddlHeader").text("Select Route")
    } else {
        $("#RouteHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulRoute .liRoute:visible').length > 0 && $('#ulRoute .highlighted:visible').length == $('#ulRoute .liRoute:visible').length) {
        $('#Route_SelectAll').prop('checked', true);
    } else {
        $('#Route_SelectAll').prop('checked', false);
    }
    //#endregion

    ResetAddress()
    getAddressbyRoute();
}


$("#ancRoute_chosen").click(function () {
    $("#ddlRoute_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divRoute').is(e.target)
        && $('#divRoute').has(e.target).length === 0
    ) {
        $('#ddlRoute_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});



function filterRoute(element) {

    var value = $(element).val().toLowerCase();

    $("#ulRoute > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulRoute .liRoute:visible').length > 0 && $('#ulRoute .highlighted:visible').length == $('#ulRoute .liRoute:visible').length) {
        $('#Route_SelectAll').prop('checked', true);
    } else {
        $('#Route_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Route_SelectAll').on('click', function () {
    if ($('#ulRoute .liRoute').length > 0) {

        if (this.checked) {
            $("li.liRoute:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulRoute .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#RouteHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#RouteHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }

        }
        else {
            $("li.liRoute:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulRoute .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#RouteHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#RouteHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }
        }


    }
    selectRoute()
});


function getAddressbyRoute() {

    var projectId = $('#ddlProject').val();

    var ddlRoutevalues = [];
    var Routevalues = $('#ulRoute li.highlighted')
    Routevalues.each(function (index, item) {
        ddlRoutevalues.push($(this).val())
    });


    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) {
        // ddlCyclevalues.push($(this).val())
        ddlCyclevalues.push($(this).text())
    });


    var fromDate = $('#datepicker1').val();
    var Todate = $('#datepicker2').val();

    var urlGetAddressUrl = $('#urlGetAddressUrl').val();

    $('#ulAddress').empty();

    if (ddlRoutevalues != null && ddlRoutevalues.length > 0) {

        $.ajax({
            url: urlGetAddressUrl,
            async: true,
            type: "POST",
            dataType: "json",
            data: { Route: ddlRoutevalues, Cycle: ddlCyclevalues, ProjectID: projectId, fromDate: fromDate, toDate: Todate },
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (jsonResult) {

                $('#ulAddress').empty();
                $.each(jsonResult.AllAddress, function (index, item) {
                    $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street));
                });

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
            }
        });

    }
};
function selectAddress() {

    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#AddressHeader").removeClass("ddlHeader").text("Select Address")
    } else {
        $("#AddressHeader").addClass("ddlHeader").text(selectedlilength + " Address selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
        $('#Address_SelectAll').prop('checked', true);
    } else {
        $('#Address_SelectAll').prop('checked', false);
    }
    //#endregion

}

$("#ancAddress_chosen").click(function () {

    $("#ddlAddress_chosen").toggleClass("chosen-with-drop chosen-container-active")
});

$('body').on('click', function (e) {
    if (!$('#divAddress').is(e.target)
        && $('#divAddress').has(e.target).length === 0
    ) {
        $('#ddlAddress_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});



function filterAddress(element) {

    var value = $(element).val().toLowerCase();

    $("#ulAddress > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
        $('#Address_SelectAll').prop('checked', true);
    } else {
        $('#Address_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Address_SelectAll').on('click', function () {
    if ($('#ulAddress .liAddress').length > 0) {

        if (this.checked) {
            $("li.liAddress:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulAddress .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#AddressHeader").removeClass("ddlHeader").text("Select Address")
            } else {
                $("#AddressHeader").addClass("ddlHeader").text(selectedlilength + " Address selected")
            }

        }
        else {
            $("li.liAddress:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulAddress .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#AddressHeader").removeClass("ddlHeader").text("Select Address")
            } else {
                $("#AddressHeader").addClass("ddlHeader").text(selectedlilength + " Address selected")
            }
        }


    }
    selectAddress
});



// Handle click on "Select all" control
$('#example-select-all').on('click', function () {

    $('input[name="id[]"]').prop('checked', this.checked);
});

// Handle click on checkbox to set state of "Select all" control
$('#dynamic-table').on('click', 'input[name="id[]"]', function () {
    // If checkbox is not checked


    if (!this.checked) {
        var el = $('#example-select-all').get(0);
        // If "Select all" control is checked and has 'indeterminate' property
        if (el && el.checked && ('indeterminate' in el)) {
            // Set visual state of "Select all" control 
            // as 'indeterminate'
            el.indeterminate = true;
        }
    }
});

// Handle form submission event
$('#btnExport').on('click', function () {


    var ReportRowList = [];
    var SelectedRows = $('input[name="id[]"]:checked')
    SelectedRows.each(function (index, item) {

        var ReportRow = new Object();
        ReportRow.UploadedID = this.id;
        ReportRow.FormID = this.value;
        ReportRowList.push(ReportRow)
    });
    if (ReportRowList.length == 0) {

        var $toast = toastr["error"]("Please select records to export", "Notification");
        return false;
    }

    var urlExportReportMultipleIDs = $('#urlExportReportMultipleIDs').val();
    var urlGeneratePDFmultipleRows = $('#urlGeneratePDFmultipleRows').val();

    $.ajax({
        type: 'POST',
        url: urlExportReportMultipleIDs,
        data: { ReportRowList: ReportRowList },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            console.log("In success");
            if (msg.Status == true) {
                window.open(
                    urlGeneratePDFmultipleRows,
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
            else {
                var $toast = toastr["error"]("Report will not genrated as no record has complete status.", "Notification");
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
});




function Export_SkipRTU(data) {
    var urlGetSkipRTUID = $('#urlGetSkipRTUID').val();
    var urlDownloadFileSkipRTU = $('#urlDownloadFileSkipRTU').val();

    $.ajax({
        type: 'GET',
        data: { id: data.id, formID: data.name, buttonclicked: null },
        url: urlGetSkipRTUID,
        success: function (data1) {

            window.open(
                urlDownloadFileSkipRTU,
                '_blank' // <- This is what makes it open in a new window.
            );
            //location.href = urlGeneratePDFbyID;
        },
        error: function () {
            alert("something seems wrong");
        }
    });



}
//To export skip and rtu records