
var oTable;
$(document).ready(function () {


    $('#dynamic-table').DataTable({
        "sDom": '<"dt_head"fp>t<"F"l>'
    });
    // make current page menu color as active
    $("#Reports").addClass('active');
    $("#subReports").addClass('block');


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liClockTimeReport');
    listate.addClass("active");

    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#InstallerName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#YearName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#MonthName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#WeekName").chosen({ no_results_text: "Oops, nothing found!" });
    
    setControllOnBack();
    getProjectList();
    //GetAllInstallerList();
    //GetyearList();
    GetMonthList();
    $('#btnExport').prop("disabled", true);
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
                bindTable();
            }
            else {
                // var $toast = toastr["error"]('No data found for the current search.', '');

                bindTable();
            }

        }
    });
};

var formId;
function bindTable() {
    //debugger;;                    // Changes we will required 

    // waitingDialog.show('Loading Please Wait...');
    var urlActiveRecords = $("#UrlReportsList").val();
    $.ajax({
        url: urlActiveRecords,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: 'POST',
        success: function (msg) {
            //debugger;
            if (msg.status === true) {
                oTable = $('#dynamic-table').DataTable(
                    {
                        //"oLanguage": {
                        //   "sLengthMenu": '_MENU_ Records per page'
                        //},
                        //"sDom": '<"col-lg-12 drp-rec"fl><"col-lg-12 rep-shw-ent"ip>rt<lip><"clear">',
                        "bAutoWidth": false,
                        "bDestroy": true,
                        "bserverSide": true,
                        "bDeferRender": true,
                        "bPaginate": false, //#hide pagination control
                        "bFilter": false, //#hide filter control
                        "showNEntries": false,
                        "bInfo": false,
                        "aaSorting": [],
                        "iDisplayLength": -1,
                        //"aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],

                        "aaData": msg.demo1,
                        "aoColumns": [
                            { "mData": "Description" },
                            { "mData": "Monday" },
                            { "mData": "Tuesday" },
                            { "mData": "Wednesday" },
                            { "mData": "Thursday" },
                            { "mData": "Friday" },
                            { "mData": "Saturday" },
                            { "mData": "TotalWorkInWeek" },
                            { "mData": "Pay" },
                            { "mData": "Total" },
                        ],
                        "fnRowCallback": function (nRow, data, iDisplayIndex, iDisplayIndexFull) {
                            if (iDisplayIndex === 0 || iDisplayIndex === 5) {
                                $('td', nRow).each(function () {
                                    $(this).html('<b>' + $(this).text() + '</b>');
                                });
                            }
                        }
                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
                waitingDialog.hide();
                if ((msg.length == 0)) {
                    var $toast = toastr["error"]('No data found for the current search.', '');
                }
                document.getElementById("totalInstalledMeter").innerHTML = msg.TotalInstalledMeterInWeek;
                document.getElementById("ttHoursInWeek").innerHTML = msg.TotalWorkingHoursInWeek;
                document.getElementById("avgPerHours").innerHTML = msg.AvrPerHours;
            }
            else {
                waitingDialog.hide();
                var $toast = toastr["error"]('No record found.', '');
            }
        },
        error: function () {
            waitingDialog.hide();
            alert("something seems wrong");
        }
    });
}

// Get all active project list in dropdown
function getProjectList() {
    //debugger;
    var GetProject = $('#urlGetProject').val();
    $.ajax({
        url: GetProject,
        data: {},
        success: function (ProjectList) {

            $('#ddlProject').empty();
            //debugger;
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

// Get all installer name list in dropdown
$('#ddlProject').change(function () {
    GetAllInstallerList();
    GetyearList();
});
function GetAllInstallerList() {
    var GetProject = $('#UrlGetInstallerList').val();
    var ProjectID = $("#ddlProject").val();
    $.ajax({
        url: GetProject,
        data: { ProjectID: ProjectID },
        //data: {  },
        success: function (ProjectList) {
      
            $('#InstallerName').empty();
            //debugger;
            $('#InstallerName').append($('<option></option>').val(''));
            $.each(ProjectList, function (index, item) {
                $('#InstallerName').append($('<option></option>').val(item.UserId).html(item.UserName));

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

// Fill year in drowdown
function GetyearList() {

    var GetYearList = $('#UrlYearList').val();
    var ProjectID = $("#ddlProject").val();
    $.ajax({
        url: GetYearList,
        data: { ProjectID: ProjectID },
        success: function (YearList) {

            $('#YearName').empty();
            $('#YearName').append($('<option></option>').val(''));
            $.each(YearList, function (index, item) {
                $('#YearName').append($('<option></option>').val(item.YearId).html(item.YearName));
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

    //Determine the Current Year.
    /*var currentYear = (new Date()).getFullYear();
    $('#YearName').empty();
    $('#YearName').append($('<option></option>').val(''));
    //Loop and add the Year values to DropDownList.
    for (var i = 2017; i <= currentYear; i++) {
        $('#YearName').append($('<option></option>').val(i).html(i));
    }*/

    /*var YearList = [];        // old filling years
        
        YearList = [
        { ID: 2015, year: "2015" },
        { ID: 2016, year: "2016" },
        { ID: 2017, year: "2017" },
        { ID: 2018, year: "2018" },
        { ID: 2019, year: "2019" },
        { ID: 2020, year: "2020" },
        { ID: 2021, year: "2021" },
        { ID: 2022, year: "2022" },
        { ID: 2023, year: "2023" },
        { ID: 2024, year: "2024" },
        { ID: 2025, year: "2025" },
        { ID: 2026, year: "2026" },
        { ID: 2027, year: "2027" },
        { ID: 2028, year: "2028" },
        { ID: 2029, year: "2029" },
        { ID: 2030, year: "2030" },
    ]

    $('#YearName').empty();

    $('#YearName').append($('<option></option>').val(''));
    $.each(YearList, function (index, item) {
        $('#YearName').append($('<option></option>').val(item.ID).html(item.year));

    });*/
}

// fill dropdown week name
function GetMonthList() {
    var MonthName = [];
    MonthName = [
        { ID: 1, Month: "JANUARY" }, 
        { ID: 2, Month: "FEBRUARY" },
        { ID: 3, Month: "MARCH" },
        { ID: 4, Month: "APRIL" },
        { ID: 5, Month: "MAY" },
        { ID: 6, Month: "JUNE" },
        { ID: 7, Month: "JULY" },
        { ID: 8, Month: "AUGUST" },
        { ID: 9, Month: "SEPTEMBER" },
        { ID: 10, Month: "OCTOBER" },
        { ID: 11, Month: "NOVEMBER" },
        { ID: 12, Month: "DECEMBER" },
    ]

    $('#MonthName').empty();

    $('#MonthName').append($('<option></option>').val(''));
    $.each(MonthName, function (index, item) {
        $('#MonthName').append($('<option></option>').val(item.ID).html(item.Month));

    });
}

$('#YearName').change(function () {
   
    GetWeekDayRangeList();
});

$('#MonthName').change(function () {
                                        // un used function
    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select Project name first', 'Error Notification');
        return false;
    }
    if ($('#InstallerName').val() < 1) {
        var $toast = toastr["error"]('Please select Installer Name first', 'Error Notification');
        return false;
    }
    if ($('#YearName').val() < 1) {
        var $toast = toastr["error"]('Please select Year first', 'Error Notification');
        return false;
    }
    var _year = $("#YearName").val();
    var _month = $("#MonthName").val();
    //var proj = $("#MonthName option:selected").val();

    var Getweeks = $('#UrlGetWeekDayRangeList').val();
    $.ajax({
        url: Getweeks,
        data: { _year: _year, _month: _month },
        success: function (WeekRangeList) {
            $('#WeekName').empty();
            //debugger;
            $('#WeekName').append($('<option></option>').val(''));
            $.each(WeekRangeList, function (index, item) {
                $('#WeekName').append($('<option></option>').val(item.WeekRangeDate).html(item.DisplayWeekRangeDate));

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
});

function GetWeekDayRangeList() {
    debugger;
    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select Project name first', 'Error Notification');
        return false;
    }
    if ($('#InstallerName').val() < 1) {
        var $toast = toastr["error"]('Please select Installer Name first', 'Error Notification');
        return false;
    }
    if ($('#YearName').val() < 1) {
        var $toast = toastr["error"]('Please select Year first', 'Error Notification');
        return false;
    }
    var _year = $("#YearName").val();
    var ProjectID = $("#ddlProject").val();

    var Getweeks = $('#UrlGetWeekDayRangeList').val();
    $.ajax({
        url: Getweeks,
        data: { _year: _year, ProjectID: ProjectID },
        success: function (WeekRangeList) {
            debugger;
            $('#WeekName').empty();
            //debugger;
            $('#WeekName').append($('<option></option>').val(''));
            $.each(WeekRangeList, function (index, item) {
                $('#WeekName').append($('<option></option>').val(item.WeekRangeDate).html(item.DisplayWeekRangeDate));

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

$('#btnSearch').unbind("click").click(function () {
    searchClick();
});

function searchClick() {
    //debugger;
    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select Project name first', 'Error Notification');
        return false;
    }
    if ($('#InstallerName').val() < 1) {
        var $toast = toastr["error"]('Please select Installer Name first', 'Error Notification');
        return false;
    }
    if ($('#YearName').val() < 1) {
        var $toast = toastr["error"]('Please select Year first', 'Error Notification');
        return false;
    }
    /*if ($('#MonthName').val() < 1) {
        var $toast = toastr["error"]('Please select Month Name first', 'Error Notification');
        return false;
    }*/
    if ($('#WeekName').val() < 1) {
        var $toast = toastr["error"]('Please select Week Range first', 'Error Notification');
        return false;
    }

    $('#btnExport').prop("disabled", true);
    var _ProjectID = $("#ddlProject").val();
    var _installer = $("#InstallerName").val();
    var _year = $("#YearName").val();
    var _month = $("#MonthName").val();
    var _WeekRangeDate = $("#WeekName").val();

    var UrlGetUserTimeClockReport = $('#UrlGetUserTimeClockReport').val();

    $.ajax({
        type: 'POST',
        url: UrlGetUserTimeClockReport,
        data: { _ProjectID: _ProjectID, _installer: _installer, _WeekRangeDate: _WeekRangeDate },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        success: function (msg) {
            //debugger;
            if (msg.status == true && msg.demo1.length > 0) {
                oTable = $('#dynamic-table').DataTable(
                    {
                        //"oLanguage": {
                        //   "sLengthMenu": '_MENU_ Records per page'
                        //},
                        //"sDom": '<"col-lg-12 drp-rec"fl><"col-lg-12 rep-shw-ent"ip>rt<lip><"clear">',
                        "bAutoWidth": false,
                        "bDestroy": true,
                        "bserverSide": true,
                        "bDeferRender": true,
                        "bPaginate": false, //#hide pagination control
                        "bFilter": false, //#hide filter control
                        "showNEntries": false,
                        "bInfo": false,
                        "aaSorting": [],
                        "iDisplayLength": -1,
                        //"aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                        
                        "aaData": msg.demo1,
                        "aoColumns": [
                            { "mData": "Description" },
                            { "mData": "Monday" },
                            { "mData": "Tuesday" },
                            { "mData": "Wednesday" },
                            { "mData": "Thursday" },
                            { "mData": "Friday" },
                            { "mData": "Saturday" },
                            { "mData": "TotalWorkInWeek" },
                            { "mData": "Pay" },
                            { "mData": "Total" },
                        ],
                        "fnRowCallback": function (nRow, data, iDisplayIndex, iDisplayIndexFull) {
                            /*if (data.IsDuplicate == true) {
                                $('td', nRow).css('background-color', 'yellow');
                            }*/
                            if (iDisplayIndex === 0 || iDisplayIndex === 5) {
                                $('td', nRow).each(function () {
                                    $(this).html('<b>' + $(this).text() + '</b>');
                                });
                            }
                        }
                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
                waitingDialog.hide();
                if ((msg.length == 0)) {
                    var $toast = toastr["error"]('No data found for the current search.', '');
                }
                document.getElementById("totalInstalledMeter").innerHTML = msg.TotalInstalledMeterInWeek;
                document.getElementById("ttHoursInWeek").innerHTML = msg.TotalWorkingHoursInWeek;
                document.getElementById("avgPerHours").innerHTML = msg.AvrPerHours;
                debugger;
                var today = new Date();
                var dd = String(today.getDate()).padStart(2, '0');
                var mm = String(today.getMonth() + 1).padStart(2, '0'); 
                var yyyy = today.getFullYear();
                today = mm + '/' + dd + '/' + yyyy;
                document.getElementById("fTodayDate").innerHTML = today;
                document.getElementById("sTodayDate").innerHTML = today;

                document.getElementById("clockTimeReportFilePath").innerHTML = msg.filePath;
                $('#btnExport').prop("disabled", false);
            }
            else {
                waitingDialog.hide();
                var $toast = toastr["error"]('No record found.', '');
            }
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

$("#btnReset").click(function () {
    ResetAllData();
})

function ResetAllData() {
    debugger;
    $('#WeekName').val("");
    getProjectList();
    $('#InstallerName').val(0).attr("selected", "selected");
    GetyearList();
    GetMonthList();
    $('#btnExport').prop("disabled", true);
    document.getElementById("clockTimeReportFilePath").innerHTML = "";
}


$('#btnExport').on('click', function () {

    GeneratePDF();
});

function GeneratePDF() {
    debugger;
    //var gh = 'http://localhost:6070/InventoryUpload/sampleMar-24-202103-28-06.pdf';
    var clockTimeReportFilePath = document.getElementById("clockTimeReportFilePath").innerText;
    window.open(
        clockTimeReportFilePath,
        '_blank' // <- This is what makes it open in a new window.
    );
}