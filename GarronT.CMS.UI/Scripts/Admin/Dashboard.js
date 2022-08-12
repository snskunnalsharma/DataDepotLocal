var spinner, opts, target;
var oTable;
var StateData;
var CityData;
var BindCon;
var colorcodearray = Array('#CCCCCC', '#1DAADF', '#37B7E6', '#54C7F3', '#6FD9FF',
                    '#20CA6A', '#46DE89', '#51FF9D', '#78FFB3', '#808080', '#999999', '#B3B3B3');

$(document).ready(function () {
    $("#Dashboard").addClass('active').css('background-color', '#EF6C00').css('border-radius', '6px');
    $("#Dashboard").children().css('color', '#fff');


    $("#ddlProjectStatus").chosen({ placeholder_text_single: "Select Project Status", no_results_text: "Oops, nothing found!" });
    $('#ddlProjectStatus').trigger('chosen:updated');

    $("#ddlUtilityType").chosen({ placeholder_text_single: "Select Utility", no_results_text: "Oops, nothing found!" });
    $('#ddlUtilityType').trigger('chosen:updated');

    $("#ddlProjectComvsPending").chosen({ placeholder_text_single: "Select Project", no_results_text: "Oops, nothing found!" }).trigger('chosen:updated');;
    GetCurrentProjectDefault();    // For Dashboard showing project info comment getProjectList() function  call bellow


});


function GetCurrentProjectDefault()     //Code for displaying project info  Pie chart according to utility
{
    var GetProjectUrl = $("#urlLodDefualtProjectDetails").val();


    $.ajax({
        type: 'GET',
        url: GetProjectUrl,
        async: false,
        success: function (data) {



            if (data != undefined && data.length != '') {

                $('#ddlUtilityType').val(data.Utilitytype === "Electric" ? '1' : (data.Utilitytype === "Gas" ? '2' : '3')).trigger('chosen:updated');

                getProjectList();

            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("error " + textStatus);
            console.log("incoming Text " + jqXHR.responseText);
        }
    });



}



function getProjectList() {

    var projectUtilityType = $('#ddlUtilityType').val() == "0" ? "no" :
        ($('#ddlUtilityType').val() == 1 ? "Electric" : ($('#ddlUtilityType').val() == 2 ? "Gas" : "Water"));

    var projectStatus = $('#ddlProjectStatus').val() == "0" ? "no" :
        ($('#ddlProjectStatus').val() == 1 ? "Active" : "Complete");

    var GetProject = $('#urlGetProject').val();

    $.ajax({
        type: 'GET',
        url: GetProject,
        async: false,
        data: { utilityType: projectUtilityType, ProjectStatus: projectStatus },
        success: function (data) {

            $('#ddlProjectComvsPending').empty();
            var id;
            var name;
            var count = data.length;
            if (count > 0) {

                $.each(data, function (index, item) {
                    if (index == 0) {
                        id = item.ProjectId;
                    }
                    $('#ddlProjectComvsPending').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));
                });

            }

            $('#ddlProjectComvsPending').val(id).trigger('chosen:updated');
            bindGraph();

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }

    });

}


$('#ddlUtilityType').change(function () {
    getProjectList();
})

$('#ddlProjectStatus').change(function () {
    getProjectList();
})

var CompletionvsPendingprojectname = "";
var InstallationPerInstallerprojectname = "";
var InstallationChartprojectname = "";
var ProjectwiseInstallationprojectname = "";


$('#ddlProjectComvsPending').change(function () {
    bindGraph();
})

function bindGraph() {
    var projectID = $('#ddlProjectComvsPending').val();
    if (projectID != "" && projectID != undefined) {


        if (projectID != 0) {
            CompletionvsPendingprojectname = $("#ddlProjectComvsPending option[value='" + projectID + "']").text();

            //Complete vs Pending chart
            CompletionvsPendingChart(projectID);


            //LoadInstallationPerProject(projectID);

            LoadInstallationChart(projectID);

            //Installation per installer chart
            LoadInstallationPerInstaller(projectID);


            //Project completion in percent chart.
            LoadProjectCompletionInPercentGraph();

            //Project Installation- Audit Chart added by sominath on 04/01/2017
            LoadInstallationAuditDataChart(projectID);

        }
        else {
            CompletionvsPendingprojectname = "";
            clearCompletedVsPending();

            clearInstallationPerInstaller();

            clearProjectCompletion();

            clearMonthWiseInstallation();

            cleareInstallerAllocationChart();
        }
    }
    else {
        //Complete vs Pending chart
        clearCompletedVsPending();

        clearInstallationPerInstaller();

        clearProjectCompletion();

        clearMonthWiseInstallation();

        cleareInstallerAllocationChart();
    }
}

function clearCompletedVsPending() {
    var data = eval(null);
    var achivementdata = "";
    var dataPoints = [];
    //for (var i = 0; i <= data.length - 1; i++) {
    //    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status });
    //}

    var chart = new CanvasJS.Chart("chartContainer", {
        title: {
            text: "  Completed vs Pending Chart",
            fontSize: 20,
            backgroundColor: "#FFFFE0",
        },
        animationEnabled: true,
        axisX: {
            interval: 1,
        },
        data: [
        {
            //color: "#006633",
            type: "pie",
            name: "Installations",
            showInLegend: true,
            //  indexLabelBackgroundColor: "red",
            indexLabel: "{legendText} {Installation}",
            // legendText: "{indexLabel}",
            toolTipContent: "{legendText}<br/><span style='\"'color: {color};'\"'><strong>{name}</strong></span>: {Installation}",
            //color: '#CCCCCC',
            dataPoints: dataPoints
        }
        ]
    });
    chart.render();
}

function clearInstallationPerInstaller() {
    var data = eval(null);
    var totaluploadedrecords = 0;
    var dataPoint_Completed = [];
    var CompletedTotal = 0;


    var dataPoints_Pending = [];
    var PendingTotal = 0;


    var dataPoints_skipped = [];
    var SkippedTotal = 0;

    var dataPoints_rtu = [];
    var RTUTotal = 0;



    var chart = new CanvasJS.Chart("chartContainerInstallationbyInstaller", {
        // exportEnabled: true,
        exportFileName: "Installations Per Installer",
        zoomEnabled: true,
        theme: "theme1",
        animationEnabled: true,

        title: {
            text: InstallationPerInstallerprojectname + "  Installations Per Installer",
            fontSize: 20,
            backgroundColor: "#FFFFE0",
        },
        toolTip: {
            shared: true
        },
        axisY: {
            title: "Installations",
            minimum: 0
            // includeZero: true
        },
        axisX: {
            interval: 1,
            labelAngle: 130,
            title: "Installers "
        },
        data: [
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 16,
                    color: "Green",
                    type: "column",
                    name: "Completed",
                    legendText: "Completed-" + CompletedTotal,
                    showInLegend: true,
                    toolTipContent: "<strong>{label}: {total}</strong> <br/><span style='\"'color: {color};'\"'>Completed</span>: {y}",
                    dataPoints: dataPoint_Completed
                },
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    color: "Blue",
                    indexLabelFontSize: 16,
                    type: "column",
                    name: "Pending",
                    legendText: "Pending-" + PendingTotal,
                    showInLegend: true,
                    dataPoints: dataPoints_Pending
                },
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelBackgroundColor: "Blue",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    color: "Yellow",
                    indexLabelFontSize: 16,
                    type: "column",
                    name: "Skipped",
                    legendText: "Skipped-" + SkippedTotal,
                    showInLegend: true,
                    dataPoints: dataPoints_skipped
                },

                 {
                     indexLabel: "{y}",
                     indexLabelPlacement: "inside",
                     indexLabelBackgroundColor: "Blue",
                     indexLabelOrientation: "vertical",
                     indexLabelFontStyle: "bold",
                     indexLabelFontColor: "black",
                     color: "Red",
                     indexLabelFontSize: 16,
                     type: "column",
                     name: "RTU",
                     legendText: "RTU -" + RTUTotal,
                     showInLegend: true,
                     dataPoints: dataPoints_rtu
                 },
                 {
                     legendText: "Total-" + totaluploadedrecords,
                     showInLegend: true,
                 }
        ],

    });
    chart.render();
}

function clearMonthWiseInstallation() {


    var dataPoint_Completed = [];
    var CompletedTotal = 0;


    var dataPoints_Pending = [];
    var PendingTotal = 0;


    var dataPoints_skipped = [];
    var SkippedTotal = 0;




    var chart = new CanvasJS.Chart("chartContainerInstallation", {
        // exportEnabled: true,
        exportFileName: "Installations Per Month",
        zoomEnabled: true,
        theme: "theme1",
        animationEnabled: true,
        title: {
            text: "  Installations Per Month",
            fontSize: 20,
            backgroundColor: "#FFFFE0",
        },
        toolTip: {
            shared: true
        },
        axisY: {
            title: "Installations",
            minimum: 0
        },
        axisX: {
            interval: 1,
            labelAngle: 130,
            title: "Months"

        },
        data: [
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    color: "#A2DE73",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 16,
                    type: "column",
                    name: "Completed",
                    legendText: "Completed-",
                    showInLegend: true,


                    //toolTipContent: "<strong>{label}: '1000'</strong> <br/><span style='\"'color: {color};'\"'>Completed</span>: {y}",

                    dataPoints: dataPoint_Completed
                },
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 16,
                    color: "#819AD5",
                    type: "column",
                    name: "Pending",
                    legendText: "Pending-",
                    showInLegend: true,
                    dataPoints: dataPoints_Pending
                },
                {

                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 16,
                    color: "#E1554A",
                    type: "column",
                    name: "Skipped",
                    legendText: "Skipped-",
                    showInLegend: true,
                    dataPoints: dataPoints_skipped
                },
                {
                    legendText: "Total-",
                    showInLegend: true,
                },

        ],
    });
    chart.render();
}

function clearProjectCompletion() {


    var dataPoint_Completed = [];
    var CompletedTotal = 0;

    var chart = new CanvasJS.Chart("chartProjectPercentage", {
        //exportEnabled: true,
        exportFileName: "Project completion in percentage",
        zoomEnabled: true,
        theme: "theme1",
        animationEnabled: true,
        title: {
            text: "Project completion in percentage",
            fontSize: 20,
            backgroundColor: "#FFFFE0",
        },
        toolTip: {
            shared: true
        },
        axisY: {
            title: "Percentage",
            gridThickness: 1
        },
        axisX: {
            interval: 1,
            labelAngle: 130,
            title: "Projects "
        },
        data: [

                {
                    indexLabel: "{y} %",
                    indexLabelPlacement: "outside",
                    indexLabelOrientation: "horizontal",
                    indexLabelBackgroundColor: "#000",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 18,
                    type: "column",
                    toolTipContent: "<strong>{label}</strong> <br/><span style='\"'color: {color};'\"'>Percentage</span>: {y}%",
                    dataPoints: dataPoint_Completed
                }


        ],

    });
    chart.render();
}

function cleareInstallerAllocationChart() {
    var data = eval(null);
    var InstallationTotal = 0;
    var AssignedTotal = 0;
    var CompleteTotal = 0;
    var PassedTotal = 0;
    var FailedTotal = 0;
    var dataPoint_TotalInstallation = [];
    var dataPoint_TotalAssigned = [];
    var dataPoint_TotalComplete = [];
    var dataPoint_Passed = [];
    var dataPoint_Failled = [];




    var chart = new CanvasJS.Chart("chartInstallerAuditChart", {
        //exportEnabled: true,
        exportFileName: " Audits Per Installer",
        zoomEnabled: true,
        theme: "theme1",
        animationEnabled: true,

        title: {
            text: " Audit Per Installer",
            fontSize: 20,
            backgroundColor: "#FFFFE0",
        },
        toolTip: {
            shared: true
        },
        axisY: {
            title: "Audits",
            minimum: 0
            // includeZero: true
        },
        axisX: {
            interval: 1,
            labelAngle: 130,
            title: "Installers"
        },
        legend: {
            fontSize: 14,
        },
        data: [
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    indexLabelFontSize: 16,
                    color: "Orange",
                    type: "column",
                    name: "Total Inatallation",
                    legendText: "Total Inatallation -" + InstallationTotal,
                    showInLegend: true,
                    // toolTipContent: "<strong>{label}: {total}</strong> <br/><span style='\"'color: {color};'\"'>TotalInstallation</span>: {y}",
                    dataPoints: dataPoint_TotalInstallation
                },
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    color: "Blue",
                    indexLabelFontSize: 16,
                    type: "column",
                    name: "Total Assiged",
                    legendText: "Assigned Total -" + AssignedTotal,
                    showInLegend: true,
                    dataPoints: dataPoint_TotalAssigned
                },
                {
                    indexLabel: "{y}",
                    indexLabelPlacement: "inside",
                    indexLabelBackgroundColor: "blue",
                    indexLabelOrientation: "vertical",
                    indexLabelFontStyle: "bold",
                    indexLabelFontColor: "black",
                    color: "Skyblue",
                    indexLabelFontSize: 16,
                    type: "column",
                    name: "Total Complete",
                    legendText: "Total Complete -" + CompleteTotal,
                    showInLegend: true,
                    dataPoints: dataPoint_TotalComplete
                },
                 {
                     indexLabel: "{y}",
                     indexLabelPlacement: "inside",
                     indexLabelBackgroundColor: "blue",
                     indexLabelOrientation: "vertical",
                     indexLabelFontStyle: "bold",
                     indexLabelFontColor: "black",
                     color: "Green",
                     indexLabelFontSize: 16,
                     type: "column",
                     name: "Total Passed",
                     legendText: "Total Passed-" + PassedTotal,
                     showInLegend: true,
                     dataPoints: dataPoint_Passed
                 },
                 {
                     indexLabel: "{y}",
                     indexLabelPlacement: "inside",
                     indexLabelBackgroundColor: "blue",
                     indexLabelOrientation: "vertical",
                     indexLabelFontStyle: "bold",
                     indexLabelFontColor: "black",
                     color: "Red",
                     indexLabelFontSize: 16,
                     type: "column",
                     name: "Total Failed",
                     legendText: "Total Failed -" + FailedTotal,
                     showInLegend: true,
                     dataPoints: dataPoint_Failled
                 },
                  {
                      legendText: "Total -" + InstallationTotal,
                      showInLegend: true,
                  }

        ],

    });
    chart.render();

}

function cleareAuditChart() {

}


function BindYear() {
    var actualDate = new Date(); // convert to actual date
    var currentDay = actualDate.getDate()
    var newDate;
    if (currentDay > 25) {
        newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());
    }
    else {
        newDate = new Date(actualDate.getFullYear(), actualDate.getMonth(), actualDate.getDate());
    }

    var prevDate = new Date(actualDate.getFullYear(), actualDate.getMonth() - 1, actualDate.getDate());

    $('#txtYear').datepicker({
        changeMonth: false,
        changeYear: true,
        defaultDate: newDate,
        maxDate: newDate,
        //  showButtonPanel: true,
        dateFormat: 'yy',

        onChangeMonthYear: function (year, month, inst) {

            var date = $("#txtYear").val();
            if ($.trim(date) != "") {
                var newDate = new Date(month + "/" + inst.currentDay + "/" + year);
                $("#txtYear").val($.datepicker.formatDate('yy', newDate));
            }
            LoadInstallationChart($("#txtYear").val())
        }
    }).datepicker("setDate", newDate).focus(function () {
        //  $('.ui-datepicker-calendar').detach();

    })



}

function createPDF(canvas) {
    var data = canvas.toDataURL("image/jpeg", 1.0);
    var docDefinition = {

        pageMargins: [10, 60, 40, 60],
        content: [{
            image: data,
            width: 500,
            height: 400,
            margin: [10, 10, 20, 10],
            fit: [550, 200]
        }]
    };

    var win = window.open('Test.pdf', '_blank');

    // pass the "win" argument
    pdfMake.createPdf(docDefinition).open({}, win);
}


$("#btnExportCPChart").click(function () {

    //debugger;
    var canvas = $("#chartContainer .canvasjs-chart-canvas").get(0);
    createPDF(canvas)
    // var dataURL = canvas.toDataURL();
    //console.log(dataURL);


    // var pdf = new jsPDF();
    // pdf.addImage(dataURL, 'JPEG', 10, 10, 200, 100);
    //pdf.output('dataurlnewwindow');
    //var imgData = canvas.toDataURL("image/jpeg", 1.0);
    //var pdf = new jsPDF();

    //pdf.addImage(imgData, 'JPEG', 0, 0);
    //var download = document.getElementById('download');

    // pdf.save("download.pdf");


    //var data = canvas.toDataURL("image/jpeg", 1.0);
    //var docDefinition = {


    //    content: [{
    //        image: data,
    //       // width: 400,
    //        //height: 400,
    //        fit: [600,1000]
    //    }]
    //};

    //var win = window.open('Test.pdf', '_blank');

    //// pass the "win" argument
    //pdfMake.createPdf(docDefinition).open({}, win);

})

$("#btnExportCPChart1").click(function () {
    //debugger;
    var canvas = $("#chartContainerInstallation .canvasjs-chart-canvas").get(0);
    createPDF(canvas)

    //var dataURL = canvas.toDataURL();

    //var pdf = new jsPDF();
    //pdf.addImage(dataURL, 'JPEG', 10, 10, 200, 100);
    //pdf.output('dataurlnewwindow');
})

$("#btnExportCPChart2").click(function () {
    //debugger;
    var canvas = $("#chartContainerInstallationbyInstaller .canvasjs-chart-canvas").get(0);

    createPDF(canvas)

    //var dataURL = canvas.toDataURL();
    ////console.log(dataURL);


    //var pdf = new jsPDF();
    //pdf.addImage(dataURL, 'JPEG', 10, 10, 200, 100);
    //pdf.output('dataurlnewwindow');
})

$("#btnExportCPChart3").click(function () {
    //debugger;
    var canvas = $("#chartProjectPercentage .canvasjs-chart-canvas").get(0);

    createPDF(canvas)

    //var dataURL = canvas.toDataURL();

    //var pdf = new jsPDF();
    //pdf.addImage(dataURL, 'JPEG', 10, 10, 200, 100);
    //pdf.output('dataurlnewwindow');
})

//Added by Sominath on 05/01/2016
$("#btnExportAuditChart1").click(function () {
    //debugger;
    var canvas = $("#chartInstallerAuditChart .canvasjs-chart-canvas").get(0);

    createPDF(canvas)

    //var dataURL = canvas.toDataURL();

    //var pdf = new jsPDF();
    //pdf.addImage(dataURL, 'JPEG', 10, 10, 200, 100);
    //pdf.output('dataurlnewwindow');
})

//
function CompletionvsPendingChart(projectID) {

    var urlGetCompleteVsPendingChartData = $('#urlGetCompleteVsPendingChartData').val();

    $.ajax({
        url: urlGetCompleteVsPendingChartData,
        type: 'GET',
        data: { projectID: projectID },
        success: function (msg) {
            var data = eval(msg);
            var achivementdata = "";
            var dataPoints = [];
            for (var i = 0; i <= data.length - 1; i++) {
                if (data[i].Status == "Completed")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Green", markerBorderThickness: 2 });
                if (data[i].Status == "RTU")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Red", markerBorderThickness: 2 });
                if (data[i].Status == "Skipped")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Yellow", markerBorderThickness: 2 });
                if (data[i].Status == "Pending")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Blue", markerBorderThickness: 2 });
                if (data[i].Status == "Not Allocated")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Grey", markerBorderThickness: 2 });
                if (data[i].Status == "UnAllocated Skip")
                    dataPoints.push({ Installation: parseInt(data[i].Installations), y: parseInt(data[i].percentage), legendText: data[i].Status, color: "Ash Gray", markerBorderThickness: 2 });
            }

            var chart = new CanvasJS.Chart("chartContainer", {
                title: {
                    text: CompletionvsPendingprojectname + "  Completed vs Pending Chart",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                animationEnabled: true,
                //  height: 1200,
                // width: 1000,
                axisX: {
                    interval: 1,
                },
                legend: {
                    fontSize: 14,
                },
                data: [
                {
                    //color: "#006633",
                    type: "pie",
                    name: "Installations",
                    showInLegend: true,
                    //  indexLabelBackgroundColor: "red",
                    indexLabel: "{legendText} {Installation}",
                    // legendText: "{indexLabel}",
                    toolTipContent: "{legendText}<br/><span style='\"'color: {color};'\"'><strong>{name}</strong></span>: {Installation}",
                    //color: '#CCCCCC',
                    BorderThickness: 2,
                    BorderColor: 'Black',
                    dataPoints: dataPoints
                }
                ]
            });
            chart.render();

        },
        error: function () {
            //alert("something seems wrong" + urlMyGoalChart);
        }
    });

}



//Installations per month
function LoadInstallationChart(projectID) {

    var urlGetInstallationspermonth = $('#urlGetInstallationspermonth').val();
    $.ajax({
        url: urlGetInstallationspermonth,
        type: 'GET',
        data: { projectID: projectID },
        beforeSend: function () {
            //waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            // waitingDialog.hide();
        },
        success: function (msg) {

            //debugger;
            var data = eval(msg.list);

            var dataPoint_Completed = [];
            var CompletedTotal = 0;

            for (var i = 0; i <= data.length - 1; i++) {
                CompletedTotal += data[i].Complete;
                //dataPoint_Completed.push({ label: data[i].Month, goal: data[i].Complete, y: parseInt(data[i].Complete) });
                dataPoint_Completed.push({ label: data[i].Month, y: (parseInt(data[i].Complete) > 0 ? parseInt(data[i].Complete) : "0"), total: parseInt(data[i].Value) });
            }

            var dataPoints_Pending = [];
            var PendingTotal = data[0].Pending;
            for (var i = 0; i <= data.length - 1; i++) {
                // PendingTotal += data[i].Pending
                dataPoints_Pending.push({ label: data[i].Month, y: (parseInt(data[i].Pending) > 0 ? parseInt(data[i].Pending) : "0") });
            }

            var dataPoints_skipped = [];
            var SkippedTotal = 0;
            for (var i = 0; i <= data.length - 1; i++) {
                SkippedTotal += data[i].Skipped
                dataPoints_skipped.push({ label: data[i].Month, y: (parseInt(data[i].Skipped) > 0 ? parseInt(data[i].Skipped) : "0") });
            }



            var chart = new CanvasJS.Chart("chartContainerInstallation", {
                //exportEnabled: true,
                exportFileName: "Installations Per Month",
                zoomEnabled: true,
                theme: "theme1",
                animationEnabled: true,
                title: {
                    text: InstallationChartprojectname + "  Installations Per Month",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                toolTip: {
                    shared: true
                },
                axisY: {
                    title: "Installations",
                    minimum: 0
                },
                axisX: {
                    interval: 1,
                    labelAngle: 130,
                    title: "Months"

                },
                data: [
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            color: "#A2DE73",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 16,
                            type: "column",
                            name: "Completed",
                            legendText: "Completed-" + CompletedTotal,
                            showInLegend: true,


                            //toolTipContent: "<strong>{label}: '1000'</strong> <br/><span style='\"'color: {color};'\"'>Completed</span>: {y}",

                            dataPoints: dataPoint_Completed
                        },
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 16,
                            color: "#819AD5",
                            type: "column",
                            name: "Pending",
                            legendText: "Pending-" + msg.PendingRecords,
                            showInLegend: true,
                            dataPoints: dataPoints_Pending
                        },
                        {

                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 16,
                            color: "#E1554A",
                            type: "column",
                            name: "Skipped",
                            legendText: "Skipped-" + SkippedTotal,
                            showInLegend: true,
                            dataPoints: dataPoints_skipped
                        },
                        {
                            legendText: "Total-" + msg.totaluploadedrecords,
                            showInLegend: true,
                        },

                ],
            });
            chart.render();

        },
        error: function () {
            //alert("something seems wrong" + urlMyGoalChart);
        }
    });

}

//Installations per Installer
function LoadInstallationPerInstaller(projectID) {

    var urlGetInstallationPerInstaller = $('#urlGetInstallationPerInstaller').val();
    $.ajax({
        url: urlGetInstallationPerInstaller,
        type: 'GET',
        data: { projectID: projectID },
        beforeSend: function () {
            //  waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            //debugger;
            var data = eval(msg.list);
            var totaluploadedrecords = msg.totaluploadedrecords;
            var dataPoint_Completed = [];
            var CompletedTotal = 0;
            for (var i = 0; i <= data.length - 1; i++) {
                CompletedTotal += data[i].Complete;

                dataPoint_Completed.push({ label: data[i].Installer, y: (parseInt(data[i].Complete) > 0 ? parseInt(data[i].Complete) : "0"), total: parseInt(data[i].Total), color: "Green" });
            }

            var dataPoints_Pending = [];
            var PendingTotal = 0;
            for (var i = 0; i <= data.length - 1; i++) {
                PendingTotal += data[i].Pending
                dataPoints_Pending.push({ label: data[i].Installer, y: (parseInt(data[i].Pending) > 0 ? parseInt(data[i].Pending) : "0"), color: "Blue" });
            }

            var dataPoints_skipped = [];
            var SkippedTotal = 0;
            for (var i = 0; i <= data.length - 1; i++) {
                SkippedTotal += data[i].Skipped
                dataPoints_skipped.push({ label: data[i].Installer, y: (parseInt(data[i].Skipped) > 0 ? parseInt(data[i].Skipped) : "0"), color: "Yellow" });
            }

            var dataPoints_rtu = [];
            var RTUTotal = 0;
            for (var i = 0; i <= data.length - 1; i++) {
                RTUTotal += data[i].RTU
                dataPoints_rtu.push({ label: data[i].Installer, y: (parseInt(data[i].RTU) > 0 ? parseInt(data[i].RTU) : "0"), color: "Red" });
            }



            var chart = new CanvasJS.Chart("chartContainerInstallationbyInstaller", {
                //exportEnabled: true,
                exportFileName: CompletionvsPendingprojectname + " Installations Per Installer",
                zoomEnabled: true,
                theme: "theme1",
                animationEnabled: true,

                title: {
                    text: CompletionvsPendingprojectname + " Installations Per Installer",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                toolTip: {
                    shared: true
                },
                axisY: {
                    title: "Installations",
                    minimum: 0
                    // includeZero: true
                },
                axisX: {
                    interval: 1,
                    labelAngle: 130,
                    title: "Installers "
                },
                legend: {
                    fontSize: 14,
                },
                data: [
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 16,
                            color: "Green",
                            type: "column",
                            name: "Completed",
                            legendText: "Completed -" + CompletedTotal,
                            showInLegend: true,
                            toolTipContent: "<strong>{label}: {total}</strong> <br/><span style='\"'color: {color};'\"'>Completed</span>: {y}",
                            dataPoints: dataPoint_Completed
                        },
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            color: "Blue",
                            indexLabelFontSize: 16,
                            type: "column",
                            name: "Pending",
                            legendText: "Pending -" + PendingTotal,
                            showInLegend: true,
                            dataPoints: dataPoints_Pending
                        },
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelBackgroundColor: "blue",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            color: "Yellow",
                            indexLabelFontSize: 16,
                            type: "column",
                            name: "Skipped",
                            legendText: "Skipped -" + SkippedTotal,
                            showInLegend: true,
                            dataPoints: dataPoints_skipped
                        },
                         {
                             indexLabel: "{y}",
                             indexLabelPlacement: "inside",
                             indexLabelBackgroundColor: "blue",
                             indexLabelOrientation: "vertical",
                             indexLabelFontStyle: "bold",
                             indexLabelFontColor: "black",
                             color: "Red",
                             indexLabelFontSize: 16,
                             type: "column",
                             name: "RTU",
                             legendText: "RTU -" + RTUTotal,
                             showInLegend: true,
                             dataPoints: dataPoints_rtu
                         },
                         {
                             legendText: "Total-" + totaluploadedrecords,
                             showInLegend: true,
                         }
                ],

            });
            chart.render();

        },
        error: function () {
            //alert("something seems wrong" + urlMyGoalChart);
        }
    });

}

function LoadInstallationPerProject() {

    var urlGetProjectInsatallation = $('#urlGetProjectInsatallation').val();
    $.ajax({
        url: urlGetProjectInsatallation,
        //data: { projectID: projectID },
        type: 'GET',
        success: function (msg) {
            //debugger;
            var data = eval(msg);
            var achivementdata = "";
            var dataPoints = [];
            for (var i = 0; i <= data.length - 1; i++) {
                dataPoints.push({ indexLabel: data[i].Project, goal: data[i].Sales, y: parseInt(data[i].Sales), legendText: data[i].Project });
            }

            var chart = new CanvasJS.Chart("chartContainerProjectwiseInstallation", {
                title: {
                    text: "Installations Per Project",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                animationEnabled: true,
                axisX: {
                    interval: 1,

                },
                data: [
                {
                    //color: "#006633",
                    type: "pie",
                    name: "Installations",
                    showInLegend: true,
                    // legendText: "{indexLabel}",
                    toolTipContent: "{indexLabel}<br/><span style='\"'color: {color};'\"'><strong>{name}</strong></span>: {goal}",
                    //color: '#CCCCCC',
                    dataPoints: dataPoints
                }
                ]
            });
            chart.render();

        },
        error: function () {
            //alert("something seems wrong" + urlMyGoalChart);
        }
    });

}

//Project Completion percent
function LoadProjectCompletionInPercentGraph() {

    var projectUtilityType = $('#ddlUtilityType').val() == "0" ? "no" :
       ($('#ddlUtilityType').val() == 1 ? "Electric" : ($('#ddlUtilityType').val() == 2 ? "Gas" : "Water"));

    var projectStatus = $('#ddlProjectStatus').val() == "0" ? "no" :
       ($('#ddlProjectStatus').val() == 1 ? "Active" : "Complete");

    var urlLoadProjectCompletionInPercentGraph = $('#urlLoadProjectCompletionInPercentGraph').val();
    $.ajax({
        url: urlLoadProjectCompletionInPercentGraph,
        type: 'GET',
        data: { utilityType: projectUtilityType, ProjectStatus: projectStatus },
        beforeSend: function () {
            //  waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (msg) {
            
            var data = eval(msg);

            var dataPoint_Completed = [];
            var CompletedTotal = 0;

            for (var i = 0; i <= data.length - 1; i++) {
                CompletedTotal += data[i].Complete;
                dataPoint_Completed.push({ label1: data[i].projectName, label: data[i].projectDisplayName, y: parseInt(data[i].percentage), indexLabelBackgroundColor: "lightgreen" });
            }





            var chart = new CanvasJS.Chart("chartProjectPercentage", {
                //exportEnabled: true,
                exportFileName: "Project completion in percentage",
                zoomEnabled: true,
                theme: "theme1",
                animationEnabled: true,
                title: {
                    text: "Project completion in percentage",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                toolTip: {
                    shared: true
                },
                axisY: {
                    title: "Percentage",
                    gridThickness: 1
                },
                axisX: {
                    interval: 1,
                    labelAngle: 130,
                    title: "Projects "
                },
                data: [

                        {
                            indexLabel: "{y} %",
                            indexLabelPlacement: "outside",
                            indexLabelOrientation: "horizontal",
                            indexLabelBackgroundColor: "#000",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 18,
                            type: "column",
                            toolTipContent: "<strong>{label1}</strong> <br/><span style='\"'color: {color};'\"'>Percentage</span>: {y}%",
                            dataPoints: dataPoint_Completed
                        }


                ],

            });
            chart.render();

        },
        error: function () {
            //alert("something seems wrong" + urlMyGoalChart);
        }
    });

}

//Project Installation- Audit Dashboard Chart
//Added by Sominath on 05/01/2016
function LoadInstallationAuditDataChart(projectID) {

    var getUrl = $("#urlLoadInstallationAuditChart").val();

    $.ajax({
        type: 'GET',
        url: getUrl,
        async: false,
        data: { projectID: projectID },
        success: function (data) {


            var InstallationTotal = 0;
            var AssignedTotal = 0;
            var CompleteTotal = 0;
            var PassedTotal = 0;
            var FailedTotal = 0;
            if (data.length != '') {


                var dataPoint_TotalInstallation = [];

                for (var i = 0; i <= data.length - 1; i++) {
                    InstallationTotal += data[i].TotalInstallation;
                    dataPoint_TotalInstallation.push({ label: data[i].Installer, y: (parseInt(data[i].TotalInstallation) > 0 ? parseInt(data[i].TotalInstallation) : "0"), total: parseInt(data[i].TotalInstallation), color: "Orange" });
                }



                var dataPoint_TotalAssigned = [];

                for (var i = 0; i <= data.length - 1; i++) {
                    AssignedTotal += data[i].TotalAssignedAudit;
                    dataPoint_TotalAssigned.push({ label: data[i].Installer, y: (parseInt(data[i].TotalAssignedAudit) > 0 ? parseInt(data[i].TotalAssignedAudit) : "0"), total: parseInt(data[i].TotalAssignedAudit), color: "Blue" });
                }

                var dataPoint_TotalComplete = [];

                for (var i = 0; i <= data.length - 1; i++) {
                    CompleteTotal += data[i].TotalCompleteAudit;
                    dataPoint_TotalComplete.push({ label: data[i].Installer, y: (parseInt(data[i].TotalCompleteAudit) > 0 ? parseInt(data[i].TotalCompleteAudit) : "0"), total: parseInt(data[i].TotalCompleteAudit), color: "Skyblue" });
                }
                var dataPoint_Passed = [];

                for (var i = 0; i <= data.length - 1; i++) {
                    PassedTotal += data[i].TotalPassedAudit;
                    dataPoint_Passed.push({ label: data[i].Installer, y: (parseInt(data[i].TotalPassedAudit) > 0 ? parseInt(data[i].TotalPassedAudit) : "0"), total: parseInt(data[i].TotalPassedAudit), color: "Green" });
                }
                var dataPoint_Failled = [];

                for (var i = 0; i <= data.length - 1; i++) {
                    FailedTotal += data[i].TotalFailedAudit;
                    dataPoint_Failled.push({ label: data[i].Installer, y: (parseInt(data[i].TotalFailedAudit) > 0 ? parseInt(data[i].TotalFailedAudit) : "0"), total: parseInt(data[i].TotalFailedAudit), color: "Red" });
                }

            }



            var chart = new CanvasJS.Chart("chartInstallerAuditChart", {
                //exportEnabled: true,
                exportFileName: CompletionvsPendingprojectname + " Audits Per Installer",
                zoomEnabled: true,
                theme: "theme1",
                animationEnabled: true,

                title: {
                    text: CompletionvsPendingprojectname + " Audit Per Installer",
                    fontSize: 20,
                    backgroundColor: "#FFFFE0",
                },
                toolTip: {
                    shared: true
                },
                axisY: {
                    title: "Audits",
                    minimum: 0
                    // includeZero: true
                },
                axisX: {
                    interval: 1,
                    labelAngle: 130,
                    title: "Installers"
                },
                legend: {
                    fontSize: 14,
                },
                data: [
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            indexLabelFontSize: 16,
                            lineThickness: 0,
                            color: "Orange",
                            type: "column",
                            name: "Total Audit Records",
                            legendText: "Total Audit Records- " + InstallationTotal,
                            showInLegend: true,
                            dataPoints: dataPoint_TotalInstallation
                        },
                        {
                            indexLabel: "{y}",
                            indexLabelPlacement: "inside",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            color: "Blue",
                            indexLabelFontSize: 16,
                            type: "column",
                            name: "Total Assigned Audit",
                            legendText: "Total Assigned Audit- " + AssignedTotal,
                            showInLegend: true,
                            dataPoints: dataPoint_TotalAssigned
                        },
                        {
                            indexLabel: "{y}",
                            visible: false,
                            lineThickness: 0,
                            indexLabelPlacement: "inside",
                            indexLabelBackgroundColor: "blue",
                            indexLabelOrientation: "vertical",
                            indexLabelFontStyle: "bold",
                            indexLabelFontColor: "black",
                            color: "Skyblue",
                            indexLabelFontSize: 16,
                            type: "column",
                            name: "Total Completed Audit",
                            legendText: "Total Completed Audit- " + CompleteTotal,
                            showInLegend: true,

                        },
                         {
                             indexLabel: "{y}",
                             indexLabelPlacement: "inside",
                             indexLabelBackgroundColor: "blue",
                             indexLabelOrientation: "vertical",
                             indexLabelFontStyle: "bold",
                             indexLabelFontColor: "black",
                             color: "Green",
                             indexLabelFontSize: 16,
                             type: "column",
                             name: "Total Passed Audit",
                             legendText: "Total Passed Audit- " + PassedTotal,
                             showInLegend: true,
                             dataPoints: dataPoint_Passed
                         },
                         {
                             indexLabel: "{y}",
                             indexLabelPlacement: "inside",
                             indexLabelBackgroundColor: "blue",
                             indexLabelOrientation: "vertical",
                             indexLabelFontStyle: "bold",
                             indexLabelFontColor: "black",
                             color: "Red",
                             indexLabelFontSize: 16,
                             type: "column",
                             name: "Total Failed Audit",
                             legendText: "Total Failed Audit- " + FailedTotal,
                             showInLegend: true,
                             dataPoints: dataPoint_Failled
                         },


                ],

            });
            chart.render();

        },
        error: function () {
            var $toast = toastr["error"]("something seems wrong", "Error Notification");
        }

    });


}


