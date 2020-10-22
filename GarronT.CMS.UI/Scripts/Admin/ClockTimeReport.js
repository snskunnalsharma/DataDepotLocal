
// this page create by Deepak date 28 sep 2020
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

    $("#InstallerName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#YearName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#MonthName").chosen({ no_results_text: "Oops, nothing found!" });
    $("#WeekName").chosen({ no_results_text: "Oops, nothing found!" });
    
    
    setControllOnBack();
    GetAllInstallerList();
    GetyearList();
    GetMonthList();

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

            oTable = $('#dynamic-table').dataTable(
                {
                    "oLanguage": {
                        "sLengthMenu": '_MENU_ Records per page'
                    },
                    // "sDom": '<flip><"clear">',
                    "sDom": '<"col-lg-12 drp-rec"fl><"col-lg-12 rep-shw-ent"ip>rt<lip><"clear">',
                    "bPaginate": true,
                    "bAutoWidth": false,
                    "bDestroy": true, "bserverSide": true,
                    "bDeferRender": true, //"sAjaxDataProp": 'aData',

                    //"sAjaxSource": msg.result,

                    "aaSorting": [],
                    "iDisplayLength": -1,
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

                        { "bSearchable": false, "mData": null },
                    ],

                    "aoColumnDefs": [
                        {
                            "mRender": function (data, type, full, row) {
                                var timeInMillis = Date.parse(full.Dates);

                                if (full.ProjectStatus == "Active") {
                                   /* if (full.CategoryType == "COMPLETED") {
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
                                    }*/
                                }
                                else {
                                    /*if (full.CategoryType == "SKIP" || full.CategoryType == "RTU") {

                                        return '<a href="#" id=' + full.ID + ' data-toggle="modal" data-backdrop="static" onclick="GetSKipRTU_Details(' + full.InstallerId + ',' + full.ProjectId + ',' + full.ID + ',' + full.FormId + ');" class="btn btn-primary btn-xs rep_btn_primary" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'

                                    } else {
                                        //return
                                        //'<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'
                                        if (full.CategoryType == "COMPLETED") {
                                            return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' onclick="PDFRecords(this);"  title="PDF" >  <i class="fa fa-file-text"></i></a>'

                                        } else {
                                            return '<a href="#" id=' + full.ID + ' name=' + full.FormId + ' > </a>'
                                        }


                                    }*/

                                }
                            },
                            "aTargets": [8]
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

// Get all installer name list in dropdown

function GetAllInstallerList() {
    var GetProject = $('#UrlGetInstallerList').val();
    $.ajax({
        url: GetProject,
        //data: { ProjectID: 10076 },
        data: {  },
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
    var YearList = [];
    YearList = [
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

    });
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

$('#MonthName').change(function () {
    
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

$('#btnSearch').unbind("click").click(function () {
    searchClick();
    //searchClickmmm();
});

function searchClick() {

    if ($('#InstallerName').val() < 1) {
        var $toast = toastr["error"]('Please select Installer Name first', 'Error Notification');
        return false;
    }
    if ($('#YearName').val() < 1) {
        var $toast = toastr["error"]('Please select Year first', 'Error Notification');
        return false;
    }
    if ($('#MonthName').val() < 1) {
        var $toast = toastr["error"]('Please select Month Name first', 'Error Notification');
        return false;
    }
    if ($('#WeekName').val() < 1) {
        var $toast = toastr["error"]('Please select Week Range first', 'Error Notification');
        return false;
    }

    var _installer = $("#InstallerName").val();
    var _year = $("#YearName").val();
    var _month = $("#MonthName").val();
    var _WeekRangeDate = $("#WeekName").val();

    var UrlGetUserTimeClockReport = $('#UrlGetUserTimeClockReport').val();

    $.ajax({
        type: 'POST',
        url: UrlGetUserTimeClockReport,
        data: { _installer: _installer, _WeekRangeDate: _WeekRangeDate},
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        success: function (msg) {
            debugger;
            if (msg.status == true) {
                /*var allProperties = Object.getOwnPropertyNames(msg.demo1[0]);
                var tableHeader = '<tr>';
                for (var i = 0; i < allProperties.length; i++) {
                    tableHeader += '<th>' + allProperties[i] + '</th>'
                }
                var allRows = tableHeader + '</tr>';
                $('#dynamic-table').append(allRows);*/
                oTable = $('#dynamic-table').DataTable(
                    {
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },
                        "sDom": '<"col-lg-12 drp-rec"fl><"col-lg-12 rep-shw-ent"ip>rt<lip><"clear">',
                        "bPaginate": true,
                        "bAutoWidth": false,
                        "bDestroy": true, "bserverSide": true,
                        "bDeferRender": true,

                        "aaSorting": [],
                        "iDisplayLength": -1,
                        "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                      
                        "aaData": msg.demo1,
                        "aoColumns": [
                            { "mData": "Description" },
                            { "mData": "Monday" },  
                            { "mData": "Tuesday" },
                            { "mData": "Wednesday" },
                            { "mData": "Thursday" },
                            { "mData": "Friday" },
                            { "mData": "Saturday" },
                            { "mData": "TotalWeekInstalls" },
                        ],
                        /*"aoColumnDefs": [
                            {
                                
                                "aTargets": [6]
                            },
                        ],*/
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
                waitingDialog.hide();
            }
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}

function searchClickmmm() {

    if ($('#InstallerName').val() < 1) {
        var $toast = toastr["error"]('Please select Installer Name first', 'Error Notification');
        return false;
    }
    if ($('#YearName').val() < 1) {
        var $toast = toastr["error"]('Please select Year first', 'Error Notification');
        return false;
    }
    if ($('#MonthName').val() < 1) {
        var $toast = toastr["error"]('Please select Month Name first', 'Error Notification');
        return false;
    }
    if ($('#WeekName').val() < 1) {
        var $toast = toastr["error"]('Please select Week Range first', 'Error Notification');
        return false;
    }

    var _installer = $("#InstallerName").val();
    var _year = $("#YearName").val();
    var _month = $("#MonthName").val();
    var _WeekRangeDate = $("#WeekName").val();

    var UrlGetUserTimeClockReport = $('#UrlGetUserTimeClockReport').val();

    $.ajax({
        type: 'POST',
        url: UrlGetUserTimeClockReport,
        data: { _installer: _installer, _WeekRangeDate: _WeekRangeDate },
        beforeSend: function () {
            //waitingDialog.show('Loading Please Wait...');
        },
        success: function (msg) {
            debugger;
            var allProperties = Object.getOwnPropertyNames(msg.demo1[0]);
            var tableHeader = '<tr>';
            for (var i = 0; i < allProperties.length; i++) {
                tableHeader += '<th>' + allProperties[i] + '</th>'
            }
            debugger;
            var allRows = tableHeader + '</tr>';
            for (var index = 0; index < msg.demo1.length; index++) {
                var row = '<tr>';
                for (var i = 0; i < allProperties.length; i++) {
                    row += '<td>' + msg.demo1[index][allProperties[i]] + '</td>'
                }
                row += '</tr>';
                allRows += row;
            }
            debugger;
            $('#dynamic-table').append(allRows);
            debugger;
            $('#table1').append(allRows);
        },
        error: function () {
            alert("something seems wrong");
            waitingDialog.hide();
        }
    });
}


function tablebind(mylist) {
    debugger;
    var obj = mylist;
    if (obj.length > 0) {
        var data = obj[0];
        var table = $("<table />");
        table[0].border = "1";
        var row$;
        debugger;
        var columns = addAllColumnHeaders(data);
        for (var i = 0; i < data.length; i++) {
            row$ = $('<tr/>');

            for (var colIndex = 0; colIndex < columns.length; colIndex++) {
                var cellValue = data[i][columns[colIndex]];

                if (cellValue == null) { cellValue = ""; }

                row$.append($('<td/>').html(cellValue));
            }
            $("#jsonTable").append(row$);
        }
    }
}
function addAllColumnHeaders(myList) {
    debugger;
 
    var allProperties = myList;
    var tableHeader = '<tr>';
    for (var i = 0; i < allProperties.length; i++) {
        tableHeader += '<th>' + allProperties[i] + '</th>'
    }
    var allRows = tableHeader + '</tr>';
    $('#av').append(allRows);
    return tableHeader;
}  