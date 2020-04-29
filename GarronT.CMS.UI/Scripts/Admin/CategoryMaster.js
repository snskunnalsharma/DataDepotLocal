$(document).ready(function () {
    $("#InventoryManagement").addClass('active');
    $("#subInventoryManagement").addClass('block');

    $('.sub').find("*").removeClass('active');
    $('#liCategoryMaster').addClass("active");

    BindDataTable(true);

})

function BindDataTable(checkBStatus) {

    var urlBindTblData = $("#urlGetAllData").val();

    $.ajax({
        url: urlBindTblData,
        contentType: "application/json; charset=utf-8",
        data: { "Status": checkBStatus },
        dataType: "json",
        type: 'GET',
        success: function (data) {

            oTable = $('#dynamic-table').dataTable(
            {
                "bDestroy": true,
                "bserverSide": true,
                "bDeferRender": true,
                "oLanguage": {
                    "sLengthMenu": '_MENU_ Records per page'
                },

                "aaData": data.objList,
                "aoColumns": [
                               { "mData": "CategoryName" },
                               { "bSearchable": false, "mData": null },
                ],
                "aoColumnDefs": [
                                {
                                    "mRender": function (data, type, full, row) {
                                        if (checkBStatus) {
                                            return '<a id="' + full.CategoryId + '" onclick="EditRecord(' + full.CategoryId + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>' +
                                                '<a data-backdrop="static" data-toggle="modal" id=' + full.CategoryId + ' onclick="Deactivate(' + full.CategoryId + ' );" class="btn btn-danger btn-xs" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban"></i></a>';
                                        }
                                        else {
                                            return '<a data-toggle="modal" data-backdrop="static" id=' + full.CategoryId + ' onclick="Activate(' + full.CategoryId + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                        }
                                    },
                                    "aTargets": [1]
                                }
                ],

            }
        ), $('.dataTables_filter input').attr('maxlength', 50);

        }
    });



}



$('#chkShowDeactivate').change(function () {

    if ($(this).is(":checked")) {

        BindDataTable(false);
    }
    else {

        BindDataTable(true);
    }
});



$('#btnAddCategory').click(function () {
    // debugger;
    var urlCreateNewRecord = $('#urlCreateRecord').val();

    $.ajax({
        url: urlCreateNewRecord,
        datatype: "text",
        type: "GET",
        success: function (data) {
            //debugger;
            if (data.success === undefined) {
                $('#AddEditModel').html(data);
                fnValidateDynamicContent("#AddEditModel")

                $("#AddEditModel").modal({ show: true, backdrop: 'static', keyboard: false });
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }

        },
        error: function (request, status, error) {

            var $toast = toastr["error"](request.responseText, "Notification !");

            console.log(request.responseText)

        }
    });

})


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





$(document).on("click", '#btnSave', function () {

    // debugger;
    var form = $("#formAddEdit");

    form.validate();

    if (form.valid()) {

        var formData = $('#formAddEdit').serializeArray();

        var FormObject = {};

        $(formData).each(function (index, obj) { FormObject[obj.name] = obj.value; });


        if ($('#CategoryId').val() == "0") {

            var urlCreateNewRecord = $('#urlCreateRecord').val();
            $.ajax({
                url: urlCreateNewRecord,
                datatype: "text",
                type: "POST",
                data: { 'objCategoryBO': FormObject },
                success: function (data) {
                    //debugger;
                    if (data.statusMessage.Success) {

                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
                        var $toast = toastr["error"](data.statusMessage.Message, "Notification !");
                    }

                },
                error: function (request, status, error) {
                    var $toast = toastr["error"](request.responseText, "Notification !");
                    console.log(request.responseText)
                }
            });
        }
        else {

            var urlEditRecord = $('#urlEdit').val();
            $.ajax({
                url: urlEditRecord,
                datatype: "text",
                type: "POST",
                data: { 'objCategoryBO': FormObject },
                success: function (data) {
                    //debugger;
                    if (data.statusMessage.Success) {
                        var $toast = toastr["success"](data.statusMessage.Message, "Notification !");
                        $('#AddEditModel').modal('hide');
                        BindDataTable(true);
                    }
                    else {
                        $('#RoleId').prop('disabled', true);
                        var $toast = toastr["error"](data.statusMessage.Message, "Notification !");
                    }


                },
                error: function (request, status, error) {
                    var $toast = toastr["error"](request.responseText, "Notification !");
                    console.log(request.responseText)
                }
            });
        }
    }

})




$(document).on("click", '#btnCancel', function () {

    $("#AddEditModel").modal('hide');

})



function EditRecord(id) {
    //debugger;

    var urlEditRecord = $("#urlEdit").val();

    $.ajax({
        type: "GET",
        url: urlEditRecord,
        data: { 'Id': id },
        datatype: "text",
        success: function (data) {
            // debugger;

            if (data.success === undefined) {
                $('#AddEditModel').html(data);
                fnValidateDynamicContent("#AddEditModel")


                $("#AddEditModel").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });
            }
            else {
                var $toast = toastr["error"](data.returnMessage, "Notification !");
            }

        },
        error: function (request, status, error) {
            var $toast = toastr["error"](request.responseText, "Notification !");
            console.log(request.responseText)
        }
    });

}








//#region Activate record
function Activate(id) {
    //debugger;

    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Activate Category!',
        content: 'Do you want to activate the current category?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        ActiveDeactivateFunction(id, true)
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


function Deactivate(id) {
    //debugger;


    $.confirm({
        icon: 'fa fa-warning',
        columnClass: 'col-md-6 col-md-offset-3',
        closeIcon: true,
        animationBounce: 2,
        title: 'Deactivate Category!',
        content: 'Do you want to deactivate the current Category?',
        buttons: {
            Yes:
                {
                    text: 'Yes', // 
                    btnClass: 'btn-danger',
                    action: function () {

                        ActiveDeactivateFunction(id, false)
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


// Active deactive the individual customer
function ActiveDeactivateFunction(id, makeActive) {

    var urlActivateRecord = $("#urlActiveDeactive").val();

    $.ajax({
        type: "GET",
        url: urlActivateRecord,
        contentType: "application/json; charset=utf-8",
        data: { 'Id': id, 'Status': makeActive },
        dataType: "json",
        async: true,

        success: function (data) {
            //debugger;

            if (data.statusMessage.Success) {
                var $toast = toastr["success"](data.statusMessage.Message, "Notification !");

            }
            else {
                var $toast = toastr["error"](data.statusMessage.Message, "Notification !");

            }
            if ($("#chkShowDeactivate").is(":checked")) {

                BindDataTable(false);
            }
            else {

                BindDataTable(true);
            }
        },
        error: function (request, status, error) {
            var $toast = toastr["error"](request.responseText, "Notification !");
            console.log(request.responseText)
        }
    });
}

//#