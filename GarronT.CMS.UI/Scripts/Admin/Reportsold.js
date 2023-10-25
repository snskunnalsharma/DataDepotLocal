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

    $("#modalAuditorAllocation").modal('hide');
    $('#dynamic-table').DataTable({
        //"sDom": '<"dt_head"fp>t<"F"l>'
        //"sDom": "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
        //    "<'row'<'col-sm-12 col-md-6'i><'col-sm-12 col-md-6'p>>" +
        //    "<'row'<'col-sm-12'tr>>" +
        //    "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        //"sDom": '<"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 drp-rec"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"l><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"f>><"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 rep-shw-ent"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"i><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"p>>rt<ip><"clear">',
        "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
    });

    $("#Reports").addClass('active');
    $("#subReports").addClass('block');

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liReport');
    listate.addClass("active");

    //#endregion

    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlReopenInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlReasonMaster").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlAuditor").chosen({ no_results_text: "Oops, nothing found!" });

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
    document.getElementById("quickSearch").disabled = true;
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
        { ID: "RTU", ProjectStatus: "RTU" },
        { ID: "PENDING", ProjectStatus: "PENDING" },
        { ID: "NOT ALLOCATED", ProjectStatus: "NOT ALLOCATED" },
        { ID: "UNALLOCATED SKIP", ProjectStatus: "UNALLOCATED SKIP" },
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

$('#ddlProjectStatus_chosen li.highlighted').change(function () {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();
});
$('#ulProjectStatus li.highlighted').change(function () {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();
});
$('#ddlProject').change(function () {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();
    document.getElementById("quickSearch").disabled = false;
    var projectID = $("#ddlProject option:selected").val();
    ResetInstaller()
    if (projectID != "0" && projectID != undefined) {

        $("#datepicker").datepicker("destroy");
        $("#datepicker1").datepicker("destroy");

        if (_ProjectList != undefined && _ProjectList != null) {
            $.each(_ProjectList, function (index, item) {
                if (item.ProjectId == projectID) {
                    var todaysDate = new Date();
                    prjFromDate = new Date(item.stringFromDate);
                    prjToDate = new Date(item.stringToDate);
                    //alert(item.stringFromDate + " " + item.stringToDate)

                    $("#datepicker1").datepicker({
                        changeMonth: true,
                        changeYear: true,
                        //minDate: prjFromDate,
                        //maxDate: prjToDate,
                        onSelect: function () {

                            var date = $("#datepicker1").val();
                            if ($.trim(date) != "") {
                                var newDate1 = new Date(date);
                                $("#datepicker2").datepicker('option', 'minDate', newDate1);
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

                            var date = $("#datepicker2").val();
                            if ($.trim(date) != "") {
                                var newDate1 = new Date(date);
                                $("#datepicker1").datepicker('option', 'minDate', newDate1);
                            }


                            // validator.resetForm();

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

        GetInstallerByProjectID(projectID)
        GetCycleListbyProjectID(projectID)

    }

});

function GetInstallerByProjectID(projectID) {
    var urlGetInstallerList = $('#urlGetInstallerList').val();

    var i = 0;
    $.ajax({
        url: urlGetInstallerList,
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

/*var table = $('#dynamic-table').DataTable({
    colReorder: true
});
 
$('#btnReset').click(function () {
    table.colReorder.ResetAllData();
});*/
//$('#btnReset').dataTables().fnDraw(false)

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
    var ProjectIdssss = $('#ddlProject').val();
    ResetAllData();
    
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();
   

})


function ResetProject() {

    var table = $('#dynamic-table').DataTable();
    table.clear().draw();

    $('#ddlProject').empty();
    $('.chosen-select').trigger('chosen:updated');

    ResetInstaller()
}

function ResetInstaller() {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();
    $('#ulInstaller').empty();
    $("#InstallerHeader").removeClass("ddlHeader").text("Select Installer")
    $('#txtfilterallInstaller').val("");
    $('#Installer_SelectAll').prop('checked', false);
    ResetCycle()
}

function ResetCycle() {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();

    $('#ulCycle').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $('#txtfilterallCycle').val("");
    $('#Cycle_SelectAll').prop('checked', false);
    ResetRoute()
}

function ResetRoute() {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();

    $('#ulRoute').empty();
    $("#RouteHeader").removeClass("ddlHeader").text("Select Route")
    $('#txtfilterallRoute').val("");
    $('#Route_SelectAll').prop('checked', false);


    ResetAddress()
}

function ResetAddress() {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();

    $('#txtfilterallAddress').val("");
    $('#Address_SelectAll').prop('checked', false);
    $('#ulAddress').empty();
    $("#AddressHeader").removeClass("ddlHeader").text("Select Address")
}

function ResetAllData() {

    GetProjectStatusList()
    GetStatusList()
    ResetProject()
    $('#ulProjectStatus').val("");
    $('.txtfilterall').val("");
    $('#ddlProject').val("");
    $('.chbSelectAll').prop('checked', false);
    $('#datepicker1').datepicker('setDate', null);
    $('#datepicker2').datepicker('setDate', null);

    document.getElementById("quickSearch").disabled = true;
}
//#region Cancel button on Create Model
$("#btnCancel1").click(function () {

    //location.href = $('#UrlDashboard').val()
    $("#myModal").modal('hide');
})
//#endregion


//#region get account number
$('#quickSearch').change(function () {

    if ($(this).is(":checked")) {
        var table = $('#dynamic-table').DataTable();
        table.clear().draw();

        $('#divStatus .chosen-drop').css('display', 'none');
        $('#divInstaller .chosen-drop').css('display', 'none');
        $('#divCycle .chosen-drop').css('display', 'none');
        $('#divRoute .chosen-drop').css('display', 'none');
        $('#divAddress .chosen-drop').css('display', 'none');

        var projectID = $("#ddlProject option:selected").val();
        GetStatusList();
        ResetInstaller();
        GetInstallerByProjectID(projectID);
        GetCycleListbyProjectID(projectID);
        $('.txtfilterall').val("");
        $('.chbSelectAll').prop('checked', false);
        //$('#datepicker1').datepicker('setDate', null);
        //$('#datepicker2').datepicker('setDate', null);
        document.getElementById('datepicker1').readOnly = true;
        document.getElementById('datepicker2').readOnly = true;

        $('#accountNo').val("");
        document.getElementById('accountNo').readOnly = false;
        //document.getElementById('accountNo').removeAttribute('readonly');

    }
    else {

        $('#divStatus .chosen-drop').css('display', 'block');
        $('#divInstaller .chosen-drop').css('display', 'block');
        $('#divCycle .chosen-drop').css('display', 'block');
        $('#divRoute .chosen-drop').css('display', 'block');
        $('#divAddress .chosen-drop').css('display', 'block');

        document.getElementById('datepicker1').readOnly = false;
        document.getElementById('datepicker2').readOnly = false;

        $('#accountNo').val("");
        document.getElementById('accountNo').readOnly = true;
        console.log("sdff");
    }
});
//#endregion


$('#btnSearch').unbind("click").click(function () {
    //alert("fds");
    debugger;
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
                        /*'responsive': true,*/
                        "bDeferRender": true,
                        //"sAjaxDataProp": 'aData',
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },
                        //"sDom": '<"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 drp-rec"fl><"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 rep-shw-ent"ip>rt<ip><"clear">',
                        //"sDom": '<"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 drp-rec"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"l><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"f>><"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 rep-shw-ent"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"i><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"p>>rt<ip><"clear">',
                        "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
                        "aaSorting": [],
                        "iDisplayLength": 10,
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
                            //{ "mData": "FormName" },   
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "Street" },

                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterSize" },
                            { "mData": "OLDMETERFINALREADING" },
                            { "mData": "NewMeterNo" },
                            { "mData": "NewMeterSize" },
                            { "mData": "NEWRADIONUMBER" },


                            { "mData": "FirstName" },
                            { "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            //{ "mData": "IsDuplicate" },
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
                                "aTargets": [14]
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
var accountHistory = "";
function _searchClick() {

    var objProjectStatusList = [];
    var ProjectStatusvalue = $('#ulProjectStatus li.highlighted')
    ProjectStatusvalue.each(function (index, item) { objProjectStatusList.push($(this)[0].id) });

    /*if (objProjectStatusList.length < 1) {
        var $toast = toastr["error"]('Please select Project Status first', 'Error Notification');
        return false;
    }*/

    var objStatusList = [];
    var Statusvalue = $('#ulStatus li.highlighted')
    Statusvalue.each(function (index, item) { objStatusList.push($(this)[0].id) });

   /* if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select project name first', 'Error Notification');
        return false;
    }*/

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

    var objAccountNo = $('#accountNo').val();
    var objCheckBox = document.getElementById("quickSearch");
    if (objCheckBox.checked == true)
    {
        if (objAccountNo.length > 0) {
            console.log("test");
        }
        else {
            var $toast = toastr["error"]('Please Enter Account Number', 'Error Notification');
            return false;
        }
    }
    /*if ($('#ddlProject').val() < 1) {
        var ProjectId = newproj;

        //ProjectId = "10085";
        alert(ProjectId);
        fromDate= "Jun-12-2023";
        Todate= "Jun-27-2023";

  
    }*/
    $.ajax({
        type: 'POST',
        url: urlRoleData,
        data: { ProjectId: ProjectId, InstallerId: InstallerIdList, fromDate: fromDate, Todate: Todate, cycleId: cycleIdList, RouteId: RouteIdList, AdressId: AdressIdList, Status: objStatusList, accountNo: objAccountNo},
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
                        //'responsive': true,
                        "bDeferRender": true,
                        //"sAjaxDataProp": 'aData',
                        //col-lg-3 col-md-4 col-sm-6 col-xs-6 col-xxs-12
                        //"sDom": '<"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 drp-rec"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"l><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"f>><"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 rep-shw-ent"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"i><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"p>>rt<ip><"clear">',
                        /*"sDom": "<'row'<'col-sm-6'l><'col-sm-6'f><'col-sm-12'ip>>" +
                            "<'row'<'col-sm-12'tr>>" +
                            "<'row'<'col-sm-6'i><'col-sm-6'p>>",*/
                        "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },

                        "aaSorting": [],
                        "iDisplayLength": 10,
                        "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                        // "sDom": 'ltipr',


                        "aaData": msg.demo1,
                        "aoColumns": [
                            { "bSearchable": false, "mData": null },
                            { "mData": "Dates" },
                            { "mData": "Account" },
                            //{ "mData": "FormName" },   
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "Street" },

                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterSize" },
                            { "mData": "OLDMETERFINALREADING" },
                            { "mData": "OldMeterRadioNumber" },
                            { "mData": "NewMeterNo" },
                            { "mData": "NewMeterSize" },
                            { "mData": "NEWRADIONUMBER" },


                            { "mData": "FirstName" },
                            { "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            //{ "mData": "IsDuplicate" },
                            { "bSearchable": false, "mData": null },
                            { "bSearchable": false, "mData": null, "sClass": "details-control" },
                        ],
                        "aoColumnDefs": [
                            {
                                "mRender": function (data, type, full, row) {
                                    //console.log("data === " + JSON.stringify(full));
                                    var timeInMillis = Date.parse(full.Dates);

                                    if (full.ProjectStatus == "Active") {
                                        if (full.CategoryType == "COMPLETED") {
                                            

                                            if (full.AuditStaus == 1) {
                                                return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                    // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                                    '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',\'' + full.CategoryType + '\',\'' + full.NewMeterNo + '\',\'' + full.NEWRADIONUMBER + '\');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>' +
                                                    ' <a href="#" id=' + full.ID + ' class="btn btn-default btn-circle" name=' + full.FormId + ' onclick="GetAuditData(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ');"  title="Audit Status - Add To Audit " > <i class="fa fa-font" aria-hidden="true"></i></a>' +
                                                    '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';
                                            }
                                            else if (full.AuditStaus == 2) {
                                                return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                    // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                                    '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',\'' + full.CategoryType + '\',\'' + full.NewMeterNo + '\',\'' + full.NEWRADIONUMBER + '\');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>' +
                                                    ' <a href="#" id=' + full.ID + ' class="btn btn-primary btn-circle" name=' + full.FormId + ' onclick="GetAuditView(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ');"  title="Audit Status - Pending" > <i class="fa fa-font" aria-hidden="true"></i></a>'
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';

                                            }
                                            else if (full.AuditStaus == 3) {
                                                return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                    // '<a href="/admin/Reports/DownloadFile?id=' + full.ID + '&frmID=' + full.FormId + '" title="PDF" target="_blank"><i class="fa fa-file-text"></i></a>';
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                                    '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',\'' + full.CategoryType + '\',\'' + full.NewMeterNo + '\',\'' + full.NEWRADIONUMBER + '\');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>' +
                                                    ' <a href="#" id=' + full.ID + ' class="btn btn-success btn-circle" name=' + full.FormId + ' onclick="GetAuditView(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ');"  title="Audit Status - Completed" > <i class="fa fa-font" aria-hidden="true"></i></a>'
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';
                                            }

                                            else {
                                                return '<a href="#" id=' + full.ID + ' onclick="EditRecords(' + full.InstallerId + ',' + +full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>' +
                                                    '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',\'' + full.CategoryType + '\',\'' + full.NewMeterNo + '\',\'' + full.NEWRADIONUMBER + '\');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>'
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';
                                            }
                                        } else {
                                            if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {
                                                return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                                    //Export_SKipRTU Added by AniketJ on 1-sep-2016
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                                    + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="Reopen(' + full.ID + ',' + full.InstallerId + ',' + full.FormId + ',\'' + full.CategoryType + '\',\'' + full.NewMeterNo + '\',\'' + full.NEWRADIONUMBER + '\');"  title="Reopen" >  <i class="fa fa-retweet"></i></a>'
                                                + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';
                                            } else {
                                                return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'

                                            }
                                        }
                                    }
                                    else {
                                        if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {

                                            return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                            + '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="ViewImages(' + full.ID + ');"  title="View Images" > <i class="fa fa-eye" aria-hidden="true"></i></a>';
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
                                "aTargets": [15]
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
                            {
                                "mRender": function (data, type, full, row) {
                                    var rowDetail = full.ProjectId + '|' + full.ID;
                                    return "<input type='hidden' value='" + rowDetail + "'>";
                                },
                                "aTargets": [16]
                            },
                        ],
                        "fnRowCallback": function (nRow, data, iDisplayIndex, iDisplayIndexFull) {

                            if (data.IsDuplicate == true) {
                                $('td', nRow).css('background-color', 'yellow');
                            }

                        }
                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
                waitingDialog.hide();

                if ((msg.length == 0)) {
                    var $toast = toastr["error"]('No data found for the current search.', '');
                }
            }
            else {
                //var $toast = toastr["error"](ErrorMessage, '');
                waitingDialog.hide();
            }
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
    // waitingDialog.hide();
}

//$('.dataTables_filter input').attr('maxlength', 50);
$('#dynamic-table').on('click', 'td.details-control', function () {
    var tr = $(this).closest('tr');
    var detailId = $(this).closest('tr td:last-child').find('input').val();
    console.log(detailId);
    var row = $("#dynamic-table").DataTable().row(tr);

    var detailIdArray = detailId.split('|');
    var proId = detailIdArray[0];
    var accId = detailIdArray[1];
    
    accountHistory = "";

    if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.removeClass('shown');
        console.log('Close data details');
    } else {
        //hide opened row
        $("#dynamic-table").DataTable().rows().every(function () {
            var tr = $(this.node());
            this.child.hide();
            tr.removeClass('shown');
        });
        // Open this row
        console.log('Open data details');
        showDetail(proId, accId);
        //console.log(accountHistory);
        row.child(accountHistory).show();
        tr.addClass('shown');
    }
});

// Load Account history data
function showDetail(ProjectID, AccountId) {
    //var _projectID = $('#ddlProject').val();
    waitingDialog.show('Loading Please Wait...');
    var urlGetAccountHistory = $("#urlGetAccountHistory").val();
    $.ajax({
        type: 'POST',
        url: urlGetAccountHistory,
        async: false,
        data: { ProjectId: ProjectID, AccountId: AccountId },
        success: function (data) {
            waitingDialog.hide();
            if (data.success == "false") {
                console.log("error from function");
                var $toast = toastr["error"](data.ErrorMessage, "Notification");
            }
            else {
                debugger;
                if (data.objSkipAccountDetailLIst != null) {
                    accountHistory = ShowDataAsChildTable(data.objSkipAccountDetailLIst);
                    //console.log("###### " + accountHistory)
                }
                else {
                    var $toast = toastr["error"]("No Account History Available", "Notification");
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

function ShowDataAsChildTable(varData) {
    //console.log(varData);
    var data = "";
    $.each(varData, function (index, value) {
        data +=
            "<tr>" +
            "<td>" + value.AccountNo + "</td>" +
            "<td>" + value.CustomerAddress + "</td>" +
            "<td>" + value.Cycle + "</td>" +
            "<td>" + value.Route + "</td>" +
            "<td>" + value.InstallerName + "</td>" +
            "<td>" + value.AssignedUser + "</td>" +
            "<td>" + value.ActionDate + "</td>" +
            "<td>" + value.Status + "</td>" +
            "<td>" + value.SkippedReason + "</td>" +
            "<td>" + value.SkipComment + "</td>" +
            "</tr>"
    });
    //console.log(data);
    var tblData = "<table id='dynamic-table-detail-data' class='display table-bordered table-striped dataTable' aria-describedby='dynamic-table_info'>"+
                "<thead style='background-color: darkgrey;'>"+
                    "<tr>"+
                        "<th width='10%'>Account No</th>"+
                        "<th width='13%'>Customer Address</th>"+
                        "<th width='7%'>Cycle</th>" +
                        "<th width='5%'>Route</th>" +
                        "<th width='7%'>Installer Name</th>" +
                        "<th width='5%'>Assigned User</th>" +
                        "<th width='10%'>Action Date</th>" +
                        "<th width='5%'>Status</th>" +
                        "<th width='18%'>Skipped Reason</th>" +
                        "<th width='20%'>Skip Comment</th>" +
                    "</tr>"+
                "</thead>"+
                "<tbody id='viewTable'>"+data+
                
                "</tbody>"+
        "</table>";
    //console.log(tblData);
    return tblData;
    //return $('#dynamic-table-detail-data').prop('outerHTML');
}

$('#btnReopen').click(function () {
    debugger;

    var uploadIdV = $("#upLoadId").val();
    var FormIDV = $("#formId").val();
    var reopenid = $("#ddlReasonMaster").val();
    var reopenComm = $("#txtReopenReason").val();
    var taskStatus = $("#taskStatus").val();
    var meterId = $("#meterId").val();
    var radioId = $("#radioId").val();

    var UrlReopen = $("#urlReopenForm").val();
    var InstallerId = $("#ddlReopenInstaller option:selected").val();
    var oldInstallerId = $('#oldInstallerId').val();
    //var strstartDate = $('#txtFromDate').val();
    //var strendDate = $('#txtToDate').val();
    debugger;
    $.ajax({
        type: 'GET',
        url: UrlReopen,
        data: {
            "uploadId": uploadIdV,
            "installerId": InstallerId,
            "formId": FormIDV,
            "reOpenId": reopenid,
            "reOpenComment": reopenComm,
            "taskStatus": taskStatus,
            "meterId": meterId,
            "radioId": radioId,
            "oldInstallerId": oldInstallerId
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
    debugger;
    $("#upLoadId").val('');
    $("#formId").val('');
    $("#txtReopenReason").val('');

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


                $("#myModal1").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

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

}

function Reopen(UpId, instId, formId, taskStatus, NewMeterNumber, NewMeterRadio) {
    debugger;
    $("#upLoadId").val('');
    $("#formId").val('');
    $("#meterId").val('');
    $("#radioId").val('');
    $("#taskStatus").val('');
    $("#txtReopenReason").val('');
    console.log("serial no === " + NewMeterNumber + ", " + NewMeterRadio + ", " + taskStatus);
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
                $("#meterId").val(NewMeterNumber);
                $("#radioId").val(NewMeterRadio);
                $("#taskStatus").val(taskStatus);
                $("#oldInstallerId").val(instId);
                $("#myModal1").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

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

}


function PDFRecords(data) {
    debugger;
    var showmap = false;
    if ($('#chkShowMap').is(":checked")) {
        showmap = true;
    }
    else {
        showmap = false;
    }
    var urlstoredPDFIDs = $('#urlstoredPDFIDs').val();

    var urlGeneratePDFbyID = $('#urlGeneratePDFbyID').val();
    $.ajax({
        type: 'GET',
        data: { id: data.id, frmID: data.name, buttonclicked: null, showMap: showmap },
        url: urlstoredPDFIDs,
        success: function (data1) {

            if (data1.success === undefined) {
                window.open(
                    urlGeneratePDFbyID,
                    '_blank' // <- This is what makes it open in a new window.
                );
                //location.href = urlGeneratePDFbyID;
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
    //debugger;;

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
                    /*'responsive': true,*/
                    "bDeferRender": true,
                    //"sAjaxDataProp": 'aData',
                    "oLanguage": {
                        "sLengthMenu": '_MENU_ Records per page'
                    },
                    //"sDom": '<"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 drp-rec"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"l><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"f>><"col-lg-12 col-md-12 col-sm-12 col-xs-12 col-xxs-12 rep-shw-ent"<"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"i><"col-lg-6 col-md-6 col-sm-12 col-xs-12 col-xxs-12"p>>rt<ip><"clear">',
                    "sDom": '<"col-lg-12 drp-rec"lf><"col-lg-12 rep-shw-ent"ip>rt<ip><"clear">',
                    //"sAjaxSource": msg.result,

                    "aaSorting": [],
                    "iDisplayLength": 10,
                    "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    //  "sDom": 'ltipr',

                    "aaData": msg,
                    "aoColumns": [

                            { "bSearchable": false, "mData": null },
                            { "mData": "Dates" },
                            { "mData": "Account" },
                            //{ "mData": "FormName" },   
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "Street" },

                            { "mData": "OldMeterNo" },
                            { "mData": "OldMeterSize" },
                            { "mData": "OLDMETERFINALREADING" },
                            { "mData": "NewMeterNo" },
                            { "mData": "NewMeterSize" },
                            { "mData": "NEWRADIONUMBER" },


                            { "mData": "FirstName" },
                            { "mData": "CategoryType" },
                            //{ "mData": "SkipReason" },
                            //{ "mData": "ShortComment" },
                            //{ "mData": "IsDuplicate" },
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
                            "aTargets": [14]
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

function GetSKipRTU_Details(InstallerId, ProjectId, ID, FormId) {

    //  $("#activationID").val(ID);
    waitingDialog.show('Loading record please wait...');

    var urlGetData = $('#urlGetSkipEditData').val();

    $("#srcVideo").attr("src", '');
    $("#srcAudio").attr("src", '');
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { _ID: ID, _InstallerID: InstallerId, _FormID: FormId },
        success: function (msg) {

            console.log("msg === " + JSON.stringify(msg));
            waitingDialog.hide();
            if (msg.success === undefined) {

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
                            debugger;
                            var imagetag = $("#divImage")
                            var path = "";
                            if (item.strCloudUrl !== null && item.strCloudUrl !== "") {
                                path = item.strCloudUrl;
                            }
                            else {
                                path = item.strFilePath;
                            }
                            var imageElement = $('<img/>', {
                                src: path,
                                class: "mySlides",
                                style: "width: 29em;height: 25em;"
                            }).appendTo('#divImage');
                        }
                        else if (item.IsAudioVideoImage == "1") {
                            debugger;
                            $("#audionotfound").text("");
                            $("#divAudio").show();
                            var path = "";
                            if (item.strCloudUrl !== null && item.strCloudUrl !== "") {
                                path = item.strCloudUrl;
                            }
                            else {
                                path = item.strFilePath;
                            }
                            var audiotag = $("#srcAudio")
                            audiotag.attr("src", path)
                            $("#aT").load();
                        }
                        else if (item.IsAudioVideoImage == "2") {
                            debugger;
                            $("#videonotfound").text("");
                            $("#divVideo").show();
                            var path = "";
                            if (item.strCloudUrl !== null && item.strCloudUrl !== "") {
                                path = item.strCloudUrl;
                            }
                            else {
                                path = item.strFilePath;
                            }
                            var videotag = $("#srcVideo");
                            videotag.attr("src", path)
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

                if (msg.Latitude != null) {
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

    var createurl = $("#urlRedirectCreate").val();
    console.log('in EditRecords');
    $.ajax({
        url: createurl,
        datatype: "text",
        type: "GET",
        //long userId, int projectId, long uploadId, long reportFormId
        data: { 'userId': userId, 'projectId': projectId, 'uploadId': accinifiledId, 'reportFormId': reportFId },
        success: function (data) {

            if (data.success === undefined) {
                console.log('in undefined');
                waitingDialog.show('Please Wait...');


                //long userId, int projectId, long uploadId, long reportFormId
                createurl = createurl + "?userId=" + userId + "&projectId=" + projectId + "&uploadId=" + accinifiledId + "&reportFormId=" + reportFId;



                var win = window.open(createurl, '_blank');
                if (win) {
                    //Browser has allowed it to be opened
                    win.focus();
                } else {
                    //Browser has blocked it
                    var $toast = toastr["error"]('Please allow popups for this website', "Notification");

                }
                waitingDialog.hide();
            }
            else {
                console.log('in fail');
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }
        },
        error: function (request, status, error) {
            var $toast = toastr["error"](request.responseText, "Notification !");
            console.log(request.responseText)
        }
    });





}

//Save data after click on Sva button
$("#btnAuditSave").click(function () {

    var auditor = $("#ddlAuditor option:selected").val();
    if (auditor == '0') {

        var $toast = toastr["error"]('Please select Auditor', 'Notification');
        return;
    }
    else {

        var ProjectID = document.getElementById('ProjectId').value;

        var AuditorAllocationData = new Object();
        AuditorAllocationData.Account = $("#lblAccount2").text();
        AuditorAllocationData.ProjectName = $("#lblProjectName2").text();
        AuditorAllocationData.Route = $("#lblRoute2").text();
        AuditorAllocationData.Cycle = $("#lblCycle2").text();
        AuditorAllocationData.Address = $("#lblAddress2").text();
        AuditorAllocationData.InstallerName = $("#lblInataller2").text();
        AuditorAllocationData.ProjectId = $("#AUProjectId").val();
        AuditorAllocationData.UploadId = $("#FkUploadId").val();



        AuditorAllocationData.AuditorID = $("#ddlAuditor option:selected").val();

        var UrlRecord = $("#urlInserProjectAuditAllocation").val();

        $.ajax({

            url: UrlRecord,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(AuditorAllocationData),
            type: 'POST',
            success: function (data) {
                //debugger;;
                if (data.success == true) {

                    $("#modalAuditorAllocation").modal('hide');
                    var $toast = toastr["success"]("Record added to audit successfully.", "Notification");
                }
                else {
                    var $toast = toastr["error"](data.returnMsg, "Notification");
                }
                $("#modalAuditorAllocation").hide();
                _searchClick();
            }

        });

    }



});




function GetAuditData(UpId, InstallerId, FormId) {
    //debugger;;
    $("#upLoadId").val('');
    $("#formId").val('');
    $("#AUProjectId").val('');
    $("#FkUploadId").val('');
    $("#modalAuditorAllocation").modal('hide');

    var Project = $("#ddlProject option:selected")
    var urlGetData = $('#urlGetDataForAddAuditor').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        data: { uploadedId: UpId, projectId: Project.val(), installerId: InstallerId, formId: FormId },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            //debugger;;
            $("#ddlAuditor").empty();

            // var count = data.auditorList;

            if (data.success = true) {
                //debugger;;

                if (data.auditorList != '' || data.auditorList != 'undefined') {
                    //debugger;;
                    $("#modalAuditorAllocation").modal('show');
                    $.each(data.auditorList, function (index, item) {
                        //debugger;;
                        if (index == 0) { $('#ddlAuditor').append($('<option></option>').val('0').html("")); }
                        $('#ddlAuditor').append($('<option></option>').val(item.UserId).html(item.UserName));
                    });

                    $('#ddlAuditor').trigger('chosen:updated');
                }


                // document.getElementById('ProjectId').value = ProjectID;

                // document.getElementById('UploadId').value = UpId;

                $("#AUProjectId").val(Project.val());
                $("#FkUploadId").val(UpId);
                $("#lblProjectName2").text(data.AuditData.ProjectName);

                $("#lblCycle2").text(data.AuditData.Cycle);
                $("#lblRoute2").text(data.AuditData.Route);
                $("#lblAddress2").text(data.AuditData.Address);
                $("#lblAccount2").text(data.AuditData.Account);
                $("#lblInataller2").text(data.AuditData.InstallerName);
                $("#lblFormName2").text(data.AuditData.FormName);
                $("#lblVisitDateTime2").text(data.AuditData.VisitDateTime);
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
//$("#modalAuditorAllocation").modal({ show: true, backdrop: 'static', keyboard: false });



function GetAuditView(UpId, InstallerId, FormId) {
    debugger;
    $("#upLoadId").val('');
    $("#formId").val('');
    $("#auditModel").modal('hide');
    var ProjectID = $("#ddlProject option:selected").val();
    var Project = $("#ddlProject option:selected");
    var urlGetReopenData = $('#urlGetDataForViewAuditor').val();
    $.ajax({
        type: 'GET',
        url: urlGetReopenData,
        data: { uploadedId: UpId, projectId: Project.val(), installerId: InstallerId, formId: FormId },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            //debugger;;
            if (data.success === true) {
                
                if (data.dataList !== '' || data.dataList !== 'undefined') {
                    //debugger;;
                    //var projectID = data.dataList[0].ProjectId;
                    document.getElementById('ProjectId').value = ProjectID;
                    // var uploadID = data.dataList[0].UploadId;
                    document.getElementById('UploadId').value = UpId;
                    $("#lblProjectName").text(data.dataList.ProjectName);
                    $("#lblCycle").text(data.dataList.Cycle);
                    $("#lblRoute").text(data.dataList.Route);
                    $("#lblAddress").text(data.dataList.Address);
                    $("#lblAccount").text(data.dataList.Account);
                    $("#lblInataller").text(data.dataList.InstallerName);
                    $("#lblFormName").text(data.dataList.FormName);
                    $("#lblVisitDateTime").text(data.dataList.VisitDateTime);
                    $("#lblAuditorName").text(data.dataList.AuditorName);

                    $("#auditModel").modal('show');
                }

            }
            // $("#ddlReasonMaster").empty();

            // var count = data.InstallerList.length;
            //if (count > 0) {

            //    $.each(data.InstallerList, function (index, item) {
            //        if (index == 0) { $('#ddlReopenInstaller').append($('<option></option>').val('0').html(""));  }
            //        $('#ddlReopenInstaller').append($('<option></option>').val(item.UserId).html(item.UserName));
            //    });
            //    $('#ddlReopenInstaller').val(instId).attr("selected", "selected");
            //    $('#ddlReopenInstaller').trigger('chosen:updated');
            //}

            $("#upLoadId").val(UpId);
            $("#formId").val(formId);



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


    $("#auditModel").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}




$("#btnAuditCancel").click(function () {


    $("#modalAuditorAllocation").modal('hide');
    $("#modalAuditorAllocation").hide();
});


$("#btnAuditCancel2").click(function () {


    $("#auditModel").modal('hide');
    $("#auditModel").hide();
});



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

var iv2 = null;
var CurrentImageItems = [];

function ViewImages(ProjectFieldDataId) {

    CurrentImageItems = [];

    $("h4.modal-title").text('');
    $('#txtCurrentImageId').val('');

    var urlGetData = $('#urlGetImagesList').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { "ProjectFieldDataId": ProjectFieldDataId },
        success: function (msg) {

            //FieldName = "Skipped Image",
            //           isServiceData = false,
            //           ServiceName = "",
            //           strFilePath = o,

            if (msg.success == true && msg.returnMsg != null && msg.returnMsg.length > 0) {


                var i = 1;

                $.each(msg.returnMsg, function (index, item) {

                    var defaults = { ImagePath: item.strFilePath, ImageName: item.FieldName, SerialNo: i };
                    CurrentImageItems.push(defaults);
                    i++;
                });



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
                    $(".iviewer_remove_image").css("display", "none");
                }
                else {
                    if (CurrentImageItems.length > 1) {
                        $(".iviewer_next_image").css("display", "block");
                    }
                    else {
                        $(".iviewer_next_image").css("display", "none");
                    }

                    iv2.iviewer('loadNextImage', CurrentImageItems[0].ImagePath);
                    $(".iviewer_remove_image").css("display", "none");
                    $(".iviewer_previous_image").css("display", "none");
                }


                $("#allImages").modal({ show: true, backdrop: 'static', keyboard: false });
                $('#txtCurrentImageId').val("1");
            }
            else {

                //if no image found  make icon color red.

                var $toast = toastr["error"]("No Image present for selected records", "Notification");
                return false;
            }




            waitingDialog.hide();
        }
    });
}
//$('#btnPreviousImage').click(function () {
$(document).on('click', '#btnPreviousImage', function () {
    debugger;



    $.each(CurrentImageItems, function (index, item) {

        if (item.SerialNo.toString() === $('#txtCurrentImageId').val()) {

            $('#txtCurrentImageId').val(CurrentImageItems[index - 1].SerialNo);

            iv2.iviewer('loadNextImage', CurrentImageItems[index - 1].ImagePath);


            $("h4.modal-title").text(CurrentImageItems[index - 1].ImageName);


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
$(document).on('click', '#btnNextImage', function () {
    debugger;


    $.each(CurrentImageItems, function (index, item) {

        if (item.SerialNo.toString() === $('#txtCurrentImageId').val()) {


            iv2.iviewer('loadNextImage', CurrentImageItems[index + 1].ImagePath);

            $('#txtCurrentImageId').val(CurrentImageItems[index + 1].SerialNo);




            $("h4.modal-title").text(CurrentImageItems[index + 1].ImageName);


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

    });

});


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

    document.getElementById("quickSearch").disabled = true;
    ResetProject()
    GetProjectListbyProjectStatus()
}

function selectStatus() {
    var table = $('#dynamic-table').DataTable();
    table.clear().draw();

    var selectedlivalue = $('#ulStatus li.highlighted').map(function (i, el) {
        return $(el).val();
    });

    var selectedlivalue1 = $('#ulStatus li.highlighted').map(function (i, el) {
        return $(el);
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#StatusHeader").removeClass("ddlHeader").text("Select visit Status")
    } else if (selectedlilength == 1) {
        $("#StatusHeader").addClass("ddlHeader").text(selectedlivalue1[0].text())
    }
    else {
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
        //var table = $('#dynamic-table').DataTable();
       // table.clear().draw();

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
                var table = $('#dynamic-table').DataTable();
                table.clear().draw();
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
        $("#InstallerHeader").removeClass("ddlHeader").text("Select Installer")
    } else {
        $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Installer selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulInstaller .liInstaller:visible').length > 0 && $('#ulInstaller .highlighted:visible').length == $('#ulInstaller .liInstaller:visible').length) {
        $('#Installer_SelectAll').prop('checked', true);
    } else {
        $('#Installer_SelectAll').prop('checked', false);
    }
    //#endregion

}

//#region open/close Cycle dropdown
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

//#endregion

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
                $("#InstallerHeader").removeClass("ddlHeader").text("Select Installer")
            } else {
                $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Installer selected")
            }

        }
        else {
            $("li.liInstaller:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulInstaller .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#InstallerHeader").removeClass("ddlHeader").text("Select Installer")
            } else {
                $("#InstallerHeader").addClass("ddlHeader").text(selectedlilength + " Installer selected")
            }
        }


    }
});

//#endregion


//#region New code for Cycle multiselect dropdown
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
    //debugger;;
    getRouteListbyCycle()
}

//#region open/close Cycle dropdown
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

//#endregion

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
    debugger;
    if ($('#ulCycle .liCycle').length > 0) {

        if (this.checked) {
            $("li.liCycle:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }

        }
        else {
            $("li.liCycle:visible").removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) { return $(el)[0].value; });
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

//#endregion
//#region New code for Route multiselect dropdown
function getRouteListbyCycle() {

    //debugger;;
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

//#region open/close Route dropdown
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

//#endregion

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

//#endregion
//#region New code for Address multiselect dropdown
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

//#region open/close Address dropdown
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

//#endregion

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

//#endregion

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



$('#btnCloseallImages').on('click', function () {
    $("#allImages").modal('hide');
});
// Handle form submission event
$('#btnExport').on('click', function () {
    debugger;
    var showmap = false;
    if ($('#chkShowMap').is(":checked")) {
        showmap = true;
    }
    else {
        showmap = false;
    }


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
        data: { ReportRowList: ReportRowList, showMap: showmap },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.Status == true) {
                window.open(
                    urlGeneratePDFmultipleRows,
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
            //else {
            //    var $toast = toastr["error"]("Report will not genrated as no record has complete status.", "Notification");
            //}
        },
        error: function (msg) {
        }
    });
});



//Created by Aniket on 1-sep-2016
$('#btnExportlink').on('click', function () {
    debugger;
    var showmap = false;
    if ($('#chkShowMap').is(":checked")) {
        showmap = true;
    }
    else {
        showmap = false;
    }


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

    $.ajax({
        type: 'POST',
        url: urlExportReportMultipleIDs,
        data: { ReportRowList: ReportRowList, showMap: showmap },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {

            if (msg.Status == true) {
                window.open(
                    
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
            //else {
            //    var $toast = toastr["error"]("Report will not genrated as no record has complete status.", "Notification");
            //}
        },
        error: function (msg) {
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