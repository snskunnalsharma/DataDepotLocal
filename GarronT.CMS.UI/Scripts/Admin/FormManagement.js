var spinner, opts, target;

$(document).ready(function () {
   


    //#region Spin loading
    $("#loading").fadeIn();
    opts = {
        lines: 12, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        color: '#000', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false // Whether to use hardware acceleration
    };
    target = document.getElementById('loading');
    spinner = new Spinner(opts);
    //#endregion


    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }





    var urlgetRowPartial = $('#getPreviousRecordsPartial').val();
    $.ajax({
        url: urlgetRowPartial,
        type: "GET",
        async: false,
        success: function (data) {
            $("#divLandingPage").append(data);
        }
    });


    //#region ToRemove the left side selected class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liForm');
    listate.addClass("active");

    //#endregion

    bindActiveTable(true);

  
    var navListItems = $('div.setup-panel div a'),
                 allWells = $('.setup-content'),
                 allNextBtn = $('.nextBtn');
    

    $(document).on('mousedown', '.nextBtn', function (e) {
        debugger;
        var pId = $(this).parent().attr("id");
        var $container = $("#divCustmizedContainer");
        //if (e.which == 3 && pId == "destinationFields") //1: left, 2: middle, 3: right
        //{
        //    $("#myModal1").modal('show');
        //}
        //else
        if (e.which == 1 && pId == "destinationFields") {

            $("#childPartialDiv").empty();

            var urlgetRowPartial12 = $('#get_setField1Partial').val();
            $.ajax({
                url: urlgetRowPartial12,
                type: "GET",
                async: false,
                success: function (data) {
                    $("#childPartialDiv").append(data);
                }
            });
        }
        //else if (e.which == 1 && pId == "sourceFields" && $container.length == 1) {

        //    e.preventDefault();
        //    var $toast = toastr["error"]('Please add section first', 'Error Notification');


        //}

    })


    $(document).on('click', '#btnAddCustomized', function () {
        debugger;
        var $txtFormName = $("#txtNewFormName").val();
        // var $ddlProject = $("select[id='#ddlProject'] option:selected").index();
        var $ddlProject = $("#ddlProject").val();

        if ($ddlProject == -1) {
            var $toast = toastr["error"]('Please select project name', 'Error Notification');
        }
        else if ($txtFormName == '') {
            var $toast = toastr["error"]('Please enter the form name', 'Error Notification');
        }
        else {
            $("#txtDivName").val('');
            $("#txtsectionName").val('');
            $("#myModal").modal('show');
        }

    })

    $(document).on('click', '#btnCancelSection', function () {
        $("#txtDivName").val('');
        $("#txtsectionName").val('');
        $("#myModal").modal('hide');


    })

    $(document).on('click', '#btnResetSection', function () {
        $("#txtDivName").val('');
        $("#txtsectionName").val('');
        $("#txtsectionName").focus();

    })

    $(document).on('click', '#btnSaveSection', function () {
        debugger;

        var SpanName = $("#txtDivName").val();
        if (SpanName == '') {
            //var $container = $("#divCustmizedContainer");
            //$container.append('<div id="answerdiv' + $container.children().length + 1
            //    + '" class="alert-message"> <h4 class="modal-title">' + $("#txtsectionName").val() + '</h4></div>');
            //$("#myModal").modal('hide');

            var $container = $("#divCustmizedContainer");
            var _id = ($container.children().length + 1);
            var _idDiv = 'answerdiv' + _id;
            $container.append('<div id="answerdiv' + _idDiv
                + '" class="btn" style="width:100%" > <span id="' + _id + '" class="spanSectionName">' + $("#txtsectionName").val() + '</span>' +
                '<a href="#" class="btn btn-primary btn-xs section" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a></div>');
        }
        else {
            $('#' + SpanName).text($("#txtsectionName").val());
            $("#myModal").modal('hide');
        }
        $("#txtDivName").val('');
        $("#txtsectionName").val('');

    })
    var $sourceFields, $destinationFields, $chooser, navListItems
    $(document).on('change', '#ddlProject', function () {
        debugger;
        $('#sourceFields').empty();
        $('#destinationFields').empty();
        var $ddlProject = $("#ddlProject").val();

        if ($ddlProject > -1) {
            var urlgetFieldsForProject = $('#GetDynamicDivData').val();
            $.ajax({
                url: urlgetFieldsForProject,
                data: { ProjectId: $ddlProject },
                type: "GET",
                async: false,
                success: function (data) {
                    $("#sourceFields").append(data);
                }
            });
            $sourceFields = $("#sourceFields");
            $destinationFields = $("#destinationFields");
            $chooser = $("#fieldChooser").fieldChooser(sourceFields, destinationFields);
            navListItems = $('div.setup-panel div a'),
                allWells = $('.setup-content'),
                allNextBtn = $('.nextBtn');
            allWells.hide();

            var $container = $("#divCustmizedContainer");
            var _id = ($container.children().length + 1);
            var _idDiv = 'answerdiv' + _id;
            $container.append('<div id="answerdiv' + _idDiv
                + '" class="btn" style="width:100%" > <span id="' + _id + '" class="spanSectionName">Section 1</span>' +
                '<a href="#" class="btn btn-primary btn-xs section" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a></div>');

           
        }


        $(document).on('click', ".section", function () {
            debugger;
            var m = $("#" + $(this.closest('div')).attr('id')).children("span").attr('id');
            $("#txtDivName").val('');
            $("#txtsectionName").val('');
            $("#txtDivName").val(m);
            $("#txtsectionName").val($("#" + m).text());
            $("#myModal").modal('show');
        });

        $(document).on('click', navListItems, function (e) {

            //e.preventDefault();
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


    });




});



$(document).on('change', '#chkShowDeactivate', function () {

    if ($(this).is(":checked")) {

        bindActiveTable(false);
    }
    else {

        bindActiveTable(true);
    }
});



function bindActiveTable(checkBStatus) {
    spinner.spin(target);
    debugger;
    var urlActiveRecords = $("#GetFormListURL").val();

    $.ajax({
        url: urlActiveRecords,
        async: false,
        data: {
            "Status": checkBStatus,
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
                          { "mData": "City" },
                         { "mData": "ClientName" },
                          { "mData": "ProjectName" },
                            { "mData": "FormName" },
                          { "mData": "stringFromDate" },
                           { "mData": "stringToDate" },
                           { "bSearchable": true, "mData": null },
       ],
       "aoColumnDefs": [
                                              {
                                                  "mRender": function (data, type, full, row) {
                                                      if (checkBStatus)
                                                      { return '<a href="#" id=' + full.FormId + ' onclick="Edit(' + full.FormId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a> <a href="#confirmDeactivatedialog" data-backdrop="static" data-toggle="modal" id=' + full.FormId + ' onclick="Deactivate(' + full.FormId + ');" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>'; }
                                                      else
                                                      {
                                                          return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.FormId + ' onclick="Activate(' + full.FormId + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                                      }

                                                  },
                                                  "aTargets": [6]
                                              }
       ],

   }
), $('.dataTables_filter input').attr('maxlength', 50);
        }

    });
    spinner.stop(target);

}



function Activate(id) {
    debugger;
    $("#activationID").val(id);
    $("h4.modal-title").text("Activate Form");
}

function ActivateRecords(status, id) {
    debugger;
    var urlDeleteData = $('#A_Or_D_FormRecordURL').val();
    var uri = urlDeleteData;
    $.ajax({
        url: uri,
        async: false,
        data: {
            "status": status,
            "FormId": id
        },
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmActivatedialog").modal('hide');

                bindActiveTable((status == true ? false : true));
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);

        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
//#endregion

//#region Record Deactivate functions
function Deactivate(id) {

    $("#deactivationID").val(id);
    $("h4.modal-title").text("Deactivate Form");
}

function DeactivateRecords(id) {
    var urlDeleteData = $('#DeactivateCustomFieldRecordUrl').val();
    var uri = urlDeleteData + '?customFieldId=' + id + '&msg=' + "Deactivate";
    $.ajax({
        url: uri,
        type: 'POST',
        success: function (msg) {
            if (msg.success == true) {
                $("#confirmDeactivatedialog").modal('hide');
                bindActiveTable(true);
                var $toast = toastr["success"](msg.returnMessage, "Success Notification");
            }
            else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}


//#region Activate Record Yes button
$(document).on('click', '#btnActivateYes', function () {
    spinner.spin(target);
    var id = $("#activationID").val();
    ActivateRecords(true, id);
})
//#endregion

//#region Activate Record No button
$(document).on('click', '#btnActivatNo', function () {

    $("#confirmActivatedialog").modal('hide');
})
//#endregion


//#region Deactivate Record No button
$(document).on('click', '#btnDeactivatNo', function () {

    $("#confirmDeactivatedialog").modal('hide');
})
//#endregion

//#region Deactivate Record Yes button
$(document).on('click', '#btnDeactivateYes', function () {
    spinner.spin(target);
    $("#confirmDeactivatedialog").modal('hide');
    var id = $("#deactivationID").val();
    // DeactivateRecords(id);
    ActivateRecords(false, id);
})
//#endregion


function EditRecord() {
    debugger;




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
                spinner.stop(target);
                bindActiveTable(true);

                var $toast = toastr["success"](msg.returnMessage, "Success Notification");

            } else {

                var $toast = toastr["error"](msg.returnMessage, "Error Notification");
            }
            spinner.stop(target);


        },
        error: function () {
            alert("something seems wrong");
        }
    });
}


function Edit(ID) {
    debugger;
    NewData = "old";
    var urlGetData = $('#GetCustomFieldRecordUrl').val();
    $.ajax({
        type: 'GET',
        url: urlGetData,
        async: false,
        data: { "customFieldId": ID },
        success: function (msg) {
            debugger;
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
            alert("something seems wrong");
        }

    });


}

