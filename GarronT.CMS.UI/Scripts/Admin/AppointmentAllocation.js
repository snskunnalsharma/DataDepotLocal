/*
 * -------------------------------------------------------------------------------
 * Create By     : Sominath Zambare 
 * Created On    : 19-Dec-2016 
 * Description   : Appointment Allocation Javascript code
 *--------------------------------------------------------------------------------
 */


$(document).ready(function () {



    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');
    //#region Selected menu Active class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liAppointment');
    listate.addClass("active");




    $('#dynamic-table').DataTable({
        "language": {
            "emptyTable": "No Data To Show"
        }
    });

    //#endregion

    $("#ddlProjectList").chosen({ no_results_text: "Oops, nothing found!" });
    $("#InstallerDLL").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlUtilityType").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlAppointments").chosen({ no_results_text: "Oops, nothing found!" });





    //#region --Utility Type Select Function--
    $('#ddlUtilityType').change(function () {


        $('#ddlProjectList').empty().append($('<option></option>').val('0'));
        $('#ddlProjectList').trigger('chosen:updated');


        if ($('#ddlProjectStatus').val() != "0") {

            var GetProject = $('#urlGetProject').val();
            var _utility = $('#ddlUtilityType').val();



            $.ajax({
                type: 'GET',
                url: GetProject,
                async: false,
                data: { "utility": _utility },
                beforeSend: function () {
                    waitingDialog.show('Loading please wait...');
                },
                complete: function () {
                    waitingDialog.hide();
                },
                success: function (data) {
                    if (data.success == true) {
                        $.each(data.projectList, function (index, item) {

                            if (index == 0) {
                                $('#ddlProjectList').append(
                                $('<option></option>').val('0').html(""));
                                id = item.ProjectId;
                                name = item.ProjectCityState;
                            }

                            $('#ddlProjectList').append($('<option></option>').val(item.ProjectId).html(item.ProjectCityState));
                        });
                        $('.chosen-select').trigger('chosen:updated');
                    }
                    else {
                        var $toast = toastr["error"](data.returnMessage, "Error Notification");
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
                }

            });
        }

    });
    //#endregion


    //#region --Search Button Function--
    $('#btnSearch').click(function () {

        var _utilityType = $("#ddlUtilityType option:selected").val();
        var _projectID = $("#ddlProjectList option:selected").val();
        if (_utilityType == "0") {
            var $toast = toastr["error"]('Please select project utility first', 'Notification');
        }
        else if (_projectID == "0") {
            var $toast = toastr["error"]('Please select project first', 'Notification');
        }
        else {
            //alert("Call");
            LoadTable();
        }




    });
    //#endregion

    $("#btnSave").click(function () {

        var installer = $("#InstallerDLL option:selected").val();
        if (installer == '0') {

            var $toast = toastr["error"]('Please select Installer', 'Notification');
            return;
        }
        else {
            waitingDialog.show('Loading Please Wait...');
            //var InstallerMapModel = new Object();
            var AppointmentModel = new Object();
            AppointmentModel.AccountNumber = $("#lblAccount").text();
            AppointmentModel.ProjectName = $("#lblProjectName").text();
            AppointmentModel.UtilityType = $("#lblUtility").text();
            AppointmentModel.Route = $("#lblRoute").text();
            AppointmentModel.Cycle = $("#lblCycle").text();
            AppointmentModel.FirstName = $("#lblCustName").text();
            AppointmentModel.EmailAddress = $("#lblCustEmail").text();
            AppointmentModel.DayTimePhone = $("#lblCustPhone").text();
            AppointmentModel.Address = $("#lblAddress").text();
            AppointmentModel.FirstPreferedDate = $("#lblAppDate").text();
            AppointmentModel.FirstTime = $("#lblAppTime").text();
            AppointmentModel.FK_AppointmentID = $("#lblAppID").text();
            AppointmentModel.DateForApp = $("#lblAppDate").text();

            AppointmentModel.ProjectId = $("#ddlProjectList option:selected").val();
            AppointmentModel.InstallerID = $("#InstallerDLL option:selected").val();
            //InstallerMapModel.FK_UploadedExcelData_Id = $("#lblUploadedId").text();
            //InstallerMapModel.FKAppointmentID = $("#lblAppID").text();
            var UrlRecord = $("#UrlInsertRecord").val();

            $.ajax({

                url: UrlRecord,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(AppointmentModel),
                type: 'POST',
                success: function (data) {
                    //debugger;
                    if (data.msg == true) {

                        $("#myModal").modal('hide');
                        var $toast = toastr["success"](data.returnMsg, "Success Notification");
                        LoadTable();

                    }
                    else {
                        var $toast = toastr["error"](data.returnMsg, "Notification");
                    }
                    waitingDialog.hide();
                },
                error: function () {
                    waitingDialog.hide();
                    var $toast = toastr["error"](data.returnMsg, "Error Notification");
                }


            });

        }



    });

    //#region Cancel button on Create Model
    $("#btnCancel").click(function () {

        $("#myModal").modal('hide');
        CleareLables();
        //$("#form")[0].reset();



    });
    //#endregion





    //#region --Reset Button Function--
    $('#btnReset').click(function () {

        var url = $("#RedirctUrl").val();//Changed on 02/01/2017
        window.location.href = url;


    });
    //#endregion



    function LoadTable() {

        var PassedFlag = $("#ddlAppointments option:selected").val();
        // alert(PassedFlag);
        var _projectID = $("#ddlProjectList option:selected").val();
        var urlActiveRecords = $("#UrlGetProjectDetails").val();


        var urlViewRecords = $('#UrlViewRecords').val();
        $.ajax({
            url: urlActiveRecords,
            data: { "projectID": _projectID, "appointmentFlag": PassedFlag },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            type: 'GET',
            beforeSend: function () {
                waitingDialog.show('Loading please wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                if (data.success == true) {
                    oTable = $('#dynamic-table').dataTable(
                               {
                                   "bFilter": true,
                                   "bLengthChange": true,
                                   "bDestroy": true,
                                   "bserverSide": true,
                                   "aaSorting": [],
                                   "bDeferRender": true,
                                   //"sAjaxDataProp": 'aData',
                                   "oLanguage": {
                                       "sLengthMenu": '_MENU_ Records per page'
                                   },
                                   //"sAjaxSource": msg.result,
                                   "aaData": data.AppModelVM,
                                   "aoColumns": [

                                                      { "mData": "AccountNumber" },
                                                      { "mData": "Cycle" },
                                                      { "mData": "Route" },
                                                      { "mData": "Address" },
                                                      { "mData": "DateOnTable" },
                                                      { "mData": "FirstTime" },
                                                      { "mData": "InstallerName" },

                                                      { "bSearchable": false, "mData": null },
                                   ],
                                   "aoColumnDefs": [
                                                                          {
                                                                              "mRender": function (data, type, full, row) {
                                                                                  // alert(full.AppointmentID);
                                                                                  if (full.PassedDateFlag == 0) {
                                                                                      return '<a href="" data-backdrop="static" data-toggle="modal" id=' + full.AppointmentID + ' ' + 'onclick="GetInstallerList(' + full.AccountNumber + ',' + full.AppointmentID + ');" class="btn btn-primary btn-xs" role="button" alt="InstallerAllocation" title="Edit For Installer Allocation">' + '<i class="fa fa-pencil"></i></a>'

                                                                                  } else {
                                                                                      return '<a href="" data-backdrop="static" data-toggle="modal" id=' + full.AccountNumber + ' ' + 'onclick="ViewApppointment(' + full.AccountNumber + ',' + full.AppointmentID + ');" class="btn btn-primary btn-xs" role="button" alt="View" title="View">' + '<i class="fa fa-eye"></i></a>' +
                                                                                             '<a href="" data-backdrop="static" data-toggle="modal" id=' + full.AppointmentID + ' ' + 'onclick="RescheduleAppointment(' + full.AccountNumber + ',' + full.AppointmentID + ');" class="btn btn-primary btn-xs" role="button" alt="Appointment Reschedule" title="Appointment Reschedule">' + '<i class="fa fa-calendar"></i></a>'
                                                                                  }


                                                                              },
                                                                              "aTargets": [7]
                                                                          }
                                   ],

                               }
                                )//, $('.dataTables_filter input').attr('maxlength', 50);
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Error Notification");
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
            }
        });

    }


});

function RescheduleAppointment(Account, AppointmentID) {
    //debugger;
    var ProjectID = $("#ddlProjectList option:selected").val();
    var urlRescheduleApp = $("#urlCreateRescheduleAppUrl").val();

    $.ajax({
        type: 'GET',
        url: urlRescheduleApp,
        data: { projectId: ProjectID, account: Account, appointmentid: AppointmentID },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {

            //debugger;

            if (data.msg == true) {

                window.location.href = data.redirectToUrl;

            }
            else {
                //debugger;
                // $("#lbltextError").text(data.returnMessage);
                var $toast = toastr["error"](data.returnMessage, "Error Notification");

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


    //$("#myRescheduleAppointment").modal('show');
}
//Get installer list and upcomming  appointment record 
function GetInstallerList(Account, AppointmentID) {

    var PassedDateFlag = $("#ddlAppointments option:selected").val();
    $("#myModal").modal('hide');

    var ProjectID = $("#ddlProjectList option:selected").val();
    var UrlStr = $("#UrlInstallerList").val();

    $.ajax({
        type: 'GET',
        url: UrlStr,
        data: { appointmentid: AppointmentID },

        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            if (data.success === undefined) {
                CleareLables();
                $('#InstallerDLL').empty();
                //debugger;

                if (data.latlongPresent === true) {
                    var count = data.InstallerList.length;

                    if (count > 0) {
                        //debugger;
                        $.each(data.InstallerList, function (index, item) {
                            if (index == 0) {
                                $('#InstallerDLL').append(
                                  $('<option></option>').val('0'));

                            }

                            $('#InstallerDLL').append(
                                                  $('<option></option>').val(item.UserId).html(item.UserName)
                                            );
                        });

                        //debugger;
                        if (data.details.InstallerID != '') {
                            $('#InstallerDLL').val(data.details.InstallerID).attr("selected", "selected");

                        }
                        else {
                            $('#InstallerDLL').append(
                                  $('<option></option>').val('0'));
                        }
                        $('.chosen-select').trigger('chosen:updated');

                        $("#lblProjectName").text(data.details.ProjectName);
                        $("#lblUtility").text(data.details.UtilityType);
                        $("#lblCycle").text(data.details.Cycle);
                        $("#lblRoute").text(data.details.Route);
                        $("#lblAddress").text(data.details.Address);
                        $("#lblAccount").text(data.details.AccountNumber);
                        $("#lblCustName").text(data.details.FirstName);
                        $("#lblCustEmail").text(data.details.EmailAddress);
                        $("#lblCustPhone").text(data.details.DayTimePhone);
                        $("#lblAppID").text(data.details.AppointmentID);
                        $("#lblAppDate").text(data.details.DateOnTable);
                        $("#lblAppTime").text(data.details.FirstTime);
                        //$("#lblUploadedId").text(data.OtherDetails[0].ID).hide();
                        document.getElementById("lblUploadedId").style.visibility = "hidden";
                        document.getElementById("hiddenLbl").style.visibility = "hidden";
                        $("#lblInataller").text(data.details.InstallerName);

                        $("#myModal").modal('show');
                        $("#DDLInstallerListDIV").show();
                        $("#btnSave").show();
                    }



                }
                else {

                    $.confirm({
                        icon: 'fa fa-warning',
                        columnClass: 'col-md-6 col-md-offset-3',
                        closeIcon: true,
                        animationBounce: 2,
                        title: 'Operation can not complete',
                        content: 'Latitude & longitude not present for the current record. Please update latitude & longitude for the record & then try.',

                    });
                }
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }


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

//for passed appointments
function ViewApppointment(Account, AppointmentID) {
    //alert(AppointmentID);

    var PassedDateFlag = $("#ddlAppointments option:selected").val();
    var ProjectID = $("#ddlProjectList option:selected").val();
    var GetUrl = $("#UrlPassedAppointment").val();

    //debugger;

    $.ajax({
        type: 'GET',
        url: GetUrl,
        data: { appointmentid: AppointmentID },//data: { projectId: ProjectID, account: Account, appointmentid: AppointmentID },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            //debugger;
            if (data.success == undefined) {
                if (data.length != '') {
                    // $("#ModalPassedAppointment").modal('show');
                    CleareLables();
                    if (data.latlongPresent === true) {
                        //debugger;
                        if (data.details.InstallerName != '') {
                            $("#lblInataller").text(data.details.InstallerName);
                        }
                        $("#lblProjectName").text(data.details.ProjectName);
                        $("#lblUtility").text(data.details.UtilityType);
                        $("#lblCycle").text(data.details.Cycle);
                        $("#lblRoute").text(data.details.Route);
                        $("#lblAddress").text(data.details.Address);
                        $("#lblAccount").text(data.details.AccountNumber);
                        $("#lblCustName").text(data.details.FirstName);
                        $("#lblCustEmail").text(data.details.EmailAddress);
                        $("#lblCustPhone").text(data.details.DayTimePhone);
                        $("#lblAppID").text(data.details.AppointmentID);
                        $("#lblAppDate").text(data.details.DateOnTable);
                        $("#lblAppTime").text(data.details.FirstTime);
                        //$("#lblUploadedId").text(data.OtherDetails[0].ID).hide();
                        document.getElementById("lblUploadedId").style.visibility = "hidden";
                        document.getElementById("hiddenLbl").style.visibility = "hidden";
                        $("#lblInataller").text(data.details.InstallerName);
                        $("#myModal").modal('show');
                        $("#DDLInstallerListDIV").hide();
                        $("#btnSave").hide();

                        //$("#lblInataller2").text(data.installerName);
                        //$("#lblProjectName2").text(data.AppDetails[0].ProjectName);
                        //$("#lblUtility2").text(data.AppDetails[0].UtilityType);
                        //$("#lblCycle2").text(data.OtherDetails[0].Cycle);
                        //$("#lblRoute2").text(data.OtherDetails[0].Route);
                        //$("#lblAddress2").text(data.OtherDetails[0].Street);
                        //$("#lblAccount2").text(data.AppDetails[0].AccountNumber);
                        //$("#lblCustName2").text(data.AppDetails[0].FirstName);
                        //$("#lblCustEmail2").text(data.AppDetails[0].EmailAddress);
                        //$("#lblCustPhone2").text(data.AppDetails[0].DayTimePhone);
                        //$("#lblAppID2").text(data.AppDetails[0].AppointmentID);
                        //$("#lblAppDate2").text(data.AppDetails[0].DateOnTable);
                        //$("#lblAppTime2").text(data.AppDetails[0].FirstTime);
                        //// $("#lblUploadedId2").text(data.OtherDetails[0].ID).hide;
                        //document.getElementById("lblUploadedId2").style.visibility = "hidden";
                        //document.getElementById("hideLbl2").style.visibility = "hidden";

                    } else {

                        $.confirm({
                            icon: 'fa fa-warning',
                            columnClass: 'col-md-6 col-md-offset-3',
                            closeIcon: true,
                            animationBounce: 2,
                            title: 'Operation can not complete',
                            content: 'Latitude & longitude not present for the current record. Please update latitude & longitude for the record & then try.',

                        });
                    }
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Notification !");
                }


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

function CleareLables() {
    $("#lblProjectName").text('');
    $("#lblCycle").text('');
    $("#lblRoute").text('');
    $("#lblAddress").text('');
    $("#lblAccount").text('');
    $("#lblCustName").text('');
    $("#lblCustEmail").text('');
    $("#lblCustPhone").text('');
    $("#lblAppID").text('');
    $("#lblAppDate").text('');
    $("#lblAppTime").text('');
    $("#lblUploadedId").text('');
    $("#lblUtility").text('');
    $("#lblInataller").text('');
    $('#InstallerDLL').val("0").trigger('chosen:updated');



}


function copyToClipboard() {
    //debugger;
    // Create a "hidden" input
    var aux = document.createElement("input");
    // Assign it the value of the specified element
    aux.setAttribute("value", "Please sorry.... sorry...!");
    // Append it to the body
    document.body.appendChild(aux);
    // Highlight its content
    aux.select();
    // Copy the highlighted text
    document.execCommand("copy");
    // Remove it from the body
    document.body.removeChild(aux);
    // alert("Print screen desabled.");
}
