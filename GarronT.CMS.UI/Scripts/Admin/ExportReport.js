$(document).ready(function () {
    $("#ddlProjectStatus").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlformtype").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlform").chosen({ no_results_text: "Oops, nothing found!" });
    //$("#ddlVisitStatus").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlLatLong").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlMedia").chosen({ no_results_text: "Oops, nothing found!" });




    var actualDate = new Date(); // convert to actual date
    var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());

    $('#datepicker1').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,
        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onSelect: function (selectedDate) {
            //$("#datepicker1").datepicker("option", "minDate", selectedDate);
            var dt = new Date(selectedDate);
            dt.setDate(dt.getDate());
            $("#datepicker2").datepicker("option", "minDate", dt);

        }
    }).datepicker("setDate", actualDate);


    $('#datepicker2').datepicker({
        changeMonth: true,
        changeYear: true,
        defaultDate: actualDate,

        showButtonPanel: true,
        dateFormat: 'M-dd-yy',
        onSelect: function (selectedDate) {
            // $("#datepicker").datepicker("option", "maxDate", selectedDate);
            var dt = new Date(selectedDate);
            dt.setDate(dt.getDate());
            $("#datepicker1").datepicker("option", "maxDate", dt);
        }
    }).datepicker("setDate", actualDate);


    //getProjectList();

    //getformname();
    // getFormData();

    $("#Reports").addClass('active');
    $("#subReports").addClass('block');

    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liExportReport');
    listate.addClass("active");



    //#region visit status dropdown

    $("#ulVisitStatus").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;



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

           

        }
    });

    //#endregion
    var checkAdmin = document.querySelector('.classCheckAdmin').id;
    if (checkAdmin == 1) {             // 1 for admin
        $('#' + checkAdmin + '').show();
    }
    else {
        $('#' + checkAdmin + '').hide();
    }

});

var Bindproject;
var Bindproject1;

$("#btnSave").click(function () {
    //debugger;

    debugger;
    var Project1 = $("#ddlProject option:selected").val();
    var ddlForm = $("#ddlform option:selected").val();
    var ddlformType = $("#ddlformtype option:selected").val();
    //////////////////////////////////
    var GetField = $('#urlSaveFieldsNew').val();
    var arrFieldDetail = [];
    var arrFieldDetail1 = [];
    var arrFieldDetail2 = [];

    //get source fld records
    $('#sourceFields1 li').each(function () { arrFieldDetail.push({ value: this.value, text: this.innerText }); });

    //get destination fld records
    $('#destinationFields1 li').each(function () { arrFieldDetail1.push({ value: this.value, text: this.innerText }); });

    $('#lableFields li').each(function () { arrFieldDetail2.push({ value: this.value, text: this.innerText }); });

    if (Project1 == null || Project1 == "" || Project1 == "0") {
        var $toast = toastr["error"]("Please select the project.", "Notification");
        return;
    }
    else if (ddlformType == null || ddlformType == "" || ddlformType == "0") {
        var $toast = toastr["error"]("Please select the form type.", "Notification");
        return;
    }
    else if (arrFieldDetail1 == null || arrFieldDetail1.length == 0) {
        var $toast = toastr["error"]("Please move atleast 1 field in report field section & then try.", "Notification");
        return;
    }
    else {
        waitingDialog.show('Saving please wait...');
        debugger


        $.ajax({
            url: GetField,
            data: { arrFieldDetail: arrFieldDetail, arrFieldDetail1: arrFieldDetail1, arrFieldDetail2: arrFieldDetail2, formTypeId: ddlformType, projectId: Project1 },
            type: 'POST',
            dataType: 'json',
            success: function (msg) {
                if (msg.success) {
                    var $toast = toastr["success"]("Saved successfully.", "Success Notification");
                }
                else {
                    var $toast = toastr["error"]("something seems wrong", " Notification");
                }
                waitingDialog.hide();
                /*$('#lableFields').html('');
                $('#destinationFields1 li').each(function () {
                    $('#lableFields').append(
                        $('<li class="lableName" onclick="changeLable(this)"></li>').val(this.value).html(this.innerText)
                    );
                });*/
                $('#ddlformtype').trigger('change');
            },
            error: function () {
                alert("something seems wrong");
                waitingDialog.hide();
            }
        });
    }

});

$("#btnExport").click(function () {
    //debugger;
    var Formdata = $("#ddlformtype").val();
    var projectId = $("#ddlProject").val();
    var VisitStatus = $("#ddlVisitStatus").val();
    var _latlongRequired = $("#ddlLatLong").val();
    var _mediaRequired = $("#ddlMedia").val();
    var _startDate = $("#datepicker1").val();
    var _endDate = $("#datepicker2").val();


    var ddlCyclevalues = [];
    var cyclevalue = $('#ulVisitStatus li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });

   

    if (_latlongRequired == null || _latlongRequired == "0") {
        _latlongRequired = 1;
    }

    if (_mediaRequired == null || _mediaRequired == "0") {
        _mediaRequired = 1;
    }


    if ($('#ddlProjectStatus').val() == "0") {
        resetData();
    }
    var arrFieldDetail1 = [];
    $('#destinationFields1 li').each(function () { arrFieldDetail1.push({ value: this.value, text: this.innerText }); });

    if (Formdata == null || Formdata == "") {
        var $toast = toastr["error"]("Please set report fields first & then try.", "Notification");
        return;
    }
    else if (arrFieldDetail1 == null || arrFieldDetail1.length == 0) {
        var $toast = toastr["error"]("Please set report fields first & then try.", "Notification");
        return;
    }
    else if (ddlCyclevalues == null || ddlCyclevalues.length === 0) {
        var $toast = toastr["error"]("Please select visit status first & then try.", "Notification");
        return;
    }
    else {

       
        // long formId, int visitStatus, int latlongRequired, int mediaRequired
        debugger;
        waitingDialog.show('Loading Please Wait...');
        var GetField = $('#urlcreateExcelNew').val();
        var DownloadExcelExport = $('#urlDownloadExcelExport').val();
        $.ajax({
            url: GetField,
            data: {
                fromDate: _startDate,
                toDate: _endDate,
                formTypeId: Formdata,
                projectId: projectId,
                visitStatus: ddlCyclevalues,
                latlongRequired: _latlongRequired,
                mediaRequired: _mediaRequired
            },
            type: 'POST',
            dataType: 'json',
            success: function (msg) {

                if (msg.status == true) {


                    location.href = DownloadExcelExport;


                }
                else {
                    var $toast = toastr["error"](msg.ErrorMessage, "Notification");

                }
                waitingDialog.hide();
            },
            error: function () {
                alert("something seems wrong");
                waitingDialog.hide();
            }
        });

        // function JSONToCSVConvertor(JSONData, ) {


    }

});

$('#ddlProjectStatus').change(function () {
    debugger;
    if ($('#ddlProjectStatus').val() == "0") {
        resetData();
    }
    else {
        $('#ddlProject').empty();
        $('#ddlProject').trigger("chosen:updated");
        var GetProject = $('#urlGetProject').val();
        var _proStatus = $('#ddlProjectStatus').val();
        $.ajax({
            type: 'GET',
            url: GetProject,
            async: false,
            data: { "status": _proStatus },
            success: function (data) {
                $('#ddlProject').empty();

                var count = data.length;
                if (count > 0) {

                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlProject').append(
                              $('<option></option>').val('0'));
                        }
                        $('#ddlProject').append(
                                              $('<option></option>').val(item.ProjectId).html(item.ProjectCityState)
                                        );
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }

        });
    }

});

function resetData() {
    debugger;

    $('#sourceFields1').empty();
    $('#destinationFields1').empty();

    $(".source, .target").sortable({
        connectWith: ".connected",
        containment: ".sideBySide"
    });

    //getProjectList();
    $('#ddlformtype').empty();
    $('#ddlform').empty();
    $('#ddlformtype').trigger("chosen:updated");
    $('#ddlform').trigger("chosen:updated");
    //$('#ddlProjectStatus').empty();
    //$('#ddlProjectStatus').trigger("chosen:updated");
    //$('#ddlVisitStatus').empty();
    //$('#ddlVisitStatus').trigger("chosen:updated");

    $('#ddlVisitStatus').val('0').attr("selected", "selected");

    $('#ddlLatLong').val('0').attr("selected", "selected");
    $('#ddlMedia').val('0').attr("selected", "selected");

    $('#ddlVisitStatus').trigger("chosen:updated");
    $('#ddlLatLong').trigger("chosen:updated");
    $('#ddlMedia').trigger("chosen:updated");

    $('#ddlProject').empty();
    $('#ddlProject').trigger("chosen:updated");
}
$('#ddlProject').change(function () {

    var Project = $("#ddlProject option:selected")
    var fromtype = $("#ddlformtype option:selected")

    if (Project.length > 0) {
        var GetFormType = $('#urlGetFormType').val();
        $.ajax({
            type: 'GET',
            url: GetFormType,
            success: function (data) {
                $('#ddlformtype').empty();
                var count = data.length;
                if (count > 0) {
                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlformtype').append($('<option></option>').val('0'));
                        }
                        $('#ddlformtype').append($('<option></option>').val(item.ID).html(item.FormType));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                    $('#ddlform').empty();
                    $('#ddlform').trigger("chosen:updated");

                    $('#sourceFields1').empty();
                    $('#destinationFields1').empty();
                    $(".source, .target").sortable({
                        connectWith: ".connected",
                        containment: ".sideBySide"
                    });
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }
        });
    }
});

function changeLable(obj) {
    console.log('Do something with ');
    var liText = obj.innerText;
    var liVal = obj.value;
    console.log('testing == ' + liText);
    $("#myModalChangeLable").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
    $('#myModalChangeLable').on('shown.bs.modal', function () {
        $('#txtnewCustomFieldName').focus();
        $('#txtnewCustomFieldName').val(liText);
        $('#txtnewCustomFieldId').val(liVal);
    })
}
$("#btnChangeField").click(function () {
    debugger;
    $("#txtnewCustomFieldName").focus();
    if ($('#txtnewCustomFieldName').val() == '') {
        var $toast = toastr["error"]("Please enter the custom field name.", "Error Notification");
    }
    else {
        //waitingDialog.show('Please Wait...');
        $('#lableFields li').each(function () {
            //debugger;
            var liVal = $(this).val();
            var liId = $('#txtnewCustomFieldId').val();
            console.log(liVal + ',' + liId);
            if (liVal == liId) {
                //debugger;
                $(this).html($('#txtnewCustomFieldName').val());
            }
        });
        
        $("#myModalChangeLable").modal('hide');
        $("#txtnewCustomFieldId").val('');
        $("#txtnewCustomFieldName").val('');
    }

})
$("#btnChangeFieldCancel").click(function () {

    $("#txtnewCustomFieldName").val('');
    $("#txtnewCustomFieldId").val('');
    $("#myModalChangeLable").modal('hide');
})
$('#ddlformtype').change(function () {
   // debugger;

    $('#sourceFields1').empty();
    $('#destinationFields1').empty();
    $('#lableFields').empty();
    var Project = $("#ddlProject").val();
    var fromtype = $("#ddlformtype").val();

    if (Project.length > 0) {

        var urlGetFieldList = $('#urlGetFormTypeFields').val();

        $.ajax({
            type: 'GET',
            url: urlGetFieldList,
            data: { projectId: Project, formTypeId: fromtype },
            //async: true,
            beforeSend: function () {
                // waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (data) {
               // debugger;

                if (data != null) {
                    $.each(data, function (index, item) {
                        //debugger;
                        if (item.Flag == 0) {
                            $('#sourceFields1').append(
                                $('<li></li>').val(item.Id).html(item.FieldName)
                                            );
                        }
                        else {
                            $('#destinationFields1').append(
                                $('<li></li>').val(item.Id).html(item.FieldName)
                            );
                            $('#lableFields').append(
                                $('<li class="lableName" onclick="changeLable(this)"></li>').val(item.Id).html(item.ReportFieldName)
                            );
                        }
                    });


                    $('.txtfilterall').val("");
                    $('.chbSelectAll').prop('checked', false);

                    $('#ulVisitStatus').empty();
                    $("#VisitStatusHeader").removeClass("ddlHeader").text("Select Visit Status")



                    $('#ulVisitStatus').append($('<li></li>').val('5').addClass("liVisitStatus active-result").html('Completed'));
                    $('#ulVisitStatus').append($('<li></li>').val('1').addClass("liVisitStatus active-result").html('Not Allocated'));
                    $('#ulVisitStatus').append($('<li></li>').val('2').addClass("liVisitStatus active-result").html('Pending'));
                    $('#ulVisitStatus').append($('<li></li>').val('4').addClass("liVisitStatus active-result").html('RTU'));
                    $('#ulVisitStatus').append($('<li></li>').val('3').addClass("liVisitStatus active-result").html('Skip'));

                }


            },
            error: function (jqXHR, textStatus, errorThrown) {
                //  waitingDialog.hide();
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
    else {
        $('#sourceFields1').empty();
        $('#destinationFields1').empty();
    }
    $(".source, .target").sortable({
        connectWith: ".connected",
        containment: ".sideBySide"
    });
});

//#region open/close Cycle dropdown
$("#ancVisitStatus_chosen").click(function () {

    if ($("#ddlVisitStatus_chosen").hasClass('chosen-container-active')) {

        var selectedlivalue = $('#ulVisitStatus li.highlighted').map(function (i, el) { return $(el).val(); });
        var selectedlilength = selectedlivalue.length;

        if (selectedlilength == 0) {
            $("#VisitStatusHeader").removeClass("ddlHeader").text("Select Visit Status")
        } else {
            $("#VisitStatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
        }

        //#region SelectAll checked/unchecked
        if ($('#ulVisitStatus .liVisitStatus:visible').length > 0 && $('#ulVisitStatus .highlighted:visible').length == $('#ulVisitStatus .liVisitStatus:visible').length) {
            $('#ddlVisitStatus_SelectAll').prop('checked', true);
        } else {
            $('#ddlVisitStatus_SelectAll').prop('checked', false);
        }
        //#endregion
        //  $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');

        $("#ddlVisitStatus_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }
    else {
        $("#ddlVisitStatus_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }
});


function filterCycle(element) {

    var value = $(element).val().toLowerCase();

    $("#ulVisitStatus > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulVisitStatus .liVisitStatus:visible').length > 0 && $('#ulVisitStatus .highlighted:visible').length == $('#ulVisitStatus .liVisitStatus:visible').length) {
        $('#ddlVisitStatus_SelectAll').prop('checked', true);
    } else {
        $('#ddlVisitStatus_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#ddlVisitStatus_SelectAll').on('click', function () {
    debugger;
    if ($('#ulVisitStatus .liVisitStatus').length > 0) {

        if (this.checked) {
            $("li.liVisitStatus:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulVisitStatus .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#VisitStatusHeader").removeClass("ddlHeader").text("Select Visit Status")
            } else {
                $("#VisitStatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
            }
        }
        else {
            $("li.liVisitStatus:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulVisitStatus .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#VisitStatusHeader").removeClass("ddlHeader").text("Select Visit Status")
            } else {
                $("#VisitStatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
            }
        }


    }
});

$('body').on('click', function (e) {
    if (!$('#divVisitStatus').is(e.target)
       && $('#divVisitStatus').has(e.target).length === 0
   ) {

        var selectedlivalue = $('#ulVisitStatus li.highlighted').map(function (i, el) { return $(el).val(); });
        var selectedlilength = selectedlivalue.length;

        if (selectedlilength == 0) {
            $("#VisitStatusHeader").removeClass("ddlHeader").text("Select Visit Status")
        } else {
            $("#VisitStatusHeader").addClass("ddlHeader").text(selectedlilength + " Visit Status selected")
        }

        //#region SelectAll checked/unchecked
        if ($('#ulVisitStatus .liVisitStatus:visible').length > 0 && $('#ulVisitStatus .highlighted:visible').length == $('#ulVisitStatus .liVisitStatus:visible').length) {
            $('#ddlVisitStatus_SelectAll').prop('checked', true);
        } else {
            $('#ddlVisitStatus_SelectAll').prop('checked', false);
        }
        //#endregion
        $('#ddlVisitStatus_chosen').removeClass('chosen-with-drop chosen-container-active');
    }
});






$("#btnReset").click(function () {

    resetData();
    $('#ddlProjectStatus').val('0').attr("selected", "selected");
    $('#ddlProjectStatus').trigger("chosen:updated");
});




$('#ddlform').change(function () {

    $('#sourceFields1').empty();
    $('#destinationFields1').empty();
    var formIds = $("#ddlform option:selected")
    debugger
    if (formIds.length > 0) {

        var urlGetsourceField = $('#urlGetFormField').val();

        $.ajax({
            type: 'GET',
            url: urlGetsourceField,
            data: { formId: formIds.val() },
            async: true,
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            success: function (data) {
                debugger;
                //   var count = data.length;           
                if (data != null) {
                    $.each(data.objList, function (index, item) {
                        debugger;
                        if (item.Flag == 0) {
                            $('#sourceFields1').append(
                                                  $('<li></li>').val(item.Id).html(item.FieldName)
                                            );
                        }
                        else {
                            $('#destinationFields1').append(
                                                  $('<li></li>').val(item.Id).html(item.FieldName)
                                            );
                        }
                    });
                }
                //else {
                //    getalldata();
                //}
            },
        });

    }
    else {
        $('#sourceFields1').empty();
        $('#destinationFields1').empty();
    }

    $(".source, .target").sortable({
        connectWith: ".connected",
        containment: ".sideBySide"
    });
});




