//

$(document).ready(function () {

    $("#FormManagement").addClass('active').css('background-color', '#EF6C00').css('border-radius', '6px');
    $("#FormManagement").children().css('color', '#fff');
});
//'use strict';

//setTimeout(function () { $scope.$apply(); });


var oTable, spinner, opts, target;
var uniqueSectionID;
var recordaddEdit = "Add";
var recordView = false;
var EditID = 0;
var CloneID = 0;
var sourcefields;
var recordsubmitted = false;
var projectActive = true;
var projectDateexpire = false;
var prjFromDate;
var prjToDate;
var OptionFieldList = [];

function checkformsubmittedbyinstaller(formid) {
    var urlCheckFormUsed = $("#urlCheckFormUsed").val();

    $.ajax({
        url: urlCheckFormUsed,
        async: false,
        data: {
            "FormID": formid,
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        type: 'GET',
        success: function (msg) {
            recordsubmitted = msg.recordexist;
            projectActive = msg.projectActive;
            projectDateexpire = msg.projectDateexpire;
            if (projectDateexpire == false) {
                projectActive = false;
            }

            if (recordsubmitted == true || projectActive == false || projectDateexpire == false) {
                recordView = true;

            }

        }

    });
}

function CheckProjectActive(formid) {
    var urlCheckFormUsed = $("#urlCheckFormUsed").val();

    $.ajax({
        url: urlCheckFormUsed,
        async: false,
        data: {
            "FormID": formid,
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        type: 'GET',
        success: function (msg) {

            recordsubmitted = msg.recordexist;
            projectActive = msg.projectActive;
            if (msg.recordexist == true || msg.projectActive == true) {
                recordView = true;
            }

        }

    });
}


angularApp
    //.controller('FormManagementCtrl', function ($scope, ngDialog, $compile, $dialog, $rootScope, $location) {
    .controller('FormManagementCtrl', ['$scope', 'ngDialog', '$compile', '$dialog', '$rootScope', '$location', function ($scope, ngDialog, $compile, $dialog, $rootScope, $location) {



        recordaddEdit = "Add";
        $scope.AddFormUrl = $rootScope.projectPath + "FormManagement/Add";

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

        //#region toaster
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
        //#endregion

        //#region Active datatable

        bindActiveTable(true);

        $(document).on('change', '#chkShowDeactivate', function () {

            if ($(this).is(":checked")) {

                bindActiveTable(false);
            }
            else {

                bindActiveTable(true);
            }
        });








        $(document).on('click', 'a[name="editform"]', function () {
            //waitingDialog.show('Loading Please Wait...');

            recordaddEdit = "Edit";
            recordView = false;
            EditID = this.id;

            checkformsubmittedbyinstaller(EditID)

            //#region Check existing records

            if (recordsubmitted == true) {
                ngDialog.openConfirm({
                    template: '\
                <p>This form is submitted by installer, you can not update it.</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">Cancel</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">View</button>\
                </div>',
                    plain: true,
                    showClose: false,
                    closeByDocument: false,
                    closeByEscape: false
                }).then(function (confirm) {
                    recordView = true;
                    var indexpath = $rootScope.projectPath + "FormManagement/View/" + EditID;

                    $location.path(indexpath);

                }, function (reject) {


                    return false;

                });

            }
            else if (projectActive == false) {

                ngDialog.openConfirm({
                    template: '\
                <p>Project of this form is deactivated, you can not update it.</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">Cancel</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">View</button>\
                </div>',
                    plain: true,
                    showClose: false,
                    closeByDocument: false,
                    closeByEscape: false
                }).then(function (confirm) {
                    recordView = true;
                    var indexpath = $rootScope.projectPath + "FormManagement/View/" + EditID;

                    $location.path(indexpath);

                }, function (reject) {


                    return false;

                });

            }
            else if (projectDateexpire == false) {
                ngDialog.openConfirm({
                    template: '\
                <p>Project expire, you can not update it.</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">Cancel</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">View</button>\
                </div>',
                    plain: true,
                    showClose: false,
                    closeByDocument: false,
                    closeByEscape: false
                }).then(function (confirm) {
                    projectActive = false;
                    recordView = true;
                    var indexpath = $rootScope.projectPath + "FormManagement/View/" + EditID;

                    $location.path(indexpath);

                }, function (reject) {


                    return false;

                });


            }
            else {
                var indexpath = $rootScope.projectPath + "FormManagement/Edit/" + EditID;
                $scope.$apply(function () {
                    $location.path(indexpath);
                });
            }

            //#endregion

        });


        $(document).on('click', 'a[name="viewform"]', function () {

            recordaddEdit = "Edit";
            recordView = true;
            EditID = this.id;
            var indexpath = $rootScope.projectPath + "FormManagement/View/" + EditID;
            $scope.$apply(function () {
                $location.path(indexpath);
            });
        });

        $(document).on('click', 'a[name="cloneform"]', function () {
            //waitingDialog.show('Loading Please Wait...');

            recordaddEdit = "Clone";
            CloneID = this.id;

            var indexpath = $rootScope.projectPath + "FormManagement/Clone";
            $scope.$apply(function () {
                $location.path(indexpath);
            });


        });

        $(document).on('click', 'a[name="deleteform"]', function () {
            var DeleteID = this.id;
            ngDialog.openConfirm({
                template: '\
                <p>Are you sure you want to deactivate record?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                plain: true,
                showClose: false,
                closeByDocument: false,
                closeByEscape: false
            }).then(function (confirm) {

                waitingDialog.show('Deactivating record...');

                var urlDeactivateRecords = $("#urlDeactivateRecords").val();

                $.ajax({
                    url: urlDeactivateRecords,
                    async: true,
                    data: {
                        "DeactivateID": DeleteID,
                    },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    type: 'GET',
                    success: function (data) {
                        if (data.success == true) {

                            var $toast = toastr["success"](data.returnMessage, "Success Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                            bindActiveTable(true);
                        } else {

                            var $toast = toastr["error"](data.returnMessage, "Error Notification");
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

            }, function (reject) {

                return false;

            });

        });


        $(document).on('click', 'a[name="activateform"]', function () {
            var ActivateidID = this.id;
            ngDialog.openConfirm({
                template: '\
                <p>Are you sure you want to activate record?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                plain: true,
                showClose: false,
                closeByDocument: false,
                closeByEscape: false
            }).then(function (confirm) {
                waitingDialog.show('Activating record...');

                var urlActivateRecords = $("#urlActivateRecords").val();

                $.ajax({
                    url: urlActivateRecords,
                    async: true,
                    data: {
                        "ActivateID": ActivateidID,
                    },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    //  data: JSON.stringify(WidgetViewModel),
                    type: 'GET',
                    success: function (data) {
                        if (data.success == true) {

                            var $toast = toastr["success"](data.returnMessage, "Success Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                            bindActiveTable(false);
                        } else {

                            var $toast = toastr["error"](data.returnMessage, "Error Notification");
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

            }, function (reject) {

                return false;

            });


        });

        function bindActiveTable(checkBStatus) {
            spinner.spin(target);
            //   waitingDialog.show('Loading record...');
            var urlActiveRecords = $("#GetFormListURL").val();
            //var urlActiveRecords = "/Admin/Form/GetFormList";
            $.ajax({
                url: urlActiveRecords,
                async: true,
                data: {
                    "Status": checkBStatus,
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                //  data: JSON.stringify(WidgetViewModel),
                type: 'GET',
                success: function (msg) {
                    //   waitingDialog.hide();
                    oTable = $('#dynamic-table').dataTable(
                        {
                            "bAutoWidth": false,
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

                                { "mData": "ClientName" },
                                { "mData": "ProjectName" },
                                { "mData": "FormName" },
                                { "mData": "City" },
                                { "mData": "stringFromDate" },
                                { "mData": "stringToDate" },
                                { "bSearchable": true, "mData": null },
                            ],
                            "aoColumnDefs": [
                                {
                                    "mRender": function (data, type, full, row) {
                                        if (checkBStatus) {
                                            return '<a href="#" name="editform" id=' + full.FormId + '  class=" btn btn-success btn-xs editform rep_btn_danger" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a>'
                                                + '<a href="#" name="viewform" id=' + full.FormId + '  class=" btn btn-primary btn-xs viewform rep_btn_danger" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a>'
                                                + '<a href="#" name="cloneform" id=' + full.FormId + '  class="btn btn-info btn-xs cloneform rep_btn_danger" role="button" alt="Clone" title="Clone"><i class="fa fa-files-o"></i></a>'
                                                + '<a href="#" name="deleteform" id=' + full.FormId + '  class="btn btn-danger btn-xs deleteform rep_btn_danger" role="button" alt="Deactivate" title="Deactivate"><i class="fa fa-ban "></i></a>';

                                        }
                                        else {
                                            return '<a href="#" name="activateform" id=' + full.FormId + '  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
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

        //#endregion


    }]);

angularApp
    //.controller('FormAddctrl', function ($scope, $timeout, ngDialog, $compile, $dialog, $location, $routeParams, $rootScope, $route) {
    .controller('FormAddctrl', ['$scope', '$timeout', 'ngDialog', '$compile', '$dialog', '$location', '$routeParams', '$rootScope', '$route', function ($scope, $timeout, ngDialog, $compile, $dialog, $location, $routeParams, $rootScope, $route) {
        var OptionsList = [];
        var uniqueOptionID;
        var selectedSection = "";
        var _selectedFieldText = "";
        var _CurrentSelectedSectionName = "";
        var _selectedSection = "";
        var _ProjectList = "";
        var _ExistedProjectSourceFields;

        debugger;
        var paramValue = $route.current.$$route.paramExample;
        var ActionName="ADD";

        if (paramValue == "EditParam") {
            ActionName = "EDIT";
            
        }
        else if (paramValue == "ViewParam") {
            ActionName = "VIEW";
            
        }
        else if (paramValue == "CloneParam") {
            ActionName = "CLONE";
            
        }
        else {

        }

        CheckPermission();

        function CheckPermission() {
            var urlCheckUserPermission = $('#urlCheckUserPermission').val();
            $.ajax({
                type: 'GET',
                data: { "straction": ActionName },
                async: false,
                url: urlCheckUserPermission,
                success: function (data) {
                    debugger;
                    if (data.success === false) {
                        var $toast = toastr["error"](data.returnMessage, "Notification !");

                        var indexpath = $rootScope.projectPath + "FormManagement";

                        $location.path(indexpath);
                        return false;
                    }
                   
                },

            });
        }

        $('#lblnote').hide()
        var callback = function () {

            // $('.chosen-container.chosen-container-single').innerWidth("100%");
            // $('.chosen-with-drop').css({ minWidth: '100%', width: '' });
        };

        $(document).ready(callback);

        $(window).resize(callback);//responsive width


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

        //#region toaster
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
        //#endregion


        try {
            $("#ddlProject").chosen({
                no_results_text: "Oops, nothing found!",
                // width: "95%"
            });
            $("#ddlExistedProject").chosen({ no_results_text: "Oops, nothing found!" });
            $("#ddlExistedProjectForm").chosen({ no_results_text: "Oops, nothing found!" });
            $("#ddlFormType").chosen({ no_results_text: "Oops, nothing found!", width: 155 });

            function GetFormTypeList() {

                var urlGetFormTypeList = $('#urlGetFormTypeList').val();
                $.ajax({
                    type: 'GET',
                    // data: {},
                    async: false,
                    url: urlGetFormTypeList,
                    success: function (data) {

                        $('#ddlFormType').empty();
                        $.each(data, function (index, item) {
                            if (index == 0) {
                                $('#ddlFormType').append(
                                    $('<option></option>').val('').html(""));
                            }

                            $('#ddlFormType').append(
                                $('<option></option>').val(item.ID).html(item.FormType)
                            );
                        });

                        $('.chosen-select').trigger('chosen:updated');
                    },

                });

            }

            GetFormTypeList()

          function disableallelements() {

                $("#datatypediv *").attr("disabled", "disabled").off('click');
            }

            //$('#ddlProject').multiselect({
            //    includeSelectAllOption: false,
            //    enableFiltering: true,
            //    enableCaseInsensitiveFiltering: true
            //});
            //$('#ddlExistedProject').multiselect({
            //    includeSelectAllOption: false,
            //    enableFiltering: true,
            //    enableCaseInsensitiveFiltering: true
            //});
            //$('#ddlExistedProjectForm').multiselect({
            //    includeSelectAllOption: false,
            //    enableFiltering: true,
            //    enableCaseInsensitiveFiltering: true
            // });


            var validator = $('#FormManagementForm').validate({
                debug: true, onsubmit: false,
                rules: {
                    ddlProjectdropdown: {
                        required: true,
                    },
                    NewFormName: {
                        required: true,
                    },
                    FromDate: {
                        required: true,
                    },
                    ToDate: {
                        required: true,
                    }
                },
                messages: {
                    ddlProjectdropdown: {
                        required: "Select Project",
                    },
                    NewFormName: {
                        required: "Enter Form Name",
                    },
                    FromDate: {
                        required: "Select Start Date",
                    },
                    ToDate: {
                        required: "Select End Date",
                    }
                },
                //errorElement: 'label',
                errorClass: 'error',
                highlight: function (element) {
                    return false;
                },
                unhighlight: function (element) {
                    return false;
                },
                submitHandler: function (form) {

                    //EditRecord();
                    //$("#element_to_pop_up").dialog("close");
                    //return false;
                },
                invalidHandler: function () {

                }
            });

            var validator = $('#SetFieldForm').validate({
                debug: true, onsubmit: false,
                rules: {
                    fieldtitle: {
                        required: true,
                    },

                },
                messages: {
                    fieldtitle: {
                        required: "Enter Field Title",
                    },

                },
                //errorElement: 'label',
                errorClass: 'error',
                highlight: function (element) {
                    return false;
                },
                unhighlight: function (element) {
                    return false;
                },
                submitHandler: function (form) {

                    //EditRecord();
                    //$("#element_to_pop_up").dialog("close");
                    //return false;
                },
                invalidHandler: function () {

                }
            });

            function ResetFunction() {

                $("#ddlExistedProject").empty()
                $("#ddlExistedProjectForm").empty()

                $('.chosen-select').trigger('chosen:updated');

                $('#FormManagementForm').validate().resetForm();




                $("#txtFromDate").val("");
                $("#txtToDate").val("");

                $("#sortable1").empty();
                $("#sortable2").empty();
                $("#hdnFormID").val("");


                //$timeout(function () {
                //    $scope.$apply(function () {
                $scope.SetFieldAttr = false;
                // $("#SetFieldAttr").hide();

                $scope.SetFieldddl = false;
                //$scope.selected = 0;
                uniqueSectionID = 1;
                uniqueOptionID = 1;
                $scope.sectionList = [];
                $scope.SourceFieldList = [];

                $scope.field = {};

                $scope.FinalFormField = [];
                $scope.NewFormName = "";
                $('#txtNewFormName').val(""),

                //$("#currentfieldsection").text("Sections");
                    $scope.FieldForCurrentSection = "Current ";
                $scope.SetFieldFor = "";

                $scope.ProjectForms = []
                selectedProject = "0";
                OptionsList = [];
                $scope.AllOptionList = [];

                $scope.DivListOptionShow = false;

                //    });
                //});
            }

            function ResetFormChangeFunction() {


                $("#sortable1").empty();
                $("#sortable2").empty();
                $("#hdnFormID").val("");

                //$timeout(function () {
                //    $scope.$apply(function () {
                $scope.SetFieldAttr = false;
                // $("#SetFieldAttr").hide();

                $scope.SetFieldddl = false;
                //$scope.selected = 0;
                uniqueSectionID = 1;
                uniqueOptionID = 1;
                $scope.sectionList = [];

                $scope.sectionList = [];

                $scope.SourceFieldList = [];

                $scope.field = {};

                $scope.FinalFormField = [];


                //$("#currentfieldsection").text("Sections");
                $scope.FieldForCurrentSection = "Current ";
                $scope.SetFieldFor = "";

                $scope.ProjectForms = []

                OptionsList = [];
                $scope.AllOptionList = [];

                $scope.DivListOptionShow = false;

                //    });
                //});
            }

            function GetProjectList() {

                var urlGetProjectList = $('#urlGetProjectList').val();
                // var urlGetProjectList = "/Admin/Form/GetProjectList";
                $.ajax({
                    type: 'GET',
                    async: false,
                    data: { Activeproject: projectActive },
                    url: urlGetProjectList,
                    success: function (data) {


                        _ProjectList = data;

                        $('#ddlProject').empty();
                        $('#ddlProject').append(
                            $('<option></option>').val(''))
                        //.attr("selected", "selected").prop("disabled", true).val('0').html("Select Project"));

                        //$('#ddlProject').append(
                        //        $('<option></option>').attr("selected", "selected").prop("disabled", true).val('0').html("--Select Project--"));
                        $.each(data, function (index, item) {

                            $('#ddlProject').append(
                                $('<option></option>').val(item.ProjectId).html(item.ProjectCityState)
                            );
                        });
                        // $("#ddlProject").multiselect('rebuild');
                        $('.chosen-select').trigger('chosen:updated');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        waitingDialog.hide();
                        if (status === 306) {
                            location.reload();
                        }
                        var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                        console.log('jqXHR:');
                        console.log(jqXHR);
                        console.log('textStatus:');
                        console.log(textStatus);
                        console.log('errorThrown:');
                        console.log(errorThrown);
                    },
                });
                $('#ddlProject').find('option:first').attr('selected', 'selected');
            }

            function BindDates() {
                var actualDate = new Date();
                var newDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());
                $('#txtFromDate').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: actualDate,
                    showButtonPanel: true,
                    dateFormat: 'M-dd-yy',
                    onSelect: function () {

                        $(this).change();
                        var date = $("#txtFromDate").val();
                        if ($.trim(date) != "") {
                            var newDate1 = new Date(date);

                            // $("#txtFromDate").val($.datepicker.formatDate('dd-M-yy', newDate));
                            $("#txtToDate").datepicker('option', 'minDate', newDate1);


                        }
                        // GetVendor()
                    },
                }).datepicker("setDate", actualDate);


                $('#txtToDate').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: actualDate,
                    showButtonPanel: true,
                    dateFormat: 'M-dd-yy',
                }).datepicker("setDate", newDate);

            }

            function GetSourcefieldsbyProjectID(projectid) {

                $scope.SourceFieldList = [];
                var IsAuditor = "";
                var formtype = $('#ddlFormType').val();
                if (formtype == '1') {
                    IsAuditor = "true";
                }
                else {
                    IsAuditor = "false";
                }


                if (projectid != "0") {

                    var urlgetFieldsForProject = $("#GetGetDynamicDivData").val();

                    $.ajax({
                        url: urlgetFieldsForProject,
                        data: { ProjectId: projectid, isAuditor: IsAuditor },
                        type: "GET",
                        async: false,
                        success: function (data) {

                            sourcefields = data;
                            if (recordaddEdit == "Add") {
                                $("#sortable1").empty();
                                for (var i = 0; i < data.length; i++) {
                                    var sourceli = "";
                                    if (IsAuditor == "true" && (data[i].FieldName == "NEW METER NUMBER" || data[i].FieldName == "NEW METER RADIO")) {
                                        sourceli = " <li lang='1' id='" + data[i].Id + "' class='ui-state-default'>" + data[i].FieldName + "</li>"
                                    }
                                    else {
                                        sourceli = " <li lang='" + data[i].isMappedToExcel + "' id='" + data[i].Id + "' class='ui-state-default'>" + data[i].FieldName + "</li>"
                                    }
                                    $("#sortable1").append(sourceli)
                                }
                            }



                        }
                    });

                }
            }

            function GetDataTypeDropdownData() {

                var urlGetDataTypeMaster = $('#GetDataTypeMaster').val();
                $.ajax({
                    url: urlGetDataTypeMaster,

                    type: "GET",
                    async: false,
                    success: function (DataTypeData) {
                        $scope.DataTypeddlData = DataTypeData;

                    }
                });


            }


            function SelectFirstSection() {


                if ($scope.sectionList.length > 0) {
                    $timeout(function () {
                        angular.element("#FormSectionTable").children().children().first().triggerHandler('click');
                    }, 0);
                }
            }

            function comparedestiField(a, b) {


                if (a.fieldorder < b.fieldorder)
                    return -1;
                else if (a.fieldorder > b.fieldorder)
                    return 1;
                else
                    return 0;
            }

            function compareSectionList(a, b) {
                if (a.SectionOrder < b.SectionOrder)
                    return -1;
                else if (a.SectionOrder > b.SectionOrder)
                    return 1;
                else
                    return 0;
            }

            function compareOptionList(a, b) {
                if (a.OrderNo < b.OrderNo)
                    return -1;
                else if (a.OrderNo > b.OrderNo)
                    return 1;
                else
                    return 0;
            }

            if ($routeParams.param != undefined && paramValue == "EditParam") {

                checkformsubmittedbyinstaller($routeParams.param)

                recordaddEdit = "Edit";
            }

            if ($routeParams.param != undefined && paramValue == "ViewParam") {

                checkformsubmittedbyinstaller($routeParams.param)

                recordaddEdit = "Edit";
            }

            if (paramValue == "CloneParam" && recordaddEdit != "Clone") {
                var indexpath = $rootScope.projectPath + "FormManagement";
                $location.path(indexpath);
            }

            else if (recordaddEdit == "Edit" && (paramValue == "EditParam" || paramValue == "ViewParam")) {

                ResetFunction()
                GetProjectList();
                BindDates()
                $("#btnSave").text("Update");
                $("#btnReset").hide();
                $("#DivExistedProject").hide();
                var recordfrom = "Edit";
                var ddlProject = "0";
                // waitingDialog.show('Loading...');
                var urlgetRecordsforEdit = $("#urlgetRecordsforEdit").val();

                var editid = $routeParams.param;
                $.ajax({
                    url: urlgetRecordsforEdit,
                    data: { RecordID: editid, ProjectId: ddlProject, recordfrom: recordfrom },
                    type: "GET",
                    async: true,
                    beforeSend: function () {
                        waitingDialog.show('Loading...');
                    },
                    complete: function () {
                        waitingDialog.hide();
                    },
                    success: function (data) {

                        if (data.formused == true) {
                            // $('#btnSave').prop("disabled", true)
                            //$('#btnSave').hide()
                            //$('#lblnote').show()
                        }
                        else {
                            // $('#btnSave').prop("disabled", false)
                            $('#btnSave').show()
                            $('#lblnote').hide()
                        }
                        if (data.success == true) {
                            if (data.masterFields != null && data.masterFields != undefined) {
                                $('#txtNewFormName').val(data.masterFields.FormName);


                                $('#hdnFormID').val(data.masterFields.FormId);

                                $.each(_ProjectList, function (index, item) {
                                    if (item.ProjectId == data.masterFields.ProjectId) {
                                        prjFromDate = new Date(item.stringFromDate);
                                        prjToDate = new Date(item.stringToDate);
                                    }

                                })

                                $("#txtFromDate").datepicker('option', 'minDate', prjFromDate);
                                $("#txtToDate").datepicker('option', 'minDate', prjFromDate);

                                $("#txtFromDate").datepicker('option', 'maxDate', prjToDate);
                                $("#txtToDate").datepicker('option', 'maxDate', prjToDate);

                                $('#txtFromDate').val(data.masterFields.FromDate);
                                $('#txtToDate').val(data.masterFields.ToDate);

                                // $('#ddlFormType').val(data.masterFields.FormTypeID);

                                $('#ddlFormType').find('option[value=' + data.masterFields.FormTypeID + ']').attr('selected', 'selected');
                                $('#ddlProject').find('option[value=' + data.masterFields.ProjectId + ']').attr('selected', 'selected');
                                $("#ddlProject").attr('disabled', true).trigger("chosen:updated")
                                $("#ddlFormType").attr('disabled', true).trigger("chosen:updated")
                                $('.chosen-select').trigger('chosen:updated');
                                //$('#ddlProject').attr("disabled", "disabled")

                                // $('#ddlFormType').attr("disabled", "disabled")

                                //$scope.selectedItemvalue = data.masterFields.ProjectId;
                            }

                            if (data.Optiondata != null) {

                                $scope.AllOptionList = data.Optiondata;
                            }

                            GetSourcefieldsbyProjectID(data.masterFields.ProjectId)
                            GetDataTypeDropdownData()
                            $("#sortable2").empty();
                            $("#sortable1").empty();


                            var destinationfieldsList = [];
                            for (var i = data.formdata.length - 1; i >= 0; i--) {
                                for (var j = data.formdata[i].SectionFields.length - 1; j >= 0; j--) {
                                    var destinationfields = {
                                        "sectionid": data.formdata[i].SectionFields[j].field_SectionID,
                                        "fieldorder": data.formdata[i].SectionFields[j].field_FieldOrder,
                                        "fieldid": data.formdata[i].SectionFields[j].field_FieldID,
                                        "fieldName": data.formdata[i].SectionFields[j].field_FieldName,
                                    }
                                    destinationfieldsList.push(destinationfields);

                                }


                            }

                            destinationfieldsList.sort(comparedestiField);

                            for (var s = 0; s < sourcefields.length; s++) {
                                var fieldfoundflag = false;
                                for (var j = 0; j < destinationfieldsList.length; j++) {

                                    //if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName && destinationfieldsList[j].fieldid == sourcefields[s].Id) {
                                    if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName) {
                                        var formtype = $('#ddlFormType').val();
                                        var sourceli = ""
                                        if (formtype == "1" && (destinationfieldsList[j].fieldName == "NEW METER NUMBER" || destinationfieldsList[j].fieldName == "NEW METER RADIO")) {
                                            sourceli = " <li fieldorder='" + destinationfieldsList[j].fieldorder + "' lang='1' id='" + sourcefields[s].Id + "' name='" + destinationfieldsList[j].sectionid + "'  class='ui-state-default dataSaved'>" + sourcefields[s].FieldName + "</li>"
                                        }
                                        else {

                                            sourceli = " <li fieldorder='" + destinationfieldsList[j].fieldorder + "' lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "' name='" + destinationfieldsList[j].sectionid + "'  class='ui-state-default dataSaved'>" + sourcefields[s].FieldName + "</li>"
                                        }

                                        $("#sortable2").append(sourceli)
                                        fieldfoundflag = true;
                                    }

                                }
                                if (fieldfoundflag == false) {
                                    var sourceli = " <li lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "'   class='ui-state-default'>" + sourcefields[s].FieldName + "</li>"
                                    $("#sortable1").append(sourceli)
                                }
                            }

                            if (data.formdata != null && data.formdata != undefined) {

                                for (var i = data.formdata.length - 1; i >= 0; i--) {

                                    //#region section
                                    var section = {
                                        SectionOrder: uniqueSectionID,
                                        SectionID: data.formdata[i].SectionID,
                                        SectionName: data.formdata[i].SectionName
                                    }



                                    $scope.sectionList.push(section);
                                    setTimeout(function () {
                                        $scope.$apply();
                                    });

                                    var SectionFields = [];

                                    var SectionObject = {
                                        //"SectionID": uniqueSectionID,
                                        "SectionID": data.formdata[i].SectionID,
                                        "SectionName": data.formdata[i].SectionName,
                                        "SectionOrder": data.formdata[i].SectionOrder,
                                        "SectionFields": data.formdata[i].SectionFields
                                    };
                                    $scope.ProjectForms.push(SectionObject);

                                    uniqueSectionID++;
                                    //#endregion
                                }

                                SelectFirstSection();
                            }
                        }
                        else {

                            var $toast = toastr["error"](data.returnMessage, "Error Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                        }


                        if (recordView == true) {
                            $('#btnSave').hide()
                            $('#lblnote').hide()

                            $("#sectionActions").hide();
                            $("#txtNewFormName").prop("disabled", true);
                            $("#ddlFormType").attr('disabled', true).trigger("chosen:updated")
                            $("#txtFromDate").prop("disabled", true);
                            $("#txtToDate").prop("disabled", true);
                            $("#FormSectionTable").sortable("disable");
                            $("#sortable1, #sortable2").sortable("disable");
                        }
                        else {
                            $("#sectionActions").show();
                            $("#txtNewFormName").prop("disabled", false);
                            $("#txtFromDate").prop("disabled", false);
                            $("#txtToDate").prop("disabled", false);
                            $("#FormSectionTable").sortable("enable");
                            $("#sortable1, #sortable2").sortable("enable");
                        }
                        waitingDialog.hide();
                    },
                    error: function () {
                        waitingDialog.hide();
                        var $toast = toastr["error"]("Record not found", "Error Notification");
                        var indexpath = $rootScope.projectPath + "FormManagement";
                        $location.path(indexpath);
                    }

                });

            }

            else if (recordaddEdit == "Clone" && paramValue == "CloneParam") {
                debugger;
                $("#sectionActions").show();
                $("#txtNewFormName").prop("disabled", false);
                $("#txtFromDate").prop("disabled", false);
                $("#txtToDate").prop("disabled", false);
                recordView = false;


                $("#btnReset").hide();
                ResetFunction()
                GetProjectList();
                var urlgetRecordsforEdit = $("#urlgetRecordsforEdit").val();
                var ddlProject = "0";
                var recordfrom = "Clone1";
                var editid = CloneID;
                $.ajax({
                    url: urlgetRecordsforEdit,
                    data: { RecordID: editid, ProjectId: ddlProject, recordfrom: recordfrom },
                    type: "GET",
                    async: true,
                    beforeSend: function () {
                        waitingDialog.show('Loading...');
                    },
                    complete: function () {
                        waitingDialog.hide();
                    },
                    success: function (data) {


                        if (data.success == true) {
                            if (data.masterFields != null && data.masterFields != undefined) {
                                $('#txtNewFormName').val(data.masterFields.FormName);
                                $('#ddlFormType').find('option[value=' + data.masterFields.FormTypeID + ']').attr('selected', 'selected');
                                $("#ddlFormType").attr('disabled', true).trigger("chosen:updated")
                                //$('#hdnFormID').val(data.masterFields.FormId);

                                //$.each(_ProjectList, function (index, item) {
                                //    if (item.ProjectId == data.masterFields.ProjectId) {
                                //        prjFromDate = new Date(item.stringFromDate);
                                //        prjToDate = new Date(item.stringToDate);
                                //    }

                                //})

                                //$("#txtFromDate").datepicker('option', 'minDate', prjFromDate);
                                //$("#txtToDate").datepicker('option', 'minDate', prjFromDate);

                                //$("#txtFromDate").datepicker('option', 'maxDate', prjToDate);
                                //$("#txtToDate").datepicker('option', 'maxDate', prjToDate);

                                //$('#txtFromDate').val(data.masterFields.FromDate);
                                //$('#txtToDate').val(data.masterFields.ToDate);

                                //$('#ddlProject').find('option[value=' + data.masterFields.ProjectId + ']').attr('selected', 'selected');
                                //$(".chosen-select").attr('disabled', true).trigger("chosen:updated")
                                //$('.chosen-select').trigger('chosen:updated');
                                //$('#ddlProject').attr("disabled", "disabled")

                                //$scope.selectedItemvalue = data.masterFields.ProjectId;
                            }

                            if (data.Optiondata != null) {

                                $scope.AllOptionList = data.Optiondata;
                            }

                            GetSourcefieldsbyProjectID(data.masterFields.ProjectId)
                            GetDataTypeDropdownData()
                            $("#sortable2").empty();
                            $("#sortable1").empty();


                            var destinationfieldsList = [];
                            for (var i = data.formdata.length - 1; i >= 0; i--) {
                                for (var j = data.formdata[i].SectionFields.length - 1; j >= 0; j--) {
                                    var destinationfields = {
                                        "sectionid": data.formdata[i].SectionFields[j].field_SectionID,
                                        "fieldorder": data.formdata[i].SectionFields[j].field_FieldOrder,
                                        "fieldid": data.formdata[i].SectionFields[j].field_FieldID,
                                        "fieldName": data.formdata[i].SectionFields[j].field_FieldName,
                                    }
                                    destinationfieldsList.push(destinationfields);

                                }


                            }

                            destinationfieldsList.sort(comparedestiField);

                            for (var s = 0; s < sourcefields.length; s++) {
                                var fieldfoundflag = false;
                                for (var j = 0; j < destinationfieldsList.length; j++) {

                                    //if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName && destinationfieldsList[j].fieldid == sourcefields[s].Id) {
                                    if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName) {
                                        var sourceli = " <li fieldorder='" + destinationfieldsList[j].fieldorder + "' lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "' name='" + destinationfieldsList[j].sectionid + "'  class='ui-state-default dataSaved'>" + sourcefields[s].FieldName + "</li>"
                                        $("#sortable2").append(sourceli)
                                        fieldfoundflag = true;
                                    }

                                }
                                if (fieldfoundflag == false) {
                                    var sourceli = " <li lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "'   class='ui-state-default'>" + sourcefields[s].FieldName + "</li>"
                                    $("#sortable1").append(sourceli)
                                }
                            }

                            if (data.formdata != null && data.formdata != undefined) {

                                for (var i = data.formdata.length - 1; i >= 0; i--) {

                                    //#region section
                                    var section = {
                                        SectionOrder: uniqueSectionID,
                                        SectionID: data.formdata[i].SectionID,
                                        SectionName: data.formdata[i].SectionName
                                    }



                                    $scope.sectionList.push(section);
                                    setTimeout(function () { $scope.$apply(); });

                                    var SectionFields = [];

                                    var SectionObject = {
                                        //"SectionID": uniqueSectionID,
                                        "SectionID": data.formdata[i].SectionID,
                                        "SectionName": data.formdata[i].SectionName,
                                        "SectionOrder": data.formdata[i].SectionOrder,
                                        "SectionFields": data.formdata[i].SectionFields
                                    };
                                    $scope.ProjectForms.push(SectionObject);

                                    uniqueSectionID++;
                                    //#endregion
                                }

                                SelectFirstSection();
                            }
                        }
                        else {

                            var $toast = toastr["error"](data.returnMessage, "Error Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                        }

                    },
                    error: function () {

                        var $toast = toastr["error"]("Record not found", "Error Notification");
                        var indexpath = $rootScope.projectPath + "FormManagement";
                        $location.path(indexpath);
                    },


                });

            }

            else if (recordaddEdit == "Add") {
                projectActive = true;
                recordView = false;
                $("#btnSave").text("Save");
                $("#DivExistedProject").show();
                ResetFunction()
                GetProjectList();
                //   BindDates()
            }



            //#region Sortable intialization

            $(function () {
                $("#sortable1, #sortable2").sortable({
                    // containment: ".connectedSortable",
                    tolerance: "pointer",
                    connectWith: ".connectedSortable",
                    start: function (event, ui) {

                        if (event.target.id == "sortable2") {
                            //  ui.item.removeClass("showli").removeClass('hideli');



                        }
                        if ($scope.sectionList.length == 0) {

                            //var $toast = toastr["error"]('Please Add section', 'Error Notification');
                            event.stopPropagation();
                            return false;
                        }
                    },
                    stop: function (event, ui) {

                        if (event.originalEvent.target.parentElement.id == "sortable2") {
                            //if (event.toElement.parentElement.id == "sortable2") {
                            ui.item.attr("name", selectedSection)


                            //#region Section order
                            var sectionid = ui.item.attr("name");

                            var sectionList = $("#sortable2").children("li[name='" + sectionid + "']")
                            var sectionorder = 1;
                            for (var i = 0; i < sectionList.length; i++) {
                                var CurrentFieldID = sectionList[i].id;
                                var CurrentFieldName = sectionList[i].innerHTML;
                                for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                                    for (var k = $scope.ProjectForms[j].SectionFields.length - 1; k >= 0; k--) {
                                        if ($scope.ProjectForms[j].SectionFields[k].field_SectionID == sectionid && $scope.ProjectForms[j].SectionFields[k].field_FieldName == CurrentFieldName) {
                                            $scope.$apply(function () {
                                                $scope.ProjectForms[j].SectionFields[k].field_FieldOrder = sectionorder;
                                                sectionorder++;
                                            });
                                        }
                                    }
                                }
                            }
                            //#endregion
                        }

                        if (event.originalEvent.target.parentElement.id == "sortable1") {
                            //if (event.toElement.parentElement.id == "sortable1") {

                            var FieldID = ui.item.attr("id");
                            var FieldName = ui.item.text();
                            var sectionid = ui.item.attr("name");
                            //#region Section order


                            var sectionList = $("#sortable2").children("li[name='" + sectionid + "']")
                            var sectionorder = 1;
                            for (var i = 0; i < sectionList.length; i++) {
                                var CurrentFieldID = sectionList[i].id;
                                var CurrentFieldName = sectionList[i].innerHTML;
                                for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                                    for (var k = $scope.ProjectForms[j].SectionFields.length - 1; k >= 0; k--) {
                                        if ($scope.ProjectForms[j].SectionFields[k].field_SectionID == sectionid && $scope.ProjectForms[j].SectionFields[k].field_FieldName == CurrentFieldName) {
                                            $scope.$apply(function () {
                                                $scope.ProjectForms[j].SectionFields[k].field_FieldOrder = sectionorder;
                                                sectionorder++;
                                            });
                                        }
                                    }
                                }
                            }
                            //#endregion

                            for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                                if ($scope.ProjectForms[i].SectionID == sectionid) {
                                    for (var j = $scope.ProjectForms[i].SectionFields.length - 1; j >= 0; j--) {
                                        if ($scope.ProjectForms[i].SectionFields[j].field_SectionID == sectionid && $scope.ProjectForms[i].SectionFields[j].field_FieldName == FieldName)
                                            $scope.$apply(function () {
                                                $scope.ProjectForms[i].SectionFields.splice(j, 1);
                                            });
                                    }

                                }

                            }

                            //#region Delete dropdown optionlist
                            for (var i = 0; i < $scope.AllOptionList.length; i++) {
                                if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == FieldName) {

                                    $scope.AllOptionList.splice(i, 1);
                                }
                            }

                            //#endregion

                            //  $("#myDropdown[name='" + FieldID + "']").children().remove()
                            if ($("#sortable2").children().length == 0) {
                                $scope.$apply(function () {
                                    $scope.SetFieldAttr = false;

                                    // $("#SetFieldAttr").hide();
                                    $scope.SetFieldFor = "";
                                });
                                $('#myDropdown').ddslick('destroy');
                            }

                            //#endregion
                            if (ui.item.hasClass("active")) {
                                $scope.$apply(function () {
                                    $scope.SetFieldFor = "";
                                    $scope.SetFieldAttr = false;
                                    // $("#SetFieldAttr").hide();
                                    $scope.SetFieldddl = false;
                                });
                                if ($("#sortable2").children().length > 0) {
                                    var datafrom = "afterActiveDelete"
                                    // SelectFirstFieldinDestination(datafrom)
                                }
                            }

                            ui.item.removeClass("showli").removeClass("active").removeClass('hideli').removeClass('dataSaved').removeAttr("name").removeAttr("style");
                        }



                        if (event.originalEvent.target.parentElement.id == "sortable2") {
                            //if (event.toElement.parentElement.id == "sortable2") {
                            // SelectFirstFieldinDestination()

                        }
                    }
                }).disableSelection();
            });

            //#endregion


            //#region Get Existed project form

            $("#ddlExistedProject").unbind("change").change(function () {
                $('#ddlExistedProjectForm').empty();
                $('.chosen-select').trigger('chosen:updated');
                //  ProjectChange()
                // ResetFormChangeFunction()

                var ddlProject = $("#ddlExistedProject").val();
                if (ddlProject != "") {


                    var urlGetFormsbyProjectID = $('#urlGetFormsbyProjectID').val();

                    $.ajax({
                        type: 'GET',
                        async: false,
                        url: urlGetFormsbyProjectID,
                        data: { ProjectID: ddlProject },
                        success: function (data) {
                            $('#ddlExistedProjectForm').empty();
                            if (data.length == 0) {
                                $('#ddlExistedProjectForm').append(
                                    $('<option></option>'))
                                //.attr("selected", "selected").prop("disabled", false).val('').html("No records found"));

                            }
                            else {
                                $('#ddlExistedProjectForm').append(
                                    $('<option></option>'))
                                //.attr("selected", "selected").prop("disabled", false).val('0').html("--Select Form--"));
                            }



                            $.each(data, function (index, item) {

                                $('#ddlExistedProjectForm').append(
                                    $('<option></option>').val(item.FormId).html(item.FormName)
                                );
                            });
                            $('.chosen-select').trigger('chosen:updated');
                            //   $("#ddlExistedProjectForm").multiselect('rebuild');

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

            });

            $("#ddlExistedProjectForm").unbind("change").change(function () {

                var ddlFormID = $("#ddlExistedProjectForm").val();
                if (ddlFormID != "") {

                    //sourcefields

                    var urlgetRecordsforEdit = $("#urlgetRecordsforEdit").val();
                    var recordfrom = "Edit";
                    var ddlProject = "0";

                    $.ajax({
                        url: urlgetRecordsforEdit,
                        data: { RecordID: editid, ProjectId: ddlProject, recordfrom: recordfrom },
                        type: "GET",
                        async: false,
                        success: function (data) {

                            if (data.success == true) {

                                sourcefields
                                ResetFormChangeFunction()

                                if (data.Optiondata != null) {

                                    $scope.AllOptionList = data.Optiondata;
                                }

                                //  GetSourcefieldsbyProjectID(data.masterFields.ProjectId)

                                GetDataTypeDropdownData()

                                $("#sortable2").empty();
                                $("#sortable1").empty();


                                var destinationfieldsList = [];

                                for (var i = data.formdata.length - 1; i >= 0; i--) {
                                    for (var j = data.formdata[i].SectionFields.length - 1; j >= 0; j--) {
                                        var destinationfields = {
                                            "sectionid": data.formdata[i].SectionFields[j].field_SectionID,
                                            "fieldorder": data.formdata[i].SectionFields[j].field_FieldOrder,
                                            "fieldid": data.formdata[i].SectionFields[j].field_FieldID,
                                            "fieldName": data.formdata[i].SectionFields[j].field_FieldName,
                                        }
                                        destinationfieldsList.push(destinationfields);

                                    }


                                }

                                destinationfieldsList.sort(comparedestiField);

                                for (var s = 0; s < sourcefields.length; s++) {
                                    var fieldfoundflag = false;
                                    for (var j = 0; j < destinationfieldsList.length; j++) {

                                        //if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName && destinationfieldsList[j].fieldid == sourcefields[s].Id) {
                                        if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName) {
                                            var sourceli = " <li fieldorder='" + destinationfieldsList[j].fieldorder + "' lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "' name='" + destinationfieldsList[j].sectionid + "'  class='ui-state-default dataSaved'>" + sourcefields[s].FieldName + "</li>"
                                            $("#sortable2").append(sourceli)
                                            fieldfoundflag = true;
                                        }

                                    }
                                    if (fieldfoundflag == false) {
                                        var sourceli = " <li lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "'   class='ui-state-default'>" + sourcefields[s].FieldName + "</li>"
                                        $("#sortable1").append(sourceli)
                                    }
                                }

                                if (data.formdata != null && data.formdata != undefined) {

                                    for (var i = data.formdata.length - 1; i >= 0; i--) {

                                        //#region section
                                        var section = {
                                            SectionOrder: uniqueSectionID,
                                            SectionID: data.formdata[i].SectionID,
                                            SectionName: data.formdata[i].SectionName
                                        }



                                        $scope.sectionList.push(section);
                                        setTimeout(function () { $scope.$apply(); });

                                        var SectionFields = [];

                                        var SectionObject = {
                                            //"SectionID": uniqueSectionID,
                                            "SectionID": data.formdata[i].SectionID,
                                            "SectionName": data.formdata[i].SectionName,
                                            "SectionOrder": data.formdata[i].SectionOrder,
                                            "SectionFields": data.formdata[i].SectionFields
                                        };
                                        $scope.ProjectForms.push(SectionObject);

                                        uniqueSectionID++;
                                        //#endregion
                                    }
                                    // setTimeout(function () { $scope.$apply(); });
                                    SelectFirstSection();
                                }
                            }
                            else {

                                var $toast = toastr["error"](data.returnMessage, "Error Notification");
                                var indexpath = $rootScope.projectPath + "FormManagement";
                                $location.path(indexpath);
                            }
                            waitingDialog.hide();
                        },
                        error: function () {
                            waitingDialog.hide();
                            var $toast = toastr["error"]("Record not found", "Error Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                        }

                    });




                }


            })
            //#endregion


            //#region Bind Date




            //#endregion

            //#region Section code

            $("#FormSectionTable").sortable({
                containment: "parent",
                tolerance: "pointer",
                stop: function (event, ui) {
                    //#region Section order
                    angular.element("#FormSectionTable")
                    var sectionList = angular.element("#FormSectionTable").children("tr");
                    var sectionorder = 1;
                    for (var i = 0; i < sectionList.length; i++) {
                        var SectionID = sectionList[i].id;
                        for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                            if ($scope.ProjectForms[j].SectionID == SectionID) {
                                //$timeout(function () {
                                $scope.$apply(function () {
                                    $scope.ProjectForms[j].SectionOrder = sectionorder;
                                    sectionorder++;
                                });
                                //});

                            }
                        }
                    }

                    sectionListorder()

                    $scope.$apply();

                    //#endregion



                }
            }).disableSelection();

            function sectionorder() {

                //#region Section order
                angular.element("#FormSectionTable")
                var sectionList = angular.element("#FormSectionTable").children("tr");
                var sectionorder = 1;
                for (var i = 0; i < sectionList.length; i++) {
                    var SectionID = sectionList[i].id;
                    for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                        if ($scope.ProjectForms[j].SectionID == SectionID) {
                            //$timeout(function () {
                            $scope.$apply(function () {
                                $scope.ProjectForms[j].SectionOrder = sectionorder;
                                sectionorder++;
                            });
                            //});

                        }
                    }
                }

                //#endregion
            }



            $scope.SectionSelect = function ($event, sectionid) {


                var data = $("#txtfieldtitle").val();
                if (data == "" && data != undefined && SetFieldAttr == true && SetFieldddl == true) {

                    return false
                }


                var recordFound = false;
                $scope.DatatypenotselectedFieldList = [];

                for (var i = 0; i < $("#sortable2").children().length; i++) {
                    var datasaved = $($("#sortable2").children()[i]).hasClass("dataSaved")
                    if (datasaved == false) {
                        recordFound = true;
                        var fieldname = $($("#sortable2").children()[i]).text()
                        $scope.DatatypenotselectedFieldList.push(fieldname)

                    }
                }

                if (recordFound == true) {


                    // var compiledeHTML = $compile(tbody)($scope);
                    ngDialog.openConfirm({
                        template: $rootScope.projectPath + "Angular/views/Form/FieldDTempty.html",
                        scope: $scope,
                        // plain: true,
                        // showClose: false,
                        closeByDocument: false,
                        closeByEscape: false
                    }).then(function (confirm) {
                        return false;

                    }, function (reject) {

                        return false;

                    });
                    return false

                }



                selectedSection = "";
                $("#FormSectionTable>tr>td").removeClass("Active")
                $event.currentTarget.className = "Active "
                selectedSection = sectionid;
                var destAllFields = document.getElementById("sortable2").children;
                var destFields = document.getElementsByName(sectionid);
                $(destAllFields).removeClass('showli');
                $(destAllFields).addClass('hideli');
                $(destFields).removeClass('hideli');

                $(destFields).addClass('showli');
                $(destFields).removeClass('showli');

                $(function () {

                    var elems = $("#sortable2").children("li[name='" + sectionid + "']").get();

                    elems.sort(function (a, b) {

                        if (a.attributes["fieldorder"] != undefined && b.attributes["fieldorder"] != undefined) {
                            //return parseInt(a.attributes["fieldorder"].value) > parseInt(b.attributes["fieldorder"].value);



                            if (parseInt(a.attributes["fieldorder"].value) < parseInt(b.attributes["fieldorder"].value))
                                return -1;
                            else if (parseInt(a.attributes["fieldorder"].value) > parseInt(b.attributes["fieldorder"].value))
                                return 1;
                            else
                                return 0;

                        }

                    });
                    $('#sortable2').append(elems);
                });

                // $scope.selected = index;


                $scope.selectedSection = sectionid;
                $("#sortable2>li").removeClass("active")

                // SelectFirstFieldinDestination("sectionreselect", selectedSection)

                $scope.FieldForCurrentSection = $event.currentTarget.children[0].innerHTML;
                _CurrentSelectedSectionName = $event.currentTarget.children[0].innerHTML;
                _selectedSection = sectionid;
                // $("#currentfieldsection").text($event.currentTarget.children[0].innerHTML)


                $scope.SetFieldFor = "";
                $scope.SetFieldAttr = false;
                // $("#SetFieldAttr").hide();
                $scope.SetFieldddl = false;


            }



            $scope.resetsection = function () {
                SelectFirstSection()
                $scope.sectionName = '';
            };

            $scope.SaveSaction = function () {


                var SerialNo = 0;
                var last = $scope.sectionList.length
                if (last == 0) {
                    uniqueSectionID = 1;
                    SerialNo = "Section" + uniqueSectionID;
                }
                else {
                    SerialNo = "Section" + uniqueSectionID;

                }
                var sectionName1 = $scope.sectionName

                if (sectionName1 != "") {
                    var section = {
                        SectionOrder: "",
                        SectionID: SerialNo,
                        SectionName: sectionName1
                    }

                    for (var i = $scope.sectionList.length - 1; i >= 0; i--) {
                        if ($scope.sectionList[i].SectionName.toLowerCase() == sectionName1.toLowerCase()) {
                            var $toast = toastr["error"]('Section name already exist', 'Error Notification');
                            return false;
                        }
                    }

                    $scope.sectionList.push(section);


                    var SectionFields = [];

                    var SectionObject = {
                        "SectionID": SerialNo,
                        "SectionName": sectionName1,
                        "SectionOrder": uniqueSectionID,
                        "SectionFields": SectionFields
                    };
                    $scope.ProjectForms.push(SectionObject);

                    uniqueSectionID++;



                    //#region Section order
                    angular.element("#FormSectionTable")
                    var sectionList = angular.element("#FormSectionTable").children("tr");
                    var sectionorder = 1;
                    for (var i = 0; i < sectionList.length; i++) {
                        var SectionID = sectionList[i].id;
                        for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                            if ($scope.ProjectForms[j].SectionID == SectionID) {
                                //$timeout(function () {
                                // $scope.$apply(function () {
                                $scope.ProjectForms[j].SectionOrder = sectionorder;
                                sectionorder++;

                                //  });
                                //});

                            }
                        }
                    }


                    sectionListorder()




                    //#endregion

                    if ($scope.sectionList.length == 1) {
                        SelectFirstSection();
                    }

                    $scope.sectionName = "";
                }
                $("#myModal").modal('hide');
            }

            function sectionListorder() {


                var sectionList = angular.element("#FormSectionTable").children("tr");
                var sectionorder = 1;
                for (var i = 0; i < sectionList.length; i++) {
                    var SectionID = sectionList[i].id;
                    for (var j = $scope.sectionList.length - 1; j >= 0; j--) {
                        if ($scope.sectionList[j].SectionID == SectionID) {
                            //$timeout(function () {
                            // $scope.$apply(function () {
                            $scope.sectionList[j].SectionOrder = sectionorder;
                            sectionorder++;
                            //  });
                            //});

                        }
                    }
                }
            }




            $(document.body).unbind("click").click(function (e) {

                var SetFieldAttr = $scope.SetFieldAttr
                // $("#SetFieldAttr").hide();
                var SetFieldddl = $scope.SetFieldddl
                var data = $("#txtfieldtitle").val();
                if (e.target.type == "button" && e.target.id == "btnconfirm") {
                    //  
                    if (data == "" && data != undefined && SetFieldAttr == true && SetFieldddl == true) {
                        $("#txtfieldtitle").focus();
                    }

                    return false;
                }

                if (data == "" && data != undefined && SetFieldAttr == true && SetFieldddl == true) {
                    e.stopPropagation();
                    e.preventDefault();
                    ngDialog.openConfirm({
                        template: '\
                <div ><h4 class="modal-title">Please correct the following error(s)</h4></div>\
                 <br>\
                <p>Field Label required</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" id="btnconfirm" ng-click="confirm(1)">Ok</button>\
                </div>',
                        plain: true,
                        showClose: false,
                        closeByDocument: false,
                        closeByEscape: false
                    }).then(function (confirm) {
                        $("#txtfieldtitle").focus();

                        return false;

                    }, function (reject) {

                    });
                }
            });


            $scope.FieldLabelCheck1 = function (data) {


                if (data == "" && data != undefined) {

                    ngDialog.openConfirm({
                        template: '\
                <div ><h4 class="modal-title">Please correct the following error(s)</h4></div>\
                 <br>\
                <p>Field Label required</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Ok</button>\
                </div>',
                        plain: true,
                        showClose: false,
                        closeByDocument: false,
                        closeByEscape: false
                    }).then(function (confirm) {
                        $("#txtfieldtitle").focus();

                        return false;

                    }, function (reject) {

                    });
                }
            }

            $scope.DeleteSection = function () {

                if (selectedSection == "" || selectedSection == undefined) {
                    var $toast = toastr["error"]('Please Select section', 'Error Notification');
                    return false;
                }
                ngDialog.openConfirm({
                    template: '\
                <p>Are you sure you want to delete selected section?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                    plain: true,
                    showClose: false,
                    closeByDocument: false,
                    closeByEscape: false
                }).then(function (confirm) {

                    $("#FormSectionTable>tr>td").hasClass("Active");

                    if ($("#FormSectionTable").children("#" + selectedSection).children().hasClass("Active")) {

                        for (var i = $scope.sectionList.length - 1; i >= 0; i--) {
                            if ($scope.sectionList[i].SectionID == selectedSection)
                                $scope.sectionList.splice(i, 1);

                        }




                        $scope.sectionList.sort(compareSectionList);



                        bindSectionList($scope.sectionList)

                        for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                            if ($scope.ProjectForms[i].SectionID == selectedSection) {

                                $scope.ProjectForms.splice(i, 1);

                            }

                        }

                        //#region Delete dropdown optionlist
                        for (var i = 0; i < $scope.AllOptionList.length; i++) {
                            if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName) {

                                $scope.AllOptionList.splice(i, 1);
                            }
                        }

                        //#endregion

                        SelectFirstSection();
                    }

                    if ($scope.sectionList.length == 0) {
                        uniqueSectionID = 1;
                    }

                    var destFields = document.getElementsByName(selectedSection);


                    $(destFields).attr("style", "");
                    $(destFields).removeClass('hideli').removeClass('dataSaved').removeClass('active');
                    $(destFields).removeClass('showli');
                    $("#sortable1").append(destFields)

                    //#region Section order
                    angular.element("#FormSectionTable")
                    var sectionList = angular.element("#FormSectionTable").children("tr");
                    var sectionorder = 1;
                    for (var i = 0; i < sectionList.length; i++) {
                        var SectionID = sectionList[i].id;
                        for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                            if ($scope.ProjectForms[j].SectionID == SectionID) {
                                //$timeout(function () {
                                // $scope.$apply(function () {
                                $scope.ProjectForms[j].SectionOrder = sectionorder;
                                sectionorder++;
                                //  });
                                //});

                            }
                        }
                    }

                    //#endregion



                    if ($("#sortable2").children().length == 0) {
                        //$scope.$apply(function () {
                        $scope.SetFieldAttr = false;
                        // $("#SetFieldAttr").hide();
                        $scope.SetFieldFor = "";
                        //});
                        $('#myDropdown').ddslick('destroy');
                    }

                }, function (reject) {

                    return false;

                });


            }

            function bindSectionList(data) {

                var newsectionlist = angular.copy(data)
                $scope.sectionList = newsectionlist;
            }

            $scope.EditSection = function () {

                if (selectedSection == "" || selectedSection == undefined) {
                    var $toast = toastr["error"]('Please Select section', 'Error Notification');
                    return false;
                }
                $("#EditSectionModal").modal('show');
                for (var i = $scope.sectionList.length - 1; i >= 0; i--) {
                    if ($scope.sectionList[i].SectionID == selectedSection) {

                        $("#txtEditsectionName").val($scope.sectionList[i].SectionName)
                        $scope.EditsectionName = $scope.sectionList[i].SectionName

                    }
                }

            }

            $scope.SaveEditSection = function () {

                if ($scope.selectedSection == "") {
                    var $toast = toastr["error"]('Please Select section', 'Error Notification');
                }

                for (var i = $scope.sectionList.length - 1; i >= 0; i--) {
                    if ($scope.sectionList[i].SectionID != $scope.selectedSection) {
                        if ($scope.sectionList[i].SectionName.toLowerCase() == $scope.EditsectionName.toLowerCase()) {
                            var $toast = toastr["error"]('Section name already exist', 'Error Notification');
                            return false;
                        }
                    }
                }

                //$("#currentfieldsection").text($scope.EditsectionName)
                $scope.FieldForCurrentSection = $scope.EditsectionName;
                // $("#EditSectionModal").modal('show');
                for (var i = $scope.sectionList.length - 1; i >= 0; i--) {
                    if ($scope.sectionList[i].SectionID == $scope.selectedSection) {

                        //$("#txtEditsectionName").val($scope.sectionList[i].SectionName)
                        $scope.sectionList[i].SectionName = $scope.EditsectionName

                    }
                }
                for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                    if ($scope.ProjectForms[i].SectionID == $scope.selectedSection) {
                        $scope.ProjectForms[i].SectionName = $scope.EditsectionName

                    }

                }


                $("#EditSectionModal").modal('hide');
            }

            function SelectFirstFieldinDestination(datafrom, selectedSection) {

                if (datafrom == "afterActiveDelete") {
                    var firstli = $("#sortable2>li")[0];
                    DestinationFieldClick(firstli)
                    return false;
                }
                if (datafrom == "sectionreselect") {
                    var destFields = document.getElementsByName(selectedSection)

                    if (destFields.length > 0) {
                        var firstli = destFields[0];
                        DestinationFieldClick(firstli)
                        return false;
                    }
                    else {
                        $scope.SetFieldFor = "";
                        $scope.SetFieldAttr = false;
                        // $("#SetFieldAttr").hide();
                        $scope.SetFieldddl = false;
                        return false;
                    }
                }

                if (angular.element("#sortable2").children().length == 1) {
                    //$timeout(function () {
                    //    angular.element("#sortable2").children().first().triggerHandler('click');
                    //}, 0);
                    var firstli = $("#sortable2>li")[0];
                    DestinationFieldClick(firstli)
                }

            }

            $("#btnAddSection").unbind("click").click(function () {

                var $txtFormName = $("#txtNewFormName").val();
                var $ddlProject = $("#ddlProject").val();

                if ($ddlProject == '0') {
                    var $toast = toastr["error"]('Please select project name', 'Error Notification');
                }
                else if ($txtFormName == '') {
                    var $toast = toastr["error"]('Please enter the form name', 'Error Notification');
                }
                else {
                    // $scope.sectionName = 
                    $("#txtDivName").val('');
                    $("#txtsectionName").val('');
                    $("#myModal").modal('show');
                    $('#myModal').on('shown.bs.modal', function () {
                        $("#txtsectionName").focus();
                        $("#txtsectionName").val('');
                    })
                }
            })

            //#endregion
            var formtypevalue = "0";
            $("#ddlFormType").unbind("change").change(function () {


                if ($scope.ProjectForms != undefined && $scope.ProjectForms.length > 0 && formtypevalue != "0") {

                    ngDialog.openConfirm({
                        template: '\
                <p>Are you sure you want to change Form Type?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                        plain: true,
                        showClose: false,
                        closeByDocument: false,
                        closeByEscape: false
                    }).then(function (confirm) {
                        if (recordaddEdit == "Add") {
                            ResetFunction()
                            ProjectChange()
                        }
                        if (recordaddEdit == "Clone") {
                            projectChangeClone()
                        }


                    }, function (reject) {

                        $('#ddlProject').find('option[value=' + selectedProject + ']').attr('selected', 'selected');
                        return false;

                    });


                }
                else {
                    if (recordaddEdit == "Add") {
                        ResetFunction()
                        ProjectChange()
                    }
                    if (recordaddEdit == "Clone") {
                        projectChangeClone()
                    }
                }

                formtypevalue = $("#ddlFormType").val();

            });


            $("#ddlProject").unbind("change").change(function () {


                if ($scope.ProjectForms != undefined && $scope.ProjectForms.length > 0 && selectedProject != "0") {

                    ngDialog.openConfirm({
                        template: '\
                <p>Are you sure you want to change project?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                        plain: true,
                        showClose: false,
                        closeByDocument: false,
                        closeByEscape: false
                    }).then(function (confirm) {
                        if (recordaddEdit == "Add") {
                            ResetFunction()
                            ProjectChange()
                        }
                        if (recordaddEdit == "Clone") {
                            projectChangeClone()
                        }


                    }, function (reject) {

                        $('#ddlProject').find('option[value=' + selectedProject + ']').attr('selected', 'selected');
                        return false;

                    });


                }
                else {
                    if (recordaddEdit == "Add") {
                        ResetFunction()
                        ProjectChange()
                    }
                    if (recordaddEdit == "Clone") {
                        projectChangeClone()
                    }
                }



            });

            var selectedProject = "0";

            function ProjectChange() {

                var ddlProject = $("#ddlProject").val();
                selectedProject = ddlProject;
                if (ddlProject != "0") {
                    $("#txtFromDate").datepicker("destroy");
                    $("#txtToDate").datepicker("destroy");


                    $('#ddlExistedProject').empty();

                    $('#ddlExistedProject').append($('<option></option>'))
                    //.attr("selected", "selected").prop("disabled", false).val("").html("--Select Project--"));

                    $.each(_ProjectList, function (index, item) {
                        if (item.ProjectId == ddlProject) {

                            prjFromDate = new Date(item.stringFromDate);
                            prjToDate = new Date(item.stringToDate);
                            //alert(item.stringFromDate + " " + item.stringToDate)

                            $('#txtFromDate').datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                showButtonPanel: true,
                                dateFormat: 'M-dd-yy',
                                onSelect: function () {

                                    $(this).change();
                                    var date = $("#txtFromDate").val();
                                    if ($.trim(date) != "") {
                                        var newDate1 = new Date(date);

                                        // $("#txtFromDate").val($.datepicker.formatDate('dd-M-yy', newDate));
                                        $("#txtToDate").datepicker('option', 'minDate', newDate1);


                                    }
                                    // GetVendor()
                                },
                            }).datepicker("setDate", prjFromDate);


                            $('#txtToDate').datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                showButtonPanel: true,
                                dateFormat: 'M-dd-yy',
                            }).datepicker("setDate", prjToDate);
                        }


                        if (item.ProjectId != ddlProject) {

                            $('#ddlExistedProject').append(
                                $('<option></option>').val(item.ProjectId).html(item.ProjectCityState)
                            );

                        }

                    })

                    // $("#ddlExistedProject").multiselect('rebuild');
                    $('.chosen-select').trigger('chosen:updated');
                    GetSourcefieldsbyProjectID(ddlProject)

                    GetDataTypeDropdownData()

                    //#region section
                    var section = {
                        SectionOrder: uniqueSectionID,
                        SectionID: "Section" + uniqueSectionID,
                        SectionName: "Basic info"
                    }




                    $scope.sectionList.push(section);

                    setTimeout(function () { $scope.$apply(); });

                    var SectionFields = [];

                    var SectionObject = {
                        "SectionID": "Section" + uniqueSectionID,
                        "SectionName": "Basic info",
                        "SectionOrder": uniqueSectionID,
                        "SectionFields": SectionFields
                    };
                    $scope.ProjectForms.push(SectionObject);

                    uniqueSectionID++;
                    //#endregion

                    SelectFirstSection();
                    $("#txtNewFormName").focus();
                }
            }

            function projectChangeClone() {


                ResetFormChangeFunction()

                var ddlProject = $("#ddlProject").val();
                selectedProject = ddlProject;
                if (ddlProject != "0") {
                    $("#txtFromDate").datepicker("destroy");
                    $("#txtToDate").datepicker("destroy");

                    $.each(_ProjectList, function (index, item) {
                        if (item.ProjectId == ddlProject) {

                            prjFromDate = new Date(item.stringFromDate);
                            prjToDate = new Date(item.stringToDate);


                            $('#txtFromDate').datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                showButtonPanel: true,
                                dateFormat: 'M-dd-yy',
                                onSelect: function () {

                                    $(this).change();
                                    var date = $("#txtFromDate").val();
                                    if ($.trim(date) != "") {
                                        var newDate1 = new Date(date);

                                        // $("#txtFromDate").val($.datepicker.formatDate('dd-M-yy', newDate));
                                        $("#txtToDate").datepicker('option', 'minDate', newDate1);


                                    }
                                    // GetVendor()
                                },
                            }).datepicker("setDate", prjFromDate);


                            $('#txtToDate').datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                showButtonPanel: true,
                                dateFormat: 'M-dd-yy',
                            }).datepicker("setDate", prjToDate);
                        }

                    })

                    GetSourcefieldsbyProjectID(ddlProject)

                    GetDataTypeDropdownData()


                    $("#txtNewFormName").focus();
                }


                var urlgetRecordsforEdit = $("#urlgetRecordsforEdit").val();
                var recordfrom = "Clone";
                var editid = CloneID;
                $.ajax({
                    url: urlgetRecordsforEdit,
                    data: { RecordID: editid, ProjectId: ddlProject, recordfrom: recordfrom },
                    type: "GET",
                    async: true,
                    beforeSend: function () {
                        waitingDialog.show('Loading...');
                    },
                    success: function (data) {

                        if (data.success == true) {
                            if (data.masterFields != null && data.masterFields != undefined) {
                                // $('#txtNewFormName').val(data.masterFields.FormName);


                                //$('#hdnFormID').val(data.masterFields.FormId);

                                //$.each(_ProjectList, function (index, item) {
                                //    if (item.ProjectId == data.masterFields.ProjectId) {
                                //        prjFromDate = new Date(item.stringFromDate);
                                //        prjToDate = new Date(item.stringToDate);
                                //    }

                                //})

                                //$("#txtFromDate").datepicker('option', 'minDate', prjFromDate);
                                //$("#txtToDate").datepicker('option', 'minDate', prjFromDate);

                                //$("#txtFromDate").datepicker('option', 'maxDate', prjToDate);
                                //$("#txtToDate").datepicker('option', 'maxDate', prjToDate);

                                //$('#txtFromDate').val(data.masterFields.FromDate);
                                //$('#txtToDate').val(data.masterFields.ToDate);

                                //$('#ddlProject').find('option[value=' + data.masterFields.ProjectId + ']').attr('selected', 'selected');
                                //$(".chosen-select").attr('disabled', true).trigger("chosen:updated")
                                //$('.chosen-select').trigger('chosen:updated');
                                //$('#ddlProject').attr("disabled", "disabled")

                                //$scope.selectedItemvalue = data.masterFields.ProjectId;
                            }

                            if (data.Optiondata != null) {

                                $scope.AllOptionList = data.Optiondata;
                            }

                            // GetSourcefieldsbyProjectID(data.masterFields.ProjectId)
                            // GetDataTypeDropdownData()
                            $("#sortable2").empty();
                            $("#sortable1").empty();


                            var destinationfieldsList = [];
                            for (var i = data.formdata.length - 1; i >= 0; i--) {
                                for (var j = data.formdata[i].SectionFields.length - 1; j >= 0; j--) {
                                    var destinationfields = {
                                        "sectionid": data.formdata[i].SectionFields[j].field_SectionID,
                                        "fieldorder": data.formdata[i].SectionFields[j].field_FieldOrder,
                                        "fieldid": data.formdata[i].SectionFields[j].field_FieldID,
                                        "fieldName": data.formdata[i].SectionFields[j].field_FieldName,
                                    }
                                    destinationfieldsList.push(destinationfields);

                                }


                            }

                            destinationfieldsList.sort(comparedestiField);

                            for (var s = 0; s < sourcefields.length; s++) {
                                var fieldfoundflag = false;
                                for (var j = 0; j < destinationfieldsList.length; j++) {

                                    //if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName && destinationfieldsList[j].fieldid == sourcefields[s].Id) {
                                    if (destinationfieldsList[j].fieldName == sourcefields[s].FieldName) {
                                        var sourceli = " <li fieldorder='" + destinationfieldsList[j].fieldorder + "' lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "' name='" + destinationfieldsList[j].sectionid + "'  class='ui-state-default dataSaved'>" + sourcefields[s].FieldName + "</li>"
                                        $("#sortable2").append(sourceli)
                                        fieldfoundflag = true;
                                    }

                                }
                                if (fieldfoundflag == false) {
                                    var sourceli = " <li lang='" + sourcefields[s].isMappedToExcel + "' id='" + sourcefields[s].Id + "'   class='ui-state-default'>" + sourcefields[s].FieldName + "</li>"
                                    $("#sortable1").append(sourceli)
                                }
                            }

                            var formdatafinal;
                            if (data.formdata != null && data.formdata != undefined) {
                                formdatafinal = angular.copy(data.formdata);









                                for (var i = data.formdata.length - 1; i >= 0; i--) {

                                    //#region section
                                    var section = {
                                        SectionOrder: uniqueSectionID,
                                        SectionID: data.formdata[i].SectionID,
                                        SectionName: data.formdata[i].SectionName
                                    }



                                    $scope.sectionList.push(section);
                                    setTimeout(function () { $scope.$apply(); });

                                    var SectionFields = [];

                                    var SectionObject = {
                                        //"SectionID": uniqueSectionID,
                                        "SectionID": data.formdata[i].SectionID,
                                        "SectionName": data.formdata[i].SectionName,
                                        "SectionOrder": data.formdata[i].SectionOrder,
                                        "SectionFields": data.formdata[i].SectionFields
                                    };

                                    $scope.ProjectForms.push(SectionObject);

                                    uniqueSectionID++;
                                    //#endregion
                                }

                                SelectFirstSection();
                            }
                        }
                        else {

                            var $toast = toastr["error"](data.returnMessage, "Error Notification");
                            var indexpath = $rootScope.projectPath + "FormManagement";
                            $location.path(indexpath);
                        }

                    },
                    error: function () {

                        var $toast = toastr["error"]("Record not found", "Error Notification");
                        var indexpath = $rootScope.projectPath + "FormManagement";
                        $location.path(indexpath);
                    },
                    complete: function () {
                        waitingDialog.hide();
                    },
                });

            }


            var FieldDataTypeSaved = false;

            function DestinationFieldClick(data) {

                if (data == undefined) {
                    return false;
                }

                $("#sortable2>li").removeClass("active")
                angular.element(data).addClass("active ")
                var selectedele = angular.element(data);
                var selectedsection = angular.element(data).attr("name");
                var ismappedtoexcel = angular.element(data).attr("lang");
                var selectedFieldText = angular.element(data).text();
                var selectedFieldID = angular.element(data).attr("id");

                _selectedFieldText = selectedFieldText;

                var newindex = -1;

                $scope.$apply(function () {
                    $scope.SetFieldAttr = false;
                    $scope.SetFieldddl = true;
                    $scope.SetFieldFor = " for " + selectedFieldText;
                });

                //#region dropdown fill
                $('#myDropdown').ddslick("destroy")
                var ddData = [];
                if ($scope.DataTypeddlData.length > 0) {

                    $.each($scope.DataTypeddlData, function (index, item) {

                        var Iconename = item.FieldDataTypeName.replace(/ /g, '').replace("/", '').replace("/", '');
                        var IconPath = $rootScope.projectPath + "Angular/views/icon/" + Iconename + ".png";

                        var sampledata = {
                            text: item.FieldDataTypeName,
                            value: item.DataTypeId,
                            selected: false,
                            description: "",
                            imageSrc: IconPath
                        }
                        ddData.push(sampledata);

                    });

                    angular.element("#myDropdown").ddslick({
                        //width: 373,
                        data: ddData,
                        selectText: "Select Field DataType",
                        imagePosition: "left",
                    });
                }

                //#endregion

                //#region dropdown select
                for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {
                    if ($scope.ProjectForms[i].SectionID == selectedsection) {
                        for (var j = $scope.ProjectForms[i].SectionFields.length - 1; j >= 0; j--) {
                            if ($scope.ProjectForms[i].SectionFields[j].field_SectionID == selectedsection && $scope.ProjectForms[i].SectionFields[j].field_FieldName == selectedFieldText) {
                                $scope.$apply(function () {
                                    $scope.ProjectForms[i].SectionFields[j].field_MappedtoExcel = ismappedtoexcel,
                                        $scope.field = $scope.ProjectForms[i].SectionFields[j]
                                });
                                var searchBy = $scope.ProjectForms[i].SectionFields[j].field_DatatypeID;

                                $('#myDropdown li').each(function (index1) {

                                    var curValue = $(this).find('.dd-option-value').val()
                                    curValue = curValue

                                    if (curValue == searchBy) {
                                        newindex = $(this).index();

                                        if (curValue == "Text") {
                                            newindex = 0;
                                        }
                                        FieldDataTypeSaved = true;
                                        $scope.SetFieldAttr = true;

                                        if (recordView == true) {
                                            setTimeout(disableallelements, 1000)
                                            // $("#datatypediv *").attr("disabled", "disabled").off('click');
                                        }
                                        spinner.stop(target);

                                    }
                                });
                            }
                        }

                    };
                };
                //#endregion

                ddData = [];

                $('#myDropdown').ddslick("destroy")

                if ($scope.DataTypeddlData.length > 0) {

                    $.each($scope.DataTypeddlData, function (index, item) {



                        debugger;
                        var Iconename = item.FieldDataTypeName.replace(/ /g, '').replace("/", '').replace("/", '');
                        var IconPath = $rootScope.projectPath + "Angular/views/icon/" + Iconename + ".png";
                        if (ismappedtoexcel == "1" && _selectedFieldText != "LOCATION/GPS") {
                            $scope.DivListOptionShow = false;
                            if (item.FieldDataTypeName.toLowerCase().trim() == "List".toLowerCase()) {
                                var sampledata = {
                                    text: item.FieldDataTypeName,
                                    value: item.DataTypeId,
                                    selected: true,
                                    description: item.FieldDataTypeDescription,
                                    imageSrc: IconPath
                                }
                                ddData.push(sampledata);


                            }
                        }

                        else if (_selectedFieldText == "LOCATION/GPS") {
                            if (item.FieldDataTypeName.toLowerCase().trim() == "LOCATION/GPS".toLowerCase()) {
                                var sampledata = {
                                    text: item.FieldDataTypeName,
                                    value: item.DataTypeId,
                                    selected: true,
                                    description: item.FieldDataTypeDescription,
                                    imageSrc: IconPath
                                }
                                ddData.push(sampledata);


                            }

                        }
                        else if (_selectedFieldText == "WORK COMPLETED") {
                            if (item.FieldDataTypeName.toLowerCase().trim() == "Yes/No".toLowerCase()) {
                                var sampledata = {
                                    text: item.FieldDataTypeName,
                                    value: item.DataTypeId,
                                    selected: true,
                                    description: item.FieldDataTypeDescription,
                                    imageSrc: IconPath
                                }
                                ddData.push(sampledata);



                            }

                        }
                            //AddedBy Aniket on 13-Mar-2018
                            //else if (_selectedFieldText == "NEW METER NUMBER") {
                            //    if (item.FieldDataTypeName.toLowerCase().trim() == "List".toLowerCase()) {
                            //        var sampledata = {
                            //            text: item.FieldDataTypeName,
                            //            value: item.DataTypeId,
                            //            selected: true,
                            //            description: item.FieldDataTypeDescription,
                            //            imageSrc: IconPath
                            //        }
                            //        ddData.push(sampledata);


                            //    }

                            //}





                        else {

                            if (index == newindex) {
                                var sampledata = {
                                    text: item.FieldDataTypeName,
                                    value: item.DataTypeId,
                                    selected: true,
                                    description: item.FieldDataTypeDescription,
                                    imageSrc: IconPath
                                }
                                ddData.push(sampledata);

                            } else {

                                var sampledata = {
                                    text: item.FieldDataTypeName,
                                    value: item.DataTypeId,
                                    selected: false,
                                    description: item.FieldDataTypeDescription,
                                    imageSrc: IconPath
                                }
                                ddData.push(sampledata);
                            }
                        }
                    });

                }


                angular.element("#myDropdown").ddslick({
                    //width: 373,
                    data: ddData,
                    selectText: "Select Field DataType",
                    imagePosition: "left",
                    onSelected: function (selectedData) {

                        $scope.selecteddatatype;
                        $scope.SetFieldAttr = true;
                        $scope.$apply(function () {
                            $scope.selecteddatatype = selectedData.selectedData.text.replace(/ /g, '').replace("/", '').replace("/", '');

                        })

                        var htmlfilename = selectedData.selectedData.text.replace(/ /g, '').replace("/", '').replace("/", '');

                        $scope.$apply(function () {
                            $scope.FieldTemplateName = $rootScope.projectPath + "Angular/views/Form/field/" + htmlfilename + ".html";
                        });


                        $("#sortable2").children("li[id='" + selectedFieldID + "']").addClass("dataSaved")
                        var isoptionsexist = false;
                        if (ismappedtoexcel == "0") {

                            if ($.trim(selectedData.selectedData.text) == "List") {
                                $("#ListOptionsTable").empty();
                                $scope.DivListOptionShow = true;

                                for (var i = 0; i < $scope.AllOptionList.length; i++) {
                                    if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {

                                        $scope.AllOptionList[i].OptionList;
                                        isoptionsexist = true;
                                        CreateOptionList($scope.AllOptionList[i].OptionList)
                                    }
                                }
                                if (isoptionsexist == false) {
                                    var OptionFields = [];

                                    var ListOptionObject = {
                                        "SectionID": _selectedSection,
                                        "SectionName": _CurrentSelectedSectionName,
                                        "FieldName": _selectedFieldText,
                                        "OptionList": OptionFields
                                    };

                                    $scope.AllOptionList.push(ListOptionObject);
                                }
                            }
                        }

                        $scope.finishLoading = function () {

                            var isoptionsexist = false;
                            if (ismappedtoexcel == "0") {
                                if ($.trim(selectedData.selectedData.text) == "List") {
                                    $("#ListOptionsTable").empty();
                                    $scope.DivListOptionShow = true;

                                    for (var i = 0; i < $scope.AllOptionList.length; i++) {
                                        if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {

                                            $scope.AllOptionList[i].OptionList;
                                            isoptionsexist = true;
                                            CreateOptionList($scope.AllOptionList[i].OptionList)
                                        }
                                    }
                                    if (isoptionsexist == false) {
                                        var OptionFields = [];

                                        var ListOptionObject = {
                                            "SectionID": _selectedSection,
                                            "SectionName": _CurrentSelectedSectionName,
                                            "FieldName": _selectedFieldText,
                                            "OptionList": OptionFields
                                        };

                                        $scope.AllOptionList.push(ListOptionObject);
                                    }


                                    $("#ListOptionsTable").sortable({
                                        containment: "parent",
                                        tolerance: "pointer",
                                        stop: function (event, ui) {


                                            var OptionsListnew = angular.element("#ListOptionsTable").children("tr");
                                            var sectionorder = 1;
                                            for (var i = 0; i < OptionsListnew.length; i++) {
                                                var optionName = $(OptionsListnew[i]).find("td[name='nameoptionName']").text()
                                                var optionValue = $(OptionsListnew[i]).find("td[name='nameoptionValue']").text()
                                                var optionFilterkey = $(OptionsListnew[i]).find("td[name='nameoptionFilterkey']").text()
                                                var optionid = $(OptionsListnew[i]).find("td[name='nameuniqueoptionid']").attr("id");

                                                for (var j = 0; j < $scope.AllOptionList.length; j++) {
                                                    if ($scope.AllOptionList[j].SectionID == _selectedSection && $scope.AllOptionList[j].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[j].FieldName == _selectedFieldText) {
                                                        for (var k = 0; k < $scope.AllOptionList[j].OptionList.length; k++) {
                                                            //if ($scope.AllOptionList[j].OptionList[k].FieldText == optionName && $scope.AllOptionList[j].OptionList[k].FieldValue == optionValue && $scope.AllOptionList[j].OptionList[k].FieldFilterkey == optionFilterkey) {
                                                            if ($scope.AllOptionList[j].OptionList[k].optionID == optionid) {
                                                                $scope.$apply(function () {
                                                                    $scope.AllOptionList[j].OptionList[k].OrderNo = i + 1;
                                                                    $(OptionsListnew[i]).find("td[name='nameoptionSerialNo']").text((i + 1))
                                                                })

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }
                            }

                            debugger;
                            if (_selectedFieldText == "WORK COMPLETED") {
                                $("#divfield_SynchFlag").show();
                            }
                            else {
                                $("#divfield_SynchFlag").hide();
                            }

                            if (recordView == true) {

                                $("#datatypediv *").attr("disabled", "disabled").off('click');
                            }



                            //AddedBy AniketJ on 02-Mar-2017
                            if ($scope.selecteddatatype == "Calculation") {
                                getNumericFields()
                            }
                        }

                        if ($scope.selecteddatatype == "Calculation") {
                            getNumericFields()
                        }


                        for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {
                            if ($scope.ProjectForms[i].SectionID == selectedsection) {
                                for (var j = $scope.ProjectForms[i].SectionFields.length - 1; j >= 0; j--) {
                                    if ($scope.ProjectForms[i].SectionFields[j].field_SectionID == selectedsection && $scope.ProjectForms[i].SectionFields[j].field_FieldName == selectedFieldText) {
                                        $scope.$apply(function () {
                                            if (FieldDataTypeSaved == true) {

                                                FieldDataTypeSaved = false;
                                                $scope.ProjectForms[i].SectionFields[j].field_DatatypeName = $.trim(selectedData.selectedData.text);
                                                $scope.ProjectForms[i].SectionFields[j].field_DatatypeID = selectedData.selectedData.value;
                                                $scope.ProjectForms[i].SectionFields[j].field_MappedtoExcel = ismappedtoexcel;
                                            }
                                            else {


                                                $scope.ProjectForms[i].SectionFields[j].field_DatatypeName = $.trim(selectedData.selectedData.text);
                                                $scope.ProjectForms[i].SectionFields[j].field_DatatypeID = selectedData.selectedData.value;
                                                $scope.ProjectForms[i].SectionFields[j].field_MappedtoExcel = ismappedtoexcel,

                                                    $scope.ProjectForms[i].SectionFields[j].field_FieldLabel = selectedFieldText,
                                                    $scope.ProjectForms[i].SectionFields[j].field_DefaultValue = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_DecimalPositions = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_HintText = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_FieldFilterkey = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_EnforceMinMax = false,
                                                    $scope.ProjectForms[i].SectionFields[j].field_HideFieldLabel = false,
                                                    $scope.ProjectForms[i].SectionFields[j].field_MaxLengthValue = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_MinLengthValue = "",
                                                    $scope.ProjectForms[i].SectionFields[j].field_IsRequired = true,
                                                    $scope.ProjectForms[i].SectionFields[j].field_IsEnabled = false,
                                                    $scope.ProjectForms[i].SectionFields[j].field_CaptureGeolocation = false,
                                                    $scope.ProjectForms[i].SectionFields[j].field_CaptureTimestamp = false,
                                                    $scope.ProjectForms[i].SectionFields[j].field_HasAlert = "NONE",
                                                    $scope.ProjectForms[i].SectionFields[j].field_AllowNotApplicable = false

                                                //AddedBy AniketJ on 03-Mar-2016
                                                $scope.ProjectForms[i].SectionFields[j].field_FieldCalculation = "";
                                                $scope.ProjectForms[i].SectionFields[j].field_AllowRepeat = false;

                                                //AddedBy AniketJ on 15-Feb-2018
                                                $scope.ProjectForms[i].SectionFields[j].field_SynchFlag = false;

                                                if (selectedData.selectedData.text == "Yes/No")//N/A
                                                {
                                                    $scope.ProjectForms[i].SectionFields[j].field_DefaultValue = "No";
                                                    $scope.ProjectForms[i].SectionFields[j].field_IsRequired = true;

                                                    $scope.ProjectForms[i].SectionFields[j].field_SynchFlag = false;
                                                }
                                                else if (selectedData.selectedData.text == "Video")//video
                                                {
                                                    $scope.ProjectForms[i].SectionFields[j].field_VideoQuality = "Low";
                                                }
                                                else if (selectedData.selectedData.text == "SinglePhoto")//single photo
                                                {
                                                    $scope.ProjectForms[i].SectionFields[j].field_PhotoQuality = "JpgLow";
                                                }
                                            }
                                            $scope.field = $scope.ProjectForms[i].SectionFields[j]

                                        });

                                        //#region Delete dropdown optionlist
                                        //for (var i = 0; i < $scope.AllOptionList.length; i++) {
                                        //    if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == selectedFieldText) {

                                        //        $scope.AllOptionList.splice(i, 1);
                                        //    }
                                        //  }

                                        //#endregion

                                        return false;
                                    }
                                }

                            };
                        };

                        if (newindex == -1) {



                            $scope.$apply(function () {
                                $scope.field = {
                                    "field_MappedtoExcel": ismappedtoexcel,
                                    "field_ProjectID": $("#ddlProject").val(),
                                    "field_SectionID": selectedsection,
                                    "field_FieldID": selectedFieldID,
                                    "field_FieldName": selectedFieldText,
                                    "field_FieldOrder": 1,
                                    "field_DatatypeID": selectedData.selectedData.value,
                                    "field_DatatypeName": selectedData.selectedData.text,

                                    "field_FieldLabel": selectedFieldText,
                                    "field_DefaultValue": "",
                                    "field_DecimalPositions": "",
                                    "field_HintText": "",
                                    "field_FieldFilterkey": "",
                                    "field_EnforceMinMax": false,
                                    "field_HideFieldLabel": false,
                                    "field_AllowMultiSelection": false,
                                    "field_MaxLengthValue": "",
                                    "field_MinLengthValue": "",
                                    "field_IsRequired": false,
                                    "field_IsEnabled": true,
                                    "field_CaptureGeolocation": false,
                                    "field_CaptureTimestamp": false,
                                    "field_HasAlert": "NONE",

                                    "field_PhotoLibraryEnabled": false,
                                    "field_AllowAnnotation": false,
                                    "field_CameraEnabled": false,
                                    "field_GPSTagging": false,
                                    "field_ExcludeonSync": false,
                                    "field_PhotoQuality": "",
                                    "field_MaximumHeight": "",
                                    "field_MaximumWidth": "",
                                    "field_AcknowlegementText": "",
                                    "field_AcknowlegementButtonText": "",
                                    "field_AcknowlegementClickedButtonText": "",
                                    "field_PhotoLibraryEnabled": false,


                                    "field_MaxrecordabletimeInSeconds": "",
                                    "field_VideoQuality": "",

                                    "field_RatingMax": "3",
                                    "field_AllowNotApplicable": false,
                                    "field_ShowMultiselect": false,
                                    "field_ShowRatingLabels": false,

                                    //AddedBy AniketJ on 03-Mar-2016
                                    "field_FieldCalculation": "",
                                    "field_AllowRepeat": false,


                                    //AddedBy AniketJ on 15-Feb-2018
                                    "field_SynchFlag": false,
                                }

                                if (selectedData.selectedData.text == "Yes/No")//N/A
                                {
                                    $scope.field.field_DefaultValue = "No";
                                    $scope.field.field_IsRequired = true;

                                    $scope.field.field_SynchFlag = false;
                                }
                                else if (selectedData.selectedData.text == "Video")//video
                                {
                                    $scope.field.field_VideoQuality = "Low";
                                }
                                else if (selectedData.selectedData.text == "SinglePhoto" || selectedData.selectedData.text == "MultiPhoto")//single photo
                                {
                                    $scope.field.field_PhotoQuality = "JpgLow";
                                }

                                for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                                    if ($scope.ProjectForms[i].SectionID == selectedsection) {
                                        $scope.ProjectForms[i].SectionFields.push($scope.field)
                                    }
                                }
                            });
                        }

                        //#region Section order
                        var sectionid = selectedsection;

                        var sectionList = $("#sortable2").children("li[name='" + sectionid + "']")
                        var sectionorder = 1;
                        for (var i = 0; i < sectionList.length; i++) {
                            var CurrentFieldID = sectionList[i].id;
                            var CurrentFieldName = sectionList[i].innerHTML;
                            for (var j = $scope.ProjectForms.length - 1; j >= 0; j--) {
                                for (var k = $scope.ProjectForms[j].SectionFields.length - 1; k >= 0; k--) {
                                    if ($scope.ProjectForms[j].SectionFields[k].field_SectionID == sectionid && $scope.ProjectForms[j].SectionFields[k].field_FieldName == CurrentFieldName) {
                                        $scope.$apply(function () {
                                            $scope.ProjectForms[j].SectionFields[k].field_FieldOrder = sectionorder;
                                            sectionorder++;
                                        });
                                    }
                                }
                            }
                        }
                        //#endregion

                        if (recordView == true) {
                            setTimeout(disableallelements, 2000)

                        }

                        debugger;
                        if (_selectedFieldText == "WORK COMPLETED") {
                            $("#divfield_SynchFlag").show();
                        }
                        else {
                            $("#divfield_SynchFlag").hide();
                        }
                    }
                });

                $("#myDropdown").attr("name", selectedFieldID)

                debugger;
                if (_selectedFieldText == "WORK COMPLETED") {
                    $("#divfield_SynchFlag").show();
                }
                else {
                    $("#divfield_SynchFlag").hide();
                }
            }

            $(document).unbind("click").on('click', '#sortable2>li', function () {

                DestinationFieldClick(this);


            });

            //#region Options set for list
            var AllFieldOptionList = {};

            $scope.AddOptionsinList = function () {

                $("#ListOptionsTable").sortable({
                    containment: "parent",
                    tolerance: "pointer",
                    stop: function (event, ui) {


                        var OptionsListnew = angular.element("#ListOptionsTable").children("tr");
                        var sectionorder = 1;
                        for (var i = 0; i < OptionsListnew.length; i++) {
                            var optionName = $(OptionsListnew[i]).find("td[name='nameoptionName']").text()
                            var optionValue = $(OptionsListnew[i]).find("td[name='nameoptionValue']").text()
                            var optionFilterkey = $(OptionsListnew[i]).find("td[name='nameoptionFilterkey']").text()
                            var optionid = $(OptionsListnew[i]).find("td[name='nameuniqueoptionid']").attr("id");

                            for (var j = 0; j < $scope.AllOptionList.length; j++) {
                                if ($scope.AllOptionList[j].SectionID == _selectedSection && $scope.AllOptionList[j].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[j].FieldName == _selectedFieldText) {
                                    for (var k = 0; k < $scope.AllOptionList[j].OptionList.length; k++) {
                                        //if ($scope.AllOptionList[j].OptionList[k].FieldText == optionName && $scope.AllOptionList[j].OptionList[k].FieldValue == optionValue && $scope.AllOptionList[j].OptionList[k].FieldFilterkey == optionFilterkey) {
                                        if ($scope.AllOptionList[j].OptionList[k].optionID == optionid) {
                                            $scope.$apply(function () {
                                                $scope.AllOptionList[j].OptionList[k].OrderNo = i + 1;
                                                $(OptionsListnew[i]).find("td[name='nameoptionSerialNo']").text((i + 1))
                                            })

                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                var newName = $("#txtnewName").val();
                var newValue = $("#txtnewValue").val();
                var newFilterkey = $("#txtnewFilterkey").val();

                var options = {
                    FieldName: _selectedFieldText,
                    optionID: uniqueOptionID,
                    OrderNo: "",
                    FieldText: newName,
                    FieldValue: newValue,
                    FieldFilterkey: newFilterkey
                };

                var elementAddEdit = "Add";
                var orderuniqueno = $("#hndOrderNo").val();

                for (var i = 0; i < $scope.AllOptionList.length; i++) {
                    if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {
                        for (var j = 0; j < $scope.AllOptionList[i].OptionList.length; j++) {
                            if (orderuniqueno != $scope.AllOptionList[i].OptionList[j].OrderNo) {
                                if ($scope.AllOptionList[i].OptionList[j].FieldText == newName) {

                                    var $toast = toastr["error"](newName + " name already exist ", "Error Notification");
                                    return false;
                                }

                                if ($scope.AllOptionList[i].OptionList[j].FieldValue == newValue) {

                                    var $toast = toastr["error"](newValue + " value already exist ", "Error Notification");
                                    return false;
                                }
                            }

                        }

                    }
                }


                if (orderuniqueno != "") {
                    for (var i = 0; i < $scope.AllOptionList.length; i++) {
                        if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {
                            for (var j = 0; j < $scope.AllOptionList[i].OptionList.length; j++) {

                                if ($scope.AllOptionList[i].OptionList[j].OrderNo == orderuniqueno) {
                                    //if ($scope.AllOptionList[i].OptionList[j].FieldText == FieldText && $scope.AllOptionList[i].OptionList[j].FieldValue==) {
                                    $scope.AllOptionList[i].OptionList[j].FieldText = newName;
                                    $scope.AllOptionList[i].OptionList[j].FieldValue = newValue;
                                    $scope.AllOptionList[i].OptionList[j].FieldFilterkey = newFilterkey;

                                    $("#btnOptionSave").val("Add");
                                    elementAddEdit = "Edit";

                                    CreateOptionList($scope.AllOptionList[i].OptionList)
                                }

                            }

                        }
                    }
                }

                if (elementAddEdit == "Add") {
                    for (var i = 0; i < $scope.AllOptionList.length; i++) {
                        if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {

                            $scope.AllOptionList[i].OptionList.push(options);
                        }
                    }

                    //OptionsList.push(options);

                    var td1 = '<td name="nameoptionSerialNo" style="text-align:center"></td>';
                    var td2 = '<td name="nameoptionName">' + newName + '</td>';
                    var td3 = '<td name="nameoptionValue">' + newValue + '</td>';
                    var td4 = '<td name="nameoptionFilterkey">' + newFilterkey + '</td>';
                    var td5 = '<td><span ng-click="Editcurrentoption($event)" style="cursor:pointer" class="btn btn-success btn-xs"><i class="fa fa-pencil-square-o"></i></span></td>';
                    var td6 = '<td><span ng-click="Deletecurrentoption($event)" style="cursor:pointer" class="btn btn-danger btn-xs"><i class="fa fa-trash"></i></span></td>';
                    var td7 = '<td id="' + uniqueOptionID + '" name="nameuniqueoptionid"></td>';
                    var td8 = '<td id="' + _selectedFieldText + '" name="nameselectedFieldText"></td>';
                    var tr = $("<tr>").append(td7).append(td8).append(td1).append(td2).append(td3).append(td4).append(td5).append(td6);

                    var compiledeHTML = $compile(tr)($scope);

                    $("#ListOptionsTable").append(compiledeHTML);

                    uniqueOptionID++;
                }

                $("#txtnewName").val("");
                $("#txtnewValue").val("");
                $("#txtnewFilterkey").val("");
                $("#hndOrderNo").val("");

                giveserialnotoOptions()
            }

            $scope.CancelOptionsinList = function () {
                $("#txtnewName").val("");
                $("#txtnewValue").val("");
                $("#txtnewFilterkey").val("");
                $("#hndOrderNo").val("");
                $("#btnOptionSave").val("Add");
            }

            function CreateOptionList(data) {



                data.sort(compareOptionList);


                $("#ListOptionsTable").empty();
                for (var i = 0; i < data.length; i++) {

                    var td1 = '<td name="nameoptionSerialNo" style="text-align:center"></td>';
                    var td2 = '<td name=nameoptionName>' + data[i].FieldText + '</td>';
                    var td3 = '<td name=nameoptionValue>' + data[i].FieldValue + '</td>';
                    var td4 = '<td name=nameoptionFilterkey>' + data[i].FieldFilterkey + '</td>';
                    var td5 = '<td><span ng-click="Editcurrentoption($event)" style="cursor:pointer" class="btn btn-success btn-xs"><i class="fa fa-pencil-square-o"></i></span></td>';
                    var td6 = '<td><span ng-click="Deletecurrentoption($event)" style="cursor:pointer" class="btn btn-danger btn-xs"><i class="fa fa-trash"></i></span></td>';
                    var td7 = '<td id="' + data[i].optionID + '" name="nameuniqueoptionid"></td>';
                    var td8 = '<td id="' + _selectedFieldText + '" name="nameselectedFieldText"></td>';
                    var tr = $("<tr>").append(td7).append(td8).append(td1).append(td2).append(td3).append(td4).append(td5).append(td6);

                    var compiledeHTML = $compile(tr)($scope);

                    $("#ListOptionsTable").append(compiledeHTML);
                    uniqueOptionID++;
                }

                giveserialnotoOptions()


                $("#ListOptionsTable").sortable({
                    containment: "parent",
                    tolerance: "pointer",
                    stop: function (event, ui) {


                        var OptionsListnew = angular.element("#ListOptionsTable").children("tr");
                        var sectionorder = 1;
                        for (var i = 0; i < OptionsListnew.length; i++) {
                            var optionName = $(OptionsListnew[i]).find("td[name='nameoptionName']").text()
                            var optionValue = $(OptionsListnew[i]).find("td[name='nameoptionValue']").text()
                            var optionFilterkey = $(OptionsListnew[i]).find("td[name='nameoptionFilterkey']").text()
                            var optionid = $(OptionsListnew[i]).find("td[name='nameuniqueoptionid']").attr("id");

                            for (var j = 0; j < $scope.AllOptionList.length; j++) {
                                if ($scope.AllOptionList[j].SectionID == _selectedSection && $scope.AllOptionList[j].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[j].FieldName == _selectedFieldText) {
                                    for (var k = 0; k < $scope.AllOptionList[j].OptionList.length; k++) {
                                        //if ($scope.AllOptionList[j].OptionList[k].FieldText == optionName && $scope.AllOptionList[j].OptionList[k].FieldValue == optionValue && $scope.AllOptionList[j].OptionList[k].FieldFilterkey == optionFilterkey) {
                                        if ($scope.AllOptionList[j].OptionList[k].optionID == optionid) {
                                            $scope.$apply(function () {
                                                $scope.AllOptionList[j].OptionList[k].OrderNo = i + 1;
                                                $(OptionsListnew[i]).find("td[name='nameoptionSerialNo']").text((i + 1))
                                            })

                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }

            $scope.Deletecurrentoption = function ($event) {

                ngDialog.openConfirm({
                    template: '\
                <p>Are you sure you want to delete selected option?</p>\
                <div class="ngdialog-buttons">\
                    <button type="button" class="ngdialog-button ngdialog-button-secondary" ng-click="closeThisDialog(0)">No</button>\
                    <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Yes</button>\
                </div>',
                    plain: true,
                    showClose: false,
                    closeByDocument: false,
                    closeByEscape: false
                }).then(function (confirm) {

                    var name = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionName']").text();
                    var value = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionValue']").text();
                    var Filterkey = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionFilterkey']").text();
                    var optionid = $($event.currentTarget.parentElement.parentElement).find("td[name='nameuniqueoptionid']").attr("id");
                    var fieldname = $($event.currentTarget.parentElement.parentElement).find("td[name='nameselectedFieldText']").attr("id");
                    $($event.currentTarget.parentElement.parentElement).first().remove()

                    for (var i = 0; i < $scope.AllOptionList.length; i++) {
                        if ($scope.AllOptionList[i].SectionID == _selectedSection && $scope.AllOptionList[i].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[i].FieldName == _selectedFieldText) {
                            for (var j = 0; j < $scope.AllOptionList[i].OptionList.length; j++) {
                                if ($scope.AllOptionList[i].OptionList[j].FieldText == name && $scope.AllOptionList[i].OptionList[j].FieldValue == value && $scope.AllOptionList[i].OptionList[j].FieldName == fieldname) {
                                    //if ($scope.AllOptionList[i].OptionList[j].optionID == optionid && $scope.AllOptionList[i].OptionList[j].FieldName == fieldname) {
                                    $scope.AllOptionList[i].OptionList.splice(j, 1);
                                }
                            }
                        }
                    }



                    giveserialnotoOptions()

                }, function (reject) {

                    return false;

                });


            }

            $scope.Editcurrentoption = function ($event) {
                var name = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionName']").text();
                var value = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionValue']").text();
                var Filterkey = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionFilterkey']").text();
                var orderno = $($event.currentTarget.parentElement.parentElement).find("td[name='nameoptionSerialNo']").text();
                var optionid = $($event.currentTarget.parentElement.parentElement).find("td[name='nameuniqueoptionid']").attr("id");
                var fieldname = $($event.currentTarget.parentElement.parentElement).find("td[name='nameselectedFieldText']").attr("id");

                $("#txtnewName").val(name);
                $("#txtnewValue").val(value);
                $("#txtnewFilterkey").val(Filterkey);
                $("#hndOrderNo").val(orderno);

                $("#btnOptionSave").val("Update");

            }

            $scope.AddOptionsindisable = function () {
                var newName = $("#txtnewName").val();
                var newValue = $("#txtnewValue").val();
                if (newName != "" && newValue != "") {
                    return false;
                } else {
                    return true;
                }
            }

            function giveserialnotoOptions() {

                //#region Section order

                var OptionsListnew = angular.element("#ListOptionsTable").children("tr");
                var sectionorder = 1;
                for (var i = 0; i < OptionsListnew.length; i++) {
                    var optionName = $(OptionsListnew[i]).find("td[name='nameoptionName']").text();
                    var optionValue = $(OptionsListnew[i]).find("td[name='nameoptionValue']").text();
                    var optionFilterkey = $(OptionsListnew[i]).find("td[name='nameoptionFilterkey']").text();
                    var optionid = $(OptionsListnew[i]).find("td[name='nameuniqueoptionid']").attr("id");
                    var fieldname = $(OptionsListnew[i]).find("td[name='nameselectedFieldText']").attr("id");



                    for (var j = 0; j < $scope.AllOptionList.length; j++) {
                        if ($scope.AllOptionList[j].SectionID == _selectedSection && $scope.AllOptionList[j].SectionName == _CurrentSelectedSectionName && $scope.AllOptionList[j].FieldName == _selectedFieldText) {
                            for (var k = 0; k < $scope.AllOptionList[j].OptionList.length; k++) {
                                //if ($scope.AllOptionList[j].OptionList[k].FieldText == optionName && $scope.AllOptionList[j].OptionList[k].FieldValue == optionValue && $scope.AllOptionList[j].OptionList[k].FieldFilterkey == optionFilterkey) {
                                if ($scope.AllOptionList[j].OptionList[k].optionID == optionid && $scope.AllOptionList[j].OptionList[k].FieldName == fieldname) {
                                    $scope.AllOptionList[j].OptionList[k].OrderNo = i + 1;
                                    $(OptionsListnew[i]).find("td[name='nameoptionSerialNo']").text((i + 1))
                                }
                            }
                        }
                    }


                    //for (var j = OptionsList.length - 1; j >= 0; j--) {
                    //    if (OptionsList[j].Name == optionName && OptionsList[j].Value == optionValue && OptionsList[j].Filterkey == optionFilterkey) {
                    //        OptionsList[j].OrderNo = i + 1;
                    //        $(OptionsListnew[i]).find("td[name='nameoptionSerialNo']").text((i + 1))
                    //    }
                    //}
                }

                //#endregion

            }
            //#endregion

            $scope.EnforceMinMax = function (data) {
                if (data == false) {
                    $scope.field.field_MaxLengthValue = "";
                    $scope.field.field_MinLengthValue = "";
                }
            }
            var formid;
            $scope.FormSave = function () {
                debugger;



                var data1 = $("#txtfieldtitle").val();
                if (data1 == "" && data1 != undefined) {
                    return false
                }
                if ($("#ddlProject").val() == "") {
                    var $toast = toastr["error"]("Please Select Project", "Error Notification");
                    return false
                }
                if ($("#ddlFormType").val() == "") {
                    var $toast = toastr["error"]("Please Select Form Type", "Error Notification");
                    return false
                }

                if ($('#FormManagementForm').valid()) {

                    // waitingDialog.show('Loading Please Wait...');


                    //#region sourcefieldcheck
                    var recordFound = false;
                    var workcompletednotsaved = false;
                    $scope.DatatypenotselectedFieldList = [];

                    for (var i = 0; i < $("#sortable2").children().length; i++) {
                        var datasaved = $($("#sortable2").children()[i]).hasClass("dataSaved")
                        if (datasaved == false) {
                            recordFound = true;
                            var fieldname = $($("#sortable2").children()[i]).text()
                            $scope.DatatypenotselectedFieldList.push(fieldname)

                        }
                    }

                    for (var i = 0; i < $("#sortable1").children().length; i++) {
                        var sourcefields = $($("#sortable1").children()[i]).text()
                        if (sourcefields == "WORK COMPLETED") {
                            workcompletednotsaved = true;
                        }
                    }

                    if (recordFound == true) {


                        // var compiledeHTML = $compile(tbody)($scope);
                        ngDialog.openConfirm({
                            template: $rootScope.projectPath + "Angular/views/Form/FieldDTempty.html",
                            scope: $scope,
                            // plain: true,
                            // showClose: false,
                            closeByDocument: false,
                            closeByEscape: false
                        }).then(function (confirm) {
                            return false;

                        }, function (reject) {

                            return false;

                        });
                        return false

                    }
                    else if (workcompletednotsaved == true) {
                        ngDialog.openConfirm({
                            template: '\
                                    <h4><p>Work completed field is not selected</p></h4>\
                                    <div class="ngdialog-buttons">\
                                        <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Ok</button>\
                                    </div>',
                            plain: true,
                            showClose: false,
                            closeByDocument: false,
                            closeByEscape: false
                        }).then(function (confirm) {
                            return false;

                        }, function (reject) {

                            return false;

                        });
                        return false
                    }
                    //#endregion
                    GetOptionField();

                    var optionlist = $scope.AllOptionList

                    for (var o = 0; o < optionlist.length; o++) {
                        var optionlength = optionlist[o].OptionList.length;
                        if (optionlength == 0) {

                            var fieldname = optionlist[o].FieldName;
                            var sectionName = optionlist[o].SectionName;

                            if (OptionFieldList.length > 0) {

                                var isoptionfield = OptionFieldList.indexOf(fieldname);

                                if (isoptionfield != -1) {


                                    ngDialog.openConfirm({
                                        template: '\
                                    <h4><p>Please add options in following list datatype field.</p></h4>\
                                    <h5><p>Field Name- ' + fieldname + '</p></h5>\
                                    <div class="ngdialog-buttons">\
                                        <button type="button" class="ngdialog-button ngdialog-button-primary" ng-click="confirm(1)">Ok</button>\
                                    </div>',
                                        plain: true,
                                        showClose: false,
                                        closeByDocument: false,
                                        closeByEscape: false
                                    }).then(function (confirm) {
                                        return false;

                                    }, function (reject) {

                                        return false;

                                    });

                                    return false;
                                }
                            }
                        }

                    }



                    var FormMasterFields = {
                        "FormId": $("#hdnFormID").val(),
                        "ProjectId": $("#ddlProject").val(),
                        "FormName": $('#txtNewFormName').val(),
                        "FromDate": $('#txtFromDate').val(),
                        "ToDate": $('#txtToDate').val(),
                        "FormTypeID": $('#ddlFormType').val()
                    }

                    var urlsaveFormFields = $('#urlsaveFormFields').val();
                    $.ajax({
                        url: urlsaveFormFields,
                        async: true,
                        type: "POST",
                        beforeSend: function () {
                            waitingDialog.show('Loading Please Wait...');
                        },
                        complete: function () {
                            waitingDialog.hide();
                        },
                        // dataType: "json",
                        //data: JSON.stringify({ formdata: $scope.FinalFormField }),
                        data: { formdata: $scope.ProjectForms, masterFields: FormMasterFields, optionList: $scope.AllOptionList },
                        success: function (data) {

                            if (data.success == true) {
                                $("#hdnFormID").val(data.formid);
                                $("#btnSave").text("Update");
                                $("#btnReset").hide();

                                var $toast = toastr["success"](data.returnMessage, "Success Notification");
                                if (recordaddEdit == "Add" || recordaddEdit == "Clone") {
                                    //var indexpath = $rootScope.projectPath + "FormManagement";
                                    //$location.path(indexpath);
                                }

                            } else {

                                var $toast = toastr["error"](data.returnMessage, "Error Notification");
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
            }

         
            function GetOptionField() {
                debugger;
                OptionFieldList = [];
                for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                    for (var j = $scope.ProjectForms[i].SectionFields.length - 1; j >= 0; j--) {

                        if ($scope.ProjectForms[i].SectionFields[j].field_MappedtoExcel == '0' && $scope.ProjectForms[i].SectionFields[j].field_DatatypeName == "List") {
                            var fields = {};// new object

                            fields.fieldName = $scope.ProjectForms[i].SectionFields[j].field_FieldName;
                            fields.ProjectID = $scope.ProjectForms[i].SectionFields[j].field_ProjectID;
                            fields.FieldID = $scope.ProjectForms[i].SectionFields[j].field_FieldID;
                            fields.DatatypeID = $scope.ProjectForms[i].SectionFields[j].field_DatatypeID;
                            fields.DatatypeName = $scope.ProjectForms[i].SectionFields[j].field_DatatypeName;

                            OptionFieldList.push(fields.fieldName);
                        }
                    }
                };


            }


            $scope.FormReset = function () {

                $('#ddlProject').find('option:first').attr('selected', 'selected');

                ResetFunction()
            }



            $scope.FormCancel = function () {
                var indexpath = $rootScope.projectPath + "FormManagement";
                $location.path(indexpath);
            }

            $scope.DisableFinalSaveButoon = function () {



                if ($scope.ProjectForms != undefined && $scope.ProjectForms.length > 0) {

                    if ($scope.ProjectForms[0] != undefined) {
                        for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                            if ($scope.ProjectForms[i].SectionFields.length > 0) {
                                return false;
                            }
                            else {
                                return true;
                            }

                        }

                        //if ($scope.ProjectForms[0].SectionFields.length > 0) {
                        //    return false;
                        //} else {
                        //    return true;
                        //}

                    }
                    else {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }

            $scope.DisableSectionEditDelete = function () {

                if ($scope.ProjectForms != undefined && $scope.ProjectForms.length > 1) {

                    return false;
                }
                else {

                    // $("#currentfieldsection").text("sections");

                    // $scope.FieldForCurrentSection = "Current ";
                    return true;
                }
            }


            //AddedBy AniketJ on 02-Mar-2017
            function getNumericFields() {

                var NumericFieldList = [];
                for (var i = $scope.ProjectForms.length - 1; i >= 0; i--) {

                    for (var j = $scope.ProjectForms[i].SectionFields.length - 1; j >= 0; j--) {

                        if ($scope.ProjectForms[i].SectionFields[j].field_DatatypeID == '4' || $scope.ProjectForms[i].SectionFields[j].field_DatatypeName == "Numeric") {
                            var fields = {};// new object

                            fields.fieldName = $scope.ProjectForms[i].SectionFields[j].field_FieldName;
                            fields.ProjectID = $scope.ProjectForms[i].SectionFields[j].field_ProjectID;
                            fields.FieldID = $scope.ProjectForms[i].SectionFields[j].field_FieldID;
                            fields.DatatypeID = $scope.ProjectForms[i].SectionFields[j].field_DatatypeID;
                            fields.DatatypeName = $scope.ProjectForms[i].SectionFields[j].field_DatatypeName;

                            NumericFieldList.push(fields);
                        }
                    }
                };


                if (NumericFieldList.length > 0) {
                    $("#ulCalculationFields").empty();

                    for (var i = 0; i < NumericFieldList.length; i++) {
                        var sourceli = "<li lang='' id='" + NumericFieldList[i].FieldID + "' style='z-index: 999999;' class='placeholder ui-state-default'>" + NumericFieldList[i].fieldName + "</li>"
                        $("#ulCalculationFields").append(sourceli)
                    }
                    myfunction()
                }
            }
            function myfunction() {

                var options = {
                    accept: "li.placeholder",
                    drop: function (ev, ui) {
                        insertAtCaret($("textarea#areaCalculationFieldsFormula").get(0), ui.draggable.eq(0).text());
                    }
                };

                $("li.placeholder").draggable({
                    helper: 'clone',
                    start: function (event, ui) {

                        var txta = $("textarea#areaCalculationFieldsFormula");
                        $("div#dropable-area").css({
                            position: "absolute",

                            top: txta.position().top,
                            left: txta.position().left,
                            width: txta.width(),
                            height: txta.height()
                        }).droppable(options).show();
                    },
                    stop: function (event, ui) {

                        $("div#dropable-area").droppable('destroy').hide();
                    }
                });
            }
            function insertAtCaret(area, text1) {

                $scope.$apply(function () {

                    var text = "$" + text1.split(' ').join('_') + "$"
                    var scrollPos = area.scrollTop;
                    var strPos = 0;
                    var br = ((area.selectionStart || area.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
                    if (br == "ie") {
                        area.focus();
                        var range = document.selection.createRange();
                        range.moveStart('character', -(area.value.length));
                        strPos = range.text.length;
                    } else if (br == "ff")
                        strPos = area.selectionStart;
                    var front = (area.value).substring(0, strPos);
                    var back = (area.value).substring(strPos, area.value.length);
                    area.value = front + text + back;
                    $scope.field.field_FieldCalculation = area.value;
                    strPos = strPos + text.length;
                    if (br == "ie") {
                        area.focus();
                        var range = document.selection.createRange();
                        range.moveStart('character', -(area.value.length));
                        range.moveStart('character', strPos);
                        range.moveEnd('character', 0);
                        range.select();
                    } else if (br == "ff") {
                        area.selectionStart = strPos;
                        area.selectionEnd = strPos;
                        area.focus();
                    }
                    area.scrollTop = scrollPos;
                });
            }
        }
        catch (e) {

            console.log(e)
            alert(e.name + ': ' + e.message);
        }

    }]);

